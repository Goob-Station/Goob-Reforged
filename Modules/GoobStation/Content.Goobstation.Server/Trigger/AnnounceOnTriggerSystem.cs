using Content.Server.Chat.Systems;
using Content.Shared.Trigger;

namespace Content.Goobstation.Server.Trigger;

public sealed partial class AnnounceOnTriggerSystem : EntitySystem
{
    [Dependency] private ChatSystem _chat = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AnnounceOnTriggerComponent, TriggerEvent>(OnAnnounceTrigger);
    }

    private void OnAnnounceTrigger(Entity<AnnounceOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var comp = ent.Comp;
        var message = Loc.GetString(comp.Message);
        var sender = comp.Sender != null ? Loc.GetString(comp.Sender) : Loc.GetString("chat-manager-sender-announcement");
        _chat.DispatchGlobalAnnouncement(message, sender, playSound: true, comp.Sound, comp.Color);
    }
}
