sampler s0;
texture lightMap;
sampler lightSampler = sampler_state {
	Texture = lightMap;
	MipFilter = Point;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	float4 light = tex2D(lightSampler, coords);
	return color * light;
}

technique Lighting
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
