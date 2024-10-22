#pragma once

#include "FFI.h"
#include "TextureFormat.h"
#include "../types.h"
#include "../Object.h"

namespace cc
{
    // 语义化的 bool 值
    enum class Switch : uint8_t
    {
        /* 关闭 */
        Off = 0,
        /* 启用 */
        On = 1,
    };

    enum class PrimitiveTopologyType: uint8_t
    {
        Triangle,
        TriangleStrip,
        Point,
        Line,
        LineStrip,
    };

    struct FColorMask
    {
        uint8_t r : 1;
        uint8_t g : 1;
        uint8_t b : 1;
        uint8_t a : 1;

        bool all() const { return r && g && b && a; }

        // ReSharper disable once CppNonExplicitConvertingConstructor
        constexpr FColorMask(
            // ReSharper disable CppDFAConstantParameter
            const bool r = true, const bool g = true, const bool b = true, const bool a = true
        ) : r(r), g(g), b(b), a(a)
        {
        }
    };

    // 混合类型
    enum class BlendType : uint8_t
    {
        Zero                = 1,
        One                 = 2,
        SrcColor            = 3,
        InvSrcColor         = 4,
        SrcAlpha            = 5,
        InvSrcAlpha         = 6,
        DstAlpha            = 7,
        InvDstAlpha         = 8,
        DstColor            = 9,
        InvDstColor         = 10,
        SrcAlphaSat         = 11,
        BlendFactor         = 14,
        BlendInvBlendFactor = 15,
        Src1Color           = 16,
        InvSrc1Color        = 17,
        Src1Alpha           = 18,
        InvSrc1Alpha        = 19,
        AlphaFactor         = 20,
        InvAlphaFactor      = 21,
    };

    // 混合操作
    enum class BlendOp : uint8_t
    {
        None   = 0,
        Add    = 1,
        Sub    = 2,
        RevSub = 3,
        Min    = 4,
        Max    = 5,
    };

    // 逻辑操作
    enum class LogicOp : uint8_t
    {
        None = 0,
        Clear,
        One,
        Copy,
        CopyInv,
        Noop,
        Inv,
        And,
        NAnd,
        Or,
        Nor,
        Xor,
        Equiv,
        AndRev,
        AndInv,
        OrRev,
        OrInv,
    };

    // 填充模式
    enum class FillMode : uint8_t
    {
        /* 绘制连接顶点的线条， 不绘制相邻顶点 */
        WireFrame = 2,
        /* 填充顶点形成的三角形， 不绘制相邻顶点 */
        Solid = 3,
    };

    // 剔除模式
    enum class CullMode : uint8_t
    {
        /* 始终绘制所有三角形 */
        Off = 1,
        /* 不要绘制正面的三角形 */
        Front = 2,
        /* 不要绘制朝背的三角形 */
        Back = 3,
    };

    enum class CmpFunc : uint8_t
    {
        Off          = 0,
        Never        = 1,
        Less         = 2,
        Equal        = 3,
        LessEqual    = 4,
        Greater      = 5,
        NotEqual     = 6,
        GreaterEqual = 7,
        Always       = 8,
    };

    enum class StencilFailOp : uint8_t
    {
        Keep    = 1,
        Zero    = 2,
        Replace = 3,
        IncSat  = 4,
        DecSat  = 5,
        Invert  = 6,
        Inc     = 7,
        Dec     = 8,
    };

    enum class DepthWriteMask : uint8_t
    {
        Zero = 0,
        All  = 1,
    };

    struct RtBlendState
    {
        Switch blend{true};
        BlendType src_blend{BlendType::One};
        BlendType dst_blend{BlendType::Zero};
        BlendOp blend_op{BlendOp::Add};
        BlendType src_alpha_blend{BlendType::One};
        BlendType dst_alpha_blend{BlendType::One};
        BlendOp alpha_blend_op{BlendOp::Max};
        LogicOp logic_op{LogicOp::None};
        FColorMask write_mask{true, true, true, false};

        // 设置写入到 a 通道
        void AllowWriteAlpha()
        {
            write_mask.a = true;
        }

        // 设置为透明混合
        void UseAlphaBlend()
        {
            src_blend = BlendType::SrcAlpha;
            dst_blend = BlendType::InvSrcAlpha;
            src_alpha_blend = BlendType::SrcAlpha;
            dst_alpha_blend = BlendType::InvSrcAlpha;
            alpha_blend_op = BlendOp::Add;
        }

        // 设置为预乘混合
        void UsePreMultiplied()
        {
            src_blend = BlendType::One;
            dst_blend = BlendType::InvSrcAlpha;
            src_alpha_blend = BlendType::One;
            dst_alpha_blend = BlendType::InvSrcAlpha;
            alpha_blend_op = BlendOp::Add;
        }
    };

    struct BlendState
    {
        RtBlendState rts[8]{{}, {}, {}, {}, {}, {}, {}, {}};
        Switch alpha_to_coverage{false};
        Switch independent_blend{false};
    };

    struct RasterizerState
    {
        FillMode fill_mode{FillMode::Solid};
        CullMode cull_mode{CullMode::Back};
        Switch depth_clip{true};
        Switch multisample{false};
        uint32_t forced_sample_count{0};
        int32_t depth_bias{0};
        float depth_bias_clamp{0};
        float slope_scaled_depth_bias{0};
        Switch aa_line{false};
        Switch conservative{false};
    };

    struct StencilState
    {
        StencilFailOp fail_op{StencilFailOp::Keep};
        StencilFailOp depth_fail_op{StencilFailOp::Keep};
        StencilFailOp pass_op{StencilFailOp::Keep};
        CmpFunc func{CmpFunc::Always};
    };

    struct DepthStencilState
    {
        CmpFunc depth_func{CmpFunc::Off};
        DepthWriteMask depth_write_mask{DepthWriteMask::All};
        Switch stencil_enable{false};
        uint8_t stencil_read_mask{0xff};
        uint8_t stencil_write_mask{0xff};
        StencilState front_face{};
        StencilState back_face{};
    };

    struct SampleState
    {
        uint32_t count{1};
        int32_t quality{0};
    };

    struct GraphicsPipelineState
    {
        BlendState blend_state{};
        PrimitiveTopologyType primitive_topology{PrimitiveTopologyType::Triangle};
        RasterizerState rasterizer_state{};
        DepthStencilState depth_stencil_state{};
        uint32_t sample_mask{0xffffffff};
        int32_t rt_count{1};
        TextureFormat rtv_formats[8]{
            TextureFormat::R8G8B8A8_UNorm, TextureFormat::R8G8B8A8_UNorm,
            TextureFormat::R8G8B8A8_UNorm, TextureFormat::R8G8B8A8_UNorm,
            TextureFormat::R8G8B8A8_UNorm, TextureFormat::R8G8B8A8_UNorm,
            TextureFormat::R8G8B8A8_UNorm, TextureFormat::R8G8B8A8_UNorm,
        };
        TextureFormat dsv_format{TextureFormat::D24_UNorm_S8_UInt};
        SampleState sample_state{};
    };


    struct GraphicsPipelineFormatOverride
    {
        int32_t rt_count{1};
        TextureFormat rtv_formats[8]{
            TextureFormat::R8G8B8A8_UNorm, TextureFormat::R8G8B8A8_UNorm,
            TextureFormat::R8G8B8A8_UNorm, TextureFormat::R8G8B8A8_UNorm,
            TextureFormat::R8G8B8A8_UNorm, TextureFormat::R8G8B8A8_UNorm,
            TextureFormat::R8G8B8A8_UNorm, TextureFormat::R8G8B8A8_UNorm,
        };
        TextureFormat dsv_format{TextureFormat::D24_UNorm_S8_UInt};
    };

    enum class ShaderStage : uint8_t
    {
        Unknown,
        Cs,
        Ps,
        Vs,
        Ms,
        Ts,
    };

    struct FShaderStages
    {
        uint8_t cs : 1;
        uint8_t ps : 1;
        uint8_t vs : 1;
        uint8_t ms : 1;
        uint8_t ts : 1;
    };

    struct FShaderPassData
    {
        constexpr static size_t MaxModules = 3;

        GraphicsPipelineState state;
        FrBlob modules[MaxModules];
        FShaderStages stages;
    };

    struct FShaderPass : IObject
    {
        IMPL_INTERFACE("60e8339a-dfc0-4f91-8550-d14a3836e3c3", IObject);

        constexpr static size_t MaxModules = 3;

        virtual FError DataPtr(FShaderPassData** out) noexcept = 0;
    };

    struct FShaderPipeline : IObject
    {
        IMPL_INTERFACE("148fe4f0-51ad-464d-ad39-b8b06bc359af", IObject);

        // out 是 ID3D12PipelineState*
        virtual FError RawPtr(void** out) const noexcept = 0;
    };

    struct FGraphicsShaderPipeline : FShaderPipeline
    {
        IMPL_INTERFACE("3fa74850-cdd9-47eb-8ae3-bfc090f7077a", FShaderPipeline);

        virtual FError StatePtr(const GraphicsPipelineState** out) const noexcept = 0;
    };
}
