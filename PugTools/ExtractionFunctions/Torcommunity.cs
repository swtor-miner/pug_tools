using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Windows;
using System.Threading;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using GomLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MessageBox = System.Windows.Forms.MessageBox;

namespace tor_tools
{
    public partial class Tools
    {
        public void getTorc()
        {
            Clearlist2();

            LoadData();

            //getTooltips();

            //getAuctionCats();

            //getitemIds();

            torheadscanner();
            
            EnableButtons();
        }

        public void getAuctionCats()
        {
            ClearProgress();
            LoadData();
            GomLib.Models.AuctionCategory.Load(currentDom);
            var gomList = GomLib.Models.AuctionCategory.AuctionCategoryList;
            var count = gomList.Count();
            int i = 0;
            WriteFile("", "aucCats.txt", false);
            foreach (var gom in gomList)
            {
                progressUpdate(i, count);
                WriteFile(String.Join(Environment.NewLine, Environment.NewLine + gom.Value.Name + " (" + gom.Value.Id + ")", String.Join(Environment.NewLine, gom.Value.SubCategories.Select(x => "  " + x.Value.Name + " (" + x.Value.Id + ")").ToList())), "aucCats.txt", true);
                i++;
            }
        }

        public void getitemIds()
        {
            ClearProgress();
            LoadData();
            var gomList = currentDom.GetObjectsStartingWith("itm.");
            var count = gomList.Count();
            int i = 0;
            WriteFile("", "itemIds.txt", false);
            foreach (var gom in gomList)
            {
                
                progressUpdate(i, count);
                var itm = currentDom.itemLoader.Load(gom);
                WriteFile(String.Format("{0}: http://torcommunity.com/db/{1}{2}", itm.Name, itm.Base62Id, Environment.NewLine), "itemIds.txt", true);
                i++;
            }
        }

        public void torheadscanner()
        {
            //http://www.torhead.com/item/ace+in+the+hole
            //http://www.torhead.com/item/schematic:+[artifact]+microfilament+skill+d-device
            //http://www.torhead.com/item/agent's-birthright-headgear

            ClearProgress();
            LoadData();
            var gomList = currentDom.GetObjectsStartingWith("itm.");
            var count = gomList.Count();
            int i = 0;
            Dictionary<string, GomLib.Models.Item> itemlist = new Dictionary<string, GomLib.Models.Item>();
            foreach (var gom in gomList)
            {
                progressUpdate(i, count);
                var itm = currentDom.itemLoader.Load(gom);
                if (itemlist.ContainsKey(itm.Name))
                {
                    //add code here to prioritize items based on quality/item level
                }
                else if(itm.Name != null && itm.Name != ""){
                    itemlist.Add(itm.Name, itm);
                }
                i++;
            }
            addtolist("item list created in memory, scanning torhead");
            addtolist(String.Format("Found {0} unique names, full scan", itemlist.Count));
            TimeSpan t = TimeSpan.FromSeconds(itemlist.Count/5);

            string answer = String.Format("will take {0:D2}h:{1:D2}m:{2:D2}s",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);
            addtolist(answer);
            Dictionary<string, string> nametourlmap = new Dictionary<string, string>();
            WriteFile("", "torheadurls.txt", false);
            WriteFile("", "badtorheadurls.txt", false);
            WriteFile("", "torheadurlserrors.txt", false);

            ClearProgress();
            count = itemlist.Count();
            i = 0;
            EnableButtons();
            foreach (var kvp in itemlist)
            {
                progressUpdate(i, count);
                string url = String.Format("http://www.torhead.com/item/{0}", kvp.Key.Replace(" ", "+"));
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.CreateHttp(url);
                req.AllowAutoRedirect = false;
                try
                {
                    var response = (HttpWebResponse)req.GetResponse();
                    if (response.StatusCode != HttpStatusCode.InternalServerError)
                    {
                        string loc = response.GetResponseHeader("Location");
                        nametourlmap.Add(kvp.Value.Base62Id, loc);
                        WriteFile(String.Format("{0},{1},http://www.torhead.com{2}{3}", kvp.Value.Base62Id, url, loc, Environment.NewLine), "torheadurls.txt", true);
                        addtolist2(url);
                    }
                    else
                        WriteFile(String.Format("{0},{1}{2}", kvp.Value.Base62Id, url, Environment.NewLine), "badtorheadurls.txt", true);
                }
                catch (WebException ex)
                {
                    WriteFile(String.Format("{0},{1}{2}", kvp.Value.Base62Id, url, Environment.NewLine), "torheadurlserrors.txt", true);
                    addtolist(String.Format("Error: {0}", ex.Message));
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        addtolist(String.Format("Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode));
                        addtolist(String.Format("Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription));
                    }
                }
                i++;
                Thread.Sleep(200);
            }
            addtolist("Done scanning torhead!");
        }

        #region Discipline Calculator
        public void getDisciplineCalcData()
        {
            Clearlist2();
            ClearProgress();

            LoadData();
            var chrAdvancedClassDataPrototype = currentDom.GetObject("chrAdvancedClassDataPrototype");
            var chrAdvancedClassSetPerClass = chrAdvancedClassDataPrototype.Data.ValueOrDefault<Dictionary<object, object>>("chrAdvancedClassSetPerClass")
                .ToDictionary(x => (ulong)x.Key, x => ((Dictionary<object, object>)x.Value).Keys.ToList().ConvertAll(z => (ulong)z));

            Dictionary<string, JObject> impClasses = new Dictionary<string, JObject>();
            Dictionary<string, JObject> repClasses = new Dictionary<string, JObject>();
            var nameTable = currentDom.stringTable.Find("str.gui.classnames");

            foreach (var baseClass in chrAdvancedClassSetPerClass)
            {
                var baseClassData = currentDom.GetObject(baseClass.Key);
                var baseClassNameId = baseClassData.Data.ValueOrDefault<long>("chrClassDataNameId");
                baseClassData.Unload();

                var name = nameTable.GetText(baseClassNameId, "str.gui.classnames");
                var resource = "";
                switch (name)
                {
                    case "Jedi Consular":
                    case "Sith Inquisitor":
                        resource = "Force";
                        break;
                    case "Trooper":
                        resource = "Ammo";
                        break;
                    case "Jedi Knight":
                        resource = "Focus";
                        break;
                    case "Sith Warrior":
                        resource = "Rage";
                        break;
                    case "Smuggler":
                    case "Imperial Agent":
                        resource = "Energy";
                        break;
                    case "Bounty Hunter":
                        resource = "Heat";
                        break;
                    default:
                        Console.WriteLine("WTF CLASS IS THIS!?!");
                        break;
                }
                List<GomLib.Models.AdvancedClass> acs = new List<GomLib.Models.AdvancedClass>();
                foreach (var acId in baseClass.Value)
                {
                    var advClassData = currentDom.GetObject(acId);
                    var ac = currentDom.advancedClassLoader.Load(advClassData);
                    advClassData.Unload();
                    acs.Add(ac);
                }
                acs.Sort((x, y) => string.Compare(x.Name.Replace(" ", ""), y.Name.Replace(" ", "")));
                string icon = "icon";
                switch (name)
                {
                    case "Jedi Knight":
                    case "Trooper":
                        icon = "republic";
                        break;
                    case "Jedi Consular":
                    case "Smuggler":
                        acs.Reverse();
                        icon = "republic";
                        break;
                    case "Sith Inquisitor":
                    case "Sith Warrior":
                    case "Bounty Hunter":
                    case "Imperial Agent":
                        icon = "empire";
                        break;
                }
                OutputIcon(icon);

                bool available = true;
                /*switch (name)
                {
                    case "Trooper":
                    case "Bounty Hunter":
                    case "Sith Inquisitor":
                    case "Jedi Consular":
                    case "Imperial Agent":
                        available = true;
                        break;
                }*/
                JObject classObj = new JObject(
                    new JProperty("ClassName", new JValue(name)),
                    new JProperty("Available", new JValue(available)),
                    new JProperty("Icon", new JValue(GetIconFilename(icon))),
                    new JProperty("Resource", new JValue(resource)),
                    new JProperty("AdvancedClasses",
                        new JArray(
                            from ac in acs
                            //orderby ac.Name
                            select AdvancedClassToMinifiedJSON(ac, icon, name))));
                switch (name.Split(' ')[0])
                {
                    case "Jedi":
                    case "Smuggler":
                    case "Trooper":
                        repClasses.Add(name, classObj);
                        break;
                    case "Sith":
                    case "Bounty":
                    case "Imperial":
                        impClasses.Add(name, classObj);
                        break;
                    default:
                        Console.WriteLine("WTF CLASS IS THIS!?!");
                        break;
                }

            }

            JObject output = new JObject(
               new JProperty("Imperial",
                   new JArray(
                       impClasses["Bounty Hunter"],
                       impClasses["Imperial Agent"],
                       impClasses["Sith Inquisitor"],
                       impClasses["Sith Warrior"]
                /*from c in impClasses
                    orderby c.Value<string>("ClassName")
                    select c*/)),
               new JProperty("Republic",
                   new JArray(
                       repClasses["Trooper"],
                       repClasses["Smuggler"],
                       repClasses["Jedi Consular"],
                       repClasses["Jedi Knight"]
                /*from c in repClasses
                orderby c.Value<string>("ClassName")
                select c*/)));
            WriteFile(output.ToString(/*Newtonsoft.Json.Formatting.None*/), "DiscContent\\Data\\Classes.json", false);

            /*foreach (var file in System.IO.Directory.EnumerateFiles(String.Join("", Config.ExtractPath, "DiscContent\\Data\\"), "*"))
            {
                CreateGzip(file.Replace(Config.ExtractPath, ""));
                System.IO.File.Delete(file);
            }*/
        }

        private JObject AdvancedClassToMinifiedJSON(GomLib.Models.AdvancedClass ac, string icon, string className)
        {
            OutputIcon(ac.ClassBackground);
            Dictionary<double, JObject> util = new Dictionary<double, JObject>();
            List<JObject> unusedUtilities = new List<JObject>();
            foreach (var pAbl in ac.UtilityPkg.PackageAbilities)
            {
                var abl = pAbl.Ability;
                abl.Level = pAbl.Level;
                double pos = pAbl.UtilityTier + (double)(pAbl.UtilityPosition / 100.0);
                util.Add(pos, AbilityToMinifiedJSON(abl));
            }
            foreach (var pTal in ac.UtilityPkg.PackageTalents)
            {
                var tal = pTal.Talent;
                double pos = pTal.UtilityTier + (double)(pTal.UtilityPosition / 100.0);
                var jTal = TalentToMinifiedJSON(tal, (int)pTal.Level);
                if (pos == 0)
                    unusedUtilities.Add(jTal);
                else
                    util.Add(pos, jTal);
            }
            /*for (int i = 1; i <= 21; i++)
            {
                if (!util.ContainsKey(i))
                    util.Add(i, new JObject(
                        new JProperty("Name", new JValue("Unknown")),
                        new JProperty("Description", new JValue("No Utility with this index")),
                        new JProperty("Icon", new JValue("icon"))
                        ));
            }*/
            bool available = true;
            /*switch (ac.Name)
            {
                case "Operative":
                case "Scoundrel":
                case "Gunslinger":
                    available = false;
                    break;
            }*/
            bool utilAvailable = available;
            //if (ac.Name == "Sniper")
            //utilAvailable = false;

            JObject acObj = new JObject(
                    new JProperty("Name", new JValue(ac.Name)),
                    new JProperty("Description", new JValue(ac.Description)),
                    new JProperty("Icon", new JValue(GetIconFilename(icon))),
                    new JProperty("Available", new JValue(available)),
                    new JProperty("UtilitiesAvailable", new JValue(utilAvailable)),
                    new JProperty("Background", new JValue(GetIconFilename(ac.ClassBackground))),
                    new JProperty("JsonPath", new JValue(ac.Name.Replace(' ', '_'))),
                    new JProperty("UtilitiesPath", new JValue(String.Format("{0}_utilities", ac.Name.Replace(' ', '_')))),
                    new JProperty("Disciplines",
                        new JArray(from d in ac.Disciplines
                                   orderby d.SortIdx
                                   select new JObject(
                                       new JProperty("Name", new JValue(d.Name)),
                                       new JProperty("Description", new JValue(d.Description)),
                                       new JProperty("Role", new JValue(d.Role)),
                                       new JProperty("Icon", new JValue(GetIconFilename(d.Icon))),
                                       new JProperty("Available", new JValue(
                                           available
                                       /*&&
                                       (d.Name == "Engineering") ? false : true
                                       &&
                                       (d.Name == "Marksmanship") ? false : true*/)),
                                       new JProperty("JsonPath", new JValue(d.Name.Replace(' ', '_'))),
                                       new JProperty("BaseSkills",
                                           new JArray(from b in d.BaseAbilities
                                                      orderby b.Level
                                                      select AbilityToMinifiedJSON(b)
                                           )
                                       )
                                   )
                        )
                    )
                );
            JObject utilObj = new JObject(
                new JProperty("UtilitySkills",
                        new JArray(from kvp in util
                                   orderby kvp.Key
                                   select kvp.Value
                        )
                    )
                );
            WriteFile(utilObj.ToString(/*Newtonsoft.Json.Formatting.None*/), String.Format("DiscContent\\Data\\{0}_utilities.json", ac.Name.Replace(' ', '_')), false);

            foreach (var dis in ac.Disciplines)
            {
                WriteFile(DisciplineToMinifiedJSON(dis).ToString(/*Newtonsoft.Json.Formatting.None*/), String.Format("DiscContent\\Data\\{0}.json", dis.Name.Replace(' ', '_')), false);
            }

            //generate ability json
            JObject acPkgObj = new JObject(
                new JProperty("Name", new JValue(ac.Name)),
                new JProperty("BaseClassJsonPath", new JValue(className)),
                AbilityPackageToMinifiedJSON("AdvancedClassPackage", ac.AdvancedClassPkgs //BasePackage
                    .Where(x => x.Fqn.Contains("base"))
                    .First())
                );
            WriteFile(acPkgObj.ToString(/*Newtonsoft.Json.Formatting.None*/), String.Format("DiscContent\\Data\\{0}.json", ac.Name.Replace(' ', '_')), false);


            JObject cPkgObj = new JObject(
                new JProperty("Name", new JValue(className)),
                AbilityPackageToMinifiedJSON("BaseClassPackage", ac.BaseClassPkgs //BasePackage
                    .Where(x => x.Fqn.Contains("base"))
                    .First()),
                AbilityPackageToMinifiedJSON("General", ac.BaseClassPkgs //BasePackage
                    .Where(x => x.Fqn.Contains("default"))
                    .First()));
            className = className.Replace(' ', '_');
            WriteFile(cPkgObj.ToString(/*Newtonsoft.Json.Formatting.None*/), String.Format("DiscContent\\Data\\{0}.json", className), false);
            return acObj;
        }

        private JObject DisciplineToMinifiedJSON(GomLib.Models.Discipline dis)
        {
            OutputIcon(dis.Icon);
            Dictionary<int, JObject> path = new Dictionary<int, JObject>();
            foreach (var pAbl in dis.PathAbilities.PackageAbilities)
            {
                var abl = pAbl.Ability;
                abl.Level = pAbl.Level;
                path.Add(abl.Level, AbilityToMinifiedJSON(abl));
            }
            foreach (var pTal in dis.PathAbilities.PackageTalents)
            {
                var tal = pTal.Talent;
                var level = (int)pTal.Level;
                path.Add(level, TalentToMinifiedJSON(tal, level));
            }
            var disUtilityLevelsPrototype = currentDom.GetObject("disUtilityLevelsPrototype");
            var disUtilityLevelsLookup = disUtilityLevelsPrototype.Data.ValueOrDefault<Dictionary<object, object>>("disUtilityLevelsLookup").ToDictionary(x => Convert.ToInt32(x.Key), x => Convert.ToInt32(x.Value));
            int f = 1;
            foreach (var lvu in disUtilityLevelsLookup)
            {
                if (lvu.Value == f)
                {
                    var uAbl = new GomLib.Models.Ability();
                    uAbl.Level = lvu.Key;
                    uAbl.IsPassive = true;
                    path.Add(lvu.Key, AbilityToMinifiedJSON(uAbl));
                    f++;
                }
            }

            JObject acObj = new JObject(
                    new JProperty("Name", new JValue(dis.Name)),
                    new JProperty("Description", new JValue(dis.Description)),
                    new JProperty("Icon", new JValue(GetIconFilename(dis.Icon))),
                    new JProperty("Role", new JValue(dis.Role)),
                    new JProperty("DisciplinePath",
                        new JArray(from kvp in path
                                   orderby kvp.Key
                                   select kvp.Value
                        )
                    ));
            return acObj;
        }

        private JProperty AbilityPackageToMinifiedJSON(string name, GomLib.Models.AbilityPackage ablPkg)
        {
            return AbilityPackageToMinifiedJSON(name, ablPkg, false);
        }

        private JProperty AbilityPackageToMinifiedJSON(string name, GomLib.Models.AbilityPackage ablPkg, bool removeEmptyValues)
        {
            return new JProperty(name,
                new JArray(
                    ablPkg.PackageAbilities
                /*.Where(x => !x.Ability.IsHidden)*/
                    .Select(x => AbilityToMinifiedJSON(x.Ability, x, removeEmptyValues))
                    .Where(x => x != null)
                .Concat(ablPkg
                    .PackageTalents
                    .Select(x => TalentToMinifiedJSON(x.Talent, (int)x.Level))
                    )
                .OrderBy(x => ((JObject)x).Value<int>("Level"))
                .ThenBy(x => ((JObject)x).Value<string>("Name"))
                            ));
        }
        private JObject AbilityToMinifiedJSON(GomLib.Models.Ability abl)
        {
            return AbilityToMinifiedJSON(abl, null);
        }

        private JObject AbilityToMinifiedJSON(GomLib.Models.Ability abl, GomLib.Models.PackageAbility pAbl)
        {
            return AbilityToMinifiedJSON(abl, pAbl, false);
        }
        private JObject AbilityToMinifiedJSON(GomLib.Models.Ability abl, GomLib.Models.PackageAbility pAbl, bool removeEmptyValues)
        {
            OutputIcon(abl.Icon);
            int level = 0;
            List<int> ranks = new List<int>();
            bool scales = false;
            if (pAbl != null)
            {
                level = pAbl.Level;
                ranks = pAbl.Levels;
                scales = pAbl.Scales;
            }
            else
                level = abl.Level;


            JObject ablObj = new JObject(
                        new JProperty("Name", new JValue(abl.Name ?? "")),
                        new JProperty("Description", new JValue(abl.Description ?? "")),
                        new JProperty("Cooldown", new JValue(abl.Cooldown)),
                        new JProperty("CastTime", new JValue(abl.CastingTime)),
                        new JProperty("ChannelTime", new JValue(abl.ChannelingTime)),
                        new JProperty("Range", new JValue((int)(abl.MaxRange * 10))),
                        new JProperty("Cost", new JValue((int)(abl.ApCost + abl.EnergyCost + abl.ForceCost))),
                        new JProperty("Icon", new JValue(GetIconFilename(abl.Icon ?? "").Replace("'", ""))),
                        new JProperty("Level", new JValue(level)),
                        new JProperty("IsUtilityPoint", new JValue(abl.Name == null)),
                        new JProperty("IsHighlighted", new JValue(!abl.IsPassive)),
                        new JProperty("IsHidden", new JValue(abl.IsHidden))
                        );
            if (removeEmptyValues)
            {
                ablObj.Remove("IsUtilityPoint");
                ablObj.Remove("IsHighlighted");
                ablObj.Remove("IsHidden");
                if (ablObj.Value<float>("Cooldown") == 0.0)
                    ablObj.Remove("Cooldown");
                if (ablObj.Value<float>("CastTime") == 0.0)
                    ablObj.Remove("CastTime");
                if (ablObj.Value<float>("ChannelTime") == 0.0)
                    ablObj.Remove("ChannelTime");
                if (ablObj.Value<int>("Cost") == 0)
                    ablObj.Remove("Cost");
                if (ablObj.Value<int>("Range") == 0)
                    ablObj.Remove("Range");
                if (ablObj.Value<int>("Level") == 1)
                    ablObj.Remove("Level");
                if (ablObj.Value<string>("Icon") == "")
                    ablObj.Remove("Icon");
                if (ablObj.Value<string>("Name") == "" && ablObj.Value<string>("Description") == "" && ablObj.Properties().Count() == 2)
                    return null;
            }
            if (abl.DescriptionTokens != null)
                ablObj.Add(AbilityTokensToMinifiedJSON(abl.DescriptionTokens));
            if (ranks.Count > 1 && !scales) ablObj.Add(new JProperty("Ranks", new JArray(ranks)));
            return ablObj;
        }

        private JProperty AbilityTokensToMinifiedJSON(Dictionary<int, Dictionary<string, object>> descTokens)
        {
            if (descTokens == null) return new JProperty(new JProperty("Tokens"));
            JProperty jDescTokens = new JProperty("Tokens", new JArray(descTokens
                .Select(x => new JObject(
                    new JProperty("TokenId", x.Key),
                    new JProperty("TokenData", x.Value["ablParsedDescriptionToken"].ToString()),
                    new JProperty("TokenType", x.Value["ablDescriptionTokenType"].ToString().Replace("ablDescriptionTokenType", ""))
                        )
                    )
                )
            );

            return jDescTokens;
        }

        private JObject TalentToMinifiedJSON(GomLib.Models.Talent tal)
        {
            return TalentToMinifiedJSON(tal, 0);
        }
        private JObject TalentToMinifiedJSON(GomLib.Models.Talent tal, int level)
        {
            OutputIcon(tal.Icon);
            JObject jTal = new JObject(
                        new JProperty("Name", new JValue(tal.Name ?? "")),
                        new JProperty("Description", new JValue(tal.Description ?? "")),
                        new JProperty("Icon", new JValue(GetIconFilename(tal.Icon ?? "").Replace("'", ""))),
                        new JProperty("Level", new JValue(level)),
                        new JProperty("IsUtilityPoint", new JValue(false)),
                        new JProperty("IsHighlighted", new JValue(false))
                        );
            if (tal.TokenList != null)
                jTal.Add(TalentTokensToMinifiedJSON(tal.TokenList));
            return jTal;
        }

        private JProperty TalentTokensToMinifiedJSON(List<float> descTokens)
        {
            if (descTokens == null) return new JProperty(new JProperty("Tokens"));
            JArray tempArray = new JArray();
            for (var i = 0; i < descTokens.Count; i++)
            {
                tempArray.Add(new JObject(
                    new JProperty("TokenId", i + 1),
                    new JProperty("TokenData", descTokens[i]),
                    new JProperty("TokenType", "Talent")
                        )
                    );
            }
            return new JProperty("Tokens", tempArray);
        }

        private void OutputIcon(string icon)
        {
            OutputIcon(icon, "DiscContent");
        }

        #endregion
    }
}
