using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Coplt.Dropping;
using Game.Native;

namespace Game.Windowing;

public record struct WindowId(uint Value);

[Dropping(Unmanaged = true)]
internal unsafe partial class WindowHandle
{
    internal FWindowHandle* m_ptr;
    internal WindowId m_id;
    public string Title { get; private set; }
    public WindowId Id => m_id;

    internal WindowHandle(FWindowHandle* ptr, string title)
    {
        m_ptr = ptr;
        Title = title;
        FWindowId id;
        ptr->Id(&id).TryThrow();
        m_id = new(id.value);
    }

    [Drop]
    private void Drop()
    {
        if (ExchangeUtils.ExchangePtr(ref m_ptr, null, out var inner) is null) return;
        inner->Release();
    }

    public string GetTitle() => Title = Marshal.PtrToStringUTF8((IntPtr)m_ptr->Title())!;
    public void SetTitle(string value)
    {
        Title = value;
        fixed (byte* title = Encoding.UTF8.GetBytes(value))
        {
            m_ptr->SetTitle(title).TryThrow();
        }
    }

    public uint2 Size
    {
        get
        {
            uint2 size;
            m_ptr->Size(&size).TryThrow();
            return size;
        }
    }

    public uint2 PixelSize
    {
        get
        {
            uint2 size;
            m_ptr->PixelSize(&size).TryThrow();
            return size;
        }
    }

    public void Show() => m_ptr->Show().TryThrow();

    public void Hide() => m_ptr->Hide().TryThrow();
}
