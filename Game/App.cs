using Game.Native;
using Game.Rendering;
using Game.Utilities;
using Game.Windowing;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D12;
using Silk.NET.Maths;

namespace Game;

public static partial class App
{
    internal static unsafe AppVars* s_appVars;
    internal static unsafe FApp* s_native_app;

    public static unsafe ref AppVars Vars => ref *s_appVars;

    public static Window Window { get; private set; } = null!;
    public static RenderingManager Rendering { get; private set; } = null!;

    internal static unsafe int Main()
    {
        s_native_app->Init();
        Rendering = new();
        MessageLoop.InitMessageLoop();
        MainLoop();
        MessageLoop.Loop();
        s_native_app->Exit();
        return 0;
    }

    internal static async void MainLoop()
    {
        await Shaders.LoadShaders();
        Window = await Window.Create(new("CC", 1280, 720) { MinSize = new(640, 360), Resizable = true, });
        Window.MarkMain();
        Rendering.MakeContext(Window);
        await TaskUtils.SwitchToLongRunning();

        var shader_ui_rect = Shaders.TryGetShader("ui/ui")!["rect"];

        while (Vars.running)
        {
            Rendering.Graph.BeginRecording(Window.Surface!);

            Draw(shader_ui_rect);

            Rendering.Graph.EndRecordingAndExecute();
        }
    }

    internal class RenderData
    {
        public ShaderPass shader_ui_rect = null!;
    }

    internal static unsafe void Draw(ShaderPass shader_ui_rect)
    {
        var builder = Rendering.Graph.AddPass("Foo", out RenderData data);
        data.shader_ui_rect = shader_ui_rect;
        builder.SetRenderFunc(static (ctx, data) =>
        {
            ctx.cmd.ClearRt(ctx.Surface, new(0, 0, 0, 1));
            ctx.cmd.SetRt(ctx.Surface);
            ctx.cmd.DrawFullScreen(data.shader_ui_rect);
        });
    }
}
