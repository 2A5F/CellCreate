namespace Game.Core;

public class TestModule : IModule
{
    public void Init(ModuleInitCtx ctx)
    {
        var e = ctx.EcsWorld.Entity();
        e.SetTransformDefault();
    }
}
