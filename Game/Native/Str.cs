using System.Text;

namespace Game.Native;

public partial struct FrStr8
{
    public unsafe ReadOnlySpan<byte> AsSpan => new(m_data, (int)m_size);
    public override string ToString() => Encoding.UTF8.GetString(AsSpan);
    public static implicit operator FrStr8(FmStr8 str) => new(str);
}

public partial struct FmStr8
{
    public unsafe Span<byte> AsSpan => new(m_data, (int)m_size);
    public override unsafe string ToString() => Encoding.UTF8.GetString(AsSpan);
}

public partial struct FrStr16
{
    public unsafe ReadOnlySpan<char> AsSpan => new(m_data, (int)m_size);
    public override unsafe string ToString() => AsSpan.ToString();
    public static implicit operator FrStr16(FmStr16 str) => new(str);
}

public partial struct FmStr16
{
    public unsafe Span<char> AsSpan => new(m_data, (int)m_size);
    public override unsafe string ToString() => AsSpan.ToString();
}
