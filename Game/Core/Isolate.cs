using System.Reflection;
using Coplt.Dropping;
using Flecs.NET.Core;

namespace Game.Core;

[Dropping(Unmanaged = true)]
public partial class Isolate
{
    [Drop]
    public World EcsWorld = World.Create();

    internal void InitModules() => InitModules(typeof(Isolate).Assembly);
    internal void InitModules(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface) continue;
            if (!type.IsAssignableTo(typeof(IModule))) continue;
            var inst = (IModule)Activator.CreateInstance(type)!;
            inst.Init(new ModuleInitCtx { EcsWorld = ref EcsWorld });
        }
    }

    internal void Tick()
    {
        // todo delta time
        EcsWorld.Progress();
    }
}
