using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class ScFFColorOption : IEquatable<ScFFColorOption>
    {
        //scFFColorOptionMasterPrototype - scFFComponentColorUIData - class lookup

        public string Availability { get; set; }    //scFFColorIsAvailable - enum : scFFUnavailable
        public string ColorCode { get; set; }       //scFFColorCodeId - "SPVP_E_LTRED"
        public string Icon { get; set; }            //scFFComponentColorIcon - "spvp_engine_light_red"
        public bool IsAvailable { get; set; }
        public bool IsDeprecated { get; set; }
        public string Name { get; set; }
        public ulong NameId { get; set; }            //scFFColorName - 1395142816694530
        public long ShipId { get; set; }            //scFFColorShipId - 1086966210362573345
        public long ShortId { get; set; }           /*scFFColorShortId - examples 1794, 1808, 1790, 1816, 1813, 1793,
                                                     * Short Lookup Id in scFFColorOptionMasterPrototype - scFFColorShortIdData*/
        public string Type { get; set; }            //scFFColorType - enum : scFFColorOptionEngine

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is ScFFColorOption sco)) return false;

            return Equals(sco);
        }

        public bool Equals(ScFFColorOption sco)
        {
            if (sco == null) return false;

            if (ReferenceEquals(this, sco)) return true;

            if (this.Availability != sco.Availability)
                return false;
            if (this.ColorCode != sco.ColorCode)
                return false;
            if (this.Icon != sco.Icon)
                return false;
            if (this.IsAvailable != sco.IsAvailable)
                return false;
            if (this.IsDeprecated != sco.IsDeprecated)
                return false;
            if (this.Name != sco.Name)
                return false;
            if (this.NameId != sco.NameId)
                return false;
            if (this.ShipId != sco.ShipId)
                return false;
            if (this.ShortId != sco.ShortId)
                return false;
            if (this.Type != sco.Type)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
