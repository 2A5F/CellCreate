using System.Text;

namespace Game.Native;

public partial struct FrStr8
{
    public override unsafe string ToString() => Encoding.UTF8.GetString(new Span<byte>(m_data, (int)m_size));
}

public partial struct FmStr8
{
    public override unsafe string ToString() => Encoding.UTF8.GetString(new Span<byte>(m_data, (int)m_size));
}

public partial struct FrStr16
{
    public override unsafe string ToString() => new Span<char>(m_data, (int)m_size).ToString();
}

public partial struct FmStr16
{
    public override unsafe string ToString() => new Span<char>(m_data, (int)m_size).ToString();
}
