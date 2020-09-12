using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class ScFFComponent : GameObject, IEquatable<ScFFComponent>
    {
        public string ColorOption { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public long ComponentId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public ulong ControllerNodeId { get; set; }
        public int Cost { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public ulong CostLookupId { get; set; }
        public string Description { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public long DescriptionId { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        //public string Fqn { get; set; }
        public string Icon { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeprecated { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public long NameId { get; set; }
        //public ulong Id { get; set; }
        public long NumUpgradeTiers { get; set; }
        public string Slot { get; set; }
        public Dictionary<string, float> StatsList { get; set; }
        public Dictionary<int, int> TalentCostList { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<int, GameObject> TalentList; // this should only be used when the TalentTree is empty
        public ScFFTalentTree Talents;
        [Newtonsoft.Json.JsonIgnore]
        public long UsedByShipId { get; set; }
        public long UnknownId { get; set; }
        public override List<SQLProperty> SQLProperties { get => base.SQLProperties; set => base.SQLProperties = value; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is ScFFComponent scp)) return false;

            return Equals(scp);
        }

        public bool Equals(ScFFComponent scp)
        {
            if (scp == null) return false;

            if (ReferenceEquals(this, scp)) return true;

            if (ColorOption != scp.ColorOption)
                return false;
            if (ComponentId != scp.ComponentId)
                return false;
            if (ControllerNodeId != scp.ControllerNodeId)
                return false;
            if (Cost != scp.Cost)
                return false;
            if (CostLookupId != scp.CostLookupId)
                return false;
            if (Description != scp.Description)
                return false;
            if (DescriptionId != scp.DescriptionId)
                return false;
            if (Fqn != scp.Fqn)
                return false;
            if (Icon != scp.Icon)
                return false;
            if (Id != scp.Id)
                return false;
            if (IsAvailable != scp.IsAvailable)
                return false;
            if (IsDeprecated != scp.IsDeprecated)
                return false;
            if (Model != scp.Model)
                return false;
            if (Name != scp.Name)
                return false;
            if (NameId != scp.NameId)
                return false;
            if (NumUpgradeTiers != scp.NumUpgradeTiers)
                return false;
            if (Slot != scp.Slot)
                return false;

            var sfComp = new DictionaryComparer<string, float>();
            if (!sfComp.Equals(StatsList, scp.StatsList))
                return false;

            var iiComp = new DictionaryComparer<int, int>();
            if (!iiComp.Equals(TalentCostList, scp.TalentCostList))
                return false;

            if (TalentList != null)
            {
                if (scp.TalentList != null)
                {
                    if (!TalentList.Keys.SequenceEqual(scp.TalentList.Keys))
                        return false;
                    foreach (var kvp in TalentList)
                    {
                        if (!kvp.Value.Equals(scp.TalentList[kvp.Key]))
                            return false;
                    }
                }
                else
                    return false;
            }

            if (!Talents.Equals(scp.Talents))
                return false;
            if (UnknownId != scp.UnknownId)
                return false;
            if (UsedByShipId != scp.UsedByShipId)
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
            XElement stats = new XElement("Stats");
            if (StatsList != null)
            {
                stats.Add(StatsList.Select(x => new XElement("Stat", new XAttribute("Id", ((Dom_.statData.ToStat(x.Key)) ?? new DetailedStat()).DisplayName ?? x.Key), x.Value)));
                if (!verbose)
                {
                    stats.Elements().Where(x => x.Attribute("Id").Value.Contains("4611") || x.Attribute("Id").Value.Contains("OBSOLETE")).Remove();
                }
            }
            component.Add(stats);
            if (TalentList.ContainsKey(0))
            {
                var type = TalentList[0].GetType();
                if (type.Name == "Talent")
                {
                    if (verbose) component.Add(new XElement("Ability"));
                    component.Add(((Talent)TalentList[0]).ToXElement(verbose));
                }
                else
                {
                    component.Add(((Ability)TalentList[0]).ToXElement(verbose));
                    component.Add(new XElement("Talent"));
                }
            }
            if (verbose)
            {
                XElement talentTree = new XElement("TalentTree", new XAttribute("NumUpgdTiers", NumUpgradeTiers));
                //if(Talents.Ability != null)
                //talentTree.Add(Talents.Ability.ToXElement(verbose));
                foreach (var row in Talents.Tree)
                {
                    XElement xRow = new XElement("Row", new XAttribute("Id", row.Key));
                    foreach (var column in row.Value)
                    {
                        XElement xCol = null; //new XElement("Column"); 
                        if (column.Value[0].GetType() == typeof(Talent))
                        {
                            xCol = ((Talent)column.Value[0]).ToXElement(verbose);
                        }
                        else
                        {
                            xCol = ((Ability)column.Value[0]).ToXElement(verbose);
                        }
                        xCol.Add(new XAttribute("Priority", column.Key));
                        if (column.Value[1] == null)
                        {
                            xCol.Add(new XAttribute("Target", "All"));
                        }
                        else
                        {
                            xCol.Add(new XAttribute("Target", column.Value[1]));
                        }
                        xRow.Add(xCol);
                    }
                    talentTree.Add(xRow);
                }
                component.Add(talentTree);
            }
            return component;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToJSON()
        {
            return base.ToJSON();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override string ToString(bool verbose)
        {
            return base.ToString(verbose);
        }

        public override XElement ToXElement(GomObject gomItm)
        {
            return base.ToXElement(gomItm);
        }

        public override XElement ToXElement(GomObject gomItm, bool verbose)
        {
            return base.ToXElement(gomItm, verbose);
        }

        public override XElement ToXElement(ulong nodeId, DataObjectModel dom)
        {
            return base.ToXElement(nodeId, dom);
        }

        public override XElement ToXElement(ulong nodeId, DataObjectModel dom, bool verbose)
        {
            return base.ToXElement(nodeId, dom, verbose);
        }

        public override XElement ToXElement(string fqn, DataObjectModel dom)
        {
            return base.ToXElement(fqn, dom);
        }

        public override XElement ToXElement(string fqn, DataObjectModel dom, bool verbose)
        {
            return base.ToXElement(fqn, dom, verbose);
        }

        public override XElement ToXElement()
        {
            return base.ToXElement();
        }

        public override HashSet<string> GetDependencies()
        {
            return base.GetDependencies();
        }
    }
}
