#pragma once
#include <cstdint>

#include "../Object.h"

namespace cc
{
    struct FChunkedVectorConsts
    {
        static constexpr int32_t ChunkSize = 1024 * 16;
        static constexpr int32_t InitCapacity = 1;
    };

    struct FChunkedVectorData : IObject, FChunkedVectorConsts
    {
        IMPL_INTERFACE("270057f3-e830-4bca-9fab-0b2fa08bbbdf", IObject);

        size_t type_size;
        void** chunks;
        size_t chunk_count;
        size_t count;
        size_t cur_chunk;
        size_t cur_in_chunk;
    };
}
