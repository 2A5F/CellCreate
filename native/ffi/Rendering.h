#pragma once
#include "Shader.h"
#include "Window.h"
#include "../Object.h"

namespace cc
{
    namespace gpu
    {
        struct FGpuStreamCommands;
    }

    struct FGpuConsts
    {
        static constexpr uint32_t FrameCount = 3;
    };

    struct FGraphicSurface;
    struct FGpuResource;
    struct FGpuBuffer;
    struct FGpuTexture;
    struct FGpuBufferCreateOptions;
    struct FGpuGraph;

    struct FRenderingState
    {
        uint64_t frame_count;
        uint8_t _frame_index;
        b8 v_sync;
        b8 _on_recording;
    };

    struct FRendering : IObject, FGpuConsts
    {
        IMPL_INTERFACE("84cb940f-f9e2-4154-b330-5833e593bc94", IObject);

        virtual FRenderingState* StatePtr() noexcept = 0;

        virtual FError MakeContext(FWindowHandle* window_handle, FGraphicSurface** out) noexcept = 0;

        virtual FError CreateShaderPass(const FShaderPassData* data, FShaderPass** out) noexcept = 0;
        virtual FError CreateGraph(FGpuGraph** out) noexcept = 0;
        virtual FError CreateGraphicsShaderPipeline(
            const FShaderPassData* pass, /* opt */const GraphicsPipelineFormatOverride* override,
            FGraphicsShaderPipeline** out
        ) noexcept = 0;
        virtual FError CreateBuffer(const FGpuBufferCreateOptions* options, FGpuBuffer** out) noexcept = 0;

        virtual FError ReadyFrame() noexcept = 0;
        virtual FError EndFrame() noexcept = 0;

        // out 是 ID3D12Device2**
        virtual FError GetDevice(void** out) noexcept = 0;
        // out 是 ID3D12GraphicsCommandList6**
        virtual FError CurrentCommandList(void** out) noexcept = 0;

        virtual FError ClearSurface(FGraphicSurface* ctx, float4 color) noexcept = 0;
        // out 是 D3D12_CPU_DESCRIPTOR_HANDLE*
        virtual FError CurrentFrameRtv(FGraphicSurface* ctx, void** out) noexcept = 0;
    };

    struct FGpuQueue : IObject, FGpuConsts
    {
        IMPL_INTERFACE("f9e68473-6fee-49e0-9e65-58d2f77fb849", IObject);
    };

    struct FGpuTask : IObject
    {
        IMPL_INTERFACE("91ecc8c1-f971-44a2-9123-c152bfc7bc0d", IObject);
    };

    struct FGraphicSurfaceData
    {
        // D3D12_CPU_DESCRIPTOR_HANDLE
        void* current_frame_rtv;
        uint2 size;
        TextureFormat format;
    };

    struct FGraphicSurface : IObject, FGpuConsts
    {
        IMPL_INTERFACE("b0378104-80ee-4272-9c58-3af6e35ec437", IObject);

        // 销毁，被 Rendering 持有正常析构不会销毁需要手动销毁
        virtual FError Destroy() noexcept = 0;

        virtual FError DataPtr(FGraphicSurfaceData** out) noexcept = 0;

        virtual FError OnResize(uint2 size) noexcept = 0;
    };

    enum class GpuHeapType
    {
        Gpu,
        Upload,
        ReadBack,
    };

    enum class FGpuResourceState
    {
        Common = 0x0,
        VertexAndConstantBuffer = 0x1,
        IndexBuffer = 0x2,
        RenderTarget = 0x4,
        UnorderedAccess = 0x8,
        DepthWrite = 0x10,
        DepthRead = 0x20,
        NonPixel = 0x40,
        Pixel = 0x80,
        StreamOut = 0x100,
        IndirectArgument = 0x200,
        CopyDst = 0x400,
        CopySrc = 0x800,
        ResolveDst = 0x1000,
        ResolveSrc = 0x2000,
        RayTracingAccelerationStructure = 0x400000,
        ShadingRateSrc = 0x1000000,
        AllShaderResource = Pixel | NonPixel,
        GenericRead = VertexAndConstantBuffer | IndexBuffer | AllShaderResource | IndirectArgument | CopySrc
    };

    struct FGpuResourceData
    {
        FGpuResourceState state;
    };

    struct FGpuResource : IObject, FGpuConsts
    {
        IMPL_INTERFACE("dc3ca943-ad5e-4150-bfe8-b5bc12f3285d", IObject);

        // out 是 ID3D12Resource**
        virtual FError RawPtr(void** out) noexcept = 0;
        virtual FError DataPtr(FGpuResourceData** out) noexcept = 0;

        virtual FError SetName(const wchar_t* name) noexcept = 0;
    };

    struct FGpuBuffer : FGpuResource
    {
        IMPL_INTERFACE("01d2e268-62c6-4175-855f-88c9ac5f2f86", FGpuResource);
    };

    struct FGpuTexture : FGpuResource
    {
        IMPL_INTERFACE("7af4b0c3-13dd-4897-b807-81253c4a3e2e", FGpuResource);
    };

    struct FGpuBufferCreateOptions
    {
        u32 size;
        FGpuResourceState initial_state;
        GpuHeapType heap_type;
        b8 uav;
    };

    struct FGpuGraph : IObject
    {
        IMPL_INTERFACE("e3f38929-74e9-4df0-8001-e82eed2a23f7", IObject);

        virtual FError ExecuteCommand(gpu::FGpuStreamCommands cmds) noexcept = 0;
    };

    namespace gpu
    {
        enum class FGpuCommandOp : u8
        {
            // 无操作
            Nop,
            // 使用 FGpuCommandString
            DebugScopeStart,
            // 无结构
            DebugScopeEnd,
            // 清空指定的 rtv
            ClearRtv,
            // 清空指定的 dsv
            ClearDsv,
            // 设置当前渲染目标
            SetRt,
            // 设置视口
            SetViewPort,
            // 设置裁剪矩形
            SetScissorRect,
            // 设置着色器
            SetShader,
            // 绘制
            DrawInstanced,
            // 调度 Compute shader
            Dispatch,
            // 调度 Mesh shader，也使用 FGpuCommandDispatch
            DispatchMesh,
        };

        struct FGpuCommandString
        {
            // 不持有所有权，需要 c# 部分保证生命周期
            FString16* str;
        };

        struct FGpuCommandClearRtv
        {
            // 颜色
            f32 color_r;
            f32 color_g;
            f32 color_b;
            f32 color_a;
            // 0 表示清空全部
            u32 rects;
            // 尾随 1 个 rtv D3D12_CPU_DESCRIPTOR_HANDLE
            // 尾随 rects 个 int4 (l, t, r, b)
        };

        struct FGpuCommandClearDsv
        {
            // 0 表示清空全部
            u32 rects;
            // 深度
            f32 depth;
            // 模板
            u8 stencil;

            struct
            {
                u8 depth   : 1;
                u8 stencil : 1;
            } flags;

            // 尾随 1 个 dsv D3D12_CPU_DESCRIPTOR_HANDLE
            // 尾随 rects 个 int4 (l, t, r, b)
        };

        struct FGpuCommandSetRt
        {
            // 有多少个 rtv
            u8 rtv_count;
            b8 has_dsv;
            // 尾随 has_dsv ? 1 : 0 个 dsv D3D12_CPU_DESCRIPTOR_HANDLE
            // 尾随 rtv_count 个 rtv D3D12_CPU_DESCRIPTOR_HANDLE
            // 尾随 has_dsv ? 1 : 0 个 TextureFormat
            // 尾随 rtv_count 个 TextureFormat
        };

        struct FGpuCommandSetViewPort
        {
            // 有多少个视口
            u8 count;
            // 尾随 count 个 float[6] (top left x, top left y, width, height, min depth, max depth)
        };

        struct FGpuCommandSetScissorRect
        {
            // 有多少个裁剪矩形
            u8 count;
            // 尾随 count 个 int[4] (x, y, w, h)
        };

        struct FGpuCommandSetShader
        {
            // shader pass，将自动根据上下文创建 pipeline
            FShaderPass* pass;
        };

        struct FGpuCommandDrawInstanced
        {
            u32 vertex_count_per_instance;
            u32 instance_count;
            u32 start_vertex_location;
            u32 start_instance_location;
        };

        struct FGpuCommandDispatch
        {
            // x y z
            u32 thread_group_count_x;
            u32 thread_group_count_y;
            u32 thread_group_count_z;
        };

        struct FGpuStreamCommands
        {
            // (FGpuCommandOp, ..var size data)[]
            FGpuCommandOp** stream;
            size_t count;
        };
    }
}
