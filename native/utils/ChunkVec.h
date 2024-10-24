#pragma once
#include <mimalloc.h>

#include "../ffi/Collections.h"
#include "error.h"

namespace cc
{
    class ChunkedVectorData final : public Object<FChunkedVectorData>
    {
        IMPL_OBJECT();

        friend class UnsafeChunkVec;
        template <class T>
        friend class ChunkVec;

    protected:
        ~ChunkedVectorData() override;
    };

    class UnsafeChunkVec : public FChunkedVectorConsts
    {
    public:
        UnsafeChunkVec(const UnsafeChunkVec&) = delete;
        UnsafeChunkVec& operator=(const UnsafeChunkVec&) = delete;

    protected:
        Rc<ChunkedVectorData> m_data;

        struct SizeInfo
        {
            size_t type_size;
            size_t chunk_cap;
        };

        explicit UnsafeChunkVec(Rc<ChunkedVectorData>&& data);

        bool EnsureChunkAllocated() const;

        bool IsChunkFull(const SizeInfo& size_info) const;

        void Grow() const;

        void AllocNewChunk() const;

        void* AddUnsafe(const SizeInfo& size_info) const;
    };

    template <class T>
    class ChunkVec : UnsafeChunkVec
    {
        constexpr static SizeInfo size_info()
        {
            static_assert(sizeof(T) < ChunkSize, "sizeof T must < ChunkSize");
            return {sizeof(T), ChunkSize / sizeof(T)};
        }

        struct create_t
        {
        };

        explicit ChunkVec(create_t) : UnsafeChunkVec(Rc(new ChunkedVectorData()))
        {
            m_data->type_size = sizeof(T);
        }

        explicit ChunkVec(create_t, Rc<ChunkedVectorData>&& data) : UnsafeChunkVec(std::move(data))
        {
            if (m_data->type_size != sizeof(T)) throw CcError("Wrong T type");
        }

    public:
        static ChunkVec Create()
        {
            return ChunkVec(create_t());
        }

        static ChunkVec UnsafeFromData(Rc<ChunkedVectorData>&& data)
        {
            return ChunkVec(create_t(), std::forward<Rc<ChunkedVectorData>>(data));
        }

        ~ChunkVec()
        {
            if constexpr (!std::is_trivially_destructible_v<T>)
            {
                if (m_data && m_data->count > 0 && m_data->chunk_count > 0)
                {
                    const auto start = m_data->chunk_count;
                    for (size_t n = m_data->cur_in_chunk; n > 0; n--)
                    {
                        UnsafeChunks()[start - 1].AsArray()[n - 1].~T();
                    }
                    if (start > 1)
                    {
                        constexpr auto chunk_cap = size_info().chunk_cap;
                        for (size_t i = start - 1; i > 0; i--)
                        {
                            for (size_t n = 0; n < chunk_cap; n++)
                            {
                                UnsafeChunks()[start - 1].AsArray()[n].~T();
                            }
                        }
                    }
                }
            }
        }

        struct Chunk
        {
            T* const ptr;

            auto& AsArray() const
            {
                return *reinterpret_cast<std::array<T, ChunkSize / sizeof(T)>*>(ptr);
            }
        };

        std::span<Chunk> UnsafeChunks() const
        {
            return {reinterpret_cast<Chunk*>(m_data->chunks), m_data->chunk_count};
        }

        void Add(const T& value)
        {
            T copy = value;
            Add(std::move(copy));
        }

        void Add(T&& value)
        {
            *static_cast<T*>(AddUnsafe(size_info())) = std::move(value);
        }
    };
} // cc
