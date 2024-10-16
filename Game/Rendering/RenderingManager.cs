using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Coplt.Dropping;
using Game.Native;
using Game.Utilities;
using Game.Windowing;
using Silk.NET.Direct3D12;

namespace Game.Rendering;

[Dropping(Unmanaged = true)]
public sealed unsafe partial class RenderingManager
{
    internal FRendering* m_ptr;
    internal readonly FRenderingState* m_state;

    internal RenderingManager()
    {
        FRendering* ptr;
        App.s_native_app->CreateRendering(&ptr).TryThrow();
        m_ptr = ptr;
        m_state = m_ptr->StatePtr();
    }

    [Drop]
    private void Drop()
    {
        if (ExchangeUtils.ExchangePtr(ref m_ptr, null, out var ptr) is null) return;
        ptr->Release();
    }

    internal readonly ConcurrentDictionary<WindowHandle, RenderingContext> RenderingContexts = new();

    internal RenderingContext MakeContext(Window window) => RenderingContexts.GetOrAdd(window.Handle,
        static (Handle, data) =>
        {
            var (window, self) = data;
            FRenderingContext* ptr;
            self.m_ptr->MakeContext(Handle.m_ptr, &ptr).TryThrow();
            var ctx = new RenderingContext(ptr, self, Handle);
            window.Context = ctx;
            return ctx;
        }, (window, this));

    public bool VSync
    {
        get => m_ptr is null ? throw new NullReferenceException() : m_state->v_sync;
        set => m_state->v_sync = m_ptr is null ? throw new NullReferenceException() : value;
    }

    public ulong FrameCount => m_ptr is null ? throw new NullReferenceException() : m_state->frame_count;

    internal void ReadyFrame() => m_ptr->ReadyFrame().TryThrow();

    internal void EndFrame() => m_ptr->EndFrame().TryThrow();

    public ID3D12Device2* Device
    {
        get
        {
            ID3D12Device2* ptr;
            m_ptr->GetDevice((void**)&ptr).TryThrow();
            return ptr;
        }
    }

    public ID3D12GraphicsCommandList6* CurrentCommandList
    {
        get
        {
            ID3D12GraphicsCommandList6* ptr;
            m_ptr->CurrentCommandList((void**)&ptr).TryThrow();
            return ptr;
        }
    }

    public void ClearSurface(RenderingContext ctx, float4 color) => m_ptr->ClearSurface(ctx.m_ptr, color).TryThrow();
}
