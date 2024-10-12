#pragma once
#include "../Object.h"
#include "FFI.h"

namespace cc
{
    struct FWindowCreateOptions
    {
        const char* title;
        uint2 size;
        uint2 max_size;
        uint2 min_size;
        b8 has_max_size;
        b8 has_min_size;
        b8 resizable;
        b8 hide;
    };

    struct FWindowId
    {
        u32 value;
    };

    struct FWindowHandle : IObject
    {
        IMPL_INTERFACE("16ae5438-28d8-4c7e-b1b0-e420478cce6e", IObject)
        virtual FError Id(FWindowId& out) noexcept = 0;

        virtual FError Hwnd(void*& hwnd) noexcept = 0;

        virtual FError SetTitle(const char* title) noexcept = 0;
        virtual const char* Title() noexcept = 0;

        virtual FError Size(uint2& out) noexcept = 0;
        virtual FError PixelSize(uint2& out) noexcept = 0;

        virtual FError Show() noexcept = 0;
        virtual FError Hide() noexcept = 0;
    };
}
