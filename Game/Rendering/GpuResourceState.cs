namespace Game.Rendering;

[Flags]
public enum GpuResourceState
{
    Common = 0x0,
    VertexAndConstantBuffer = 0x1,
    IndexBuffer = 0x2,
    RenderTarget = 0x4,
    UnorderedAccess = 0x8,
    DepthWrite = 0x10,
    DepthRead = 0x20,
    NonPixel = 0x40,
    Pixel = 0x80,
    StreamOut = 0x100,
    IndirectArgument = 0x200,
    CopyDst = 0x400,
    CopySrc = 0x800,
    ResolveDst = 0x1000,
    ResolveSrc = 0x2000,
    RayTracingAccelerationStructure = 0x400000,
    ShadingRateSrc = 0x1000000,
    AllShaderResource = Pixel | NonPixel,
    GenericRead = VertexAndConstantBuffer | IndexBuffer | AllShaderResource | IndirectArgument | CopySrc,
}
