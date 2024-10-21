namespace Game.Rendering;

public record struct ViewPort(float X, float Y, float Width, float Height, float MinDepth = 0, float MaxDepth = 1)
{
    public float X = X;
    public float Y = Y;
    public float Width = Width;
    public float Height = Height;
    public float MinDepth = MinDepth;
    public float MaxDepth = MaxDepth;

    public ViewPort(float width, float height) : this(0, 0, width, height) { }
}
