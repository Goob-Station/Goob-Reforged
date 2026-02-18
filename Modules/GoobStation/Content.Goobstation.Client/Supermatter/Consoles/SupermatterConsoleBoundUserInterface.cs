using Content.Goobstation.Shared.Supermatter.Consoles;

namespace Content.Goobstation.Client.Supermatter.Consoles;

public sealed class SupermatterConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private SupermatterConsoleWindow? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = new SupermatterConsoleWindow(this, Owner);
        _menu.OpenCentered();
        _menu.OnClose += Close;
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        var castState = (SupermatterConsoleBoundInterfaceState)state;
        _menu?.UpdateUI(castState.Supermatters, castState.FocusData);
    }

    public void SendFocusChangeMessage(NetEntity? netEntity)
    {
        SendMessage(new SupermatterConsoleFocusChangeMessage(netEntity));
    }
}
