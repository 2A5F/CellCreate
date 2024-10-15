#pragma once

#include "./FFI.h"
#include "./Window.h"
#include "Rendering.h"

namespace cc
{
    enum class FMessage
    {
        Nop,
        SwitchThread, // allow send msg
        WindowClose, // data is window id
        WindowResize,// data is window id
    };

    using FFI_Action__FMessage__voidp = void CO_CDECL(FMessage, void*);

    struct FMessageVtb
    {
        FFI_Action__FMessage__voidp* on_message;
    };

    struct FApp : IObject
    {
        IMPL_INTERFACE("d11e54f4-30b3-4e1a-92ea-de86d9e9e64f", IObject);

        virtual FError Init() noexcept = 0;
        virtual FError Exit() noexcept = 0;

        virtual FMessageVtb* MsgVtb() noexcept = 0;
        virtual FError MsgLoop() noexcept = 0;
        virtual FError SendMsg(FMessage type, void* data) noexcept = 0;

        virtual FError CreateWindowHandle(FWindowCreateOptions& options, FWindowHandle*& out) noexcept = 0;
        virtual FError CreateRendering(FRendering*& out) noexcept = 0;
    };
}
