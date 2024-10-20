#pragma once
#include "Shader.h"
#include "Window.h"
#include "../Object.h"

namespace cc
{
    struct FGpuConsts
    {
        static constexpr uint32_t FrameCount = 3;
    };

    struct FRenderingContext;
    struct FCommandList;
    struct FGpuResource;
    struct FGpuBuffer;
    struct FGpuTexture;
    struct FGpuBufferCreateOptions;

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

        virtual FError MakeContext(FWindowHandle* window_handle, FRenderingContext** out) noexcept = 0;

        virtual FError CreateGraphicsShaderPipeline(FShaderPassData* pass, FGraphicsShaderPipeline** out) noexcept = 0;
        virtual FError CreateBuffer(const FGpuBufferCreateOptions* options, FGpuBuffer** out) noexcept = 0;

        virtual FError ReadyFrame() noexcept = 0;
        virtual FError EndFrame() noexcept = 0;

        // out 是 ID3D12Device2**
        virtual FError GetDevice(void** out) noexcept = 0;
        // out 是 ID3D12GraphicsCommandList6**
        virtual FError CurrentCommandList(void** out) noexcept = 0;

        virtual FError ClearSurface(FRenderingContext* ctx, float4 color) noexcept = 0;
        // out 是 D3D12_CPU_DESCRIPTOR_HANDLE*
        virtual FError CurrentFrameRtv(FRenderingContext* ctx, void** out) noexcept = 0;
    };

    struct FRenderingContext : IObject, FGpuConsts
    {
        IMPL_INTERFACE("b0378104-80ee-4272-9c58-3af6e35ec437", IObject);

        // 销毁，被 Rendering 持有正常析构不会销毁需要手动销毁
        virtual FError Destroy() noexcept = 0;

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

    namespace gpu
    {
        struct FGpuGraphConsts
        {
            constexpr static u32 NullViewId = 0;
            constexpr static u32 SurfaceRtvId = 0xffffffff - 1;
        };

        enum class FGpuCommandOp : u8
        {
            // 无操作
            Nop,
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
            // 设置管线
            SetPipeline,
            // 绘制
            DrawInstanced,
            // 调度 Compute shader
            Dispatch,
            // 调度 Mesh shader，也使用 FGpuCommandDispatch
            DispatchMesh,
        };

        struct FGpuCommandClearRtv
        {
            // rtv id
            u32 rtv;
            // 颜色
            f32 color[4];
            // 0 表示清空全部
            u32 rects;
            // 尾随 rects 个 int4 (l, t, r, b)
        };

        struct FGpuCommandClearDsv
        {
            // dsv id
            u32 dsv;

            struct
            {
                u8 depth   : 1;
                u8 stencil : 1;
            } flags;

            // 模板
            u8 stencil;
            // 深度
            f32 depth;

            // 0 表示清空全部
            u32 rects;
            // 尾随 rects 个 int4 (l, t, r, b)
        };

        struct FGpuCommandSetRt
        {
            // dsv id
            u32 dsv;
            // 有多少个 rtv
            u32 rtv_count;
            // 尾随 rtv_count 个 u32 rtv id
        };

        struct FGpuCommandSetViewPort
        {
            // 有多少个视口
            u32 count;
            // 尾随 count 个 float[6] (top left x, top left y, width, height, min depth, max depth)
        };

        struct FGpuCommandSetScissorRect
        {
            // 有多少个裁剪矩形
            u32 count;
            // 尾随 count 个 int[4] (x, y, w, h)
        };

        struct FGpuCommandSetPipeline
        {
            // 管线 id
            u32 pipeline;
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
            u32 thread_group_count[3];
        };

        struct FGpuStreamCommands
        {
            // (FGpuCommandOp, ..var size data)[]
            FGpuCommandOp** stream;
            size_t count;
        };
    }
}
