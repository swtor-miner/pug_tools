using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GomLib.ModelLoader
{
    public class CodexCatByFactionLoader
    {
        static long EmpireFaction = -1855280666668608219;
        static long RepublicFaction = 1086966210362573345;
        DataObjectModel _dom;

        public CodexCatByFactionLoader(DataObjectModel dom)
        {
            _dom = dom;
        }

        public Models.CodexCatByFaction Load(Models.CodexCatByFaction factionCats, long Id, Dictionary<object, object> objData)
        {
            if (objData == null)
                return factionCats;
            if (factionCats == null)
                return null;

            //Check if the faction we are handling is the empire or not.
            bool factionIsEmpire = (Id == EmpireFaction);
            factionCats.Id = Id;
            factionCats._dom = _dom;
            factionCats.Prototype = "cdxCategoryTotalsPrototype";
            factionCats.ProtoDataTable = "cdxFactionToClassToPlanetToTotalLookupList";
            
            if(factionIsEmpire)
            {
                factionCats.FactionName = "Empire";
            }
            else
            {
                factionCats.FactionName = "Republic";
            }

            //Lets get the data for the Codex Overview Totals
            Dictionary<string, Dictionary<string, long>> classCdxOverview = new Dictionary<string, Dictionary<string, long>>();
            foreach(KeyValuePair<object, object> kvp in objData)
            {
                ulong classNodeID = (ulong)kvp.Key;
                if(factionIsEmpire != BaseClassNodeIsEmpire(classNodeID))
                {
                    //If the faction doesn't match the one we are working with then skip it.
                    continue;
                }
                //The dictionary we store the overview totals in for this class.
                Dictionary<string, long> OverviewTotals = new Dictionary<string, long>();
                string className = GetClassNameFromNode(classNodeID);

                Dictionary<object, object> classCdxOverviewTotals = kvp.Value as Dictionary<object, object>;
                foreach(KeyValuePair<object, object> classCdxOverviewKvp in classCdxOverviewTotals)
                {
                    ulong cdxNodeID = (ulong)classCdxOverviewKvp.Key;
                    long cdxTotal = (long)classCdxOverviewKvp.Value;
                    if(cdxNodeID == 16140950445578029361)
                    {
                        OverviewTotals.Add("UnknownTotal", cdxTotal);
                    }
                    else if(cdxNodeID == 16141057659009624245)
                    {
                        OverviewTotals.Add("UnknownTotalTwo", cdxTotal);
                    }
                    else
                    {
                        Models.Codex cdx = _dom.codexLoader.Load(cdxNodeID);
                        if(cdx == null)
                        {
                            continue;
                        }

                        string faction = cdx.Faction.ToString();
                        if(faction != "None")
                        {
                            //Determine if this is a planet we should skip or not because it's not part of our faction.
                            if(faction == "Empire" && !factionIsEmpire)
                            {
                                continue;
                            } else if(faction == "Republic" && factionIsEmpire)
                            {
                                continue;
                            }
                        }

                        //Planet name, value.
                        OverviewTotals.Add(cdx.Name, cdxTotal);
                    }
                }
                classCdxOverview.Add(className, OverviewTotals);
            }
            factionCats.CdxOverviewByClass = classCdxOverview;

            //Get the string table for categories and 
            StringTable catStb = _dom.stringTable.Find("str.sys.codexcategories");
            GomObject totalsNode = _dom.GetObject("cdxCategoryTotalsPrototype");

            //Parse the cdxFactionToClassToCategoryToPlanetToTotalLookupList table. I heard you like foreach..
            Dictionary<object, object> catTotalsData = totalsNode.Data.Get<Dictionary<object, object>>("cdxFactionToClassToCategoryToPlanetToTotalLookupList");
            foreach(KeyValuePair<object, object> catTotalsKvp in catTotalsData)
            {
                long factionID = (long)catTotalsKvp.Key;
                if(Id != factionID)
                {
                    //If the faction ID isn't the one we are dealing with right now then skip it.
                    continue;
                }

                //Key is class node id, value is dictionary cat str id, values dict.
                Dictionary<object, object> classDicts = catTotalsKvp.Value as Dictionary<object, object>;
                //Holds the category data by class name
                Dictionary<string, Dictionary<string, Dictionary<string, long>>> catNameValueDictByClassName =
                        new Dictionary<string, Dictionary<string, Dictionary<string, long>>>();
                foreach(KeyValuePair<object, object> CatIdValueDictByCatStrIdKvp in classDicts)
                {
                    //If the class isn't the same faction as we are handling then skip.
                    ulong classNodeID = (ulong)CatIdValueDictByCatStrIdKvp.Key;
                    bool isClassEmpire = BaseClassNodeIsEmpire(classNodeID);
                    if(isClassEmpire && !factionIsEmpire)
                    {
                        continue;
                    } else if(!isClassEmpire && factionIsEmpire)
                    {
                        continue;
                    }
                    string className = GetClassNameFromNode(classNodeID);

                    //Create the dictionary to hold the totals for this class and get the data and start iterating through it.
                    Dictionary<string, Dictionary<string, long>> catValueByNameDict = new Dictionary<string, Dictionary<string, long>>();
                    Dictionary<object, object> catValuesDictBySID = CatIdValueDictByCatStrIdKvp.Value as Dictionary<object, object>;
                    foreach(KeyValuePair<object, object> catValuesDictBySIDkvp in catValuesDictBySID)
                    {
                        //Create the dictionary to hold the value each cat by name.
                        string catName = catStb.GetText((long)catValuesDictBySIDkvp.Key, string.Empty);
                        if(catName.Length == 0)
                        {
                            //If we can't find the string for the codex cat then store the data anyway.
                            catName = ((long)catValuesDictBySIDkvp.Key).ToString();
                        }

                        //Create the dictionary to store the category totals by name.
                        Dictionary<string, long> cdxValueByNameDict = new Dictionary<string, long>();
                        Dictionary<object, object> cdxValueByCdxNodeIDDict = catValuesDictBySIDkvp.Value as Dictionary<object, object>;
                        foreach(KeyValuePair<object, object> cdxValueByNodeID in cdxValueByCdxNodeIDDict)
                        {
                            ulong cdxNodeID = (ulong)cdxValueByNodeID.Key;
                            if (cdxNodeID == 16140950445578029361)
                            {
                                cdxValueByNameDict.Add("Total", (long)cdxValueByNodeID.Value);
                            }
                            else if (cdxNodeID == 16141057659009624245)
                            {
                                cdxValueByNameDict.Add("UnknownNumber", (long)cdxValueByNodeID.Value);
                            }
                            else
                            {
                                Models.Codex cdx = _dom.codexLoader.Load(cdxNodeID);
                                if(cdx == null)
                                {
                                    continue;
                                }

                                string faction = cdx.Faction.ToString();
                                if (faction != "None")
                                {
                                    //Determine if this is a planet we should skip or not because it's not part of our faction.
                                    if (faction == "Empire" && !factionIsEmpire)
                                    {
                                        continue;
                                    }
                                    else if (faction == "Republic" && factionIsEmpire)
                                    {
                                        continue;
                                    }
                                }

                                //Planet name, value.
                                cdxValueByNameDict.Add(cdx.Name, (long)cdxValueByNodeID.Value);
                            }
                        }
                        catValueByNameDict.Add(catName, cdxValueByNameDict);
                    }
                    catNameValueDictByClassName.Add(className, catValueByNameDict);
                }
                factionCats.PlanetValueByPlanetNameByCatNameByClassName = catNameValueDictByClassName;
            }

            Dictionary<object, object> catTotalsByPlanet = totalsNode.Data.Get<Dictionary<object, object>>("cdxFactionToClassToPlanetToCategoryLookupList");
            foreach (KeyValuePair<object, object> catPlanetsKvp in catTotalsByPlanet)
            {
                long factionID = (long)catPlanetsKvp.Key;
                if (Id != factionID)
                {
                    //If the faction ID isn't the one we are dealing with right now then skip it.
                    continue;
                }

                //Key is class node id, value is dictionary cat str id, values dict.
                Dictionary<object, object> classDicts = catPlanetsKvp.Value as Dictionary<object, object>;
                //Holds the category data by class name
                Dictionary<string, Dictionary<string, Dictionary<string, long>>> catNameValueDictByClassName =
                        new Dictionary<string, Dictionary<string, Dictionary<string, long>>>();
                foreach (KeyValuePair<object, object> CatIdValueDictByCatStrIdKvp in classDicts)
                {
                    //If the class isn't the same faction as we are handling then skip.
                    ulong classNodeID = (ulong)CatIdValueDictByCatStrIdKvp.Key;
                    bool isClassEmpire = BaseClassNodeIsEmpire(classNodeID);
                    if (isClassEmpire && !factionIsEmpire)
                    {
                        continue;
                    }
                    else if (!isClassEmpire && factionIsEmpire)
                    {
                        continue;
                    }
                    string className = GetClassNameFromNode(classNodeID);

                    //Create the dictionary to hold the totals for this class and get the data and start iterating through it.
                    Dictionary<string, Dictionary<string, long>> catNameValueByName = new Dictionary<string, Dictionary<string, long>>();
                    Dictionary<object, object> catValuesDictByCdxID = CatIdValueDictByCatStrIdKvp.Value as Dictionary<object, object>;
                    foreach (KeyValuePair<object, object> catValuesDictByCdxIDkvp in catValuesDictByCdxID)
                    {
                        ulong cdxNodeID = (ulong)catValuesDictByCdxIDkvp.Key;
                        string planetName;
                        if (cdxNodeID == 16140950445578029361)
                        {
                            planetName = "Totals";
                        }
                        else if (cdxNodeID == 16141057659009624245)
                        {
                            planetName = "Unknown";
                        } else {
                            Models.Codex cdx = _dom.codexLoader.Load(cdxNodeID);
                            if(cdx == null)
                            {
                                continue;
                            }

                            string faction = cdx.Faction.ToString();
                            if (faction != "None")
                            {
                                //Determine if this is a planet we should skip or not because it's not part of our faction.
                                if (faction == "Empire" && !factionIsEmpire)
                                {
                                    continue;
                                }
                                else if (faction == "Republic" && factionIsEmpire)
                                {
                                    continue;
                                }
                            }
                            planetName = cdx.Name;
                        }

                        //Create the dictionary to store the category totals by name.
                        Dictionary<string, long> catValueByCatName = new Dictionary<string, long>();
                        Dictionary<object, object> catValueByCatSID = catValuesDictByCdxIDkvp.Value as Dictionary<object, object>;
                        foreach (KeyValuePair<object, object> cdxValueByCatSID in catValueByCatSID)
                        {
                            string catName = catStb.GetText((long)cdxValueByCatSID.Key, string.Empty);
                            if (catName.Length == 0)
                            {
                                //If we can't find the string for the codex cat then store the data anyway.
                                long catSID = (long)cdxValueByCatSID.Key;
                                if (catSID == -3598045246210160864)
                                {
                                    //Store the total elsewhere for easy access.
                                    factionCats.ClassCodexTotals.Add(className, (long)cdxValueByCatSID.Value);
                                    continue;
                                }
                                else
                                {
                                    catName = catSID.ToString();
                                }
                            }

                            catValueByCatName.Add(catName, (long)cdxValueByCatSID.Value);
                        }
                        catNameValueByName.Add(planetName, catValueByCatName);
                    }
                    catNameValueDictByClassName.Add(className, catNameValueByName);
                }
                factionCats.PlanetCatTotalByPlanetNameByCatNameByClassName = catNameValueDictByClassName;
            }

            return factionCats;
        }

        private bool BaseClassNodeIsEmpire(ulong id)
        {
            GomObject classNode = _dom.GetObject(id);
            if (classNode == null)
            {
                return false;
            }

            switch(classNode.Name)
            {
                case "class.pc.bounty_hunter":
                    return true;
                case "class.pc.spy":
                    return true;
                case "class.pc.sith_sorcerer":
                    return true;
                case "class.pc.sith_warrior":
                    return true;
                default:
                    return false;
            }
        }

        private string GetClassNameFromNode(ulong id)
        {
            GomObject classNode = _dom.GetObject(id);
            if(classNode == null)
            {
                return string.Empty;
            }

            return classNode.Data.Get<string>("chrClassDataName");
        }
    }
}
