using Content.Goobstation.Shared.Genetics.Mutations.Components;
using Content.Shared.Interaction;
using Content.Shared.Paper;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.Genetics.Mutations.Systems;

// Unfinished!
public sealed partial class AphasiaSystem : EntitySystem
{
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private TagSystem _tag = default!;

    private ProtoId<TagPrototype> _writeTag = "Write";

    [SubscribeLocalEvent]
    private void OnInteractUsing(Entity<AphasiaComponent> ent, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (_tag.HasTag(args.Used, _writeTag) && HasComp<PaperComponent>(args.Target))
        {
            args.Handled = true;
            _popup.PopupEntity(Loc.GetString(ent.Comp.LocCannotRead), ent, ent);
        }
    }
}
