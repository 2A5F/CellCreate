#include "Rendering.h"

#include <ranges>
#include <dxgi1_6.h>
#include <directxtk12/DirectXHelpers.h>
#include <directx/d3dx12_pipeline_state_stream.h>

#include "../utils/error.h"
#include "./gpu_convert.h"
#include "GpuResource.h"
#include "../RenderGraph.h"

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

    namespace static_samplers
    {
        constexpr D3D12_STATIC_SAMPLER_DESC s_static_samplers[] = {
            /* point clamp */ {
                .Filter = D3D12_FILTER_MIN_MAG_MIP_POINT,
                .AddressU = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                .AddressV = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                .AddressW = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                .MaxAnisotropy = 1,
                .MinLOD = -FLT_MAX,
                .MaxLOD = FLT_MAX,
                .ShaderRegister = 0,
            },
            /* point wrap */ {
                .Filter = D3D12_FILTER_MIN_MAG_MIP_POINT,
                .AddressU = D3D12_TEXTURE_ADDRESS_MODE_WRAP,
                .AddressV = D3D12_TEXTURE_ADDRESS_MODE_WRAP,
                .AddressW = D3D12_TEXTURE_ADDRESS_MODE_WRAP,
                .MaxAnisotropy = 1,
                .MinLOD = -FLT_MAX,
                .MaxLOD = FLT_MAX,
                .ShaderRegister = 1,
            },
            /* point mirror */ {
                .Filter = D3D12_FILTER_MIN_MAG_MIP_POINT,
                .AddressU = D3D12_TEXTURE_ADDRESS_MODE_MIRROR,
                .AddressV = D3D12_TEXTURE_ADDRESS_MODE_MIRROR,
                .AddressW = D3D12_TEXTURE_ADDRESS_MODE_MIRROR,
                .MaxAnisotropy = 1,
                .MinLOD = -FLT_MAX,
                .MaxLOD = FLT_MAX,
                .ShaderRegister = 2,
            },
            /* liner clamp */ {
                .Filter = D3D12_FILTER_MIN_MAG_MIP_LINEAR,
                .AddressU = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                .AddressV = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                .AddressW = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                .MaxAnisotropy = 1,
                .MinLOD = -FLT_MAX,
                .MaxLOD = FLT_MAX,
                .ShaderRegister = 3,
            },
            /* liner wrap */ {
                .Filter = D3D12_FILTER_MIN_MAG_MIP_LINEAR,
                .AddressU = D3D12_TEXTURE_ADDRESS_MODE_WRAP,
                .AddressV = D3D12_TEXTURE_ADDRESS_MODE_WRAP,
                .AddressW = D3D12_TEXTURE_ADDRESS_MODE_WRAP,
                .MaxAnisotropy = 1,
                .MinLOD = -FLT_MAX,
                .MaxLOD = FLT_MAX,
                .ShaderRegister = 4,
            },
            /* liner mirror */ {
                .Filter = D3D12_FILTER_MIN_MAG_MIP_LINEAR,
                .AddressU = D3D12_TEXTURE_ADDRESS_MODE_MIRROR,
                .AddressV = D3D12_TEXTURE_ADDRESS_MODE_MIRROR,
                .AddressW = D3D12_TEXTURE_ADDRESS_MODE_MIRROR,
                .MaxAnisotropy = 1,
                .MinLOD = -FLT_MAX,
                .MaxLOD = FLT_MAX,
                .ShaderRegister = 5,
            },
        };
    }
}

Rendering::~Rendering()
{
    if (m_state._on_recording) EndFrame();
    AfterSubmit();
    WaitAll();

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
    }

    // 创建分配器
    {
        D3D12MA::ALLOCATOR_DESC allocator_desc = {};
        allocator_desc.pDevice = m_device.get();
        allocator_desc.pAdapter = m_adapter.get();
        allocator_desc.Flags =
            D3D12MA::ALLOCATOR_FLAG_MSAA_TEXTURES_ALWAYS_COMMITTED |
            D3D12MA::ALLOCATOR_FLAG_DEFAULT_POOLS_NOT_ZEROED;
        check_error << CreateAllocator(&allocator_desc, m_gpu_allocator.put());
    }

    // 创建队列
    {
        m_queue = std::make_unique<Queue>(
            this, D3D12_COMMAND_LIST_TYPE_DIRECT, D3D12_COMMAND_QUEUE_PRIORITY_HIGH, L"Main"
        );
        m_queue_compute = std::make_unique<Queue>(
            this, D3D12_COMMAND_LIST_TYPE_COMPUTE, D3D12_COMMAND_QUEUE_PRIORITY_HIGH, L"Compute"
        );
        m_queue_copy = std::make_unique<Queue>(
            this, D3D12_COMMAND_LIST_TYPE_COPY, D3D12_COMMAND_QUEUE_PRIORITY_NORMAL, L"Copy"
        );
    }

    // 创建当前命令列表
    {
        check_error << m_device->CreateCommandList(
            0, D3D12_COMMAND_LIST_TYPE_DIRECT, m_queue->m_command_allocators[0].get(), nullptr,
            RT_IID_PPV_ARGS(m_current_command_list)
        );
        check_error << m_current_command_list->Close();
    }

    // 创建栅栏事件
    {
        m_fence_event = CreateEvent(nullptr, FALSE, FALSE, nullptr);
        if (m_fence_event == nullptr) winrt::throw_last_error();
    }

    // 创建无绑定根签名
    {
        D3D12_ROOT_SIGNATURE_DESC desc = {};
        desc.Flags = D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT
            | D3D12_ROOT_SIGNATURE_FLAG_CBV_SRV_UAV_HEAP_DIRECTLY_INDEXED
            | D3D12_ROOT_SIGNATURE_FLAG_SAMPLER_HEAP_DIRECTLY_INDEXED;
        desc.pStaticSamplers = static_samplers::s_static_samplers;
        desc.NumStaticSamplers = std::size(static_samplers::s_static_samplers);
        D3D12_ROOT_PARAMETER root_parameter[1] = {};
        root_parameter[0].ParameterType = D3D12_ROOT_PARAMETER_TYPE_32BIT_CONSTANTS;
        root_parameter[0].Constants.Num32BitValues = 4;
        desc.pParameters = root_parameter;
        desc.NumParameters = std::size(root_parameter);
        check_error << DirectX::CreateRootSignature(m_device.get(), &desc, m_bind_less_root_signature.put());

        if (args().debug)
        {
            check_error << m_bind_less_root_signature->SetName(L"Bind Less Root Signature");
        }
    }

    // 创建描述符堆
    {
        m_descriptors = std::make_unique<DescriptorSet>(this);
    }
}

FRenderingState* Rendering::StatePtr() noexcept
{
    return &m_state;
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

FError Rendering::MakeContext(FWindowHandle* window_handle, FGraphicSurface** out) noexcept
{
    void* hwnd;
    uint2 size;
    if (const auto err = window_handle->Hwnd(hwnd); !err.IsNone()) return err;
    if (const auto err = window_handle->Size(size); !err.IsNone()) return err;
    return ferr_back(
        [&]
        {
            Rc r = new GraphicSurface(this, window_handle, hwnd, size);
            m_contexts[r->m_id] = r;
            *out = r.leak();
        }
    );
}

FError Rendering::CreateGraph(FGpuGraph** out) noexcept
{
    return ferr_back(
        [&]
        {
            Rc r = new RenderGraph(this->CloneThis());
            *out = r.leak();
        }
    );
}

FError Rendering::CreateGraphicsShaderPipeline(
    const FShaderPassData* pass, /* opt */const GraphicsPipelineFormatOverride* override,
    FGraphicsShaderPipeline** out
) noexcept
{
    return ferr_back(
        [&]
        {
            Rc r = new GraphicsShaderPipeline(this->CloneThis(), pass, override);
            *out = r.leak();
        }
    );
}

FError Rendering::CreateBuffer(const FGpuBufferCreateOptions* options, FGpuBuffer** out) noexcept
{
    if (options == nullptr) return FError::Common(str16(u"options is null"));
    if (options->size <= 0) return FError::Common(str16(u"size must > 0"));
    return ferr_back(
        [&]
        {
            Rc r = new GpuBuffer(this->CloneThis(), options);
            *out = r.leak();
        }
    );
}

void Rendering::WaitAll() const
{
    m_queue->WaitAll(m_fence_event);
    m_queue_compute->WaitAll(m_fence_event);
    m_queue_copy->WaitAll(m_fence_event);
}

void Rendering::WaitCurrentFrame() const
{
    WaitFrame(m_state._frame_index);
}

void Rendering::WaitFrame(const UINT32 index) const
{
    m_queue->WaitFrame(index, m_fence_event);
    m_queue_compute->WaitFrame(index, m_fence_event);
    m_queue_copy->WaitFrame(index, m_fence_event);
}

void Rendering::MoveToNextFrame()
{
    ++m_state.frame_count;
    ++m_state._frame_index;
    if (m_state._frame_index >= FrameCount) m_state._frame_index = 0;
}

void Rendering::ResetCommandAllocator() const
{
    ResetCommandAllocator(m_state._frame_index);
}

void Rendering::ResetCommandAllocator(const UINT32 index) const
{
    m_queue->ReSet(index);
    m_queue_compute->ReSet(index);
    m_queue_copy->ReSet(index);
}

void Rendering::ResetAllCommandAllocator() const
{
    for (auto i = 0; i < FrameCount; ++i)
    {
        ResetCommandAllocator(i);
    }
}

void Rendering::AfterSubmit() const
{
    AfterSubmit(m_state._frame_index);
}

void Rendering::AfterSubmit(const UINT32 index) const
{
    m_queue->SignalFrame(index);
    m_queue_compute->SignalFrame(index);
    m_queue_copy->SignalFrame(index);
}

FError Rendering::ReadyFrame() noexcept
{
    return ferr_back(
        [&]
        {
            const auto wait_all = std::ranges::any_of(
                m_contexts | std::views::values, [](const auto& context) { return context->m_resized; }
            );
            if (wait_all)
            {
                WaitAll();
                ResetAllCommandAllocator();

                for (const auto& context : m_contexts | std::views::values)
                {
                    if (!context->m_resized) continue;
                    context->do_re_size();
                }

                MoveToNextFrame();
            }
            else
            {
                MoveToNextFrame();
                WaitCurrentFrame();
            }

            ResetCommandAllocator();
            check_error << m_current_command_list->Reset(
                m_queue->m_command_allocators[m_state._frame_index].get(), nullptr
            );

            {
                const auto descriptor_heaps = m_descriptors->CurrentHeaps(m_state._frame_index);
                m_current_command_list->SetDescriptorHeaps(2, descriptor_heaps.data());
                m_current_command_list->SetGraphicsRootSignature(m_bind_less_root_signature.get());
                m_current_command_list->SetComputeRootSignature(m_bind_less_root_signature.get());
            }

            for (const auto& context : m_contexts | std::views::values)
            {
                context->ReadyFrame(m_current_command_list.get());
            }

            m_state._on_recording = true;
        }
    );
}

FError Rendering::EndFrame() noexcept
{
    return ferr_back(
        [&]
        {
            m_state._on_recording = false;

            for (const auto& context : m_contexts | std::views::values)
            {
                context->EndFrame(m_current_command_list.get());
            }

            check_error << m_current_command_list->Close();

            ID3D12CommandList* command_lists[] = {m_current_command_list.get()};
            m_queue->m_queue->ExecuteCommandLists(1, command_lists);

            for (const auto& context : m_contexts | std::views::values)
            {
                context->Present();
            }

            AfterSubmit();
        }
    );
}

FError Rendering::GetDevice(void** out) noexcept
{
    *out = m_device.get();
    return FError::None();
}

FError Rendering::CurrentCommandList(void** out) noexcept
{
    *out = m_current_command_list.get();
    return FError::None();
}

FError Rendering::ClearSurface(FGraphicSurface* ctx, float4 color) noexcept
{
    if (!m_state._on_recording) return FError::Common(str16(u"Frame not started"));
    return ferr_back(
        [&]
        {
            const auto* context = static_cast<GraphicSurface*>(ctx); // NOLINT(*-pro-type-static-cast-downcast)
            m_current_command_list->ClearRenderTargetView(
                context->m_current_cpu_handle, reinterpret_cast<FLOAT*>(&color), 0, nullptr
            );
        }
    );
}

FError Rendering::CurrentFrameRtv(FGraphicSurface* ctx, void** out) noexcept
{
    if (!m_state._on_recording) return FError::Common(str16(u"Frame not started"));
    return ferr_back(
        [&]
        {
            const auto* context = static_cast<GraphicSurface*>(ctx); // NOLINT(*-pro-type-static-cast-downcast)
            *out = reinterpret_cast<void*>(context->m_current_cpu_handle.ptr);
        }
    );
}

Queue::Queue(
    Rendering* rendering, const D3D12_COMMAND_LIST_TYPE type, const D3D12_COMMAND_QUEUE_PRIORITY priority, LPCWSTR name
) : m_rendering(rendering)
{
    auto* device = rendering->m_device.get();

    // 创建队列
    {
        D3D12_COMMAND_QUEUE_DESC queue_desc = {
            .Type = type,
            .Priority = priority,
            .Flags = D3D12_COMMAND_QUEUE_FLAG_NONE,
        };
        check_error << device->CreateCommandQueue(&queue_desc, RT_IID_PPV_ARGS(m_queue));

        if (args().debug)
        {
            const auto item_name = fmt::format(L"{} Queue", name);
            check_error << m_queue->SetName(item_name.c_str());
        }
    }

    // 创建栅栏
    {
        check_error << device->CreateFence(m_fence_value, D3D12_FENCE_FLAG_NONE, RT_IID_PPV_ARGS(m_fence));
        if (args().debug)
        {
            const auto item_name = fmt::format(L"{} Fence", name);
            check_error << m_fence->SetName(item_name.c_str());
        }
    }

    // 创建命令分配器
    {
        for (auto& m_command_allocator : m_command_allocators)
        {
            check_error << device->CreateCommandAllocator(
                D3D12_COMMAND_LIST_TYPE_DIRECT, RT_IID_PPV_ARGS(m_command_allocator)
            );
        }

        if (args().debug)
        {
            for (size_t i = 0; i < FrameCount; ++i)
            {
                const auto item_name = fmt::format(L"{} Command Allocator {}", name, i);
                check_error << m_command_allocators[i]->SetName(item_name.c_str());
            }
        }
    }
}

void Queue::Wait(const UINT64 fence_value, HANDLE event) const
{
    if (m_fence->GetCompletedValue() < fence_value)
    {
        check_error << m_fence->SetEventOnCompletion(fence_value, event);
        WaitForSingleObjectEx(event, INFINITE, false);
    }
}

void Queue::WaitAll(HANDLE event) const
{
    Wait(m_fence_value, event);
}

void Queue::WaitFrame(const UINT32 index, HANDLE event) const
{
    Wait(m_frame_current_fence_value[index], event);
}

void Queue::SignalFrame(const UINT32 index)
{
    check_error << m_queue->Signal(m_fence.get(), m_frame_current_fence_value[index] = ++m_fence_value);
}

void Queue::ReSet(const UINT32 index)
{
    check_error << m_command_allocators[index]->Reset();
}

void GraphicSurface::re_create_rts()
{
    CD3DX12_CPU_DESCRIPTOR_HANDLE rtvHandle(m_rtv_heap->GetCPUDescriptorHandleForHeapStart());

    // 为每一帧创建一个 RTV。
    for (size_t n = 0; n < FrameCount; n++)
    {
        winrt::check_hresult(m_swap_chain->GetBuffer(n, RT_IID_PPV_ARGS(m_rts[n])));
        m_rendering->m_device->CreateRenderTargetView(m_rts[n].get(), nullptr, rtvHandle);
        rtvHandle.Offset(1, m_rtv_descriptor_size);
    }
}

void GraphicSurface::do_re_size()
{
    for (auto& rt : m_rts) rt = nullptr;

    DXGI_SWAP_CHAIN_DESC1 desc = {};
    check_error << m_swap_chain->GetDesc1(&desc);
    check_error << m_swap_chain->ResizeBuffers(
        FrameCount, m_new_size.x, m_new_size.y, desc.Format, desc.Flags
    );

    m_data.size = m_current_size = m_new_size;

    re_create_rts();

    m_resized = false;
}

namespace
{
    std::atomic_size_t s_rendering_context_id_inc{0};
}

GraphicSurface::GraphicSurface(Rendering* rendering, FWindowHandle* window, void* hwnd, uint2 size)
    : m_id(s_rendering_context_id_inc++), m_rendering(rendering), m_current_size(size), m_new_size(size)
{
    m_data.size = size;
    m_window = Rc<FWindowHandle>::UnsafeClone(window);

    const auto device = m_rendering->m_device.get();

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
        check_error << rendering->m_factory->CreateSwapChainForHwnd(
            rendering->m_queue->m_queue.get(),
            static_cast<HWND>(hwnd),
            &swap_chain_desc,
            nullptr, nullptr,
            swap_chain.put()
        );
        swap_chain.as(m_swap_chain);
    }

    // 创建交换链RTV描述符堆
    {
        D3D12_DESCRIPTOR_HEAP_DESC rtv_heap_desc = {};
        rtv_heap_desc.NumDescriptors = FrameCount;
        rtv_heap_desc.Type = D3D12_DESCRIPTOR_HEAP_TYPE_RTV;
        rtv_heap_desc.Flags = D3D12_DESCRIPTOR_HEAP_FLAG_NONE;
        check_error << device->CreateDescriptorHeap(&rtv_heap_desc, RT_IID_PPV_ARGS(m_rtv_heap));
        m_rtv_descriptor_size = device->GetDescriptorHandleIncrementSize(
            D3D12_DESCRIPTOR_HEAP_TYPE_RTV
        );
    }

    re_create_rts();
}

void GraphicSurface::ReadyFrame(ID3D12GraphicsCommandList6* list)
{
    m_frame_index = m_swap_chain->GetCurrentBackBufferIndex();
    m_current_cpu_handle = CD3DX12_CPU_DESCRIPTOR_HANDLE(
        m_rtv_heap->GetCPUDescriptorHandleForHeapStart(), static_cast<INT>(m_frame_index), m_rtv_descriptor_size
    );
    m_data.current_frame_rtv = reinterpret_cast<void*>(m_current_cpu_handle.ptr);

    D3D12_RESOURCE_BARRIER barrier{};
    barrier.Type = D3D12_RESOURCE_BARRIER_TYPE_TRANSITION;
    barrier.Transition = {
        .pResource = m_rts[m_frame_index].get(),
        .Subresource = D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES,
        .StateBefore = D3D12_RESOURCE_STATE_PRESENT,
        .StateAfter = D3D12_RESOURCE_STATE_RENDER_TARGET,
    };
    list->ResourceBarrier(1, &barrier);
}

void GraphicSurface::EndFrame(ID3D12GraphicsCommandList6* list)
{
    D3D12_RESOURCE_BARRIER barrier{};
    barrier.Type = D3D12_RESOURCE_BARRIER_TYPE_TRANSITION;
    barrier.Transition = {
        .pResource = m_rts[m_frame_index].get(),
        .Subresource = D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES,
        .StateBefore = D3D12_RESOURCE_STATE_RENDER_TARGET,
        .StateAfter = D3D12_RESOURCE_STATE_PRESENT,
    };
    list->ResourceBarrier(1, &barrier);
}

void GraphicSurface::Present() const
{
    check_error << m_swap_chain->Present(m_rendering->m_state.v_sync ? 1 : 0, 0);
}

FError GraphicSurface::Destroy() noexcept
{
    return ferr_back(
        [&]
        {
            m_rendering->m_contexts.erase(m_id);
        }
    );
}

FError GraphicSurface::DataPtr(FGraphicSurfaceData** out) noexcept
{
    *out = &m_data;
    return FError::None();
}

FError GraphicSurface::OnResize(uint2 size) noexcept
{
    if (m_current_size == size) return FError::None();
    m_new_size = size;
    m_resized = true;
    return FError::None();
}

DescriptorHeap::DescriptorHeap(const Rendering* rendering, const D3D12_DESCRIPTOR_HEAP_TYPE type)
{
    auto* device = rendering->m_device.get();
    D3D12_DESCRIPTOR_HEAP_DESC desc{};
    desc.Type = type;
    desc.NumDescriptors = InitSize;
    check_error << device->CreateDescriptorHeap(&desc, RT_IID_PPV_ARGS(m_cpu_heap));
    desc.Flags = D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE;
    for (auto& gpu_heap : m_gpu_heap)
    {
        check_error << device->CreateDescriptorHeap(&desc, RT_IID_PPV_ARGS(gpu_heap));
    }

    if (args().debug)
    {
        auto t = type == D3D12_DESCRIPTOR_HEAP_TYPE_SAMPLER ? L"Sampler" : L"Resource";
        {
            const auto item_name = fmt::format(L"Cpu {} Descriptor Heap", t);
            check_error << m_cpu_heap->SetName(item_name.c_str());
        }
        for (int i = 0; i < FrameCount; ++i)
        {
            if (m_gpu_heap[i] != nullptr)
            {
                const auto item_name = fmt::format(L"Gpu {} Descriptor Heap {}", t, i);
                check_error << m_gpu_heap[i]->SetName(item_name.c_str());
            }
        }
    }
}

ID3D12DescriptorHeap* DescriptorHeap::CurrentHeap(const UINT32 frame) const
{
    return m_gpu_heap[frame].get();
}

void DescriptorHeap::ReadyFrame()
{
    // todo
}

DescriptorSet::DescriptorSet(Rendering* rendering) : m_rendering(rendering),
    m_heap_resource(rendering, D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV),
    m_heap_sampler(rendering, D3D12_DESCRIPTOR_HEAP_TYPE_SAMPLER)
{
}

std::array<ID3D12DescriptorHeap*, 2> DescriptorSet::CurrentHeaps(const UINT32 frame) const
{
    return {m_heap_resource.CurrentHeap(frame), m_heap_sampler.CurrentHeap(frame)};
}

void DescriptorSet::ReadyFrame()
{
    m_heap_resource.ReadyFrame();
    m_heap_sampler.ReadyFrame();
}

ShaderPass::ShaderPass(const FShaderPassData* data) : m_data(*data)
{
    for (auto i = 0; i < MaxModules; ++i)
    {
        const auto& module = data->modules[i];
        const auto span = std::span(module.data(), module.size());
        new(&m_modules[i]) std::vector(span.begin(), span.end());
        new(&m_data.modules[i]) FrBlob(m_modules[i].data(), m_modules[i].size());
    }
}

FError ShaderPass::DataPtr(FShaderPassData** out) noexcept
{
    *out = &m_data;
    return FError::None();
}

namespace
{
    void set_state(auto& desc, const auto& state)
    {
        to_dx(desc.BlendState, state.blend_state, state.rt_count);
        desc.SampleMask = state.sample_mask;
        to_dx(desc.RasterizerState, state.rasterizer_state);
        to_dx(desc.DepthStencilState, state.depth_stencil_state);
        desc.PrimitiveTopologyType = to_dx_t(state.primitive_topology);
        desc.NumRenderTargets = state.rt_count;
        desc.SampleDesc.Count = state.sample_state.count;
        desc.SampleDesc.Quality = state.sample_state.quality;
        for (int i = 0; i < state.rt_count; ++i)
        {
            desc.RTVFormats[i] = to_dx(state.rtv_formats[i]);
        }
        desc.DSVFormat = to_dx(state.dsv_format);
    }
}

GraphicsShaderPipeline::GraphicsShaderPipeline(
    Rc<Rendering>&& rendering, const FShaderPassData* pass, /* opt */ const GraphicsPipelineFormatOverride* override
) : m_rendering(
    std::move(rendering)
), m_state(pass->state)
{
    CD3DX12_PIPELINE_STATE_STREAM2 stream{};
    D3D12_INPUT_ELEMENT_DESC input_elements{};

    if (override)
    {
        m_state.rt_count = override->rt_count;
        m_state.dsv_format = override->dsv_format;
        for (auto i = 0; i < override->rt_count; ++i)
        {
            m_state.rtv_formats[i] = override->rtv_formats[i];
        }
    }

    if (pass->stages.vs)
    {
        m_is_mesh_shader = false;

        const auto& ps = pass->modules[0];
        const auto& vs = pass->modules[1];

        D3D12_GRAPHICS_PIPELINE_STATE_DESC desc{};
        desc.pRootSignature = m_rendering->m_bind_less_root_signature.get();
        desc.PS = {ps.data(), ps.size()};
        desc.VS = {vs.data(), vs.size()};

        set_state(desc, m_state);

        // todo 反射

        // desc.InputLayout.NumElements = 1;
        // desc.InputLayout.pInputElementDescs = &input_elements;
        //
        // input_elements.SemanticName = "POSITION";
        // input_elements.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
        // input_elements.SemanticIndex = 0;
        // input_elements.InputSlot = 0;
        // input_elements.AlignedByteOffset = 0;
        // input_elements.InputSlotClass = D3D12_INPUT_CLASSIFICATION_PER_VERTEX_DATA;
        // input_elements.InstanceDataStepRate = 0;

        stream = CD3DX12_PIPELINE_STATE_STREAM2(desc);
    }
    else if (pass->stages.ms)
    {
        m_is_mesh_shader = true;

        const auto& ps = pass->modules[0];
        const auto& ms = pass->modules[1];
        const auto& ts = pass->stages.ts ? pass->modules[2] : FrBlob(nullptr, 0);

        D3DX12_MESH_SHADER_PIPELINE_STATE_DESC desc{};
        desc.pRootSignature = m_rendering->m_bind_less_root_signature.get();
        desc.PS = {ps.data(), ps.size()};
        desc.MS = {ms.data(), ms.size()};
        if (pass->stages.ts) desc.AS = {ts.data(), ts.size()};

        set_state(desc, m_state);

        stream = CD3DX12_PIPELINE_STATE_STREAM2(desc);
    }
    else throw CcError("Missing vertex or mesh shader");

    D3D12_PIPELINE_STATE_STREAM_DESC stream_desc;
    stream_desc.pPipelineStateSubobjectStream = &stream;
    stream_desc.SizeInBytes = sizeof(stream);

    check_error << m_rendering->m_device->CreatePipelineState(&stream_desc, RT_IID_PPV_ARGS(m_pipeline_state));
}

FError GraphicsShaderPipeline::RawPtr(void** out) const noexcept
{
    *out = m_pipeline_state.get();
    return FError::None();
}

FError GraphicsShaderPipeline::StatePtr(const GraphicsPipelineState** out) const noexcept
{
    *out = &m_state;
    return FError::None();
}
