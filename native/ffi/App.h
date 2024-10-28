#pragma once

#include "./FFI.h"
#include "./Window.h"
#include "Collections.h"
#include "Rendering.h"

namespace cc
{
    struct FLikeLinkedList
    {
        FLikeLinkedList* next;
    };

    enum class FMessage
    {
        Nop,
        SwitchThread, // allow send msg
        WindowClose,  // data is window id
        WindowResize, // data is window id
    };

    using FFI_Action__FMessage__voidp = void CO_CDECL(FMessage, void*);

    struct FMessageVtb
    {
        FFI_Action__FMessage__voidp* on_message;
    };

    struct FApp : IObject
    {
        IMPL_INTERFACE("d11e54f4-30b3-4e1a-92ea-de86d9e9e64f", IObject);

        virtual FError MemAlloc(size_t size, void** out) noexcept = 0;
        virtual FError MemFree(void* ptr) noexcept = 0;
        virtual FError MemReAlloc(void* ptr, size_t new_size, void** out) noexcept = 0;
        // 释放链表，要求必须是单链表
        virtual FError MemFreeLinkedList(FLikeLinkedList* ptr) noexcept = 0;

        virtual FError Init() noexcept = 0;
        virtual FError Exit() noexcept = 0;

        virtual FMessageVtb* MsgVtb() noexcept = 0;
        virtual FError MsgLoop() noexcept = 0;
        virtual FError SendMsg(FMessage type, void* data) noexcept = 0;

        virtual FError CreateString(FrStr16 str, FString16*& out) noexcept = 0;

        virtual FError CreateWindowHandle(FWindowCreateOptions& options, FWindowHandle*& out) noexcept = 0;
        virtual FError CreateRendering(FRendering*& out) noexcept = 0;

        virtual FError CreatePaddedChunkedVectorData(FChunkedVectorData*& out) noexcept = 0;
    };
}
