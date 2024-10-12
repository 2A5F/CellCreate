#pragma once
#include <dxgi1_2.h>
#include <dxgi1_4.h>
#include <dxgi1_6.h>
#include <directx/d3d12.h>
#include <directx/d3dx12_root_signature.h>
#include <D3D12MemAlloc.h>

#include "../Object.h"
#include "../ffi/FFI.h"
#include "../ffi/Rendering.h"

namespace cc
{
    class Rendering;

    class Rendering final : public Object<FRendering>
    {
        IMPL_OBJECT();

    public:
        ComPtr<ID3D12Debug> m_debug_controller{};
        ComPtr<IDXGIFactory4> m_factory{};
        ComPtr<IDXGIAdapter1> m_adapter{};

        ComPtr<ID3D12Device2> m_device{};
        ComPtr<ID3D12InfoQueue1> m_info_queue{};
        DWORD m_callback_cookie{};

        ComPtr<D3D12MA::Allocator> m_gpu_allocator{};

        ComPtr<ID3D12CommandQueue> m_queue{};
        ComPtr<ID3D12CommandQueue> m_queue_compute{};
        ComPtr<ID3D12CommandQueue> m_queue_copy{};

        ComPtr<ID3D12CommandAllocator> m_command_allocator{};
        ComPtr<ID3D12CommandAllocator> m_command_allocator_compute{};
        ComPtr<ID3D12CommandAllocator> m_command_allocator_copy{};

        ComPtr<IDXGISwapChain3> m_swap_chain{};
        ComPtr<ID3D12DescriptorHeap> m_rtv_heap{};
        UINT m_rtv_descriptor_size{};
        UINT m_frame_index{};
        ComPtr<ID3D12Resource> m_rts[FrameCount]{};
        CD3DX12_CPU_DESCRIPTOR_HANDLE m_current_cpu_handle{};

        ComPtr<ID3D12Fence> m_fence{};
        UINT64 m_frame_fence_values[FrameCount]{};
        UINT64 m_fence_value{};
        HANDLE m_fence_event{};

        uint2 m_current_size{};
        uint2 m_new_size{};
        DXGI_FORMAT m_format{DXGI_FORMAT_R8G8B8A8_UNORM};
        b8 m_resized{false};
        b8 m_v_sync{false};

        void re_create_rts();

        void Wait(UINT64 fence_value);
        void WaitAll();
        void WaitCurrent();
        void Signal(UINT64 fence_value);
        void SignalCurrent();

    protected:
        ~Rendering() override;

    public:
        static FError Create(FRendering*& out) noexcept;
        static Rc<Rendering> Create();

        explicit Rendering();

        FError Init(FWindowHandle* window_handle) noexcept override;
        void init(void* hwnd, uint2 size);


        FError OnResize(uint2 size) noexcept override;
        b8 VSync() noexcept override;
        FError SetVSync(b8 enable) noexcept override;
        FError ReadyFrame() noexcept override;
        FError EndFrame() noexcept override;
    };
} // cc
