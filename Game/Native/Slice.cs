using System.Collections;
using System.Runtime.CompilerServices;

namespace Game.Native;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly unsafe struct Slice<T>(T* data, UIntPtr size)
{
    public readonly T* data = data;
    public readonly nuint size = size;

    public Span<T> AsSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(data, (int)size);
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref AsSpan[index];
    }

    public T* this[nuint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index >= size) throw new IndexOutOfRangeException(nameof(index));
            return &data[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    public PointerEnumerator Ptrs
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }

    public struct Enumerator(Slice<T> slice)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => this;

        private readonly Slice<T> slice = slice;
        private nuint index = nuint.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var index = this.index + 1;
            if (index >= slice.size) return false;
            this.index = index;
            return true;
        }

        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *slice[index - 1];
        }
    }

    public struct PointerEnumerator(Slice<T> slice)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PointerEnumerator GetEnumerator() => this;

        private readonly Slice<T> slice = slice;
        private nuint index = nuint.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var index = this.index + 1;
            if (index >= slice.size) return false;
            this.index = index;
            return true;
        }

        public T* Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => slice[index - 1];
        }
    }
}

public static unsafe class SliceEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Slice<T> Slice<T>(this Slice<T> slice, nuint start)
    {
        if (start > slice.size) throw new IndexOutOfRangeException(nameof(start));
        return new(slice.data + start, slice.size - start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Slice<T> Slice<T>(this Slice<T> slice, nuint start, nuint length)
    {
        if (start + length > slice.size) throw new ArgumentOutOfRangeException();
        return new(slice.data + start, length);
    }
}
