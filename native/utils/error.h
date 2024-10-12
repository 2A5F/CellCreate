#pragma once

#include <stdexcept>

#include <winrt/base.h>
#include <SDL3/SDL.h>
#include <fmt/core.h>
#include <cpptrace/cpptrace.hpp>
#include <cpptrace/from_current.hpp>
#include <spdlog/spdlog.h>

#include "../App.h"
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

    FError ferr_back(auto f) noexcept
    {
        try
        {
            ::cpptrace::detail::try_canary cpptrace_try_canary;
            [&]
            {
                __try
                {
                    [&]
                    {
                        f();
                    }();
                }
                __except (::cpptrace::detail::exception_filter())
                {
                }
            }();
        }
        catch (std::exception& ex)
        {
            const auto msg = fmt::format("{}\n{}", ex.what(), cpptrace::from_current_exception().to_string());
            Log(FLogLevel::Error, FrStr8(reinterpret_cast<const char8_t*>(msg.data()), msg.size()));
            return FError::Common(str16(u"Native Exception"));
        }
        catch (winrt::hresult_error& ex)
        {
            const auto msg = fmt::format(
                L"{}\n{}", ex.message().c_str(),
                fmt::detail::utf8_to_utf16(cpptrace::from_current_exception().to_string()).c_str()
            );
            Log(FLogLevel::Error, FrStr16(reinterpret_cast<const char16_t*>(msg.data()), msg.size()));
            return FError::Common(str16(u"HResult Exception"));
        }
        catch (...)
        {
            const auto msg = fmt::format(
                "Unknown failure occurred. Possible memory corruption\n{}",
                cpptrace::from_current_exception().to_string()
            );
            Log(FLogLevel::Error, FrStr8(reinterpret_cast<const char8_t*>(msg.data()), msg.size()));
            return FError::Common(str16(u"Unknown failure occurred. Possible memory corruption"));
        }
        return FError::None();
    }
}
