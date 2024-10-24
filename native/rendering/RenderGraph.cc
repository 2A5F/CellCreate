#include "RenderGraph.h"

#include "CmdEncoder.h"
#include "../utils/error.h"

using namespace cc;

RenderGraph::RenderGraph(Rc<Rendering>&& rendering) : m_rendering(std::move(rendering))
{

}

FError RenderGraph::ExecuteCommand(gpu::FGpuStreamCommands cmds) noexcept
{
    return ferr_back([&]
    {
        CmdEncoder encoder(m_rendering->m_current_command_list.get());
        encoder.Add(cmds);
    });
}
