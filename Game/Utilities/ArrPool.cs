using System.Buffers;

namespace Game.Utilities;

public readonly struct ArrPool<T>(T[] Array, int Size) : IDisposable
{
    public readonly T[] Array = Array;
    public readonly int Size = Size;
    public Span<T> Span => Array.AsSpan(0, Size);

    public static ArrPool<T> Get(int size) => new(ArrayPool<T>.Shared.Rent(size), size);
    
    public void Dispose() => ArrayPool<T>.Shared.Return(Array);
}
