using Game.Native;
using Game.Rendering;
using Game.Utilities;
using Game.Windowing;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D12;
using Silk.NET.Maths;

namespace Game;

public static class App
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
        var pipeline_ui_rect =
            shader_ui_rect.GetOrCreatePipelineState(
                new(TextureFormat.D24_UNorm_S8_UInt, [TextureFormat.R8G8B8A8_UNorm]));

        while (Vars.running)
        {
            Rendering.Graph.BeginRecording(Window.Context!);
            Rendering.ClearSurface(Window.Context!, new(0, 0, 0, 1));

            Draw(pipeline_ui_rect);

            Rendering.Graph.EndRecordingAndExecute();
        }
    }

    internal class RenderData
    {
        public ShaderPipeline pipeline_state = null!;
    }

    internal static unsafe void Draw(ShaderPipeline pipeline_state)
    {
        var builder = Rendering.Graph.AddPass("Foo", out RenderData data);
        data.pipeline_state = pipeline_state;
        builder.SetRenderFunc(static (ctx, data) =>
        {
            
        });

        var size = Window.PixelSize;
        var frame = Rendering.CurrentFrameRtv(Window.Context!);
        var cmd_list = Rendering.CurrentCommandList;
        cmd_list->OMSetRenderTargets(1, &frame, false, null);
        cmd_list->RSSetViewports(1, new Viewport(0, 0, size.x, size.y, 0, 1));
        cmd_list->RSSetScissorRects(1, new Box2D<int>(0, 0, (int)size.x, (int)size.y));
        cmd_list->IASetPrimitiveTopology(D3DPrimitiveTopology.D3DPrimitiveTopologyTrianglestrip);
        cmd_list->SetPipelineState(pipeline_state.RawPtr);
        cmd_list->DrawInstanced(4, 1, 0, 0);
    }
}
