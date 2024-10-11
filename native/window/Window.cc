#include "Window.h"

#include "../utils/error.h"

using namespace cc;

WindowHandle::~WindowHandle()
{
    if (const auto window = std::exchange(m_window, nullptr))
    {
        SDL_DestroyWindow(window);
    }
}

WindowHandle::WindowHandle(SDL_Window* window): m_window(window)
{
}

SDL_Window* WindowHandle::sdl_window() const
{
    return m_window;
}

void* WindowHandle::hwnd() const
{
    const auto props = SDL_GetWindowProperties(m_window);
    if (props == 0) throw SdlError();
    return SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WIN32_HWND_POINTER, nullptr);
}

FError WindowHandle::Size(uint2& out) noexcept
{
    int w, h;
    if (!SDL_GetWindowSize(m_window, &w, &h)) return SdlError::FFI();
    out = {w, h};
    return FError::None();
}

FError WindowHandle::PixelSize(uint2& out) noexcept
{
    int w, h;
    if (!SDL_GetWindowSizeInPixels(m_window, &w, &h)) return SdlError::FFI();
    out = {w, h};
    return FError::None();
}
