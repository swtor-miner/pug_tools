//Start LightHelper.fx

struct DirectionalLight
{
	float4 Ambient;
	float4 Diffuse;
	float4 Specular;
	float3 Direction;
	float pad;
};

struct Material
{
	float4 Ambient;
	float4 Diffuse;
	float4 Specular; // w = SpecPower
	float4 Reflect;
};

//---------------------------------------------------------------------------------------
// Computes the ambient, diffuse, and specular terms in the lighting equation
// from a directional light.  We need to output the terms separately because
// later we will modify the individual terms.
//---------------------------------------------------------------------------------------
void ComputeDirectionalLight(Material mat, DirectionalLight L, 
                             float3 normal, float3 toEye,
					         out float4 ambient,
						     out float4 diffuse,
						     out float4 spec)
{
	// Initialize outputs.
	ambient = float4(0.0f, 0.0f, 0.0f, 0.0f);
	diffuse = float4(0.0f, 0.0f, 0.0f, 0.0f);
	spec    = float4(0.0f, 0.0f, 0.0f, 0.0f);

	// The light vector aims opposite the direction the light rays travel.
	float3 lightVec = -L.Direction;

	// Add ambient term.
	ambient = mat.Ambient * L.Ambient;	

	// Add diffuse and specular term, provided the surface is in 
	// the line of site of the light.
	
	float diffuseFactor = dot(lightVec, normal);

	// Flatten to avoid dynamic branching.
	[flatten]
	if( diffuseFactor > 0.0f )
	{
		float3 v         = reflect(-lightVec, normal);
		float specFactor = pow(max(dot(v, toEye), 0.0f), mat.Specular.w);
					
		diffuse = diffuseFactor * mat.Diffuse * L.Diffuse;
		spec    = specFactor * mat.Specular * L.Specular;
	}
}

float3 NormalSampleToWorldSpace(float3 normalMapSample, float3 unitNormalW, float3 tangentW)
{
	// Uncompress each component from [0,1] to [-1,1].
	float3 normalT = 2.0f*normalMapSample - 1.0f;

	// Build orthonormal basis.
	float3 N = unitNormalW;
	float3 T = normalize(tangentW - dot(tangentW, N)*N);
	float3 B = cross(N, T);

	float3x3 TBN = float3x3(T, B, N);

	// Transform from tangent space to world space.
	float3 bumpedNormalW = mul(normalT, TBN);

	return bumpedNormalW;
}
// End LightHelper.fx


//Start Common.h.fx functions
float ManipulateAO(in float ao, float brightness, float contrast)
{
	brightness += 1;	
	float ret = ao * (brightness + (1 - brightness) * ao);
	return saturate(ret);
}

float3 ConvertHSLToRGB(in float3 HSL)
{
	const float fOneThird = 0.333333333f;
	const float fTwoThirds = 0.666666666f;
	float3 color;
	
	float temp1, temp2;
	float H = HSL.r;
	float S = HSL.g;
	float L = HSL.b;
	
	//if (S == 0)
	//	return float3(L, L, L);
	 
	float LtimesS = L*S;
	
	if (L<0.5f)
		temp2 = L + LtimesS;
	else
		temp2 = L + S - LtimesS;

	temp1 = 2.0f*L-temp2;
	
	float3 temp3 = frac(float3(H+fOneThird, H, H-fOneThird));
	
	float3 temp3Times6 = 6.0f * temp3;
	float3 temp3Times2 = 2.0f * temp3;
	float3 temp3Times1point5 = 1.5f * temp3;
	float3 firstEquation = temp1+(temp2-temp1)*6.0f*temp3;
	float3 secondEquation = temp1+(temp2-temp1)*(fTwoThirds-temp3)*6.0f;
	
	if (temp3Times6.r < 1.0f)
		color.r = firstEquation.r;
	else if (temp3Times2.r < 1.0f)
		color.r = temp2;
	else if (temp3Times1point5.r < 1.0f)
		color.r = secondEquation.r;
	else
		color.r = temp1;
		
	if (temp3Times6.g < 1.0f)
		color.g = firstEquation.g;	
	else if (temp3Times2.g < 1.0f)
		color.g = temp2;
	else if (temp3Times1point5.g < 1.0f)
		color.g = secondEquation.g;
	else
		color.g = temp1;	
		
	if (temp3Times6.b < 1.0f)
		color.b = firstEquation.b;
	else if (temp3Times2.b < 1.0f)
		color.b = temp2;
	else if (temp3Times1point5.b < 1.0f)
		color.b = secondEquation.b;
	else
		color.b = temp1;
	
	return color;
}

float3 ExpandHSL(in float3 HSL)
{
	//float3 outputVal = inVal * (maxVal - minVal) + minVal;
	float3 expanded = HSL;
	expanded.r = (HSL.r * (.706 - .3137)) + .3137; //reexpand hue
	expanded.r -= .41176; //offset hue
	expanded.g = (HSL.g * .5882);
	expanded.b = (HSL.b * .70588);
	return expanded;
}

float3 AdjustLightness(in float3 HSL, float brightness, float contrast) //Adjust the lightness of the HSL
{
	HSL.b = pow(HSL.b, contrast) * contrast;
	HSL.b = brightness + ((1 - brightness) * HSL.b);
	return HSL;
}

float OffsetHue(in float hue, float hueOffset)
{
	float H = frac(hue + hueOffset);
	return H;
}

float OffsetSaturation(in float saturation, float satOffset)
{
	float S = saturation;
	S = pow(S, satOffset);
	S = S * (1 - satOffset);
	S = saturate(S);
	return S;
}

float3 OffsetHSL(in float3 HSL, float hueOffset, float satOffset)
{
	float H = OffsetHue(HSL.x, hueOffset);
	float S = OffsetSaturation(HSL.y, satOffset);
	
	return float3(H, S, HSL.z);
}

float3 ManipulateHSL(in float3 HSL, float4 palette)
{
	HSL = ExpandHSL(HSL);
	HSL = AdjustLightness(HSL, palette.z, palette.w);
	HSL = OffsetHSL(HSL.rgb, palette.x, palette.y);
	return HSL;
}
//End Common.h.fx functions

cbuffer cbPerFrame
{
	DirectionalLight gDirLights[3];
	float3 gEyePosW;
	float4 gPalette1;
	float4 gPalette2;
	float4 gPalette1Spec;
	float4 gPalette2Spec;
	float4 gPalette1MetSpec;
	float4 gPalette2MetSpec;
	float4 gFlushTone;
	float2 gFleshBrightness;
};

cbuffer cbPerObject
{
	float4x4 gWorld;
	float4x4 gWorldInvTranspose;
	float4x4 gWorldViewProj;
	float4x4 gWorldViewProjTex;
	float4x4 gTexTransform;
	float4x4 gShadowTransform; 
	Material gMaterial;
	float2 gAlphaClipValue;
	float2 gPolyIgnore;
}; 

struct VS_INPUT
{
	float3 PosL    : POSITION;
	float3 NormalL : NORMAL;
	float2 Tex     : TEXCOORD;
	float3 TangentL : TANGENT;
};


struct VS_OUTPUT
{
	float4 PosH : SV_POSITION;
	float3 PosW       : POSITION;
    float3 NormalW    : NORMAL;
	float3 TangentW : TANGENT;
	float2 Tex        : TEXCOORD;
	float4 Color : COLOR0;
};

Texture2D gDiffuseMap;
Texture2D gRotationMap;
Texture2D gGlossMap;
Texture2D gPaletteMap;
Texture2D gPaletteMaskMap;
Texture2D gComplexionMap;
Texture2D gFacepaintMap;
Texture2D gAgeMap;

SamplerState samLinear
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};


VS_OUTPUT VS( VS_INPUT vin)
{
	VS_OUTPUT vout;
	
	// Transform to world space space.
	vout.PosW    = mul(float4(vin.PosL, 1.0f), gWorld).xyz;
	vout.NormalW = mul(vin.NormalL, (float3x3)gWorldInvTranspose);
	vout.TangentW = mul(vin.TangentL, (float3x3)gWorld);
		
	// Transform to homogeneous clip space.
	vout.PosH = mul(float4(vin.PosL, 1.0f), gWorldViewProj);
	
	// Output vertex attributes for interpolation across triangle.
	vout.Tex = mul(float4(vin.Tex, 0.0f, 1.0f), gTexTransform).xy;

	vout.Color = float4( 1.0f, 1.0f, 0.0f, 1.0f );

	return vout;
}

float4 PS( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);

	// The toEye vector is used in lighting.
	float3 toEye = gEyePosW - pin.PosW;

	// Cache the distance to the eye from this surface point.
	float distToEye = length(toEye);

	// Normalize.
	toEye /= distToEye;
	
	Material gMaterialLoaded;

	float4 diffuseMapValue = gDiffuseMap.Sample( samLinear, pin.Tex ).rgba;
	float4 rotationMapValue = gRotationMap.Sample( samLinear, pin.Tex ).rgba;
	float alpha = 1.0f - rotationMapValue.r;
	float4 outputColor = float4(diffuseMapValue.rgb, alpha);
		
	//Disable lighting code for now

	/*
	//float4 rotationMapValue = gRotationMap.Sample( samLinear, pin.Tex ).rgba;
	float4 glossMapValue = gGlossMap.Sample( samLinear, pin.Tex ).rgba;		
	gMaterialLoaded.Diffuse = float4(diffuseMapValue.rgba);
	gMaterialLoaded.Ambient = float4(diffuseMapValue.rgba);
	gMaterialLoaded.Specular = float4(glossMapValue.rgba);

	float2 normalMapSample = gRotationMap.Sample(samLinear, pin.Tex).ag * 2 - 1;
	float3 bumpedNormalW = float3(normalMapSample.x, normalMapSample.y, sqrt(1 - dot(normalMapSample.xy, normalMapSample.xy)));

	float4 ambient = float4(0.0f, 0.0f, 0.0f, 0.0f);
	float4 diffuse = float4(0.0f, 0.0f, 0.0f, 0.0f);
	float4 spec    = float4(0.0f, 0.0f, 0.0f, 0.0f);			
	
	float4 litColor = float4(0,0,0,0);

	[unroll]	
	for(int i = 0; i < gLightCount; ++i)
	{
		float4 A, D, S;
		ComputeDirectionalLight(gMaterialLoaded, gDirLights[i], pin.NormalW, toEye, A, D, S);
		ambient += A;    
		diffuse += D;
		spec    += S;
	}
	
	litColor = ambient + diffuse + spec;
	litColor.a = 1.0f - rotationMapValue.r;	
	
	return litColor;
	*/
	return outputColor;
}


float4 PS_Uber_Deffuse( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);
	
	float3 toEye = gEyePosW - pin.PosW;
	float3 lightVec = -gDirLights[0].Direction;	
		
	Material gMaterialLoaded;

	float4 diffuseMapValue = gDiffuseMap.Sample( samLinear, pin.Tex ).rgba;
	float4 rotationMapValue = gRotationMap.Sample( samLinear, pin.Tex ).rgba;
	float alpha = 1.0f - rotationMapValue.r;
	clip(alpha);	

	float2 bumpVal = rotationMapValue.ag * 2 - 1;
	float3 bumpNormal = float3(bumpVal.x, bumpVal.y, sqrt(1 - dot(bumpVal.xy, bumpVal.xy)));	

	float3 diffuse = saturate(dot(bumpNormal, lightVec)) * diffuseMapValue.rgb;
	return float4(diffuse, alpha);
}

float4 PS_Uber_Ambient( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);
	
	float3 toEye = gEyePosW - pin.PosW;
	float3 lightVec = -gDirLights[0].Direction;	
		
	Material gMaterialLoaded;

	float4 diffuseMapValue = gDiffuseMap.Sample( samLinear, pin.Tex ).rgba;
	float4 rotationMapValue = gRotationMap.Sample( samLinear, pin.Tex ).rgba;
	float alpha = 1.0f - rotationMapValue.r;
	clip(alpha);
	
	float3 ambient = diffuseMapValue.rgb;
	
	return float4(ambient, alpha);
}

float4 PS_Uber_Emissive( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);
	
	float3 toEye = gEyePosW - pin.PosW;
	float3 lightVec = -gDirLights[0].Direction;
		
	Material gMaterialLoaded;

	float4 diffuseMapValue = gDiffuseMap.Sample( samLinear, pin.Tex ).rgba;
	float4 rotationMapValue = gRotationMap.Sample( samLinear, pin.Tex ).rgba;
	float alpha = 1.0f - rotationMapValue.r;
	clip(alpha);
	
	float3 emissive = diffuseMapValue.rgb * rotationMapValue.b;
	
	return float4(emissive, alpha);
}


float4 PS_Uber_Specular( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);
	
	float3 toEye = gEyePosW - pin.PosW;
	float3 lightVec = -gDirLights[0].Direction;
	float3 halfway = normalize(lightVec + toEye);
		
	Material gMaterialLoaded;
	
	float4 rotationMapValue = gRotationMap.Sample( samLinear, pin.Tex ).rgba;
	float alpha = 1.0f - rotationMapValue.r;
	clip(alpha);
	float4 glossMapValue = gGlossMap.Sample( samLinear, pin.Tex ).rgba;	

	float2 bumpVal = rotationMapValue.ag * 2 - 1;
	float3 bumpNormal = float3(bumpVal.x, bumpVal.y, sqrt(1 - dot(bumpVal.xy, bumpVal.xy)));
	
	float3 specColor = glossMapValue.rgb;
	float specPower = glossMapValue.a;	
	float3 specular = pow(saturate(dot(bumpNormal, halfway)), specPower) * specColor;
	return float4(specular, alpha);
}


float4 PS_Garment( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);

	// The toEye vector is used in lighting.
	float3 toEye = gEyePosW - pin.PosW;

	// Cache the distance to the eye from this surface point.
	float distToEye = length(toEye);

	// Normalize.
	toEye /= distToEye;

	float4 diffuseMapValue = gDiffuseMap.Sample( samLinear, pin.Tex ).rgba;
	float4 rotationMapValue = gRotationMap.Sample( samLinear, pin.Tex ).rgba;
	float alpha = 1.0f - rotationMapValue.r;
	clip(alpha - gAlphaClipValue.x);	
	float4 specularMapValue = gGlossMap.Sample( samLinear, pin.Tex ).rgba;

	float4 paletteMapValue = gPaletteMap.Sample( samLinear, pin.Tex ).rgba;
	float4 paletteMaskMapValue = gPaletteMaskMap.Sample( samLinear, pin.Tex ).rgba;	

	float3 diffuseColor = diffuseMapValue.rgb;
	float4 specularColor = specularMapValue;

	float paletteMaskSum = paletteMaskMapValue.x + paletteMaskMapValue.y;

	float4 cPalette;
	float3 cSpecColor, cMetSpecColor;


	if(gPolyIgnore.x == 1.0 && gAlphaClipValue.x > 0)
	{	
		if(paletteMaskMapValue.a < 1.0)
		{
			alpha = 0.0f + paletteMaskMapValue.a;
		}
	}	

	//Black
	if(paletteMaskMapValue.r < .5 && paletteMaskMapValue.g < .5 && paletteMaskMapValue.b < .5)
	{
		return float4(diffuseColor, alpha);
	}

	if(paletteMaskMapValue.x < paletteMaskMapValue.y)
	{
		cPalette = gPalette2;
		cSpecColor = gPalette2Spec.rgb;
		cMetSpecColor = gPalette2MetSpec.rgb;
	}else{
		cPalette = gPalette1;
		cSpecColor = gPalette1Spec.rgb;
		cMetSpecColor = gPalette1MetSpec.rgb;
	}

	float3 HSL = ManipulateHSL(float3(paletteMapValue.g, paletteMapValue.b, paletteMapValue.a), cPalette);
	float ambientOcclusion = ManipulateAO(paletteMapValue.r, cPalette.z, cPalette.w);
	HSL.z *= ambientOcclusion;
	float3 RGB = ConvertHSLToRGB(HSL);

	diffuseColor = lerp(diffuseMapValue.rgb, RGB.rgb, paletteMaskSum);	

	//Red
	if(paletteMaskMapValue.r > .5 && paletteMaskMapValue.g < .5 && paletteMaskMapValue.b < .5)
	{
		return float4(diffuseColor, alpha);
	}

	//Green
	if(paletteMaskMapValue.r < .5 && paletteMaskMapValue.g > .5 && paletteMaskMapValue.b < .5)
	{
		return float4(diffuseColor, alpha);
	}


	// Determine the hue specular color
	float3 hueSpecColor;
	const float3 white = float3(1,1,1);
	float metallicMask = paletteMaskMapValue.b;
	//1.0 uses cMetSpecColor, .5 uses white, 0.0 uses cSpecColor      
	if( metallicMask > .5 )
	{
		hueSpecColor = lerp(white, cMetSpecColor, (metallicMask - 0.5) * 2);
	}
	else
	{
		hueSpecColor = lerp(cSpecColor, white, metallicMask * 2);
	}
	hueSpecColor *= specularMapValue.r;
	specularColor.rgb = lerp(specularColor.rgb, hueSpecColor, paletteMaskSum);

	//Blue
	if(paletteMaskMapValue.r < .5 && paletteMaskMapValue.g < .5 && paletteMaskMapValue.b > .5)
	{
		return float4(specularColor.rgb, alpha);
	}

	//Purple
	if(paletteMaskMapValue.r > .5 && paletteMaskMapValue.g < .5 && paletteMaskMapValue.b > .5)
	{
		float3 finalColor = diffuseColor.rgb + specularColor.rgb;
		return float4(finalColor, alpha);
	}

	//Yellow
	if(paletteMaskMapValue.r < .5 && paletteMaskMapValue.g > .5 && paletteMaskMapValue.b > .5)
	{
		float3 finalColor = diffuseColor.rgb + specularColor.rgb;
		return float4(finalColor, alpha);
	}

	float3 finalColor = diffuseColor.rgb + specularColor.rgb;
	return float4(finalColor, alpha);
}

float4 PS_Hair( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);

	// The toEye vector is used in lighting.
	float3 toEye = gEyePosW - pin.PosW;

	// Cache the distance to the eye from this surface point.
	float distToEye = length(toEye);

	// Normalize.
	toEye /= distToEye;

	float4 diffuseMapValue = gDiffuseMap.Sample( samLinear, pin.Tex ).rgba;
	float4 rotationMapValue = gRotationMap.Sample( samLinear, pin.Tex ).rgba;
	float alpha = 1.0f - rotationMapValue.r;
	clip(alpha - gAlphaClipValue.x);	
	float4 specularMapValue = gGlossMap.Sample( samLinear, pin.Tex ).rgba;

	float4 paletteMapValue = gPaletteMap.Sample( samLinear, pin.Tex ).rgba;
	float4 paletteMaskMapValue = gPaletteMaskMap.Sample( samLinear, pin.Tex ).rgba;	

	float3 diffuseColor = diffuseMapValue.rgb;
	float4 specularColor = specularMapValue;

	float paletteMaskSum = paletteMaskMapValue.x + paletteMaskMapValue.y;

	float4 cPalette;
	float3 cSpecColor, cMetSpecColor;

	if(paletteMaskMapValue.x < paletteMaskMapValue.y)
	{
		cPalette = gPalette2;
		cSpecColor = gPalette2Spec.rgb;
		cMetSpecColor = gPalette2MetSpec.rgb;
	}else{
		cPalette = gPalette1;
		cSpecColor = gPalette1Spec.rgb;
		cMetSpecColor = gPalette1MetSpec.rgb;
	}

	float3 HSL = ManipulateHSL(float3(paletteMapValue.g, paletteMapValue.b, paletteMapValue.a), cPalette);
	float ambientOcclusion = ManipulateAO(paletteMapValue.r, cPalette.z, cPalette.w);
	HSL.z *= ambientOcclusion;
	float3 RGB = ConvertHSLToRGB(HSL);

	diffuseColor = lerp(diffuseMapValue.rgb, RGB.rgb, paletteMaskSum);		

	// Determine the hue specular color
	float3 hueSpecColor;
	const float3 white = float3(1,1,1);
	float metallicMask = paletteMaskMapValue.b;
	//1.0 uses cMetSpecColor, .5 uses white, 0.0 uses cSpecColor      
	if( metallicMask > .5 )
	{
		hueSpecColor = lerp(white, cMetSpecColor, (metallicMask - 0.5) * 2);
	}
	else
	{
		hueSpecColor = lerp(cSpecColor, white, metallicMask * 2);
	}
	hueSpecColor *= specularMapValue.r;
	specularColor.rgb = lerp(specularColor.rgb, hueSpecColor, paletteMaskSum);

	float3 finalColor = diffuseColor.rgb; // + specularColor.rgb;
	return float4(diffuseColor, alpha);
}

float4 PS_Skin( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);

	// The toEye vector is used in lighting.
	float3 toEye = gEyePosW - pin.PosW;

	// Cache the distance to the eye from this surface point.
	float distToEye = length(toEye);

	// Normalize.
	toEye /= distToEye;

	float4 diffuseMapValue = gDiffuseMap.Sample( samLinear, pin.Tex ).rgba;
	float4 rotationMapValue = gRotationMap.Sample( samLinear, pin.Tex ).rgba;	
	float4 specularMapValue = gGlossMap.Sample( samLinear, pin.Tex ).rgba;
	//float alpha = diffuseMapValue.a;
	//clip(alpha);

	float4 paletteMapValue = gPaletteMap.Sample( samLinear, pin.Tex ).rgba;
	float4 paletteMaskMapValue = gPaletteMaskMap.Sample( samLinear, pin.Tex ).rgba;	

	float3 diffuseColor = diffuseMapValue.rgb;
	float4 specularColor = specularMapValue;
	
	float paletteMaskSum = paletteMaskMapValue.x + paletteMaskMapValue.y;

	float4 cPalette;
	float3 cSpecColor, cMetSpecColor;

	if(paletteMaskMapValue.x < paletteMaskMapValue.y)
	{
		cPalette = gPalette2;
		cSpecColor = gPalette2Spec.rgb;
		cMetSpecColor = gPalette2MetSpec.rgb;
	}else{
		cPalette = gPalette1;
		cSpecColor = gPalette1Spec.rgb;
		cMetSpecColor = gPalette1MetSpec.rgb;
	}

	float3 HSL = ManipulateHSL(float3(paletteMapValue.g, paletteMapValue.b, paletteMapValue.a), cPalette);
	float ambientOcclusion = ManipulateAO(paletteMapValue.r, cPalette.z, cPalette.w);
	HSL.z *= ambientOcclusion;
	float3 RGB = ConvertHSLToRGB(HSL);

	diffuseColor = lerp(diffuseMapValue.rgb, RGB.rgb, paletteMaskSum);	

	// Determine the hue specular color
	float3 hueSpecColor;
	const float3 white = float3(1,1,1);
	float metallicMask = paletteMaskMapValue.b;
	//1.0 uses cMetSpecColor, .5 uses white, 0.0 uses cSpecColor      
	if( metallicMask > .5 )
	{
		hueSpecColor = lerp(white, cMetSpecColor, (metallicMask - 0.5) * 2);
	}
	else
	{
		hueSpecColor = lerp(cSpecColor, white, metallicMask * 2);
	}
	hueSpecColor *= specularMapValue.r;
	specularColor.rgb = lerp(specularColor.rgb, hueSpecColor, paletteMaskSum);

	//float4 ageMapValue = gAgeMap.Sample( samLinear, pin.Tex ).rgba;
	//float3 ageDarkening = lerp(gFlushTone.rgb, white, ageMapValue.b);

	float3 complexionMapValue = gComplexionMap.Sample( samLinear, pin.Tex ).rgb;
	float4 facepaintMapValue = gFacepaintMap.Sample( samLinear, pin.Tex ).rgba;

	float3 fragmentDiffuseColor = diffuseColor;
	fragmentDiffuseColor = fragmentDiffuseColor * complexionMapValue; // * ageDarkening);
	fragmentDiffuseColor.rgb = lerp(fragmentDiffuseColor.rgb, facepaintMapValue.rgb, facepaintMapValue.a);
	
	float flushFactor = (saturate(2.0f - 0.27f) * 3) * gFleshBrightness.x; //2.0f should be nDotL
	float3 flushFrag = gFlushTone.rgb * flushFactor * diffuseColor;

	fragmentDiffuseColor.rgb = fragmentDiffuseColor.rgb + flushFrag.rgb;

	float3 finalColor = fragmentDiffuseColor.rgb; // + specularColor.rgb;
	return float4(finalColor, 1.0f);
}

float4 PS_Eye( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);

	// The toEye vector is used in lighting.
	float3 toEye = gEyePosW - pin.PosW;

	// Cache the distance to the eye from this surface point.
	float distToEye = length(toEye);

	// Normalize.
	toEye /= distToEye;

	float4 diffuseMapValue = gDiffuseMap.Sample( samLinear, pin.Tex ).rgba;
	float4 rotationMapValue = gRotationMap.Sample( samLinear, pin.Tex ).rgba;	
	float4 specularMapValue = gGlossMap.Sample( samLinear, pin.Tex ).rgba;
	float alpha = 1.0f - rotationMapValue.r;
	clip(alpha - gAlphaClipValue.x);	

	float4 paletteMapValue = gPaletteMap.Sample( samLinear, pin.Tex ).rgba;
	float4 paletteMaskMapValue = gPaletteMaskMap.Sample( samLinear, pin.Tex ).rgba;	

	float3 diffuseColor = diffuseMapValue.rgb;
	float4 specularColor = specularMapValue;
	
	float paletteMaskSum = paletteMaskMapValue.x + paletteMaskMapValue.y;

	float4 cPalette;
	float3 cSpecColor, cMetSpecColor;

	if(paletteMaskMapValue.x < paletteMaskMapValue.y)
	{
		cPalette = gPalette2;
		cSpecColor = gPalette2Spec.rgb;
		cMetSpecColor = gPalette2MetSpec.rgb;
	}else{
		cPalette = gPalette1;
		cSpecColor = gPalette1Spec.rgb;
		cMetSpecColor = gPalette1MetSpec.rgb;
	}

	float3 HSL = ManipulateHSL(float3(paletteMapValue.g, paletteMapValue.b, paletteMapValue.a), cPalette);
	float ambientOcclusion = ManipulateAO(paletteMapValue.r, cPalette.z, cPalette.w);
	HSL.z *= ambientOcclusion;
	float3 RGB = ConvertHSLToRGB(HSL);

	diffuseColor = lerp(diffuseMapValue.rgb, RGB.rgb, paletteMaskSum);	

	// Determine the hue specular color
	float3 hueSpecColor;
	const float3 white = float3(1,1,1);
	float metallicMask = paletteMaskMapValue.b;
	//1.0 uses cMetSpecColor, .5 uses white, 0.0 uses cSpecColor      
	if( metallicMask > .5 )
	{
		hueSpecColor = lerp(white, cMetSpecColor, (metallicMask - 0.5) * 2);
	}
	else
	{
		hueSpecColor = lerp(cSpecColor, white, metallicMask * 2);
	}
	hueSpecColor *= specularMapValue.r;
	specularColor.rgb = lerp(specularColor.rgb, hueSpecColor, paletteMaskSum);	

	float3 finalColor = diffuseColor.rgb; // + specularColor.rgb;
	return float4(finalColor, alpha);
}

float4 PS_Skin_Complexion( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);
	float3 complexionMapValue = gComplexionMap.Sample( samLinear, pin.Tex ).rgb;
	
	return float4(complexionMapValue, 1.0f);
}

float4 PS_Skin_Facepaint( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);
	float3 facepaintMapValue = gFacepaintMap.Sample( samLinear, pin.Tex ).rgb;
	
	return float4(facepaintMapValue, 1.0f);
}

float4 PS_Skin_Age( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);
	float3 ageMapValue = gAgeMap.Sample( samLinear, pin.Tex ).rgb;
	
	return float4(ageMapValue, 1.0f);
}

float4 PS_Garment_Mask( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);

	// The toEye vector is used in lighting.
	float3 toEye = gEyePosW - pin.PosW;

	// Cache the distance to the eye from this surface point.
	float distToEye = length(toEye);

	// Normalize.
	toEye /= distToEye;	
	float4 paletteMaskMapValue = gPaletteMaskMap.Sample( samLinear, pin.Tex ).rgba;	
	return paletteMaskMapValue;	
}

float4 PS_Garment_Diffuse( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);

	// The toEye vector is used in lighting.
	float3 toEye = gEyePosW - pin.PosW;

	// Cache the distance to the eye from this surface point.
	float distToEye = length(toEye);

	// Normalize.
	toEye /= distToEye;	
	float4 diffuseMapValue = gDiffuseMap.Sample( samLinear, pin.Tex ).rgba;
	return diffuseMapValue;	
}

float4 PS_Garment_Palette1( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);

	// The toEye vector is used in lighting.
	float3 toEye = gEyePosW - pin.PosW;

	// Cache the distance to the eye from this surface point.
	float distToEye = length(toEye);

	// Normalize.
	toEye /= distToEye;	

	float4 cPalette = gPalette1;
	float3 cSpecColor = gPalette1Spec.rgb;
	float3 cMetSpecColor = gPalette1MetSpec.rgb;	

	return float4(cPalette.rgb, 1.0f);
}

float4 PS_Garment_Palette2( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);

	// The toEye vector is used in lighting.
	float3 toEye = gEyePosW - pin.PosW;

	// Cache the distance to the eye from this surface point.
	float distToEye = length(toEye);

	// Normalize.
	toEye /= distToEye;	
		
	float4 cPalette = gPalette2;
	float3 cSpecColor = gPalette2Spec.rgb;
	float3 cMetSpecColor = gPalette2MetSpec.rgb;	

	return float4(cPalette.rgb, 1.0f);
}

float4 PS_Garment_PaletteMap( VS_OUTPUT pin, uniform int gLightCount ) : SV_Target
{
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);

	// The toEye vector is used in lighting.
	float3 toEye = gEyePosW - pin.PosW;

	// Cache the distance to the eye from this surface point.
	float distToEye = length(toEye);

	// Normalize.
	toEye /= distToEye;	
	float4 paletteMapValue = gPaletteMap.Sample( samLinear, pin.Tex ).rgba;	
	return paletteMapValue;	
}

technique11 Light1
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS(1 ) ) );
	}
}

technique11 Light2
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS(2 ) ) );
	}
}

technique11 Light1Garment
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Garment(1 ) ) );
	}
}


technique11 Light1GarmentMask
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Garment_Mask(1 ) ) );
	}
}

technique11 Light1GarmentDiffuse
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Garment_Diffuse(1 ) ) );
	}
}

technique11 Light1GarmentPalette1
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Garment_Palette1(1 ) ) );
	}
}
technique11 Light1GarmentPalette2
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Garment_Palette2(1 ) ) );
	}
}

technique11 Light1GarmentPaletteMap
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Garment_PaletteMap(1 ) ) );
	}
}


technique11 Light1UberDiffuse
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Uber_Deffuse(1 ) ) );
	}
}

technique11 Light1UberAmbient
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Uber_Ambient(1 ) ) );
	}
}

technique11 Light1UberEmissive
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Uber_Emissive(1 ) ) );
	}
}

technique11 Light1UberSpecular
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Uber_Specular(1 ) ) );
	}
}


technique11 Light1Skin
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Skin(1 ) ) );
	}
}


technique11 Light1Eye
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Eye(1 ) ) );
	}
}


technique11 Light1Hair
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Hair(1 ) ) );
	}
}

technique11 Light1Complexion
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Skin_Complexion(1 ) ) );
	}
}

technique11 Light1Facepaint
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Skin_Facepaint(1 ) ) );
	}
}

technique11 Light1Age
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS( ) ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_5_0, PS_Skin_Age(1 ) ) );
	}
}

