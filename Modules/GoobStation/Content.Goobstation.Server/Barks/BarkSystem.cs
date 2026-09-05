using Content.Goobstation.Common.Barks;
using Content.Goobstation.Common.ConVars;
using Robust.Shared.Configuration;
using Content.Shared.Chat;

namespace Content.Goobstation.Server.Barks;

public sealed class BarkSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpeechSynthesisComponent, EntitySpokeEvent>(OnEntitySpoke);
    }

    private void OnEntitySpoke(EntityUid uid, SpeechSynthesisComponent comp, EntitySpokeEvent args)
    {
        if (comp.VoicePrototypeId is null
            //|| !args.Language.SpeechOverride.RequireSpeech // todo marty languages
            || !_configurationManager.GetCVar(GoobConVars.BarksEnabled))
            return;

        var sourceEntity = GetNetEntity(uid);
        RaiseNetworkEvent(new PlayBarkEvent(sourceEntity, args.Message
            //,args.IsWhisper
        ));
    }
}
