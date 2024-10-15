using System.Text;
using Game.Native;
using Game.Rendering;

namespace Game.Windowing;

public sealed class Window
{
    internal WindowHandle? m_handle;

    internal Window(WindowHandle mHandle) => m_handle = mHandle;

    internal WindowHandle Handle => m_handle ?? throw new NullReferenceException("The window was destroyed");

    public RenderingContext? Context { get; internal set; }

    public event Action<uint2>? OnResize;

    internal void Destroy()
    {
        Context?.Destroy();
        Handle.Dispose();
        m_handle = null;
    }

    public record struct Options(string Title, uint2 Size)
    {
        public uint2? MinSize { get; set; }
        public uint2? MaxSize { get; set; }
        public bool Resizable { get; set; }
        public bool Hide { get; set; }

        public Options(string Title, uint w, uint h) : this(Title, new(w, h)) { }
    }

    public static async ValueTask<Window> Create(Options options)
    {
        await MessageLoop.SwitchToMessageThread();
        var r = CreateOnMessageThread(options);
        await Task.Yield();
        return r;
    }

    public static unsafe Window CreateOnMessageThread(Options options)
    {
        fixed (byte* title = Encoding.UTF8.GetBytes(options.Title))
        {
            FWindowCreateOptions f_options = new()
            {
                title = title,
                size = options.Size,
                resizable = options.Resizable,
                hide = options.Hide,
            };
            if (options.MinSize.HasValue)
            {
                f_options.min_size = options.MinSize.Value;
                f_options.has_min_size = true;
            }
            if (options.MaxSize.HasValue)
            {
                f_options.max_size = options.MaxSize.Value;
                f_options.has_max_size = true;
            }
            FWindowHandle* p_handle;
            App.s_native_app->CreateWindowHandle(&f_options, &p_handle).TryThrow();
            var handle = new WindowHandle(p_handle, options.Title);
            var window = new Window(handle);
            WindowManager.RegisterWindow(handle.Id, window);
            return window;
        }
    }

    public WindowId Id => Handle.Id;

    public uint2 Size => Handle.Size;
    public uint2 PixelSize => Handle.PixelSize;

    public void Show() => Handle.Show();
    public void Hide() => Handle.Hide();

    public void MarkMain()
    {
        WindowManager.MarkMain(this);
    }

    public void Close()
    {
        WindowManager.OnWindowClosed(Id);
    }

    internal void OnResized()
    {
        var new_size = Size;
        OnResize?.Invoke(new_size);
        Context?.OnResize(new_size);
    }
}
