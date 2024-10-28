#pragma once
#include <dxgi1_2.h>
#include <dxgi1_4.h>
#include <directx/d3d12.h>
#include <directx/d3dx12_root_signature.h>
#include <D3D12MemAlloc.h>

#include <concurrentqueue/concurrentqueue.h>
#include <parallel_hashmap/phmap.h>

#include "../Object.h"
#include "../ffi/FFI.h"
#include "../ffi/Rendering.h"
#include "../utils/rent.h"

namespace cc
{
    class ShaderPass;
    class GraphicsShaderPipeline;
    class GpuTask;
    class GpuFence;
    class GpuGraphicQueue;
    class GpuBackgroundQueue;
    class GraphicSurface;
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

        Rc<GpuGraphicQueue> m_queue{};
        Rc<GpuBackgroundQueue> m_queue_compute{};
        Rc<GpuBackgroundQueue> m_queue_copy{};

        Box<DescriptorSet> m_descriptors{};

        ComPtr<ID3D12RootSignature> m_bind_less_root_signature{};

        FRenderingState m_state{};

        HashMap<size_t, Rc<GraphicSurface>> m_contexts{};
        DWORD m_callback_cookie{};

    protected:
        ~Rendering() override;

    private:
        void WaitAllFrame() const;
        void WaitCurrentFrame() const;

        void MoveToNextFrame();

    public:
        static FError Create(FRendering*& out) noexcept;
        static Rc<Rendering> Create();

        explicit Rendering();

        FRenderingState* StatePtr() noexcept override;
        const Rc<GpuTask>& CurrentTask() const;

        FError MakeContext(FWindowHandle* window_handle, FGraphicSurface** out) noexcept override;

        FError CreateShaderPass(const FShaderPassData* data, FShaderPass** out) noexcept override;
        FError CreateGraph(FGpuGraph** out) noexcept override;
        FError CreateGraphicsShaderPipeline(
            const FShaderPassData* pass, /* opt */const GraphicsPipelineFormatOverride* override,
            FGraphicsShaderPipeline** out
        ) noexcept override;
        FError CreateBuffer(const FGpuBufferCreateOptions* options, FGpuBuffer** out) noexcept override;

        FError ReadyFrame() noexcept override;
        FError EndFrame() noexcept override;

        FError GetDevice(void** out) noexcept override;
        FError CurrentCommandList(void** out) noexcept override;

        FError ClearSurface(FGraphicSurface* ctx, float4 color) noexcept override;
        FError CurrentFrameRtv(FGraphicSurface* ctx, void** out) noexcept override;
    };

    class AGpuQueue : public Object<FGpuQueue>
    {
        friend Rendering;

    public:
        // Rendering* 持有 Queue
        Rendering* m_rendering;
        ComPtr<ID3D12CommandQueue> m_queue{};
        D3D12_COMMAND_LIST_TYPE m_type{};

        std::optional<std::wstring> m_name{};

        explicit AGpuQueue(
            Rendering* rendering, D3D12_COMMAND_LIST_TYPE type, D3D12_COMMAND_QUEUE_PRIORITY priority,
            std::optional<std::wstring>&& name
        );
    };

    class GpuGraphicQueue final : public AGpuQueue
    {
        IMPL_OBJECT();

        friend Rendering;

    public:
        Rc<GpuTask> m_tasks[FrameCount]{};

        explicit GpuGraphicQueue(
            Rendering* rendering, D3D12_COMMAND_LIST_TYPE type, D3D12_COMMAND_QUEUE_PRIORITY priority,
            std::optional<std::wstring>&& name
        );

        // 获取指定帧的 task
        const Rc<GpuTask>& GetTask(u32 frame) const noexcept;
    };

    class GpuBackgroundQueue final : public AGpuQueue
    {
        IMPL_OBJECT();

        friend Rendering;

    public:
        std::atomic<size_t> m_task_id{0};
        moodycamel::ConcurrentQueue<Rc<GpuTask>> m_task_pool{};

        explicit GpuBackgroundQueue(
            Rendering* rendering, D3D12_COMMAND_LIST_TYPE type, D3D12_COMMAND_QUEUE_PRIORITY priority,
            std::optional<std::wstring>&& name
        );

        // 租借 task
        Rent<Rc<GpuTask>> RentTask();
        // 归还 task，将会尝试提交和等待
        void ReturnTask(Rc<GpuTask>&& task);
        // 归还 task，不会尝试提交和等待
        void UnsafeReturnTask(Rent<Rc<GpuTask>>&& task);
    };

    class GpuFence final : public Object<>
    {
        IMPL_OBJECT();

    protected:
        ~GpuFence() override;

    public:
        UINT64 m_fence_value{};
        ComPtr<ID3D12Fence> m_fence{};
        HANDLE m_fence_event{};

        explicit GpuFence(ID3D12Device& device);

        void SetName(LPCWSTR name) const;

        void Wait() const;

        void Signal(ID3D12CommandQueue& queue);
    };

    class GpuTask final : public Object<FGpuTask>
    {
        IMPL_OBJECT();

    public:
        // Queue* 持有 Task
        AGpuQueue* m_queue{};
        Rc<GpuFence> m_fence{};
        ComPtr<ID3D12CommandAllocator> m_command_allocators{};
        ComPtr<ID3D12GraphicsCommandList7> m_command_list{};

        std::optional<std::wstring> m_name{};

        enum class State
        {
            Idle,
            Submitted,
        };

        State m_state{State::Idle};

    protected:
        ~GpuTask() override;

    private:
        void do_reset() const;

    public:
        explicit GpuTask(AGpuQueue* queue, std::optional<std::wstring>&& name);

        ID3D12GraphicsCommandList7* GetList() const;

        void Submit();
        void Wait();
    };

    template <>
    struct Rent<Rc<GpuTask>>
    {
        Rc<Rendering> m_rendering{};
        Rc<GpuBackgroundQueue> m_queue;
        Rc<GpuTask> m_task{};
        bool m_not_moved{true};

        explicit Rent(Rc<GpuBackgroundQueue>&& queue, Rc<GpuTask>&& task) :
            m_rendering(queue->m_rendering->CloneThis()),
            m_queue(std::move(queue)), m_task(std::move(task))
        {
        }

        Rent(Rent& other) = delete;

        Rent& operator=(Rent& other) = delete;

        Rent(Rent&& other) noexcept
        {
            m_rendering = std::move(other.m_rendering);
            m_queue = std::move(other.m_queue);
            m_task = std::move(other.m_task);
            other.m_not_moved = false;
        }

        Rent& operator=(Rent&& other) noexcept
        {
            m_rendering = std::move(other.m_rendering);
            m_queue = std::move(other.m_queue);
            m_task = std::move(other.m_task);
            other.m_not_moved = false;
            this->m_not_moved = true;
            return *this;
        }

        ~Rent()
        {
            if (m_not_moved)
            {
                m_queue->ReturnTask(std::move(m_task));
            }
        }

        GpuTask& operator*() const
        {
            return m_task.operator*();
        }

        GpuTask* operator->() const
        {
            return m_task.operator->();
        }
    };

    class GraphicSurface final : public Object<FGraphicSurface>
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

        FGraphicSurfaceData m_data{
            .current_frame_rtv = nullptr,
            .size = {0, 0},
            .format = TextureFormat::R8G8B8A8_UNorm,
        };

        uint2 m_current_size{};
        uint2 m_new_size{};
        DXGI_FORMAT m_format{DXGI_FORMAT_R8G8B8A8_UNORM};
        bool m_resized{false};

    private:
        void re_create_rts();
        void do_re_size();

        explicit GraphicSurface(Rendering* rendering, FWindowHandle* window, void* hwnd, uint2 size);

        void ReadyFrame(ID3D12GraphicsCommandList6* list);
        void EndFrame(ID3D12GraphicsCommandList6* list);
        void Present() const;

    public:
        FError Destroy() noexcept override;

        FError DataPtr(FGraphicSurfaceData** out) noexcept override;

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

    struct ShaderPassGraphicsPipelinePack
    {
        using GraphicsPipelines = HashMap<GraphicsPipelineFormatOverride, Rc<GraphicsShaderPipeline>>;

        ShaderPass* m_pass;
        RwLock<GraphicsPipelines> m_graphics_pipelines{};

        explicit ShaderPassGraphicsPipelinePack(ShaderPass* pass);

        Rc<GraphicsShaderPipeline> GetOrCreateGraphicsPipeline();
        Rc<GraphicsShaderPipeline> GetOrCreateGraphicsPipeline(const GraphicsPipelineFormatOverride& override);
    };

    class ShaderPass final : public Object<FShaderPass>
    {
        IMPL_OBJECT();

    public:
        Rc<Rendering> m_rendering;
        List<char> m_modules[MaxModules];
        FShaderPassData m_data;
        Box<ShaderPassGraphicsPipelinePack> m_graphics_pipelines{};

        explicit ShaderPass(Rc<Rendering>&& rendering, const FShaderPassData* data);

        FError DataPtr(FShaderPassData** out) noexcept override;

        FError GetOrCreateGraphicsPipeline(
            const GraphicsPipelineFormatOverride* override, FGraphicsShaderPipeline** out
        ) noexcept override;
    };

    class GraphicsShaderPipeline final : public Object<FGraphicsShaderPipeline>
    {
        IMPL_OBJECT();

    public:
        Rc<Rendering> m_rendering;
        ComPtr<ID3D12PipelineState> m_pipeline_state{};
        GraphicsPipelineState m_state{};
        bool m_is_mesh_shader{};

        explicit GraphicsShaderPipeline(
            Rc<Rendering>&& rendering, const FShaderPassData* pass, /* opt */
            const GraphicsPipelineFormatOverride* override
        );

        FError RawPtr(void** out) const noexcept override;
        FError StatePtr(const GraphicsPipelineState** out) const noexcept override;
    };
} // cc
