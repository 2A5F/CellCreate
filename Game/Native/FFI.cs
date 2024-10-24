using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Game.Native.BlendOp;
using static Game.Native.BlendType;

namespace Game.Native
{
    public unsafe partial struct FmBlob
    {
        [NativeTypeName("char *const")]
        private byte* m_data;

        [NativeTypeName("const size_t")]
        private nuint m_size;

        public FmBlob([NativeTypeName("char *")] byte* data, [NativeTypeName("const size_t")] nuint size)
        {
            m_data = data;
            m_size = size;
        }

        [return: NativeTypeName("char *")]
        public byte* data()
        {
            return m_data;
        }

        [return: NativeTypeName("size_t")]
        public nuint size()
        {
            return m_size;
        }
    }

    public unsafe partial struct FmStr8
    {
        [NativeTypeName("cc::Char8 *")]
        private byte* m_data;

        [NativeTypeName("const size_t")]
        private nuint m_size;

        public FmStr8([NativeTypeName("cc::Char8 *")] byte* data, [NativeTypeName("const size_t")] nuint size)
        {
            m_data = data;
            m_size = size;
        }

        [return: NativeTypeName("cc::Char8 *")]
        public byte* data()
        {
            return m_data;
        }

        [return: NativeTypeName("size_t")]
        public nuint size()
        {
            return m_size;
        }
    }

    public unsafe partial struct FmStr16
    {
        [NativeTypeName("cc::Char16 *")]
        private char* m_data;

        [NativeTypeName("const size_t")]
        private nuint m_size;

        public FmStr16([NativeTypeName("cc::Char16 *")] char* data, [NativeTypeName("const size_t")] nuint size)
        {
            m_data = data;
            m_size = size;
        }

        [return: NativeTypeName("cc::Char16 *")]
        public char* data()
        {
            return m_data;
        }

        [return: NativeTypeName("size_t")]
        public nuint size()
        {
            return m_size;
        }
    }

    public unsafe partial struct FrBlob
    {
        [NativeTypeName("const char *const")]
        private byte* m_data;

        [NativeTypeName("const size_t")]
        private nuint m_size;

        public FrBlob([NativeTypeName("const char *")] byte* data, [NativeTypeName("const size_t")] nuint size)
        {
            m_data = data;
            m_size = size;
        }

        public FrBlob([NativeTypeName("cc::FmBlob")] FmBlob m)
        {
            m_data = m.data();
            m_size = m.size();
        }

        [return: NativeTypeName("const char *")]
        public byte* data()
        {
            return m_data;
        }

        [return: NativeTypeName("size_t")]
        public nuint size()
        {
            return m_size;
        }
    }

    public unsafe partial struct FrStr8
    {
        [NativeTypeName("const Char8 *const")]
        private byte* m_data;

        [NativeTypeName("const size_t")]
        private nuint m_size;

        public FrStr8([NativeTypeName("const Char8 *")] byte* data, [NativeTypeName("const size_t")] nuint size)
        {
            m_data = data;
            m_size = size;
        }

        public FrStr8([NativeTypeName("cc::FmStr8")] FmStr8 m)
        {
            m_data = m.data();
            m_size = m.size();
        }

        [return: NativeTypeName("const Char8 *")]
        public byte* data()
        {
            return m_data;
        }

        [return: NativeTypeName("size_t")]
        public nuint size()
        {
            return m_size;
        }
    }

    public unsafe partial struct FrStr16
    {
        [NativeTypeName("const Char16 *const")]
        private char* m_data;

        [NativeTypeName("const size_t")]
        private nuint m_size;

        public FrStr16([NativeTypeName("const Char16 *")] char* data, [NativeTypeName("const size_t")] nuint size)
        {
            m_data = data;
            m_size = size;
        }

        public FrStr16([NativeTypeName("cc::FmStr16")] FmStr16 m)
        {
            m_data = m.data();
            m_size = m.size();
        }

        [return: NativeTypeName("const Char16 *")]
        public char* data()
        {
            return m_data;
        }

        [return: NativeTypeName("size_t")]
        public nuint size()
        {
            return m_size;
        }
    }

    public enum FErrorType
    {
        None,
        Common,
        Sdl,
    }

    public enum FErrorMsgType
    {
        CStr,
        Str8,
        Str16,
    }

    public unsafe partial struct FError
    {
        [NativeTypeName("cc::FErrorType")]
        public FErrorType type;

        [NativeTypeName("cc::FErrorMsgType")]
        public FErrorMsgType msg_type;

        [NativeTypeName("__AnonymousRecord_FFI_L227_C9")]
        public _Anonymous_e__Union Anonymous;

        [UnscopedRef]
        public ref byte* c_str
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Anonymous.c_str;
            }
        }

        [UnscopedRef]
        public ref FrStr8 str8
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Anonymous.str8;
            }
        }

        [UnscopedRef]
        public ref FrStr16 str16
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Anonymous.str16;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            [NativeTypeName("const char *")]
            public byte* c_str;

            [FieldOffset(0)]
            [NativeTypeName("cc::FrStr8")]
            public FrStr8 str8;

            [FieldOffset(0)]
            [NativeTypeName("cc::FrStr16")]
            public FrStr16 str16;
        }
    }

    public enum FLogLevel
    {
        Fatal,
        Error,
        Warn,
        Info,
        Debug,
        Trace,
    }

    public partial struct FApp
    {
    }

    [NativeTypeName("struct FApp : cc::IObject")]
    public unsafe partial struct FApp
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "d11e54f4-30b3-4e1a-92ea-de86d9e9e64f";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FApp*, void>)(lpVtbl[0]))((FApp*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FApp*, nuint>)(lpVtbl[1]))((FApp*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FApp*, nuint>)(lpVtbl[2]))((FApp*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FApp*, nuint>)(lpVtbl[3]))((FApp*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FApp*, nuint>)(lpVtbl[4]))((FApp*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FApp*, B8>)(lpVtbl[5]))((FApp*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FApp*, B8>)(lpVtbl[6]))((FApp*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FApp*, void*>)(lpVtbl[7]))((FApp*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FApp*, Guid, void*>)(lpVtbl[8]))((FApp*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError MemAlloc([NativeTypeName("size_t")] nuint size, void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, nuint, void**, FError*>)(lpVtbl[9]))((FApp*)Unsafe.AsPointer(ref this), &result, size, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError MemFree(void* ptr)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, void*, FError*>)(lpVtbl[10]))((FApp*)Unsafe.AsPointer(ref this), &result, ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError MemReAlloc(void* ptr, [NativeTypeName("size_t")] nuint new_size, void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, void*, nuint, void**, FError*>)(lpVtbl[11]))((FApp*)Unsafe.AsPointer(ref this), &result, ptr, new_size, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError MemFreeLinkedList([NativeTypeName("cc::FLikeLinkedList *")] FLikeLinkedList* ptr)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FLikeLinkedList*, FError*>)(lpVtbl[12]))((FApp*)Unsafe.AsPointer(ref this), &result, ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError Init()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FError*>)(lpVtbl[13]))((FApp*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError Exit()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FError*>)(lpVtbl[14]))((FApp*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FMessageVtb *")]
        public FMessageVtb* MsgVtb()
        {
            return ((delegate* unmanaged[Thiscall]<FApp*, FMessageVtb*>)(lpVtbl[15]))((FApp*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError MsgLoop()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FError*>)(lpVtbl[16]))((FApp*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError SendMsg([NativeTypeName("cc::FMessage")] FMessage type, void* data)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FMessage, void*, FError*>)(lpVtbl[17]))((FApp*)Unsafe.AsPointer(ref this), &result, type, data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CreateWindowHandle([NativeTypeName("cc::FWindowCreateOptions &")] FWindowCreateOptions* options, [NativeTypeName("FWindowHandle *&")] FWindowHandle** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FWindowCreateOptions*, FWindowHandle**, FError*>)(lpVtbl[18]))((FApp*)Unsafe.AsPointer(ref this), &result, options, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CreateRendering([NativeTypeName("FRendering *&")] FRendering** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FRendering**, FError*>)(lpVtbl[19]))((FApp*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CreatePaddedChunkedVectorData([NativeTypeName("FChunkedVectorData *&")] FChunkedVectorData** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FChunkedVectorData**, FError*>)(lpVtbl[20]))((FApp*)Unsafe.AsPointer(ref this), &result, @out);
        }
    }

    public unsafe partial struct AppFnVtb
    {
        [NativeTypeName("cc::FFI_Func___i32 *")]
        public delegate* unmanaged[Cdecl]<int> main;

        [NativeTypeName("cc::FFI_Action__FLogLevel__ccharp *")]
        public delegate* unmanaged[Cdecl]<FLogLevel, byte*, void> logger_cstr;

        [NativeTypeName("cc::FFI_Action__FLogLevel__cwcharp *")]
        public delegate* unmanaged[Cdecl]<FLogLevel, char*, void> logger_wstr;

        [NativeTypeName("cc::FFI_Action__FLogLevel__FrStr8 *")]
        public delegate* unmanaged[Cdecl]<FLogLevel, FrStr8, void> logger_str8;

        [NativeTypeName("cc::FFI_Action__FLogLevel__FrStr16 *")]
        public delegate* unmanaged[Cdecl]<FLogLevel, FrStr16, void> logger_str16;
    }

    public partial struct AppVars
    {
        [NativeTypeName("cc::u64")]
        public ulong a_hash_rand_0;

        [NativeTypeName("cc::u64")]
        public ulong a_hash_rand_1;

        [NativeTypeName("cc::u64")]
        public ulong a_hash_rand_2;

        [NativeTypeName("cc::u64")]
        public ulong a_hash_rand_3;

        [NativeTypeName("cc::b8")]
        public B8 debug;

        [NativeTypeName("cc::b8")]
        public B8 running;
    }

    public unsafe partial struct InitParams
    {
        [NativeTypeName("cc::AppVars *")]
        public AppVars* p_vas;

        [NativeTypeName("cc::FApp *")]
        public FApp* p_native_app;
    }

    public unsafe partial struct InitResult
    {
        [NativeTypeName("cc::AppFnVtb *")]
        public AppFnVtb* fn_vtb;
    }

    public unsafe partial struct FLikeLinkedList
    {
        [NativeTypeName("cc::FLikeLinkedList *")]
        public FLikeLinkedList* next;
    }

    public enum FMessage
    {
        Nop,
        SwitchThread,
        WindowClose,
        WindowResize,
    }

    public unsafe partial struct FMessageVtb
    {
        [NativeTypeName("cc::FFI_Action__FMessage__voidp *")]
        public delegate* unmanaged[Cdecl]<FMessage, void*, void> on_message;
    }

    public unsafe partial struct FWindowCreateOptions
    {
        [NativeTypeName("const char *")]
        public byte* title;

        [NativeTypeName("cc::uint2")]
        public uint2 size;

        [NativeTypeName("cc::uint2")]
        public uint2 max_size;

        [NativeTypeName("cc::uint2")]
        public uint2 min_size;

        [NativeTypeName("cc::b8")]
        public B8 has_max_size;

        [NativeTypeName("cc::b8")]
        public B8 has_min_size;

        [NativeTypeName("cc::b8")]
        public B8 resizable;

        [NativeTypeName("cc::b8")]
        public B8 hide;
    }

    public partial struct FWindowId
    {
        [NativeTypeName("cc::u32")]
        public uint value;
    }

    [NativeTypeName("struct FWindowHandle : cc::IObject")]
    public unsafe partial struct FWindowHandle
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "16ae5438-28d8-4c7e-b1b0-e420478cce6e";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FWindowHandle*, void>)(lpVtbl[0]))((FWindowHandle*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FWindowHandle*, nuint>)(lpVtbl[1]))((FWindowHandle*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FWindowHandle*, nuint>)(lpVtbl[2]))((FWindowHandle*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FWindowHandle*, nuint>)(lpVtbl[3]))((FWindowHandle*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FWindowHandle*, nuint>)(lpVtbl[4]))((FWindowHandle*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FWindowHandle*, B8>)(lpVtbl[5]))((FWindowHandle*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FWindowHandle*, B8>)(lpVtbl[6]))((FWindowHandle*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FWindowHandle*, void*>)(lpVtbl[7]))((FWindowHandle*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FWindowHandle*, Guid, void*>)(lpVtbl[8]))((FWindowHandle*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError Id([NativeTypeName("cc::FWindowId &")] FWindowId* @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FWindowHandle*, FError*, FWindowId*, FError*>)(lpVtbl[9]))((FWindowHandle*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError Hwnd([NativeTypeName("void *&")] void** hwnd)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FWindowHandle*, FError*, void**, FError*>)(lpVtbl[10]))((FWindowHandle*)Unsafe.AsPointer(ref this), &result, hwnd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError SetTitle([NativeTypeName("const char *")] byte* title)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FWindowHandle*, FError*, byte*, FError*>)(lpVtbl[11]))((FWindowHandle*)Unsafe.AsPointer(ref this), &result, title);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("const char *")]
        public byte* Title()
        {
            return ((delegate* unmanaged[Thiscall]<FWindowHandle*, byte*>)(lpVtbl[12]))((FWindowHandle*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError Size([NativeTypeName("cc::uint2 &")] uint2* @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FWindowHandle*, FError*, uint2*, FError*>)(lpVtbl[13]))((FWindowHandle*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError PixelSize([NativeTypeName("cc::uint2 &")] uint2* @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FWindowHandle*, FError*, uint2*, FError*>)(lpVtbl[14]))((FWindowHandle*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError Show()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FWindowHandle*, FError*, FError*>)(lpVtbl[15]))((FWindowHandle*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError Hide()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FWindowHandle*, FError*, FError*>)(lpVtbl[16]))((FWindowHandle*)Unsafe.AsPointer(ref this), &result);
        }
    }

    public partial struct FGpuConsts
    {

        [NativeTypeName("const uint32_t")]
        public const uint FrameCount = 3;
    }

    public partial struct FRenderingState
    {
        [NativeTypeName("uint64_t")]
        public ulong frame_count;

        [NativeTypeName("uint8_t")]
        public byte _frame_index;

        [NativeTypeName("cc::b8")]
        public B8 v_sync;

        [NativeTypeName("cc::b8")]
        public B8 _on_recording;
    }

    [NativeTypeName("struct FRendering : cc::IObject, cc::FGpuConsts")]
    public unsafe partial struct FRendering
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "84cb940f-f9e2-4154-b330-5833e593bc94";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FRendering*, void>)(lpVtbl[0]))((FRendering*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FRendering*, nuint>)(lpVtbl[1]))((FRendering*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FRendering*, nuint>)(lpVtbl[2]))((FRendering*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FRendering*, nuint>)(lpVtbl[3]))((FRendering*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FRendering*, nuint>)(lpVtbl[4]))((FRendering*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FRendering*, B8>)(lpVtbl[5]))((FRendering*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FRendering*, B8>)(lpVtbl[6]))((FRendering*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FRendering*, void*>)(lpVtbl[7]))((FRendering*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FRendering*, Guid, void*>)(lpVtbl[8]))((FRendering*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FRenderingState *")]
        public FRenderingState* StatePtr()
        {
            return ((delegate* unmanaged[Thiscall]<FRendering*, FRenderingState*>)(lpVtbl[9]))((FRendering*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError MakeContext([NativeTypeName("cc::FWindowHandle *")] FWindowHandle* window_handle, FGraphicSurface** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FWindowHandle*, FGraphicSurface**, FError*>)(lpVtbl[10]))((FRendering*)Unsafe.AsPointer(ref this), &result, window_handle, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CreateShaderPass([NativeTypeName("const FShaderPassData *")] FShaderPassData* data, FShaderPass** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FShaderPassData*, FShaderPass**, FError*>)(lpVtbl[11]))((FRendering*)Unsafe.AsPointer(ref this), &result, data, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CreateGraph(FGpuGraph** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FGpuGraph**, FError*>)(lpVtbl[12]))((FRendering*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CreateGraphicsShaderPipeline([NativeTypeName("const FShaderPassData *")] FShaderPassData* pass, [NativeTypeName("const GraphicsPipelineFormatOverride *")] GraphicsPipelineFormatOverride* @override, FGraphicsShaderPipeline** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FShaderPassData*, GraphicsPipelineFormatOverride*, FGraphicsShaderPipeline**, FError*>)(lpVtbl[13]))((FRendering*)Unsafe.AsPointer(ref this), &result, pass, @override, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CreateBuffer([NativeTypeName("const FGpuBufferCreateOptions *")] FGpuBufferCreateOptions* options, FGpuBuffer** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FGpuBufferCreateOptions*, FGpuBuffer**, FError*>)(lpVtbl[14]))((FRendering*)Unsafe.AsPointer(ref this), &result, options, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError ReadyFrame()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FError*>)(lpVtbl[15]))((FRendering*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError EndFrame()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FError*>)(lpVtbl[16]))((FRendering*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError GetDevice(void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, void**, FError*>)(lpVtbl[17]))((FRendering*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CurrentCommandList(void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, void**, FError*>)(lpVtbl[18]))((FRendering*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError ClearSurface([NativeTypeName("cc::FGraphicSurface *")] FGraphicSurface* ctx, [NativeTypeName("cc::float4")] float4 color)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FGraphicSurface*, float4, FError*>)(lpVtbl[19]))((FRendering*)Unsafe.AsPointer(ref this), &result, ctx, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CurrentFrameRtv([NativeTypeName("cc::FGraphicSurface *")] FGraphicSurface* ctx, void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FGraphicSurface*, void**, FError*>)(lpVtbl[20]))((FRendering*)Unsafe.AsPointer(ref this), &result, ctx, @out);
        }
    }

    public unsafe partial struct FGraphicSurfaceData
    {
        public void* current_frame_rtv;

        [NativeTypeName("cc::uint2")]
        public uint2 size;

        [NativeTypeName("cc::TextureFormat")]
        public TextureFormat format;
    }

    [NativeTypeName("struct FGraphicSurface : cc::IObject, cc::FGpuConsts")]
    public unsafe partial struct FGraphicSurface
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "b0378104-80ee-4272-9c58-3af6e35ec437";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FGraphicSurface*, void>)(lpVtbl[0]))((FGraphicSurface*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicSurface*, nuint>)(lpVtbl[1]))((FGraphicSurface*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicSurface*, nuint>)(lpVtbl[2]))((FGraphicSurface*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicSurface*, nuint>)(lpVtbl[3]))((FGraphicSurface*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicSurface*, nuint>)(lpVtbl[4]))((FGraphicSurface*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicSurface*, B8>)(lpVtbl[5]))((FGraphicSurface*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicSurface*, B8>)(lpVtbl[6]))((FGraphicSurface*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicSurface*, void*>)(lpVtbl[7]))((FGraphicSurface*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicSurface*, Guid, void*>)(lpVtbl[8]))((FGraphicSurface*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError Destroy()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGraphicSurface*, FError*, FError*>)(lpVtbl[9]))((FGraphicSurface*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError DataPtr(FGraphicSurfaceData** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGraphicSurface*, FError*, FGraphicSurfaceData**, FError*>)(lpVtbl[10]))((FGraphicSurface*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError OnResize([NativeTypeName("cc::uint2")] uint2 size)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGraphicSurface*, FError*, uint2, FError*>)(lpVtbl[11]))((FGraphicSurface*)Unsafe.AsPointer(ref this), &result, size);
        }
    }

    public enum GpuHeapType
    {
        Gpu,
        Upload,
        ReadBack,
    }

    public enum FGpuResourceState
    {
        Common = 0x0,
        VertexAndConstantBuffer = 0x1,
        IndexBuffer = 0x2,
        RenderTarget = 0x4,
        UnorderedAccess = 0x8,
        DepthWrite = 0x10,
        DepthRead = 0x20,
        NonPixel = 0x40,
        Pixel = 0x80,
        StreamOut = 0x100,
        IndirectArgument = 0x200,
        CopyDst = 0x400,
        CopySrc = 0x800,
        ResolveDst = 0x1000,
        ResolveSrc = 0x2000,
        RayTracingAccelerationStructure = 0x400000,
        ShadingRateSrc = 0x1000000,
        AllShaderResource = Pixel | NonPixel,
        GenericRead = VertexAndConstantBuffer | IndexBuffer | AllShaderResource | IndirectArgument | CopySrc,
    }

    public partial struct FGpuResourceData
    {
        [NativeTypeName("cc::FGpuResourceState")]
        public FGpuResourceState state;
    }

    [NativeTypeName("struct FGpuResource : cc::IObject, cc::FGpuConsts")]
    public unsafe partial struct FGpuResource
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "dc3ca943-ad5e-4150-bfe8-b5bc12f3285d";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FGpuResource*, void>)(lpVtbl[0]))((FGpuResource*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuResource*, nuint>)(lpVtbl[1]))((FGpuResource*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuResource*, nuint>)(lpVtbl[2]))((FGpuResource*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuResource*, nuint>)(lpVtbl[3]))((FGpuResource*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuResource*, nuint>)(lpVtbl[4]))((FGpuResource*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuResource*, B8>)(lpVtbl[5]))((FGpuResource*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuResource*, B8>)(lpVtbl[6]))((FGpuResource*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuResource*, void*>)(lpVtbl[7]))((FGpuResource*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FGpuResource*, Guid, void*>)(lpVtbl[8]))((FGpuResource*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError RawPtr(void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGpuResource*, FError*, void**, FError*>)(lpVtbl[9]))((FGpuResource*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError DataPtr(FGpuResourceData** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGpuResource*, FError*, FGpuResourceData**, FError*>)(lpVtbl[10]))((FGpuResource*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError SetName([NativeTypeName("const wchar_t *")] char* name)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGpuResource*, FError*, char*, FError*>)(lpVtbl[11]))((FGpuResource*)Unsafe.AsPointer(ref this), &result, name);
        }
    }

    [NativeTypeName("struct FGpuBuffer : cc::FGpuResource")]
    public unsafe partial struct FGpuBuffer
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "01d2e268-62c6-4175-855f-88c9ac5f2f86";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FGpuBuffer*, void>)(lpVtbl[0]))((FGpuBuffer*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuBuffer*, nuint>)(lpVtbl[1]))((FGpuBuffer*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuBuffer*, nuint>)(lpVtbl[2]))((FGpuBuffer*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuBuffer*, nuint>)(lpVtbl[3]))((FGpuBuffer*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuBuffer*, nuint>)(lpVtbl[4]))((FGpuBuffer*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuBuffer*, B8>)(lpVtbl[5]))((FGpuBuffer*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuBuffer*, B8>)(lpVtbl[6]))((FGpuBuffer*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuBuffer*, void*>)(lpVtbl[7]))((FGpuBuffer*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FGpuBuffer*, Guid, void*>)(lpVtbl[8]))((FGpuBuffer*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError RawPtr(void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGpuBuffer*, FError*, void**, FError*>)(lpVtbl[9]))((FGpuBuffer*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError DataPtr(FGpuResourceData** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGpuBuffer*, FError*, FGpuResourceData**, FError*>)(lpVtbl[10]))((FGpuBuffer*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError SetName([NativeTypeName("const wchar_t *")] char* name)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGpuBuffer*, FError*, char*, FError*>)(lpVtbl[11]))((FGpuBuffer*)Unsafe.AsPointer(ref this), &result, name);
        }
    }

    [NativeTypeName("struct FGpuTexture : cc::FGpuResource")]
    public unsafe partial struct FGpuTexture
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "7af4b0c3-13dd-4897-b807-81253c4a3e2e";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FGpuTexture*, void>)(lpVtbl[0]))((FGpuTexture*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuTexture*, nuint>)(lpVtbl[1]))((FGpuTexture*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuTexture*, nuint>)(lpVtbl[2]))((FGpuTexture*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuTexture*, nuint>)(lpVtbl[3]))((FGpuTexture*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuTexture*, nuint>)(lpVtbl[4]))((FGpuTexture*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuTexture*, B8>)(lpVtbl[5]))((FGpuTexture*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuTexture*, B8>)(lpVtbl[6]))((FGpuTexture*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuTexture*, void*>)(lpVtbl[7]))((FGpuTexture*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FGpuTexture*, Guid, void*>)(lpVtbl[8]))((FGpuTexture*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError RawPtr(void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGpuTexture*, FError*, void**, FError*>)(lpVtbl[9]))((FGpuTexture*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError DataPtr(FGpuResourceData** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGpuTexture*, FError*, FGpuResourceData**, FError*>)(lpVtbl[10]))((FGpuTexture*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError SetName([NativeTypeName("const wchar_t *")] char* name)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGpuTexture*, FError*, char*, FError*>)(lpVtbl[11]))((FGpuTexture*)Unsafe.AsPointer(ref this), &result, name);
        }
    }

    public partial struct FGpuBufferCreateOptions
    {
        [NativeTypeName("cc::u32")]
        public uint size;

        [NativeTypeName("cc::FGpuResourceState")]
        public FGpuResourceState initial_state;

        [NativeTypeName("cc::GpuHeapType")]
        public GpuHeapType heap_type;

        [NativeTypeName("cc::b8")]
        public B8 uav;
    }

    [NativeTypeName("struct FGpuGraph : cc::IObject")]
    public unsafe partial struct FGpuGraph
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "e3f38929-74e9-4df0-8001-e82eed2a23f7";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FGpuGraph*, void>)(lpVtbl[0]))((FGpuGraph*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuGraph*, nuint>)(lpVtbl[1]))((FGpuGraph*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuGraph*, nuint>)(lpVtbl[2]))((FGpuGraph*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuGraph*, nuint>)(lpVtbl[3]))((FGpuGraph*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuGraph*, nuint>)(lpVtbl[4]))((FGpuGraph*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuGraph*, B8>)(lpVtbl[5]))((FGpuGraph*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuGraph*, B8>)(lpVtbl[6]))((FGpuGraph*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FGpuGraph*, void*>)(lpVtbl[7]))((FGpuGraph*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FGpuGraph*, Guid, void*>)(lpVtbl[8]))((FGpuGraph*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError ExecuteCommand([NativeTypeName("cc::gpu::FGpuStreamCommands")] FGpuStreamCommands cmds)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGpuGraph*, FError*, FGpuStreamCommands, FError*>)(lpVtbl[9]))((FGpuGraph*)Unsafe.AsPointer(ref this), &result, cmds);
        }
    }

    [NativeTypeName("cc::u8")]
    public enum FGpuCommandOp : byte
    {
        Nop,
        ClearRtv,
        ClearDsv,
        SetRt,
        SetViewPort,
        SetScissorRect,
        SetShader,
        DrawInstanced,
        Dispatch,
        DispatchMesh,
    }

    public partial struct FGpuCommandClearRtv
    {
        [NativeTypeName("cc::f32")]
        public float color_r;

        [NativeTypeName("cc::f32")]
        public float color_g;

        [NativeTypeName("cc::f32")]
        public float color_b;

        [NativeTypeName("cc::f32")]
        public float color_a;

        [NativeTypeName("cc::u32")]
        public uint rects;
    }

    public partial struct FGpuCommandClearDsv
    {
        [NativeTypeName("cc::u32")]
        public uint rects;

        [NativeTypeName("cc::f32")]
        public float depth;

        [NativeTypeName("cc::u8")]
        public byte stencil;

        [NativeTypeName("__AnonymousRecord_Rendering_L201_C13")]
        public _flags_e__Struct flags;

        public partial struct _flags_e__Struct
        {
            public byte _bitfield;

            [NativeTypeName("cc::u8 : 1")]
            public byte depth
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                readonly get
                {
                    return (byte)(_bitfield & 0x1u);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set
                {
                    _bitfield = (byte)((_bitfield & ~0x1u) | (value & 0x1u));
                }
            }

            [NativeTypeName("cc::u8 : 1")]
            public byte stencil
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                readonly get
                {
                    return (byte)((_bitfield >> 1) & 0x1u);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set
                {
                    _bitfield = (byte)((_bitfield & ~(0x1u << 1)) | ((value & 0x1u) << 1));
                }
            }
        }
    }

    public partial struct FGpuCommandSetRt
    {
        [NativeTypeName("cc::u8")]
        public byte rtv_count;

        [NativeTypeName("cc::b8")]
        public B8 has_dsv;
    }

    public partial struct FGpuCommandSetViewPort
    {
        [NativeTypeName("cc::u8")]
        public byte count;
    }

    public partial struct FGpuCommandSetScissorRect
    {
        [NativeTypeName("cc::u8")]
        public byte count;
    }

    public unsafe partial struct FGpuCommandSetShader
    {
        [NativeTypeName("cc::FShaderPass *")]
        public FShaderPass* pass;
    }

    public partial struct FGpuCommandDrawInstanced
    {
        [NativeTypeName("cc::u32")]
        public uint vertex_count_per_instance;

        [NativeTypeName("cc::u32")]
        public uint instance_count;

        [NativeTypeName("cc::u32")]
        public uint start_vertex_location;

        [NativeTypeName("cc::u32")]
        public uint start_instance_location;
    }

    public partial struct FGpuCommandDispatch
    {
        [NativeTypeName("cc::u32")]
        public uint thread_group_count_x;

        [NativeTypeName("cc::u32")]
        public uint thread_group_count_y;

        [NativeTypeName("cc::u32")]
        public uint thread_group_count_z;
    }

    public unsafe partial struct FGpuStreamCommands
    {
        public FGpuCommandOp** stream;

        [NativeTypeName("size_t")]
        public nuint count;
    }

    [NativeTypeName("uint8_t")]
    public enum Switch : byte
    {
        Off = 0,
        On = 1,
    }

    [NativeTypeName("uint8_t")]
    public enum PrimitiveTopologyType : byte
    {
        Triangle,
        TriangleStrip,
        Point,
        Line,
        LineStrip,
    }

    public partial struct FColorMask
    {
        public byte _bitfield;

        [NativeTypeName("uint8_t : 1")]
        public byte r
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return (byte)(_bitfield & 0x1u);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield = (byte)((_bitfield & ~0x1u) | (value & 0x1u));
            }
        }

        [NativeTypeName("uint8_t : 1")]
        public byte g
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return (byte)((_bitfield >> 1) & 0x1u);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield = (byte)((_bitfield & ~(0x1u << 1)) | ((value & 0x1u) << 1));
            }
        }

        [NativeTypeName("uint8_t : 1")]
        public byte b
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return (byte)((_bitfield >> 2) & 0x1u);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield = (byte)((_bitfield & ~(0x1u << 2)) | ((value & 0x1u) << 2));
            }
        }

        [NativeTypeName("uint8_t : 1")]
        public byte a
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return (byte)((_bitfield >> 3) & 0x1u);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield = (byte)((_bitfield & ~(0x1u << 3)) | ((value & 0x1u) << 3));
            }
        }

        public FColorMask([NativeTypeName("const bool")] bool r = true, [NativeTypeName("const bool")] bool g = true, [NativeTypeName("const bool")] bool b = true, [NativeTypeName("const bool")] bool a = true)
        {
            this.r = (byte)((r) ? 1 : 0);
            this.g = (byte)((g) ? 1 : 0);
            this.b = (byte)((b) ? 1 : 0);
            this.a = (byte)((a) ? 1 : 0);
        }

        public bool all()
        {
            return (r) != 0 && (g) != 0 && (b) != 0 && (a) != 0;
        }
    }

    [NativeTypeName("uint8_t")]
    public enum BlendType : byte
    {
        Zero = 1,
        One = 2,
        SrcColor = 3,
        InvSrcColor = 4,
        SrcAlpha = 5,
        InvSrcAlpha = 6,
        DstAlpha = 7,
        InvDstAlpha = 8,
        DstColor = 9,
        InvDstColor = 10,
        SrcAlphaSat = 11,
        BlendFactor = 14,
        BlendInvBlendFactor = 15,
        Src1Color = 16,
        InvSrc1Color = 17,
        Src1Alpha = 18,
        InvSrc1Alpha = 19,
        AlphaFactor = 20,
        InvAlphaFactor = 21,
    }

    [NativeTypeName("uint8_t")]
    public enum BlendOp : byte
    {
        None = 0,
        Add = 1,
        Sub = 2,
        RevSub = 3,
        Min = 4,
        Max = 5,
    }

    [NativeTypeName("uint8_t")]
    public enum LogicOp : byte
    {
        None = 0,
        Clear,
        One,
        Copy,
        CopyInv,
        Noop,
        Inv,
        And,
        NAnd,
        Or,
        Nor,
        Xor,
        Equiv,
        AndRev,
        AndInv,
        OrRev,
        OrInv,
    }

    [NativeTypeName("uint8_t")]
    public enum FillMode : byte
    {
        WireFrame = 2,
        Solid = 3,
    }

    [NativeTypeName("uint8_t")]
    public enum CullMode : byte
    {
        Off = 1,
        Front = 2,
        Back = 3,
    }

    [NativeTypeName("uint8_t")]
    public enum CmpFunc : byte
    {
        Off = 0,
        Never = 1,
        Less = 2,
        Equal = 3,
        LessEqual = 4,
        Greater = 5,
        NotEqual = 6,
        GreaterEqual = 7,
        Always = 8,
    }

    [NativeTypeName("uint8_t")]
    public enum StencilFailOp : byte
    {
        Keep = 1,
        Zero = 2,
        Replace = 3,
        IncSat = 4,
        DecSat = 5,
        Invert = 6,
        Inc = 7,
        Dec = 8,
    }

    [NativeTypeName("uint8_t")]
    public enum DepthWriteMask : byte
    {
        Zero = 0,
        All = 1,
    }

    public partial struct RtBlendState
    {
        [NativeTypeName("cc::Switch")]
        public Switch blend;

        [NativeTypeName("cc::BlendType")]
        public BlendType src_blend;

        [NativeTypeName("cc::BlendType")]
        public BlendType dst_blend;

        [NativeTypeName("cc::BlendOp")]
        public BlendOp blend_op;

        [NativeTypeName("cc::BlendType")]
        public BlendType src_alpha_blend;

        [NativeTypeName("cc::BlendType")]
        public BlendType dst_alpha_blend;

        [NativeTypeName("cc::BlendOp")]
        public BlendOp alpha_blend_op;

        [NativeTypeName("cc::LogicOp")]
        public LogicOp logic_op;

        [NativeTypeName("cc::FColorMask")]
        public FColorMask write_mask;

        public void AllowWriteAlpha()
        {
            write_mask.a = (byte)((true) ? 1 : 0);
        }

        public void UseAlphaBlend()
        {
            src_blend = SrcAlpha;
            dst_blend = InvSrcAlpha;
            src_alpha_blend = SrcAlpha;
            dst_alpha_blend = InvSrcAlpha;
            alpha_blend_op = Add;
        }

        public void UsePreMultiplied()
        {
            src_blend = One;
            dst_blend = InvSrcAlpha;
            src_alpha_blend = One;
            dst_alpha_blend = InvSrcAlpha;
            alpha_blend_op = Add;
        }
    }

    public partial struct BlendState
    {
        [NativeTypeName("RtBlendState[8]")]
        public _rts_e__FixedBuffer rts;

        [NativeTypeName("cc::Switch")]
        public Switch alpha_to_coverage;

        [NativeTypeName("cc::Switch")]
        public Switch independent_blend;

        [InlineArray(8)]
        public partial struct _rts_e__FixedBuffer
        {
            public RtBlendState e0;
        }
    }

    public partial struct RasterizerState
    {
        [NativeTypeName("cc::FillMode")]
        public FillMode fill_mode;

        [NativeTypeName("cc::CullMode")]
        public CullMode cull_mode;

        [NativeTypeName("cc::Switch")]
        public Switch depth_clip;

        [NativeTypeName("cc::Switch")]
        public Switch multisample;

        [NativeTypeName("uint32_t")]
        public uint forced_sample_count;

        [NativeTypeName("int32_t")]
        public int depth_bias;

        public float depth_bias_clamp;

        public float slope_scaled_depth_bias;

        [NativeTypeName("cc::Switch")]
        public Switch aa_line;

        [NativeTypeName("cc::Switch")]
        public Switch conservative;
    }

    public partial struct StencilState
    {
        [NativeTypeName("cc::StencilFailOp")]
        public StencilFailOp fail_op;

        [NativeTypeName("cc::StencilFailOp")]
        public StencilFailOp depth_fail_op;

        [NativeTypeName("cc::StencilFailOp")]
        public StencilFailOp pass_op;

        [NativeTypeName("cc::CmpFunc")]
        public CmpFunc func;
    }

    public partial struct DepthStencilState
    {
        [NativeTypeName("cc::CmpFunc")]
        public CmpFunc depth_func;

        [NativeTypeName("cc::DepthWriteMask")]
        public DepthWriteMask depth_write_mask;

        [NativeTypeName("cc::Switch")]
        public Switch stencil_enable;

        [NativeTypeName("uint8_t")]
        public byte stencil_read_mask;

        [NativeTypeName("uint8_t")]
        public byte stencil_write_mask;

        [NativeTypeName("cc::StencilState")]
        public StencilState front_face;

        [NativeTypeName("cc::StencilState")]
        public StencilState back_face;
    }

    public partial struct SampleState
    {
        [NativeTypeName("uint32_t")]
        public uint count;

        [NativeTypeName("int32_t")]
        public int quality;
    }

    public partial struct GraphicsPipelineState
    {
        [NativeTypeName("cc::BlendState")]
        public BlendState blend_state;

        [NativeTypeName("cc::PrimitiveTopologyType")]
        public PrimitiveTopologyType primitive_topology;

        [NativeTypeName("cc::RasterizerState")]
        public RasterizerState rasterizer_state;

        [NativeTypeName("cc::DepthStencilState")]
        public DepthStencilState depth_stencil_state;

        [NativeTypeName("uint32_t")]
        public uint sample_mask;

        [NativeTypeName("int32_t")]
        public int rt_count;

        [NativeTypeName("TextureFormat[8]")]
        public _rtv_formats_e__FixedBuffer rtv_formats;

        [NativeTypeName("cc::TextureFormat")]
        public TextureFormat dsv_format;

        [NativeTypeName("cc::SampleState")]
        public SampleState sample_state;

        [InlineArray(8)]
        public partial struct _rtv_formats_e__FixedBuffer
        {
            public TextureFormat e0;
        }
    }

    public partial struct GraphicsPipelineFormatOverride
    {
        [NativeTypeName("int32_t")]
        public int rt_count;

        [NativeTypeName("TextureFormat[8]")]
        public _rtv_formats_e__FixedBuffer rtv_formats;

        [NativeTypeName("cc::TextureFormat")]
        public TextureFormat dsv_format;

        [InlineArray(8)]
        public partial struct _rtv_formats_e__FixedBuffer
        {
            public TextureFormat e0;
        }
    }

    [NativeTypeName("uint8_t")]
    public enum ShaderStage : byte
    {
        Unknown,
        Cs,
        Ps,
        Vs,
        Ms,
        Ts,
    }

    public partial struct FShaderStages
    {
        public byte _bitfield;

        [NativeTypeName("uint8_t : 1")]
        public byte cs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return (byte)(_bitfield & 0x1u);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield = (byte)((_bitfield & ~0x1u) | (value & 0x1u));
            }
        }

        [NativeTypeName("uint8_t : 1")]
        public byte ps
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return (byte)((_bitfield >> 1) & 0x1u);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield = (byte)((_bitfield & ~(0x1u << 1)) | ((value & 0x1u) << 1));
            }
        }

        [NativeTypeName("uint8_t : 1")]
        public byte vs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return (byte)((_bitfield >> 2) & 0x1u);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield = (byte)((_bitfield & ~(0x1u << 2)) | ((value & 0x1u) << 2));
            }
        }

        [NativeTypeName("uint8_t : 1")]
        public byte ms
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return (byte)((_bitfield >> 3) & 0x1u);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield = (byte)((_bitfield & ~(0x1u << 3)) | ((value & 0x1u) << 3));
            }
        }

        [NativeTypeName("uint8_t : 1")]
        public byte ts
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return (byte)((_bitfield >> 4) & 0x1u);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield = (byte)((_bitfield & ~(0x1u << 4)) | ((value & 0x1u) << 4));
            }
        }
    }

    public partial struct FShaderPassData
    {
        [NativeTypeName("cc::GraphicsPipelineState")]
        public GraphicsPipelineState state;

        [NativeTypeName("FrBlob[3]")]
        public _modules_e__FixedBuffer modules;

        [NativeTypeName("cc::FShaderStages")]
        public FShaderStages stages;

        [NativeTypeName("const size_t")]
        public const nuint MaxModules = 3;

        [InlineArray(3)]
        public partial struct _modules_e__FixedBuffer
        {
            public FrBlob e0;
        }
    }

    [NativeTypeName("struct FShaderPass : cc::IObject")]
    public unsafe partial struct FShaderPass
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "60e8339a-dfc0-4f91-8550-d14a3836e3c3";

        [NativeTypeName("const size_t")]
        public const nuint MaxModules = 3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FShaderPass*, void>)(lpVtbl[0]))((FShaderPass*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPass*, nuint>)(lpVtbl[1]))((FShaderPass*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPass*, nuint>)(lpVtbl[2]))((FShaderPass*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPass*, nuint>)(lpVtbl[3]))((FShaderPass*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPass*, nuint>)(lpVtbl[4]))((FShaderPass*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPass*, B8>)(lpVtbl[5]))((FShaderPass*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPass*, B8>)(lpVtbl[6]))((FShaderPass*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPass*, void*>)(lpVtbl[7]))((FShaderPass*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPass*, Guid, void*>)(lpVtbl[8]))((FShaderPass*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError DataPtr(FShaderPassData** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FShaderPass*, FError*, FShaderPassData**, FError*>)(lpVtbl[9]))((FShaderPass*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError GetOrCreateGraphicsPipeline([NativeTypeName("const GraphicsPipelineFormatOverride *")] GraphicsPipelineFormatOverride* @override, FGraphicsShaderPipeline** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FShaderPass*, FError*, GraphicsPipelineFormatOverride*, FGraphicsShaderPipeline**, FError*>)(lpVtbl[10]))((FShaderPass*)Unsafe.AsPointer(ref this), &result, @override, @out);
        }
    }

    [NativeTypeName("struct FShaderPipeline : cc::IObject")]
    public unsafe partial struct FShaderPipeline
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "148fe4f0-51ad-464d-ad39-b8b06bc359af";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FShaderPipeline*, void>)(lpVtbl[0]))((FShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPipeline*, nuint>)(lpVtbl[1]))((FShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPipeline*, nuint>)(lpVtbl[2]))((FShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPipeline*, nuint>)(lpVtbl[3]))((FShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPipeline*, nuint>)(lpVtbl[4]))((FShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPipeline*, B8>)(lpVtbl[5]))((FShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPipeline*, B8>)(lpVtbl[6]))((FShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPipeline*, void*>)(lpVtbl[7]))((FShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FShaderPipeline*, Guid, void*>)(lpVtbl[8]))((FShaderPipeline*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError RawPtr(void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FShaderPipeline*, FError*, void**, FError*>)(lpVtbl[9]))((FShaderPipeline*)Unsafe.AsPointer(ref this), &result, @out);
        }
    }

    [NativeTypeName("struct FGraphicsShaderPipeline : cc::FShaderPipeline")]
    public unsafe partial struct FGraphicsShaderPipeline
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "3fa74850-cdd9-47eb-8ae3-bfc090f7077a";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FGraphicsShaderPipeline*, void>)(lpVtbl[0]))((FGraphicsShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicsShaderPipeline*, nuint>)(lpVtbl[1]))((FGraphicsShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicsShaderPipeline*, nuint>)(lpVtbl[2]))((FGraphicsShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicsShaderPipeline*, nuint>)(lpVtbl[3]))((FGraphicsShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicsShaderPipeline*, nuint>)(lpVtbl[4]))((FGraphicsShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicsShaderPipeline*, B8>)(lpVtbl[5]))((FGraphicsShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicsShaderPipeline*, B8>)(lpVtbl[6]))((FGraphicsShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicsShaderPipeline*, void*>)(lpVtbl[7]))((FGraphicsShaderPipeline*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FGraphicsShaderPipeline*, Guid, void*>)(lpVtbl[8]))((FGraphicsShaderPipeline*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError RawPtr(void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGraphicsShaderPipeline*, FError*, void**, FError*>)(lpVtbl[9]))((FGraphicsShaderPipeline*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError StatePtr([NativeTypeName("const GraphicsPipelineState **")] GraphicsPipelineState** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FGraphicsShaderPipeline*, FError*, GraphicsPipelineState**, FError*>)(lpVtbl[10]))((FGraphicsShaderPipeline*)Unsafe.AsPointer(ref this), &result, @out);
        }
    }

    public enum TextureFormat
    {
        Unknown = 0,
        R32G32B32A32_TypeLess = 1,
        R32G32B32A32_Float = 2,
        R32G32B32A32_UInt = 3,
        R32G32B32A32_SInt = 4,
        R32G32B32_TypeLess = 5,
        R32G32B32_Float = 6,
        R32G32B32_UInt = 7,
        R32G32B32_SInt = 8,
        R16G16B16A16_TypeLess = 9,
        R16G16B16A16_Float = 10,
        R16G16B16A16_UNorm = 11,
        R16G16B16A16_UInt = 12,
        R16G16B16A16_SNorm = 13,
        R16G16B16A16_SInt = 14,
        R32G32_TypeLess = 15,
        R32G32_Float = 16,
        R32G32_UInt = 17,
        R32G32_SInt = 18,
        R32G8X24_TypeLess = 19,
        D32_Float_S8X24_UInt = 20,
        R32_Float_X8X24_TypeLess = 21,
        X32_TypeLess_G8X24_Float = 22,
        R10G10B10A2_TypeLess = 23,
        R10G10B10A2_UNorm = 24,
        R10G10B10A2_UInt = 25,
        R11G11B10_Float = 26,
        R8G8B8A8_TypeLess = 27,
        R8G8B8A8_UNorm = 28,
        R8G8B8A8_UNorm_sRGB = 29,
        R8G8B8A8_UInt = 30,
        R8G8B8A8_SNorm = 31,
        R8G8B8A8_SInt = 32,
        R16G16_TypeLess = 33,
        R16G16_Float = 34,
        R16G16_UNorm = 35,
        R16G16_UInt = 36,
        R16G16_SNorm = 37,
        R16G16_SInt = 38,
        R32_TypeLess = 39,
        D32_Float = 40,
        R32_Float = 41,
        R32_UInt = 42,
        R32_SInt = 43,
        R24G8_TypeLess = 44,
        D24_UNorm_S8_UInt = 45,
        R24_UNorm_X8_TypeLess = 46,
        X24_TypeLess_G8_UInt = 47,
        R8G8_TypeLess = 48,
        R8G8_UNorm = 49,
        R8G8_UInt = 50,
        R8G8_SNorm = 51,
        R8G8_SInt = 52,
        R16_TypeLess = 53,
        R16_Float = 54,
        D16_UNorm = 55,
        R16_UNorm = 56,
        R16_UInt = 57,
        R16_SNorm = 58,
        R16_SInt = 59,
        R8_TypeLess = 60,
        R8_UNorm = 61,
        R8_UInt = 62,
        R8_SNorm = 63,
        R8_SInt = 64,
        A8_UNorm = 65,
        R1_UNorm = 66,
        R9G9B9E5_SharedExp = 67,
        R8G8_B8G8_UNorm = 68,
        G8R8_G8B8_UNorm = 69,
        BC1_TypeLess = 70,
        BC1_UNorm = 71,
        BC1_UNorm_sRGB = 72,
        BC2_TypeLess = 73,
        BC2_UNorm = 74,
        BC2_UNorm_sRGB = 75,
        BC3_TypeLess = 76,
        BC3_UNorm = 77,
        BC3_UNorm_sRGB = 78,
        BC4_TypeLess = 79,
        BC4_UNorm = 80,
        BC4_SNorm = 81,
        BC5_TypeLess = 82,
        BC5_UNorm = 83,
        BC5_SNorm = 84,
        B5G6R5_UNorm = 85,
        B5G5R5A1_UNorm = 86,
        B8G8R8A8_UNorm = 87,
        B8G8R8X8_UNorm = 88,
        R10G10B10_XR_Bias_A2_UNorm = 89,
        B8G8R8A8_TypeLess = 90,
        B8G8R8A8_UNorm_sRGB = 91,
        B8G8R8X8_TypeLess = 92,
        B8G8R8X8_UNorm_sRGB = 93,
        BC6H_TypeLess = 94,
        BC6H_UF16 = 95,
        BC6H_SF16 = 96,
        BC7_TypeLess = 97,
        BC7_UNorm = 98,
        BC7_UNorm_sRGB = 99,
    }

    public partial struct FChunkedVectorConsts
    {

        [NativeTypeName("const int32_t")]
        public const int ChunkSize = 1024 * 16;

        [NativeTypeName("const int32_t")]
        public const int InitCapacity = 1;
    }

    [NativeTypeName("struct FChunkedVectorData : cc::IObject, cc::FChunkedVectorConsts")]
    public unsafe partial struct FChunkedVectorData
    {
        public void** lpVtbl;

        [NativeTypeName("size_t")]
        public nuint type_size;

        public void** chunks;

        [NativeTypeName("size_t")]
        public nuint chunk_count;

        [NativeTypeName("size_t")]
        public nuint count;

        [NativeTypeName("size_t")]
        public nuint cur_chunk;

        [NativeTypeName("size_t")]
        public nuint cur_in_chunk;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "270057f3-e830-4bca-9fab-0b2fa08bbbdf";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FChunkedVectorData*, void>)(lpVtbl[0]))((FChunkedVectorData*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FChunkedVectorData*, nuint>)(lpVtbl[1]))((FChunkedVectorData*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FChunkedVectorData*, nuint>)(lpVtbl[2]))((FChunkedVectorData*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FChunkedVectorData*, nuint>)(lpVtbl[3]))((FChunkedVectorData*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FChunkedVectorData*, nuint>)(lpVtbl[4]))((FChunkedVectorData*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FChunkedVectorData*, B8>)(lpVtbl[5]))((FChunkedVectorData*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FChunkedVectorData*, B8>)(lpVtbl[6]))((FChunkedVectorData*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FChunkedVectorData*, void*>)(lpVtbl[7]))((FChunkedVectorData*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FChunkedVectorData*, Guid, void*>)(lpVtbl[8]))((FChunkedVectorData*)Unsafe.AsPointer(ref this), id);
        }
    }
}
