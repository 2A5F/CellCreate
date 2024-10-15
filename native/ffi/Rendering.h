#pragma once
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

    struct FRenderingConfig
    {
        size_t frame_count;
        b8 v_sync;
    };

    struct FRendering : IObject, FGpuConsts
    {
        IMPL_INTERFACE("84cb940f-f9e2-4154-b330-5833e593bc94", IObject);

        virtual FRenderingConfig* GetConfigs() noexcept = 0;

        virtual FError MakeContext(FWindowHandle* window_handle, FRenderingContext** out) noexcept = 0;

        virtual FError ReadyFrame() noexcept = 0;
        virtual FError EndFrame() noexcept = 0;

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
