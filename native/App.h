#pragma once
#include <SDL3/SDL.h>

#include "ffi/FFI.h"
#include "ffi/App.h"

namespace cc
{
    AppVars& args();

    AppFnVtb& vtb();

    Rc<FApp>& app();

    inline void Log(const FLogLevel level, const char* msg)
    {
        vtb().logger_cstr(level, msg);
    }

    inline void Log(const FLogLevel level, const wchar_t* msg)
    {
        vtb().logger_wstr(level, msg);
    }

    inline void Log(const FLogLevel level, const FrStr8 msg)
    {
        vtb().logger_str8(level, msg);
    }

    inline void Log(const FLogLevel level, const FrStr16 msg)
    {
        vtb().logger_str16(level, msg);
    }

    template <size_t N>
    void Log(const FLogLevel level, const char8_t (&msg)[N])
    {
        vtb().logger_str8(level, {msg, N});
    }

    template <size_t N>
    void Log(const FLogLevel level, const char16_t (&msg)[N])
    {
        vtb().logger_str8(level, {msg, N});
    }

    class App final : public Object<FApp>
    {
        IMPL_OBJECT();

        FMessageVtb m_vtb{};

        bool EventFilter(const SDL_Event& event);

    public:
        explicit App() = default;

        FError Init() noexcept override;
        FError Exit() noexcept override;

        FMessageVtb* MsgVtb() noexcept override;
        FError MsgLoop() noexcept override;
        FError SendMsg(FMessage type, void* data) noexcept override;

        FError CreateWindowHandle(FWindowCreateOptions& options, FWindowHandle*& out) noexcept override;
        FError CreateRendering(FRendering*& out) noexcept override;
    };
}
