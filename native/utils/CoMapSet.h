#pragma once

#include <vector>
#include <thread>
#include <shared_mutex>

#include <ankerl/unordered_dense.h>
#include <fmt/base.h>

#include "./RwLock.h"
#include "./hash_eq.h"
#include "./bitcount.h"

namespace cc
{
    // 并发映射
    template <class K, class V,
        class Hash = typename T_HashOf<K, ankerl::unordered_dense::hash<K>>::Type,
        class Eq = typename T_EqOf<K, std::equal_to<K>>::Type //,
    >
    class CoHashMap
    {
        using Self = CoHashMap;

        uint64_t shift{};
        std::vector<RwLock<ankerl::unordered_dense::map<K, V, Hash, Eq>>> m_shards;
        Hash m_hasher{};

        uint64_t determine_shard(const uint64_t hash) const
        {
            return hash >> shift;
        }

    public:
        explicit CoHashMap(uint64_t shard_amount)
            : shift(sizeof(uint64_t) * 8 - tzcnt(shard_amount)), m_shards(shard_amount)
        {
        }

        explicit CoHashMap() : Self(std::thread::hardware_concurrency())
        {
        }

        CoHashMap(const CoHashMap&) = delete;
        CoHashMap& operator=(const CoHashMap&) = delete;

        // todo 优化，避免多次计算 hash

        template <class Mapper, class Default>
        auto GetOrDefault(const K& key, Mapper mapper, Default create_default)
        {
            const auto hash = m_hasher(key);
            const auto idx = determine_shard(hash);

            auto shard = m_shards[idx].read();
            auto iter = shard->find(key);
            if (iter != shard->end()) return std::forward<Mapper>(mapper)(iter->second);

            return std::forward<Default>(create_default)();
        }

        void AddOrReplace(const K& key, V&& value)
        {
            const auto hash = m_hasher(key);
            const auto idx = determine_shard(hash);

            auto shard = m_shards[idx].write();

            shard->insert_or_assign(key, std::forward<V>(value));
        }

        void AddOrReplace(K&& key, V&& value)
        {
            const auto hash = m_hasher(key);
            const auto idx = determine_shard(hash);

            auto shard = m_shards[idx].write();

            shard->insert_or_assign(std::forward<K>(key), std::forward<V>(value));
        }

        template <class Ctor, class Mapper>
        auto GetOrAdd(const K& key, Ctor ctor, Mapper mapper)
        {
            const auto hash = m_hasher(key);
            const auto idx = determine_shard(hash);

            {
                auto shard = m_shards[idx].read();
                auto iter = shard->find(key);
                if (iter != shard->end()) return std::forward<Mapper>(mapper)(iter->second);
            }
            {
                auto shard = m_shards[idx].write();
                auto r = shard->emplace(key, std::forward<Ctor>(ctor)(key));
                return std::forward<Mapper>(mapper)(r.first->second);
            }
        }

        template <class Ctor>
        auto GetOrAdd(const K& key, Ctor ctor)
        {
            return GetOrAdd(key, std::forward<Ctor>(ctor), [](auto& r) { return r; });
        }
    };
}
