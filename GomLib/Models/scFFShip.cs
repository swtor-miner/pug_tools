using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class scFFShip : PseudoGameObject, IEquatable<scFFShip>
    {
        public AbilityPackage abilityPackage { get; set; }
        public string Category { get; set; }
        public Dictionary<string, List<scFFColorOption>> ColorOptions { get; set; }
        public Dictionary<string, List<scFFComponent>> ComponentMap { get; set; }
        public long Cost { get; set; }
        public ulong damagedPackageNodeId { get; set; }
        public Dictionary<string, ulong> DefaultLoadout { get; set; }
        public string Description { get; set; }
        public long DescriptionId { get; set; }
        public ulong engStatsNodeId { get; set; }
        public ulong EppDynamicCollectionId { get; set; }
        public string Faction { get; set; }
        public string Icon { get; set; }
        //public long Id { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeprecated { get; set; }
        public bool IsHidden { get; set; }
        public bool IsPurchasedWithCC { get; set; }
        public long LookupId { get; set; }
        public ulong majorComponentsContainerId { get; set; }
        public Dictionary<string, long> MajorComponentSlots { get; set; }
        public string majorEquipType { get; set; }
        public ulong minorComponentsContainerId { get; set; }
        public Dictionary<string, long> MinorComponentSlots { get; set; }
        public string minorEquipType { get; set; }
        public string Model { get; set; }
        //public string Name { get; set; }
        public long NameId { get; set; }
        public List<scFFPattern> PatternOptions { get; set; }
        public string ShipIcon { get; set; }
        public Dictionary<string, float> Stats { get; set; }
        public float unknownStat1 { get; set; }
        public float unknownStat2 { get; set; }
        public float unknownStat3 { get; set; }
        public float unknownStat4 { get; set; }
        public float unknownStat5 { get; set; }
        public float unknownStat6 { get; set; }
        public float unknownStat7 { get; set; }
        public float unknownStat8 { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            scFFShip shp = obj as scFFShip;
            if (shp == null) return false;

            return Equals(shp);
        }

        public bool Equals(scFFShip shp)
        {
            if (shp == null) return false;

            if (ReferenceEquals(this, shp)) return true;

            if (this.abilityPackage != null)
            {
                if (shp.abilityPackage != null)
                {
                    if (!this.abilityPackage.Equals(shp.abilityPackage))
                        return false;
                }
                else
                    return false;
            }
            else if (shp.abilityPackage != null)
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
                        List<scFFComponent> oldSlot;
                        shp.ComponentMap.TryGetValue(kvp.Key, out oldSlot);
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
            if (this.damagedPackageNodeId != shp.damagedPackageNodeId)
                return false;

            var suComp = new DictionaryComparer<string, ulong>();
            if (!suComp.Equals(this.DefaultLoadout, shp.DefaultLoadout))
                return false;

            if (this.Description != shp.Description)
                return false;
            if (this.DescriptionId != shp.DescriptionId)
                return false;
            if (this.engStatsNodeId != shp.engStatsNodeId)
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
            if (this.majorComponentsContainerId != shp.majorComponentsContainerId)
                return false;

            var slComp = new DictionaryComparer<string, long>();
            if (!slComp.Equals(this.MajorComponentSlots, shp.MajorComponentSlots))
                return false;

            if (this.majorEquipType != shp.majorEquipType)
                return false;
            if (this.minorComponentsContainerId != shp.minorComponentsContainerId)
                return false;
            if (!slComp.Equals(this.MinorComponentSlots, shp.MinorComponentSlots))
                return false;
            if (this.minorEquipType != shp.minorEquipType)
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
                        var pat = shp.PatternOptions.Where( x => x.Id == this.PatternOptions[i].Id);
                        if (! this.PatternOptions[i].Equals(pat.Single()))
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

            if (this.unknownStat1 != shp.unknownStat1)
                return false;
            if (this.unknownStat2 != shp.unknownStat2)
                return false;
            if (this.unknownStat3 != shp.unknownStat3)
                return false;
            if (this.unknownStat4 != shp.unknownStat4)
                return false;
            if (this.unknownStat5 != shp.unknownStat5)
                return false;
            if (this.unknownStat6 != shp.unknownStat6)
                return false;
            if (this.unknownStat7 != shp.unknownStat7)
                return false;
            if (this.unknownStat8 != shp.unknownStat8)
                return false;
            return true;
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
                    stats.Add(Stats.Select(x => new XElement("Stat", new XAttribute("Id", _dom.statData.ToStat(x.Key)), x.Value)));
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
                    shipContainer.Add(new XElement("s1", unknownStat1),
                        new XElement("s2", unknownStat2),
                        new XElement("s3", unknownStat3),
                        new XElement("s4", unknownStat4),
                        new XElement("s5", unknownStat5),
                        new XElement("s6", unknownStat6),
                        new XElement("s7", unknownStat7),
                        new XElement("s8", unknownStat8));
                }
            }
            return shipContainer;
        }

        private XElement ContainerToXElement(Dictionary<string, long> containerMap,
            Dictionary<string, List<GomLib.Models.scFFComponent>> componentMap,
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
                            foreach (GomLib.Models.scFFComponent comp in componentMap[containerMapSlot.Key])
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
