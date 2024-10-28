using System.Runtime.CompilerServices;
using Coplt.Dropping;

namespace Game.Native;

[Dropping(Unmanaged = true)]
public sealed unsafe partial class NativeString16 : IEquatable<NativeString16>
{
    internal FString16* m_ptr;
    internal readonly string m_str;

    public NativeString16(string str)
    {
        m_str = str;
        fixed (char* ptr = str)
        {
            FString16* p_ptr;
            App.s_native_app->CreateString(new(ptr, (nuint)str.Length), &p_ptr).TryThrow();
            m_ptr = p_ptr;
        }
    }

    [Drop]
    private void Drop()
    {
        if (ExchangeUtils.ExchangePtr(ref m_ptr, null, out var ptr) is null) return;
        ptr->Release();
    }

    public FrStr16 AsStr => m_ptr->AsStr();
    public ReadOnlySpan<char> AsSpan
    {
        get
        {
            var str = AsStr;
            return new(str.data(), (int)str.size());
        }
    }

    public override string ToString() => m_str;

    #region Equals

    public bool Equals(NativeString16? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return m_ptr == other.m_ptr || m_str == other.m_str;
    }
    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj)
        || (obj is NativeString16 other && Equals(other))
        || (obj is string str && str == m_str);
    public override int GetHashCode() => m_str.GetHashCode();
    public static bool operator ==(NativeString16? left, NativeString16? right) => Equals(left, right);
    public static bool operator !=(NativeString16? left, NativeString16? right) => !Equals(left, right);
    public static bool operator ==(NativeString16? left, string? right) => left?.m_str == right;
    public static bool operator !=(NativeString16? left, string? right) => left?.m_str != right;
    public static bool operator ==(string? left, NativeString16? right) => left == right?.m_str;
    public static bool operator !=(string? left, NativeString16? right) => left != right?.m_str;

    #endregion
}
