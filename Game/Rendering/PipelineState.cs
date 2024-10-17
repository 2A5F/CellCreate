using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Coplt.Dropping;
using Game.Native;
using Game.Rendering.Contents;
using Serilog;
using Silk.NET.Direct3D12;

namespace Game.Rendering;

public record struct PipelineStateIndex
{
    public TextureFormat Dsv;
    public RtvArray Rtvs;
    public byte RtvCount;

    public PipelineStateIndex() : this(TextureFormat.D24_UNorm_S8_UInt, [TextureFormat.R8G8B8A8_UNorm]) { }

    internal PipelineStateIndex(in ShaderStateModel state) : this() { } // todo state

    public PipelineStateIndex(TextureFormat dsv, ReadOnlySpan<TextureFormat> rtvs)
    {
        Dsv = dsv;
        RtvCount = (byte)rtvs.Length;
        Unsafe.SkipInit(out Rtvs);
        rtvs.CopyTo(RtvSpan);
    }

    [UnscopedRef]
    public Span<TextureFormat> RtvSpan => ((Span<TextureFormat>)Rtvs)[..RtvCount];

    [UnscopedRef]
    public readonly ReadOnlySpan<TextureFormat> RtvReadonlySpan => ((ReadOnlySpan<TextureFormat>)Rtvs)[..RtvCount];

    [InlineArray(8)]
    public struct RtvArray
    {
        private TextureFormat _;
    }

    public readonly bool Equals(PipelineStateIndex other)
    {
        return Dsv == other.Dsv && RtvCount == other.RtvCount && RtvReadonlySpan.SequenceEqual(other.RtvReadonlySpan);
    }

    public readonly override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Dsv);
        hash.Add(RtvCount);
        foreach (var rtv in RtvReadonlySpan)
        {
            hash.Add(rtv);
        }
        return hash.ToHashCode();
    }

    public override string ToString() => $"{{ Dsv = {Dsv}, Rtvs = [{string.Join(", ", RtvReadonlySpan.ToArray())}] }}";
}

[Dropping(Unmanaged = true)]
public abstract unsafe partial class ShaderPipeline
{
    internal abstract ID3D12PipelineState* RawPtr { get; }
}

[Dropping(Unmanaged = true)]
public sealed unsafe partial class GraphicsShaderPipeline : ShaderPipeline
{
    internal FGraphicsShaderPipeline* m_ptr;
    internal readonly GraphicsPipelineState* m_state;

    public ref readonly GraphicsPipelineState State => ref *m_state;

    internal GraphicsShaderPipeline(ShaderPass pass) : this(pass, in pass.States, new(in pass.States)) { }
    internal GraphicsShaderPipeline(ShaderPass pass, PipelineStateIndex index) : this(pass, in pass.States, index) { }
    internal GraphicsShaderPipeline(ShaderPass pass, in ShaderStateModel States, PipelineStateIndex index)
    {
        if ((pass.Stages & ShaderStages.Cs) != 0)
            throw new ArgumentException("The graphics pipeline does not support the compute stage");
        if ((pass.Stages & ShaderStages.Ps) == 0 || pass.Ps is null)
            throw new ArgumentException("Missing pixel stage");
        if ((pass.Stages & (ShaderStages.Vs | ShaderStages.Ms)) == 0 || pass is { Vs: null, Ms: null })
            throw new ArgumentException("Missing vertex or mesh stage");

        FShaderPassData pass_data;
        m_ptr = CreateStage0(&pass_data, pass, in States);

        GraphicsPipelineState* p_state;
        m_ptr->StatePtr(&p_state).TryThrow();
        m_state = p_state;

        if (App.Vars.debug)
            Log.Debug("Created pipeline state of shader pass [\"{Shader}\"::\"{Pass}\"] with {State}",
                pass.Shader.Path, pass.Name, index);

        // set state
        static FGraphicsShaderPipeline* CreateStage0(FShaderPassData* pass_data, ShaderPass pass,
            in ShaderStateModel States)
        {
            ref readonly var states = ref States;
            var state = &pass_data->state;
            *state = new();

            // todo other state

            state->rasterizer_state.fill_mode = states.Fill;
            state->rasterizer_state.cull_mode = states.Cull;

            // todo other state

            state->primitive_topology = states.Topology;

            return CreateStage1(pass_data, pass);
        }

        // set modules
        static FGraphicsShaderPipeline* CreateStage1(FShaderPassData* pass_data, ShaderPass pass)
        {
            pass_data->stages = Unsafe.BitCast<ShaderStages, FShaderStages>(pass.Stages);
            fixed (byte* ps_blob = pass.Ps!.Blob)
            {
                pass_data->modules[0] = new(ps_blob, (nuint)pass.Ps.Blob.Length);
                if (pass is { Vs: { } vs })
                {
                    fixed (byte* vs_blob = vs.Blob)
                    {
                        pass_data->modules[1] = new(vs_blob, (nuint)vs.Blob.Length);
                        return CreateStage2(pass_data);
                    }
                }
                if (pass is { Ms: { } ms })
                {
                    fixed (byte* ms_blob = ms.Blob)
                    {
                        pass_data->modules[1] = new(ms_blob, (nuint)ms.Blob.Length);

                        if (pass is { Ts: { } ts })
                        {
                            fixed (byte* ts_blob = ts.Blob)
                            {
                                pass_data->modules[2] = new(ts_blob, (nuint)ts.Blob.Length);
                                return CreateStage2(pass_data);
                            }
                        }
                        return CreateStage2(pass_data);
                    }
                }
                throw new UnreachableException();
            }
        }

        // do create
        static FGraphicsShaderPipeline* CreateStage2(FShaderPassData* pass_data)
        {
            FGraphicsShaderPipeline* ptr;
            App.Rendering.m_ptr->CreateGraphicsShaderPipeline(pass_data, &ptr).TryThrow();
            return ptr;
        }
    }

    [Drop]
    private void Drop()
    {
        if (ExchangeUtils.ExchangePtr(ref m_ptr, null, out var ptr) is null) return;
        ptr->Release();
    }

    internal override ID3D12PipelineState* RawPtr
    {
        get
        {
            ID3D12PipelineState* ptr;
            m_ptr->RawPtr((void**)&ptr).TryThrow();
            return ptr;
        }
    }
}
