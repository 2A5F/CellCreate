using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Coplt.Dropping;
using Game.Native;

namespace Game.Rendering;

[Dropping(Unmanaged = true)]
public partial class CommandBuffer
{
    [Drop]
    internal NativeChunkList<byte> m_data = new();
    internal List<nuint> m_stream = new();

    internal RenderingManager Rendering;
    internal GraphicSurface? Surface { get; set; }

    internal CommandBuffer(RenderingManager rendering)
    {
        Rendering = rendering;
    }

    #region Reset

    [Drop(Unmanaged = false)]
    internal void Reset()
    {
        m_data.Clear();
        m_stream.Clear();
    }

    #endregion

    #region Internal

    #region InternalClearRtv

    internal unsafe void InternalClearRtv(uint rtv, float4 color, ReadOnlySpan<int4> rects)
    {
        m_stream.Add(m_data.Count);
        var data = m_data.Extra(1 + (nuint)sizeof(FGpuCommandClearRtv) + (nuint)(sizeof(int4) * rects.Length));
        data[0] = (byte)FGpuCommandOp.ClearRtv;
        ref var s = ref *(FGpuCommandClearRtv*)data[(nuint)1];
        s.rtv = rtv;
        s.color = Unsafe.BitCast<float4, FGpuCommandClearRtv._color_e__FixedBuffer>(color);
        s.rects = (uint)rects.Length;
        if (rects.Length > 0)
        {
            var r = new Span<int4>(data[1 + (nuint)sizeof(FGpuCommandClearRtv)], rects.Length);
            rects.CopyTo(r);
        }
    }

    #endregion

    #region InternalClearDsv

    internal unsafe void InternalClearDsv(uint dsv, float? depth, byte? stencil, ReadOnlySpan<int4> rects)
    {
        m_stream.Add(m_data.Count);
        var data = m_data.Extra(1 + (nuint)sizeof(FGpuCommandClearDsv) + (nuint)(sizeof(int4) * rects.Length));
        data[0] = (byte)FGpuCommandOp.ClearDsv;
        ref var s = ref *(FGpuCommandClearDsv*)data[(nuint)1];
        s.dsv = dsv;
        s.flags.depth = depth.HasValue ? (byte)1 : (byte)0;
        s.flags.stencil = stencil.HasValue ? (byte)1 : (byte)0;
        s.depth = depth ?? 0;
        s.stencil = stencil ?? 0;
        s.rects = (uint)rects.Length;
        if (rects.Length > 0)
        {
            var r = new Span<int4>(data[1 + (nuint)sizeof(FGpuCommandClearDsv)], rects.Length);
            rects.CopyTo(r);
        }
    }

    #endregion

    #region InternalSetRt

    internal unsafe void InternalSetRt(uint dsv, ReadOnlySpan<uint> rtvs)
    {
        m_stream.Add(m_data.Count);
        var data = m_data.Extra(1 + (nuint)sizeof(FGpuCommandSetRt) + (nuint)(sizeof(uint) * rtvs.Length));
        data[0] = (byte)FGpuCommandOp.SetRt;
        ref var s = ref *(FGpuCommandSetRt*)data[(nuint)1];
        s.dsv = dsv;
        s.rtv_count = (uint)rtvs.Length;
        if (rtvs.Length > 0)
        {
            var r = new Span<uint>(data[1 + (nuint)sizeof(FGpuCommandSetRt)], rtvs.Length);
            rtvs.CopyTo(r);
        }
    }

    #endregion

    #region InternalSetViewPort

    internal unsafe void InternalSetViewPort(ReadOnlySpan<ViewPort> viewports)
    {
        m_stream.Add(m_data.Count);
        var data = m_data.Extra(
            1 + (nuint)sizeof(FGpuCommandSetViewPort) + (nuint)(sizeof(ViewPort) * viewports.Length));
        data[0] = (byte)FGpuCommandOp.SetViewPort;
        ref var s = ref *(FGpuCommandSetViewPort*)data[(nuint)1];
        s.count = (uint)viewports.Length;
        if (viewports.Length > 0)
        {
            var r = new Span<ViewPort>(data[1 + (nuint)sizeof(FGpuCommandSetViewPort)], viewports.Length);
            viewports.CopyTo(r);
        }
    }

    #endregion

    #region InternalSetViewPort

    internal unsafe void InternalSetScissorRect(ReadOnlySpan<int4> rects)
    {
        m_stream.Add(m_data.Count);
        var data = m_data.Extra(
            1 + (nuint)sizeof(FGpuCommandSetScissorRect) + (nuint)(sizeof(int4) * rects.Length));
        data[0] = (byte)FGpuCommandOp.SetScissorRect;
        ref var s = ref *(FGpuCommandSetScissorRect*)data[(nuint)1];
        s.count = (uint)rects.Length;
        if (rects.Length > 0)
        {
            var r = new Span<int4>(data[1 + (nuint)sizeof(FGpuCommandSetScissorRect)], rects.Length);
            rects.CopyTo(r);
        }
    }

    #endregion

    #region InternalSetPipeline

    internal unsafe void InternalSetPipeline(uint pipeline)
    {
        m_stream.Add(m_data.Count);
        var data = m_data.Extra(1 + (nuint)sizeof(FGpuCommandSetPipeline));
        data[0] = (byte)FGpuCommandOp.SetPipeline;
        ref var s = ref *(FGpuCommandSetPipeline*)data[(nuint)1];
        s.pipeline = pipeline;
    }

    #endregion

    #region InternalDrawInstanced

    internal unsafe void InternalDrawInstanced(
        uint vertex_count_per_instance,
        uint instance_count,
        uint start_vertex_location,
        uint start_instance_location
    )
    {
        m_stream.Add(m_data.Count);
        var data = m_data.Extra(1 + (nuint)sizeof(FGpuCommandDrawInstanced));
        data[0] = (byte)FGpuCommandOp.DrawInstanced;
        *(FGpuCommandDrawInstanced*)data[(nuint)1] = new()
        {
            vertex_count_per_instance = vertex_count_per_instance,
            instance_count = instance_count,
            start_vertex_location = start_vertex_location,
            start_instance_location = start_instance_location,
        };
    }

    #endregion

    #region InternalDispatch InternalDispatchMesh

    internal void InternalDispatch(uint thread_group_count_x, uint thread_group_count_y, uint thread_group_count_z) =>
        InternalDispatch(FGpuCommandOp.Dispatch, thread_group_count_x, thread_group_count_y, thread_group_count_z);

    internal void InternalDispatchMesh(uint thread_group_count_x, uint thread_group_count_y,
        uint thread_group_count_z) =>
        InternalDispatch(FGpuCommandOp.DispatchMesh, thread_group_count_x, thread_group_count_y, thread_group_count_z);

    private unsafe void InternalDispatch(FGpuCommandOp op,
        uint thread_group_count_x, uint thread_group_count_y, uint thread_group_count_z
    )
    {
        m_stream.Add(m_data.Count);
        var data = m_data.Extra(1 + (nuint)sizeof(FGpuCommandDispatch));
        data[0] = (byte)op;
        ref var dispatch = ref *(FGpuCommandDispatch*)data[(nuint)1];
        dispatch.thread_group_count[0] = thread_group_count_x;
        dispatch.thread_group_count[1] = thread_group_count_y;
        dispatch.thread_group_count[2] = thread_group_count_z;
    }

    #endregion

    #endregion

    #region State

    private PipelineHandle m_current_pipeline;

    #endregion

    #region Commands

    #region Surface

    public void ClearSurface(float4 color) => ClearSurface(color, []);
    public void ClearSurface(float4 color, ReadOnlySpan<int4> rects)
    {
        if (Surface is null) throw new NotSupportedException("The current context does not contain a surface");
        InternalClearRtv(FGpuGraphConsts.SurfaceRtvId, color, rects);
    }

    public void SetRtToSurface()
    {
        if (Surface is null) throw new NotSupportedException("The current context does not contain a surface");
        InternalSetRt(0, [FGpuGraphConsts.SurfaceRtvId]);
        var size = Surface.PixelSize;
        InternalSetViewPort([new(size.x, size.y)]);
        InternalSetScissorRect([new(0, 0, (int)size.x, (int)size.y)]);
    }

    #endregion

    #region Draw

    public void DrawFullScreen(PipelineHandle pipeline) => DrawInstanced(pipeline, 4, 1, 0, 0);
    
    public void DrawInstanced(
        PipelineHandle pipeline,
        uint VertexCountPerInstance,
        uint InstanceCount,
        uint StartVertexLocation,
        uint StartInstanceLocation
    )
    {
        if (m_current_pipeline != pipeline)
        {
            m_current_pipeline = pipeline;
            InternalSetPipeline(pipeline.Id);
        }
        InternalDrawInstanced(VertexCountPerInstance, InstanceCount, StartVertexLocation, StartInstanceLocation);
    }

    #endregion

    #endregion
}
