namespace Game.Utilities;

public abstract class UnsafeObjectPool
{
    public abstract object UnsafeGet();
    public abstract void UnsafeReturn(object value);
}

public sealed class ObjectPool<T>(Func<T> Ctor) : UnsafeObjectPool where T : class
{
    internal readonly Queue<T> Buffer = new();

    public T Get() => Buffer.TryDequeue(out var r) ? r : Ctor();
    public void Return(T value) => Buffer.Enqueue(value);

    public override object UnsafeGet() => Get();
    public override void UnsafeReturn(object value) => Return((T)value);
}

public class ObjectPool
{
    internal readonly Dictionary<Type, UnsafeObjectPool> inner = new();

    public static ObjectPool<T> Create<T>() where T : class, new() => new(static () => new());
    public static ObjectPool<T> Create<T>(Func<T> ctor) where T : class => new(ctor);

    public T Get<T>() where T : class, new() =>
        ((ObjectPool<T>)inner.GetOrAdd(typeof(T), static _ => Create<T>())).Get();

    public void Return<T>(T value) where T : class, new() =>
        ((ObjectPool<T>)inner.GetOrAdd(typeof(T), static _ => Create<T>())).Return(value);

    public T Get<T>(Func<T> ctor) where T : class =>
        ((ObjectPool<T>)inner
            .GetOrAdd(typeof(T), ctor, static (ctor, _) => Create(ctor))).Get();

    public void Return<T>(T value, Func<T> ctor) where T : class =>
        ((ObjectPool<T>)inner
            .GetOrAdd(typeof(T), ctor, static (ctor, _) => Create(ctor))).Return(value);
}

public static class ObjectPoolEx
{
    internal static void Dispose<T>(this ObjectPool<T> pool) where T : class, IDisposable
    {
        foreach (var item in pool.Buffer)
        {
            item.Dispose();
        }
    }
}
