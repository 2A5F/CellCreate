using System.Runtime.CompilerServices;
using Game.Rendering;

namespace Game.Native;

public partial struct RtBlendState
{
    public RtBlendState()
    {
        blend = Switch.On;
        src_blend = BlendType.One;
        dst_blend = BlendType.Zero;
        blend_op = BlendOp.Add;
        src_alpha_blend = BlendType.One;
        dst_alpha_blend = BlendType.One;
        alpha_blend_op = BlendOp.Max;
        logic_op = LogicOp.None;
        write_mask = Unsafe.BitCast<ColorMask, FColorMask>(ColorMask.RGB);
    }
}

public partial struct BlendState
{
    public BlendState()
    {
        foreach (ref var rt in rts)
        {
            rt = new();
        }
    }
}

public partial struct RasterizerState
{
    public RasterizerState()
    {
        fill_mode = FillMode.Solid;
        cull_mode = CullMode.Back;
        depth_clip = Switch.On;
    }
}

public partial struct StencilState
{
    public StencilState()
    {
        fail_op = StencilFailOp.Keep;
        depth_fail_op = StencilFailOp.Keep;
        pass_op = StencilFailOp.Keep;
        func = CmpFunc.Always;
    }
}

public partial struct DepthStencilState
{
    public DepthStencilState()
    {
        depth_func = CmpFunc.Off;
        depth_write_mask = DepthWriteMask.All;
        stencil_read_mask = 0xff;
        stencil_write_mask = 0xff;
        front_face = new();
        back_face = new();
    }
}

public partial struct SampleState
{
    public SampleState()
    {
        count = 1;
    }
}

public partial struct GraphicsPipelineState
{
    public GraphicsPipelineState()
    {
        blend_state = new();
        primitive_topology = PrimitiveTopologyType.Triangle;
        rasterizer_state = new();
        depth_stencil_state = new();
        sample_mask = 0xffffffff;
        rt_count = 1;
        foreach (ref var rtv_format in rtv_formats)
        {
            rtv_format = TextureFormat.R8G8B8A8_UNorm;
        }
        dsv_format = TextureFormat.D24_UNorm_S8_UInt;
        sample_state = new();
    }
}
