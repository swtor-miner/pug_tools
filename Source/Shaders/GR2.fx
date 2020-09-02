// ===================================================================================
// Shader Parameters
//
// Parameters with binding names are used in the material system.  (However, there
// could be some extra ones that aren't really used.
// ===================================================================================

static const float3 lightDirection = float3(0.0f, 0.0f, 1.0f);

cbuffer cbPerObject
{
    float4x4            mvMatrix;
    float4x4            world;

    int                 AlphaMode;
    float               AlphaTestValue;
};

cbuffer cbPerFrame
{
    float4              Palette1;
    float4              Palette2;
    float3              Palette1Specular;
    float3              Palette2Specular;
    float3              Palette1MetallicSpecular;
    float3              Palette2MetallicSpecular;
    float4              FlushTone;
    float               FleshBrightness;
    float               RimWidth;
    float               RimStrength;
    float4              GlassParams;
};

struct Vertex
{
    float3 pos          : POSITION;
    float4 nor          : NORMAL;
    float4 tan          : TANGENT;
    float2 texCo        : TEXCOORD;
};

struct outputVertex
{
    float2 texCo        : TEXCOORD;
    float3 norW         : NORMAL;
    float3 tanW         : TANGENT;
    float3 binW         : BINORMAL;
    float4 pos          : SV_POSITION;
};

SamplerState samLinear
{
    Filter   =          MIN_MAG_MIP_LINEAR;
    AddressU =          WRAP;
    AddressV =          WRAP;
};

// ===================================================================================
// End shader parameters
// ===================================================================================

// Start Common.h.fx functions

// ===================================================================================
// HUEING
// ===================================================================================

//Utility Functions
float3 ExpandHSL(in float3 HSL)
{
    // float3 outputVal = inVal * (maxVal - minVal) + minVal;
    float3 expanded = HSL;
    expanded.r      = (HSL.r * (.706 - .3137)) + .3137; //reexpand hue
    expanded.r     -= .41176; //offset hue
    expanded.g      = (HSL.g * .5882);
    expanded.b      = (HSL.b * .70588);
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
    S       = pow(S, satOffset);
    S      *= (1 - satOffset);
    S       = saturate(S);
    return S;
}

float3 OffsetHSL(in float3 HSL, float hueOffset, float satOffset)
{
    float H = OffsetHue(HSL.x, hueOffset);
    float S = OffsetSaturation(HSL.y, satOffset);
    
    return float3(H, S, HSL.z);
}

float3 ConvertHSLToRGB(in float3 HSL)
{
    const float fOneThird  = 0.333333333f;
    const float fTwoThirds = 0.666666666f;
    float3 color;
    
    float temp1, temp2;
    float H = HSL.r;
    float S = HSL.g;
    float L = HSL.b;
    
    // if (S == 0)
    //     return float3(L, L, L);
     
    float LtimesS = L*S;
    
    if (L<0.5f)
        temp2 = L + LtimesS;
    else
        temp2 = L + S - LtimesS;

    temp1                    = 2.0f*L-temp2;
    
    float3 temp3             = frac(float3(H+fOneThird, H, H-fOneThird));
    
    float3 temp3Times6       = 6.0f * temp3;
    float3 temp3Times2       = 2.0f * temp3;
    float3 temp3Times1point5 = 1.5f * temp3;
    float3 firstEquation     = temp1+(temp2-temp1)*6.0f*temp3;
    float3 secondEquation    = temp1+(temp2-temp1)*(fTwoThirds-temp3)*6.0f;
    
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

float3 ManipulateHSL(in float3 HSL, float4 palette)
{
    HSL = ExpandHSL(HSL);
    HSL = AdjustLightness(HSL, palette.z, palette.w);
    HSL = OffsetHSL(HSL.rgb, palette.x, palette.y);
    return HSL;
}

float OffsetSkinSaturation(in float saturation, float satOffset)
{
    float S = saturation;
    S = saturate(S + (0.5 - satOffset));
    return S;
}

float3 OffsetSkinHSL(in float3 HSL, float hueOffset, float satOffset)
{
    float H = OffsetHue(HSL.x, hueOffset-0.5f);
    float S = OffsetSkinSaturation(HSL.y, satOffset);
    
    return float3(H, S, HSL.z);
}

float3 ManipulateSkinHSL(in float3 HSL, float4 palette)
{
    HSL = AdjustLightness(HSL, palette.z, palette.w);
    HSL = OffsetSkinHSL(HSL.rgb, palette.x, palette.y);
    return HSL;
}

float ManipulateAO(in float ao, float brightness, float contrast)
{
    brightness += 1;
    // brightness + ((1 - brightness) * HSL.b);    
    float ret = ao * (brightness + (1 - brightness) * ao);
    return saturate(ret);
}

void HuePixel(
    in float3  diffuseMapValue,
    in float4  glossMapValue,
    in float4  paletteMaskMapValue,
    in float4  paletteMapValue,
    out float3 fragmentDiffuseColor,
    out float4 fragmentSpecularColor
    )
{
    fragmentDiffuseColor               = diffuseMapValue.rgb;
    fragmentSpecularColor              = glossMapValue;

    float paletteMaskSum               = paletteMaskMapValue.x + paletteMaskMapValue.y;

    // Only use the palette that applies 
    float4 palette;
    float3 chosenSpecColor, chosenMetallicSpecColor;

    if (paletteMaskMapValue.x < paletteMaskMapValue.y)
    {
        palette                        = Palette2;
        chosenSpecColor                = Palette2Specular.rgb;
        chosenMetallicSpecColor        = Palette2MetallicSpecular.rgb;
    }
    else
    {
        palette                        = Palette1;
        chosenSpecColor                = Palette1Specular.rgb;
        chosenMetallicSpecColor        = Palette1MetallicSpecular.rgb;
    }

    // Get the palette map, apply the deltas, and convert it to RGB
    float3 HSL                         = ManipulateHSL(float3(paletteMapValue.g,
                                                              paletteMapValue.b,
                                                              paletteMapValue.a),
                                                              palette);
    float ambientOcclusion             = ManipulateAO(paletteMapValue.r, palette.z, palette.w);
    HSL.z                             *= ambientOcclusion;
    float3 RGB                         = ConvertHSLToRGB(HSL);

    // Blend the result into the original diffuse
    fragmentDiffuseColor               = lerp(diffuseMapValue.rgb, RGB.rgb, paletteMaskSum);

    // Determine the hue specular color
    float3 hueSpecColor;
    const float3 white                 = float3(1,1,1);
    float metallicMask                 = paletteMaskMapValue.b;

    // 1.0 uses chosenMetallicSpecColor, .5 uses white, 0.0 uses chosenSpecColor      
    if( metallicMask > .5 )
        hueSpecColor                   = lerp(white, chosenMetallicSpecColor, (metallicMask - 0.5) * 2.0);
    else
        hueSpecColor                   = lerp(chosenSpecColor, white, metallicMask * 2.0);

    hueSpecColor                      *= glossMapValue.r;
    fragmentSpecularColor.rgb          = lerp(fragmentSpecularColor.rgb, hueSpecColor,
                                              paletteMaskSum);
}

// Should refactor this and HuePixel to reuse code, only one line difference
void HueSkinPixel(
    in float3  diffuseMapValue,
    in float4  glossMapValue,
    in float4  paletteMaskMapValue,
    in float4  paletteMapValue,
    out float3 fragmentDiffuseColor,
    out float4 fragmentSpecularColor
    )
{
    fragmentDiffuseColor               = diffuseMapValue.rgb;
    fragmentSpecularColor              = glossMapValue;

    float paletteMaskSum               = paletteMaskMapValue.x + paletteMaskMapValue.y;
    

    // Only use the palette that applies 
    float4 palette;
    float3 chosenSpecColor, chosenMetallicSpecColor;

    if (paletteMaskMapValue.x < paletteMaskMapValue.y)
    {
        palette                        = Palette2;
        chosenSpecColor                = Palette2Specular.rgb;
        chosenMetallicSpecColor        = Palette2MetallicSpecular.rgb;
    }
    else
    {
        palette                        = Palette1;
        chosenSpecColor                = Palette1Specular.rgb;
        chosenMetallicSpecColor        = Palette1MetallicSpecular.rgb;
    }

    // Get the palette map, apply the deltas, and convert it to RGB
    float3 HSL                         = ManipulateSkinHSL(float3(paletteMapValue.g,
                                                                  paletteMapValue.b,
                                                                  paletteMapValue.a),
                                                                  palette);
    float3 RGB                         = ConvertHSLToRGB(HSL) * paletteMapValue.r;

    // Blend the result into the original diffuse
    fragmentDiffuseColor               = lerp(diffuseMapValue.rgb, RGB.rgb, paletteMaskSum);

    // Determine the hue specular color
    float3 hueSpecColor;
    const float3 white                 = float3(1,1,1);
    float metallicMask                 = paletteMaskMapValue.b;

    // 1.0 uses chosenMetallicSpecColor, .5 uses white, 0.0 uses chosenSpecColor      
    if( metallicMask > .5 )
        hueSpecColor                   = lerp(white, chosenMetallicSpecColor, (metallicMask - 0.5) * 2.0);
    else
        hueSpecColor                   = lerp(chosenSpecColor, white, metallicMask * 2.0);

    hueSpecColor                      *= glossMapValue.r;
    fragmentSpecularColor.rgb          = lerp(fragmentSpecularColor.rgb, hueSpecColor,
                                              paletteMaskSum);
}

// End Common.h.fx functions

// ===================================================================================
// Textures - the sampler definitions remain in the shaders
// ===================================================================================

///////////////////////////////////////////////////////////////////
// Textures

// ==========================================
// Diffuse map
Texture2D<float4> texDiffuse;
// ==========================================

// ==========================================
// Normal rotation map # 1
Texture2D<float4> texRotation;
// ==========================================

// ==========================================
// Gloss map
Texture2D<float4> texGloss;
// ==========================================

// ==========================================
Texture2D<float4> texPalette;
// ==========================================

// ==========================================
Texture2D<float4> texPaletteMask;
// ==========================================

// ==========================================
Texture2D<float4> texComplexion;
// ==========================================

// ==========================================
Texture2D<float4> texFacepaint;
// ==========================================

// ==========================================
Texture2D<float4> texAge;
// ==========================================

///////////////////////////////////////////////////////////////////

//_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-//
//                                           VERTEX PROGRAMS                                                          //
//_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-//

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Vertex Shader - Granny
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

outputVertex vsMain(Vertex In)
{
    outputVertex vout;

    // Object Normals & Tangents
    const float3 o_normal              = (In.nor.xyz * (2.0f / 255)) - 1.0f.xxx;
    const float3 o_tangent             = (In.tan.xyz * (2.0f / 255)) - 1.0f.xxx;

    // Object Position
    const float4 o_pos                 = float4(In.pos, 1.0f);
    
    // Transform to homogeneous clip space.
    vout.pos                           = mul(o_pos, mvMatrix);
    
    // Output vertex attributes for interpolation across triangle.
    vout.texCo                         = In.texCo;
    
    // Transform to world space space.
    vout.norW                          = mul(o_normal, float3x3(world[0].xyz, world[1].xyz, world[2].xyz));
    vout.tanW                          = mul(o_tangent, float3x3(world[0].xyz, world[1].xyz, world[2].xyz));
    vout.binW                          = cross(vout.norW, vout.tanW);

    return vout;
}

//_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-//
//                                           FRAGMENT PROGRAMS                                                        //
//_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-//

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Pixel shader
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

float4 psMain(outputVertex In) : SV_Target
{
    // =========================
    // Diffuse Mapping   
    const float3 diffuseSample         = texDiffuse.Sample(samLinear, In.texCo).xyz;

    // =========================
    // Normal Mapping
    const float4 rotationSample        = texRotation.Sample(samLinear, In.texCo);
    float2 bumpValue                   = (rotationSample.wy * 2.0f) - float2(1.0f, 1.0f);
    
    float emissiveLum                  = rotationSample.z;

    const float3 tangentN              = float3(bumpValue.x, bumpValue.y, sqrt(1.0f - dot(bumpValue, bumpValue)));
    const float alphaValue             = 1.0f - rotationSample.x;
    
    float3x3 toWorld                   = float3x3(float3(In.tanW), float3(In.binW), float3(In.norW));
    float3 N                           = normalize(mul(tangentN, toWorld));
    
    float nDotL                        = max(0.0f, dot(N, lightDirection));

    // =========================
    // Specular Mapping
    const float4 glossSample           = texGloss.Sample(samLinear, In.texCo);

    // =========================
    // Model Lighting
    float3 specColor                   = glossSample.xyz * pow(nDotL, (glossSample.w * 63.0f) + 1.0f);
    float3 oColor                      = ((diffuseSample * (0.4f + (nDotL * 0.6f))) + specColor) + (diffuseSample * emissiveLum);
    float4 color                       = float4(oColor, min(1.0f, alphaValue));
    
    // =========================
    // Alpha Blending
    if (AlphaMode == 1) clip(color.a - AlphaTestValue);
    if (AlphaMode >= 4) clip(color.a - 1.0f);

    float4 fragmentColor = float4(color.xyz, 1.0f);

    return fragmentColor;
}

float4 psEye(outputVertex In) : SV_Target
{
    // =========================
    // Diffuse Mapping   
    const float3 diffuseSample         = texDiffuse.Sample(samLinear, In.texCo).xyz;

    // =========================
    // Normal Mapping
    const float4 rotationSample        = texRotation.Sample(samLinear, In.texCo);
    float2 bumpValue                   = (rotationSample.wy * 2.0f) - float2(1.0f, 1.0f);
    
    float emissiveLum                  = rotationSample.z;

    const float3 tangentN              = float3(bumpValue.x, bumpValue.y, sqrt(1.0f - dot(bumpValue, bumpValue)));
    const float alphaValue             = 1.0f - rotationSample.x;
    
    float3x3 toWorld                   = float3x3(float3(In.tanW), float3(In.binW), float3(In.norW));
    float3 N                           = normalize(mul(tangentN, toWorld));
    
    float nDotL                        = max(0.0f, dot(N, lightDirection));

    // =========================
    // Specular Mapping
    const float4 glossSample           = texGloss.Sample(samLinear, In.texCo);

    // =========================
    // Hueing
    const float4 paletteMapSample      = texPalette.Sample(samLinear, In.texCo);
    const float4 paletteMapMaskSample  = texPaletteMask.Sample(samLinear, In.texCo);

    float3 huedDiffuseColor;
    float4 huedSpecularColor;

    HuePixel(diffuseSample, glossSample, paletteMapMaskSample, paletteMapSample, huedDiffuseColor, huedSpecularColor);

    float3 fragmentDiffuseColor        = huedDiffuseColor;

    // =========================
    // Model Lighting
    float3 specColor                   = huedSpecularColor.xyz * pow(nDotL, (huedSpecularColor.w * 63.0f) + 1.0f);
    float3 oColor                      = ((fragmentDiffuseColor * (0.4f + (nDotL * 0.6f))) + specColor) + (fragmentDiffuseColor * emissiveLum);
    float4 color                       = float4(oColor, min(1.0f, alphaValue));
    
    // =========================
    // Alpha Blending
    if (AlphaMode == 1) clip(color.a - AlphaTestValue);
    if (AlphaMode >= 4) clip(color.a - 1.0f);

    float4 fragmentColor = float4(color.xyz, 1.0f);

    return fragmentColor;
}

float4 psGarment(outputVertex In) : SV_Target
{
    // =========================
    // Diffuse Mapping   
    const float3 diffuseSample         = texDiffuse.Sample(samLinear, In.texCo).xyz;

    // =========================
    // Normal Mapping
    const float4 rotationSample        = texRotation.Sample(samLinear, In.texCo);
    float2 bumpValue                   = (rotationSample.wy * 2.0f) - float2(1.0f, 1.0f);
    
    float emissiveLum                  = rotationSample.z;

    const float3 tangentN              = float3(bumpValue.x, bumpValue.y, sqrt(1.0f - dot(bumpValue, bumpValue)));
    const float alphaValue             = 1.0f - rotationSample.x;
    
    float3x3 toWorld                   = float3x3(float3(In.tanW), float3(In.binW), float3(In.norW));
    float3 N                           = normalize(mul(tangentN, toWorld));
    
    float nDotL                        = max(0.0f, dot(N, lightDirection));

    // =========================
    // Specular Mapping
    const float4 glossSample           = texGloss.Sample(samLinear, In.texCo);

    // =========================
    // Hueing
    const float4 paletteMapSample      = texPalette.Sample(samLinear, In.texCo);
    const float4 paletteMapMaskSample  = texPaletteMask.Sample(samLinear, In.texCo);

    float3 huedDiffuseColor;
    float4 huedSpecularColor;

    HuePixel(diffuseSample, glossSample, paletteMapMaskSample, paletteMapSample, huedDiffuseColor, huedSpecularColor);

    float3 fragmentDiffuseColor        = huedDiffuseColor;

    // =========================
    // Model Lighting
    float3 specColor                   = huedSpecularColor.xyz * pow(nDotL, (huedSpecularColor.w * 63.0f) + 1.0f);
    float3 oColor                      = ((fragmentDiffuseColor * (0.4f + (nDotL * 0.6f))) + specColor) + (fragmentDiffuseColor * emissiveLum);
    float4 color                       = float4(oColor, min(1.0f, alphaValue));
    
    // =========================
    // Alpha Blending
    if (AlphaMode == 1) clip(color.a - AlphaTestValue);
    if (AlphaMode >= 4) clip(color.a - 1.0f);

    float4 fragmentColor               = float4(color.xyz, 1.0f);

    return fragmentColor;
}

float4 psHairC(outputVertex In) : SV_Target
{
    // =========================
    // Diffuse Mapping   
    const float3 diffuseSample         = texDiffuse.Sample(samLinear, In.texCo).xyz;

    // =========================
    // Normal Mapping
    const float4 rotationSample        = texRotation.Sample(samLinear, In.texCo);
    float2 bumpValue                   = (rotationSample.wy * 2.0f) - float2(1.0f, 1.0f);
    
    float emissiveLum                  = rotationSample.z;

    const float3 tangentN              = float3(bumpValue.x, bumpValue.y, sqrt(1.0f - dot(bumpValue, bumpValue)));
    const float alphaValue             = 1.0f - rotationSample.x;
    
    float3x3 toWorld                   = float3x3(float3(In.tanW), float3(In.binW), float3(In.norW));
    float3 N                           = normalize(mul(tangentN, toWorld));
    
    float nDotL                        = max(0.0f, dot(N, lightDirection));

    // =========================
    // Specular Mapping
    const float4 glossSample           = texGloss.Sample(samLinear, In.texCo);

    // =========================
    // Hueing
    const float4 paletteMapSample      = texPalette.Sample(samLinear, In.texCo);
    const float4 paletteMapMaskSample  = texPaletteMask.Sample(samLinear, In.texCo);

    float3 huedDiffuseColor;
    float4 huedSpecularColor;

    HuePixel(diffuseSample, glossSample, paletteMapMaskSample, paletteMapSample, huedDiffuseColor, huedSpecularColor);

    float3 fragmentDiffuseColor        = huedDiffuseColor;

    // =========================
    // Model Lighting
    float3 specColor                   = huedSpecularColor.xyz * pow(nDotL, (huedSpecularColor.w * 63.0f) + 1.0f);
    float3 oColor                      = ((fragmentDiffuseColor * (0.4f + (nDotL * 0.6f))) + specColor) + (fragmentDiffuseColor * emissiveLum);
    float4 color                       = float4(oColor, min(1.0f, alphaValue));
    
    // =========================
    // Alpha Blending
    if (AlphaMode == 1) clip(color.a - AlphaTestValue);
    if (AlphaMode >= 4) clip(color.a - 1.0f);

    float4 fragmentColor = float4(color.xyz, 1.0f);

    return fragmentColor;
}

float4 psSkinB(outputVertex In) : SV_Target
{
    // =========================
    // Diffuse Mapping   
    const float3 diffuseSample         = texDiffuse.Sample(samLinear, In.texCo).xyz;

    // =========================
    // Normal Mapping
    const float4 rotationSample        = texRotation.Sample(samLinear, In.texCo);
    float2 bumpValue                   = (rotationSample.wy * 2.0f) - float2(1.0f, 1.0f);
    
    float emissiveLum                  = rotationSample.z;

    const float3 tangentN              = float3(bumpValue.x, bumpValue.y, sqrt(1.0f - dot(bumpValue, bumpValue)));
    const float alphaValue             = 1.0f - rotationSample.x;
    
    float3x3 toWorld                   = float3x3(float3(In.tanW), float3(In.binW), float3(In.norW));
    float3 N                           = normalize(mul(tangentN, toWorld));
    
    float nDotL                        = max(0.0f, dot(N, lightDirection));

    // =========================
    // Specular Mapping
    const float4 glossSample           = texGloss.Sample(samLinear, In.texCo);

    // =========================
    // Hueing
    const float4 paletteMapSample      = texPalette.Sample(samLinear, In.texCo);
    const float4 paletteMapMaskSample  = texPaletteMask.Sample(samLinear, In.texCo);

    float3 huedDiffuseColor;
    float4 huedSpecularColor;

    HueSkinPixel(diffuseSample, glossSample, paletteMapMaskSample, paletteMapSample, huedDiffuseColor, huedSpecularColor);

    float3 fragmentDiffuseColor        = huedDiffuseColor;

    // Modulate the diffuse map rgb by the complexion map
    const float4 complexionSample      = texComplexion.Sample(samLinear, In.texCo);
    fragmentDiffuseColor              *= complexionSample.rgb;

    // Blend the difuse map with the facepaint map using the fpm alpha as the lerp arg
    const float4 facepaintSample       = texFacepaint.Sample(samLinear, In.texCo);
    fragmentDiffuseColor               = lerp(fragmentDiffuseColor, facepaintSample.rgb, facepaintSample.a);

    // GetFlushColor Function
    float flushFactor                  = (saturate(nDotL - 0.27) * 3) * FleshBrightness;
    float3 flushColor                  = FlushTone.xyz * flushFactor * huedDiffuseColor;
    fragmentDiffuseColor              += flushColor;

    // =========================
    // Model Lighting
    float3 specColor                   = huedSpecularColor.xyz * pow(nDotL, (huedSpecularColor.w * 63.0f) + 1.0f);
    float3 oColor                      = ((fragmentDiffuseColor * (0.4f + (nDotL * 0.6f))) + specColor) + (fragmentDiffuseColor * emissiveLum);
    float4 color                       = float4(oColor, min(1.0f, alphaValue));
    
    // =========================
    // Alpha Blending
    if (AlphaMode == 1) clip(color.a - AlphaTestValue);
    if (AlphaMode >= 4) clip(color.a - 1.0f);

    float4 fragmentColor = float4(color.xyz, 1.0f);

    return fragmentColor;
}

// ================================================================================================
// PugTools Filters
// ================================================================================================

float4 psFilterDiffuse(outputVertex In) : SV_Target
{
    const float4 fragmentColor    = texDiffuse.Sample(samLinear, In.texCo).rgba;

    return fragmentColor;
}

float4 psFilterPaletteMap(outputVertex In) : SV_Target
{
    const float4 fragmentColor    = texPalette.Sample(samLinear, In.texCo).rgba;

    return fragmentColor;
}

float4 psFilterPaletteMask(outputVertex In) : SV_Target
{
    const float4 fragmentColor    = texPaletteMask.Sample(samLinear, In.texCo).rgba;

    return fragmentColor;
}

float4 psFilterPalette1(outputVertex In) : SV_Target
{
    float3 HSL                    = ManipulateHSL(float3(0.97f, 0.74f, 0.73f), Palette1);
    float3 RGB                    = ConvertHSLToRGB(HSL);

    const float4 fragmentColor    = float4(RGB, 1.0f);

    return fragmentColor;
}

float4 psFilterPalette2(outputVertex In) : SV_Target
{
    float3 HSL                    = ManipulateHSL(float3(0.97f, 0.74f, 0.73f), Palette2);
    float3 RGB                    = ConvertHSLToRGB(HSL);

    const float4 fragmentColor    = float4(RGB, 1.0f);

    return fragmentColor;
}

float4 psFilterSpecular(outputVertex In) : SV_Target
{
    const float4 rotationSample   = texRotation.Sample(samLinear, In.texCo);
    float2 bumpValue              = (rotationSample.wy * 2.0f) - float2(1.0f, 1.0f);

    const float3 tangentN         = float3(bumpValue.x, bumpValue.y, sqrt(1.0f - dot(bumpValue, bumpValue)));
    const float alphaValue        = 1.0f - rotationSample.x;

    float3x3 toWorld              = float3x3(float3(In.tanW), float3(In.binW), float3(In.norW));
    float3 N                      = normalize(mul(tangentN, toWorld));
    
    float nDotL                   = max(0.0f, dot(N, lightDirection));

    const float4 glossSample      = texGloss.Sample(samLinear, In.texCo);

    float3 specColor              = glossSample.xyz * pow(nDotL, (glossSample.w * 63.0f) + 1.0f);

    const float4 fragmentColor    = float4(specColor, alphaValue);

    clip(alphaValue - AlphaTestValue);

    return fragmentColor;
}

float4 psFilterComplexion(outputVertex In) : SV_Target
{
    const float4 fragmentColor    = texComplexion.Sample(samLinear, In.texCo).rgba;

    return fragmentColor;
}

float4 psFilterFacepaint(outputVertex In) : SV_Target
{
    const float4 fragmentColor    = texFacepaint.Sample(samLinear, In.texCo).rgba;

    return fragmentColor;
}

float4 psFilterAge(outputVertex In) : SV_Target
{
    const float4 fragmentColor    = texAge.Sample(samLinear, In.texCo).rgba;

    return fragmentColor;
}

float4 psFilterEmissive(outputVertex In) : SV_Target
{
    const float4 diffuseSample    = texDiffuse.Sample(samLinear, In.texCo).rgba;
    const float4 rotationSample   = texRotation.Sample(samLinear, In.texCo);
    float alphaValue              = 1.0f - rotationSample.r;
    
    const float4 fragmentColor    = float4(diffuseSample.rgb * rotationSample.b, alphaValue);

    clip(alphaValue - AlphaTestValue);

    return fragmentColor;
}


//_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-//
//                                           TECHNIQUES                                                               //
//_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-_-"-//

technique11 Generic
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psMain()));
    }
}

technique11 Eye
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psEye()));
    }
}

technique11 Garment
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psGarment()));
    }
}

technique11 HairC
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psHairC()));
    }
}

technique11 SkinB
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psSkinB()));
    }
}

// ================================================================================================
// PugTools Filters
// ================================================================================================

technique11 FilterDiffuse
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psFilterDiffuse()));
    }
}

technique11 FilterPaletteMap
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psFilterPaletteMap()));
    }
}

technique11 FilterPaletteMask
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psFilterPaletteMask()));
    }
}

technique11 FilterPalette1
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psFilterPalette1()));
    }
}

technique11 FilterPalette2
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psFilterPalette2()));
    }
}

technique11 FilterSpecular
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psFilterSpecular()));
    }
}

technique11 FilterComplexion
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psFilterComplexion()));
    }
}

technique11 FilterFacepaint
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psFilterFacepaint()));
    }
}

technique11 FilterAge
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psFilterAge()));
    }
}

technique11 FilterEmissive
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, vsMain()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, psFilterEmissive()));
    }
}
