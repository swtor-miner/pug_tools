using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class NpcVisualData : IEquatable<NpcVisualData>
    {
        [Newtonsoft.Json.JsonIgnore]
        DataObjectModel _dom;

        public NpcVisualData(DataObjectModel dom)
        {
            _dom = dom;
        }

        public string CharSpec { get; set; }
        public float ScaleAdjustment { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong MeleeWepId { get; set; }
        public string MeleeWepB62Id { get { return MeleeWepId.ToMaskedBase62(); } }
        [Newtonsoft.Json.JsonIgnore]
        internal Item _MeleeWep;
        [Newtonsoft.Json.JsonIgnore]
        public Item MeleeWep
        {
            get
            {
                if (_MeleeWep == null)
                    _MeleeWep = _dom.itemLoader.Load(MeleeWepId);
                return _MeleeWep;
            }
        }
        [JsonConverter(typeof(ULongConverter))]
        public ulong MeleeOffWepId { get; set; }
        public string MeleeOffWepB62Id { get { return MeleeOffWepId.ToMaskedBase62(); } }
        [Newtonsoft.Json.JsonIgnore]
        internal Item _MeleeOffWep;
        [Newtonsoft.Json.JsonIgnore]
        public Item MeleeOffWep
        {
            get
            {
                if (_MeleeOffWep == null)
                    _MeleeOffWep = _dom.itemLoader.Load(MeleeOffWepId);
                return _MeleeOffWep;
            }
        }
        [JsonConverter(typeof(ULongConverter))]
        public ulong RangedWepId { get; set; }
        public string RangedWepB62Id { get { return RangedWepId.ToMaskedBase62(); } }
        [Newtonsoft.Json.JsonIgnore]
        internal Item _RangedWep;
        [Newtonsoft.Json.JsonIgnore]
        public Item RangedWep
        {
            get
            {
                if (_RangedWep == null)
                    _RangedWep = _dom.itemLoader.Load(RangedWepId);
                return _RangedWep;
            }
        }
        [JsonConverter(typeof(ULongConverter))]
        public ulong RangedOffWepId { get; set; }
        public string RangedOffWepB62Id { get { return RangedOffWepId.ToMaskedBase62(); } }
        [Newtonsoft.Json.JsonIgnore]
        internal Item _RangedOffWep;
        [Newtonsoft.Json.JsonIgnore]
        public Item RangedOffWep
        {
            get
            {
                if (_RangedOffWep == null)
                    _RangedOffWep = _dom.itemLoader.Load(RangedOffWepId);
                return _RangedOffWep;
            }
        }
        [JsonConverter(typeof(ULongConverter))]
        public ulong AppearanceId { get; set; }
        public string AppearanceFqn { get; set; }
        internal NpcAppearance _Appearance;
        [Newtonsoft.Json.JsonIgnore]
        public NpcAppearance Appearance
        {
            get
            {
                if (_Appearance == null)
                    _Appearance = (NpcAppearance)_dom.appearanceLoader.Load(AppearanceFqn);
                return _Appearance;
            }
        }
        public long SpeciesScale { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            NpcVisualData nvd = obj as NpcVisualData;
            if (nvd == null) return false;

            return Equals(nvd);
        }

        public bool Equals(NpcVisualData nvd)
        {
            if (nvd == null) return false;

            if (ReferenceEquals(this, nvd)) return true;

            if (this.AppearanceId != nvd.AppearanceId)
                return false;
            if (this.CharSpec != nvd.CharSpec)
                return false;
            if (this.MeleeOffWepId != nvd.MeleeOffWepId)
                return false;
            if (this.MeleeWepId != nvd.MeleeWepId)
                return false;
            if (this.RangedOffWepId != nvd.RangedOffWepId)
                return false;
            if (this.RangedWepId != nvd.RangedWepId)
                return false;
            if (this.ScaleAdjustment != nvd.ScaleAdjustment)
                return false;
            if (this.SpeciesScale != nvd.SpeciesScale)
                return false;
            return true;
        }

        public XElement ToXElement()
        {
            return ToXElement(0);
        }
        public XElement ToXElement(int i)
        {
            XElement visualDataElement = new XElement("VisualData", new XAttribute("Id", i));

            visualDataElement.Add(new XElement("Skeleton", CharSpec),
                new XElement("ScaleAdjustment", ScaleAdjustment),
                new XElement("Appearance", AppearanceFqn, new XAttribute("Id", AppearanceId)));
            if (SpeciesScale != 0) visualDataElement.Add(new XElement("SpeciesScale", SpeciesScale));
            if (MeleeWepId != 0) visualDataElement.Add(new XElement("MeleeWep", (MeleeWep ?? new Item()).ToXElement(false), new XAttribute("Id", MeleeWepId)));
            if (MeleeOffWepId != 0) visualDataElement.Add(new XElement("MeleeOffWep", (MeleeOffWep ?? new Item()).ToXElement(false), new XAttribute("Id", MeleeOffWepId)));
            if (RangedWepId != 0) visualDataElement.Add(new XElement("RangedWep", (RangedWep ?? new Item()).ToXElement(false), new XAttribute("Id", RangedWepId)));
            if (RangedOffWepId != 0) visualDataElement.Add(new XElement("RangedOffWep", (RangedOffWep ?? new Item()).ToXElement(false), new XAttribute("Id", RangedOffWepId)));
            if (AppearanceId != 0)
            {
                XElement npp = (Appearance ?? new NpcAppearance(_dom)).ToXElement(true);
                visualDataElement.Add(npp);
                /*if (ExportNPP)
                {
                    WriteFile(new XDocument(npp), AppearanceFqn.Replace(".", "\\") + ".npp", false);
                }*/
            }

            return visualDataElement;
        }
    }
}
