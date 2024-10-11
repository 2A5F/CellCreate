#pragma once
#include "ffi/FFI.h"

namespace cc
{
    AppVars& args();

    AppFnVtb& vtb();

    Rc<FApp>& app();

    class App final : public Object<FApp>
    {
        IMPL_OBJECT();

    public:
        explicit App() = default;
    };
}
