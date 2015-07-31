using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class AppSlot : PseudoGameObject, IEquatable<AppSlot>
    {
        public AppSlot(DataObjectModel dom)
        {
            _dom = dom;
        }

        public string BodyType { get; set; }
        public string Type 
        {
            get { return _type.Substring(7).ToLower();; }
            set
            {
                _type = value;
            }
        }
        internal string _type;
        public long ModelID{ get; set; }
        public string Model
        {
            get
            {
                if (AMI != null)
                    return AMI.BaseFile; //.Replace("[BT]", BodyType).Replace("[bt]", BodyType);
                return "";
            }
        }
        internal AMIEntry _AMI;
        [Newtonsoft.Json.JsonIgnore]
        public AMIEntry AMI
        {
            get
            {
                if (_AMI == null)
                    _AMI = _dom.ami.Find(Type, ModelID);
                return _AMI;
            }
        }
        public long MaterialIndex { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal string _Material0;
        [Newtonsoft.Json.JsonIgnore] //Need to fix this
        public string Material0
        {
            get
            {
                if (_Material0 == null)
                    FillMats();
                return _Material0; //.Replace("[BT]", BodyType).Replace("[bt]", BodyType);
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        internal string _MaterialMirror;
        [Newtonsoft.Json.JsonIgnore] //Need to fix this
        public string MaterialMirror
        {
            get
            {
                if (_MaterialMirror == null)
                    FillMats();
                return _MaterialMirror; //.Replace("[BT]", BodyType).Replace("[bt]", BodyType);
            }
        }
        internal void FillMats()
        {
            if (AMI == null)
            {
                _Material0 = "";
                _MaterialMirror = "";
                return;
            }
            var matKvP = AMI.GetMaterial(MaterialIndex);
            _Material0 = matKvP.Key;
            _MaterialMirror = matKvP.Value;
        }
        public List<long> Attachments { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal List<string> _AttachedModels;
        public List<string> AttachedModels
        {
            get
            {
                if (_AttachedModels == null)
                {
                    if (Attachments == null)
                        return null;
                    _AttachedModels = new List<string>();
                    foreach (var attachId in Attachments)
                    {
                        if (AMI == null)
                            _AttachedModels.Add("");
                        else
                            _AttachedModels.Add(AMI.GetAttachment(attachId)); //.Replace("[BT]", BodyType).Replace("[bt]", BodyType));
                    }
                }
                return _AttachedModels;
            }
        }
        public long PrimaryHueId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal AMIEntry _PrimaryAMI;
        [Newtonsoft.Json.JsonIgnore]
        private AMIEntry PrimaryAMI
        {
            get
            {
                if (_PrimaryAMI == null)
                    _PrimaryAMI = _dom.ami.Find("garmenthue", PrimaryHueId);
                return _PrimaryAMI;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        internal string _PrimaryHue;
        public string PrimaryHue
        {
            get
            {
                if (_PrimaryHue == null)
                    if (PrimaryAMI != null)
                        _PrimaryHue = String.Format("{0};{1}", PrimaryAMI.BaseFile, PrimaryAMI.ColorAsVector4);
                    else
                        return "";
                return _PrimaryHue;
            }
        }

        public long SecondaryHueId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal AMIEntry _SecondaryAMI;
        [Newtonsoft.Json.JsonIgnore]
        private AMIEntry SecondaryAMI
        {
            get
            {
                if (_SecondaryAMI == null)
                    _SecondaryAMI = _dom.ami.Find("garmenthue", SecondaryHueId);
                return _SecondaryAMI;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        internal string _SecondaryHue;
        public string SecondaryHue
        {
            get
            {
                if (_SecondaryHue == null)
                    if (SecondaryAMI != null)
                        _SecondaryHue = String.Format("{0};{1}", SecondaryAMI.BaseFile, SecondaryAMI.ColorAsVector4);
                    else
                        return "";
                return _SecondaryHue;
            }
        }

        public long RandomWeight { get; set; }

        public override HashSet<string> GetDependencies()
        {
            HashSet<string> returnList = new HashSet<string>();
            returnList.Add(this.Model);
            //finish this off
            return returnList;
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (BodyType != null) hash ^= BodyType.GetHashCode();
            if (Type != null) hash ^= Type.GetHashCode();
            hash ^= ModelID.GetHashCode();
            //if (AMI != null) hash ^= AMI.GetHashCode(); //this can change without changing the appearance
            hash ^= MaterialIndex.GetHashCode();
            if (Material0 != null) hash ^= Material0.GetHashCode();
            if (MaterialMirror != null) hash ^= MaterialMirror.GetHashCode();
            if (Material0 != null) hash ^= Material0.GetHashCode();
            if (Material0 != null) hash ^= Material0.GetHashCode();
            if (Material0 != null) hash ^= Material0.GetHashCode();
            if (Material0 != null) hash ^= Material0.GetHashCode();
            if (Attachments != null) foreach (var x in Attachments) { hash ^= x.GetHashCode(); }
            if (AttachedModels != null) foreach (var x in AttachedModels) { hash ^= x.GetHashCode(); }
            hash ^= PrimaryHueId.GetHashCode();
            if (PrimaryHue != null) hash ^= PrimaryHue.GetHashCode();
            hash ^= SecondaryHueId.GetHashCode();
            if (SecondaryHue != null) hash ^= SecondaryHue.GetHashCode();
            hash ^= RandomWeight.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            AppSlot aps = obj as AppSlot;
            if (aps == null) return false;

            return Equals(aps);
        }

        public bool Equals(AppSlot aps)
        {
            if (aps == null) return false;

            if (ReferenceEquals(this, aps)) return true;

            if (!Attachments.SequenceEqual(aps.Attachments))
                return false;
            if (MaterialIndex != aps.MaterialIndex)
                return false;
            if (ModelID != aps.ModelID)
                return false;
            if (RandomWeight != aps.RandomWeight)
                return false;
            if (Type != aps.Type)
                return false;
            //checked low hanging fruit
            if (AMI != null)
            {
                if (!AMI.Equals(aps.AMI))
                {
                    //AMI changed, but did this ModelID/MaterialIndex?
                    if (!AttachedModels.SequenceEqual(aps.AttachedModels))
                        return false;
                    if (Material0 != aps.Material0)
                        return false;
                    if (MaterialMirror != aps.MaterialMirror)
                        return false;
                    if (Model != aps.Model)
                        return false;
                }
            }
            else if (aps.AMI != null)
                return false;

            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement appearanceSlot = new XElement("AppearanceSlot", new XAttribute("Id", ModelID));

            appearanceSlot.Add(new XElement("Type", Type));
            if (ModelID != 0) appearanceSlot.Add(new XElement("Model", Model, new XAttribute("Id", ModelID)));
            if (MaterialIndex != 0) appearanceSlot.Add(new XElement("Material", Material0, new XAttribute("Id", MaterialIndex)),
                new XElement("MaterialMirror", Material0, new XAttribute("Id", MaterialIndex)));
            if (Attachments.Count > 0)
            {
                XElement attachments = new XElement("Attachments");
                for (int i = 0; i < Attachments.Count; i++)
                {
                    attachments.Add(new XElement("Attachment", AttachedModels[i], new XAttribute("Id", Attachments[i])));
                }
                appearanceSlot.Add(attachments);
            }
            if (PrimaryHueId != 0) appearanceSlot.Add(new XElement("PrimaryHue", PrimaryHue, new XAttribute("Id", PrimaryHueId)));
            if (SecondaryHueId != 0) appearanceSlot.Add(new XElement("SecondaryHue", SecondaryHue, new XAttribute("Id", SecondaryHueId)));

            appearanceSlot.Add(new XElement("RandomWeight", RandomWeight));
            return appearanceSlot;
        }

    }

    public class ItemAppearance : GameObject, IEquatable<ItemAppearance>
    {
        public ItemAppearance(DataObjectModel dom)
        {
            _dom = dom;
        }

        public ItemAppearance() { }

        public AppSlot IPP { get; set; }
        public long ColorScheme { get; set; } 
        public string VOSoundTypeOverride { get; set; } //only found on IPPs of type appSlotFace
        [Newtonsoft.Json.JsonIgnore]
        internal List<string> _SimilarAppearances { get; set; }
        public List<string> SimilarAppearances
        {
            get
            {
                if (_dom.GetObject(Id).References != null)
                {
                    if (_SimilarAppearances == null)
                    {
                        _SimilarAppearances = new List<string>();
                        if (_dom.GetObject(Id).References.ContainsKey("similarAppearance"))
                        {
                            _SimilarAppearances = _dom.GetObject(Id).References["similarAppearance"].Select(x => x.ToMaskedBase62()).ToList();
                        }
                    }
                    return _SimilarAppearances;
                }
                return null;
            }
        }
        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (IPP != null) hash ^= IPP.GetHashCode();
            hash ^= ColorScheme.GetHashCode();
            if (VOSoundTypeOverride != null) hash ^= VOSoundTypeOverride.GetHashCode();

            return hash;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ItemAppearance ipp = obj as ItemAppearance;
            if (ipp == null) return false;

            return Equals(ipp);
        }
        public bool Equals(ItemAppearance ipp)
        {
            if (ipp == null) return false;

            if (ReferenceEquals(this, ipp)) return true;

            if (ColorScheme != ipp.ColorScheme)
                return false;
            if (VOSoundTypeOverride != ipp.VOSoundTypeOverride)
                return false;
            if (IPP != null)
            {
                if (!IPP.Equals(ipp.IPP))
                    return false;
            }
            else if (ipp.IPP != null)
                return false;

            return true;
        }

        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, SQLPropSetting[])
                        new SQLProperty("NodeId", "Id", "bigint(20) unsigned NOT NULL"),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.PrimaryKey),
                        new SQLProperty("Fqn", "Fqn", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("SimilarAppearances","SimilarAppearances", "TEXT NOT NULL", SQLPropSetting.JsonSerialize),
                    };
            }
        }
        public override XElement ToXElement(bool verbose)
        {
            XElement ItemAppearance = new XElement("ItemAppearance", new XAttribute("Id", IPP.ModelID));
            XElement appSlot = IPP.ToXElement(true);

            ItemAppearance.Add(appSlot.Elements());
            ItemAppearance.Add(new XElement("ColorScheme", ColorScheme),
                new XElement("VOSoundTypeOverride", VOSoundTypeOverride));

            return ItemAppearance;
        }
    }

    public class NpcAppearance : GameObject, IEquatable<NpcAppearance>
    {
        public NpcAppearance(DataObjectModel dom)
        {
            _dom = dom;
        }

        public string BodyType { get; set; }
        public Dictionary<string, List<AppSlot>> AppearanceSlotMap { get; set; }
        public string NppType { get; set; }
        public string SoundPackage { get; set; }
        public string ArmorSoundsetOverride { get; set; }
        public Dictionary<long, string> VocalSoundsetOverride { get; set; }

        public override int GetHashCode() //needs fixed, it's changing for the same data
        {
            int hash = Id.GetHashCode();
            if (BodyType != null) hash ^= BodyType.GetHashCode();
            if (AppearanceSlotMap != null)
                foreach (var x in AppearanceSlotMap)
                {
                    if (x.Key != null) hash ^= x.Key.GetHashCode();
                    foreach (var y in x.Value)
                    { hash ^= y.GetHashCode(); }
                }
            if (NppType != null) hash ^= NppType.GetHashCode();
            if (SoundPackage != null) hash ^= SoundPackage.GetHashCode();
            if (ArmorSoundsetOverride != null) hash ^= ArmorSoundsetOverride.GetHashCode();
            if (VocalSoundsetOverride != null) foreach (var x in VocalSoundsetOverride) { hash ^= x.GetHashCode(); }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            NpcAppearance npp = obj as NpcAppearance;
            if (npp == null) return false;

            return Equals(npp);
        }

        public bool Equals(NpcAppearance npp)
        {
            if (npp == null) return false;

            if (ReferenceEquals(this, npp)) return true;

            if (AppearanceSlotMap != null)
            {
                if (npp.AppearanceSlotMap == null)
                    return false;
                foreach (var appearance in AppearanceSlotMap)
                {
                    var prevApp = new List<AppSlot>();
                    npp.AppearanceSlotMap.TryGetValue(appearance.Key, out prevApp);
                    if (!appearance.Value.SequenceEqual(prevApp))
                        return false;
                }
            }
            else if (npp.AppearanceSlotMap != null)
                return false;
            if (ArmorSoundsetOverride != npp.ArmorSoundsetOverride)
                return false;
            if (BodyType != npp.BodyType)
                return false;
            if (Fqn != npp.Fqn)
                return false;
            if (Id != npp.Id)
                return false;
            if (NppType != npp.NppType)
                return false;
            if (SoundPackage != npp.SoundPackage)
                return false;

            var lsComp = new DictionaryComparer<long, string>();
            if (!lsComp.Equals(VocalSoundsetOverride, npp.VocalSoundsetOverride))
                return false;

            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement npcAppearance = new XElement("NpcAppearance", new XAttribute("Id", Id));

            if (Id == 0) return npcAppearance;

            npcAppearance.Add(new XElement("Type", NppType),
                new XElement("BodyType", BodyType),
                new XElement("Fqn", Fqn));
            if (AppearanceSlotMap.Count > 0)
            {
                XElement slotMap = new XElement("AppearanceSlotMap");
                foreach (var appSlot in AppearanceSlotMap)
                {
                    XElement slot = new XElement(appSlot.Key);
                    foreach (var randomSlot in appSlot.Value)
                    {
                        slot.Add(randomSlot.ToXElement(true));
                    }
                    slotMap.Add(slot);
                }
                npcAppearance.Add(slotMap);
            }

            return npcAppearance;
        }
    }

    public class WeaponAppearance : PseudoGameObject, IEquatable<WeaponAppearance>
    {
        public WeaponAppearance(DataObjectModel dom)
        {
            _dom = dom;
        }

        public WeaponAppearance() { }

        public override string Name { get; set; }
        public string WeaponType { get; set; }
        public string Model { get; set; }
        public string FxSpec { get; set; }
        public string BoneName { get; set; }
        public string DynamicData { get; set; }
        public string CombatStance { get; set; }
        public List<float> StowedScale { get; set; }
        public List<float> StowedRotation { get; set; }
        public List<float> StowedOffset { get; set; }
        public List<float> DrawnScale { get; set; }
        public List<float> DrawnRotation { get; set; }
        public List<float> DrawnOffset { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            WeaponAppearance wpp = obj as WeaponAppearance;
            if (wpp == null) return false;

            return Equals(wpp);
        }

        public bool Equals(WeaponAppearance wpp)
        {
            if (wpp == null) return false;

            if (ReferenceEquals(this, wpp)) return true;

            if (Name != wpp.Name)
                return false;
            if (BoneName != wpp.BoneName)
                return false;
            if (CombatStance != wpp.CombatStance)
                return false;
            if (DrawnOffset != wpp.DrawnOffset)
                return false;
            if (DrawnRotation != wpp.DrawnRotation)
                return false;
            if (DrawnScale != wpp.DrawnScale)
                return false;
            if (DynamicData != wpp.DynamicData)
                return false;
            if (FxSpec != wpp.FxSpec)
                return false;
            if (Model != wpp.Model)
                return false;
            if (StowedOffset != wpp.StowedOffset)
                return false;
            if (StowedRotation != wpp.StowedRotation)
                return false;
            if (StowedScale != wpp.StowedScale)
                return false;
            if (WeaponType != wpp.WeaponType)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            hash ^= WeaponType.GetHashCode();
            hash ^= Model.GetHashCode();
            hash ^= FxSpec.GetHashCode();
            hash ^= BoneName.GetHashCode();
            hash ^= DynamicData.GetHashCode();
            hash ^= CombatStance.GetHashCode();
            if(StowedScale != null)
            {
                //This ensures list order.
                hash = hash * 13;
                foreach(float value in StowedScale)
                {
                    hash = hash * 31 + value.GetHashCode();
                }
            }
            if (StowedRotation != null)
            {
                //This ensures list order.
                hash = hash * 2;
                foreach (float value in StowedRotation)
                {
                    hash = hash * 31 + value.GetHashCode();
                }
            }
            if (StowedOffset != null)
            {
                //This ensures list order.
                hash = hash * 3;
                foreach (float value in StowedOffset)
                {
                    hash = hash * 31 + value.GetHashCode();
                }
            }
            if (DrawnScale != null)
            {
                //This ensures list order.
                hash = hash * 5;
                foreach (float value in DrawnScale)
                {
                    hash = hash * 31 + value.GetHashCode();
                }
            }
            if (DrawnOffset != null)
            {
                //This ensures list order.
                hash = hash * 7;
                foreach (float value in DrawnOffset)
                {
                    hash = hash * 31 + value.GetHashCode();
                }
            }
            if (DrawnRotation != null)
            {
                //This ensures list order.
                hash = hash * 11;
                foreach (float value in DrawnRotation)
                {
                    hash = hash * 31 + value.GetHashCode();
                }
            }

            return hash;
        }
    }
}
