#pragma once
#include "../Object.h"
#include "FFI.h"

namespace cc
{
    struct FWindowHandle : IObject
    {
        IMPL_INTERFACE("16ae5438-28d8-4c7e-b1b0-e420478cce6e", IObject)

        virtual FError Size(uint2& out) noexcept = 0;
        virtual FError PixelSize(uint2& out) noexcept = 0;
    };
}
