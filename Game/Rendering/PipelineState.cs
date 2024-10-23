using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Coplt.Dropping;
using Game.Native;
using Serilog;
using Silk.NET.Direct3D12;

namespace Game.Rendering;

public record struct PipelineRtOverride
{
    internal readonly GraphicsPipelineFormatOverride m_native;

    public readonly TextureFormat Dsv => m_native.dsv_format;
    [UnscopedRef]
    public readonly ReadOnlySpan<TextureFormat> Rtvs =>
        ((ReadOnlySpan<TextureFormat>)m_native.rtv_formats)[..m_native.rt_count];

    public PipelineRtOverride() : this(TextureFormat.D24_UNorm_S8_UInt, [TextureFormat.R8G8B8A8_UNorm]) { }

    public PipelineRtOverride(ShaderPass pass) : this()
    {
        m_native.dsv_format = pass.State.dsv_format;
        m_native.rt_count = pass.State.rt_count;
        Unsafe.SkipInit(out m_native.rtv_formats);
        ((ReadOnlySpan<TextureFormat>)pass.State.rtv_formats)[..pass.State.rt_count].CopyTo(m_native.rtv_formats);
    }

    public PipelineRtOverride(TextureFormat dsv, ReadOnlySpan<TextureFormat> rtvs)
    {
        m_native.dsv_format = dsv;
        m_native.rt_count = rtvs.Length;
        Unsafe.SkipInit(out m_native.rtv_formats);
        rtvs.CopyTo(m_native.rtv_formats);
    }

    public readonly bool Equals(PipelineRtOverride other)
    {
        return Dsv == other.Dsv && Rtvs.Length == other.Rtvs.Length && Rtvs.SequenceEqual(other.Rtvs);
    }

    public readonly override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Dsv);
        hash.Add(Rtvs.Length);
        foreach (var rtv in Rtvs)
        {
            hash.Add(rtv);
        }
        return hash.ToHashCode();
    }

    public override string ToString() => $"{{ Dsv = {Dsv}, Rtvs = [{string.Join(", ", Rtvs.ToArray())}] }}";
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

    internal GraphicsShaderPipeline(ShaderPass pass) : this(pass, false, in Unsafe.NullRef<PipelineRtOverride>()) { }
    internal GraphicsShaderPipeline(ShaderPass pass, in PipelineRtOverride rtOverride) :
        this(pass, true, in rtOverride) { }
    internal GraphicsShaderPipeline(ShaderPass pass, bool override_format, in PipelineRtOverride rtOverride)
    {
        if ((pass.Stages & ShaderStages.Cs) != 0)
            throw new ArgumentException("The graphics pipeline does not support the compute stage");
        if ((pass.Stages & ShaderStages.Ps) == 0)
            throw new ArgumentException("Missing pixel stage");
        if ((pass.Stages & (ShaderStages.Vs | ShaderStages.Ms)) == 0)
            throw new ArgumentException("Missing vertex or mesh stage");

        FGraphicsShaderPipeline* ptr;
        fixed (PipelineRtOverride* p_rtInfo = &rtOverride)
        {
            App.Rendering.m_ptr->CreateGraphicsShaderPipeline(
                pass.m_data, override_format ? &p_rtInfo->m_native : null, &ptr
            ).TryThrow();
        }
        m_ptr = ptr;
        GraphicsPipelineState* p_state;
        m_ptr->StatePtr(&p_state).TryThrow();
        m_state = p_state;

        if (App.Vars.debug)
            Log.Debug("Created pipeline state of shader pass [\"{Shader}\"::\"{Pass}\"] with {RtInfo}",
                pass.Shader.Path, pass.Name, rtOverride);
    }

    internal GraphicsShaderPipeline(ShaderPass pass, in PipelineRtOverride rtOverride, FGraphicsShaderPipeline* ptr)
    {
        m_ptr = ptr;
        GraphicsPipelineState* p_state;
        m_ptr->StatePtr(&p_state).TryThrow();
        m_state = p_state;

        if (App.Vars.debug)
            Log.Debug("Created pipeline state of shader pass [\"{Shader}\"::\"{Pass}\"] with {RtInfo}",
                pass.Shader.Path, pass.Name, rtOverride);
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
