#include "ChunkVec.h"

#include <mimalloc.h>

using namespace cc;

ChunkedVectorData::~ChunkedVectorData()
{
    if (chunks == nullptr) return;
    for (auto i = 0; i < chunk_count; ++i)
    {
        mi_free(&chunks[i]);
    }
    mi_free(chunks);
}

UnsafeChunkVec::UnsafeChunkVec(Rc<ChunkedVectorData>&& data): m_data(std::move(data))
{
}

bool UnsafeChunkVec::EnsureChunkAllocated() const
{
    if (m_data->chunks != nullptr) return true;
    m_data->chunks = static_cast<void**>(mi_malloc(m_data->chunk_count = InitCapacity));
    m_data->chunks[0] = mi_malloc(ChunkSize);
    m_data->cur_chunk = m_data->cur_in_chunk = 0;
    return false;
}

bool UnsafeChunkVec::IsChunkFull(const SizeInfo& size_info) const
{
    if (m_data->cur_in_chunk <= size_info.chunk_cap) return false;
    return true;
}

void UnsafeChunkVec::Grow() const
{
    const auto new_size = m_data->chunk_count * 2;
    m_data->chunks = static_cast<void**>(mi_realloc(m_data->chunks, new_size));
    m_data->chunk_count = new_size;
}

void UnsafeChunkVec::AllocNewChunk() const
{
    m_data->cur_chunk++;
    if (m_data->cur_chunk >= m_data->chunk_count) Grow();
    m_data->chunks[m_data->cur_chunk] = mi_malloc(ChunkSize);
    m_data->cur_in_chunk = 0;
}

void* UnsafeChunkVec::AddUnsafe(const SizeInfo& size_info) const
{
    if (EnsureChunkAllocated() && IsChunkFull(size_info)) AllocNewChunk();
    m_data->count++;
    return &static_cast<u8*>(m_data->chunks[m_data->cur_chunk])[m_data->cur_in_chunk++ * size_info.type_size];
}
