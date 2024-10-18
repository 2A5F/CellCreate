using Coplt.Dropping;
using Game.Native;
using Game.Windowing;

namespace Game.Rendering;

[Dropping(Unmanaged = true)]
public sealed unsafe partial class RenderingContext
{
    public RenderingManager RenderingManager { get; }
    private WindowHandle WindowHandle { get; }
    internal FRenderingContext* m_ptr;

    internal RenderingContext(FRenderingContext* ptr, RenderingManager rendering_manager, WindowHandle window_handle)
    {
        m_ptr = ptr;
        RenderingManager = rendering_manager;
        WindowHandle = window_handle;
    }

    [Drop]
    public void Destroy()
    {
        if (ExchangeUtils.ExchangePtr(ref m_ptr, null, out var ptr) is null) return;
        RenderingManager.RenderingContexts.Remove(WindowHandle, out _);
        ptr->Destroy().TryThrow();
        ptr->Release();
    }

    public void OnResize(uint2 new_size) => m_ptr->OnResize(new_size).TryThrow();
    
    public uint2 PixelSize => WindowHandle.PixelSize;
}
