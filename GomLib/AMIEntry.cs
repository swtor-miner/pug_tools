using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib
{
    public class AMIEntry : IEquatable<AMIEntry>
    {
        public long Id { get; set; }
        public string BaseFile { get; set; }
        public Dictionary<long, string> Attachments { get; set; }
        public string SlotType { get; set; }
        public Dictionary<long, Dictionary<long, string>> MaterialList { get; set; }
        public long SkinMaterialIndex { get; set; }
        public long SkinHueIndex { get; set; }
        public Dictionary<string, string> ChildSkinMaterials { get; set; }
        public long SithComplexion1 { get; set; } //Only found in ami.complexion
        public long SithComplexion2 { get; set; } //Only found in ami.complexion
        public Dictionary<string, float> RepresentativeColor { get; set; } //Only found in ami.garmenthue
        public string ColorAsVector4
        {
            get
            {
                if (RepresentativeColor != null)
                {
                    string red = "";
                    string green = "";
                    string blue = "";
                    string alpha = "";

                    if (RepresentativeColor.ContainsKey("r")) red = RepresentativeColor["r"].ToString();
                    if (RepresentativeColor.ContainsKey("g")) green = RepresentativeColor["g"].ToString();
                    if (RepresentativeColor.ContainsKey("b")) blue = RepresentativeColor["b"].ToString();
                    if (RepresentativeColor.ContainsKey("a")) alpha = RepresentativeColor["a"].ToString();
                    return String.Format("({0},{1},{2},{3})", red, green, blue, alpha);
                }
                return "";
            }
        }
        public Dictionary<string, ModelColors> Colors { get; set; } //Only found in ami.colorscheme

        public string GetAttachment(long id)
        {
            string attachment;
            if (!Attachments.TryGetValue(id, out attachment))
                attachment = "";
            return attachment;
        }

        public KeyValuePair<string, string> GetMaterial(long id)
        {
            KeyValuePair<string, string> material;

            Dictionary<long, string> matDict;
            if (!MaterialList.TryGetValue(id, out matDict))
                material = new KeyValuePair<string,string>("","");
            if (matDict == null)
                return new KeyValuePair<string, string>("", "");
            if (matDict.ContainsKey(-1))
                material = new KeyValuePair<string, string>(matDict[0], matDict[-1]);
            else
                material = new KeyValuePair<string, string>(matDict[0], matDict[0]);
            return material;
        }

        public void Load(GomObjectData data)
        {
            Id = data.ValueOrDefault<long>("appModelId", 0);
            BaseFile = data.ValueOrDefault<string>("appModelBaseFile", "");
            var attachments = data.ValueOrDefault<Dictionary<object, object>>("appModelAttachments", new Dictionary<object, object>());
            Attachments = attachments.ToDictionary(a => (long)a.Key, a => (string)a.Value);
            SlotType = data.ValueOrDefault<ScriptEnum>("appModelSlotType", new ScriptEnum()).ToString();

            var matList = data.ValueOrDefault<Dictionary<object, object>>("appModelMaterialList", new Dictionary<object, object>());

            if (matList.Count != 0)
            {
                MaterialList = new Dictionary<long, Dictionary<long, string>>();
                foreach (var mat in matList)
                {
                    Dictionary<long, string> matEntry = new Dictionary<long, string>();
                    foreach (var matFile in ((Dictionary<object, object>)mat.Value).ToDictionary(v => (long)v.Key, v => (string)v.Value))
                    {
                        matEntry.Add(matFile.Key, matFile.Value);
                    }
                    MaterialList.Add((long)mat.Key, matEntry);
                }
            }

            SkinMaterialIndex = data.ValueOrDefault<long>("appModelSkinMaterialIndex", -1);
            SkinHueIndex = data.ValueOrDefault<long>("appModelSkinHueIndex", -1);

            var childSkinMaterials = data.ValueOrDefault<Dictionary<object, object>>("appModelChildSkinMaterials", null);
            if (childSkinMaterials != null)
            {
                if (childSkinMaterials.Count != 0)
                {
                    ChildSkinMaterials = new Dictionary<string, string>();
                    foreach (var skinMat in childSkinMaterials)
                    {
                        ChildSkinMaterials.Add(((ScriptEnum)skinMat.Key).ToString(), skinMat.Value.ToString());
                    }
                }
            }
            //ChildSkinMaterials = 

            SithComplexion1 = data.ValueOrDefault<long>("appModelSithComplexion1", -1);
            SithComplexion1 = data.ValueOrDefault<long>("appModelSithComplexion1", -1);

            var repColor = data.ValueOrDefault<object>("appModelRepresentativeColor", null);

            if (repColor != null)
            {
                var repObject = repColor as GomObjectData;
                if (repObject.Dictionary.Count > 3)
                {
                    RepresentativeColor = new Dictionary<string, float>();
                    foreach(var color in repObject.Dictionary.Skip(3))
                    {
                        RepresentativeColor.Add(color.Key, (float)color.Value);
                    }
                }

            }

            var modelColors = data.ValueOrDefault<Dictionary<object, object>>("appModelColors", new Dictionary<object, object>());
            Colors = new Dictionary<string, ModelColors>();
            foreach (var kvp in modelColors)
            {
                ModelColors mc = new ModelColors((GomObjectData)(kvp.Value));
                Colors.Add(kvp.Key.ToString(), mc);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            AMIEntry ame = obj as AMIEntry;
            if (ame == null) return false;

            return Equals(ame);
        }

        public bool Equals(AMIEntry ame)
        {
            if (ame == null) return false;

            if (ReferenceEquals(this, ame)) return true;

            if (this.Id != ame.Id)
                return false;

            var lsComp = new Models.DictionaryComparer<long, string>();
            if (!lsComp.Equals(this.Attachments, ame.Attachments))
                return false;

            if (this.BaseFile != ame.BaseFile)
                return false;

            var ssComp = new Models.DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.ChildSkinMaterials, ame.ChildSkinMaterials))
                return false;

            if (this.Colors != null)
            {
                if (ame.Colors != null)
                {
                    if (this.Colors.Count != ame.Colors.Count)
                        return false;
                    foreach (var color in this.Colors)
                    {
                        ModelColors oColor;
                        ame.Colors.TryGetValue(color.Key, out oColor);
                        if (!color.Value.Equals(oColor))
                            return false;
                    }
                }
                else
                    return false;
            }
            else if (ame.Colors != null)
                return false;

            if (this.MaterialList != null)
            {
                if (ame.MaterialList != null)
                {
                    if (this.MaterialList.Count != ame.MaterialList.Count)
                        return false;
                    foreach (var material in this.MaterialList)
                    {
                        Dictionary<long, string> oMaterial;
                        ame.MaterialList.TryGetValue(material.Key, out oMaterial);
                        if (!lsComp.Equals(material.Value, oMaterial))
                            return false;
                    }
                }
                else
                    return false;
            }
            else if (ame.MaterialList != null)
                return false;

            var sfComp = new Models.DictionaryComparer<string, float>();
            if (!sfComp.Equals(this.RepresentativeColor, ame.RepresentativeColor))
                return false;

            if (this.SithComplexion1 != ame.SithComplexion1)
                return false;
            if (this.SithComplexion2 != ame.SithComplexion2)
                return false;
            if (this.SkinHueIndex != ame.SkinHueIndex)
                return false;
            if (this.SkinMaterialIndex != ame.SkinMaterialIndex)
                return false;
            if (this.SlotType != ame.SlotType)
                return false;
            return true;
        }
    }

    public class ModelColors : IEquatable<ModelColors>
    {
        long PrimaryColor { get; set; }
        long SecondaryColor { get; set; }

        public ModelColors(GomObjectData data)
        {
            PrimaryColor = data.ValueOrDefault<long>("appModelPrimaryColor", 0);
            SecondaryColor = data.ValueOrDefault<long>("appModelPrimaryColor", 0);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ModelColors ame = obj as ModelColors;
            if (ame == null) return false;

            return Equals(ame);
        }

        public bool Equals(ModelColors ame)
        {
            if (ame == null) return false;

            if (ReferenceEquals(this, ame)) return true;

            if (this.PrimaryColor != ame.PrimaryColor)
                return false;
            if (this.SecondaryColor != ame.SecondaryColor)
                return false;
            return true;
        }
    }
}
