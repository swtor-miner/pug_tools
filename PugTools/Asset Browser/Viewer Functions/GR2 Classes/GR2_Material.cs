using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using SlimDX;
using SlimDX.Direct3D11;
using ShaderResourceView = SlimDX.Direct3D11.ShaderResourceView;
using TorLib;

namespace FileFormats
{
    public class GR2_Material
    {
        public uint offsetMaterialName = 0;
        public string materialName = "";

        public string derived;
        public string polytype;

        public string diffuseDDS;
        public string rotationDDS;
        public string glossDDS;

        public ShaderResourceView diffuseSRV;
        public ShaderResourceView rotationSRV;
        public ShaderResourceView glossSRV;

        public ShaderResourceView paletteSRV;
        public ShaderResourceView paletteMaskSRV;

        public string complexionDDS;
        public string facepaintDDS;
        public string ageDDS;

        public ShaderResourceView complexionSRV;
        public ShaderResourceView facepaintSRV;
        public ShaderResourceView ageSRV;
        
        public string palette1XML;
        public string palette2XML;
        public string paletteDDS;
        public string paletteMaskDDS;
        public Vector4 palette1;
        public Vector4 palette2;
        public Vector4 palette1Spec;
        public Vector4 palette2Spec;
        public Vector4 palette1MetSpec;
        public Vector4 palette2MetSpec;

        public bool useEmissive;
        public bool alphaClip;        
        public float alphaClipValue;

        public bool parsed;
       
        public GR2_Material(string materialName)
        {
            this.materialName = materialName;
        }

        public GR2_Material(BinaryReader br)
        {
            this.offsetMaterialName = br.ReadUInt32();
            this.materialName = File_Helpers.ReadString(br, this.offsetMaterialName);
        }

        public void ParseMAT(Device device, List<GR2_Material> parentMaterials = null)
        {
            string materialFileName = "/resources/art/shaders/materials/" + this.materialName + ".mat";

            var materialID = FileId.FromFilePath(materialFileName);
            Assets currentAssets = AssetHandler.Instance.getCurrentAssets();
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

                        Vector4 metSpec = File_Helpers.stringToVec4(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Metallicspecular").InnerText);
                        Vector4 spec = File_Helpers.stringToVec4(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Specular").InnerText);
                        float hue = float.Parse(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Hue").InnerText);
                        float bright = float.Parse(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Brightness").InnerText);
                        float saturation = float.Parse(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Saturation").InnerText);
                        float contrast = float.Parse(palette1XML_Doc.DocumentElement.SelectSingleNode("/Palette/Contrast").InnerText);
                        this.palette1 = new Vector4(hue, saturation, bright, contrast);
                        this.palette1MetSpec = metSpec;
                        this.palette1Spec = spec;
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

                        Vector4 metSpec = File_Helpers.stringToVec4(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Metallicspecular").InnerText);
                        Vector4 spec = File_Helpers.stringToVec4(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Specular").InnerText);
                        float hue = float.Parse(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Hue").InnerText);
                        float bright = float.Parse(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Brightness").InnerText);
                        float saturation = float.Parse(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Saturation").InnerText);
                        float contrast = float.Parse(palette2XML_Doc.DocumentElement.SelectSingleNode("/Palette/Contrast").InnerText);
                        this.palette2 = new Vector4(hue, saturation, bright, contrast);
                        this.palette2MetSpec = metSpec;
                        this.palette2Spec = spec;

                    }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.StackTrace.ToString());
            }
            var materialFile = currentAssets.FindFile(materialFileName);
            if (materialFile == null)
            {
                materialFile = currentAssets.FindFile(materialFileName.Replace("_m_", "_u_").Replace("_f_", "_u_"));
                if (materialFile != null)
                    materialFileName = materialFileName.Replace("_m_", "_u_").Replace("_f_", "_u_");
                materialFile = currentAssets.FindFile(materialFileName.Replace("_u_", "_f_"));
                if (materialFile != null)
                    materialFileName = materialFileName.Replace("_u_", "_f_");
                materialFile = currentAssets.FindFile(materialFileName.Replace("_u_", "_m_"));
                if (materialFile != null)
                    materialFileName = materialFileName.Replace("_u_", "_m_");
            }
            if(materialFile != null)
            {
                var materialStream = materialFile.OpenCopyInMemory();
            
                XmlDocument material = new XmlDocument();                
                material.Load(materialStream);

                string matType = material.SelectSingleNode("/Material/Derived").InnerText;
                this.polytype = material.SelectSingleNode("/Material/PolyType").InnerText;

                derived = matType;
				string alphaMode = material.SelectSingleNode("/Material/AlphaMode").InnerText;
                string alphaTestValue = material.SelectSingleNode("/Material/AlphaTestValue").InnerText;

                if (alphaMode != "None")                
                    this.alphaClip = true;
                this.alphaClipValue = float.Parse(alphaTestValue);
                
                XmlNodeList nodeList = material.SelectNodes("/Material/input");
                foreach (XmlNode node in nodeList)
                {
                    string semantic = node["semantic"].InnerText;
                    string type = node["type"].InnerText;
                    string value = node["value"].InnerText.Replace("\\", "/");
                    if (semantic == "DiffuseMap")
                    {                        
                        diffuseDDS = "/resources/" + value.Replace("\\", "/") + ".dds";
                        var diffuseFile = currentAssets.FindFile(diffuseDDS);
                        if (diffuseFile != null && device != null)
                        {
                            var diffuseStream = diffuseFile.OpenCopyInMemory();
                            MemoryStream diffuseMS = new MemoryStream();
                            diffuseStream.CopyTo(diffuseMS);
                            diffuseSRV = ShaderResourceView.FromMemory(device, diffuseMS.ToArray());
                            //diffuseSRV = ShaderResourceView.FromFile(device, "n:\\_tor_extract" + diffuseDDS);
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
                        rotationDDS = "/resources/" + value + ".dds";
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
                        glossDDS = "/resources/" + value + ".dds";
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
                        this.useEmissive = Convert.ToBoolean(value);
                    }
                    if (matType == "Garment" || matType == "GarmentScrolling")
                    {                        
                        if (semantic == "PaletteMap")
                        {
                            paletteDDS = "/resources/" + value + ".dds";
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
                            paletteMaskDDS = "/resources/" + value + ".dds";
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
                            if(palette1 == new Vector4())
                                palette1 = FileFormats.File_Helpers.stringToVec4(value);
                        }
                        else if (semantic == "palette2")
                        {                            
                            if(palette2 == new Vector4())
                                palette2 = FileFormats.File_Helpers.stringToVec4(value);
                        }
                        else if (semantic == "palette1Specular")
                        {                            
                            palette1Spec = FileFormats.File_Helpers.stringToVec4(value);                            
                        }
                        else if (semantic == "palette2Specular")
                        {                            
                            palette2Spec = FileFormats.File_Helpers.stringToVec4(value);                         
                        }
                        else if (semantic == "palette1MetallicSpecular")
                        {                            
                            palette1MetSpec = FileFormats.File_Helpers.stringToVec4(value);                         
                        }
                        else if (semantic == "palette2MetallicSpecular")
                        {                            
                            palette2MetSpec = FileFormats.File_Helpers.stringToVec4(value);                         
                        }
                    }
                    if (matType == "SkinB")
                    {
                        if (semantic == "ComplexionMap")
                        {
                            complexionDDS = "/resources/" + value + ".dds";
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
                            facepaintDDS = "/resources/" + value + ".dds";
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
                            ageDDS = "/resources/" + value + ".dds";
                            var ageFile = currentAssets.FindFile(ageDDS);
                            if (ageFile != null && device != null)
                            {
                                var ageStream = ageFile.OpenCopyInMemory();
                                MemoryStream ageMS = new MemoryStream();
                                ageStream.CopyTo(ageMS);
                                ageSRV = ShaderResourceView.FromMemory(device, ageMS.ToArray());
                            }
                        }
                    }
                }

                if (this.palette1.X == 0 && this.palette1.Y == 0.5 && this.palette1.Z == 0 && this.palette1.W == 1 && parentMaterials != null)
                {
                    if (parentMaterials[0] != null)
                    {
                        this.palette1 = parentMaterials[0].palette1;
                        this.palette1MetSpec = parentMaterials[0].palette1MetSpec;
                        this.palette1Spec = parentMaterials[0].palette1Spec;
                        this.palette2 = parentMaterials[0].palette2;
                        this.palette2MetSpec = parentMaterials[0].palette2MetSpec;
                        this.palette2Spec = parentMaterials[0].palette2Spec;
                    }
                }
            }
            else
            {
                diffuseDDS = "/resources/art/defaultassets/blue.dds";
                var diffuseFile = currentAssets.FindFile(diffuseDDS);
                if (diffuseFile != null && device  != null)
                {
                    var diffuseStream = diffuseFile.OpenCopyInMemory();
                    MemoryStream diffuseMS = new MemoryStream();
                    diffuseStream.CopyTo(diffuseMS);
                    diffuseSRV = ShaderResourceView.FromMemory(device, diffuseMS.ToArray());
                }
            }
            this.parsed = true;
        }
    }
}
