#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

sampler TextureSampler : register(s0);
Texture2D Mask;
sampler MaskSampler
{
    Texture = (Mask);
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

struct PixelInput
{
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};

float4 MainPS(PixelInput input) : COLOR
{    
    float4 diffuse = tex2D(TextureSampler, input.TexCoord.xy);
    float4 mask = tex2D(MaskSampler, input.TexCoord.xy);

    return diffuse * float4(mask.a, mask.a, mask.a, mask.a);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};