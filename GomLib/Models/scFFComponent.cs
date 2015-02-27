using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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

        public override XElement ToXElement(bool verbose)
        {
            return ToXElement(false, verbose);
        }
        public XElement ToXElement(bool isDefault, bool verbose)
        {
            XElement component = new XElement("Component");
            if (verbose) component.Add(new XElement("Fqn", Fqn), new XAttribute("Id", ComponentId));
            component.Add(new XElement("Name", Name),// new XAttribute("Id", NameId)),
                new XElement("Description", Description),// new XAttribute("Id", DescriptionId)),
                //new XElement("Icon",Icon),
                new XElement("IsDefault", isDefault));
            if (verbose) component.Add(new XElement("IsAvailable_IsDeprecated", IsAvailable + "," + IsDeprecated));
            if (TalentList.ContainsKey(0))
            {
                var type = TalentList[0].GetType();
                if (type.Name == "Talent")
                {
                    if (verbose) component.Add(new XElement("Ability"));
                    component.Add(((GomLib.Models.Talent)TalentList[0]).ToXElement(verbose));
                }
                else
                {
                    component.Add(((GomLib.Models.Ability)TalentList[0]).ToXElement(verbose));
                    component.Add(new XElement("Talent"));
                }
            }
            if (verbose)
            {
                XElement talentTree = new XElement("TalentTree", new XAttribute("NumUpgdTiers", NumUpgradeTiers));
                if(Talents.Ability != null)
                    talentTree.Add(Talents.Ability.ToXElement(verbose));
                foreach (var row in Talents.Tree)
                {
                    XElement xRow = new XElement("Row", new XAttribute("Id", row.Key));
                    foreach (var column in row.Value)
                    {
                        XElement xCol = null; //new XElement("Column");
                        if (((List<object>)column.Value)[0].GetType() == typeof(GomLib.Models.Talent))
                        {
                            xCol = ((GomLib.Models.Talent)((List<object>)column.Value)[0]).ToXElement();
                        }
                        else
                        {
                            xCol = ((GomLib.Models.Ability)((List<object>)column.Value)[0]).ToXElement(verbose);
                        }
                        xCol.Add(new XAttribute("Priority", column.Key));
                        if (((List<object>)column.Value)[1] == null)
                        {
                            xCol.Add(new XAttribute("Target", "All"));
                        }
                        else
                        {
                            xCol.Add(new XAttribute("Target", ((List<object>)column.Value)[1]));
                        }
                        xRow.Add(xCol);
                    }
                    talentTree.Add(xRow);
                }
                component.Add(talentTree);
            }
            return component;
        }
    }
}
