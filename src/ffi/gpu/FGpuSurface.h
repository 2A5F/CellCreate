﻿#pragma once
#include "FGpuQueue.h"
#include "FGpuRt.h"

namespace ccc
{
    struct FGpuSurfaceCreateOptions
    {
        FrStr16 name;

        bool v_sync;

    };

    struct FGpuSurface : FGpuRt
    {
        /* 准备并等待帧可用 */
        virtual void ready_frame(FGpuQueue* queue, FError& err) noexcept = 0;

        /* 呈现帧 */
        virtual void present_frame(FError& err) noexcept = 0;

        virtual bool get_v_sync() const noexcept = 0;

        virtual void set_v_sync(bool v) noexcept = 0;
    };
} // ccc
