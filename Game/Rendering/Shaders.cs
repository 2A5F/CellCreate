using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Coplt.ShaderReflections;
using Game.Rendering.Contents;
using Serilog;
using Serilog.Core;
using ShaderStage = Game.Native.ShaderStage;

namespace Game.Rendering;

public record Shader(string Path, Guid Id)
{
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

public record ShaderPass(string Name, ShaderStages Stages)
{
    public Shader Shader { get; internal set; } = null!;
    internal ShaderStateModel States;
    public int Index { get; internal set; }
    public ShaderModule? Cs { get; internal set; }
    public ShaderModule? Ps { get; internal set; }
    public ShaderModule? Vs { get; internal set; }
    public ShaderModule? Ms { get; internal set; }
    public ShaderModule? Ts { get; internal set; }

    public override string ToString() => $"ShaderPass[{Name}]";

    #region Equals

    public virtual bool Equals(ShaderPass? other) => ReferenceEquals(this, other);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    #endregion

    internal Dictionary<PipelineStateIndex, ShaderPipeline> PipelineStates { get; } = new();

    public ShaderPipeline GetOrCreatePipelineState(PipelineStateIndex index)
    {
        if (PipelineStates.TryGetValue(index, out var pipelineState)) return pipelineState;
        if ((Stages & ShaderStages.Cs) != 0) throw new NotImplementedException("todo");
        else pipelineState = new GraphicsShaderPipeline(this);
        PipelineStates[index] = pipelineState;
        return pipelineState;
    }
}

public record ShaderModule(ShaderStage Stage, byte[] Blob, ShaderMeta Meta)
{
    public override string ToString() => $"ShaderModule[{Stage}]";

    #region Equals

    public virtual bool Equals(ShaderModule? other) => ReferenceEquals(this, other);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    #endregion
}

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

        var r = new Shader(path, id) { Passes = passes, NameToPass = by_name };
        
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

        var r = new ShaderPass(name, stages) { States = pass.States };

        foreach (var module in modules)
        {
            switch (module.Stage)
            {
                case ShaderStage.Cs:
                    r.Cs = module;
                    break;
                case ShaderStage.Ps:
                    r.Ps = module;
                    break;
                case ShaderStage.Vs:
                    r.Vs = module;
                    break;
                case ShaderStage.Ms:
                    r.Ms = module;
                    break;
                case ShaderStage.Ts:
                    r.Ts = module;
                    break;
            }
        }

        return r;
    }

    internal static async Task<ShaderModule> LoadShaderModule(string shader_path, string path, string name,
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

        return new ShaderModule(stage, blob, re!);
    }
}
