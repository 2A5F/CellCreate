#pragma once

#include <cstdint>

namespace cc
{
    #if __x86_64

    constexpr uint32_t lzcnt(uint32_t value)
    {
        return _lzcnt_u32(value);
    }

    constexpr uint64_t lzcnt(uint64_t value)
    {
        return _lzcnt_u64(value);
    }

    constexpr uint32_t tzcnt(uint32_t value)
    {
        return __tzcnt_u32(value);
    }

    constexpr uint64_t tzcnt(uint64_t value)
    {
        return __tzcnt_u64(value);
    }

    constexpr uint32_t popcnt(uint32_t value)
    {
        return _mm_popcnt_u32(value);
    }

    constexpr uint64_t popcnt(uint64_t value)
    {
        return _mm_popcnt_u64(value);
    }

    #elif __arm64

    // todo

    #endif
}
