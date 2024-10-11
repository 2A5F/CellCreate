float4 GetQuadVertexPosition(uint vid, float z = 1)
{
    float2 uv = float2((vid << 1) & 2, vid & 2);
    return float4(uv * float2(1, -1) + float2(-1, 1), z, 1);
}
