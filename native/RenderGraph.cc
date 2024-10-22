#include "RenderGraph.h"

using namespace cc;

RenderGraph::RenderGraph(Rc<Rendering>&& rendering) : m_rendering(std::move(rendering))
{

}
