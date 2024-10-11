using System.Runtime.InteropServices;
using System.Text;

namespace Game.Native;

public partial struct FError
{
    public bool IsNone => type == FErrorType.None;

    public void Throw() => throw ToException();

    public unsafe string GetString() => msg_type switch
    {
        FErrorMsgType.CStr => Marshal.PtrToStringAuto((IntPtr)c_str)!,
        FErrorMsgType.Str8 => str8.ToString(),
        FErrorMsgType.Str16 => str16.ToString(),
        _ => "There is no error"
    };

    public override string ToString() => GetString();

    public Exception ToException() => throw type switch
    {
        FErrorType.None => new ArgumentException("There is no error"),
        _ => new NativeException(GetString())
    };

    public void TryThrow()
    {
        if (IsNone) return;
        else Throw();
    }
}
