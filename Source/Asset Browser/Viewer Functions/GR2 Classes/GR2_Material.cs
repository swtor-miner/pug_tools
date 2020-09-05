using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SlimDX;
using SlimDX.Direct3D11;
using ShaderResourceView = SlimDX.Direct3D11.ShaderResourceView;
using TorLib;
using GomLib;

namespace FileFormats
{
    public class GR2_Material
    {
        public string ageDDS;
        public ShaderResourceView ageSRV;
        public bool alphaClip;
        public float alphaTestValue;
        public string alphaMode = "";
        public string complexionDDS;
        public ShaderResourceView complexionSRV;
        public string derived;
        public string diffuseDDS;
        public ShaderResourceView diffuseSRV;
        public string facepaintDDS;
        public ShaderResourceView facepaintSRV;
        public Vector4 flushTone;
        public float fleshBrightness;
        public Vector4 glassParams;
        public string glossDDS;
        public ShaderResourceView glossSRV;
        public bool isTwoSided;
        public string materialName = "";
        public uint offsetMaterialName = 0;
        public string paletteDDS;
        public ShaderResourceView paletteSRV;
        public string paletteMaskDDS;
        public ShaderResourceView paletteMaskSRV;
        public Vector4 palette1;
        public Vector4 palette2;
        public Vector4 palette1MetSpec;
        public Vector4 palette1Spec;
        public string palette1XML;
        public Vector4 palette2MetSpec;
        public Vector4 palette2Spec;
        public string palette2XML;
        public bool parsed;
        public string polytype;
        public string rotationDDS;
        public ShaderResourceView rotationSRV;
        public bool useEmissive;
        public bool useReflection;


        public GR2_Material(string materialName)
        {
            this.materialName = materialName;
        }

        public GR2_Material(BinaryReader br)
        {
            offsetMaterialName = br.ReadUInt32();
            materialName = File_Helpers.ReadString(br, offsetMaterialName);
        }

        public void ParseMAT(Device device, List<GR2_Material> parentMaterials = null)
        {
            string materialFileName = "/resources/art/shaders/materials/" + materialName + ".mat";
            // _ = FileId.FromFilePath(materialFileName);
            Assets currentAssets = AssetHandler.Instance.GetCurrentAssets();
            try
            {
                if (palette1XML != null)
                {
                    var palette1File = currentAssets.FindFile(palette1XML);
                    if (palette1File != null)
                    {
                        var palette1Stream = palette1File.OpenCopyInMemory();
                        XmlDocument palette1XML_Doc = new XmlDocument();
                        palette1XML_Doc.Load(palette1Stream);

                        Vector4 metSpec = File_Helpers.StringToVec4(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Metallicspecular").InnerText);
                        Vector4 spec = File_Helpers.StringToVec4(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Specular").InnerText);
                        float hue = float.Parse(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Hue").InnerText);
                        float bright = float.Parse(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Brightness").InnerText);
                        float saturation = float.Parse(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Saturation").InnerText);
                        float contrast = float.Parse(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Contrast").InnerText);
                        palette1 = new Vector4(hue, saturation, bright, contrast);
                        palette1MetSpec = metSpec;
                        palette1Spec = spec;
                    }
                }

                if (palette2XML != null)
                {
                    var palette2File = currentAssets.FindFile(palette2XML);
                    if (palette2File != null)
                    {
                        var palette2Stream = palette2File.OpenCopyInMemory();
                        XmlDocument palette2XML_Doc = new XmlDocument();
                        palette2XML_Doc.Load(palette2Stream);

                        Vector4 metSpec = File_Helpers.StringToVec4(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Metallicspecular").InnerText);
                        Vector4 spec = File_Helpers.StringToVec4(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Specular").InnerText);
                        float hue = float.Parse(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Hue").InnerText);
                        float bright = float.Parse(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Brightness").InnerText);
                        float saturation = float.Parse(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Saturation").InnerText);
                        float contrast = float.Parse(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Contrast").InnerText);
                        palette2 = new Vector4(hue, saturation, bright, contrast);
                        palette2MetSpec = metSpec;
                        palette2Spec = spec;

                    }
                }
            }
            catch (Exception) { }
            var materialFile = currentAssets.FindFile(materialFileName);
            if (materialFile == null)
            {
                materialFile = currentAssets.FindFile(materialFileName.Replace("_m_", "_u_").Replace("_f_", "_u_"));
                // if (materialFile != null)
                //     materialFileName = materialFileName.Replace("_m_", "_u_").Replace("_f_", "_u_");
                // materialFile = currentAssets.FindFile(materialFileName.Replace("_u_", "_f_"));
                // if (materialFile != null)
                //     materialFileName = materialFileName.Replace("_u_", "_f_");
                // materialFile = currentAssets.FindFile(materialFileName.Replace("_u_", "_m_"));
                // if (materialFile != null)
                //     _ = materialFileName.Replace("_u_", "_m_");
            }
            if (materialFile != null)
            {
                var materialStream = materialFile.OpenCopyInMemory();

                XmlDocument material = new XmlDocument();
                material.Load(materialStream);

                string matType = material.SelectSingleNode("/Material/Derived").InnerText;
                polytype = material.SelectSingleNode("/Material/PolyType").InnerText;

                derived = matType;
                string alphaMode = material.SelectSingleNode("/Material/AlphaMode").InnerText;
                string alphaTestValue = material.SelectSingleNode("/Material/AlphaTestValue").InnerText;

                if (alphaMode != "None")
                {
                    alphaClip = true;
                    this.alphaMode = alphaMode;
                }
                this.alphaTestValue = float.Parse(alphaTestValue);

                isTwoSided = material.SelectSingleNode("/Material/IsTwoSided").InnerText == "True";

                XmlNodeList nodeList = material.SelectNodes("/Material/input");
                foreach (XmlNode node in nodeList)
                {
                    string semantic = node["semantic"].InnerText;
                    // _ = node["type"].InnerText;
                    string value = node["value"].InnerText.Replace("\\", "/");
                    if (semantic == "DiffuseMap")
                    {
                        diffuseDDS = ("/resources/" + value.Replace("\\", "/") + ".dds").Replace("//", "/");
                        var diffuseFile = currentAssets.FindFile(diffuseDDS);
                        if (diffuseFile != null && device != null)
                        {
                            var diffuseStream = diffuseFile.OpenCopyInMemory();
                            MemoryStream diffuseMS = new MemoryStream();
                            diffuseStream.CopyTo(diffuseMS);
                            diffuseSRV = ShaderResourceView.FromMemory(device, diffuseMS.ToArray());
                        }
                        else
                        {
                            diffuseDDS = "/resources/art/defaultassets/blue.dds";
                            var diffuseDefaultFile = currentAssets.FindFile(diffuseDDS);
                            if (diffuseDefaultFile != null && device != null)
                            {
                                var diffuseStream = diffuseDefaultFile.OpenCopyInMemory();
                                MemoryStream diffuseMS = new MemoryStream();
                                diffuseStream.CopyTo(diffuseMS);
                                diffuseSRV = ShaderResourceView.FromMemory(device, diffuseMS.ToArray());
                            }
                        }
                    }
                    else if (semantic == "RotationMap1")
                    {
                        rotationDDS = ("/resources/" + value + ".dds").Replace("//", "/");
                        var rotationFile = currentAssets.FindFile(rotationDDS);
                        if (rotationFile != null && device != null)
                        {
                            var rotationStream = rotationFile.OpenCopyInMemory();
                            MemoryStream rotationMS = new MemoryStream();
                            rotationStream.CopyTo(rotationMS);
                            rotationSRV = ShaderResourceView.FromMemory(device, rotationMS.ToArray());
                        }
                    }
                    else if (semantic == "GlossMap")
                    {
                        glossDDS = ("/resources/" + value + ".dds").Replace("//", "/");
                        var glossFile = currentAssets.FindFile(glossDDS);
                        if (glossFile != null && device != null)
                        {
                            var glossStream = glossFile.OpenCopyInMemory();
                            MemoryStream glossMS = new MemoryStream();
                            glossStream.CopyTo(glossMS);
                            glossSRV = ShaderResourceView.FromMemory(device, glossMS.ToArray());
                        }
                    }
                    else if (semantic == "UsesEmissive")
                    {
                        useEmissive = Convert.ToBoolean(value);
                    }

                    if (matType == "Garment" || matType == "GarmentScrolling" || matType == "SkinB" || matType == "HairC" || matType == "Eye")
                    {
                        if (semantic == "PaletteMap")
                        {
                            paletteDDS = ("/resources/" + value + ".dds").Replace("//", "/");
                            var paletteFile = currentAssets.FindFile(paletteDDS);
                            if (paletteFile != null && device != null)
                            {
                                var paletteStream = paletteFile.OpenCopyInMemory();
                                MemoryStream paletteMS = new MemoryStream();
                                paletteStream.CopyTo(paletteMS);
                                paletteSRV = ShaderResourceView.FromMemory(device, paletteMS.ToArray());
                            }
                        }
                        else if (semantic == "PaletteMaskMap")
                        {
                            paletteMaskDDS = ("/resources/" + value + ".dds").Replace("//", "/");
                            var paletteMaskFile = currentAssets.FindFile(paletteMaskDDS);
                            if (paletteMaskFile != null && device != null)
                            {
                                var paletteMaskStream = paletteMaskFile.OpenCopyInMemory();
                                MemoryStream paletteMaskMS = new MemoryStream();
                                paletteMaskStream.CopyTo(paletteMaskMS);
                                paletteMaskSRV = ShaderResourceView.FromMemory(device, paletteMaskMS.ToArray());
                            }
                        }
                        else if (semantic == "palette1")
                        {
                            if (palette1 == new Vector4())
                                palette1 = File_Helpers.StringToVec4(value);
                        }
                        else if (semantic == "palette2")
                        {
                            if (palette2 == new Vector4())
                                palette2 = File_Helpers.StringToVec4(value);
                        }
                        else if (semantic == "palette1Specular")
                        {
                            palette1Spec = File_Helpers.StringToVec4(value);
                        }
                        else if (semantic == "palette2Specular")
                        {
                            palette2Spec = File_Helpers.StringToVec4(value);
                        }
                        else if (semantic == "palette1MetallicSpecular")
                        {
                            palette1MetSpec = File_Helpers.StringToVec4(value);
                        }
                        else if (semantic == "palette2MetallicSpecular")
                        {
                            palette2MetSpec = File_Helpers.StringToVec4(value);
                        }
                    }

                    if (matType == "SkinB")
                    {
                        if (semantic == "ComplexionMap")
                        {
                            complexionDDS = ("/resources/" + value + ".dds").Replace("//", "/");
                            var complexionFile = currentAssets.FindFile(complexionDDS);
                            if (complexionFile != null && device != null)
                            {
                                var complexionStream = complexionFile.OpenCopyInMemory();
                                MemoryStream complexionMS = new MemoryStream();
                                complexionStream.CopyTo(complexionMS);
                                complexionSRV = ShaderResourceView.FromMemory(device, complexionMS.ToArray());
                            }
                        }
                        else if (semantic == "FacepaintMap")
                        {
                            facepaintDDS = ("/resources/" + value + ".dds").Replace("//", "/");
                            var facepaintFile = currentAssets.FindFile(facepaintDDS);
                            if (facepaintFile != null && device != null)
                            {
                                var facepaintStream = facepaintFile.OpenCopyInMemory();
                                MemoryStream facepaintMS = new MemoryStream();
                                facepaintStream.CopyTo(facepaintMS);
                                facepaintSRV = ShaderResourceView.FromMemory(device, facepaintMS.ToArray());
                            }
                        }
                        else if (semantic == "AgeMap")
                        {
                            ageDDS = ("/resources/" + value + ".dds").Replace("//", "/");
                            var ageFile = currentAssets.FindFile(ageDDS);
                            if (ageFile != null && device != null)
                            {
                                var ageStream = ageFile.OpenCopyInMemory();
                                MemoryStream ageMS = new MemoryStream();
                                ageStream.CopyTo(ageMS);
                                ageSRV = ShaderResourceView.FromMemory(device, ageMS.ToArray());
                            }
                        }
                        else if (semantic == "FlushTone")
                        {
                            if (flushTone == new Vector4())
                                flushTone = File_Helpers.StringToVec4(value);
                        }
                        else if (semantic == "FleshBrightness")
                        {
                            if (fleshBrightness == 0)
                                fleshBrightness = float.Parse(value);
                        }
                    }

                    if (matType == "Glass")
                    {
                        if (semantic == "UsesReflection")
                        {
                            useReflection = Convert.ToBoolean(value);
                        }
                        else if (semantic == "GlassParams")
                        {
                            glassParams = File_Helpers.StringToVec4(value);
                        }
                    }
                }

                if (palette1.X == 0 && palette1.Y == 0.5 && palette1.Z == 0 && palette1.W == 1 && parentMaterials != null)
                {
                    if (parentMaterials[0] != null)
                    {
                        palette1 = parentMaterials[0].palette1;
                        palette1MetSpec = parentMaterials[0].palette1MetSpec;
                        palette1Spec = parentMaterials[0].palette1Spec;
                        palette2 = parentMaterials[0].palette2;
                        palette2MetSpec = parentMaterials[0].palette2MetSpec;
                        palette2Spec = parentMaterials[0].palette2Spec;
                    }
                }
            }
            else
            {
                diffuseDDS = "/resources/art/defaultassets/blue.dds";
                var diffuseFile = currentAssets.FindFile(diffuseDDS);
                if (diffuseFile != null && device != null)
                {
                    var diffuseStream = diffuseFile.OpenCopyInMemory();
                    MemoryStream diffuseMS = new MemoryStream();
                    diffuseStream.CopyTo(diffuseMS);
                    diffuseSRV = ShaderResourceView.FromMemory(device, diffuseMS.ToArray());
                }
            }
            parsed = true;
        }

        public void SetDynamicColor(GomObject dynObj, int paletteNum = 0)
        {
            float hue = dynObj.Data.ValueOrDefault<float>("appPaletteHue", 0);
            float saturation = dynObj.Data.ValueOrDefault("appPaletteSaturation", 0.5f);
            float brightness = dynObj.Data.ValueOrDefault<float>("appPaletteBrightness", 0);
            float contrast = dynObj.Data.ValueOrDefault("appPaletteContrast", 1.0f);
            Vector4 palette = new Vector4(hue, saturation, brightness, contrast);
            GomObjectData specData = (GomObjectData)dynObj.Data.Dictionary["appPaletteSpecular"];
            Vector4 specular = new Vector4((float)specData.Dictionary["r"], (float)specData.Dictionary["g"], (float)specData.Dictionary["b"], (float)specData.Dictionary["a"]);
            GomObjectData metSpecData = (GomObjectData)dynObj.Data.Dictionary["appPaletteMetallicSpecular"];
            Vector4 metallicSpecular = new Vector4((float)metSpecData.Dictionary["r"], (float)metSpecData.Dictionary["g"], (float)metSpecData.Dictionary["b"], (float)metSpecData.Dictionary["a"]);

            if (paletteNum != 0)
            {
                if (paletteNum == 1)
                {
                    palette1 = palette;
                    palette1MetSpec = metallicSpecular;
                    palette1Spec = specular;
                }

                if (paletteNum == 2)
                {
                    palette2 = palette;
                    palette2MetSpec = metallicSpecular;
                    palette2Spec = specular;
                }
            }
            else
            {
                palette1 = palette;
                palette1MetSpec = metallicSpecular;
                palette1Spec = specular;
                palette2 = palette;
                palette2MetSpec = metallicSpecular;
                palette2Spec = specular;
            }
        }

        public void SetFacepaintMap(Device device, string facepaintPath)
        {
            facepaintDDS = "/resources" + facepaintPath;
            var facepaintFile = AssetHandler.Instance.GetCurrentAssets().FindFile(facepaintDDS);
            if (facepaintFile != null && device != null)
            {
                var facepaintStream = facepaintFile.OpenCopyInMemory();
                MemoryStream facepaintMS = new MemoryStream();
                facepaintStream.CopyTo(facepaintMS);
                facepaintSRV = ShaderResourceView.FromMemory(device, facepaintMS.ToArray());
            }
        }

        public void SetComplexionMap(Device device, string complexionPath)
        {
            complexionDDS = "/resources" + complexionPath;
            var complexionFile = AssetHandler.Instance.GetCurrentAssets().FindFile(complexionDDS);
            if (complexionFile != null && device != null)
            {
                var complexionStream = complexionFile.OpenCopyInMemory();
                MemoryStream complexionMS = new MemoryStream();
                complexionStream.CopyTo(complexionMS);
                complexionSRV = ShaderResourceView.FromMemory(device, complexionMS.ToArray());
            }
        }
    }
}
