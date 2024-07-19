﻿using System.Runtime.InteropServices;
using System.Text;

namespace Soario.Native;

public class SoarioNativeException : Exception
{
    public SoarioNativeException() { }
    public SoarioNativeException(string message) : base(message) { }
    public SoarioNativeException(string message, Exception inner) : base(message, inner) { }
}

public partial struct FError
{
    public static bool operator true(FError e) => e.type is not FErrorType.None;
    public static bool operator false(FError e) => e.type is FErrorType.None;

    public unsafe void Throw()
    {
        if (type is FErrorType.None) throw new SoarioNativeException("Not A Error");
        throw msg_type switch
        {
            FErrorMsgType.Utf8c => new SoarioNativeException(Marshal.PtrToStringUTF8((IntPtr)msg.u8c)!),
            FErrorMsgType.Utf16c => new SoarioNativeException(Marshal.PtrToStringUni((IntPtr)msg.u16c)!),
            FErrorMsgType.Utf8s => new SoarioNativeException(Encoding.UTF8.GetString(msg.u8s.AsSpan())),
            FErrorMsgType.Utf16s => new SoarioNativeException(string.Create((int)msg.u16s.len, msg.u16s,
                static (span, str16) => str16.AsSpan().CopyTo(span))),
            _ => new ArgumentOutOfRangeException()
        };
    }
}