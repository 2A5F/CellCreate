using System.Runtime.CompilerServices;
using Coplt.Dropping;
using Game.Native;
using Silk.NET.Direct3D12;

namespace Game.Rendering;

[Dropping(Unmanaged = true)]
public sealed unsafe partial class GpuBuffer : IGpuResource
{
    internal FGpuBuffer* m_ptr;
    internal readonly FGpuResourceData* m_data;

    public record struct Options(uint Size)
    {
        public GpuResourceState InitialState { get; set; } = GpuResourceState.Common;
        public GpuHeapType HeapType { get; set; } = GpuHeapType.Gpu;
        public bool Uav { get; set; } = false;
    }

    internal GpuBuffer(FGpuBuffer* ptr)
    {
        m_ptr = ptr;
        FGpuResourceData* p_data;
        m_ptr->DataPtr(&p_data).TryThrow();
        m_data = p_data;
    }

    [Drop]
    private void Drop()
    {
        if (ExchangeUtils.ExchangePtr(ref m_ptr, null, out var ptr) is null) return;
        ptr->Release();
    }

    public GpuResourceState State
    {
        get => Unsafe.BitCast<FGpuResourceState, GpuResourceState>(m_data->state);
        internal set => m_data->state = Unsafe.BitCast<GpuResourceState, FGpuResourceState>(value);
    }

    public ID3D12Resource* RawPtr
    {
        get
        {
            ID3D12Resource* ptr;
            m_ptr->RawPtr((void**)&ptr).TryThrow();
            return ptr;
        }
    }

    public void SetName(string name)
    {
        fixed (char* ptr = name)
        {
            m_ptr->SetName(ptr).TryThrow();
        }
    }
}
