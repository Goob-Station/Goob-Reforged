'use strict';

const CONFIG = {
  claDocument: 'https://github.com/Goob-Station/Goob-Station/blob/master/CLA.md',

  signaturesBranch: 'cla-signatures',
  signaturesPath: 'signatures.json',

  statusContext: 'license/cla',

  signPhrase: 'i have read the cla document and i hereby sign the cla',
  revokePhrase: 'i revoke my cla signature',

  allowlist: [],

  marker: '<!-- goob-cla-bot -->',

  committer: {
    name: 'goob-cla-bot',
    email: '41898282+github-actions[bot]@users.noreply.github.com',
  },
};

const norm = (s) => (s || '').toLowerCase().replace(/\s+/g, ' ').trim();
const lc = (s) => (s || '').toLowerCase();

async function loadStore(ctx) {
  const { github, owner, repo } = ctx;
  try {
    const res = await github.rest.repos.getContent({
      owner,
      repo,
      path: CONFIG.signaturesPath,
      ref: CONFIG.signaturesBranch,
    });
    const raw = Buffer.from(res.data.content, 'base64').toString('utf8');
    const data = JSON.parse(raw);
    data.signatures ||= [];
    data.pending ||= {};
    return { data, sha: res.data.sha };
  } catch (err) {
    if (err.status === 404) {
      return { data: { version: 1, signatures: [], pending: {} }, sha: null };
    }
    throw err;
  }
}

async function writeStore(ctx, store, message) {
  const { github, owner, repo } = ctx;
  const res = await github.rest.repos.createOrUpdateFileContents({
    owner,
    repo,
    path: CONFIG.signaturesPath,
    branch: CONFIG.signaturesBranch,
    message,
    content: Buffer.from(JSON.stringify(store.data, null, 2) + '\n').toString('base64'),
    sha: store.sha || undefined,
    committer: CONFIG.committer,
    author: CONFIG.committer,
  });
  store.sha = res.data.content.sha;
}

async function mutateStore(ctx, message, mutate) {
  for (let attempt = 0; attempt < 4; attempt++) {
    const store = await loadStore(ctx);
    const changed = await mutate(store.data);
    if (!changed) return store;
    try {
      await writeStore(ctx, store, message);
      return store;
    } catch (err) {
      if (err.status !== 409 && err.status !== 422) throw err;
      await new Promise((r) => setTimeout(r, 500 * (attempt + 1)));
    }
  }
  throw new Error('could not write the signature ledger after 4 attempts');
}

function hasSigned(data, login) {
  return data.signatures.some((s) => lc(s.login) === lc(login));
}

function addSignature(data, user, meta) {
  if (hasSigned(data, user.login)) return false;
  data.signatures.push({
    login: user.login,
    id: user.id,
    signed_at: new Date().toISOString(),
    method: meta.method,
    pull_request: meta.pr,
    evidence: meta.evidence,
  });
  data.signatures.sort((a, b) => lc(a.login).localeCompare(lc(b.login)));
  return true;
}

function removeSignature(data, login) {
  const before = data.signatures.length;
  data.signatures = data.signatures.filter((s) => lc(s.login) !== lc(login));
  return data.signatures.length !== before;
}

function parseCosigners(body) {
  const found = [];
  const lineRe = /^[ \t>]*CLA[-_ ]?Co-?signers?[ \t]*:[ \t]*(.+)$/gim;
  const userRe = /@([A-Za-z0-9](?:[A-Za-z0-9-]{0,37}[A-Za-z0-9])?)\b/g;
  let line;
  while ((line = lineRe.exec(body || '')) !== null) {
    for (const m of line[1].matchAll(userRe)) found.push(m[1]);
  }
  return found;
}

function requiredSigners(pr) {
  const seen = new Map();
  if (pr.user && pr.user.type !== 'Bot' && !lc(pr.user.login).endsWith('[bot]')) {
    seen.set(lc(pr.user.login), pr.user.login);
  }
  for (const login of parseCosigners(pr.body)) seen.set(lc(login), login);
  for (const skip of CONFIG.allowlist) seen.delete(lc(skip));
  return [...seen.values()];
}

function renderComment(data, required, missing) {
  const lines = [CONFIG.marker, '', '## Contributor License Agreement', ''];

  if (required.length === 0) {
    lines.push('No signatures are required for this pull request.');
    return lines.join('\n');
  }

  lines.push(
    `Thanks for contributing to Goob Station. Before this can be merged, everyone`,
    `whose work is included needs to sign the [CLA](${CONFIG.claDocument}).`,
    '',
    '| Contributor | Status |',
    '| --- | --- |'
  );

  for (const login of required) {
    const sig = data.signatures.find((s) => lc(s.login) === lc(login));
    lines.push(
      sig
        ? `| @${login} | Signed ${sig.signed_at.slice(0, 10)} |`
        : `| @${login} | Not signed yet |`
    );
  }

  lines.push('');

  if (missing.length === 0) {
    lines.push('Everyone has signed. This check is green.');
  } else {
    lines.push(
      '### How to sign',
      '',
      'Either react to **this comment** with :+1:, or post a new comment containing:',
      '',
      '```',
      'I have read the CLA Document and I hereby sign the CLA',
      '```',
      '',
      'Commenting is picked up immediately. Reactions are swept every ten minutes,',
      'so give it a moment before worrying.',
      '',
      '### Porting someone else\'s work?',
      '',
      'Add a line like this to the pull request description and they will be added',
      'to the table above:',
      '',
      '```',
      'CLA-Cosigners: @their-username @another-username',
      '```'
    );
  }

  lines.push(
    '',
    '<sub>Signatures are recorded in ' +
      `[\`${CONFIG.signaturesPath}\`](../blob/${CONFIG.signaturesBranch}/${CONFIG.signaturesPath}) ` +
      'on the `' + CONFIG.signaturesBranch + '` branch. ' +
      'Reply with `I revoke my CLA signature` to withdraw.</sub>'
  );

  return lines.join('\n');
}

async function upsertComment(ctx, prNumber, body) {
  const { github, owner, repo } = ctx;
  const comments = await github.paginate(github.rest.issues.listComments, {
    owner,
    repo,
    issue_number: prNumber,
    per_page: 100,
  });
  const existing = comments.find((c) => (c.body || '').includes(CONFIG.marker));

  if (existing) {
    if (norm(existing.body) !== norm(body)) {
      await github.rest.issues.updateComment({
        owner,
        repo,
        comment_id: existing.id,
        body,
      });
    }
    return existing.id;
  }

  const created = await github.rest.issues.createComment({
    owner,
    repo,
    issue_number: prNumber,
    body,
  });
  return created.data.id;
}

async function evaluate(ctx, prNumber) {
  const { github, owner, repo, core } = ctx;

  const { data: pr } = await github.rest.pulls.get({
    owner,
    repo,
    pull_number: prNumber,
  });

  const store = await loadStore(ctx);
  const required = requiredSigners(pr);
  const missing = required.filter((login) => !hasSigned(store.data, login));

  const commentId = await upsertComment(
    ctx,
    prNumber,
    renderComment(store.data, required, missing)
  );

  await github.rest.repos.createCommitStatus({
    owner,
    repo,
    sha: pr.head.sha,
    state: missing.length === 0 ? 'success' : 'pending',
    context: CONFIG.statusContext,
    target_url: CONFIG.claDocument,
    description:
      missing.length === 0
        ? 'All contributors have signed the CLA'
        : `Waiting on ${missing.length} signature(s)`,
  });

  await mutateStore(ctx, `cla: update pending index for #${prNumber}`, (data) => {
    const key = String(prNumber);
    const shouldWatch = missing.length > 0 && pr.state === 'open';
    if (shouldWatch) {
      const entry = { comment_id: commentId, updated_at: new Date().toISOString() };
      if (JSON.stringify(data.pending[key]) === JSON.stringify(entry)) return false;
      data.pending[key] = entry;
      return true;
    }
    if (!(key in data.pending)) return false;
    delete data.pending[key];
    return true;
  });

  core.info(
    `PR #${prNumber}: ${required.length} required, ${missing.length} missing` +
      (missing.length ? ` (${missing.join(', ')})` : '')
  );
}

async function handleComment(ctx) {
  const { context, core } = ctx;
  const payload = context.payload;

  if (!payload.issue || !payload.issue.pull_request) return;

  const user = payload.comment.user;
  if (user.type === 'Bot' || lc(user.login).endsWith('[bot]')) return;
  if ((payload.comment.body || '').includes(CONFIG.marker)) return;

  const body = norm(payload.comment.body);
  const signing = body.includes(CONFIG.signPhrase);
  const revoking = body.includes(CONFIG.revokePhrase);
  if (!signing && !revoking) return;

  const prNumber = payload.issue.number;

  if (signing) {
    await mutateStore(ctx, `cla: ${user.login} signed via #${prNumber}`, (data) =>
      addSignature(data, user, {
        method: 'comment',
        pr: prNumber,
        evidence: payload.comment.html_url,
      })
    );
    core.info(`${user.login} signed by comment`);
  } else {
    await mutateStore(ctx, `cla: ${user.login} revoked via #${prNumber}`, (data) =>
      removeSignature(data, user.login)
    );
    core.info(`${user.login} revoked`);
  }

  await evaluate(ctx, prNumber);
}

async function sweepReactions(ctx) {
  const { github, owner, repo, core } = ctx;
  const store = await loadStore(ctx);
  const pending = Object.entries(store.data.pending || {});

  if (pending.length === 0) {
    core.info('no pending pull requests');
    return;
  }

  const touched = new Set();

  for (const [prNumber, entry] of pending) {
    let reactions;
    try {
      reactions = await github.paginate(github.rest.reactions.listForIssueComment, {
        owner,
        repo,
        comment_id: entry.comment_id,
        per_page: 100,
      });
    } catch (err) {
      if (err.status === 404) {
        touched.add(Number(prNumber));
        continue;
      }
      throw err;
    }

    for (const reaction of reactions) {
      if (reaction.content !== '+1') continue;
      const user = reaction.user;
      if (!user || user.type === 'Bot') continue;

      const added = await mutateStore(
        ctx,
        `cla: ${user.login} signed by reaction on #${prNumber}`,
        (data) =>
          addSignature(data, user, {
            method: 'reaction',
            pr: Number(prNumber),
            evidence: `https://github.com/${owner}/${repo}/pull/${prNumber}#issuecomment-${entry.comment_id}`,
          })
      );
      if (added) touched.add(Number(prNumber));
    }
  
  const toRefresh = new Set([...touched, ...pending.map(([n]) => Number(n))]);
  for (const prNumber of toRefresh) {
    await evaluate(ctx, prNumber);
  }
}

module.exports = async ({ github, context, core }) => {
  const ctx = { github, context, core, owner: context.repo.owner, repo: context.repo.repo };

  switch (context.eventName) {
    case 'pull_request_target':
      await evaluate(ctx, context.payload.pull_request.number);
      break;
    case 'issue_comment':
      await handleComment(ctx);
      break;
    case 'schedule':
    case 'workflow_dispatch':
      await sweepReactions(ctx);
      break;
    default:
      core.info(`nothing to do for ${context.eventName}`);
  }
};
