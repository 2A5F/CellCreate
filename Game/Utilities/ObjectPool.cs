namespace Game.Utilities;

public abstract class UnsafeObjectPool
{
    public abstract object UnsafeGet();
    public abstract void UnsafeReturn(object value);
}

public sealed class ObjectPool<T> : UnsafeObjectPool where T : class, new()
{
    private readonly Queue<T> Buffer = new();

    public T Get() => Buffer.TryDequeue(out var r) ? r : new();
    public void Return(T value) => Buffer.Enqueue(value);

    public override object UnsafeGet() => Get();
    public override void UnsafeReturn(object value) => Return((T)value);
}

public class ObjectPool
{
    private readonly Dictionary<Type, UnsafeObjectPool> inner = new();

    public T Get<T>() where T : class, new() =>
        ((ObjectPool<T>)inner.GetOrAdd(typeof(T), static _ => new ObjectPool<T>())).Get();

    public void Return<T>(T value) where T : class, new() =>
        ((ObjectPool<T>)inner.GetOrAdd(typeof(T), static _ => new ObjectPool<T>())).Return(value);
}
