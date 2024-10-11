using System.Runtime.InteropServices;

namespace Game.Native;

public readonly record struct B8(sbyte value)
{
    public static bool operator true(B8 v) => v.value != 0;
    public static bool operator false(B8 v) => v.value == 0;

    public static implicit operator bool(B8 v) => v.value != 0;
    public static implicit operator B8(bool v) => new(v ? (sbyte)1 : (sbyte)0);

    public override string ToString() => this ? "true" : "false";
}

public readonly record struct B32(int value)
{
    public static bool operator true(B32 v) => v.value != 0;
    public static bool operator false(B32 v) => v.value == 0;

    public static implicit operator bool(B32 v) => v.value != 0;
    public static implicit operator B32(bool v) => new(v ? 1 : 0);

    public override string ToString() => this ? "true" : "false";
}
