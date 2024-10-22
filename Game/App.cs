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
            Rendering.ClearSurface(Window.Surface!, new(0, 0, 0, 1));

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

        // ↓ test code，↑ target code
        var pipeline_ui_rect = shader_ui_rect.GetOrCreateGraphicsShaderPipeline(
            new(TextureFormat.D24_UNorm_S8_UInt, [TextureFormat.R8G8B8A8_UNorm])
        );
        var size = Window.PixelSize;
        var frame = Rendering.CurrentFrameRtv(Window.Surface!);
        var cmd_list = Rendering.CurrentCommandList;
        cmd_list->OMSetRenderTargets(1, &frame, false, null);
        cmd_list->RSSetViewports(1, new Viewport(0, 0, size.x, size.y, 0, 1));
        cmd_list->RSSetScissorRects(1, new Box2D<int>(0, 0, (int)size.x, (int)size.y));
        cmd_list->IASetPrimitiveTopology(D3DPrimitiveTopology.D3DPrimitiveTopologyTrianglestrip);
        cmd_list->SetPipelineState(pipeline_ui_rect.RawPtr);
        cmd_list->DrawInstanced(4, 1, 0, 0);
    }
}
