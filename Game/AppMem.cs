using System.Runtime.InteropServices;
using Game.Native;

namespace Game;

public static unsafe partial class App
{
    public static T* MemAlloc<T>() => (T*)MemAlloc((nuint)sizeof(T));
    
    public static void* MemAlloc(nuint size)
    {
        void* ptr;
        s_native_app->MemAlloc(size, &ptr).TryThrow();
        return ptr;
    }

    public static void MemFree(void* ptr) => s_native_app->MemFree(ptr).TryThrow();

    public static void* MemReAlloc(void* ptr, nuint new_size)
    {
        void* r;
        s_native_app->MemReAlloc(ptr, new_size, &r).TryThrow();
        return r;
    }

    public static void MemFreeLinkedList(FLikeLinkedList* ptr) => s_native_app->MemFreeLinkedList(ptr).TryThrow();
}
