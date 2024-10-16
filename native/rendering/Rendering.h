#pragma once
#include <dxgi1_2.h>
#include <dxgi1_4.h>
#include <directx/d3d12.h>
#include <directx/d3dx12_root_signature.h>
#include <D3D12MemAlloc.h>

#include "../Object.h"
#include "../ffi/FFI.h"
#include "../ffi/Rendering.h"

namespace cc
{
    class Queue;
    class RenderingContext;
    class DescriptorSet;

    class Rendering final : public Object<FRendering>
    {
        IMPL_OBJECT();

    public:
        ComPtr<ID3D12Debug> m_debug_controller{};
        ComPtr<IDXGIFactory4> m_factory{};
        ComPtr<IDXGIAdapter1> m_adapter{};
        ComPtr<ID3D12Device2> m_device{};
        ComPtr<ID3D12InfoQueue1> m_info_queue{};
        ComPtr<D3D12MA::Allocator> m_gpu_allocator{};

        Box<Queue> m_queue{};
        Box<Queue> m_queue_compute{};
        Box<Queue> m_queue_copy{};

        Box<DescriptorSet> m_descriptors{};

        ComPtr<ID3D12GraphicsCommandList6> m_current_command_list{};
        ComPtr<ID3D12RootSignature> m_bind_less_root_signature{};

        HANDLE m_fence_event{};

        FRenderingState m_state{};

        HashMap<size_t, Rc<RenderingContext>> m_contexts{};
        DWORD m_callback_cookie{};

    protected:
        ~Rendering() override;

    private:
        void WaitAll() const;
        void WaitCurrentFrame() const;
        void WaitFrame(UINT32 index) const;

        void MoveToNextFrame();

        void ResetCommandAllocator() const;
        void ResetCommandAllocator(UINT32 index) const;
        void ResetAllCommandAllocator() const;

        void AfterSubmit() const;
        void AfterSubmit(UINT32 index) const;

    public:
        static FError Create(FRendering*& out) noexcept;
        static Rc<Rendering> Create();

        explicit Rendering();

        FRenderingState* StatePtr() noexcept override;

        FError MakeContext(FWindowHandle* window_handle, FRenderingContext** out) noexcept override;

        FError CreateGraphicsShaderPipeline(FShaderPassData* pass, FGraphicsShaderPipeline** out) noexcept override;

        FError ReadyFrame() noexcept override;
        FError EndFrame() noexcept override;

        FError GetDevice(void** out) noexcept override;
        FError CurrentCommandList(void** out) noexcept override;

        FError ClearSurface(FRenderingContext* ctx, float4 color) noexcept override;

    };

    class Queue final : public FGpuConsts
    {
        friend Rendering;

    public:
        // Rendering* 持有 Queue
        Rendering* m_rendering;
        ComPtr<ID3D12CommandQueue> m_queue{};
        ComPtr<ID3D12Fence> m_fence{};
        ComPtr<ID3D12CommandAllocator> m_command_allocators[FrameCount]{};
        UINT64 m_frame_current_fence_value[FrameCount]{};
        UINT64 m_fence_value{};

        explicit Queue(
            Rendering* rendering, D3D12_COMMAND_LIST_TYPE type, D3D12_COMMAND_QUEUE_PRIORITY priority, LPCWSTR name
        );

        void Wait(UINT64 fence_value, HANDLE event) const;
        void WaitAll(HANDLE event) const;
        void WaitFrame(UINT32 index, HANDLE event) const;

        void SignalFrame(UINT32 index);

        void ReSet(UINT32 index);
    };

    class RenderingContext final : public Object<FRenderingContext>
    {
        IMPL_OBJECT();

        friend Rendering;

    public:
        size_t m_id;
        // Rendering* 持有 RenderingContext
        Rendering* m_rendering;
        Rc<FWindowHandle> m_window{};
        ComPtr<IDXGISwapChain3> m_swap_chain{};
        ComPtr<ID3D12DescriptorHeap> m_rtv_heap{};
        ComPtr<ID3D12Resource> m_rts[FrameCount]{};
        CD3DX12_CPU_DESCRIPTOR_HANDLE m_current_cpu_handle{};
        UINT m_rtv_descriptor_size{};
        UINT m_frame_index{};

        uint2 m_current_size{};
        uint2 m_new_size{};
        DXGI_FORMAT m_format{DXGI_FORMAT_R8G8B8A8_UNORM};
        bool m_resized{false};

    private:
        void re_create_rts();
        void do_re_size();

        explicit RenderingContext(Rendering* rendering, FWindowHandle* window, void* hwnd, uint2 size);

        void ReadyFrame(ID3D12GraphicsCommandList6* list);
        void EndFrame(ID3D12GraphicsCommandList6* list);
        void Present() const;

    public:
        FError Destroy() noexcept override;

        FError OnResize(uint2 size) noexcept override;
    };

    class DescriptorHeap final : public FGpuConsts
    {
    public:
        constexpr static size_t InitSize = 1024;

        ComPtr<ID3D12DescriptorHeap> m_cpu_heap{};
        ComPtr<ID3D12DescriptorHeap> m_gpu_heap[FrameCount]{};
        size_t m_cpu_size{InitSize};
        size_t m_gpu_size{InitSize};

        explicit DescriptorHeap(const Rendering* rendering, D3D12_DESCRIPTOR_HEAP_TYPE type);

        ID3D12DescriptorHeap* CurrentHeap(UINT32 frame) const;

        void ReadyFrame();
    };

    class DescriptorSet final
    {
    public:
        Rendering* m_rendering;
        DescriptorHeap m_heap_resource;
        DescriptorHeap m_heap_sampler;

        explicit DescriptorSet(Rendering* rendering);

        std::array<ID3D12DescriptorHeap*, 2> CurrentHeaps(UINT32 frame) const;

        void ReadyFrame();
    };
} // cc
