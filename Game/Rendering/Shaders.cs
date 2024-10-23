using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Coplt.Dropping;
using Coplt.ShaderReflections;
using Game.Native;
using Game.Rendering.Contents;
using Serilog;
using Serilog.Core;
using ShaderStage = Game.Native.ShaderStage;

namespace Game.Rendering;

public record Shader(string Path, Guid Id)
{
    public required RenderingManager Rendering { get; init; }
    internal ShaderPass[] Passes { get; init; } = null!;
    internal FrozenDictionary<string, ShaderPass> NameToPass { get; init; } = null!;

    public ShaderPass this[int index] => Passes[index];
    public ShaderPass this[string name] => NameToPass[name];

    public int NameToIndex(string name) => NameToPass[name].Index;

    public override string ToString() => $"Shader[{Path}]";

    #region Equals

    public virtual bool Equals(Shader? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();

    #endregion
}

[Dropping(Unmanaged = true)]
public unsafe partial class ShaderPass
{
    public Shader Shader { get; internal set; } = null!;
    public int Index { get; internal set; }
    public string Name { get; }

    internal FShaderPass* m_ptr;
    internal FShaderPassData* m_data;

    public ref readonly GraphicsPipelineState State => ref m_data->state;
    public ShaderStages Stages => Unsafe.BitCast<FShaderStages, ShaderStages>(m_data->stages);

    internal ShaderPass(in ShaderPassTmp tmp)
    {
        Name = tmp.Name;
        FShaderPassData pass_data;
        m_ptr = CreateStage0(&pass_data, in tmp, in tmp.States);
        FShaderPassData* p_data;
        m_ptr->DataPtr(&p_data).TryThrow();
        m_data = p_data;

        // set state
        static FShaderPass* CreateStage0(
            FShaderPassData* pass_data, in ShaderPassTmp pass, in ShaderStateModel States
        )
        {
            ref readonly var states = ref States;
            var state = &pass_data->state;
            *state = new();

            // todo other state

            state->rasterizer_state.fill_mode = states.Fill;
            state->rasterizer_state.cull_mode = states.Cull;

            // todo other state

            state->primitive_topology = states.Topology;

            return CreateStage1(pass_data, in pass);
        }

        // set modules
        static FShaderPass* CreateStage1(
            FShaderPassData* pass_data, in ShaderPassTmp pass
        )
        {
            pass_data->stages = Unsafe.BitCast<ShaderStages, FShaderStages>(pass.Stages);
            if (pass is { Cs: { } cs })
            {
                fixed (byte* cs_blob = cs.Blob)
                {
                    pass_data->modules[0] = new(cs_blob, (nuint)cs.Blob.Length);
                    return CreateStage2(pass_data);
                }
            }
            else if (pass is { Ps: { } ps })
            {
                fixed (byte* ps_blob = ps.Blob)
                {
                    pass_data->modules[0] = new(ps_blob, (nuint)ps.Blob.Length);
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
                }
            }
            throw new NotSupportedException("Invalid shader pass");
        }

        // do create
        static FShaderPass* CreateStage2(FShaderPassData* pass_data)
        {
            FShaderPass* ptr;
            App.Rendering.m_ptr->CreateShaderPass(pass_data, &ptr).TryThrow();
            return ptr;
        }
    }

    [Drop]
    private void Drop()
    {
        if (ExchangeUtils.ExchangePtr(ref m_ptr, null, out var ptr) is null) return;
        ptr->Release();
    }

    #region Todo : remove

    internal ConcurrentDictionary<PipelineRtOverride, ShaderPipeline> PipelineStates { get; } = new();

    public ShaderPipeline GetOrCreateGraphicsShaderPipeline(PipelineRtOverride rtOverride)
    {
        return PipelineStates.GetOrAdd(rtOverride, static (rtOverride, pass) =>
        {
            FGraphicsShaderPipeline* ptr;
            pass.m_ptr->GetOrCreateGraphicsPipeline(&rtOverride.m_native, &ptr).TryThrow();
            return new GraphicsShaderPipeline(pass, rtOverride, ptr);
        }, this);
    }

    #endregion
}

internal record struct ShaderPassTmp(string Name, ShaderStages Stages)
{
    internal ShaderStateModel States;
    public ShaderModuleTmp? Cs { get; internal set; }
    public ShaderModuleTmp? Ps { get; internal set; }
    public ShaderModuleTmp? Vs { get; internal set; }
    public ShaderModuleTmp? Ms { get; internal set; }
    public ShaderModuleTmp? Ts { get; internal set; }
}

internal record ShaderModuleTmp(ShaderStage Stage, byte[] Blob, ShaderMeta Meta);

public static class Shaders
{
    internal static FrozenDictionary<Guid, Shader> s_id_to_shader = null!;
    internal static FrozenDictionary<string, Shader> s_path_to_shader = null!;

    public static Shader? TryGetShader(Guid id) => s_id_to_shader.TryGetValue(id, out var r) ? r : null;
    public static Shader? TryGetShader(string path) => s_path_to_shader.TryGetValue(path, out var r) ? r : null;

    internal static async ValueTask LoadShaders()
    {
        Log.Information("Loading shaders...");
        var exe_path = Environment.GetCommandLineArgs()[0];
        var shader_root = Path.GetFullPath(Path.Combine(exe_path, "..", "content", "shaders"));
        if (App.Vars.debug) Log.Debug("Shader root path: {ShaderRootPath}", shader_root);
        var manifest_path = Path.Combine(shader_root, ".manifest");
        if (App.Vars.debug) Log.Debug("Shader manifest path: {ShaderManifestPath}", manifest_path);

        Dictionary<string, Guid>? manifest;
        {
            await using var manifest_stream = File.OpenRead(manifest_path);
            manifest = await JsonSerializer.DeserializeAsync<Dictionary<string, Guid>>(manifest_stream);
        }
        if (App.Vars.debug) Log.Debug("Shader manifest: {ShaderManifest}", manifest);

        var shaders = await Task.WhenAll(manifest!.Select(kv => LoadShader(shader_root, kv.Key, kv.Value)));

        s_id_to_shader = shaders.ToFrozenDictionary(static a => a.Id, a => a);
        s_path_to_shader = shaders.ToFrozenDictionary(static a => a.Path, a => a);

        Log.Information("{Num} shaders loaded", shaders.Length);
    }

    internal static readonly JsonSerializerOptions MetaDeserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    internal static readonly JsonSerializerOptions ReflectionDeserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    internal static async Task<Shader> LoadShader(string shader_root, string path, Guid id)
    {
        var shader_path = Path.Combine(shader_root, $"{id:D}");
        var shader_peta_path = Path.Combine(shader_path, ".meta");
        if (App.Vars.debug) Log.Debug("Shader [\"{Shader}\"] meta path: {ShaderMetaPath}", path, shader_peta_path);

        ShaderMetaModel meta;
        {
            await using var meta_stream = File.OpenRead(shader_peta_path);
            meta = await JsonSerializer.DeserializeAsync<ShaderMetaModel>(meta_stream, MetaDeserializeOptions);
        }
        if (App.Vars.debug) Log.Debug("Shader [\"{Shader}\"] meta: {ShaderMeta}", path, meta);

        var passes = await Task.WhenAll(meta.Passes.Select(kv => LoadShaderPass(shader_path, path, kv.Key, kv.Value)));
        var by_name = passes.ToFrozenDictionary(static a => a.Name, static a => a);

        foreach (var (pass, index) in passes.Select(static (a, b) => (a, b)))
        {
            pass.Index = index;
        }

        var r = new Shader(path, id)
        {
            Rendering = App.Rendering,
            Passes = passes,
            NameToPass = by_name,
        };

        foreach (var pass in passes)
        {
            pass.Shader = r;
        }

        Log.Information("Shader [\"{Shader}\"] loaded", path);

        return r;
    }

    internal static async Task<ShaderPass> LoadShaderPass(string shader_path, string path, string name,
        ShaderPassModel pass)
    {
        var modules = await Task.WhenAll(pass.Stages.Select(stage => LoadShaderModule(shader_path, path, name, stage)));

        ShaderStages stages = default;

        foreach (var module in modules)
        {
            stages |= module.Stage switch
            {
                ShaderStage.Cs => ShaderStages.Cs,
                ShaderStage.Ps => ShaderStages.Ps,
                ShaderStage.Vs => ShaderStages.Vs,
                ShaderStage.Ms => ShaderStages.Ms,
                ShaderStage.Ts => ShaderStages.Ts,
                _ => default,
            };
        }

        var tmp = new ShaderPassTmp(name, stages) { States = pass.States };

        foreach (var module in modules)
        {
            switch (module.Stage)
            {
                case ShaderStage.Cs:
                    tmp.Cs = module;
                    break;
                case ShaderStage.Ps:
                    tmp.Ps = module;
                    break;
                case ShaderStage.Vs:
                    tmp.Vs = module;
                    break;
                case ShaderStage.Ms:
                    tmp.Ms = module;
                    break;
                case ShaderStage.Ts:
                    tmp.Ts = module;
                    break;
            }
        }

        return new ShaderPass(in tmp);
    }

    internal static async Task<ShaderModuleTmp> LoadShaderModule(string shader_path, string path, string name,
        ShaderStage stage)
    {
        var module_path = Path.Combine(shader_path, $"{name}.{stage}.dxil");
        var re_path = Path.Combine(shader_path, $"{name}.{stage}.re");
        if (App.Vars.debug)
            Log.Debug("Shader Module [\"{Shader}\"::\"{Name}\"::{Stage}] path: {Path}", path, name, stage, module_path);

        var blob = await File.ReadAllBytesAsync(module_path);

        ShaderMeta? re;
        {
            await using var re_stream = File.OpenRead(re_path);
            re = await JsonSerializer.DeserializeAsync<ShaderMeta>(re_stream, ReflectionDeserializeOptions);
        }

        return new ShaderModuleTmp(stage, blob, re!);
    }
}
