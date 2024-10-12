using Coplt.Dropping;
using Game.Native;
using Game.Windowing;

namespace Game.Rendering;

[Dropping(Unmanaged = true)]
public sealed unsafe partial class RenderingManager
{
    internal FRendering* m_ptr;

    internal RenderingManager()
    {
        FRendering* ptr;
        App.s_native_app->CreateRendering(&ptr).TryThrow();
        m_ptr = ptr;
    }

    [Drop]
    private void Drop()
    {
        if (ExchangeUtils.ExchangePtr(ref m_ptr, null, out var ptr) is null) return;
        ptr->Release();
    }

    internal void Init(Window window)
    {
        m_ptr->Init(window.Handle.m_ptr).TryThrow();
    }

    public bool VSync
    {
        get => m_ptr->VSync();
        set => m_ptr->SetVSync(value).TryThrow();
    }

    internal void OnResize(uint2 size) => m_ptr->OnResize(size).TryThrow();

    internal void ReadyFrame() => m_ptr->ReadyFrame().TryThrow();

    internal void EndFrame() => m_ptr->EndFrame().TryThrow();
}
