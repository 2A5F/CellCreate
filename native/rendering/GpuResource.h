#pragma once
#include <directx/d3d12.h>

#include "Rendering.h"
#include "../Object.h"
#include "../ffi/Rendering.h"

namespace cc
{
    class GpuBuffer final : public Object<FGpuBuffer>
    {
    private:
        IMPL_OBJECT();

    public:
        Rc<Rendering> m_rendering{};
        ComPtr<D3D12MA::Allocation> m_allocation{};
        FGpuResourceData m_data{};

        explicit GpuBuffer(Rc<Rendering>&& rendering, const FGpuBufferCreateOptions* options);

        FError SetName(const wchar_t* name) noexcept override;

        FError RawPtr(void** out) noexcept override;
        FError DataPtr(FGpuResourceData** out) noexcept override;
    };
} // cc
