#include "gpu_convert.h"

using namespace cc;

D3D12_BLEND cc::to_dx(const BlendType type)
{
    switch (type)
    {
    case BlendType::Zero:
        return D3D12_BLEND_ZERO;
    case BlendType::One:
        return D3D12_BLEND_ONE;
    case BlendType::SrcColor:
        return D3D12_BLEND_SRC_COLOR;
    case BlendType::InvSrcColor:
        return D3D12_BLEND_INV_SRC_COLOR;
    case BlendType::SrcAlpha:
        return D3D12_BLEND_SRC_ALPHA;
    case BlendType::InvSrcAlpha:
        return D3D12_BLEND_INV_SRC_ALPHA;
    case BlendType::DstAlpha:
        return D3D12_BLEND_DEST_ALPHA;
    case BlendType::InvDstAlpha:
        return D3D12_BLEND_INV_DEST_ALPHA;
    case BlendType::DstColor:
        return D3D12_BLEND_DEST_COLOR;
    case BlendType::InvDstColor:
        return D3D12_BLEND_INV_DEST_COLOR;
    case BlendType::SrcAlphaSat:
        return D3D12_BLEND_SRC_ALPHA_SAT;
    case BlendType::BlendFactor:
        return D3D12_BLEND_BLEND_FACTOR;
    case BlendType::BlendInvBlendFactor:
        return D3D12_BLEND_INV_BLEND_FACTOR;
    case BlendType::Src1Color:
        return D3D12_BLEND_SRC1_ALPHA;
    case BlendType::InvSrc1Color:
        return D3D12_BLEND_INV_SRC1_ALPHA;
    case BlendType::Src1Alpha:
        return D3D12_BLEND_SRC1_ALPHA;
    case BlendType::InvSrc1Alpha:
        return D3D12_BLEND_INV_SRC1_ALPHA;
    case BlendType::AlphaFactor:
        return D3D12_BLEND_ALPHA_FACTOR;
    case BlendType::InvAlphaFactor:
        return D3D12_BLEND_INV_ALPHA_FACTOR;
    default:
        return D3D12_BLEND_ZERO;
    }
}

D3D12_BLEND_OP cc::to_dx(const BlendOp op)
{
    switch (op)
    {
    case BlendOp::Add:
        return D3D12_BLEND_OP_ADD;
    case BlendOp::Sub:
        return D3D12_BLEND_OP_SUBTRACT;
    case BlendOp::RevSub:
        return D3D12_BLEND_OP_REV_SUBTRACT;
    case BlendOp::Min:
        return D3D12_BLEND_OP_MIN;
    case BlendOp::Max:
        return D3D12_BLEND_OP_MAX;
    default:
        return D3D12_BLEND_OP_ADD;
    }
}

D3D12_LOGIC_OP cc::to_dx(const LogicOp op)
{
    switch (op)
    {
    case LogicOp::Clear:
        return D3D12_LOGIC_OP_CLEAR;
    case LogicOp::One:
        return D3D12_LOGIC_OP_SET;
    case LogicOp::Copy:
        return D3D12_LOGIC_OP_COPY;
    case LogicOp::CopyInv:
        return D3D12_LOGIC_OP_COPY_INVERTED;
    case LogicOp::Noop:
        return D3D12_LOGIC_OP_NOOP;
    case LogicOp::Inv:
        return D3D12_LOGIC_OP_INVERT;
    case LogicOp::And:
        return D3D12_LOGIC_OP_AND;
    case LogicOp::NAnd:
        return D3D12_LOGIC_OP_NAND;
    case LogicOp::Or:
        return D3D12_LOGIC_OP_OR;
    case LogicOp::Nor:
        return D3D12_LOGIC_OP_NOR;
    case LogicOp::Xor:
        return D3D12_LOGIC_OP_XOR;
    case LogicOp::Equiv:
        return D3D12_LOGIC_OP_EQUIV;
    case LogicOp::AndRev:
        return D3D12_LOGIC_OP_AND_REVERSE;
    case LogicOp::AndInv:
        return D3D12_LOGIC_OP_AND_INVERTED;
    case LogicOp::OrRev:
        return D3D12_LOGIC_OP_OR_REVERSE;
    case LogicOp::OrInv:
        return D3D12_LOGIC_OP_INVERT;
    default:
        return D3D12_LOGIC_OP_CLEAR;
    }
}

UINT8 cc::to_dx(const FColorMask mask)
{
    return *reinterpret_cast<const UINT8*>(&mask);
}

void cc::to_dx(D3D12_BLEND_DESC& desc, const BlendState& state, const int32_t rt_count)
{
    desc.AlphaToCoverageEnable = state.alpha_to_coverage == Switch::On;
    desc.IndependentBlendEnable = state.independent_blend == Switch::On;
    for (int i = 0; i < rt_count; ++i)
    {
        auto& dst = desc.RenderTarget[i];
        const auto& src = state.rts[i];
        if (src.blend != Switch::On) continue;
        dst.BlendEnable = true;
        dst.SrcBlend = cc::to_dx(src.src_blend);
        dst.DestBlend = cc::to_dx(src.dst_blend);
        dst.BlendOp = cc::to_dx(src.blend_op);
        dst.SrcBlendAlpha = cc::to_dx(src.src_alpha_blend);
        dst.DestBlendAlpha = cc::to_dx(src.dst_alpha_blend);
        dst.BlendOpAlpha = cc::to_dx(src.alpha_blend_op);
        dst.LogicOpEnable = src.logic_op != LogicOp::None;
        if (dst.LogicOpEnable) dst.LogicOp = cc::to_dx(src.logic_op);
        dst.RenderTargetWriteMask = cc::to_dx(src.write_mask);
    }
}

D3D12_FILL_MODE cc::to_dx(const FillMode mode)
{
    switch (mode)
    {
    case FillMode::WireFrame:
        return D3D12_FILL_MODE_WIREFRAME;
    case FillMode::Solid:
    default:
        return D3D12_FILL_MODE_SOLID;
    }
}

D3D12_CULL_MODE cc::to_dx(const CullMode mode)
{
    switch (mode)
    {
    case CullMode::Off:
        return D3D12_CULL_MODE_NONE;
    case CullMode::Front:
        return D3D12_CULL_MODE_FRONT;
    case CullMode::Back:
    default:
        return D3D12_CULL_MODE_BACK;
    }
}

void cc::to_dx(D3D12_RASTERIZER_DESC& desc, const RasterizerState& state)
{
    desc.FillMode = cc::to_dx(state.fill_mode);
    desc.CullMode = cc::to_dx(state.cull_mode);
    desc.DepthClipEnable = state.depth_clip == Switch::On;
    desc.MultisampleEnable = state.multisample == Switch::On;
    desc.ForcedSampleCount = state.forced_sample_count;
    desc.DepthBias = state.depth_bias;
    desc.DepthBiasClamp = state.depth_bias_clamp;
    desc.SlopeScaledDepthBias = state.slope_scaled_depth_bias;
    desc.AntialiasedLineEnable = state.aa_line == Switch::On;
    desc.ConservativeRaster = state.conservative == Switch::On
        ? D3D12_CONSERVATIVE_RASTERIZATION_MODE_ON
        : D3D12_CONSERVATIVE_RASTERIZATION_MODE_OFF;
}

D3D12_COMPARISON_FUNC cc::to_dx(const CmpFunc func)
{
    switch (func)
    {
    case CmpFunc::Less:
        return D3D12_COMPARISON_FUNC_LESS;
    case CmpFunc::Equal:
        return D3D12_COMPARISON_FUNC_EQUAL;
    case CmpFunc::LessEqual:
        return D3D12_COMPARISON_FUNC_LESS_EQUAL;
    case CmpFunc::Greater:
        return D3D12_COMPARISON_FUNC_GREATER;
    case CmpFunc::NotEqual:
        return D3D12_COMPARISON_FUNC_NOT_EQUAL;
    case CmpFunc::GreaterEqual:
        return D3D12_COMPARISON_FUNC_GREATER_EQUAL;
    case CmpFunc::Always:
        return D3D12_COMPARISON_FUNC_ALWAYS;
    default:
    case CmpFunc::Never:
        return D3D12_COMPARISON_FUNC_NEVER;
    }
}

D3D12_STENCIL_OP cc::to_dx(const StencilFailOp op)
{
    switch (op)
    {
    case StencilFailOp::Keep:
        return D3D12_STENCIL_OP_KEEP;
    case StencilFailOp::Zero:
        return D3D12_STENCIL_OP_ZERO;
    case StencilFailOp::Replace:
        return D3D12_STENCIL_OP_REPLACE;
    case StencilFailOp::IncSat:
        return D3D12_STENCIL_OP_INCR_SAT;
    case StencilFailOp::DecSat:
        return D3D12_STENCIL_OP_DECR_SAT;
    case StencilFailOp::Invert:
        return D3D12_STENCIL_OP_INVERT;
    case StencilFailOp::Inc:
        return D3D12_STENCIL_OP_INCR;
    case StencilFailOp::Dec:
        return D3D12_STENCIL_OP_DECR;
    default:
        return D3D12_STENCIL_OP_KEEP;
    }
}

void cc::to_dx(D3D12_DEPTH_STENCILOP_DESC& desc, const StencilState& state)
{
    desc.StencilPassOp = cc::to_dx(state.pass_op);
    desc.StencilFailOp = cc::to_dx(state.fail_op);
    desc.StencilDepthFailOp = cc::to_dx(state.depth_fail_op);
    desc.StencilFunc = cc::to_dx(state.func);
}

void cc::to_dx(D3D12_DEPTH_STENCIL_DESC& desc, const DepthStencilState& state)
{
    desc.DepthEnable = state.depth_func != CmpFunc::Off;
    desc.DepthWriteMask = state.depth_write_mask == DepthWriteMask::All
        ? D3D12_DEPTH_WRITE_MASK_ALL
        : D3D12_DEPTH_WRITE_MASK_ZERO;
    desc.DepthFunc = cc::to_dx(state.depth_func);
    desc.StencilEnable = state.stencil_enable == Switch::On;
    desc.StencilReadMask = state.stencil_read_mask;
    desc.StencilWriteMask = state.stencil_write_mask;
    cc::to_dx(desc.FrontFace, state.front_face);
    cc::to_dx(desc.BackFace, state.back_face);
}

D3D12_PRIMITIVE_TOPOLOGY_TYPE cc::to_dx_t(const PrimitiveTopologyType topology)
{
    switch (topology)
    {
    case PrimitiveTopologyType::Triangle:
    case PrimitiveTopologyType::TriangleStrip:
        return D3D12_PRIMITIVE_TOPOLOGY_TYPE_TRIANGLE;
    case PrimitiveTopologyType::Point:
        return D3D12_PRIMITIVE_TOPOLOGY_TYPE_POINT;
    case PrimitiveTopologyType::Line:
    case PrimitiveTopologyType::LineStrip:
        return D3D12_PRIMITIVE_TOPOLOGY_TYPE_LINE;
    default:
        return D3D12_PRIMITIVE_TOPOLOGY_TYPE_UNDEFINED;
    }
}

D3D12_PRIMITIVE_TOPOLOGY cc::to_dx(const PrimitiveTopologyType topology)
{
    switch (topology)
    {
    case PrimitiveTopologyType::Triangle:
        return D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST;
    case PrimitiveTopologyType::TriangleStrip:
        return D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP;
    case PrimitiveTopologyType::Point:
        return D3D_PRIMITIVE_TOPOLOGY_POINTLIST;
    case PrimitiveTopologyType::Line:
        return D3D_PRIMITIVE_TOPOLOGY_LINELIST;
    case PrimitiveTopologyType::LineStrip:
        return D3D_PRIMITIVE_TOPOLOGY_LINESTRIP;
    default:
        return D3D_PRIMITIVE_TOPOLOGY_UNDEFINED;
    }
}

DXGI_FORMAT cc::to_dx(const TextureFormat format)
{
    switch (format)
    {
    case TextureFormat::R32G32B32A32_TypeLess:
        return DXGI_FORMAT_R32G32B32A32_TYPELESS;
    case TextureFormat::R32G32B32A32_Float:
        return DXGI_FORMAT_R32G32B32A32_FLOAT;
    case TextureFormat::R32G32B32A32_UInt:
        return DXGI_FORMAT_R32G32B32A32_UINT;
    case TextureFormat::R32G32B32A32_SInt:
        return DXGI_FORMAT_R32G32B32A32_SINT;
    case TextureFormat::R32G32B32_TypeLess:
        return DXGI_FORMAT_R32G32B32_TYPELESS;
    case TextureFormat::R32G32B32_Float:
        return DXGI_FORMAT_R32G32B32_FLOAT;
    case TextureFormat::R32G32B32_UInt:
        return DXGI_FORMAT_R32G32B32_UINT;
    case TextureFormat::R32G32B32_SInt:
        return DXGI_FORMAT_R32G32B32_SINT;
    case TextureFormat::R16G16B16A16_TypeLess:
        return DXGI_FORMAT_R16G16B16A16_TYPELESS;
    case TextureFormat::R16G16B16A16_Float:
        return DXGI_FORMAT_R16G16B16A16_FLOAT;
    case TextureFormat::R16G16B16A16_UNorm:
        return DXGI_FORMAT_R16G16B16A16_UNORM;
    case TextureFormat::R16G16B16A16_UInt:
        return DXGI_FORMAT_R16G16B16A16_UINT;
    case TextureFormat::R16G16B16A16_SNorm:
        return DXGI_FORMAT_R16G16B16A16_SNORM;
    case TextureFormat::R16G16B16A16_SInt:
        return DXGI_FORMAT_R16G16B16A16_SINT;
    case TextureFormat::R32G32_TypeLess:
        return DXGI_FORMAT_R32G32_TYPELESS;
    case TextureFormat::R32G32_Float:
        return DXGI_FORMAT_R32G32_FLOAT;
    case TextureFormat::R32G32_UInt:
        return DXGI_FORMAT_R32G32_UINT;
    case TextureFormat::R32G32_SInt:
        return DXGI_FORMAT_R32G32_SINT;
    case TextureFormat::R32G8X24_TypeLess:
        return DXGI_FORMAT_R32G8X24_TYPELESS;
    case TextureFormat::D32_Float_S8X24_UInt:
        return DXGI_FORMAT_D32_FLOAT_S8X24_UINT;
    case TextureFormat::R32_Float_X8X24_TypeLess:
        return DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS;
    case TextureFormat::X32_TypeLess_G8X24_Float:
        return DXGI_FORMAT_X32_TYPELESS_G8X24_UINT;
    case TextureFormat::R10G10B10A2_TypeLess:
        return DXGI_FORMAT_R10G10B10A2_TYPELESS;
    case TextureFormat::R10G10B10A2_UNorm:
        return DXGI_FORMAT_R10G10B10A2_UNORM;
    case TextureFormat::R10G10B10A2_UInt:
        return DXGI_FORMAT_R10G10B10A2_UINT;
    case TextureFormat::R11G11B10_Float:
        return DXGI_FORMAT_R11G11B10_FLOAT;
    case TextureFormat::R8G8B8A8_TypeLess:
        return DXGI_FORMAT_R8G8B8A8_TYPELESS;
    case TextureFormat::R8G8B8A8_UNorm:
        return DXGI_FORMAT_R8G8B8A8_UNORM;
    case TextureFormat::R8G8B8A8_UNorm_sRGB:
        return DXGI_FORMAT_R8G8B8A8_UNORM_SRGB;
    case TextureFormat::R8G8B8A8_UInt:
        return DXGI_FORMAT_R8G8B8A8_UINT;
    case TextureFormat::R8G8B8A8_SNorm:
        return DXGI_FORMAT_R8G8B8A8_SNORM;
    case TextureFormat::R8G8B8A8_SInt:
        return DXGI_FORMAT_R8G8B8A8_SINT;
    case TextureFormat::R16G16_TypeLess:
        return DXGI_FORMAT_R16G16_TYPELESS;
    case TextureFormat::R16G16_Float:
        return DXGI_FORMAT_R16G16_FLOAT;
    case TextureFormat::R16G16_UNorm:
        return DXGI_FORMAT_R16G16_UNORM;
    case TextureFormat::R16G16_UInt:
        return DXGI_FORMAT_R16G16_UINT;
    case TextureFormat::R16G16_SNorm:
        return DXGI_FORMAT_R16G16_SNORM;
    case TextureFormat::R16G16_SInt:
        return DXGI_FORMAT_R16G16_SINT;
    case TextureFormat::R32_TypeLess:
        return DXGI_FORMAT_R32_TYPELESS;
    case TextureFormat::D32_Float:
        return DXGI_FORMAT_D32_FLOAT;
    case TextureFormat::R32_Float:
        return DXGI_FORMAT_R32_FLOAT;
    case TextureFormat::R32_UInt:
        return DXGI_FORMAT_R32_UINT;
    case TextureFormat::R32_SInt:
        return DXGI_FORMAT_R32_SINT;
    case TextureFormat::R24G8_TypeLess:
        return DXGI_FORMAT_R24G8_TYPELESS;
    case TextureFormat::D24_UNorm_S8_UInt:
        return DXGI_FORMAT_D24_UNORM_S8_UINT;
    case TextureFormat::R24_UNorm_X8_TypeLess:
        return DXGI_FORMAT_R24_UNORM_X8_TYPELESS;
    case TextureFormat::X24_TypeLess_G8_UInt:
        return DXGI_FORMAT_X24_TYPELESS_G8_UINT;
    case TextureFormat::R8G8_TypeLess:
        return DXGI_FORMAT_R8G8_TYPELESS;
    case TextureFormat::R8G8_UNorm:
        return DXGI_FORMAT_R8G8_UNORM;
    case TextureFormat::R8G8_UInt:
        return DXGI_FORMAT_R8G8_UINT;
    case TextureFormat::R8G8_SNorm:
        return DXGI_FORMAT_R8G8_SNORM;
    case TextureFormat::R8G8_SInt:
        return DXGI_FORMAT_R8G8_SINT;
    case TextureFormat::R16_TypeLess:
        return DXGI_FORMAT_R16_TYPELESS;
    case TextureFormat::R16_Float:
        return DXGI_FORMAT_R16_FLOAT;
    case TextureFormat::D16_UNorm:
        return DXGI_FORMAT_D16_UNORM;
    case TextureFormat::R16_UNorm:
        return DXGI_FORMAT_R16_UNORM;
    case TextureFormat::R16_UInt:
        return DXGI_FORMAT_R16_UINT;
    case TextureFormat::R16_SNorm:
        return DXGI_FORMAT_R16_SNORM;
    case TextureFormat::R16_SInt:
        return DXGI_FORMAT_R16_SINT;
    case TextureFormat::R8_TypeLess:
        return DXGI_FORMAT_R8_TYPELESS;
    case TextureFormat::R8_UNorm:
        return DXGI_FORMAT_R8_UNORM;
    case TextureFormat::R8_UInt:
        return DXGI_FORMAT_R8_UINT;
    case TextureFormat::R8_SNorm:
        return DXGI_FORMAT_R8_SNORM;
    case TextureFormat::R8_SInt:
        return DXGI_FORMAT_R8_SINT;
    case TextureFormat::A8_UNorm:
        return DXGI_FORMAT_A8_UNORM;
    case TextureFormat::R1_UNorm:
        return DXGI_FORMAT_R1_UNORM;
    case TextureFormat::R9G9B9E5_SharedExp:
        return DXGI_FORMAT_R9G9B9E5_SHAREDEXP;
    case TextureFormat::R8G8_B8G8_UNorm:
        return DXGI_FORMAT_R8G8_B8G8_UNORM;
    case TextureFormat::G8R8_G8B8_UNorm:
        return DXGI_FORMAT_G8R8_G8B8_UNORM;
    case TextureFormat::BC1_TypeLess:
        return DXGI_FORMAT_BC1_TYPELESS;
    case TextureFormat::BC1_UNorm:
        return DXGI_FORMAT_BC1_UNORM;
    case TextureFormat::BC1_UNorm_sRGB:
        return DXGI_FORMAT_BC1_UNORM_SRGB;
    case TextureFormat::BC2_TypeLess:
        return DXGI_FORMAT_BC2_TYPELESS;
    case TextureFormat::BC2_UNorm:
        return DXGI_FORMAT_BC2_UNORM;
    case TextureFormat::BC2_UNorm_sRGB:
        return DXGI_FORMAT_BC2_UNORM_SRGB;
    case TextureFormat::BC3_TypeLess:
        return DXGI_FORMAT_BC3_TYPELESS;
    case TextureFormat::BC3_UNorm:
        return DXGI_FORMAT_BC3_UNORM;
    case TextureFormat::BC3_UNorm_sRGB:
        return DXGI_FORMAT_BC3_UNORM_SRGB;
    case TextureFormat::BC4_TypeLess:
        return DXGI_FORMAT_BC4_TYPELESS;
    case TextureFormat::BC4_UNorm:
        return DXGI_FORMAT_BC4_UNORM;
    case TextureFormat::BC4_SNorm:
        return DXGI_FORMAT_BC4_SNORM;
    case TextureFormat::BC5_TypeLess:
        return DXGI_FORMAT_BC5_TYPELESS;
    case TextureFormat::BC5_UNorm:
        return DXGI_FORMAT_BC5_UNORM;
    case TextureFormat::BC5_SNorm:
        return DXGI_FORMAT_BC5_SNORM;
    case TextureFormat::B5G6R5_UNorm:
        return DXGI_FORMAT_B5G6R5_UNORM;
    case TextureFormat::B5G5R5A1_UNorm:
        return DXGI_FORMAT_B5G5R5A1_UNORM;
    case TextureFormat::B8G8R8A8_UNorm:
        return DXGI_FORMAT_B8G8R8A8_UNORM;
    case TextureFormat::B8G8R8X8_UNorm:
        return DXGI_FORMAT_B8G8R8X8_UNORM;
    case TextureFormat::R10G10B10_XR_Bias_A2_UNorm:
        return DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM;
    case TextureFormat::B8G8R8A8_TypeLess:
        return DXGI_FORMAT_B8G8R8A8_TYPELESS;
    case TextureFormat::B8G8R8A8_UNorm_sRGB:
        return DXGI_FORMAT_B8G8R8A8_UNORM_SRGB;
    case TextureFormat::B8G8R8X8_TypeLess:
        return DXGI_FORMAT_B8G8R8X8_TYPELESS;
    case TextureFormat::B8G8R8X8_UNorm_sRGB:
        return DXGI_FORMAT_B8G8R8X8_UNORM_SRGB;
    case TextureFormat::BC6H_TypeLess:
        return DXGI_FORMAT_BC6H_TYPELESS;
    case TextureFormat::BC6H_UF16:
        return DXGI_FORMAT_BC6H_UF16;
    case TextureFormat::BC6H_SF16:
        return DXGI_FORMAT_BC6H_SF16;
    case TextureFormat::BC7_TypeLess:
        return DXGI_FORMAT_BC7_TYPELESS;
    case TextureFormat::BC7_UNorm:
        return DXGI_FORMAT_BC7_UNORM;
    case TextureFormat::BC7_UNorm_sRGB:
        return DXGI_FORMAT_BC7_UNORM_SRGB;
    case TextureFormat::Unknown:
    default:
        return DXGI_FORMAT_UNKNOWN;
    }
}
