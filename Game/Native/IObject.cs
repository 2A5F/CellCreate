namespace Game.Native;

public unsafe interface IObject
{
    public nuint AddRef();
    public nuint Release();
    public nuint AddRefWeak();
    public nuint ReleaseWeak();
    public B8 TryDowngrade();
    public B8 TryUpgrade();
    public void* QueryInterface(Guid id);
}
