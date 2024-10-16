namespace Game.Rendering;

[Flags]
public enum ColorMask : byte
{
    None,

    R = 1 << 0,
    G = 1 << 1,
    B = 1 << 2,
    A = 1 << 3,

    RGBA = R | G | B | A,
    RGB = R | G | B,
    GBA = G | B | A,
    RBA = R | B | A,
    RG = R | G,
    RB = R | B,
    RA = R | A,
    GB = G | B,
    GA = G | A,
    BA = B | A,

    All = RGBA,
}
