using System.Runtime.CompilerServices;
using Flecs.NET.Core;

namespace Game.Core;

public static class Transforms
{
    public static ref Entity SetTransformDefault(this ref Entity entity) =>
        ref entity.SetTransform(quaternion.Identity, float3.Zero)
            .SetChunkPosition(int3.Zero);

    public static ref Entity SetTransform(this ref Entity entity, RotPos rot_pos) =>
        ref entity.SetTransform(rot_pos.rotation, rot_pos.position);
    public static ref Entity SetTransform(this ref Entity entity,
        quaternion rot, float3 pos) =>
        ref entity.Set(new Rotation { v = rot }).Set(new Position { v = pos });
    public static ref Entity SetTransform(this ref Entity entity,
        quaternion rot, float3 pos, float3 stretch) =>
        ref entity.SetTransform(rot, pos).Set(new Stretch { v = stretch });
    public static ref Entity SetTransform(this ref Entity entity,
        quaternion rot, float3 pos, float scale) =>
        ref entity.SetTransform(rot, pos).Set(new Scale { v = scale });
    public static ref Entity SetTransform(this ref Entity entity,
        quaternion rot, float3 pos, float3 stretch, float scale) =>
        ref entity.SetTransform(rot, pos).Set(new StretchScale { v = new(stretch, scale) });

    public static ref Entity SetChunkPosition(this ref Entity entity, int3 pos) =>
        ref entity.Set(new ChunkPosition { v = pos });
}

public struct RotPos
{
    public quaternion rotation;
    public float3 position;
}

public struct Rotation
{
    public quaternion v;
}

public struct Position
{
    public float3 v;
}

public struct StretchScale
{
    public float4 v;

    public float3 stretch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => v.xyz;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => v.xyz = value;
    }

    public float scale
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => v.w;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => v.w = value;
    }
}

public struct Stretch
{
    public float3 v;
}

public struct Scale
{
    public float v;
}

public struct ChunkPosition
{
    public int3 v;
}

#region StretchScaleMergeModule

public struct StretchScaleMergeModule : IModule
{
    public void Init(ModuleInitCtx ctx)
    {
        ctx.EcsWorld.System<Stretch, Scale>("Stretch Scale Merge System")
            .Each(static (Entity e, ref Stretch stretch, ref Scale scale) =>
            {
                e.Set(new StretchScale { v = new(stretch.v, scale.v) });
                e.Remove<Stretch>();
                e.Remove<Scale>();
            });

        ctx.EcsWorld.System<StretchScale, Stretch>("StretchScale Stretch Merge System")
            .Each(static (Entity e, ref StretchScale stretch_scale, ref Stretch stretch) =>
            {
                stretch_scale.stretch = stretch.v;
                e.Remove<Stretch>();
            });

        ctx.EcsWorld.System<StretchScale, Scale>("StretchScale Scale Merge System")
            .Each(static (Entity e, ref StretchScale stretch_scale, ref Scale scale) =>
            {
                stretch_scale.scale = scale.v;
                e.Remove<Scale>();
            });
    }

    // // todo code gen
    // [System("Stretch Scale Merge System")]
    // private static void Each(Entity e, ref Stretch stretch, ref Scale scale)
    // {
    //     e.Set(new StretchScale { v = new(stretch.v, scale.v) });
    //     e.Remove<Stretch>();
    //     e.Remove<Scale>();
    // }
    //
    // [System("StretchScale Stretch Merge System")]
    // private static void Each(Entity e, ref StretchScale stretch_scale, ref Stretch stretch)
    // {
    //     stretch_scale.stretch = stretch.v;
    //     e.Remove<Stretch>();
    // }
    //
    // [System("StretchScale Scale Merge System")]
    // private static void Each(Entity e, ref StretchScale stretch_scale, ref Scale scale)
    // {
    //     stretch_scale.scale = scale.v;
    //     e.Remove<Scale>();
    // }
}

#endregion
