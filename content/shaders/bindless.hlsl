SamplerState _sampler_point_clamp : register(s0, space0);
SamplerState _sampler_point_wrap : register(s1, space0);
SamplerState _sampler_point_mirror : register(s2, space0);

SamplerState _sampler_liner_clamp : register(s3, space0);
SamplerState _sampler_liner_wrap : register(s4, space0);
SamplerState _sampler_liner_mirror : register(s5, space0);

struct BindLessEntry_t
{
    uint root;
    uint scene;
    uint instances;
};

ConstantBuffer<BindLessEntry_t> _bindless_entry : register(b0, space0);

#define LoadResource(ptr) ResourceDescriptorHeap[ptr]
#define LoadSampler(ptr) SamplerDescriptorHeap[ptr]

#define LoadRoot() LoadResource(_bindless_entry.root)
#define LoadScene() LoadResource(_bindless_entry.scene)
#define LoadInstances() LoadResource(_bindless_entry.instances)

struct ResourceRef
{
    uint ptr;
};

struct SamplerRef
{
    uint ptr;

    SamplerState get()
    {
        return LoadSampler(ptr);
    }
};
