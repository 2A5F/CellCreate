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
}
