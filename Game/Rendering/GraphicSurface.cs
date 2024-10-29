using Coplt.Dropping;
using Game.Native;
using Game.Windowing;
using Silk.NET.Direct3D12;

namespace Game.Rendering;

[Dropping(Unmanaged = true)]
public sealed unsafe partial class GraphicSurface
{
    public RenderingManager RenderingManager { get; }
    private WindowHandle WindowHandle { get; }
    internal FGraphicSurface* m_ptr;
    internal FGraphicSurfaceData* m_data;

    public bool Closed { get; private set; }

    internal GraphicSurface(FGraphicSurface* ptr, RenderingManager rendering_manager, WindowHandle window_handle)
    {
        m_ptr = ptr;
        RenderingManager = rendering_manager;
        WindowHandle = window_handle;

        FGraphicSurfaceData* data;
        m_ptr->DataPtr(&data).TryThrow();
        m_data = data;
    }

    [Drop]
    public void Destroy()
    {
        if (ExchangeUtils.ExchangePtr(ref m_ptr, null, out var ptr) is null) return;
        RenderingManager.RenderingContexts.Remove(WindowHandle, out _);
        ptr->Destroy().TryThrow();
        ptr->Release();
    }

    internal void OnClose()
    {
        Closed = true;
    }

    internal CpuDescriptorHandle CurrentFrameRtv => new((UIntPtr)m_data->current_frame_rtv);

    public TextureFormat Format => m_data->format;

    public uint2 Size => m_data->size;

    public void OnResize(uint2 new_size) => m_ptr->OnResize(new_size).TryThrow();
}
