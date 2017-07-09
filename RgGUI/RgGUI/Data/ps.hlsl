struct PixelInput
{
	float4 position:SV_POSITION;
	float2 uv:TEXCOORD0;
	float4 color :COLOR;
};

Texture2D iconTexture;
SamplerState sampleType;


float4 main(PixelInput i):SV_TARGET
{
	float4 col = iconTexture.Sample(sampleType,i.uv);
	return i.color * col;
}