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

FError WindowHandle::Id(FWindowId& out) noexcept
{
    out = {SDL_GetWindowID(m_window)};
    if (out.value == 0) return SdlError::FFI();
    return FError::None();
}

FError WindowHandle::Create(const FWindowCreateOptions& options, FWindowHandle*& out) noexcept
{
    SDL_WindowFlags flags = 0;
    if (options.resizable) flags |= SDL_WINDOW_RESIZABLE;
    if (options.hide) flags |= SDL_WINDOW_HIDDEN;
    const auto window = SDL_CreateWindow(
        options.title, static_cast<int>(options.size.x), static_cast<int>(options.size.y), flags
    );
    if (window == nullptr) return SdlError::FFI();
    Rc handle = new WindowHandle(window);
    if (options.has_max_size)
    {
        if (!SDL_SetWindowMaximumSize(
            window, static_cast<int>(options.max_size.x), static_cast<int>(options.max_size.y)
        ))
            return SdlError::FFI();
    }
    if (options.has_min_size)
    {
        if (!SDL_SetWindowMinimumSize(
            window, static_cast<int>(options.max_size.x), static_cast<int>(options.max_size.y)
        ))
            return SdlError::FFI();
    }
    out = handle.leak();
    return FError::None();
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

FError WindowHandle::Hwnd(void*& hwnd) noexcept
{
    const auto props = SDL_GetWindowProperties(m_window);
    if (props == 0) return SdlError::FFI();
    hwnd = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WIN32_HWND_POINTER, nullptr);
    return FError::None();
}

FError WindowHandle::SetTitle(const char* title) noexcept
{
    if (!SDL_SetWindowTitle(m_window, title)) return SdlError::FFI();
    return FError::None();
}

const char* WindowHandle::Title() noexcept
{
    return SDL_GetWindowTitle(m_window);
}

FError WindowHandle::Show() noexcept
{
    if (!SDL_ShowWindow(m_window)) return SdlError::FFI();
    return FError::None();
}

FError WindowHandle::Hide() noexcept
{
    if (!SDL_HideWindow(m_window)) return SdlError::FFI();
    return FError::None();
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
