using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Game.Native
{
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

        [NativeTypeName("__AnonymousRecord_FFI_L173_C9")]
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
        public FError Init()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FError*>)(lpVtbl[9]))((FApp*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError Exit()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FError*>)(lpVtbl[10]))((FApp*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FMessageVtb *")]
        public FMessageVtb* MsgVtb()
        {
            return ((delegate* unmanaged[Thiscall]<FApp*, FMessageVtb*>)(lpVtbl[11]))((FApp*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError MsgPump()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FError*>)(lpVtbl[12]))((FApp*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError SendMsg([NativeTypeName("cc::FMessage")] FMessage type, void* data)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FMessage, void*, FError*>)(lpVtbl[13]))((FApp*)Unsafe.AsPointer(ref this), &result, type, data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CreateWindowHandle([NativeTypeName("cc::FWindowCreateOptions &")] FWindowCreateOptions* options, [NativeTypeName("FWindowHandle *&")] FWindowHandle** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FWindowCreateOptions*, FWindowHandle**, FError*>)(lpVtbl[14]))((FApp*)Unsafe.AsPointer(ref this), &result, options, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CreateRendering([NativeTypeName("FRendering *&")] FRendering** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FApp*, FError*, FRendering**, FError*>)(lpVtbl[15]))((FApp*)Unsafe.AsPointer(ref this), &result, @out);
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

    public enum FMessage
    {
        Nop,
        SwitchThread,
        WindowClose,
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
        [return: NativeTypeName("cc::FError")]
        public FError Init([NativeTypeName("cc::FWindowHandle *")] FWindowHandle* window_handle)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FWindowHandle*, FError*>)(lpVtbl[9]))((FRendering*)Unsafe.AsPointer(ref this), &result, window_handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError OnResize([NativeTypeName("cc::uint2")] uint2 size)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, uint2, FError*>)(lpVtbl[10]))((FRendering*)Unsafe.AsPointer(ref this), &result, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 VSync()
        {
            return ((delegate* unmanaged[Thiscall]<FRendering*, B8>)(lpVtbl[11]))((FRendering*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError SetVSync([NativeTypeName("cc::b8")] B8 enable)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, B8, FError*>)(lpVtbl[12]))((FRendering*)Unsafe.AsPointer(ref this), &result, enable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError ReadyFrame()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FError*>)(lpVtbl[13]))((FRendering*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError EndFrame()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FError*>)(lpVtbl[14]))((FRendering*)Unsafe.AsPointer(ref this), &result);
        }
    }
}
