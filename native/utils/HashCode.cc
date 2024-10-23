#include "HashCode.h"

#include "../App.h"

using namespace cc;

namespace
{
    constexpr std::array<u64, 4> s_PI = {
        0x243f'6a88'85a3'08d3,
        0x1319'8a2e'0370'7344,
        0xa409'3822'299f'31d0,
        0x082e'fa98'ec4e'6c89,
    };

    #ifdef __x86_64__

    constexpr std::array<u64, 2> SHUFFLE_MASK = {
        0x020a0700'0c01030e,
        0x050f0d08'06090b04,
    };

    u128 aesdec(u128 value, u128 xOr)
    {
        return reinterpret_cast<u128>(_mm_aesdec_si128(
            reinterpret_cast<__m128i>(value), reinterpret_cast<__m128i>(xOr)
        ));
    }

    u128 aesenc(u128 value, u128 xOr)
    {
        return reinterpret_cast<u128>(_mm_aesenc_si128(
            reinterpret_cast<__m128i>(value), reinterpret_cast<__m128i>(xOr)
        ));
    }

    u128 shuffle(u128 a)
    {
        return reinterpret_cast<u128>(_mm_shuffle_epi8(
            reinterpret_cast<__m128i>(a), *reinterpret_cast<const __m128i*>(&SHUFFLE_MASK)
        ));
    }

    u128 add_by_64s(u128 a, u128 b)
    {
        return reinterpret_cast<u128>(
            _mm_add_epi64(reinterpret_cast<__m128i>(a), reinterpret_cast<__m128i>(b)) // NOLINT(*-simd-intrinsics)
        );
    }

    u128 shuffle_and_add(const u128 base, const u128 to_add)
    {
        const auto shuffled = shuffle(base);
        return add_by_64s(shuffled, to_add);
    }

    #elif __arm64

    // todo

    #endif
}

AHasher AHasher::create()
{
    return new_with_keys(
        *reinterpret_cast<const u128*>(&args().a_hash_rand_0), *reinterpret_cast<const u128*>(&args().a_hash_rand_2)
    );
}

AHasher AHasher::new_with_keys(u128 key1, u128 key2)
{
    const auto pi = *reinterpret_cast<const std::array<u128, 4>*>(&s_PI);
    key1 = key1 ^ pi[0];
    key2 = key2 ^ pi[1];
    return {key1, key2, key1 ^ key2};
}

void AHasher::write(const u128 v)
{
    enc = aesdec(enc, v);
    sum = shuffle_and_add(sum, v);
}

u64 AHasher::finish() const
{
    const auto combined = aesenc(sum, enc);
    const auto result = aesdec(aesdec(combined, key), combined);
    return *reinterpret_cast<const u64*>(&result);
}
