#include "Rendering.h"

#include "../utils/error.h"

using namespace cc;

namespace
{
    bool get_adapter(IDXGIFactory1& factory, ComPtr<IDXGIAdapter1>& adapter);

    void debug_callback(
        D3D12_MESSAGE_CATEGORY Category,
        D3D12_MESSAGE_SEVERITY Severity,
        D3D12_MESSAGE_ID ID,
        LPCSTR pDescription,
        void* pContext
    )
    {
        FLogLevel level = FLogLevel::Debug;
        if (Severity <= D3D12_MESSAGE_SEVERITY_ERROR)
        {
            level = FLogLevel::Error;
        }
        else if (Severity == D3D12_MESSAGE_SEVERITY_WARNING)
        {
            level = FLogLevel::Warn;
        }
        else if (Severity == D3D12_MESSAGE_SEVERITY_INFO)
        {
            level = FLogLevel::Info;
        }

        const auto msg = fmt::format("[DirectX] {}", pDescription);
        Log(level, FrStr8(reinterpret_cast<const char8_t*>(msg.data()), msg.size())); //todo 添加重载避免格式化
    }
}

void Rendering::re_create_rts()
{
    CD3DX12_CPU_DESCRIPTOR_HANDLE rtvHandle(m_rtv_heap->GetCPUDescriptorHandleForHeapStart());

    // 为每一帧创建一个 RTV。
    for (size_t n = 0; n < FrameCount; n++)
    {
        winrt::check_hresult(m_swap_chain->GetBuffer(n, RT_IID_PPV_ARGS(m_rts[n])));
        m_device->CreateRenderTargetView(m_rts[n].get(), nullptr, rtvHandle);
        rtvHandle.Offset(1, m_rtv_descriptor_size);
    }
}

void Rendering::Wait(const UINT64 fence_value)
{
    if (m_fence->GetCompletedValue() < fence_value)
    {
        check_error << m_fence->SetEventOnCompletion(fence_value, m_fence_event);
        WaitForSingleObjectEx(m_fence_event, INFINITE, false);
    }
}

void Rendering::WaitAll()
{
    Wait(m_fence_value);
}

void Rendering::WaitCurrent()
{
    Wait(m_frame_fence_values[m_frame_index]);
}

void Rendering::Signal(const UINT64 fence_value)
{
    check_error << m_queue->Signal(m_fence.get(), fence_value);
}

void Rendering::SignalCurrent()
{
    Signal(m_frame_fence_values[m_frame_index] = ++m_fence_value);
}

Rendering::~Rendering()
{
    if (m_info_queue.get() != nullptr && m_callback_cookie != 0)
    {
        check_error << m_info_queue->UnregisterMessageCallback(m_callback_cookie);
    }
}

FError Rendering::Create(FRendering*& out) noexcept
{
    return ferr_back(
        [&]
        {
            auto r = Create();
            out = r.leak();
        }
    );
}

Rc<Rendering> Rendering::Create()
{
    return new Rendering();
}

Rendering::Rendering()
{
    // 创建设配器
    {
        UINT dxgi_factory_flags = 0;

        if (args().debug)
        {
            if (SUCCEEDED(D3D12GetDebugInterface(RT_IID_PPV_ARGS(m_debug_controller))))
            {
                m_debug_controller->EnableDebugLayer();
                dxgi_factory_flags |= DXGI_CREATE_FACTORY_DEBUG;
            }
        }

        check_error << CreateDXGIFactory2(dxgi_factory_flags, RT_IID_PPV_ARGS(m_factory));

        if (!get_adapter(*m_factory, m_adapter))
            throw CcError(
                "Unable to create render context, no graphics device or graphics device does not support"
            );
    }

    // 创建设备
    {
        check_error << D3D12CreateDevice(m_adapter.get(), D3D_FEATURE_LEVEL_12_2, RT_IID_PPV_ARGS(m_device));

        // 检查功能支持
        {
            D3D12_FEATURE_DATA_SHADER_MODEL shader_model = {D3D_SHADER_MODEL_6_6};
            if (FAILED(m_device->CheckFeatureSupport(D3D12_FEATURE_SHADER_MODEL, &shader_model, sizeof(shader_model)))
                || (shader_model.HighestShaderModel < D3D_SHADER_MODEL_6_6))
            {
                throw CcError("Shader Model 6.6 is not supported");
            }

            D3D12_FEATURE_DATA_D3D12_OPTIONS7 features = {};
            if (FAILED(m_device->CheckFeatureSupport(D3D12_FEATURE_D3D12_OPTIONS7, &features, sizeof(features)))
                || (features.MeshShaderTier == D3D12_MESH_SHADER_TIER_NOT_SUPPORTED))
            {
                throw CcError("Mesh Shader is not supported");
            }
        }

        if (args().debug)
        {
            check_error << m_device->SetName(L"Main Device");
            if (SUCCEEDED(m_device -> QueryInterface(RT_IID_PPV_ARGS(m_info_queue))))
            {
                if (!SUCCEEDED(
                    m_info_queue->RegisterMessageCallback(
                        debug_callback, D3D12_MESSAGE_CALLBACK_FLAG_NONE, this, & m_callback_cookie)
                ))
                {
                    spdlog::warn("register message callback failed");
                }
            }
        }

        // 创建分配器
        {
            D3D12MA::ALLOCATOR_DESC allocator_desc = {};
            allocator_desc.pDevice = m_device.get();
            allocator_desc.pAdapter = m_adapter.get();
            allocator_desc.Flags =
                D3D12MA::ALLOCATOR_FLAG_MSAA_TEXTURES_ALWAYS_COMMITTED |
                D3D12MA::ALLOCATOR_FLAG_DEFAULT_POOLS_NOT_ZEROED;
            winrt::check_hresult(CreateAllocator(&allocator_desc, m_gpu_allocator.put()));
        }
    }

    // 创建队列
    {
        D3D12_COMMAND_QUEUE_DESC queue_desc = {
            .Type = D3D12_COMMAND_LIST_TYPE_DIRECT,
            .Priority = D3D12_COMMAND_QUEUE_PRIORITY_HIGH,
            .Flags = D3D12_COMMAND_QUEUE_FLAG_NONE,
        };
        check_error << m_device->CreateCommandQueue(&queue_desc, RT_IID_PPV_ARGS(m_queue));
        queue_desc.Type = D3D12_COMMAND_LIST_TYPE_COMPUTE;
        check_error << m_device->CreateCommandQueue(&queue_desc, RT_IID_PPV_ARGS(m_queue_compute));
        queue_desc.Type = D3D12_COMMAND_LIST_TYPE_COPY;
        queue_desc.Priority = D3D12_COMMAND_QUEUE_PRIORITY_NORMAL;
        check_error << m_device->CreateCommandQueue(&queue_desc, RT_IID_PPV_ARGS(m_queue_copy));

        if (args().debug)
        {
            check_error << m_queue->SetName(L"Main Queue");
            check_error << m_queue_compute->SetName(L"Compute Queue");
            check_error << m_queue_copy->SetName(L"Copy Queue");
        }
    }

    // 创建命令分配器
    {
        check_error << m_device->CreateCommandAllocator(
            D3D12_COMMAND_LIST_TYPE_DIRECT, RT_IID_PPV_ARGS(m_command_allocator)
        );
        check_error << m_device->CreateCommandAllocator(
            D3D12_COMMAND_LIST_TYPE_COMPUTE, RT_IID_PPV_ARGS(m_command_allocator_compute)
        );
        check_error << m_device->CreateCommandAllocator(
            D3D12_COMMAND_LIST_TYPE_COPY, RT_IID_PPV_ARGS(m_command_allocator_copy)
        );

        if (args().debug)
        {
            check_error << m_command_allocator->SetName(L"Main Command Allocator");
            check_error << m_command_allocator_compute->SetName(L"Compute Command Allocator");
            check_error << m_command_allocator_copy->SetName(L"Copy Command Allocator");
        }
    }

    // 创建栅栏
    {
        check_error << m_device->CreateFence(
            m_fence_value, D3D12_FENCE_FLAG_NONE, RT_IID_PPV_ARGS(m_fence)
        );

        m_fence_event = CreateEvent(nullptr, FALSE, FALSE, nullptr);
        if (m_fence_event == nullptr) winrt::throw_last_error();
    }
}

namespace
{
    bool get_adapter(IDXGIFactory1& factory, ComPtr<IDXGIAdapter1>& adapter)
    {
        ComPtr<IDXGIFactory6> factory6{};
        if (SUCCEEDED(factory .QueryInterface(RT_IID_PPV_ARGS(factory6))))
        {
            for (UINT adapter_index = 0;
                 SUCCEEDED(
                     factory6->EnumAdapterByGpuPreference(
                         adapter_index,
                         DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE,
                         RT_IID_PPV_ARGS(adapter)
                     )
                 );
                 ++adapter_index
            )
            {
                DXGI_ADAPTER_DESC1 desc;
                check_error << adapter->GetDesc1(&desc);

                if (SUCCEEDED(
                    D3D12CreateDevice(adapter.get(), D3D_FEATURE_LEVEL_12_2, _uuidof(ID3D12Device), nullptr)
                ))
                {
                    break;
                }
            }
        }

        if (adapter.get() == nullptr)
        {
            for (
                UINT adapter_index = 0;
                SUCCEEDED(factory.EnumAdapters1(adapter_index, adapter.put()));
                ++adapter_index
            )
            {
                DXGI_ADAPTER_DESC1 desc;
                check_error << adapter->GetDesc1(&desc);

                if (SUCCEEDED(
                    D3D12CreateDevice(adapter.get(), D3D_FEATURE_LEVEL_12_2, _uuidof(ID3D12Device), nullptr)
                ))
                {
                    break;
                }
            }
        }

        return adapter.get() != nullptr;
    }
}

FError Rendering::Init(FWindowHandle* window_handle) noexcept
{
    void* hwnd;
    uint2 size;
    if (const auto err = window_handle->Hwnd(hwnd); !err.IsNone()) return err;
    if (const auto err = window_handle->Size(size); !err.IsNone()) return err;
    return ferr_back(
        [&]
        {
            init(hwnd, size);
        }
    );
}

void Rendering::init(void* hwnd, uint2 size)
{
    m_current_size = size;

    // 创建交换链
    {
        DXGI_SWAP_CHAIN_DESC1 swap_chain_desc = {};
        swap_chain_desc.BufferCount = FrameCount;
        swap_chain_desc.Width = size.x;
        swap_chain_desc.Height = size.y;
        swap_chain_desc.Format = m_format;
        swap_chain_desc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
        swap_chain_desc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD;
        swap_chain_desc.SampleDesc.Count = 1;

        ComPtr<IDXGISwapChain1> swap_chain;
        check_error << m_factory->CreateSwapChainForHwnd(
            m_queue.get(),
            static_cast<HWND>(hwnd),
            &swap_chain_desc,
            nullptr, nullptr,
            swap_chain.put()
        );
        swap_chain.as(m_swap_chain);
    }

    m_frame_index = m_swap_chain->GetCurrentBackBufferIndex();

    /* 创建交换链RTV描述符堆 */
    {
        D3D12_DESCRIPTOR_HEAP_DESC rtv_heap_desc = {};
        rtv_heap_desc.NumDescriptors = FrameCount;
        rtv_heap_desc.Type = D3D12_DESCRIPTOR_HEAP_TYPE_RTV;
        rtv_heap_desc.Flags = D3D12_DESCRIPTOR_HEAP_FLAG_NONE;
        winrt::check_hresult(
            m_device->CreateDescriptorHeap(&rtv_heap_desc, RT_IID_PPV_ARGS(m_rtv_heap))
        );
        m_rtv_descriptor_size = m_device->GetDescriptorHandleIncrementSize(
            D3D12_DESCRIPTOR_HEAP_TYPE_RTV
        );
    }

    re_create_rts();
}

FError Rendering::OnResize(uint2 size) noexcept
{
    if (m_current_size == size) return FError::None();
    m_new_size = size;
    m_resized = true;
    return FError::None();
}

b8 Rendering::VSync() noexcept
{
    return m_v_sync;
}

FError Rendering::SetVSync(const b8 enable) noexcept
{
    m_v_sync = enable;
    return FError::None();
}

FError Rendering::ReadyFrame() noexcept
{
    // todo

    return FError::None();
}

FError Rendering::EndFrame() noexcept
{
    // todo

    return FError::None();
}
