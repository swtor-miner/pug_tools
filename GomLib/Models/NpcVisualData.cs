using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public ulong MeleeWepId { get; set; }
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
        public ulong MeleeOffWepId { get; set; }
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
        public ulong RangedWepId { get; set; }
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
        public ulong RangedOffWepId { get; set; }
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
        public ulong AppearanceId { get; set; }
        public string AppearanceFqn { get; set; }
        internal NpcAppearance _Appearance;
        //[Newtonsoft.Json.JsonIgnore]
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
    }
}
