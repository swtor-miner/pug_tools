using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class scFFComponent : GameObject, IEquatable<scFFComponent>
    {
        public string ColorOption { get; set; }
        public long ComponentId { get; set; }
        public ulong ControllerNodeId { get; set; }
        public int Cost { get; set; }
        public ulong CostLookupId { get; set; }
        public string Description { get; set; }
        public long DescriptionId { get; set; }
        //public string Fqn { get; set; }
        public string Icon { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeprecated { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public long NameId { get; set; }
        //public ulong Id { get; set; }
        public long NumUpgradeTiers { get; set; }
        public string Slot { get; set; }
        public Dictionary<string, float> StatsList { get; set; }
        public Dictionary<int, int> TalentCostList { get; set; }
        public Dictionary<int, GameObject> TalentList; // this should only be used when the TalentTree is empty
        public scFFTalentTree Talents;
        public long UsedByShipId { get; set; }
        public long UnknownId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            scFFComponent scp = obj as scFFComponent;
            if (scp == null) return false;

            return Equals(scp);
        }

        public bool Equals(scFFComponent scp)
        {
            if (scp == null) return false;

            if (ReferenceEquals(this, scp)) return true;

            if (this.ColorOption != scp.ColorOption)
                return false;
            if (this.ComponentId != scp.ComponentId)
                return false;
            if (this.ControllerNodeId != scp.ControllerNodeId)
                return false;
            if (this.Cost != scp.Cost)
                return false;
            if (this.CostLookupId != scp.CostLookupId)
                return false;
            if (this.Description != scp.Description)
                return false;
            if (this.DescriptionId != scp.DescriptionId)
                return false;
            if (this.Fqn != scp.Fqn)
                return false;
            if (this.Icon != scp.Icon)
                return false;
            if (this.Id != scp.Id)
                return false;
            if (this.IsAvailable != scp.IsAvailable)
                return false;
            if (this.IsDeprecated != scp.IsDeprecated)
                return false;
            if (this.Model != scp.Model)
                return false;
            if (this.Name != scp.Name)
                return false;
            if (this.NameId != scp.NameId)
                return false;
            if (this.NumUpgradeTiers != scp.NumUpgradeTiers)
                return false;
            if (this.Slot != scp.Slot)
                return false;

            var sfComp = new DictionaryComparer<string, float>();
            if (!sfComp.Equals(this.StatsList, scp.StatsList))
                return false;

            var iiComp = new DictionaryComparer<int, int>();
            if (!iiComp.Equals(this.TalentCostList, scp.TalentCostList))
                return false;

            if (this.TalentList != null)
            {
                if (scp.TalentList != null)
                {
                    if (!this.TalentList.Keys.SequenceEqual(scp.TalentList.Keys))
                        return false;
                    foreach (var kvp in this.TalentList)
                    {
                        if (!kvp.Value.Equals(scp.TalentList[kvp.Key]))
                            return false;
                    }
                }
                else
                    return false;
            }

            if (!this.Talents.Equals(scp.Talents))
                return false;
            if (this.UnknownId != scp.UnknownId)
                return false;
            if (this.UsedByShipId != scp.UsedByShipId)
                return false;
            return true;
        }
    }
}
