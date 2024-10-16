using System.Text.Json.Serialization;
using Coplt.ShaderReflections;
using Game.Native;
using ShaderStage = Game.Native.ShaderStage;

namespace Game.Rendering.Contents;

internal record struct ShaderMetaModel()
{
    public required Dictionary<string, ShaderPassModel> Passes { get; set; } = null!;

    public override string ToString() =>
        $"{{ Passes: {{ {string.Join(", ", Passes.Select(static kv => $"\"{kv.Key}\": {kv.Value}"))} }} }}";
}

internal record struct ShaderPassModel()
{
    public required ShaderStage[] Stages { get; set; } = null!;
    public required ShaderStateModel States { get; set; }

    public override string ToString() => $"{{ Stages: [{string.Join(", ", Stages)}], States: {States} }}";
}

internal record struct ShaderStateModel()
{
    public object Blend { get; set; } // todo parser
    public object BlendOp { get; set; } // todo parser
    public object ColorMask { get; set; } // todo parser
    public FillMode Fill { get; set; } = FillMode.Solid;
    public CullMode Cull { get; set; } = CullMode.Back;
    public PrimitiveTopologyType Topology { get; set; } = PrimitiveTopologyType.Triangle;
    
    // todo other

    public override string ToString() =>
        $"{{ Blend: {Blend}, BlendOp: {BlendOp}, ColorMask: {ColorMask}, Topology: {Topology}, Fill: {Fill}, Cull: {Cull} }}";
}
