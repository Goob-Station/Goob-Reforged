using Robust.Shared.Network;

namespace Content.Goobstation.Shared.Redial;

public abstract partial class SharedRedialManager : IPostInjectInit
{
    [Dependency] protected INetManager NetManager = default!;

    public void PostInject()
    {
        Initialize();
    }

    public virtual void Initialize() { }
}
