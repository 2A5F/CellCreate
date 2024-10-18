#pragma once
#include <directx/d3d12.h>

#include "../ffi/Rendering.h"
#include "../ffi/Shader.h"

namespace cc
{
    D3D12_RESOURCE_STATES to_dx(FGpuResourceState state);

    D3D12_BLEND to_dx(BlendType type);

    D3D12_BLEND_OP to_dx(BlendOp op);

    D3D12_LOGIC_OP to_dx(LogicOp op);

    UINT8 to_dx(FColorMask mask);

    void to_dx(D3D12_BLEND_DESC& desc, const BlendState& state, int32_t rt_count);

    D3D12_FILL_MODE to_dx(FillMode mode);

    D3D12_CULL_MODE to_dx(CullMode mode);

    void to_dx(D3D12_RASTERIZER_DESC& desc, const RasterizerState& state);

    D3D12_COMPARISON_FUNC to_dx(CmpFunc func);

    D3D12_STENCIL_OP to_dx(StencilFailOp op);

    void to_dx(D3D12_DEPTH_STENCILOP_DESC& desc, const StencilState& state);

    void to_dx(D3D12_DEPTH_STENCIL_DESC& desc, const DepthStencilState& state);

    D3D12_PRIMITIVE_TOPOLOGY_TYPE to_dx_t(PrimitiveTopologyType topology);

    D3D12_PRIMITIVE_TOPOLOGY to_dx(PrimitiveTopologyType topology);

    DXGI_FORMAT to_dx(TextureFormat format);

    D3D12_HEAP_TYPE to_dx(GpuHeapType type);
}
