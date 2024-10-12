#pragma once
#include "Window.h"
#include "../Object.h"

namespace cc
{
    struct FGpuConsts
    {
        static constexpr uint32_t FrameCount = 3;
    };

    struct FRendering : IObject, FGpuConsts
    {
        IMPL_INTERFACE("84cb940f-f9e2-4154-b330-5833e593bc94", IObject);

        virtual FError Init(FWindowHandle* window_handle) noexcept = 0;

        virtual FError OnResize(uint2 size) noexcept = 0;
        virtual b8 VSync() noexcept = 0;
        virtual FError SetVSync(b8 enable) noexcept = 0;

        virtual FError ReadyFrame() noexcept = 0;
        virtual FError EndFrame() noexcept = 0;
    };
}
