using System.Collections.Concurrent;
using Game.Native;
using Game.Windowing;

namespace Game;

public static class App
{
    internal static unsafe AppVars* s_appVars;
    internal static unsafe FApp* s_native_app;

    public static unsafe ref AppVars Vars => ref *s_appVars;

    public static Window Window { get; private set; } = null!;

    internal static unsafe int Main()
    {
        s_native_app->Init();
        MessageLoop.InitMessageLoop();
        new Thread(MainLoop) { Name = "Main Loop" }.Start();
        MessageLoop.Loop();
        s_native_app->Exit();
        return 0;
    }

    internal static async void MainLoop()
    {
        Window = await Window.Create(new("CC", 1280, 720) { MinSize = new(640, 360), Resizable = true, });
        Window.MarkMain();
        while (Vars.running) { }
    }
}
