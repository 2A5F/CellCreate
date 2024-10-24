using System.Runtime.InteropServices;
using Coplt.Dropping;
using Game.Native;
using Game.Utilities;

namespace Game.Rendering;

[Dropping(Unmanaged = true)]
public sealed unsafe partial class RenderGraph
{
    #region Native And LifeTime

    internal FGpuGraph* m_ptr;
    public RenderingManager Rendering { get; }

    public RenderGraph(RenderingManager rendering)
    {
        Rendering = rendering;
        FGpuGraph* ptr;
        rendering.m_ptr->CreateGraph(&ptr).TryThrow();
        m_ptr = ptr;

        CommandBufferPool = ObjectPool.Create(() => new CommandBuffer(Rendering));
    }

    [Drop]
    private void Drop()
    {
        if (ExchangeUtils.ExchangePtr(ref m_ptr, null, out var ptr) is null) return;
        ptr->Release();
    }

    #endregion

    #region ObjectPool

    internal ObjectPool ObjectPool { get; } = new();
    internal ObjectPool<Pass> PassPool { get; } = ObjectPool.Create<Pass>();
    [Drop]
    internal ObjectPool<CommandBuffer> CommandBufferPool { get; }

    #endregion

    private readonly List<Pass> passes = new();

    #region State

    private bool IsRecording { get; set; }

    private GraphicSurface Surface = null!;
    private FramedData FramedData = new();

    #endregion

    #region Pass

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

    #endregion

    #region Recording

    public void BeginRecording(GraphicSurface surface)
    {
        Surface = surface;

        IsRecording = true;
        Rendering.ReadyFrame();

        FramedData.SurfaceSize = surface.Size;
    }

    public void EndRecordingAndExecute()
    {
        Execute();
        // todo
        Rendering.EndFrame();
        IsRecording = false;
    }

    #endregion

    #region AddPass

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

    #endregion

    #region Execute

    private void Execute()
    {
        var cmd = CommandBufferPool.Get();
        try
        {
            var ctx = new RenderGraphContext
            {
                Rendering = Rendering,
                Surface = Surface,
                cmd = cmd,
                FramedData = ref FramedData,
            };
            foreach (var pass in passes)
            {
                pass.ExecRenderFunc(pass, ctx, pass.Data, pass.RenderFunc);
            }

            {
                fixed (UIntPtr* p_stream = CollectionsMarshal.AsSpan(cmd.m_stream))
                {
                    var cmds = new FGpuStreamCommands
                    {
                        count = (nuint)cmd.m_stream.Count,
                        stream = (FGpuCommandOp**)p_stream,
                    };
                    m_ptr->ExecuteCommand(cmds).TryThrow();
                }
            }
        }
        finally
        {
            cmd.Reset();
            CommandBufferPool.Return(cmd);
            foreach (var pass in passes)
            {
                pass.Reset();
                PassPool.Return(pass);
            }
            passes.Clear();
        }
    }

    #endregion
}

#region PassBuilder

public struct PassBuilder<T>
{
    private RenderGraph.Pass Pass;

    internal PassBuilder(RenderGraph.Pass pass)
    {
        Pass = pass;
    }

    #region SetRenderFunc

    public void SetRenderFunc(RenderGraphRenderFunc<T> func)
    {
        Pass.RenderFunc = func;
    }

    #endregion
}

#endregion

#region RenderGraphRenderFunc RenderGraphExecRenderFunc

public delegate void RenderGraphRenderFunc<in T>(RenderGraphContext ctx, T data);

internal delegate void RenderGraphExecRenderFunc(
    RenderGraph.Pass pass, RenderGraphContext ctx, object data, object? func
);

#endregion

#region RenderGraphContext

public ref struct RenderGraphContext
{
    public RenderingManager Rendering;
    public GraphicSurface Surface;
    public CommandBuffer cmd;
    public ref readonly FramedData FramedData;
}

#endregion

#region FramedData

public struct FramedData
{
    public uint2 SurfaceSize;
}

#endregion

#region RtHandle

public record struct RtHandle(uint Id);

#endregion
