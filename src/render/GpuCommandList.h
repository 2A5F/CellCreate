﻿#pragma once
#include <directx/d3d12.h>

#include "IRT.h"
#include "../pch.h"

namespace ccc {
    class GpuSurface;
}

namespace ccc {
    class RenderContext;
}

namespace ccc {
    class GpuCommandList final : public ISingleThreadObject {
        friend RenderContext;
        friend GpuSurface;
        friend IRT;

        com_ptr<ID3D12GraphicsCommandList> m_command_list{};

        std::shared_ptr<IRT> m_current_rt{};

        explicit GpuCommandList(com_ptr<ID3D12GraphicsCommandList> command_list);

    public:
        // 设置渲染目标
        void set_rt(std::shared_ptr<IRT> rt);

        // 清除当前 RT 上的颜色
        void clear(float4 color);

        // 清除指定 RT 上的颜色
        void clear(const std::shared_ptr<IRT> &rt, float4 color);
    };
} // ccc
