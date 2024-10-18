using Silk.NET.Direct3D12;

namespace Game.Rendering;

public unsafe interface IGpuResource
{
    public GpuResourceState State { get; }
    public ID3D12Resource* RawPtr { get; }
    public void SetName(string name);
}
