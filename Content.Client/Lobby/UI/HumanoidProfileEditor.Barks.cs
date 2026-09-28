// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: MIT-WIZARDS

using Content.Goobstation.Common.Barks;
using System.Linq;

namespace Content.Client.Lobby.UI;

public sealed partial class HumanoidProfileEditor
{
    private List<BarkPrototype> _barkPrototypes = new();

    private void InitializeBarkVoice()
    {

        BarkVoiceButton.OnItemSelected += args =>
        {
            BarkVoiceButton.SelectId(args.Id);
            SetBarkVoice(_barkPrototypes[args.Id]);
            PlayPreviewBark();
        };

        BarkVoicePlayButton.OnPressed += _ => PlayPreviewBark();
    }

    private void UpdateBarkVoice()
    {
        if (Profile is null)
            return;

        _barkPrototypes = _prototypeManager
            .EnumeratePrototypes<BarkPrototype>()
            .Where(o => o.RoundStart &&
                        (o.SpeciesWhitelist is null ||
                         o.SpeciesWhitelist.Contains(Profile.Species)))
            .OrderBy(o => Loc.GetString(o.ID))
            .ToList();

        BarkVoiceButton.Clear();

        var selectedBarkId = -1;
        for (var i = 0; i < _barkPrototypes.Count; i++)
        {
            var bark = _barkPrototypes[i];
            if (bark == Profile.BarkVoice)
                selectedBarkId = i;

            BarkVoiceButton.AddItem(Loc.GetString(bark.Name), i);
        }

        if (selectedBarkId == -1)
            selectedBarkId = 0;

        BarkVoiceButton.SelectId(selectedBarkId);
        SetBarkVoice(_barkPrototypes[selectedBarkId]);
    }

    private void PlayPreviewBark()
    {
        if (Profile is null)
            return;

        var ev = new PreviewBarkEvent(Profile.BarkVoice);
        _entManager.EventBus.RaiseEvent(EventSource.Local, ref ev);
    }

    private void SetBarkVoice(BarkPrototype newVoice)
    {
        Profile = Profile?.WithBarkVoice(newVoice);
        IsDirty = true;
    }
}
