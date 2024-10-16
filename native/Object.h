#pragma once

#include <atomic>
#include <type_traits>
#include <utility>

#include "defines.h"
#include "types.h"

#ifdef CO_SRC
#include "uuid.h"

#define IMPL_OBJECT() \
    public:\
        void* ObjectStart() noexcept override { return this; }\
    private:

#define IMPL_INTERFACE(ID, ...)\
        constexpr static uuid UUID = uuid::from_string(ID).value();\
    private:\
        template<Interface... T> \
        friend class ::cc::_internal::DoQueryInterface;\
        constexpr static void* QueryInterfaceForBase(auto* self, uuid id)\
        {\
            return ::cc::_internal::DoQueryInterface<__VA_ARGS__>::QueryInterface(self, id);\
        }\
    public:
#else
#define IMPL_OBJECT()
#define IMPL_INTERFACE(ID, ...)\
        constexpr static auto s_FFI_UUID = L##ID;
#endif

namespace cc
{
    #ifdef CO_SRC
    using uuid = uuids::uuid;

    struct IObject;

    template <class T>
    concept Interface = requires
    {
        { T::UUID } -> std::same_as<const uuid&>;
        std::is_base_of_v<IObject, T>;
    };

    namespace _internal
    {
        template <Interface... Interfaces>
        struct DoQueryInterface;
    }
    #else
    using uuid = __uint128_t;
    #endif

    struct IObject
    {
        #ifdef CO_SRC
        constexpr static uuid UUID = uuid::from_string("00000000-0000-0000-0000-000000000000").value();

    private:
        template <Interface... T>
        friend class ::cc::_internal::DoQueryInterface;

        constexpr static void* QueryInterfaceForBase(auto* self, uuid id)
        {
            return nullptr;
        }

    public:
        #else
        constexpr static auto s_FFI_UUID = L"00000000-0000-0000-0000-000000000000";
        #endif

    protected:
        virtual ~IObject() noexcept = default;

    public:
        virtual size_t AddRef() noexcept = 0;
        virtual size_t Release() noexcept = 0;
        virtual size_t AddRefWeak() noexcept = 0;
        virtual size_t ReleaseWeak() noexcept = 0;
        virtual b8 TryDowngrade() noexcept = 0;
        virtual b8 TryUpgrade() noexcept = 0;
        virtual void* ObjectStart() noexcept = 0;

        virtual void* QueryInterface(uuid id) noexcept = 0;

        #ifdef CO_SRC
        template <Interface T>
        T* QueryInterface() noexcept
        {
            return static_cast<T*>(QueryInterface(T::UUID));
        }
        #endif
    };

    #ifdef CO_SRC

    template <class T>
    class Weak;

    template <class T>
    class Rc final
    {
        T* m_ptr;

        template <class U>
        friend class Rc;

        template <class U>
        friend class Weak;

        struct clone_t
        {
        };

        struct upgrade_t
        {
        };

        // clone
        explicit Rc(T* ptr, clone_t) : m_ptr(ptr)
        {
            static_assert(std::is_base_of_v<IObject, T>);

            if (auto p = get())
            {
                p->AddRef();
            }
        }

        // upgrade
        explicit Rc(T* ptr, upgrade_t) : m_ptr(ptr)
        {
            static_assert(std::is_base_of_v<IObject, T>);

            if (auto p = get())
            {
                if (!p->TryUpgrade())
                {
                    this->m_ptr = nullptr;
                    return;
                }
            }
        }

        void drop()
        {
            if (auto p = m_ptr)
            {
                p->Release();
            }
        }

    public:
        using value_t = T;

        inline static Rc None{};

        ~Rc()
        {
            drop();
        }

        void reset()
        {
            drop();
        }

        explicit operator bool() const
        {
            return get() != nullptr;
        }

        T* get() const
        {
            return m_ptr;
        }

        T& operator*() const
        {
            return *get();
        }

        T* operator->() const
        {
            return get();
        }

        // null
        Rc() : m_ptr(nullptr)
        {
        }

        // null
        // ReSharper disable once CppNonExplicitConvertingConstructor
        Rc(std::nullptr_t) : m_ptr(nullptr) // NOLINT(*-explicit-constructor)
        {
        }

        // create
        // ReSharper disable once CppNonExplicitConvertingConstructor
        Rc(T* ptr) : m_ptr(ptr) // NOLINT(*-explicit-constructor)
        {
            static_assert(std::is_base_of_v<IObject, T>);
        }

        // copy
        Rc(const Rc& r) : Rc(r.get(), clone_t())
        {
        }

        // copy conv
        template <typename U>
            requires std::convertible_to<U*, T*>
        // ReSharper disable once CppNonExplicitConvertingConstructor
        Rc(const Rc<U>& r): Rc(r.get(), clone_t()) // NOLINT(*-explicit-constructor)
        {
        }

        // move
        Rc(Rc&& r) noexcept: m_ptr(r.m_ptr)
        {
            r.m_ptr = nullptr;
        }

        // move conv
        template <typename U>
            requires std::convertible_to<U*, T*>
        // ReSharper disable once CppNonExplicitConvertingConstructor
        Rc(Rc<U>&& r): m_ptr(r.m_ptr) // NOLINT(*-explicit-constructor)
        {
            r.m_ptr = nullptr;
        }

        Rc& operator=(T* p)
        {
            drop();
            new(this) Rc(p);
            return *this;
        }

        // copy ass
        Rc& operator=(Rc const& r) noexcept
        {
            if (this == &r) return *this;
            drop();
            new(this) Rc(r);
            return *this;
        }

        // move ass
        Rc& operator=(Rc&& r) noexcept
        {
            if (this != &r)
            {
                drop();
                new(this) Rc(std::forward<Rc>(r));
            }
            return *this;
        }

        // copy ass conv
        template <typename U>
            requires std::convertible_to<U*, T*>
        Rc& operator=(Rc<U> const& r) noexcept
        {
            drop();
            new(this) Rc(r);
            return *this;
        }

        // move ass conv
        template <typename U>
            requires std::convertible_to<U*, T*>
        Rc& operator=(Rc<U>&& r) noexcept
        {
            drop();
            new(this) Rc(std::forward<Rc<U>>(r));
            return *this;
        }

        Rc clone() const
        {
            return Rc(get(), clone_t());
        }

        static Rc UnsafeClone(T* ptr)
        {
            return Rc(ptr, clone_t());
        }

        static Rc UnsafeCreate(T* ptr)
        {
            Rc r = ptr;
            return r;
        }

        static void UnsafeDrop(T* ptr)
        {
            Rc _ = ptr;
        }

        // Direct leakage, out of RAII management
        template <class Self>
        T* leak(this Self& self)
        {
            auto s = std::move(self);
            if (s) s->AddRef();
            return s.get();
        }

        Weak<T> downgrade() const
        {
            return Weak(get(), Weak<T>::downgrade_t());
        }

        bool operator==(std::nullptr_t) const
        {
            return get() == nullptr;
        }
    };

    template <class T>
    class Weak final
    {
        T* m_ptr;

        template <class U>
        friend class Weak;

        template <class U>
        friend class Rc;

        struct clone_t
        {
        };

        struct downgrade_t
        {
        };

        // clone
        explicit Weak(T* ptr, clone_t) : m_ptr(ptr)
        {
            static_assert(std::is_base_of_v<IObject, T>);

            if (auto p = get())
            {
                p->AddRefWeak();
            }
        }

        // downgrade
        explicit Weak(T* ptr, downgrade_t) : m_ptr(ptr)
        {
            static_assert(std::is_base_of_v<IObject, T>);

            if (auto p = get())
            {
                if (!p->TryDowngrade())
                {
                    this->m_ptr = nullptr;
                    return;
                }
            }
        }

        void drop()
        {
            if (auto p = this->m_ptr)
            {
                p->ReleaseWeak();
            }
        }

        T* get() const
        {
            return m_ptr;
        }

    public:
        using value_t = T;

        inline static Weak None{};

        ~Weak()
        {
            drop();
        }

        void reset()
        {
            drop();
        }

        explicit operator bool() const
        {
            return get() != nullptr;
        }

        // null
        Weak() : m_ptr(nullptr)
        {
        }

        // null
        // ReSharper disable once CppNonExplicitConvertingConstructor
        Weak(std::nullptr_t) : m_ptr(nullptr) // NOLINT(*-explicit-constructor)
        {
        }

        // copy
        Weak(const Weak& r) : Weak(r.get(), clone_t())
        {
        }

        // downgrade
        // ReSharper disable once CppNonExplicitConvertingConstructor
        Weak(const Rc<T>& r) : Weak(r.get(), downgrade_t()) // NOLINT(*-explicit-constructor)
        {
        }

        // copy conv
        template <typename U>
            requires std::convertible_to<U*, T*>
        // ReSharper disable once CppNonExplicitConvertingConstructor
        Weak(const Weak<U>& r): Weak(r.get(), clone_t()) // NOLINT(*-explicit-constructor)
        {
        }

        // downgrade conv
        template <typename U>
            requires std::convertible_to<U*, T*>
        // ReSharper disable once CppNonExplicitConvertingConstructor
        Weak(const Rc<U>& r): Weak(r.get(), downgrade_t()) // NOLINT(*-explicit-constructor)
        {
        }

        // move
        Weak(Weak&& r) noexcept: m_ptr(r.m_ptr)
        {
            r.m_ptr = nullptr;
        }

        // move conv
        template <typename U>
            requires std::convertible_to<U*, T*>
        // ReSharper disable once CppNonExplicitConvertingConstructor
        Weak(Weak<U>&& r): m_ptr(r.ptr) // NOLINT(*-explicit-constructor)
        {
            r.ptr = nullptr;
        }

        Weak& operator=(T* p)
        {
            drop();
            new(this) Weak(p);
            return *this;
        }

        // copy ass
        Weak& operator=(Weak const& r) noexcept
        {
            if (this == &r) return *this;
            drop();
            new(this) Weak(r);
            return *this;
        }

        // move ass
        Weak& operator=(Weak&& r) noexcept
        {
            if (this != &r)
            {
                drop();
                new(this) Weak(std::forward<Weak>(r));
            }
            return *this;
        }

        // copy ass conv
        template <typename U>
            requires std::convertible_to<U*, T*>
        Weak& operator=(Weak<U> const& r) noexcept
        {
            drop();
            new(this) Weak(r);
            return *this;
        }

        // move ass conv
        template <typename U>
            requires std::convertible_to<U*, T*>
        Weak& operator=(Weak<U>&& r) noexcept
        {
            drop();
            new(this) Weak(std::forward<Weak<U>>(r));
            return *this;
        }

        Rc<T> upgrade() const
        {
            return Rc(get(), Rc<T>::upgrade_t());
        }

        bool operator==(std::nullptr_t) const
        {
            return get() == nullptr;
        }
    };

    namespace _internal
    {
        template <Interface...>
        struct MergeInterface
        {
        };

        template <Interface First>
        struct MergeInterface<First>
        {
            using Output = First;
        };

        template <Interface First, Interface... Last>
        struct MergeInterface<First, Last...> : First, MergeInterface<Last...>::Output
        {
            using Output = MergeInterface;
        };

        template <>
        struct MergeInterface<>
        {
            using Output = IObject;
        };

        template <Interface... Interfaces>
        struct DoQueryInterface
        {
        };

        template <>
        struct DoQueryInterface<>
        {
            constexpr static void* QueryInterface(auto* self, uuid id)
            {
                return nullptr;
            }
        };

        template <Interface First, Interface... Last>
        struct DoQueryInterface<First, Last...>
        {
            constexpr static void* QueryInterface(auto* self, uuid id)
            {
                if (First::UUID == id) return static_cast<First*>(self);
                if (void* r = First::QueryInterfaceForBase(static_cast<First*>(self), id)) return r;
                return DoQueryInterface<Last...>::QueryInterface(self, id);
            }
        };
    }

    // Multiple inheritance is not allowed
    template <Interface... Interfaces>
    class Object : public _internal::MergeInterface<Interfaces...>::Output
    {
        mutable std::atomic_size_t m_strong{1};
        mutable std::atomic_size_t m_weak{1};

        CO_NOINLINE void DropSlow() noexcept
        {
            this->~Object();
            ReleaseWeak();
        }

    protected:
        virtual void Free(void* self) noexcept { operator delete(self); }

    public:
        void* QueryInterface(uuid id) noexcept override
        {
            return _internal::DoQueryInterface<Interfaces...>::QueryInterface(this, id);
        }

        void* ObjectStart() noexcept override = 0;

        template <class Self>
        Self&& Move(this Self&& self)
        {
            return std::forward<Self>(self);
        }

        template <class Self>
        Rc<Self> CloneThis(this Self& self)
        {
            return Rc<Self>::UnsafeClone(&self);
        }

        size_t AddRef() noexcept override
        {
            return m_strong.fetch_add(1, std::memory_order_relaxed);
        }

        size_t Release() noexcept override
        {
            const size_t r = m_strong.fetch_sub(1, std::memory_order_release);
            if (r != 1) return r;
            DropSlow();
            return r;
        }

        size_t AddRefWeak() noexcept override
        {
            return m_weak.fetch_add(1, std::memory_order_relaxed);
        }

        size_t ReleaseWeak() noexcept override
        {
            const size_t r = m_weak.fetch_sub(1, std::memory_order_release);
            if (r != 1) return r;
            this->Free(ObjectStart());
            return r;
        }

        b8 TryDowngrade() noexcept override
        {
            size_t cur = m_weak.load(std::memory_order_relaxed);
        re_try:
            if (cur == 0) return false;
            if (m_weak.compare_exchange_weak(cur, cur + 1, std::memory_order_acquire, std::memory_order_relaxed))
            {
                return true;
            }
            goto re_try;
        }

        b8 TryUpgrade() noexcept override
        {
            size_t cur = m_strong.load(std::memory_order_relaxed);
        re_try:
            if (cur == 0) return false;
            if (m_strong.compare_exchange_weak(cur, cur + 1, std::memory_order_acquire, std::memory_order_relaxed))
            {
                return true;
            }
            goto re_try;
        }
    };

    #endif
}
