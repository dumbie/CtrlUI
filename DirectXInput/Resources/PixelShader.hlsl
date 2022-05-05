Texture2D _texture2D : register(t0);
SamplerState _samplerState : register(s0);
cbuffer _shaderVariables : register(b0)
{
	bool HDRtoSDR;
	float HDRBrightness;
	float SDRWhiteLevel;
	float Saturation;
	float RedChannel;
	float GreenChannel;
	float BlueChannel;
	float Brightness;
	float Contrast;
	float Gamma;
};

struct PS_INPUT
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD;
};

float4 AdjustSDRWhiteLevel(float4 color)
{
	if (!HDRtoSDR) { return color; }
	return color / (SDRWhiteLevel / HDRBrightness);
}

float4 AdjustSaturation(float4 color)
{
	if (Saturation == 1.0F) { return color; }
	float colorLuminance = (color.r + color.g + color.b) / 3.0F;
	if (Saturation > 1.0F)
	{
		return max(colorLuminance + (color - colorLuminance) * Saturation, color);
	}
	else
	{
		return colorLuminance + (color - colorLuminance) * Saturation;
	}
}

float4 AdjustColorChannels(float4 color)
{
	if (RedChannel == 1.0F && GreenChannel == 1.0F && BlueChannel == 1.0F) { return color; }
	float4x4 matrix4x4 =
	{
		RedChannel, 0.0, 0.0, 0.0,
		0.0, GreenChannel, 0.0, 0.0,
		0.0, 0.0, BlueChannel, 0.0,
		0.0, 0.0, 0.0, 1.0F
	};
	return mul(matrix4x4, color);
}

float4 AdjustBrightness(float4 color)
{
	if (Brightness == 1.0F) { return color; }
	float4x4 matrix4x4 =
	{
		Brightness, 0.0, 0.0, 0.0,
		0.0, Brightness, 0.0, 0.0,
		0.0, 0.0, Brightness, 0.0,
		0.0, 0.0, 0.0, 1.0F
	};
	return mul(matrix4x4, color);
}

float4 AdjustContrast(float4 color)
{
	if (Contrast == 0.0F) { return color; }
	float4x4 matrix4x4 =
	{
		1.0F, 0.0, 0.0, Contrast,
		0.0, 1.0F, 0.0, Contrast,
		0.0, 0.0, 1.0F, Contrast,
		0.0, 0.0, 0.0, 1.0F
	};
	return mul(matrix4x4, color);
}

float4 AdjustGamma(float4 color)
{
	if (Gamma == 1.0F) { return color; }
	float adjustGamma = Gamma / 1.0F;
	return pow(color, adjustGamma);
}

float4 main(PS_INPUT input) : SV_TARGET
{
	//Get texture colors
	float4 color = _texture2D.Sample(_samplerState, input.TexCoord);

	//Adjust SDR white level
	color = AdjustSDRWhiteLevel(color);

	//Adjust saturation
	color = AdjustSaturation(color);

	//Adjust color channels
	color = AdjustColorChannels(color);

	//Adjust brightness
	color = AdjustBrightness(color);

	//Adjust contrast
	color = AdjustContrast(color);

	//Adjust gamma
	color = AdjustGamma(color);

	//Set alpha channel to maximum
	color.a = 1.0F;

	//Return updated colors
	return color;
}