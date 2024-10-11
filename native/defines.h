#pragma once

#if defined(_MSC_VER)
#define CO_INLINE inline __forceinline
#elif defined(__GNUC__) || defined(__clang__)
    #define CO_INLINE inline __attribute__((always_inline))
#else
    #define CO_INLINE inline
#endif

#if defined(_MSC_VER)
#define CO_NOINLINE __declspec(noinline)
#elif defined(__GNUC__) || defined(__clang__)
    #define CO_NOINLINE __attribute__((noinline))
#else
    #define CO_NOINLINE
#endif

#if defined(_MSC_VER)
#define CO_CDECL __cdecl
#elif defined(__GNUC__) || defined(__clang__)
    #define CO_CDECL __attribute__((__cdecl__))
#else
    #define CO_CDECL
#endif
