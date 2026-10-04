using Content.Goobstation.Shared.Genetics.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.Genetics.Types;

public sealed partial class MutationSequence
{
    [ViewVariables(VVAccess.ReadOnly)]
    public static int MutationSequenceLength = 4 * 4;

    [ViewVariables(VVAccess.ReadOnly)]
    public readonly ProtoId<MutationPrototype> ID;

    /// <summary>
    /// Used for when the mutation name is unknown. FOR HUMAN READABLE!! do not use this internally for identification.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public int NumDisplayedID = -1;

    [ViewVariables(VVAccess.ReadOnly)]
    // note that this is only the Top sequence.
    // we can just easily get the bottom one when using it so its simpler.
    public readonly List<char> Sequence = [];
}
