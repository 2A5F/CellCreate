#pragma once

#include "../defines.h"
#include "../Object.h"
#include "../types.h"

namespace cc
{
    using FFI_Action = void CO_CDECL();

    #ifdef CO_SRC
    using Char8 = char8_t;
    using Char16 = char16_t;
    #else
    using Char8 = uint8_t;
    using Char16 = uint16_t;
    #endif

    class FrStr8
    {
        const Char8* m_data;
        const size_t m_size;

    public:
        explicit FrStr8(const Char8* data, const size_t size): m_data(data), m_size(size)
        {
        }

        const Char8* data() const
        {
            return m_data;
        }

        size_t size() const
        {
            return m_size;
        }
    };

    class FrStr16
    {
        const Char16* m_data;
        const size_t m_size;

    public:
        explicit FrStr16(const Char16* data, const size_t size): m_data(data), m_size(size)
        {
        }

        const Char16* data() const
        {
            return m_data;
        }

        size_t size() const
        {
            return m_size;
        }
    };

    class FmStr8
    {
        Char8* m_data;
        const size_t m_size;

    public:
        explicit FmStr8(Char8* data, const size_t size): m_data(data), m_size(size)
        {
        }

        Char8* data() const
        {
            return m_data;
        }

        size_t size() const
        {
            return m_size;
        }
    };

    class FmStr16
    {
        Char16* m_data;
        const size_t m_size;

    public:
        explicit FmStr16(Char16* data, const size_t size): m_data(data), m_size(size)
        {
        }

        Char16* data() const
        {
            return m_data;
        }

        size_t size() const
        {
            return m_size;
        }
    };

    enum class FErrorType
    {
        None,
        Common,
        Sdl,
    };

    enum class FErrorMsgType
    {
        CStr,
        Str8,
        Str16,
    };

    struct FError
    {
        FErrorType type;
        FErrorMsgType msg_type;

        union
        {
            const char* c_str;
            FrStr8 str8;
            FrStr16 str16;
        };

        #if CO_SRC
        static constexpr FError None()
        {
            return {FErrorType::None};
        }
        #endif
    };

    enum class FLogLevel
    {
        Fatal,
        Error,
        Warn,
        Info,
        Debug,
        Trace,
    };

    struct FApp;

    struct AppFnVtb
    {
        FFI_Action* main;

        // fn_func__FrStr16__size_t* utf16_get_utf8_max_len;
        // fn_func__FrStr16_FmStr8__size_t* utf16_to_utf8;
        // fn_func__FrStr16__FString8p* utf16_to_string8;
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
        FApp* p_native_app;
    };

    struct InitResult
    {
        AppFnVtb* fn_vtb;
    };

    struct TestStruct
    {
        float2 a;
        float3 b;
        float4 c;
    };

    struct FApp : IObject
    {
        IMPL_INTERFACE("d11e54f4-30b3-4e1a-92ea-de86d9e9e64f", IObject)
    };
}
