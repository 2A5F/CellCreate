using System.Runtime.CompilerServices;
using Coplt.Dropping;
using Game.Native;

namespace Game.Windowing;

[Dropping(Unmanaged = true)]
public unsafe partial class WindowHandle
{
    internal FWindowHandle* m_inner;

    internal WindowHandle(FWindowHandle* mInner)
    {
        m_inner = mInner;
    }

    [Drop]
    private void Drop()
    {
        if (ExchangeUtils.ExchangePtr(ref m_inner, null, out var inner) is null) return;
        inner->Release();
    }

    public uint2 Size
    {
        get
        {
            uint2 size;
            m_inner->Size(&size).TryThrow();
            return size;
        }
    }

    public uint2 PixelSize
    {
        get
        {
            uint2 size;
            m_inner->PixelSize(&size).TryThrow();
            return size;
        }
    }
}
