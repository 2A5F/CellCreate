#include "CmdEncoder.h"

#include <fmt/format.h>
#include <fmt/xchar.h>

#include "Rendering.h"
#include "../App.h"
#include "../utils/error.h"

using namespace cc;
using namespace cc::gpu;

void cc::cmd_encoder(ID3D12GraphicsCommandList6* list, FGpuStreamCommands& cmds)
{
    GraphicsPipelineFormatOverride cur_rt_format{};

    for (size_t i = 0; i < cmds.count; ++i)
    {
        const auto ptr = cmds.stream[i];
        const auto op = *ptr;
        const auto root_ptr = reinterpret_cast<char*>(ptr) + 1;
        switch (op)
        {
        case FGpuCommandOp::Nop:
            break;
        case FGpuCommandOp::ClearRtv:
            {
                const auto data_ptr = reinterpret_cast<FGpuCommandClearRtv*>(root_ptr);
                const auto rtv = *reinterpret_cast<D3D12_CPU_DESCRIPTOR_HANDLE*>(
                    root_ptr + sizeof(FGpuCommandClearRtv)
                );
                const auto rects_ptr = reinterpret_cast<D3D12_RECT*>(
                    root_ptr + sizeof(FGpuCommandClearRtv) + sizeof(D3D12_CPU_DESCRIPTOR_HANDLE)
                );
                list->ClearRenderTargetView(
                    rtv,
                    &data_ptr->color_a,
                    data_ptr->rects,
                    rects_ptr
                );
                break;
            }
        case FGpuCommandOp::ClearDsv:
            {
                const auto data_ptr = reinterpret_cast<FGpuCommandClearDsv*>(root_ptr);
                const auto dsv = *reinterpret_cast<D3D12_CPU_DESCRIPTOR_HANDLE*>(
                    root_ptr + sizeof(FGpuCommandClearDsv)
                );
                const auto rects_ptr = reinterpret_cast<D3D12_RECT*>(
                    root_ptr + sizeof(FGpuCommandClearDsv) + sizeof(D3D12_CPU_DESCRIPTOR_HANDLE)
                );
                D3D12_CLEAR_FLAGS flags{};
                if (data_ptr->flags.depth) flags |= D3D12_CLEAR_FLAG_DEPTH;
                if (data_ptr->flags.stencil) flags |= D3D12_CLEAR_FLAG_STENCIL;
                list->ClearDepthStencilView(
                    dsv,
                    flags,
                    data_ptr->depth,
                    data_ptr->stencil,
                    data_ptr->rects,
                    rects_ptr
                );
                break;
            }
        case FGpuCommandOp::SetRt:
            {
                const auto data_ptr = reinterpret_cast<FGpuCommandSetRt*>(root_ptr);
                auto lp = root_ptr + sizeof(FGpuCommandSetRt);
                const auto dsv_ptr = data_ptr->has_dsv
                    ? reinterpret_cast<D3D12_CPU_DESCRIPTOR_HANDLE*>(lp)
                    : nullptr;
                const auto rtvs_ptr = reinterpret_cast<D3D12_CPU_DESCRIPTOR_HANDLE*>(
                    lp += data_ptr->has_dsv ? sizeof(D3D12_CPU_DESCRIPTOR_HANDLE) : 0
                );
                list->OMSetRenderTargets(
                    data_ptr->rtv_count,
                    rtvs_ptr,
                    false,
                    dsv_ptr
                );
                cur_rt_format.dsv_format = data_ptr->has_dsv
                    ? *reinterpret_cast<TextureFormat*>(lp)
                    : TextureFormat::Unknown;
                lp += data_ptr->has_dsv ? sizeof(TextureFormat) : 0;
                cur_rt_format.rt_count = data_ptr->rtv_count;
                const auto rtv_ptr = reinterpret_cast<TextureFormat*>(lp);
                for (auto n = 0; n < cur_rt_format.rt_count; ++n)
                {
                    cur_rt_format.rtv_formats[n] = rtv_ptr[n];
                }
                break;
            }
        case FGpuCommandOp::SetViewPort:
            {
                const auto data_ptr = reinterpret_cast<FGpuCommandSetViewPort*>(root_ptr);
                const auto viewport_ptr = reinterpret_cast<D3D12_VIEWPORT*>(
                    root_ptr + sizeof(FGpuCommandSetViewPort)
                );
                list->RSSetViewports(
                    data_ptr->count,
                    viewport_ptr
                );
                break;
            }
        case FGpuCommandOp::SetScissorRect:
            {
                const auto data_ptr = reinterpret_cast<FGpuCommandSetScissorRect*>(root_ptr);
                const auto rect_ptr = reinterpret_cast<D3D12_RECT*>(
                    root_ptr + sizeof(FGpuCommandSetScissorRect)
                );
                list->RSSetScissorRects(
                    data_ptr->count,
                    rect_ptr
                );
                break;
            }
        case FGpuCommandOp::SetShader:
            {
                const auto data_ptr = reinterpret_cast<FGpuCommandSetShader*>(root_ptr);
                const auto pass = static_cast<ShaderPass*>(data_ptr->pass); // NOLINT(*-pro-type-static-cast-downcast)
                if (pass->m_data.stages.cs)
                {
                    // todo cs
                }
                else
                {
                    if (pass->m_data.stages.vs)
                    {
                        switch (pass->m_data.state.primitive_topology)
                        {
                        case PrimitiveTopologyType::TriangleStrip:
                            list->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);
                            break;
                        case PrimitiveTopologyType::Point:
                            list->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY_POINTLIST);
                            break;
                        case PrimitiveTopologyType::Line:
                            list->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY_LINELIST);
                            break;
                        case PrimitiveTopologyType::LineStrip:
                            list->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY_LINESTRIP);
                            break;
                        default:
                            list->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
                            break;
                        }
                    }
                    const auto pipeline = pass->m_graphics_pipelines->GetOrCreateGraphicsPipeline(cur_rt_format);
                    list->SetPipelineState(pipeline->m_pipeline_state.get());
                }
                break;
            }
        case FGpuCommandOp::DrawInstanced:
            {
                const auto data_ptr = reinterpret_cast<FGpuCommandDrawInstanced*>(root_ptr);
                list->DrawInstanced(
                    data_ptr->vertex_count_per_instance,
                    data_ptr->instance_count,
                    data_ptr->start_vertex_location,
                    data_ptr->start_instance_location
                );
                break;
            }
        case FGpuCommandOp::Dispatch:
            {
                const auto data_ptr = reinterpret_cast<FGpuCommandDispatch*>(root_ptr);
                list->Dispatch(
                    data_ptr->thread_group_count_x,
                    data_ptr->thread_group_count_y,
                    data_ptr->thread_group_count_z
                );
                break;
            }
        case FGpuCommandOp::DispatchMesh:
            {
                const auto data_ptr = reinterpret_cast<FGpuCommandDispatch*>(root_ptr);
                list->DispatchMesh(
                    data_ptr->thread_group_count_x,
                    data_ptr->thread_group_count_y,
                    data_ptr->thread_group_count_z
                );
                break;
            }
        default:
            {
                const auto msg = fmt::format(L"Unknown command: {}", static_cast<u8>(op));
                vtb().logger_str16(
                    FLogLevel::Error, FrStr16{reinterpret_cast<const char16_t*>(msg.data()), msg.size()}
                );
                throw CcError("Unknown command");
            }
        }
    }
}
