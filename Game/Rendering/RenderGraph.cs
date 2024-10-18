using Game.Utilities;

namespace Game.Rendering;

public sealed class RenderGraph
{
    public required RenderingManager Rendering { get; init; }
    private bool IsRecording { get; set; }

    public ObjectPool ObjectPool { get; } = new();

    private readonly List<Pass> passes = new();

    private RenderingContext RenderingContext = null!;
    private FramedData FramedData = new();

    internal class Pass
    {
        public string Name { get; set; } = null!;
        public object Data { get; set; } = null!;
        public RenderGraphExecRenderFunc ExecRenderFunc { get; set; } = null!;
        public object? RenderFunc { get; set; }

        public void Reset()
        {
            Name = null!;
            Data = null!;
            ExecRenderFunc = null!;
            RenderFunc = null!;
        }
    }

    public void BeginRecording(RenderingContext context)
    {
        RenderingContext = context;

        IsRecording = true;
        Rendering.ReadyFrame();

        FramedData.SurfaceSize = context.PixelSize;
    }

    public void EndRecordingAndExecute()
    {
        IsRecording = false;
        // todo
        Rendering.EndFrame();
    }

    public PassBuilder<T> AddPass<T>(string name, out T data) where T : class, new()
    {
        data = ObjectPool.Get<T>();
        var pass = ObjectPool.Get<Pass>();
        pass.Name = name;
        pass.Data = data;
        pass.ExecRenderFunc = ExecRenderFunc<T>;
        passes.Add(pass);
        return new(pass);
    }

    private static void ExecRenderFunc<T>(Pass pass, RenderGraphContext ctx, object data, object? func)
    {
        if (func is null) throw new ArgumentNullException(nameof(func), $"Pass {pass.Name} no render func is set");
        ((RenderGraphRenderFunc<T>)func)(ctx, (T)data);
    }
}

public struct PassBuilder<T>
{
    private RenderGraph.Pass Pass;

    internal PassBuilder(RenderGraph.Pass pass)
    {
        Pass = pass;
    }

    public void SetRenderFunc(RenderGraphRenderFunc<T> func)
    {
        Pass.RenderFunc = func;
    }
}

public delegate void RenderGraphRenderFunc<in T>(RenderGraphContext ctx, T data);

internal delegate void RenderGraphExecRenderFunc(
    RenderGraph.Pass pass, RenderGraphContext ctx, object data, object? func
);

public ref struct RenderGraphContext
{
    public RenderingManager Rendering;
    public RenderingContext RenderingContext;
    public ref readonly FramedData FramedData;
}

public struct FramedData
{
    public uint2 SurfaceSize;
}
