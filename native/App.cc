#include "App.h"

using namespace cc;

namespace
{
    AppVars s_app_vars{};
    AppFnVtb s_app_fn_vtb{};
}

AppVars& cc::args()
{
    return s_app_vars;
}

AppFnVtb& cc::vtb()
{
    return s_app_fn_vtb;
}
