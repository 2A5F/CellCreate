#pragma once

#include <stdexcept>

#include <winrt/base.h>
#include <SDL3/SDL.h>
#include <fmt/core.h>

#include "../ffi/FFI.h"

namespace cc
{
    struct CcError final : std::runtime_error
    {
        explicit CcError(char const* const msg)
            : std::runtime_error(msg)
        {
        }

        explicit CcError(const std::string& msg)
            : std::runtime_error(msg)
        {
        }
    };

    struct SdlError final : std::runtime_error
    {
        explicit SdlError(char const* const msg)
            : std::runtime_error(fmt::format("SDL Error: {}", msg))
        {
        }

        explicit SdlError() : SdlError(SDL_GetError())
        {
        }

        static FError FFI()
        {
            return {FErrorType::Sdl, FErrorMsgType::CStr, {.c_str = SDL_GetError()}};
        }
    };

    struct CheckError final
    {
    };

    constexpr inline static CheckError check_error;

    inline winrt::hresult operator <<(CheckError, winrt::hresult r)
    {
        return check_hresult(r);
    }
}
