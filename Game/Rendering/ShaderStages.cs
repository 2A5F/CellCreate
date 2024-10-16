namespace Game.Rendering;

[Flags]
public enum ShaderStages : byte
{
    Cs = 1 << 1,
    Ps = 1 << 2,
    Vs = 1 << 3,
    Ms = 1 << 4,
    Ts = 1 << 5,
}
