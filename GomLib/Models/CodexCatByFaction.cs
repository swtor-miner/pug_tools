using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class CodexCatByFaction: PseudoGameObject, IEquatable<CodexCatByFaction>
    {
        public string FactionName { get; set; }
        public Dictionary<string, Dictionary<string, long>> CdxOverviewByClass { get; set; }
        public Dictionary<string, Dictionary<string, Dictionary<string, long>>> PlanetValueByPlanetNameByCatNameByClassName { get; set; }
        public Dictionary<string, Dictionary<string, Dictionary<string, long>>> PlanetCatTotalByPlanetNameByCatNameByClassName { get; set; }
        public Dictionary<string, long> ClassCodexTotals { get; set; }

        public CodexCatByFaction()
        {
            ClassCodexTotals = new Dictionary<string, long>();
        }

        public override bool Equals(object entry)
        {
            if (entry == null)
                return false;
            if (ReferenceEquals(this, entry))
                return true;

            CodexCatByFaction factionTotals = entry as CodexCatByFaction;
            return Equals(factionTotals);
        }

        public bool Equals(CodexCatByFaction entry)
        {
            if (entry == null)
                return false;
            if (ReferenceEquals(this, entry))
                return true;
            if (Id != entry.Id)
                return false;
            if (FactionName != entry.FactionName)
                return false;
            if (!CdxOverviewByClass.SequenceEqual(entry.CdxOverviewByClass))
                return false;
            if (!ClassCodexTotals.SequenceEqual(entry.ClassCodexTotals))
                return false;
            if (!PlanetValueByPlanetNameByCatNameByClassName.SequenceEqual(entry.PlanetValueByPlanetNameByCatNameByClassName))
                return false;
            if (!PlanetCatTotalByPlanetNameByCatNameByClassName.SequenceEqual(entry.PlanetCatTotalByPlanetNameByCatNameByClassName))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= FactionName.GetHashCode();
            foreach(KeyValuePair<string, long> kvp in ClassCodexTotals)
            {
                hash ^= kvp.Key.GetHashCode();
                hash ^= kvp.Value.GetHashCode();
            }

            foreach (KeyValuePair<string, Dictionary<string, long>> cdxOverviewClass in CdxOverviewByClass)
            {
                hash ^= cdxOverviewClass.Key.GetHashCode();
                foreach (KeyValuePair<string, long> planetTotalkvp in cdxOverviewClass.Value)
                {
                    hash ^= planetTotalkvp.Key.GetHashCode();
                    hash ^= planetTotalkvp.Value.GetHashCode();
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, long>>> classCatDetailskvp in PlanetValueByPlanetNameByCatNameByClassName)
            {
                hash ^= classCatDetailskvp.Key.GetHashCode();
                foreach (KeyValuePair<string, Dictionary<string, long>> planetTotalsByCatNamekvp in classCatDetailskvp.Value)
                {
                    hash ^= planetTotalsByCatNamekvp.Key.GetHashCode();
                    foreach (KeyValuePair<string, long> planetValueByNamein in planetTotalsByCatNamekvp.Value)
                    {
                        hash ^= planetValueByNamein.Key.GetHashCode();
                        hash ^= planetValueByNamein.Value.GetHashCode();
                    }
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, long>>> classPlanetDetailskvp in PlanetCatTotalByPlanetNameByCatNameByClassName)
            {
                hash ^= classPlanetDetailskvp.Key.GetHashCode();
                foreach (KeyValuePair<string, Dictionary<string, long>> catDetailsDictByPlanetNamekvp in classPlanetDetailskvp.Value)
                {
                    hash ^= catDetailsDictByPlanetNamekvp.Key.GetHashCode();
                    foreach (KeyValuePair<string, long> catValueByName in catDetailsDictByPlanetNamekvp.Value)
                    {
                        catValueByName.Key.GetHashCode();
                        catValueByName.Value.GetHashCode();
                    }
                }
            }

            return hash;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement factionElem = new XElement("Faction", new XAttribute("Id", FactionName));
            XElement classTotalsElem = new XElement("CodexTotals");
            foreach(KeyValuePair<string, long> ClassTotals in ClassCodexTotals)
            {
                classTotalsElem.Add(new XElement("Class", new XAttribute("Id", ClassTotals.Key), ClassTotals.Value));
            }
            factionElem.Add(classTotalsElem);

            XElement cdxOverviewElement = new XElement("CodexOverview");
            foreach(KeyValuePair<string, Dictionary<string, long>> cdxOverviewClass in CdxOverviewByClass)
            {
                XElement classOverview = new XElement("Class", new XAttribute("Id", cdxOverviewClass.Key));
                foreach(KeyValuePair<string, long> planetTotalkvp in cdxOverviewClass.Value)
                {
                    classOverview.Add(new XElement("Planet", new XAttribute("Id", planetTotalkvp.Key), planetTotalkvp.Value));
                }
                cdxOverviewElement.Add(classOverview);
            }
            factionElem.Add(cdxOverviewElement);

            XElement catTotals = new XElement("CategoryPlanetTotals");
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, long>>> classCatDetailskvp in PlanetValueByPlanetNameByCatNameByClassName)
            {
                XElement classElem = new XElement("Class", new XAttribute("Id", classCatDetailskvp.Key));
                foreach(KeyValuePair<string, Dictionary<string, long>> planetTotalsByCatNamekvp in classCatDetailskvp.Value)
                {
                    XElement catElem = new XElement("Category", new XAttribute("Id", planetTotalsByCatNamekvp.Key));
                    foreach(KeyValuePair<string, long> planetValueByNamein in planetTotalsByCatNamekvp.Value)
                    {
                        catElem.Add(new XElement("Planet", new XAttribute("Id", planetValueByNamein.Key), planetValueByNamein.Value));
                    }
                    classElem.Add(catElem);
                }
                catTotals.Add(classElem);
            }
            factionElem.Add(catTotals);

            XElement catContribs = new XElement("PlanetCodexCategorySource");
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, long>>> classPlanetDetailskvp in PlanetCatTotalByPlanetNameByCatNameByClassName)
            {
                XElement classElem = new XElement("Class", new XAttribute("Id", classPlanetDetailskvp.Key));
                foreach(KeyValuePair<string, Dictionary<string, long>> catDetailsDictByPlanetNamekvp in classPlanetDetailskvp.Value)
                {
                    XElement planetElem = new XElement("Planet", new XAttribute("Id", catDetailsDictByPlanetNamekvp.Key));
                    foreach(KeyValuePair<string, long> catValueByName in catDetailsDictByPlanetNamekvp.Value)
                    {
                        planetElem.Add(new XElement("Category", new XAttribute("Id", catValueByName.Key), catValueByName.Value));
                    }
                    classElem.Add(planetElem);
                }
                catContribs.Add(classElem);
            }
            factionElem.Add(catContribs);

            return factionElem;
        }
    }
}
