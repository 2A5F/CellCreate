#pragma once
#include "../types.h"

// https://github.com/tkaitchuck/aHash

namespace cc
{
    struct AHasher
    {
        u128 enc;
        u128 sum;
        u128 key;

        static AHasher create();

        static AHasher new_with_keys(u128 key1, u128 key2);

        void write(const u8 v){ write(static_cast<u128>(v)); }
        void write(const u16 v){ write(static_cast<u128>(v)); }
        void write(const u32 v){ write(static_cast<u128>(v)); }
        void write(const u64 v) { write(static_cast<u128>(v)); }
        void write(u128 v);

        u64 finish() const;
    };
}
