#pragma once

#include <shared_mutex>

namespace cc
{
    template <class T>
    class RwLock;

    template <class T>
    class RwLockWriteGuard // NOLINT(*-pro-type-member-init)
    {
        friend RwLock<T>;

        RwLock<T>* m_lock;
        std::unique_lock<std::shared_mutex> m_guard;

        explicit RwLockWriteGuard(RwLock<T>* lock) : m_lock(lock), m_guard(lock->m_mutex)
        {
        }

    public:
        T* operator->() const
        {
            return &m_lock->m_value;
        }

        T& operator*() const
        {
            return m_lock->m_value;
        }
    };

    template <class T>
    class RwLockReadGuard // NOLINT(*-pro-type-member-init)
    {
        friend RwLock<T>;

        RwLock<T>* m_lock;
        std::shared_lock<std::shared_mutex> m_guard;

        explicit RwLockReadGuard(RwLock<T>* lock) : m_lock(lock), m_guard(lock->m_mutex)
        {
        }

    public:
        const T* operator->() const
        {
            return &m_lock->m_value;
        }

        const T& operator*() const
        {
            return m_lock->m_value;
        }
    };

    template <class T>
    class RwLock
    {
        friend RwLockWriteGuard<T>;
        friend RwLockReadGuard<T>;

        T m_value;
        std::shared_mutex m_mutex{};

    public:
        explicit RwLock() : m_value()
        {
        }

        explicit RwLock(T&& value) : m_value(std::forward<T>(value))
        {
        }

        RwLockWriteGuard<T> write()
        {
            return RwLockWriteGuard(this);
        }

        RwLockReadGuard<T> read()
        {
            return RwLockReadGuard(this);
        }
    };
}
