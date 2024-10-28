using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Coplt.Dropping;

namespace Game.Native;

[Dropping(Unmanaged = true)]
public unsafe partial class UnsafeNativeChunkVec
{
    internal FChunkedVectorData* m_data;

    public const int ChunkSize = FChunkedVectorConsts.ChunkSize;
    public const int InitCapacity = FChunkedVectorConsts.InitCapacity;

    internal ref nuint type_size => ref m_data->type_size;
    internal ref void** chunks => ref m_data->chunks;
    internal ref nuint chunk_count => ref m_data->chunk_count;
    internal ref nuint count => ref m_data->count;
    internal ref nuint cur_chunk => ref m_data->cur_chunk;
    internal ref nuint cur_in_chunk => ref m_data->cur_in_chunk;

    protected UnsafeNativeChunkVec(FChunkedVectorData* p_data)
    {
        m_data = p_data;
    }

    [Drop]
    private void Drop()
    {
        if (ExchangeUtils.ExchangePtr(ref m_data, null, out var ptr) is null) return;
        ptr->Release();
    }

    protected readonly record struct SizeInfo(nuint type_size, nuint chunk_cap)
    {
        public readonly nuint type_size = type_size;
        public readonly nuint chunk_cap = chunk_cap;
    }

    public int Count => (int)count;

    /// <returns>Was allocated created before</returns>
    private bool EnsureChunkAllocated()
    {
        if (chunks != null) return true;
        chunks = (void**)App.MemAlloc(chunk_count = InitCapacity);
        chunks[0] = App.MemAlloc(ChunkSize);
        cur_chunk = cur_in_chunk = 0;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsChunkFull(in SizeInfo size_info)
    {
        if (cur_in_chunk <= size_info.chunk_cap) return false;
        return true;
    }

    private void Grow()
    {
        var new_size = chunk_count * 2;
        chunks = (void**)App.MemReAlloc(chunks, new_size);
        chunk_count = new_size;
    }

    private void AllocNewChunk()
    {
        cur_chunk++;
        if (cur_chunk >= chunk_count) Grow();
        chunks[cur_chunk] = App.MemAlloc(ChunkSize);
        cur_in_chunk = 0;
    }

    protected void* AddUnsafe(in SizeInfo size_info)
    {
        if (EnsureChunkAllocated() && IsChunkFull(size_info)) AllocNewChunk();
        count++;
        return &((byte*)chunks[cur_chunk])[cur_in_chunk++ * size_info.type_size];
    }

    protected void* Get(nuint index, in SizeInfo size_info)
    {
        if (index >= count) throw new IndexOutOfRangeException();
        var chunk = index / size_info.chunk_cap;
        var in_chunk = index % size_info.chunk_cap;
        return &((byte*)chunks[chunk])[in_chunk * size_info.type_size];
    }
}

public unsafe class NativeChunkVec<T> : UnsafeNativeChunkVec, IEnumerable<T>
    where T : unmanaged
{
    private static new SizeInfo SizeInfo => new((nuint)sizeof(T), (nuint)(ChunkSize / sizeof(T)));

    public readonly struct Chunk(T* ptr)
    {
        public readonly T* ptr = ptr;
        public Span<T> AsSpan => new(ptr, ChunkSize / sizeof(T));
    }

    public Span<Chunk> UnsafeChunks => new(chunks, (int)chunk_count);

    private static FChunkedVectorData* CreateFChunkedVectorData()
    {
        FChunkedVectorData* p_data;
        App.s_native_app->CreatePaddedChunkedVectorData(&p_data).TryThrow();
        return p_data;
    }

    public NativeChunkVec() : base(CreateFChunkedVectorData())
    {
        type_size = (nuint)sizeof(T);
    }

    public void Add(T value) => *(T*)AddUnsafe(SizeInfo) = value;

    public ref T this[int index] => ref *(T*)Get((nuint)index, SizeInfo);

    public ref T this[nuint index] => ref *(T*)Get(index, SizeInfo);

    #region Enumerator

    public Enumerator GetEnumerator() => new(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new EnumeratorClass(this);
    IEnumerator IEnumerable.GetEnumerator() => new EnumeratorClass(this);

    public struct Enumerator(NativeChunkVec<T> self)
    {
        private nuint index = nuint.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => index = nuint.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var new_index = index + 1;
            if (new_index >= self.count) return false;
            index = new_index;
            return true;
        }

        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref self[index];
        }
    }

    public class EnumeratorClass(NativeChunkVec<T> self) : IEnumerator<T>
    {
        private nuint index = nuint.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => index = nuint.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var new_index = index + 1;
            if (new_index >= self.count) return false;
            index = new_index;
            return true;
        }

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => self[index];
        }

        object IEnumerator.Current => Current;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() { }
    }

    #endregion
}
