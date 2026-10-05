// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Genetics.Prototypes;
using Content.Goobstation.Shared.Genetics.Types;
using Content.Shared.GameTicking;
using Content.Shared.Random.Helpers;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Goobstation.Shared.Genetics.Systems;

public sealed partial class MutationSequenceSystem : EntitySystem
{
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private INetManager _net = default!;
    [Dependency] private IRobustRandom _random = default!;

    private List<MutationSequence> _sequences = [];

    private int _curGeneratingID = 0;

    public override void Initialize()
    {
        base.Initialize();
    }

    public MutationSequence GetSequence(MutationPrototype mutation)
    {
        if (_net.IsClient)
        {
            throw new InvalidOperationException(
                $"Tried to get true mutation sequence from client!"
            );
        }

        // we haven't initiated mutation sequences yet
        if (_sequences.Count == 0)
            InitializeSequences();

        var sequence = _sequences.Find(x => x.ID == mutation);

        if (sequence is null)
            throw new InvalidOperationException(
                $"Mutation sequence was not initiated for mutation proto '{mutation.ID}'!"
            );

        return sequence;
    }

    /// <summary>
    /// Doesnt assign number ids yet
    /// </summary>
    /// <param name="mutation"></param>
    /// <returns></returns>
    private MutationSequence GenerateSequence(MutationPrototype mutation)
    {
        var sequence = new MutationSequence();
        for (int i = 0; i < MutationSequence.MutationSequenceLength; i++)
        {
            var b = _random.Pick(MutationBase.Bases);
            sequence.Sequence.Add(b);
        }

        return sequence;
    }

    private void InitializeSequences()
    {
        var old = _sequences;
        _sequences = [];

        foreach (var proto in _proto.EnumeratePrototypes<MutationPrototype>())
        {
            // when reloading protos we want to keep this the same
            var oldVal = old.Find(x => x.ID == proto);
            if (oldVal is not null)
            {
                _sequences.Add(oldVal);
                continue;
            }

            _sequences.Add(GenerateSequence(proto));
        }

        // Shuffle the sequence table randomly then assign IDs sequentially.
        // If ID is already assigned will be skipped, eg. on prototype reloading with new mutations
        _random.Shuffle(_sequences);
        foreach (var sequence in _sequences)
        {
            if (sequence.NumDisplayedID != -1)
                continue;

            sequence.NumDisplayedID = _curGeneratingID;
            _curGeneratingID++;
        }
    }

    [SubscribeLocalEvent]
    private void OnRoundRestart(RoundRestartCleanupEvent args)
    {
        _sequences.Clear();
    }

    [SubscribeLocalEvent]
    private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        if (args.WasModified<MutationPrototype>())
        {
            InitializeSequences();
        }
    }
}
