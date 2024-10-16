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
        public FError MsgLoop()
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

    public partial struct FCommandList
    {
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
        public FError MakeContext([NativeTypeName("cc::FWindowHandle *")] FWindowHandle* window_handle, FRenderingContext** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FWindowHandle*, FRenderingContext**, FError*>)(lpVtbl[10]))((FRendering*)Unsafe.AsPointer(ref this), &result, window_handle, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CreateGraphicsShaderPipeline([NativeTypeName("cc::FShaderPassData *")] FShaderPassData* pass, FGraphicsShaderPipeline** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FShaderPassData*, FGraphicsShaderPipeline**, FError*>)(lpVtbl[11]))((FRendering*)Unsafe.AsPointer(ref this), &result, pass, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError ReadyFrame()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FError*>)(lpVtbl[12]))((FRendering*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError EndFrame()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FError*>)(lpVtbl[13]))((FRendering*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError GetDevice(void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, void**, FError*>)(lpVtbl[14]))((FRendering*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError CurrentCommandList(void** @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, void**, FError*>)(lpVtbl[15]))((FRendering*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError ClearSurface([NativeTypeName("cc::FRenderingContext *")] FRenderingContext* ctx, [NativeTypeName("cc::float4")] float4 color)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRendering*, FError*, FRenderingContext*, float4, FError*>)(lpVtbl[16]))((FRendering*)Unsafe.AsPointer(ref this), &result, ctx, color);
        }
    }

    [NativeTypeName("struct FRenderingContext : cc::IObject, cc::FGpuConsts")]
    public unsafe partial struct FRenderingContext
    {
        public void** lpVtbl;

        [NativeTypeName("const wchar_t *const")]
        public const string s_FFI_UUID = "b0378104-80ee-4272-9c58-3af6e35ec437";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<FRenderingContext*, void>)(lpVtbl[0]))((FRenderingContext*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<FRenderingContext*, nuint>)(lpVtbl[1]))((FRenderingContext*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint Release()
        {
            return ((delegate* unmanaged[Thiscall]<FRenderingContext*, nuint>)(lpVtbl[2]))((FRenderingContext*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint AddRefWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FRenderingContext*, nuint>)(lpVtbl[3]))((FRenderingContext*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("size_t")]
        public nuint ReleaseWeak()
        {
            return ((delegate* unmanaged[Thiscall]<FRenderingContext*, nuint>)(lpVtbl[4]))((FRenderingContext*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryDowngrade()
        {
            return ((delegate* unmanaged[Thiscall]<FRenderingContext*, B8>)(lpVtbl[5]))((FRenderingContext*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::b8")]
        public B8 TryUpgrade()
        {
            return ((delegate* unmanaged[Thiscall]<FRenderingContext*, B8>)(lpVtbl[6]))((FRenderingContext*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* ObjectStart()
        {
            return ((delegate* unmanaged[Thiscall]<FRenderingContext*, void*>)(lpVtbl[7]))((FRenderingContext*)Unsafe.AsPointer(ref this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* QueryInterface([NativeTypeName("cc::uuid")] Guid id)
        {
            return ((delegate* unmanaged[Thiscall]<FRenderingContext*, Guid, void*>)(lpVtbl[8]))((FRenderingContext*)Unsafe.AsPointer(ref this), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError Destroy()
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRenderingContext*, FError*, FError*>)(lpVtbl[9]))((FRenderingContext*)Unsafe.AsPointer(ref this), &result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError OnResize([NativeTypeName("cc::uint2")] uint2 size)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FRenderingContext*, FError*, uint2, FError*>)(lpVtbl[10]))((FRenderingContext*)Unsafe.AsPointer(ref this), &result, size);
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
        public byte vs
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
        public byte ps
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
        public FError StatePtr(GraphicsPipelineState** @out)
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
}
