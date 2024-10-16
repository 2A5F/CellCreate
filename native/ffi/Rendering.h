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

        virtual FError ReadyFrame() noexcept = 0;
        virtual FError EndFrame() noexcept = 0;

        // out 是 ID3D12Device2**
        virtual FError GetDevice(void** out) noexcept = 0;
        // out 是 ID3D12GraphicsCommandList6**
        virtual FError CurrentCommandList(void** out) noexcept = 0;

        virtual FError ClearSurface(FRenderingContext* ctx, float4 color) noexcept = 0;
    };

    struct FRenderingContext : IObject, FGpuConsts
    {
        IMPL_INTERFACE("b0378104-80ee-4272-9c58-3af6e35ec437", IObject);

        // 销毁，被 Rendering 持有正常析构不会销毁需要手动销毁
        virtual FError Destroy() noexcept = 0;

        virtual FError OnResize(uint2 size) noexcept = 0;
    };
}
