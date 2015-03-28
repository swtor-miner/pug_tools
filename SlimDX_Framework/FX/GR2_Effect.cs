using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace SlimDX_Framework.FX {
    using System.Collections.Generic;

    using SlimDX;
    using SlimDX.Direct3D11;

    public class GR2_Effect : Effect {

        public readonly EffectTechnique Light1Tech;
        public readonly EffectTechnique Light2Tech;
        public readonly EffectTechnique Light3Tech;

        public readonly EffectTechnique Light0TexTech;
        public readonly EffectTechnique Light1TexTech;
        public readonly EffectTechnique Light2TexTech;
        public readonly EffectTechnique Light3TexTech;

        public readonly EffectTechnique Light1Garment;
        public readonly EffectTechnique Light1GarmentMask;
        public readonly EffectTechnique Light1GarmentDiffuse;
        public readonly EffectTechnique Light1GarmentPalette1;
        public readonly EffectTechnique Light1GarmentPalette2;
        public readonly EffectTechnique Light1GarmentPaletteMap;

        public readonly EffectTechnique Light1UberDiffuse;
        public readonly EffectTechnique Light1UberAmbient;
        public readonly EffectTechnique Light1UberEmissive;
        public readonly EffectTechnique Light1UberSpecular;

        public readonly EffectTechnique Light1Skin;
        public readonly EffectTechnique Light1Hair;
        public readonly EffectTechnique Light1Eye;
        public readonly EffectTechnique Light1Facepaint;
        public readonly EffectTechnique Light1Complexion;
        public readonly EffectTechnique Light1Age;        
      
        private readonly EffectMatrixVariable _worldViewProj;
        private readonly EffectMatrixVariable _world;
        private readonly EffectMatrixVariable _worldInvTranspose;
        private readonly EffectMatrixVariable _texTransform;
        private readonly EffectMatrixVariable _shadowTransform;
        private readonly EffectVectorVariable _eyePosW;        

        private readonly EffectVariable _dirLights;
        public const int MaxLights = 3;
        private readonly byte[] _dirLightsArray = new byte[DirectionalLight.Stride*MaxLights];

        private readonly EffectResourceVariable _diffuseMap;
        private readonly EffectResourceVariable _rotationMap;
        private readonly EffectResourceVariable _glossMap;
        private readonly EffectResourceVariable _shadowMap;
        private readonly EffectResourceVariable _cubeMap;
        private readonly EffectResourceVariable _normalMap;
        private readonly EffectResourceVariable _paletteMap;
        private readonly EffectResourceVariable _paletteMaskMap;
        private readonly EffectResourceVariable _complexionMap;
        private readonly EffectResourceVariable _facepaintMap;
        private readonly EffectResourceVariable _ageMap;

        private readonly EffectVectorVariable _palette1;
        private readonly EffectVectorVariable _palette2;
        private readonly EffectVectorVariable _palette1Spec;
        private readonly EffectVectorVariable _palette2Spec;
        private readonly EffectVectorVariable _palette1MetSpec;
        private readonly EffectVectorVariable _palette2MetSpec;

        private readonly EffectVectorVariable _flushTone;
        private readonly EffectVectorVariable _fleshBrightness;

        private readonly EffectVectorVariable _alphaClipValue;
        private readonly EffectVectorVariable _polyIgnore;  
        
        private readonly EffectResourceVariable _ssaoMap;
        private readonly EffectMatrixVariable _worldViewProjTex;


        public GR2_Effect(Device device, string filename) : base(device, filename) {

            Light1Garment = FX.GetTechniqueByName("Light1Garment");
            Light1GarmentMask = FX.GetTechniqueByName("Light1GarmentMask");
            Light1GarmentDiffuse = FX.GetTechniqueByName("Light1GarmentDiffuse");
            Light1GarmentPalette1 = FX.GetTechniqueByName("Light1GarmentPalette1");
            Light1GarmentPalette2 = FX.GetTechniqueByName("Light1GarmentPalette2");
            Light1GarmentPaletteMap = FX.GetTechniqueByName("Light1GarmentPaletteMap");

            Light1UberDiffuse = FX.GetTechniqueByName("Light1UberDiffuse");
            Light1UberAmbient = FX.GetTechniqueByName("Light1UberAmbient");
            Light1UberEmissive = FX.GetTechniqueByName("Light1UberEmissive");
            Light1UberSpecular = FX.GetTechniqueByName("Light1UberSpecular");

            Light1Skin = FX.GetTechniqueByName("Light1Skin");
            Light1Hair = FX.GetTechniqueByName("Light1Hair");
            Light1Eye = FX.GetTechniqueByName("Light1Eye");
            Light1Complexion = FX.GetTechniqueByName("Light1Complexion");
            Light1Facepaint = FX.GetTechniqueByName("Light1Facepaint");
            Light1Age = FX.GetTechniqueByName("Light1Age");

            Light1Tech = FX.GetTechniqueByName("Light1");
            Light2Tech = FX.GetTechniqueByName("Light2");
            //Light3Tech = FX.GetTechniqueByName("Light3");

            Light0TexTech = FX.GetTechniqueByName("Light0Tex");
            Light1TexTech = FX.GetTechniqueByName("Light1Tex");
            Light2TexTech = FX.GetTechniqueByName("Light2Tex");
            //Light3TexTech = FX.GetTechniqueByName("Light3Tex");

            _worldViewProj = FX.GetVariableByName("gWorldViewProj").AsMatrix();
            _world = FX.GetVariableByName("gWorld").AsMatrix();
            _worldInvTranspose = FX.GetVariableByName("gWorldInvTranspose").AsMatrix();
            _texTransform = FX.GetVariableByName("gTexTransform").AsMatrix();
            _eyePosW = FX.GetVariableByName("gEyePosW").AsVector();

            _dirLights = FX.GetVariableByName("gDirLights");            
            _diffuseMap = FX.GetVariableByName("gDiffuseMap").AsResource();
            _rotationMap = FX.GetVariableByName("gRotationMap").AsResource();
            _glossMap = FX.GetVariableByName("gGlossMap").AsResource();
            _shadowMap = FX.GetVariableByName("gShadowMap").AsResource();
            _cubeMap = FX.GetVariableByName("gCubeMap").AsResource();
            _normalMap = FX.GetVariableByName("gNormalMap").AsResource();
            _paletteMap = FX.GetVariableByName("gPaletteMap").AsResource();
            _paletteMaskMap = FX.GetVariableByName("gPaletteMaskMap").AsResource();

            _complexionMap = FX.GetVariableByName("gComplexionMap").AsResource();
            _facepaintMap = FX.GetVariableByName("gFacepaintMap").AsResource();
            _ageMap = FX.GetVariableByName("gAgeMap").AsResource();


            _palette1 = FX.GetVariableByName("gPalette1").AsVector();
            _palette2 = FX.GetVariableByName("gPalette2").AsVector();
            _palette1Spec = FX.GetVariableByName("gPalette1Spec").AsVector();
            _palette2Spec = FX.GetVariableByName("gPalette2Spec").AsVector();
            _palette1MetSpec = FX.GetVariableByName("gPalette1MetSpec").AsVector();
            _palette2MetSpec = FX.GetVariableByName("gPalette2MetSpec").AsVector();

            _flushTone = FX.GetVariableByName("gFlushTone").AsVector();
            _fleshBrightness = FX.GetVariableByName("gFleshBrightness").AsVector();

            _alphaClipValue = FX.GetVariableByName("gAlphaClipValue").AsVector();
            _polyIgnore = FX.GetVariableByName("gPolyIgnore").AsVector();
            

            //_shadowTransform = FX.GetVariableByName("gShadowTransform").AsMatrix();
            //_ssaoMap = FX.GetVariableByName("gSsaoMap").AsResource();

            _worldViewProjTex = FX.GetVariableByName("gWorldViewProjTex").AsMatrix();

        }
        public void SetWorldViewProj(Matrix m) {
            _worldViewProj.SetMatrix(m);
        }
        public void SetWorld(Matrix m) {
            _world.SetMatrix(m);
        }
        public void SetWorldInvTranspose(Matrix m) {
            _worldInvTranspose.SetMatrix(m);
        }
        public void SetEyePosW(Vector3 v) {
            _eyePosW.Set(v);
        }
        public void SetDirLights(DirectionalLight[] lights) {
            System.Diagnostics.Debug.Assert(lights.Length <= MaxLights, "GR2_Effect only supports up to 3 lights");

            for (int i = 0; i < lights.Length && i < MaxLights; i++) {
                var light = lights[i];
                var d = Util.GetArray(light);
                Array.Copy(d, 0, _dirLightsArray, i*DirectionalLight.Stride, DirectionalLight.Stride );
            }

            _dirLights.SetRawValue(new DataStream(_dirLightsArray, false, false), _dirLightsArray.Length);
        }     

        public void SetTexTransform(Matrix m) {
            _texTransform.SetMatrix(m);
        }

        public void SetDiffuseMap(ShaderResourceView tex) {
            _diffuseMap.SetResource(tex);
        }

        public void SetRotationMap(ShaderResourceView tex)
        {
            _rotationMap.SetResource(tex);
        }

        public void SetGlossMap(ShaderResourceView tex)
        {
            _glossMap.SetResource(tex);
        }
     
        public void SetCubeMap(ShaderResourceView tex) {
            _cubeMap.SetResource(tex);
        }

        public void SetNormalMap(ShaderResourceView tex)
        {
            _normalMap.SetResource(tex);
        }

        public void SetPaletteMap(ShaderResourceView tex)
        {
            _paletteMap.SetResource(tex);
        }

        public void SetPaletteMaskMap(ShaderResourceView tex)
        {
            _paletteMaskMap.SetResource(tex);
        }

        public void SetComplexionMap(ShaderResourceView tex)
        {
            _complexionMap.SetResource(tex);
        }

        public void SetFacepaintMap(ShaderResourceView tex)
        {
            _facepaintMap.SetResource(tex);
        }

        public void SetAgeMap(ShaderResourceView tex)
        {
            _ageMap.SetResource(tex);
        }

        public void SetPalette1(Vector4 v)
        {
            _palette1.Set(v);
        }

        public void SetPalette2(Vector4 v)
        {
            _palette2.Set(v);
        }

        public void SetPalette1Spec(Vector4 v)
        {
            _palette1Spec.Set(v);
        }

        public void SetPalette2Spec(Vector4 v)
        {
            _palette2Spec.Set(v);
        }

        public void SetPalette1MetSpec(Vector4 v)
        {
            _palette1Spec.Set(v);
        }

        public void SetPalette2MetSpec(Vector4 v)
        {
            _palette2Spec.Set(v);
        }

        public void SetAlphaClipValue(Vector2 v)
        {
            _alphaClipValue.Set(v);
        }

        public void SetPolyIgnore(Vector2 v)
        {
            _polyIgnore.Set(v);
        }

        public void SetFlushTone(Vector4 v)
        {
            _flushTone.Set(v);
        }

        public void SetFleshBrightness(Vector2 v)
        {
            _fleshBrightness.Set(v);
        }
        
        public void SetWorldViewProjTex(Matrix matrix) {
            if ( _worldViewProjTex != null )_worldViewProjTex.SetMatrix(matrix);
        }
    }
}