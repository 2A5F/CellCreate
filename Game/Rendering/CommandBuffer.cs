using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Coplt.Dropping;
using Game.Native;
using Silk.NET.Direct3D12;

namespace Game.Rendering;

[Dropping(Unmanaged = true)]
public partial class CommandBuffer
{
    [Drop]
    internal NativeChunkList<byte> m_data = new();
    internal List<nuint> m_stream = new();

    internal RenderingManager Rendering;

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

    #region InternalDebugScope

    /// <param name="str">必须静态生命周期</param>
    internal unsafe void InternalDebugScopeStart(NativeString16 str)
    {
        var data = m_data.Extra(
            (nuint)(1 + sizeof(FGpuCommandString))
        );
        m_stream.Add((nuint)data.data);
        data[0] = (byte)FGpuCommandOp.DebugScopeStart;
        var s = (FGpuCommandString*)data[(nuint)1];
        s->str = str.m_ptr;
    }

    internal unsafe void InternalDebugScopeEnd()
    {
        var data = m_data.Extra(1);
        m_stream.Add((nuint)data.data);
        data[0] = (byte)FGpuCommandOp.DebugScopeEnd;
    }

    #endregion

    #region InternalClearRtv

    internal unsafe void InternalClearRtv(CpuDescriptorHandle rtv, float4 color, ReadOnlySpan<int4> rects)
    {
        var data = m_data.Extra(
            (nuint)(1 + sizeof(FGpuCommandClearRtv) + sizeof(CpuDescriptorHandle) + sizeof(int4) * rects.Length)
        );
        m_stream.Add((nuint)data.data);
        data[0] = (byte)FGpuCommandOp.ClearRtv;
        var s = (FGpuCommandClearRtv*)data[(nuint)1];
        *(float4*)&s->color_r = color;
        s->rects = (uint)rects.Length;
        *(CpuDescriptorHandle*)data[(nuint)(1 + sizeof(FGpuCommandClearRtv))] = rtv;
        if (rects.Length > 0)
        {
            var r = new Span<int4>(
                data[(nuint)(1 + sizeof(FGpuCommandClearRtv) + sizeof(CpuDescriptorHandle))],
                rects.Length
            );
            rects.CopyTo(r);
        }
    }

    #endregion

    #region InternalClearDsv

    internal unsafe void InternalClearDsv(
        CpuDescriptorHandle dsv, float? depth, byte? stencil, ReadOnlySpan<int4> rects
    )
    {
        var data = m_data.Extra(
            (nuint)(1 + sizeof(FGpuCommandClearDsv) + sizeof(CpuDescriptorHandle) + sizeof(int4) * rects.Length)
        );
        m_stream.Add((nuint)data.data);
        data[0] = (byte)FGpuCommandOp.ClearDsv;
        var s = (FGpuCommandClearDsv*)data[(nuint)1];
        s->rects = (uint)rects.Length;
        s->depth = depth ?? 0;
        s->stencil = stencil ?? 0;
        s->flags.depth = depth.HasValue ? (byte)1 : (byte)0;
        s->flags.stencil = stencil.HasValue ? (byte)1 : (byte)0;
        *(CpuDescriptorHandle*)data[(nuint)(1 + sizeof(FGpuCommandClearDsv))] = dsv;
        if (rects.Length > 0)
        {
            var r = new Span<int4>(
                data[(nuint)(1 + sizeof(FGpuCommandClearDsv) + sizeof(CpuDescriptorHandle))],
                rects.Length
            );
            rects.CopyTo(r);
        }
    }

    #endregion

    #region InternalSetRt

    internal unsafe void InternalSetRt(
        CpuDescriptorHandle? dsv, ReadOnlySpan<CpuDescriptorHandle> rtvs, PipelineRtOverride formats
    )
    {
        var data = m_data.Extra(
            (nuint)(
                1 + sizeof(FGpuCommandSetRt)
                  + (dsv.HasValue ? sizeof(CpuDescriptorHandle) : 0)
                  + sizeof(CpuDescriptorHandle) * rtvs.Length
                  + (dsv.HasValue ? sizeof(TextureFormat) : 0)
                  + sizeof(TextureFormat) * rtvs.Length
            )
        );
        m_stream.Add((nuint)data.data);
        data[0] = (byte)FGpuCommandOp.SetRt;
        var s = (FGpuCommandSetRt*)data[(nuint)1];
        s->rtv_count = (byte)rtvs.Length;
        s->has_dsv = dsv.HasValue;
        var p = data[(nuint)(1 + sizeof(FGpuCommandSetRt))];
        if (dsv.HasValue)
        {
            *(CpuDescriptorHandle*)p = dsv.Value;
            p += sizeof(CpuDescriptorHandle);
        }
        if (rtvs.Length > 0)
        {
            var r = new Span<CpuDescriptorHandle>(
                p,
                rtvs.Length
            );
            rtvs.CopyTo(r);
            p += sizeof(CpuDescriptorHandle) * rtvs.Length;
        }
        if (dsv.HasValue)
        {
            *(TextureFormat*)p = formats.Dsv;
            p += sizeof(TextureFormat);
        }
        if (rtvs.Length > 0)
        {
            var r = new Span<TextureFormat>(
                p,
                rtvs.Length
            );
            formats.Rtvs.CopyTo(r);
        }
    }

    #endregion

    #region InternalSetViewPort

    internal unsafe void InternalSetViewPort(ReadOnlySpan<ViewPort> viewports)
    {
        var data = m_data.Extra(
            (nuint)(1 + sizeof(FGpuCommandSetViewPort) + sizeof(ViewPort) * viewports.Length)
        );
        m_stream.Add((nuint)data.data);
        data[0] = (byte)FGpuCommandOp.SetViewPort;
        var s = (FGpuCommandSetViewPort*)data[(nuint)1];
        s->count = (byte)viewports.Length;
        if (viewports.Length > 0)
        {
            var r = new Span<ViewPort>(data[(nuint)(1 + sizeof(FGpuCommandSetViewPort))], viewports.Length);
            viewports.CopyTo(r);
        }
    }

    #endregion

    #region InternalSetViewPort

    internal unsafe void InternalSetScissorRect(ReadOnlySpan<int4> rects)
    {
        var data = m_data.Extra(
            (nuint)(1 + sizeof(FGpuCommandSetScissorRect) + sizeof(int4) * rects.Length)
        );
        m_stream.Add((nuint)data.data);
        data[0] = (byte)FGpuCommandOp.SetScissorRect;
        var s = (FGpuCommandSetScissorRect*)data[(nuint)1];
        s->count = (byte)rects.Length;
        if (rects.Length > 0)
        {
            var r = new Span<int4>(data[(nuint)(1 + sizeof(FGpuCommandSetScissorRect))], rects.Length);
            rects.CopyTo(r);
        }
    }

    #endregion

    #region InternalSetShader

    internal unsafe void InternalSetShader(FShaderPass* pipeline)
    {
        var data = m_data.Extra((nuint)(1 + sizeof(FGpuCommandSetShader)));
        m_stream.Add((nuint)data.data);
        data[0] = (byte)FGpuCommandOp.SetShader;
        var s = (FGpuCommandSetShader*)data[(nuint)1];
        s->pass = pipeline;
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
        var data = m_data.Extra((nuint)(1 + sizeof(FGpuCommandDrawInstanced)));
        m_stream.Add((nuint)data.data);
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

    internal void InternalDispatch(
        uint thread_group_count_x, uint thread_group_count_y, uint thread_group_count_z
    ) => InternalDispatch(FGpuCommandOp.Dispatch, thread_group_count_x, thread_group_count_y, thread_group_count_z);

    internal void InternalDispatchMesh(
        uint thread_group_count_x, uint thread_group_count_y, uint thread_group_count_z
    ) => InternalDispatch(FGpuCommandOp.DispatchMesh, thread_group_count_x, thread_group_count_y, thread_group_count_z);

    private unsafe void InternalDispatch(FGpuCommandOp op,
        uint thread_group_count_x, uint thread_group_count_y, uint thread_group_count_z
    )
    {
        var data = m_data.Extra(1 + (nuint)sizeof(FGpuCommandDispatch));
        m_stream.Add((nuint)data.data);
        data[0] = (byte)op;
        *(FGpuCommandDispatch*)data[(nuint)1] = new()
        {
            thread_group_count_x = thread_group_count_x,
            thread_group_count_y = thread_group_count_y,
            thread_group_count_z = thread_group_count_z,
        };
    }

    #endregion

    #endregion

    #region Commands

    #region Surface

    public void ClearRt(GraphicSurface surface, float4 color) => ClearRt(surface, color, []);
    public void ClearRt(GraphicSurface surface, float4 color, ReadOnlySpan<int4> rects)
    {
        InternalClearRtv(surface.CurrentFrameRtv, color, rects);
    }

    public void SetRt(GraphicSurface surface)
    {
        InternalSetRt(null, [surface.CurrentFrameRtv], new(TextureFormat.Unknown, [surface.Format]));
        var size = surface.Size;
        InternalSetViewPort([new(size.x, size.y)]);
        InternalSetScissorRect([new(0, 0, (int)size.x, (int)size.y)]);
    }

    #endregion

    #region Draw

    public void DrawFullScreen(ShaderPass pass) => DrawInstanced(pass, 4, 1, 0, 0);

    public unsafe void DrawInstanced(
        ShaderPass pass,
        uint VertexCountPerInstance,
        uint InstanceCount,
        uint StartVertexLocation,
        uint StartInstanceLocation
    )
    {
        InternalSetShader(pass.m_ptr);
        InternalDrawInstanced(VertexCountPerInstance, InstanceCount, StartVertexLocation, StartInstanceLocation);
    }

    #endregion

    #endregion
}
