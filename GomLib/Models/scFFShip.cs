using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class ScFFShip : PseudoGameObject, IEquatable<ScFFShip>
    {
        private AbilityPackage abilityPackage;

        public AbilityPackage GetAbilityPackage()
        {
            return abilityPackage;
        }

        public void SetAbilityPackage(AbilityPackage value)
        {
            abilityPackage = value;
        }

        public string AbilityPackageB62Id
        {
            get
            {
                if (GetAbilityPackage() != null)
                    return GetAbilityPackage().Id.ToMaskedBase62();
                return null;
            }
        }
        public string Category { get; set; }
        public Dictionary<string, List<ScFFColorOption>> ColorOptions { get; set; }
        public Dictionary<string, List<ScFFComponent>> ComponentMap { get; set; }
        public long Cost { get; set; }

        private ulong damagedPackageNodeId;

        public ulong GetDamagedPackageNodeId()
        {
            return DamagedPackageNodeId;
        }

        public void SetDamagedPackageNodeId(ulong value)
        {
            DamagedPackageNodeId = value;
        }

        public Dictionary<string, ulong> DefaultLoadout { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long DescriptionId { get; set; }

        private ulong engStatsNodeId;

        public ulong GetEngStatsNodeId()
        {
            return EngStatsNodeId;
        }

        public void SetEngStatsNodeId(ulong value)
        {
            EngStatsNodeId = value;
        }

        [JsonConverter(typeof(ULongConverter))]
        public ulong EppDynamicCollectionId { get; set; }
        public string Faction { get; set; }
        public string Icon { get; set; }
        //public long Id { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeprecated { get; set; }
        public bool IsHidden { get; set; }
        public bool IsPurchasedWithCC { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public long LookupId { get; set; }

        private ulong majorComponentsContainerId;

        public ulong GetMajorComponentsContainerId()
        {
            return MajorComponentsContainerId;
        }

        public void SetMajorComponentsContainerId(ulong value)
        {
            MajorComponentsContainerId = value;
        }

        public Dictionary<string, long> MajorComponentSlots { get; set; }

        private string majorEquipType;

        public string GetMajorEquipType()
        {
            return majorEquipType;
        }

        public void SetMajorEquipType(string value)
        {
            majorEquipType = value;
        }

        private ulong minorComponentsContainerId;

        public ulong GetMinorComponentsContainerId()
        {
            return minorComponentsContainerId;
        }

        public void SetMinorComponentsContainerId(ulong value)
        {
            minorComponentsContainerId = value;
        }

        private Dictionary<string, long> minorComponentSlots;

        public Dictionary<string, long> MinorComponentSlots => minorComponentSlots;

        public void SetMinorComponentSlots(Dictionary<string, long> value) => minorComponentSlots = value;

        private string minorEquipType;

        public string GetMinorEquipType()
        {
            return MinorEquipType;
        }

        public void SetMinorEquipType(string value)
        {
            MinorEquipType = value;
        }

        public string Model { get; set; }
        //public string Name { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public List<ScFFPattern> PatternOptions { get; set; }
        public string ShipIcon { get; set; }
        public Dictionary<string, float> Stats { get; set; }

        private float unknownStat1;

        public float GetUnknownStat1()
        {
            return UnknownStat1;
        }

        public void SetUnknownStat1(float value)
        {
            UnknownStat1 = value;
        }

        private float unknownStat2;

        public float GetUnknownStat2()
        {
            return UnknownStat2;
        }

        public void SetUnknownStat2(float value)
        {
            UnknownStat2 = value;
        }

        private float unknownStat3;

        public float GetUnknownStat3()
        {
            return UnknownStat3;
        }

        public void SetUnknownStat3(float value)
        {
            UnknownStat3 = value;
        }

        private float unknownStat4;

        public float GetUnknownStat4()
        {
            return UnknownStat4;
        }

        public void SetUnknownStat4(float value)
        {
            UnknownStat4 = value;
        }

        private float unknownStat5;

        public float GetUnknownStat5()
        {
            return UnknownStat5;
        }

        public void SetUnknownStat5(float value)
        {
            UnknownStat5 = value;
        }

        private float unknownStat6;

        public float GetUnknownStat6()
        {
            return UnknownStat6;
        }

        public void SetUnknownStat6(float value)
        {
            UnknownStat6 = value;
        }

        private float unknownStat7;

        public float GetUnknownStat7()
        {
            return UnknownStat7;
        }

        public void SetUnknownStat7(float value)
        {
            UnknownStat7 = value;
        }

        private float unknownStat8;

        public float GetUnknownStat8()
        {
            return UnknownStat8;
        }

        public void SetUnknownStat8(float value)
        {
            UnknownStat8 = value;
        }

        public override long Id { get => base.Id; set => base.Id = value; }
        public override string Name { get => base.Name; set => base.Name = value; }
        public override List<SQLProperty> SQLProperties { get => base.SQLProperties; set => base.SQLProperties = value; }
        public ulong DamagedPackageNodeId { get => damagedPackageNodeId; set => damagedPackageNodeId = value; }
        public ulong EngStatsNodeId { get => engStatsNodeId; set => engStatsNodeId = value; }
        public ulong MajorComponentsContainerId { get => majorComponentsContainerId; set => majorComponentsContainerId = value; }
        public string MinorEquipType { get => minorEquipType; set => minorEquipType = value; }
        public float UnknownStat1 { get => unknownStat1; set => unknownStat1 = value; }
        public float UnknownStat2 { get => unknownStat2; set => unknownStat2 = value; }
        public float UnknownStat3 { get => unknownStat3; set => unknownStat3 = value; }
        public float UnknownStat4 { get => unknownStat4; set => unknownStat4 = value; }
        public float UnknownStat5 { get => unknownStat5; set => unknownStat5 = value; }
        public float UnknownStat6 { get => unknownStat6; set => unknownStat6 = value; }
        public float UnknownStat7 { get => unknownStat7; set => unknownStat7 = value; }
        public float UnknownStat8 { get => unknownStat8; set => unknownStat8 = value; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is ScFFShip shp)) return false;

            return Equals(shp);
        }

        public bool Equals(ScFFShip shp)
        {
            if (shp == null) return false;

            if (ReferenceEquals(this, shp)) return true;

            if (this.GetAbilityPackage() != null)
            {
                if (shp.GetAbilityPackage() != null)
                {
                    if (!this.GetAbilityPackage().Equals(shp.GetAbilityPackage()))
                        return false;
                }
                else
                    return false;
            }
            else if (shp.GetAbilityPackage() != null)
                return false;
            if (this.Category != shp.Category)
                return false;
            if (this.ColorOptions != null)
            {
                if (shp.ColorOptions != null)
                {
                    if (!this.ColorOptions.Keys.SequenceEqual(shp.ColorOptions.Keys))
                        return false;
                    foreach (var kvp in this.ColorOptions)
                    {
                        if (!kvp.Value.SequenceEqual(shp.ColorOptions[kvp.Key]))
                            return false;
                    }
                }
                else
                    return false;
            }
            if (this.ComponentMap != null)
            {
                if (shp.ComponentMap != null)
                {
                    if (!this.ComponentMap.Keys.SequenceEqual(shp.ComponentMap.Keys))
                        return false;
                    foreach (var kvp in this.ComponentMap)
                    {
                        shp.ComponentMap.TryGetValue(kvp.Key, out List<ScFFComponent> oldSlot);
                        if (kvp.Value.Count != oldSlot.Count)
                            return false;
                        foreach (var comp in kvp.Value)
                        {
                            var oldComp = oldSlot.Where(x => x.Id == comp.Id);
                            if (oldComp == null)
                                return false;
                            if (!comp.Equals(oldComp.Single()))
                                return false;
                        }

                    }
                }
                else
                    return false;
            }
            if (this.Cost != shp.Cost)
                return false;
            if (this.GetDamagedPackageNodeId() != shp.GetDamagedPackageNodeId())
                return false;

            var suComp = new DictionaryComparer<string, ulong>();
            if (!suComp.Equals(this.DefaultLoadout, shp.DefaultLoadout))
                return false;

            if (this.Description != shp.Description)
                return false;
            if (this.DescriptionId != shp.DescriptionId)
                return false;
            if (this.GetEngStatsNodeId() != shp.GetEngStatsNodeId())
                return false;
            if (this.EppDynamicCollectionId != shp.EppDynamicCollectionId)
                return false;
            if (this.Faction != shp.Faction)
                return false;
            if (this.Icon != shp.Icon)
                return false;
            if (this.Id != shp.Id)
                return false;
            if (this.IsAvailable != shp.IsAvailable)
                return false;
            if (this.IsDeprecated != shp.IsDeprecated)
                return false;
            if (this.IsHidden != shp.IsHidden)
                return false;
            if (this.IsPurchasedWithCC != shp.IsPurchasedWithCC)
                return false;
            if (this.LookupId != shp.LookupId)
                return false;
            if (this.GetMajorComponentsContainerId() != shp.GetMajorComponentsContainerId())
                return false;

            var slComp = new DictionaryComparer<string, long>();
            if (!slComp.Equals(this.MajorComponentSlots, shp.MajorComponentSlots))
                return false;

            if (this.GetMajorEquipType() != shp.GetMajorEquipType())
                return false;
            if (this.GetMinorComponentsContainerId() != shp.GetMinorComponentsContainerId())
                return false;
            if (!slComp.Equals(this.MinorComponentSlots, shp.MinorComponentSlots))
                return false;
            if (this.GetMinorEquipType() != shp.GetMinorEquipType())
                return false;
            if (this.Model != shp.Model)
                return false;
            if (this.Name != shp.Name)
                return false;
            if (this.NameId != shp.NameId)
                return false;
            if (this.PatternOptions != null)
            {
                if (shp.PatternOptions != null)
                {
                    if (this.PatternOptions.Count != shp.PatternOptions.Count)
                        return false;
                    for (int i = 0; i < this.PatternOptions.Count; i++)
                    {
                        var pat = shp.PatternOptions.Where(x => x.Id == this.PatternOptions[i].Id);
                        if (!this.PatternOptions[i].Equals(pat.Single()))
                            return false;
                    }
                }
                else
                    return false;
            }
            if (this.ShipIcon != shp.ShipIcon)
                return false;

            var sfComp = new DictionaryComparer<string, float>();
            if (!sfComp.Equals(this.Stats, shp.Stats))
                return false;

            if (this.GetUnknownStat1() != shp.GetUnknownStat1())
                return false;
            if (this.GetUnknownStat2() != shp.GetUnknownStat2())
                return false;
            if (this.GetUnknownStat3() != shp.GetUnknownStat3())
                return false;
            if (this.GetUnknownStat4() != shp.GetUnknownStat4())
                return false;
            if (this.GetUnknownStat5() != shp.GetUnknownStat5())
                return false;
            if (this.GetUnknownStat6() != shp.GetUnknownStat6())
                return false;
            if (this.GetUnknownStat7() != shp.GetUnknownStat7())
                return false;
            if (this.GetUnknownStat8() != shp.GetUnknownStat8())
                return false;
            return true;
        }

        public override HashSet<string> GetDependencies()
        {
            return base.GetDependencies();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override string ToString(bool verbose)
        {
            return base.ToString(verbose);
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement shipContainer = new XElement("Ship");
            if (this.Id != 0)
            {
                string currency = " Fleet Requisition";
                if (IsPurchasedWithCC) currency = " ???";
                shipContainer.Add(new XElement("Name", Name),
                    new XAttribute("Id", Id),
                    new XElement("Description", Description),
                    new XElement("Faction", Faction),
                    new XElement("Category", Category),
                    new XElement("IsAvailable_IsDeprecated_IsHidden", IsAvailable + "," + IsDeprecated + "," + IsHidden),
                    new XElement("Cost", Cost + currency));
                if (verbose)
                {
                    shipContainer.Add(//new XElement("Fqn", Fqn,
                                      //new XAttribute("NodeId", NodeId)),
                                      //new XAttribute("Hash", GetHashCode()),
                    new XElement("Tag", Icon),
                    new XElement("ShipIcon", ShipIcon),
                    new XElement("Model", Model));
                    foreach (var optionContainer in ColorOptions)
                    {
                        XElement colorOptions = new XElement("ColorOption",
                            new XAttribute("Id", optionContainer.Key.ToString().Replace("scFFColorOption", "")));
                        foreach (var colorOption in optionContainer.Value)
                        {
                            colorOptions.Add(new XElement("Color", new XElement("Name", colorOption.Name,
                                new XAttribute("Id", colorOption.NameId)),
                                new XElement("Icon", colorOption.Icon),
                                new XElement("IsAvailable", colorOption.IsAvailable),
                                //new XElement("Type", colorOption.type), //unneeded
                                new XAttribute("Id", colorOption.ShortId)));
                        }
                        shipContainer.Add(colorOptions);
                    }
                    XElement patternOptions = new XElement("Patterns");
                    foreach (var pattern in PatternOptions)
                    {
                        patternOptions.Add(new XElement("Pattern", new XElement("Name", pattern.Name,
                              new XAttribute("Id", pattern.NameId)),
                              new XElement("Icon", pattern.Icon),
                              new XElement("IsAvailable", pattern.IsAvailable),
                              new XElement("Texture", pattern.TextureForCurrentShip),
                              //new XElement("Type", pattern.type), //unneeded
                              new XAttribute("Id", pattern.ShortId)));
                    }
                    shipContainer.Add(patternOptions);
                }
                XElement stats = new XElement("Stats");
                if (Stats != null)
                {
                    //stats.Add("[ " + string.Join("; ", Stats.Select(x => currentDom.statD.ToStat(x.Key) + ", " + x.Value).ToArray()) + "; ]");
                    stats.Add(Stats.Select(x => new XElement("Stat", new XAttribute("Id", ((Dom_.statData.ToStat(x.Key)) ?? new DetailedStat()).DisplayName ?? x.Key), x.Value)));
                    if (!verbose)
                    {
                        stats.Elements().Where(x => x.Attribute("Id").Value.Contains("4611") || x.Attribute("Id").Value.Contains("OBSOLETE")).Remove();
                    }
                }
                shipContainer.Add(stats);
                shipContainer.Add(ContainerToXElement(MajorComponentSlots, ComponentMap, "MajorSlot", DefaultLoadout, verbose).Elements());
                shipContainer.Add(ContainerToXElement(MinorComponentSlots, ComponentMap, "MinorSlot", DefaultLoadout, verbose).Elements());

                if (verbose)
                {
                    shipContainer.Add(new XElement("s1", GetUnknownStat1()),
                        new XElement("s2", GetUnknownStat2()),
                        new XElement("s3", GetUnknownStat3()),
                        new XElement("s4", GetUnknownStat4()),
                        new XElement("s5", GetUnknownStat5()),
                        new XElement("s6", GetUnknownStat6()),
                        new XElement("s7", GetUnknownStat7()),
                        new XElement("s8", GetUnknownStat8()));
                }
            }
            return shipContainer;
        }

        public override XElement ToXElement()
        {
            return base.ToXElement();
        }

        private XElement ContainerToXElement(Dictionary<string, long> containerMap,
            Dictionary<string, List<GomLib.Models.ScFFComponent>> componentMap,
            string containerName,
            Dictionary<string, ulong> defaultLoadoutMap,
            bool verbose)
        {
            XElement container = new XElement(containerName);
            if (containerMap != null)
            {
                int c = 1;
                foreach (var containerMapSlot in containerMap)
                {
                    XElement subContainer = new XElement(containerName, new XAttribute("Name", containerMapSlot.Key), new XAttribute("Id", c), new XAttribute("NumSlots", containerMapSlot.Value.ToString()));
                    if (componentMap != null)
                    {
                        if (componentMap.ContainsKey(containerMapSlot.Key))
                        {
                            foreach (GomLib.Models.ScFFComponent comp in componentMap[containerMapSlot.Key])
                            {
                                bool isDefault = ((ulong)defaultLoadoutMap[containerMapSlot.Key] == comp.Id);
                                /* code moved to GomLib.Models.scFFComponent.cs */
                                subContainer.Add(comp.ToXElement(isDefault, verbose));
                            }
                        }
                    }
                    container.Add(subContainer);
                    c++;
                }
            }
            return container;
        }
    }
}
