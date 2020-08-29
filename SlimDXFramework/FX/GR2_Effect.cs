namespace SlimDX_Framework.FX
{
    using SlimDX;
    using SlimDX.Direct3D11;

    public class GR2_Effect : Effect
    {
        // BWA Derived Shader Types
        public readonly EffectTechnique AnimatedUV;
        public readonly EffectTechnique AnimatedUVAlphaBlend;
        public readonly EffectTechnique Creature;
        public readonly EffectTechnique Generic;
        public readonly EffectTechnique DiffuseFlat;
        public readonly EffectTechnique EmissiveOnly;
        public readonly EffectTechnique Eye;
        public readonly EffectTechnique Garment;
        public readonly EffectTechnique Glass;
        public readonly EffectTechnique HairC;
        public readonly EffectTechnique HighQualityCharacter;
        public readonly EffectTechnique Ice;
        public readonly EffectTechnique NoShadeTexFogged;
        public readonly EffectTechnique OpacityFade;
        public readonly EffectTechnique SkinB;
        public readonly EffectTechnique SkyDome;
        public readonly EffectTechnique Uber;
        public readonly EffectTechnique UberEnvBlend;
        public readonly EffectTechnique Vegetation;

        // BWA PolyType
        private readonly EffectScalarVariable _polyType;

        // BWA Alpha Mode
        private readonly EffectScalarVariable _alphaMode;

        // BWA Alpha Test
        private readonly EffectScalarVariable _alphaTestValue;

        // Texture Maps
        private readonly EffectResourceVariable _Diffuse;
        private readonly EffectResourceVariable _Rotation;
        private readonly EffectResourceVariable _Gloss;
        private readonly EffectResourceVariable _Palette;
        private readonly EffectResourceVariable _PaletteMask;
        private readonly EffectResourceVariable _Complexion;
        private readonly EffectResourceVariable _Facepaint;
        private readonly EffectResourceVariable _Age;

        // Material Inputs
        private readonly EffectVectorVariable _palette1;
        private readonly EffectVectorVariable _palette2;
        private readonly EffectVectorVariable _palette1Spec;
        private readonly EffectVectorVariable _palette2Spec;
#pragma warning disable IDE1006
        public EffectVectorVariable _palette1MetSpec { get; }
        public EffectVectorVariable _palette2MetSpec { get; }
#pragma warning restore IDE1006

        private readonly EffectVectorVariable _flushTone;
        private readonly EffectScalarVariable _fleshBrightness;

        // Constants
        private readonly EffectMatrixVariable _world;
        private readonly EffectMatrixVariable _worldViewProj;

        // Channel Filters
        public readonly EffectTechnique filterDiffuseMap;
        public readonly EffectTechnique filterPaletteMap;
        public readonly EffectTechnique filterPaletteMask;
        public readonly EffectTechnique filterPalette1;
        public readonly EffectTechnique filterPalette2;
        public readonly EffectTechnique filterSpecular;
        public readonly EffectTechnique filterComplexionMap;
        public readonly EffectTechnique filterFacepaintMap;
        public readonly EffectTechnique filterAgeMap;
        public readonly EffectTechnique filterEmissive;


        public GR2_Effect(Device device, string filename) : base(device, filename)
        {
            // BWA Derived Shader Types
            AnimatedUV = FX.GetTechniqueByName("AnimatedUV");
            AnimatedUVAlphaBlend = FX.GetTechniqueByName("AnimatedUVAlphaBlend");
            Creature = FX.GetTechniqueByName("Creature");
            Generic = FX.GetTechniqueByName("Generic");
            DiffuseFlat = FX.GetTechniqueByName("DiffuseFlat");
            EmissiveOnly = FX.GetTechniqueByName("EmissiveOnly");
            Eye = FX.GetTechniqueByName("Eye");
            Garment = FX.GetTechniqueByName("Garment");
            Glass = FX.GetTechniqueByName("Glass");
            HairC = FX.GetTechniqueByName("HairC");
            HighQualityCharacter = FX.GetTechniqueByName("HighQualityCharacter");
            Ice = FX.GetTechniqueByName("Ice");
            NoShadeTexFogged = FX.GetTechniqueByName("NoShadeTexFogged");
            OpacityFade = FX.GetTechniqueByName("OpacityFade");
            SkinB = FX.GetTechniqueByName("SkinB");
            SkyDome = FX.GetTechniqueByName("SkyDome");
            Uber = FX.GetTechniqueByName("Uber");
            UberEnvBlend = FX.GetTechniqueByName("UberEnvBlend");
            Vegetation = FX.GetTechniqueByName("Vegetation");

            // BWA PolyType
            _polyType = FX.GetVariableByName("gPolyType").AsScalar();

            // BWA Alpha Mode
            _alphaMode = FX.GetVariableByName("AlphaMode").AsScalar();

            // BWA Alpha Test
            _alphaTestValue = FX.GetVariableByName("AlphaTestValue").AsScalar();

            // Texture Maps
            _Diffuse = FX.GetVariableByName("texDiffuse").AsResource();
            _Rotation = FX.GetVariableByName("texRotation").AsResource();
            _Gloss = FX.GetVariableByName("texGloss").AsResource();
            _Palette = FX.GetVariableByName("texPalette").AsResource();
            _PaletteMask = FX.GetVariableByName("texPaletteMask").AsResource();
            _Complexion = FX.GetVariableByName("texComplexion").AsResource();
            _Facepaint = FX.GetVariableByName("texFacepaint").AsResource();
            _Age = FX.GetVariableByName("texAge").AsResource();

            // Material Inputs
            _palette1 = FX.GetVariableByName("Palette1").AsVector();
            _palette2 = FX.GetVariableByName("Palette2").AsVector();
            _palette1Spec = FX.GetVariableByName("Palette1Specular").AsVector();
            _palette2Spec = FX.GetVariableByName("Palette2Specular").AsVector();
            _palette1MetSpec = FX.GetVariableByName("Palette1MetallicSpecular").AsVector();
            _palette2MetSpec = FX.GetVariableByName("Palette1MetallicSpecular").AsVector();

            _flushTone = FX.GetVariableByName("FlushTone").AsVector();
            _fleshBrightness = FX.GetVariableByName("FleshBrightness").AsScalar();

            // Constants
            _world = FX.GetVariableByName("world").AsMatrix();
            _worldViewProj = FX.GetVariableByName("mvMatrix").AsMatrix();

            // Channel Filters
            filterDiffuseMap = FX.GetTechniqueByName("FilterDiffuse");
            filterPaletteMap = FX.GetTechniqueByName("FilterPaletteMap");
            filterPaletteMask = FX.GetTechniqueByName("FilterPaletteMask");
            filterPalette1 = FX.GetTechniqueByName("FilterPalette1");
            filterPalette2 = FX.GetTechniqueByName("FilterPalette2");
            filterSpecular = FX.GetTechniqueByName("FilterSpecular");
            filterComplexionMap = FX.GetTechniqueByName("FilterComplexion");
            filterFacepaintMap = FX.GetTechniqueByName("FilterFacepaint");
            filterAgeMap = FX.GetTechniqueByName("FilterAge");
            filterEmissive = FX.GetTechniqueByName("FilterEmissive");
        }


        // BWA PolyType
        public void SetPolyType(bool v)
        {
            _polyType.Set(v);
        }

        // BWA Alpha Mode
        public void SetAlphaMode(int v)
        {
            _alphaMode.Set(v);
        }

        // BWA Alpha Test
        public void SetAlphaTestValue(float v)
        {
            _alphaTestValue.Set(v);
        }

        // Texture Maps
        public void SetDiffuseMap(ShaderResourceView tex)
        {
            _Diffuse.SetResource(tex);
        }
        public void SetRotationMap(ShaderResourceView tex)
        {
            _Rotation.SetResource(tex);
        }
        public void SetGlossMap(ShaderResourceView tex)
        {
            _Gloss.SetResource(tex);
        }
        public void SetPaletteMap(ShaderResourceView tex)
        {
            _Palette.SetResource(tex);
        }
        public void SetPaletteMaskMap(ShaderResourceView tex)
        {
            _PaletteMask.SetResource(tex);
        }
        public void SetComplexionMap(ShaderResourceView tex)
        {
            _Complexion.SetResource(tex);
        }
        public void SetFacepaintMap(ShaderResourceView tex)
        {
            _Facepaint.SetResource(tex);
        }
        public void SetAgeMap(ShaderResourceView tex)
        {
            _Age.SetResource(tex);
        }

        // Material Inputs
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
        public void SetFlushTone(Vector4 v)
        {
            _flushTone.Set(v);
        }
        public void SetFleshBrightness(float v)
        {
            _fleshBrightness.Set(v);
        }

        // Constants
        public void SetWorld(Matrix m)
        {
            _world.SetMatrix(m);
        }
        public void SetWorldViewProj(Matrix m)
        {
            _worldViewProj.SetMatrix(m);
        }
    }
}