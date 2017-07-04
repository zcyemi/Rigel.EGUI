cbuffer MatrixBuffer
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
};

struct VertexInput
{
	float2 position:POSITION;
	float2 uv:TEXCOORD0;
};

struct PixelInput
{
	float4 position:SV_POSITION;
	float4 color :COLOR;
};


PixelInput main(VertexInput v)
{
	PixelInput o;

	o.position = float4(v.position, 0.5, 1.0);

	//o.position = mul(v.position, worldMatrix);
	//o.position = mul(o.position, viewMatrix);
	//o.position = mul(o.position, projectionMatrix);

	return o;
}