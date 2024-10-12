#include "App.h"

#include "rendering/Rendering.h"
#include "utils/error.h"
#include "window/Window.h"

using namespace cc;

namespace
{
    AppVars s_app_vars{};
    AppFnVtb s_app_fn_vtb{};
    Rc<FApp> s_app{};
}

AppVars& cc::args()
{
    return s_app_vars;
}

AppFnVtb& cc::vtb()
{
    return s_app_fn_vtb;
}

Rc<FApp>& cc::app()
{
    return s_app;
}

FError App::Init() noexcept
{
    if (!SDL_Init(SDL_INIT_VIDEO)) return SdlError::FFI();
    return FError::None();
}

FError App::Exit() noexcept
{
    SDL_Quit();
    return FError::None();
}

FMessageVtb* App::MsgVtb() noexcept
{
    return &m_vtb;
}

FError App::SendMsg(FMessage type, void* data) noexcept
{
    if (!(type == FMessage::SwitchThread))return FError::Common(str16(u"The message type is not allowed to be sent"));
    SDL_Event event{};
    event.type = SDL_EVENT_USER;
    event.user.code = static_cast<int32_t>(type);
    event.user.data1 = data;
    SDL_PushEvent(&event);
    return FError::None();
}

FError App::CreateWindowHandle(FWindowCreateOptions& options, FWindowHandle*& out) noexcept
{
    return WindowHandle::Create(options, out);
}

FError App::CreateRendering(FRendering*& out) noexcept
{
    return Rendering::Create(out);
}

FError App::MsgPump() noexcept
{
    SDL_Event event;
    if (SDL_WaitEvent(&event))
    {
        if (event.type == SDL_EVENT_QUIT)
        {
            args().running = false;
        }
        else if (event.type == SDL_EVENT_USER)
        {
            const auto type = static_cast<FMessage>(event.user.code);
            if (type == FMessage::SwitchThread)
            {
                m_vtb.on_message(type, event.user.data1);
            }
        }
        else if (event.type == SDL_EVENT_WINDOW_CLOSE_REQUESTED)
        {
            m_vtb.on_message(
                FMessage::WindowClose, reinterpret_cast<void*>(static_cast<size_t>(event.window.windowID))
            );
        }
    }
    return FError::None();
}
