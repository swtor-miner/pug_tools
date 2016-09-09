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
    public partial class Tools : Form
    {
        #region TorcDB
        public void getDBOutput()
        {
            Clearlist2();
            
            LoadData();



        }

        #endregion
        public void getTorc()
        {
            Clearlist2();

            //temp

            //var newLines = File.ReadAllLines("i:\\new.txt");
            //HashSet<string> newHash = new HashSet<string>();
            //newHash.UnionWith(newLines);

            //var oldLines = File.ReadAllLines("i:\\old.txt");
            //HashSet<string> oldHash = new HashSet<string>();
            //oldHash.UnionWith(oldLines);

            //newHash.RemoveWhere(x => oldHash.Contains(x));

            //StringBuilder t = new StringBuilder();

            //t.Append(String.Join(Environment.NewLine, newHash));
            //WriteFile(t.ToString(), "unique.txt", false);

            //temp end
            LoadData();

            groupFinder();
            getDisciplineCalcData();
            getCrewSkillData();
            outputTables();
            //getTooltips();

            //getAuctionCats();

            //getitemIds();

            //torheadscanner();
            
            
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

        public void groupFinder()
        {
            ClearProgress();
            LoadData();
               
            currentDom.groupFinderContentData.Load(0); //dummy to load data
            
            var count = currentDom.groupFinderContentData.GroupFinderLookup.Count();
            int i = 0;
            WriteFile("", "gfDat.txt", false);
            StringBuilder txter = new StringBuilder();
            Dictionary<GomLib.Models.GroupFinderTime, string> opsList = new Dictionary<GomLib.Models.GroupFinderTime, string>();
            foreach(var kvp in currentDom.groupFinderContentData.GroupFinderLookup)
            {
                progressUpdate(i, count);
                if (kvp.Value.Times != null)
                {
                    foreach (var time in kvp.Value.Times)
                    {
                        opsList.Add(time, kvp.Value.Name);
                    }
                }
                i++;
            }
            opsList = opsList.OrderBy(x => x.Key.StartTime.Date).ToDictionary(x=> x.Key, x=>x.Value);
            foreach (var kvp in opsList)
            {
                txter.Append(String.Format("Start: {0}; End: {1}; Name: {2}{3}", kvp.Key.StartTime.ToString(), kvp.Key.EndTime.ToString(), kvp.Value, Environment.NewLine));
            }
            WriteFile(txter.ToString(), "gfDat.txt", false);

        }

        public void outputTables()
        {
            ClearProgress();
            LoadData();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            string json = JsonConvert.SerializeObject(currentDom.data.armorPerLevel.TableData,settings);
            WriteFile(json, "armorPerLevelTable.json", false);

            json = JsonConvert.SerializeObject(currentDom.data.weaponPerLevel.TableData, settings);
            WriteFile(json, "weaponPerLevelTable.json", false);

            GomLib.Models.ArmorSpec.Load(currentDom);
            json = JsonConvert.SerializeObject(GomLib.Models.ArmorSpec.ArmorSpecList, settings);
            WriteFile(json, "armorSpecTable.json", false);

            GomLib.Models.WeaponSpec.Load(currentDom);
            json = JsonConvert.SerializeObject(GomLib.Models.WeaponSpec.WeaponSpecList, settings);
            WriteFile(json, "weaponSpecTable.json", false);

            currentDom.statData.ToStat("endurance"); //trick it into loading data
            json = JsonConvert.SerializeObject(currentDom.statData.StatLookup, settings);
            WriteFile(json, "statData.json", false);

            currentDom.enhanceData.ToEnhancement(1); //trick it into loading data
            json = JsonConvert.SerializeObject(currentDom.enhanceData.SlotLookup, settings);
            WriteFile(json, "slotData.json", false);

            currentDom.questLoader.Load("qst.location.coruscant.world.enemies_of_the_republic"); //trick it into loading data
            json = JsonConvert.SerializeObject(currentDom.questLoader.fullCreditRewardsTable, settings);
            WriteFile(json, "fullCreditRewardsTable.json", false);
            json = JsonConvert.SerializeObject(currentDom.questLoader.experienceTable, settings);
            WriteFile(json, "experienceTable.json", false);
            json = JsonConvert.SerializeObject(currentDom.questLoader.experienceDifficultyMultiplierTable, settings);
            WriteFile(json, "experienceDifficultyMultiplierTable.json", false);
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
                    new JProperty("TokenData", JsonConvert.SerializeObject(x.Value["ablParsedDescriptionToken"])),
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
        #region Crew Skills
        public void getCrewSkillData()
        {
            Clearlist2();
            ClearProgress();

            LoadData();
            GomLib.Smart smart = new Smart(addtolist2);
            SmartLinkSchematics(currentDom);
            /*var prfBundlesTablePrototype = currentDom.GetObject("prfBundlesTablePrototype");
            List<GomObjectData> prfBundlesTable = prfBundlesTablePrototype.Data.ValueOrDefault<List<object>>("prfBundlesTable")
                .ConvertAll<GomObjectData>(x => (GomObjectData)x);

            List<JObject> bundles = new List<JObject>();
            foreach (var gom in prfBundlesTable)
            {
                string profession = gom.ValueOrDefault<ScriptEnum>("prfEnum").ToString();
                long min = gom.ValueOrDefault<long>("prfBundleMinLevel");
                long max = gom.ValueOrDefault<long>("prfBundleMaxLevel");

                List<GomLib.Models.Schematic> items = new List<GomLib.Models.Schematic>();
                List<ulong> idList = gom.ValueOrDefault<List<object>>("prfBundleSchemList").ConvertAll<ulong>(x => (ulong)x);
                foreach (var id in idList)
                {
                    var schem = currentDom.schematicLoader.Load(id);
                    items.Add(schem);
                }
                items = items.OrderBy(x => (x.Item ?? new GomLib.Models.Item()).Name).ToList();
                JObject bundle = new JObject(
                    new JProperty("Profession", profession),
                    new JProperty("min", min),
                    new JProperty("max", max),
                    new JProperty("Schematics", new JArray(items.Select(x => (x.Item ?? new GomLib.Models.Item()).Name)))
                    );
                bundles.Add(bundle);
            }
                
            JObject output = new JObject(new JProperty("Bundles", new JArray(bundles)));*/

            var itmList = currentDom.GetObjectsStartingWith("schem.");

            Dictionary<string, Dictionary<string, List<GomLib.Models.Schematic>>> professions = new Dictionary<string, Dictionary<string, List<GomLib.Models.Schematic>>>();
            HashSet<ulong> materialIds = new HashSet<ulong>();
            Dictionary<string, HashSet<ulong>> craftedIds = new Dictionary<string, HashSet<ulong>>();

            foreach (var gom in itmList)
            {
                GomLib.Models.Schematic schem = new GomLib.Models.Schematic();
                currentDom.schematicLoader.Load(schem, gom);
                if (schem.Deprecated)
                    continue;
                string crewskill = schem.CrewSkill.ToString();
                string subtype = "";
                if (!professions.ContainsKey(crewskill))
                {
                    professions.Add(crewskill, new Dictionary<string, List<GomLib.Models.Schematic>>());
                    craftedIds.Add(crewskill, new HashSet<ulong>());
                }
                if (schem.MissionCost > 0)
                    subtype = "Missions";
                else
                {
                    subtype = schem.SubTypeName;
                    craftedIds[crewskill].Add(schem.ItemId);
                    materialIds.UnionWith((schem.Materials ?? new Dictionary<ulong, int>()).Keys);
                }

                if (!professions[crewskill].ContainsKey(subtype))
                    professions[crewskill].Add(subtype, new List<GomLib.Models.Schematic>());
                professions[crewskill][subtype].Add(schem);
            }
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;

            JObject output = new JObject(
                new JProperty("Professions",
                    new JArray(
                        from c in professions
                        orderby c.Key
                        select new JObject(
                            new JProperty("Name", Regex.Replace(c.Key, "([a-z]|[A-Z]{2,})([A-Z])", @"$1 $2")),
                            new JProperty("JsonPath", c.Key),
                            new JProperty("Subtypes",
                                new JArray(
                                    from s in c.Value
                                    orderby s.Key
                                    select new JObject(
                                        new JProperty("Name", Regex.Replace(s.Key, "([a-z]|[A-Z]{2,})([A-Z])", @"$1 $2")),
                                        new JProperty("Count", s.Value.Count)
                                        )
                                )
                            )
                        )
                    )
                )
            );

            WriteFile(output.ToString(), "PrfContent\\Data\\crewskills.json", false);

            foreach (var c in professions)
            {
                output = new JObject(from s in c.Value
                                     select new JProperty(Regex.Replace(s.Key, "([a-z]|[A-Z]{2,})([A-Z])", @"$1 $2"), new JArray(
                                         from schem in s.Value
                                         select SchematicToMinifiedJSON(schem))));

                WriteFile(output.ToString(Newtonsoft.Json.Formatting.None), String.Format("PrfContent\\Data\\{0}.json", c.Key), false);
            }

            output = new JObject(from id in materialIds
                                 select new JProperty(id.ToMaskedBase62(), ItemToMinifiedJSON(currentDom.itemLoader.Load(id))));
            WriteFile(output.ToString(Newtonsoft.Json.Formatting.None), "PrfContent\\Data\\prfMaterials.json", false);

            foreach (var kvp in craftedIds)
            {
                if (kvp.Value.Count == 0)
                    continue;
                var jDict = kvp.Value.Select(x => new KeyValuePair<ulong, JObject>(x, ItemToMinifiedJSON(currentDom.itemLoader.Load(x))));
                output = new JObject(kvp.Value.Select(x => new JProperty(x.ToMaskedBase62(), ItemToMinifiedJSON(currentDom.itemLoader.Load(x)))));
                WriteFile(output.ToString(Newtonsoft.Json.Formatting.None), String.Format("PrfContent\\Data\\{0}_Items.json", kvp.Key), false);
            }
        }

        private JObject ItemToMinifiedJSON(GomLib.Models.Item item)
        {
            if (item != null) OutputIcon(item.Icon, "PrfContent");
            if(item == null)
                return new JObject();
            JObject jItm = new JObject(
                        new JProperty("Name", new JValue(item.Name ?? "")),
                        new JProperty("Description", new JValue((item.Description ?? "").Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\r", "<br />"))),
                        new JProperty("Quality", new JValue(item.Quality.ToString())),
                        new JProperty("Icon", new JValue(GetIconFilename(item.Icon ?? "").Replace("'", ""))),
                        new JProperty("BaseLevel", new JValue(item.ItemLevel)),
                        new JProperty("CombinedLevel", new JValue(item.CombinedRating))
                        );
            if (item.RequiredLevel != 0)
                jItm.Add(new JProperty("MinLevel", new JValue(item.RequiredLevel)));
            if (item.EquipAbilityId != 0)
                jItm.Add(new JProperty("EquipAbility", new JValue(generateDescWithTokens(item.EquipAbility))));
            if (item.UseAbilityId != 0)
                jItm.Add(new JProperty("UseAbility", new JValue(generateDescWithTokens(item.UseAbility))));
            if (item.StatModifiers.ToString() != "Empty List")
            {
                string list = item.StatModifiers.ToString();
                list = list.Substring(0, list.Length - 1);
                jItm.Add(new JProperty("Stats", new JArray(list.Split(','))));
            }
            if (item.DisassembleCategory != null)
                jItm.Add(new JProperty("DissassembleCategory", new JValue(item.DisassembleCategory.ToString())));
            if(item.References.ContainsKey("createdBy"))
            {
                JArray variants = new JArray();
                if (item.References["createdBy"].Count > 1)
                {
                    string soindf = "";
                }
                else
                {
                    var schemvar = item._dom.schemVariationLoader.Load(item.References["createdBy"].First());
                    if (schemvar.Id != 0)
                    {
                        variants = new JArray(
                            schemvar.VariationPackages
                                .Select(x =>
                                    new JObject(
                                        new JProperty("Name", x.Name),
                                        new JProperty("Stats",
                                            new JArray(x.AtrributePercentages.Select(y => String.Format("+{1}% {0}", y.Key, y.Value)).ToArray()))
                                    )
                                ).ToArray()
                            );
                    }
                }
                jItm.Add(new JProperty("Variations", variants));
                
            }
            return jItm;
        }

        private JObject SchematicToMinifiedJSON(GomLib.Models.Schematic schem)
        {
            JObject jSchem = JObject.Parse(schem.ToJSON());

            if (schem.MissionCost > 0)
            {
                jSchem.Remove("CraftingTimeT1");
                jSchem.Remove("CraftingTimeT2");
                jSchem.Remove("CraftingTimeT3");
                jSchem.Remove("Workstation");
                jSchem.Remove("ItemId");
                jSchem.Remove("ItemParentId");
                jSchem.Remove("Materials");
                jSchem.Remove("Subtype");
                jSchem.Remove("ResearchQuantity1");
                jSchem.Remove("ResearchChance1");
                jSchem.Remove("ResearchQuantity2");
                jSchem.Remove("ResearchChance2");
                jSchem.Remove("ResearchQuantity3");
                jSchem.Remove("ResearchChance3");
                jSchem.Remove("TrainingCost");
                jSchem.Remove("DisableDisassemble");
                jSchem.Remove("DisableCritical");
                jSchem.Remove("NameId");
                jSchem.Remove("References");
            }
            else
            {
                jSchem.Property("ItemId").Value = jSchem.Property("ItemId").Value.ToString();
                jSchem.Remove("NameId");
                jSchem.Remove("References");
                jSchem.Remove("MissionCost");
                jSchem.Remove("MissionDescriptionId");
                jSchem.Remove("MissionDescription");
                jSchem.Remove("MissionUnlockable");
                jSchem.Remove("MissionLight");
                jSchem.Remove("MissionLightCrit");
                jSchem.Remove("MissionDark");
                jSchem.Remove("MissionDarkCrit");
                jSchem.Remove("MissionFaction");
                jSchem.Remove("MissionYieldDescriptionId");
                jSchem.Remove("MissionYieldDescription");
            }

            return jSchem;
        }

        public string generateDescWithTokens(GomLib.Models.Ability skill)
        {
            var retval = skill.Description;

            if (skill.DescriptionTokens == null)
                return retval;

            for (var i = 0; i < skill.DescriptionTokens.Count; i++)
            {
                var id = skill.DescriptionTokens.ElementAt(i).Key;
                var value = skill.DescriptionTokens.ElementAt(i).Value["ablParsedDescriptionToken"].ToString();
                var type = skill.DescriptionTokens.ElementAt(i).Value["ablDescriptionTokenType"].ToString().Replace("ablDescriptionTokenType", "");
                var start = retval.IndexOf("<<" + id);

                if (start == -1)
                {
                    //console.log("didn't find: <<" + id);
                    continue;
                }
                //console.log("id" + id + ":" + retval);
                //console.log("Start Index: " + start);

                var end = retval.Substring(start).IndexOf(">>") + 2;

                //console.log("Length: " +length);
                var fullToken = retval.Substring(start, end);
                //console.log("Full: " + fullToken);

                var durationText = "";
                if ((end - start) > 5)
                {
                    string[] durationList = new string[] { "", "", "" };
                    var partialToken = fullToken.Substring(4, fullToken.Length - 7);
                    //console.log("Partial:" + partialToken);

                    durationList = partialToken.Replace("%d", "").Split('/').ToArray();
                    //console.log(durationList);

                    int pValue;
                    Int32.TryParse(value.ToString(), out pValue);

                    durationText = "";
                    if (pValue <= 0)
                        durationText = durationList[0];
                    else if (pValue > 1)
                        durationText = durationList[2];
                    else
                        durationText = durationList[1];
                    //console.log(pValue + durationText);
                }
                //console.log(type);
                while (retval.IndexOf(fullToken) != -1)
                { //sometimes there's multiple instance of the same token.
                    switch (type)
                    {
                        case "Healing":
                        case "Damage":
                            retval = retval.Replace(fullToken, generateTokenString(value));
                            break;
                        case "Duration":
                            retval = retval.Replace(fullToken, value + durationText);
                            break;
                        case "Talent":
                            retval = retval.Replace(fullToken, value);
                            //console.log("replaced '<<" + id + ">>' :" + retval);
                            break;
                        default:
                            //console.log(type);
                            retval = retval.Replace(fullToken, "Unknown Token: " + type);
                            break;
                    }
                }

            }
            return retval;
        }

        public string generateTokenString(string value)
        {
            var retval = "";

            var splitTokens = value.Split(';');
            /*if (splitTokens.length == 2)
                retval = splitTokens[1] + " to " + splitTokens[0];
            else*/
            retval = splitTokens[0];

            var tokArray = splitTokens[0].Split(',');

            switch (tokArray[0])
            {
                case "damage":
                    if (tokArray.Count() == 7)
                    {
                        var minp = float.Parse(tokArray[5]);
                        var maxp = float.Parse(tokArray[6]);
                        if (float.Parse(tokArray[4]) == 1)
                            retval = Math.Round(float.Parse(tokArray[4]) * minp) + "-" + Math.Round(float.Parse(tokArray[4]) * maxp);
                        else
                            retval = Math.Round(float.Parse(tokArray[4]) * ((minp + maxp) / 2)).ToString();
                    }
                    else
                    {
                        switch (tokArray[4])
                        {
                            case "w":
                                var min = (float.Parse(tokArray[11]) + 1.0) * 405 + float.Parse(tokArray[6]) * 1000 + float.Parse(tokArray[7]) * 3185; /*(AmountModifierPercent + 1) * 405 * 0.3 + */
                                var max = (float.Parse(tokArray[11]) + 1.0) * 607 + float.Parse(tokArray[6]) * 1000 + float.Parse(tokArray[8]) * 3185; /*(AmountModifierPercent + 1) * 607 * 0.3 + */
                                //console.log("(" + tokArray[11] + " + 1.0) * 405 + " + tokArray[6] + " * 1000 + " + tokArray[8]  + " * 3185");
                                if (float.Parse(tokArray[5]) == 1)
                                    retval = Math.Round(float.Parse(tokArray[5]) * min) + "-" + Math.Round(float.Parse(tokArray[5]) * max);
                                else
                                    retval = Math.Round(float.Parse(tokArray[5]) * ((min + max) / 2)).ToString();
                                break;
                            case "s":
                                var mins = float.Parse(tokArray[6]) * 1000 + float.Parse(tokArray[7]) * 3185;
                                var maxs = float.Parse(tokArray[6]) * 1000 + float.Parse(tokArray[8]) * 3185;
                                if (float.Parse(tokArray[5]) == 1)
                                    retval = Math.Round(float.Parse(tokArray[5]) * mins) + "-" + Math.Round(float.Parse(tokArray[5]) * maxs);
                                else
                                    retval = Math.Round(float.Parse(tokArray[5]) * ((mins + maxs) / 2)).ToString();
                                break;
                        }
                    }
                    break;
                case "healing":
                    if (tokArray.Count() == 5)
                    {
                        var minh = float.Parse(tokArray[2]) * 1000 + float.Parse(tokArray[3]) * 14520;
                        var maxh = float.Parse(tokArray[2]) * 1000 + float.Parse(tokArray[4]) * 14520;
                        if (float.Parse(tokArray[1]) == 1)
                            retval = Math.Round(float.Parse(tokArray[1]) * minh) + "-" + Math.Round(float.Parse(tokArray[1]) * maxh);
                        else
                            retval = Math.Round(float.Parse(tokArray[1]) * ((minh + maxh) / 2)).ToString();
                    }
                    else
                    {
                        var mina = float.Parse(tokArray[2]);
                        var maxa = float.Parse(tokArray[3]);
                        if (float.Parse(tokArray[1]) == 1)
                            retval = Math.Round(float.Parse(tokArray[1]) * mina) + "-" + Math.Round(float.Parse(tokArray[1]) * maxa);
                        else
                            retval = Math.Round(float.Parse(tokArray[1]) * ((mina + maxa) / 2)).ToString();
                    }
                    break;
            }
            return retval;
        }
        #endregion
    }
}
