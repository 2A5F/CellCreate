#pragma once

#include <cstdint>
#ifdef CO_SRC
#include <memory>
#include <glm/glm.hpp>
#include <winrt/base.h>
#include <ankerl/unordered_dense.h>
#endif

namespace cc
{
    using b8 = int8_t;
    using b32 = int32_t;

    using u8 = uint8_t;
    using u16 = uint16_t;
    using u32 = uint32_t;
    using u64 = uint64_t;
    using usize = size_t;

    using i8 = int8_t;
    using i16 = int16_t;
    using i32 = int32_t;
    using i64 = int64_t;

    #ifdef CO_SRC
    using f32 = glm::f32;
    using f64 = glm::f64;

    using float2 = glm::f32vec2;
    using float3 = glm::f32vec3;
    using float4 = glm::f32vec4;

    using int2 = glm::ivec2;
    using int3 = glm::ivec3;
    using int4 = glm::ivec4;

    using uint2 = glm::uvec2;
    using uint3 = glm::uvec3;
    using uint4 = glm::uvec4;

    using float3x3 = glm::f32mat3x3;
    using float3x4 = glm::f32mat3x4;
    using float4x3 = glm::f32mat4x3;
    using float4x4 = glm::f32mat4x4;

    using quaternion = glm::f32quat;

    template <class T>
    using ComPtr = winrt::com_ptr<T>;

    template <class T>
    using Box = std::unique_ptr<T>;

    template <class T>
    using List = std::vector<T>;

    template <class K, class V>
    using HashMap = ankerl::unordered_dense::map<K, V>;

    template <class V>
    using HashSet = ankerl::unordered_dense::set<V>;

    #else
    using f32 = float;
    using f64 = double;

    struct alignas(8)  float2 { f32 x; f32 y; };
    struct alignas(16) float3 { f32 x; f32 y; f32 z; };
    struct alignas(16) float4 { f32 x; f32 y; f32 z; f32 w; };

    struct alignas(8)  int2 { i32 x; i32 y; };
    struct alignas(16) int3 { i32 x; i32 y; i32 z; };
    struct alignas(16) int4 { i32 x; i32 y; i32 z; i32 w; };

    struct alignas(8)  uint2 { u32 x; u32 y; };
    struct alignas(16) uint3 { u32 x; u32 y; u32 z; };
    struct alignas(16) uint4 { u32 x; u32 y; u32 z; u32 w; };

    struct alignas(16) float3x3 { float3 c0; float3 c1; float3 c2; };
    struct alignas(16) float3x4 { float3 c0; float3 c1; float3 c2; float3 c3; };
    struct alignas(16) float4x3 { float4 c0; float4 c1; float4 c2; };
    struct alignas(16) float4x4 { float4 c0; float4 c1; float4 c2; float4 c3; };

    struct alignas(16) quaternion { float4 value; };
    #endif
}
