#include "../bindless.hlsl"
#include "../common.hlsl"

struct UiSceneData
{
    float4x4 view_matrix;
    float4 canvas_size;
};

struct UiRectInstanceData
{
    float4x4 matrix;
    // l t r b
    float4 border;
    // lt rt rb lb
    float4 border_radius;
    float4 border_color;
    float4 bg_color;
    // l t r b
    float4 texture_uv;
    // id texture_type tile_mode _
    uint4 texture;
};

struct Attrs_Rect
{
    uint vid : SV_VertexID;
    uint iid : SV_InstanceID;
};

struct Varying_Rect
{
    float4 posCS : SV_Position;
    nointerpolation uint iid : InstanceID;
};

[shader("vertex")]
Varying_Rect vert_rect(Attrs_Rect input)
{
    ConstantBuffer<UiSceneData> scene = LoadScene();
    StructuredBuffer<UiRectInstanceData> instances = LoadInstances();
    UiRectInstanceData instance = instances.Load(input.iid);

    Varying_Rect output;
    output.iid = input.iid;
    output.posCS = GetQuadVertexPosition(input.vid);
    return output;
}

struct Output_Rect
{
    float4 color : SV_Target;
};

[shader("pixel")]
Output_Rect pixe_rect(Varying_Rect input)
{
    StructuredBuffer<UiRectInstanceData> instances = LoadInstances();
    UiRectInstanceData instance = instances.Load(input.iid);

    Output_Rect output;
    output.color = float4(1, 1, 1, 1);
    return output;
}
