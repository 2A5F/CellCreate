#include "GpuResource.h"

#include "gpu_convert.h"
#include "../utils/error.h"

using namespace cc;

GpuBuffer::GpuBuffer(Rc<Rendering>&& rendering, const FGpuBufferCreateOptions* options) : m_rendering(
    std::move(rendering)
)
{
    const auto allocator = m_rendering->m_gpu_allocator.get();

    D3D12_RESOURCE_STATES state;
    if (options->heap_type == GpuHeapType::Upload) state = D3D12_RESOURCE_STATE_GENERIC_READ;
    else if (options->heap_type == GpuHeapType::ReadBack) state = D3D12_RESOURCE_STATE_COPY_DEST;
    else state = to_dx(options->initial_state);

    D3D12MA::ALLOCATION_DESC alloc_desc = {};
    alloc_desc.HeapType = to_dx(options->heap_type);
    D3D12_RESOURCE_DESC desc = {};
    desc.Dimension = D3D12_RESOURCE_DIMENSION_BUFFER;
    desc.Alignment = D3D12_DEFAULT_RESOURCE_PLACEMENT_ALIGNMENT;
    desc.Width = std::max<uint64_t>(options->size, 1);
    desc.Height = 1;
    desc.DepthOrArraySize = 1;
    desc.MipLevels = 1;
    desc.Format = DXGI_FORMAT_UNKNOWN;
    desc.SampleDesc.Count = 1;
    desc.SampleDesc.Quality = 0;
    desc.Layout = D3D12_TEXTURE_LAYOUT_ROW_MAJOR;
    if (options->uav) desc.Flags |= D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS;

    ComPtr<ID3D12Resource> resource{};
    check_error << allocator->CreateResource(
        &alloc_desc,
        &desc,
        state,
        nullptr,
        m_allocation.put(),
        RT_IID_PPV_ARGS(resource)
    );

    m_data.state = options->initial_state;
}

FError GpuBuffer::SetName(const wchar_t* name) noexcept
{
    return ferr_back(
        [&]
        {
            m_allocation->SetName(name);
            check_error << m_allocation->GetResource()->SetName(name);
        }
    );
}

FError GpuBuffer::RawPtr(void** out) noexcept
{
    *out = m_allocation->GetResource();
    return FError::None();
}

FError GpuBuffer::DataPtr(FGpuResourceData** out) noexcept
{
    *out = &m_data;
    return FError::None();
}
