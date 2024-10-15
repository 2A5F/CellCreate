using System.Collections.Concurrent;
using Game.Native;

namespace Game.Windowing;

public static class WindowManager
{
    internal static readonly Dictionary<WindowId, Window> s_windows = new();

    internal static Window? MainWindow { get; private set; }

    internal static void MarkMain(Window window)
    {
        MainWindow = window;
    }

    internal static void RegisterWindow(WindowId id, Window handle)
    {
        s_windows.Add(id, handle);
    }

    internal static void OnWindowClosed(WindowId id)
    {
        if (!s_windows.TryGetValue(id, out var window)) return;
        window.Destroy();
        s_windows.Remove(id);
        if (window == MainWindow)
        {
            App.Vars.running = false;
        }
    }

    internal static void OnWindowResized(WindowId id)
    {
        if (!s_windows.TryGetValue(id, out var window)) return;
        window.OnResized();
    }
}
