#pragma once
#include <directx/d3d12.h>

#include "../ffi/Rendering.h"

namespace cc
{
    struct CmdEncoder
    {
        ID3D12GraphicsCommandList6* m_list;

        explicit CmdEncoder(ID3D12GraphicsCommandList6* list);

        void Add(const gpu::FGpuStreamCommands& cmds);

    private:
        GraphicsPipelineFormatOverride m_cur_rt_format{};
    };
}
