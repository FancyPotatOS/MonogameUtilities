#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

// Taken from https://gamedev.stackexchange.com/questions/38118/best-way-to-mask-2d-sprites-in-xna

sampler TextureSampler : register(s0);
Texture2D Mask;

//All of these variables are pixel values
//Feel free to replace with float2 variables
float MaskLocationX;
float MaskLocationY;
float MaskWidth;
float MaskHeight;
float BaseTextureLocationX; //This is where your texture is to be drawn
float BaseTextureLocationY; //texCoord is different, it is the current pixel
float BaseTextureWidth;
float BaseTextureHeight;
float BoundsAlpha;

sampler MaskSampler
{
    Texture = (Mask);
    AddressU = CLAMP;
    AddressV = CLAMP;
};

struct PixelInput
{
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};

float4 MainPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    //We need to calculate where in terms of percentage to sample from the MaskTexture
    float maskPixelX = texCoord.x * BaseTextureWidth + BaseTextureLocationX;
    float maskPixelY = texCoord.y * BaseTextureHeight + BaseTextureLocationY;
    float maskPixelPropX = (maskPixelX - MaskLocationX) / MaskWidth;
    float maskPixelPropY = (maskPixelY - MaskLocationY) / MaskHeight;
    float2 maskCoord = float2(maskPixelPropX, maskPixelPropY);
    
    float4 bitMask = tex2D(MaskSampler, maskCoord.xy);
    
    if (maskPixelPropX > 1 || maskPixelPropX < 0 || maskPixelPropY > 1 || maskPixelPropY < 0)
    {
        bitMask.a = BoundsAlpha;
    }

    float4 tex = tex2D(TextureSampler, texCoord.xy);
    
    return tex * (bitMask.a);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};