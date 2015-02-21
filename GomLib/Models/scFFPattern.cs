using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class scFFPattern : PseudoGameObject, IEquatable<scFFPattern>
    {
        //scFFPatternsDefinitionPrototype - scFFShipPatternUIData - class lookup

        public string Availability { get; set; }    //scFFPatternIsAvailable - enum : scFFUnavailable
        public string Icon { get; set; }            //scFFPatternIcon - "spvp_paint_job_scout"
        //public long Id { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeprecated { get; set; }
        //public string Name { get; set; }
        public long NameId { get; set; }            //scFFPatternName - 3285259139416370
        public long ShipId { get; set; }            //scFFPatternShipId - 3378619052355679612
        public long ShortId { get; set; }           /*scFFPattternShortId - examples 1794, 1808, 1790, 1816, 1813, 1793,
                                                     * Short Lookup Id in scFFPatternsDefinitionPrototype - scFFPatternsDefinitionShortIdData*/
        public string TextureForCurrentShip { get; set; }
        public Dictionary<long, string> TexturesByShipId { get; set; }
        public long VeryShortId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            scFFPattern spa = obj as scFFPattern;
            if (spa == null) return false;

            return Equals(spa);
        }

        public bool Equals(scFFPattern spa)
        {
            if (spa == null) return false;

            if (ReferenceEquals(this, spa)) return true;

            if (this.Availability != spa.Availability)
                return false;
            if (this.Icon != spa.Icon)
                return false;
            if (this.Id != spa.Id)
                return false;
            if (this.IsAvailable != spa.IsAvailable)
                return false;
            if (this.IsDeprecated != spa.IsDeprecated)
                return false;
            if (this.Name != spa.Name)
                return false;
            if (this.NameId != spa.NameId)
                return false;
            if (this.ShipId != spa.ShipId)
                return false;
            if (this.ShortId != spa.ShortId)
                return false;
            if (this.TextureForCurrentShip != spa.TextureForCurrentShip)
                return false;

            var lsComp = new DictionaryComparer<long, string>();
            if (!lsComp.Equals(this.TexturesByShipId, spa.TexturesByShipId))
                return false;

            if (this.VeryShortId != spa.VeryShortId)
                return false;

            return true;
        }
    }
}
