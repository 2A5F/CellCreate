#pragma once
#include "../ffi/Rendering.h"
#include "Rendering.h"

namespace cc
{
    class RenderGraph final : public Object<FGpuGraph>
    {
        IMPL_OBJECT();

    public:
        Rc<Rendering> m_rendering;

        explicit RenderGraph(Rc<Rendering>&& rendering);
    };
}
