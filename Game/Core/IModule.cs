using Flecs.NET.Core;

namespace Game.Core;

public ref struct ModuleInitCtx
{
    public ref World EcsWorld;
}

public interface IModule
{
    public void Init(ModuleInitCtx ctx);
}
