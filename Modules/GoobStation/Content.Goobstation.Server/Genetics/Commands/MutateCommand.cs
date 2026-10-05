// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Genetics.Components;
using Content.Goobstation.Shared.Genetics.Prototypes;
using Content.Goobstation.Shared.Genetics.Systems;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Errors;

namespace Content.Server.Genetics.Command;

[ToolshedCommand, AdminCommand(AdminFlags.Debug)]
public sealed partial class MutateCommand : ToolshedCommand
{
    [Dependency] private IPrototypeManager _proto = default!;
    private MutationSystem? _mutation;

    [CommandImplementation("ensure")]
    public IEnumerable<EntityUid> Ensure([PipedArgument] IEnumerable<EntityUid> input, ProtoId<MutationPrototype> protoId)
    {
        _mutation ??= GetSys<MutationSystem>();

        foreach (var i in input)
        {
            _mutation.EnsureMutation(
                (i, Comp<MutableComponent>(i)),
                _proto.Index(protoId),
                predicted: false
            );
            yield return i;
        }
    }

    [CommandImplementation("ensure")]
    public void Ensure(IInvocationContext ctx, ProtoId<MutationPrototype> proto)
    {
        _mutation ??= GetSys<MutationSystem>();

        if (ExecutingEntity(ctx) is not { } ent)
        {
            if (ctx.Session is {} session)
                ctx.ReportError(new SessionHasNoEntityError(session));
            else
                ctx.ReportError(new NotForServerConsoleError());
        }
        else
            _mutation.EnsureMutation(
                (ent, Comp<MutableComponent>(ent)),
                _proto.Index(proto),
                predicted: false
            );
    }

    [CommandImplementation("remove")]
    public IEnumerable<EntityUid> Remove([PipedArgument] IEnumerable<EntityUid> input, ProtoId<MutationPrototype> protoId)
    {
        _mutation ??= GetSys<MutationSystem>();

        foreach (var i in input)
        {
            _mutation.RemoveMutation(
                (i, Comp<MutableComponent>(i)),
                _proto.Index(protoId),
                predicted: false
            );
            yield return i;
        }
    }

    [CommandImplementation("remove")]
    public void Remove(IInvocationContext ctx, ProtoId<MutationPrototype> proto)
    {
        _mutation ??= GetSys<MutationSystem>();

        if (ExecutingEntity(ctx) is not { } ent)
        {
            if (ctx.Session is {} session)
                ctx.ReportError(new SessionHasNoEntityError(session));
            else
                ctx.ReportError(new NotForServerConsoleError());
        }
        else
            _mutation.RemoveMutation(
                (ent, Comp<MutableComponent>(ent)),
                _proto.Index(proto),
                predicted: false
            );
    }
}
