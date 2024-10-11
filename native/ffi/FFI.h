#pragma once

#include "../defines.h"
#include "../types.h"

using FFI_Action = void CO_CDECL();

namespace cc
{
    struct AppFnVtb
    {
        FFI_Action* main;

        // fn_func__FrStr16__size_t* utf16_get_utf8_max_len;
        // fn_func__FrStr16_FmStr8__size_t* utf16_to_utf8;
        // fn_func__FrStr16__FString8p* utf16_to_string8;

        // fn_action* start;
        // fn_action* exit;
        //
        // fn_action__voidp_FWindowEventType_voidp* window_event_handle;
        //
        // fn_func__FLogLevel_charp__void* logger_cstr;
        // fn_func__FLogLevel_wcharp__void* logger_wstr;
        // fn_func__FLogLevel_FrStr8__void* logger_str8;
        // fn_func__FLogLevel_FrStr16__void* logger_str16;
    };

    struct AppVars
    {
        b8 debug;
    };

    struct InitParams
    {
        AppVars* p_vas;
    };

    struct InitResult
    {
        AppFnVtb* fn_vtb;
    };
}
