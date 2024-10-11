namespace Game.Native;

public class NativeException : Exception
{
    public NativeException() { }
    public NativeException(string message) : base(message) { }
    public NativeException(string message, Exception inner) : base(message, inner) { }
}
