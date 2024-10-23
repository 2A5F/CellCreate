#pragma once

#include "../defines.h"
#include "../Object.h"
#include "../types.h"

namespace cc
{
    using FFI_Action = void CO_CDECL();
    using FFI_Func___i32 = i32 CO_CDECL();

    #ifdef CO_SRC
    using Char8 = char8_t;
    using Char16 = char16_t;
    #else
    using Char8 = uint8_t;
    using Char16 = uint16_t;
    #endif

    class FmBlob
    {
        char* const m_data;
        const size_t m_size;

    public:
        explicit constexpr FmBlob(char* data, const size_t size): m_data(data), m_size(size)
        {
        }

        char* data() const
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
        Char8* const m_data;
        const size_t m_size;

    public:
        explicit constexpr FmStr8(Char8* data, const size_t size): m_data(data), m_size(size)
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
        Char16* const m_data;
        const size_t m_size;

    public:
        explicit constexpr FmStr16(Char16* data, const size_t size): m_data(data), m_size(size)
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

    class FrBlob
    {
        const char* const m_data;
        const size_t m_size;

    public:
        explicit constexpr FrBlob(const char* data, const size_t size): m_data(data), m_size(size)
        {
        }

        FrBlob(FmBlob m) : m_data(m.data()), m_size(m.size()) // NOLINT(*-explicit-constructor)
        {
        }

        #if CO_SRC
        FrBlob& operator=(const FmBlob m)
        {
            new(this)FrBlob(m);
            return *this;
        }
        #endif

        const char* data() const
        {
            return m_data;
        }

        size_t size() const
        {
            return m_size;
        }
    };

    class FrStr8
    {
        const Char8* const m_data;
        const size_t m_size;

    public:
        explicit constexpr FrStr8(const Char8* data, const size_t size): m_data(data), m_size(size)
        {
        }

        FrStr8(FmStr8 m) : m_data(m.data()), m_size(m.size()) // NOLINT(*-explicit-constructor)
        {
        }

        #if CO_SRC
        FrStr8& operator=(const FmStr8 m)
        {
            new(this)FrStr8(m);
            return *this;
        }
        #endif

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
        const Char16* const m_data;
        const size_t m_size;

    public:
        explicit constexpr FrStr16(const Char16* data, const size_t size): m_data(data), m_size(size)
        {
        }

        FrStr16(FmStr16 m) : m_data(m.data()), m_size(m.size()) // NOLINT(*-explicit-constructor)
        {
        }

        #if CO_SRC
        FrStr16& operator=(const FmStr16 m)
        {
            new(this)FrStr16(m);
            return *this;
        }
        #endif

        const Char16* data() const
        {
            return m_data;
        }

        size_t size() const
        {
            return m_size;
        }
    };

    #if CO_SRC
    template <size_t N>
    constexpr FrStr8 str8(const char8_t (&msg)[N])
    {
        return FrStr8(msg, N);
    }

    template <size_t N>
    constexpr FrStr16 str16(const char16_t (&msg)[N])
    {
        return FrStr16(msg, N);
    }

    template <size_t N>
    constexpr FmStr8 str8(char8_t (&msg)[N])
    {
        return FmStr8(msg, N);
    }

    template <size_t N>
    constexpr FmStr16 str16(char16_t (&msg)[N])
    {
        return FmStr16(msg, N);
    }
    #endif

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
        constexpr bool IsNone() const noexcept
        {
            return type == FErrorType::None;
        }

        static constexpr FError None() noexcept
        {
            return {FErrorType::None};
        }

        static constexpr FError Common(const FrStr16 msg) noexcept
        {
            return {FErrorType::Common, FErrorMsgType::Str16, {.str16 = msg}};
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

    using FFI_Action__FLogLevel__ccharp = void CO_CDECL(FLogLevel, const char*);
    using FFI_Action__FLogLevel__cwcharp = void CO_CDECL(FLogLevel, const wchar_t*);
    using FFI_Action__FLogLevel__FrStr8 = void CO_CDECL(FLogLevel, FrStr8);
    using FFI_Action__FLogLevel__FrStr16 = void CO_CDECL(FLogLevel, FrStr16);

    struct AppFnVtb
    {
        FFI_Func___i32* main;

        FFI_Action__FLogLevel__ccharp* logger_cstr;
        FFI_Action__FLogLevel__cwcharp* logger_wstr;
        FFI_Action__FLogLevel__FrStr8* logger_str8;
        FFI_Action__FLogLevel__FrStr16* logger_str16;

        // fn_func__FrStr16__size_t* utf16_get_utf8_max_len;
        // fn_func__FrStr16_FmStr8__size_t* utf16_to_utf8;
        // fn_func__FrStr16__FString8p* utf16_to_string8;
        //
        // fn_action__voidp_FWindowEventType_voidp* window_event_handle;
    };

    struct AppVars
    {
        u64 a_hash_rand_0{};
        u64 a_hash_rand_1{};
        u64 a_hash_rand_2{};
        u64 a_hash_rand_3{};
        b8 debug{false};
        b8 running{true};
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
}
