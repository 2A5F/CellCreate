using Coplt.Dropping;

namespace Game.Native;

[Dropping(Unmanaged = true)]
public unsafe partial class NativeChunkList<T>
    where T : unmanaged
{
    public const int ChunkSize = 1024 * 16;

    internal Chunk* m_first;
    internal Chunk* m_current; // last
    internal nuint m_size;
    internal nuint m_size_in_chunk;
    internal readonly nuint m_chunk_cap;

    internal struct Chunk
    {
        public Chunk* next;
        public T first;

        public static Chunk* Alloc()
        {
            var chunk = (Chunk*)App.MemAlloc(ChunkSize);
            chunk->next = null;
            return chunk;
        }
    }

    public NativeChunkList()
    {
        var align = (nuint)(&((Chunk*)null)->first);
        var last_size = ChunkSize - align;
        if ((nuint)sizeof(T) > last_size) throw new ArgumentOutOfRangeException($"{typeof(T)} is too large");
        m_chunk_cap = last_size / (nuint)sizeof(T);
        m_current = m_first = Chunk.Alloc();
    }

    [Drop]
    private void Drop()
    {
        if (ExchangeUtils.ExchangePtr(ref m_first, null, out var ptr) is null) return;
        App.MemFreeLinkedList((FLikeLinkedList*)ptr);
    }

    public nuint Count => m_size;

    public Slice<T> Extra(nuint size)
    {
        if (size > m_chunk_cap) throw new ArgumentOutOfRangeException($"{nameof(size)} is too large");
        if (size + m_size_in_chunk > m_chunk_cap)
        {
            m_size_in_chunk = 0;
            if (m_current->next == null) m_current->next = Chunk.Alloc();
            m_current = m_current->next;
        }
        var span = new Slice<T>(&m_current->first + m_size_in_chunk, size);
        m_size_in_chunk += size;
        m_size += size;
        return span;
    }

    public void Clear()
    {
        m_size = 0;
        m_size_in_chunk = 0;
    }
}
