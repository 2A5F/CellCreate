#pragma once
#include <SDL3/SDL.h>

#include "../ffi/FFI.h"
#include "../ffi/Window.h"

namespace cc
{
    class WindowHandle final : public Object<FWindowHandle>
    {
        IMPL_OBJECT();

        SDL_Window* m_window;

    protected:
        ~WindowHandle() override;

    public:
        explicit WindowHandle(SDL_Window* window);

        FError Id(FWindowId& out) noexcept override;

        static FError Create(const FWindowCreateOptions& options, FWindowHandle*& out) noexcept;

        [[nodiscard]] SDL_Window* sdl_window() const;

        void* hwnd() const;
        FError Hwnd(void*& hwnd) noexcept override;

        FError SetTitle(const char* title) noexcept override;
        const char* Title() noexcept override;

        FError Size(uint2& out) noexcept override;
        FError PixelSize(uint2& out) noexcept override;

        FError Show() noexcept override;
        FError Hide() noexcept override;
    };

    class Window final : public Object<>
    {
        IMPL_OBJECT();

        Rc<WindowHandle> m_window_handle;
    };
}
