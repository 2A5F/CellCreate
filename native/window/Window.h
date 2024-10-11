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

        [[nodiscard]] SDL_Window* sdl_window() const;

        void* hwnd() const;

        FError Size(uint2& out) noexcept override;
        FError PixelSize(uint2& out) noexcept override;
    };

    class Window final : public Object<>
    {
        IMPL_OBJECT();

        Rc<WindowHandle> m_window_handle;
    };
}
