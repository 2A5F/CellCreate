#pragma once
#include <directx/d3d12.h>

#include "../ffi/Rendering.h"

namespace cc
{
    void cmd_encoder(ID3D12GraphicsCommandList6* list, gpu::FGpuStreamCommands& cmds);
}
