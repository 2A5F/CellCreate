#include <iostream>

#include <windows.h>
#include <winrt/base.h>

#include <mimalloc-new-delete.h>
#include <mimalloc.h>
#include <cpptrace/cpptrace.hpp>
#include <cpptrace/from_current.hpp>
#include <spdlog/spdlog.h>
#include <SDL3/SDL.h>

#include "App.h"
#include "dotnet.h"
#include "utils/console_utils.h"
#include "utils/error.h"

extern "C" {
    __declspec(dllexport) extern const uint32_t D3D12SDKVersion = 614;
}

extern "C" {
    __declspec(dllexport) extern const char* D3D12SDKPath = ".\\D3D12\\";
}

int err_back(auto f)
{
    int r;
    try
    {
        ::cpptrace::detail::try_canary cpptrace_try_canary;
        [&]
        {
            __try
            {
                [&]
                {
                    r = f();
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
        spdlog::error("{}", msg);
        SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, "Error", msg.c_str(), nullptr);
        return -1;
    }
    catch (winrt::hresult_error& ex)
    {
        auto msg = fmt::format(
            L"{}\n{}", ex.message().c_str(),
            fmt::detail::utf8_to_utf16(cpptrace::from_current_exception().to_string()).c_str()
        );
        spdlog::error(L"{}", msg);
        MessageBoxW(nullptr, msg.c_str(), nullptr, MB_OK);
        return -1;
    }
    catch (...)
    {
        const auto msg = fmt::format(
            "Unknown failure occurred. Possible memory corruption\n{}", cpptrace::from_current_exception().to_string()
        );
        spdlog::error("{}", msg);
        SDL_ShowSimpleMessageBox(
            SDL_MESSAGEBOX_ERROR, "Error",
            msg.c_str(), nullptr
        );
        return -1;
    }
    return r;
}

int main()
{
    mi_version();
    RedirectIOToConsole();
    return err_back(
        []
        {
            winrt::init_apartment();
            if (!SDL_Init(SDL_INIT_VIDEO)) throw cc::SdlError();

            cc::InitParams init_params{
                .p_vas = &cc::args()
            };
            cc::InitResult init_result{
                .fn_vtb = &cc::vtb()
            };
            load_dotnet(&init_params, &init_result);
            cc::vtb().main();

            SDL_Quit();
            return 0;
        }
    );
}
