struct VS_INPUT
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD;
};

struct VS_OUTPUT
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD;
};

VS_OUTPUT main(VS_INPUT input)
{
	return input;
}