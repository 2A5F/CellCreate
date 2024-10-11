using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Game.Native
{
    public unsafe partial struct FrStr8
    {
        [NativeTypeName("const Char8 *")]
        private byte* m_data;

        [NativeTypeName("const size_t")]
        private nuint m_size;

        public FrStr8([NativeTypeName("const Char8 *")] byte* data, [NativeTypeName("const size_t")] nuint size)
        {
            m_data = data;
            m_size = size;
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
        [NativeTypeName("const Char16 *")]
        private char* m_data;

        [NativeTypeName("const size_t")]
        private nuint m_size;

        public FrStr16([NativeTypeName("const Char16 *")] char* data, [NativeTypeName("const size_t")] nuint size)
        {
            m_data = data;
            m_size = size;
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

        [NativeTypeName("__AnonymousRecord_FFI_L122_C9")]
        public _Anonymous_e__Union Anonymous;

        [UnscopedRef]
        public ref sbyte* c_str
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
            public sbyte* c_str;

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

    public unsafe partial struct AppFnVtb
    {
        [NativeTypeName("cc::FFI_Action *")]
        public delegate* unmanaged[Cdecl]<void> main;
    }

    public partial struct AppVars
    {
        [NativeTypeName("cc::b8")]
        public B8 debug;
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

    public partial struct TestStruct
    {
        [NativeTypeName("cc::float2")]
        public float2 a;

        [NativeTypeName("cc::float3")]
        public float3 b;

        [NativeTypeName("cc::float4")]
        public float4 c;
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
        public FError Size([NativeTypeName("cc::uint2 &")] uint2* @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FWindowHandle*, FError*, uint2*, FError*>)(lpVtbl[9]))((FWindowHandle*)Unsafe.AsPointer(ref this), &result, @out);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NativeTypeName("cc::FError")]
        public FError PixelSize([NativeTypeName("cc::uint2 &")] uint2* @out)
        {
            FError result;
            return *((delegate* unmanaged[Thiscall]<FWindowHandle*, FError*, uint2*, FError*>)(lpVtbl[10]))((FWindowHandle*)Unsafe.AsPointer(ref this), &result, @out);
        }
    }
}
