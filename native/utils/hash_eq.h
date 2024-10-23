#pragma once

namespace cc
{
    template <class T>
    concept SubHash = requires { typename T::Hash; };

    template <class T>
    concept SubEq = requires { typename T::Eq; };

    template <class T, class Default>
    struct T_HashOf
    {
        using Type = Default;
    };

    template <SubHash T, class Default>
    struct T_HashOf<T, Default>
    {
        using Type = typename T::Hash;
    };

    template <class T, class Default>
    struct T_EqOf
    {
        using Type = Default;
    };

    template <SubEq T, class Default>
    struct T_EqOf<T, Default>
    {
        using Type = typename T::Eq;
    };
};
