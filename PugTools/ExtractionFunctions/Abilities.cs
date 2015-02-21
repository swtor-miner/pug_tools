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

namespace tor_tools
{
    public partial class Tools
    {
        public void getAbilities()
        {
            Clearlist2();
            
            LoadData();

            var itmList = currentDom.GetObjectsStartingWith("abl.").Where(obj => !obj.Name.Contains("/"));
            double ttl = itmList.Count();
            bool append = false;
            bool addedChanged = false;
            string changed = "";
            if (sql)
            {
                sqlTransactionsInitialize("INSERT INTO `ability` (`current_version`, `previous_version`, `Name`, `NodeId`, `NameId`, `Description`, `DescriptionId`, `Fqn`, `Level`, `Icon`, `IsHidden`, `IsPassive`, `Cooldown`, `CastingTime`, `ForceCost`, `EnergyCost`, `ApCost`, `ApType`, `MinRange`, `MaxRange`, `Gcd`, `GcdOverride`, `ModalGroup`, `SharedCooldown`, `TalentTokens`, `AbilityTokens`, `TargetArc`, `TargetArcOffset`, `TargetRule`, `LineOfSightCheck`, `Pushback`, `IgnoreAlacrity`, `Hash`) VALUES ",

@"ON DUPLICATE KEY UPDATE 
previous_version = IF((@update_record := (Hash <> VALUES(Hash))), current_version, previous_version),
current_version = IF(@update_record, VALUES(current_version), current_version),
Name = IF(@update_record, VALUES(Name), Name),
NodeId = IF(@update_record, VALUES(NodeId), NodeId),
NameId = IF(@update_record, VALUES(NameId), NameId),
Description = IF(@update_record, VALUES(Description), Description),
DescriptionId = IF(@update_record, VALUES(DescriptionId), DescriptionId),
Fqn = IF(@update_record, VALUES(Fqn), Fqn),
Level = IF(@update_record, VALUES(Level), Level),
Icon = IF(@update_record, VALUES(Icon), Icon),
IsHidden = IF(@update_record, VALUES(IsHidden), IsHidden),
IsPassive = IF(@update_record, VALUES(IsPassive), IsPassive),
Cooldown = IF(@update_record, VALUES(Cooldown), Cooldown),
CastingTime = IF(@update_record, VALUES(CastingTime), CastingTime),
ForceCost = IF(@update_record, VALUES(ForceCost), ForceCost),
EnergyCost = IF(@update_record, VALUES(EnergyCost), EnergyCost),
ApCost = IF(@update_record, VALUES(ApCost), ApCost),
ApType = IF(@update_record, VALUES(ApType), ApType),
MinRange = IF(@update_record, VALUES(MinRange), MinRange),
MaxRange = IF(@update_record, VALUES(MaxRange), MaxRange),
Gcd = IF(@update_record, VALUES(Gcd), Gcd),
GcdOverride = IF(@update_record, VALUES(GcdOverride), GcdOverride),
ModalGroup = IF(@update_record, VALUES(ModalGroup), ModalGroup),
SharedCooldown = IF(@update_record, VALUES(SharedCooldown), SharedCooldown),
TalentTokens = IF(@update_record, VALUES(TalentTokens), TalentTokens),
AbilityTokens = IF(@update_record, VALUES(AbilityTokens), AbilityTokens),
TargetArc = IF(@update_record, VALUES(TargetArc), TargetArc),
TargetArcOffset = IF(@update_record, VALUES(TargetArcOffset), TargetArcOffset),
TargetRule = IF(@update_record, VALUES(TargetRule), TargetRule),
LineOfSightCheck = IF(@update_record, VALUES(LineOfSightCheck), LineOfSightCheck),
Pushback = IF(@update_record, VALUES(Pushback), Pushback),
IgnoreAlacrity = IF(@update_record, VALUES(IgnoreAlacrity), IgnoreAlacrity),
Hash = IF(@update_record, VALUES(Hash), Hash);");

                AbilityDataFromFqnListToSQL(itmList);

                sqlTransactionsFlush();
            }
            else
            {
                if(chkBuildCompare.Checked)
                {
                    addedChanged = true;
                    changed = "Changed";
                }
                var filename = changed + "Abilities.xml";
                if(outputTypeName == "Text")
                {
                    filename = changed + "Abilities.txt";
                    string generatedContent = AbilityDataFromFqnList(itmList);
                    WriteFile(generatedContent, filename, append);
                }
                else
                {
                    if (addedChanged) ProcessGameObjects("abl.", "Abilities");
                    else
                    {
                        XDocument xmlContent = new XDocument(AbilityDataFromFqnListAsXElement(itmList, addedChanged));
                        WriteFile(xmlContent, filename, append);
                    }
                }

                // This section of code is to aid in exploring the unknown ability effect field names
                string effKeyList = String.Join(Environment.NewLine, currentDom.abilityLoader.effKeys);
                WriteFile(effKeyList, "effKeys.txt", false);

                currentDom.abilityLoader.effWithUnknowns = currentDom.abilityLoader.effWithUnknowns.Distinct().OrderBy(o => o).ToList();

                string effUnknowns = String.Join(Environment.NewLine, currentDom.abilityLoader.effWithUnknowns);
                WriteFile(effUnknowns, "effUnknowns.txt", false);

                currentDom.abilityLoader.effWithUnknowns = new List<string>();
                currentDom.abilityLoader.effKeys = new List<string>();

            }
            //end of ability effect code

            //MessageBox.Show("the Ability lists has been generated there are " + ttl + " Abilities.");
            EnableButtons();
        }

        #region SQL

        public string AbilityDataFromFqnListToSQL(IEnumerable<GomLib.GomObject> itmList)
        {
            double i = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            foreach (var gomItm in itmList)
            {
                GomLib.Models.Ability itm = new GomLib.Models.Ability();
                currentDom.abilityLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                addtolist2("Ability Name: " + itm.Name);

                AddAbilityToSQL(itm);
                i++;
            }
            addtolist("the ability data has been read there were " + i + " abilities parsed.");
            return txtFile.ToString();
        }

        private void AddAbilityToSQL(GomLib.Models.Ability itm)
        {
            string s = "', '";
            string value = "('" + sqlSani(patchVersion) + s + s + sqlSani(itm.Name) + s + itm.NodeId + s + itm.NameId + s + sqlSani(itm.Description) + s + itm.DescriptionId + s + itm.Fqn + s + itm.Level + s + sqlSani(itm.Icon) + s + itm.IsHidden + s + itm.IsPassive + s + itm.Cooldown + s + itm.CastingTime + s + itm.ForceCost + s + itm.EnergyCost + s + itm.ApCost + s + itm.ApType.ToString() + s + itm.MinRange + s + itm.MaxRange + s + itm.GCD + s + itm.GcdOverride + s + itm.ModalGroup + s + itm.SharedCooldown + s + sqlSani(itm.TalentTokens) + s + sqlSani(itm.AbilityTokens) + s + itm.TargetArc + s + itm.TargetArcOffset + s + (GomLib.Models.TargetRule)itm.TargetRule + s + itm.LineOfSightCheck + s + itm.Pushback + s + itm.IgnoreAlacrity + s + itm.GetHashCode() + "')";
            sqlAddTransactionValue(value);
        }
        #endregion
        #region text
        private string AbilityDataFromFqnList(IEnumerable<GomLib.GomObject> itmList)
        {
            double i = 0;
            double e = 0;
            string n = Environment.NewLine;

            var txtFile = new StringBuilder();
            foreach (var gomItm in itmList)
            {
                GomLib.Models.Ability itm = new GomLib.Models.Ability();
                currentDom.abilityLoader.Load(itm, gomItm);

                if (itm.Name == null)
                {
                    addtolist2("Effect Id: " + itm.Fqn);

                    //txtFile.Append("------------------------------------------------------------");
                    txtFile.Append("Effect Name: " + itm.Fqn + n);
                    txtFile.Append("Effect NodeId: " + itm.NodeId + n + n);
                    //txtFile.Append("------------------------------------------------------------");

                    e++;
                }
                else
                {
                    addtolist2("Ability Name: " + itm.Name);

                    txtFile.Append("Name: " + itm.Name + n);
                    txtFile.Append("NodeId: " + itm.NodeId + n);
                    txtFile.Append("Id: " + itm.Id + n);
                    txtFile.Append("  Flags (Passive/Hidden): " + itm.IsPassive + "/" + itm.IsHidden + n);
                    txtFile.Append("  Description: " + itm.Description + " (" + itm.DescriptionId + ")" + n);
                    txtFile.Append("  Tokens: " + itm.TalentTokens + n);
                    txtFile.Append("  Fqn: " + itm.Fqn + n);
                    txtFile.Append("  Icon: " + itm.Icon + n);
                    txtFile.Append("  level: " + itm.Level + n);
                    txtFile.Append("  Range (Min/Max): " + itm.MinRange + "/" + itm.MaxRange + n);
                    if (itm.ApCost != 0) { txtFile.Append("  Ammo/Heat Cost: " + itm.ApCost + n); }
                    if (itm.EnergyCost != 0) { txtFile.Append("  Energy Cost: " + itm.EnergyCost + n); }
                    if (itm.ForceCost != 0) { txtFile.Append("  Force Cost: " + itm.ForceCost + n); }
                    if (itm.ChannelingTime != 0) { txtFile.Append("  Channeling Time: " + itm.ChannelingTime + n); }
                    else
                    {
                        if (itm.CastingTime != 0) { txtFile.Append("  Casting Time: " + itm.CastingTime + n); }
                        else { txtFile.Append("  Casting Time: Instant" + n); }
                    }
                    txtFile.Append("  Cooldown Time: " + itm.Cooldown + n);
                    if (itm.GCD != -1) { txtFile.Append("  GCD: " + itm.GCD + n); } else { txtFile.Append("  GCD: No GCD" + n); }
                    if (itm.GcdOverride) { txtFile.Append("  Overrides GCD: " + itm.GcdOverride + n); }
                    txtFile.Append("  Uses Pushback: " + itm.Pushback + n);
                    txtFile.Append("  Ignores Alacrity: " + itm.IgnoreAlacrity + n);
                    txtFile.Append("  LoS Check: " + itm.LineOfSightCheck + n);
                    if (itm.ModalGroup != 0) { txtFile.Append("  Modal Group: " + itm.ModalGroup + n); }
                    if (itm.SharedCooldown != 0) { txtFile.Append("  Shared Cooldown: " + itm.SharedCooldown + n); }
                    if (itm.TargetArc != 0 && itm.TargetArcOffset != 0) { txtFile.Append("  Target Arc/Offset: " + itm.TargetArc + "/" + itm.TargetArcOffset + n); }
                    txtFile.Append("  Target Rule: " + (GomLib.Models.TargetRule)itm.TargetRule + n + n);

                    if (itm.Name != null)
                    {
                        string name = itm.Name.Replace("'", " ");
                    }

                    //sqlexec("INSERT INTO `abilities` (`ability_idc`, `quest_name`, `quest_nodeid`, `quest_id`, `IsBonus`, `BonusShareable`, `Branches`, `CanAbandon`, `Category`, `CategoryId`, `Classes`, `Difficulty`, `Fqn`, `Icon`, `IsClassQuest`, `IsHidden`, `IsRepeatable`, `Items`, `RequiredLevel`, `XpLevel`) VALUES (NULL, '" + name + "', '" + itm.NodeId + "', '" + itm.Id + "', '" + itm.IsBonus + "', '" + itm.BonusShareable + "', '" + itm.Branches + "', '" + itm.CanAbandon + "', '" + itm.Category + "', '" + itm.CategoryId + "', '" + itm.Classes + "', '" + itm.Difficulty + "', '" + itm.Fqn + "', '" + itm.Icon + "', '" + itm.IsClassQuest + "', '" + itm.IsHidden + "', '" + itm.IsRepeatable + "', '" + itm.Items + "', '" + itm.RequiredLevel + "', '" + itm.XpLevel + "');");
                    i++;
                }

            }
            addtolist("the Ability lists has been generated there are " + i + " Abilities");
            return txtFile.ToString();
        }
        #endregion
        #region XML
        private XElement AbilityDataFromFqnListAsXElement(IEnumerable<GomLib.GomObject> itmList, bool addedChangedOnly)
        {
            double i = 0;
            //double e = 0;
            XElement abilities = new XElement("Abilities");
            foreach (var gomItm in itmList)
            {
                GomLib.Models.Ability itm = new GomLib.Models.Ability();
                currentDom.abilityLoader.Load(itm, gomItm);

                addtolist2("Ability Name: " + itm.Name);
                var ability = AbilityToXElement(itm);
                ability.Add(ReferencesToXElement(gomItm.References));
                abilities.Add(ability);

                i++;
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Abilities to the loaded Patch");

                XElement addedItems = FindChangedEntries(abilities, "Abilities", "Ability");
                addedItems = SortAbilities(addedItems);
                addtolist("The Ability list has been generated there are " + addedItems.Elements("Ability").Count() + " new/changed Abilities");
                abilities = null;
                return addedItems;
            }

            abilities = SortAbilities(abilities);
            addtolist("The Ability list has been generated there are " + i + " Abilities");
            return abilities;
        }

        private XElement SortAbilities(XElement abilities)
        {
            //addtolist("Sorting Abilities");
            abilities.ReplaceNodes(abilities.Elements("Ability")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn"))
                .ThenBy(x => (string)x.Element("Name"))
                .ThenBy(x => (string)x.Attribute("Id")));
            return abilities;
        }

        public XElement AbilityPackageToXElement(GomLib.Models.AbilityPackage itm)
        {
            return AbilityPackageToXElement(itm, false);
        }

        private XElement AbilityPackageToXElement(GomLib.Models.AbilityPackage itm, bool overrideVerbose)
        {
            XElement ablPackage = new XElement("AbilityPackage");
            if (itm != null)
            {
                int i = 1;
                ablPackage.Add(new XAttribute("Id", itm.Fqn));
                if(verbose) ablPackage.Add(new XElement("NodeId", itm.Id)); //, new XAttribute("Hash", itm.GetHashCode()));
                foreach (var ablPack in itm.PackageAbilities)
                {
                    var levels = ablPack.Levels[0] + "-" + ablPack.Levels[ablPack.Levels.Count() - 1];
                    XElement package = AbilityToXElement(ablPack.Ability, overrideVerbose);
                        //new XElement("Package", new XAttribute("Index", i));
                        //new XElement("CategoryName", ablPack.CategoryName,
                        //new XAttribute("Id", ablPack.CategoryNameId)),
                    
                    if(verbose && !overrideVerbose)
                    {
                        package.Add(new XElement("PackageAutoAcquire", ablPack.AutoAcquire),
                        new XElement("PackageLevel", ablPack.Level),
                        new XElement("PackageLevels", levels), //String.Join(",", ablPack.Levels)),
                        new XElement("PackageScales", ablPack.Scales));
                        if (itm.IsUtilityPackage)
                        {
                            package.Add(new XElement("UtilityTier", ablPack.UtilityTier),
                               new XElement("UtilityPosition", ablPack.UtilityPosition));
                        }
                    }
                    //package.Add(AbilityToXElement(ablPack.Ability));
                    ablPackage.Add(package); //add ability to AbilityPackage
                    i++;
                }
                if (itm.IsUtilityPackage)
                {
                    foreach (var talPack in itm.PackageTalents)
                    {
                        XElement package = ConvertToXElement(talPack.Talent, overrideVerbose);

                        if (verbose && !overrideVerbose)
                        {
                            package.Add(new XElement("UtilityTier", talPack.UtilityTier),
                            new XElement("UtilityPosition", talPack.UtilityPosition));
                        }
                        ablPackage.Add(package); //add Talent to AbilityPackage
                        i++;
                    }
                }
            }
            return ablPackage;
        }

        public XElement AbilityToXElement(GomObject gomItm)
        {
            return AbilityToXElement(gomItm, false);
        }
        public XElement AbilityToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                GomLib.Models.Ability itm = new GomLib.Models.Ability();
                currentDom.abilityLoader.Load(itm, gomItm);
                return AbilityToXElement(itm, overrideVerbose);
            }
            return null;
        }
        public static XElement AbilityToXElement(GomLib.Models.Ability itm)
        {
            return AbilityToXElement(itm, false);
        }
        public static XElement AbilityToXElement(GomLib.Models.Ability itm, bool overrideVerbose)
        {
            XElement ability = new XElement("Ability");
            if (itm != null)
            {
                /*if (!File.Exists(String.Format("{0}{1}/icons/{2}.dds", Config.ExtractPath, prefix, itm.Icon)) && itm.Icon != null)
                {
                    var stream = itm._dom._assets.FindFile(String.Format("/resources/gfx/icons/{0}.dds", itm.Icon));
                    if (stream != null)
                        WriteFile((MemoryStream)stream.OpenCopyInMemory(), String.Format("/icons/{0}.dds", itm.Icon));
                }*/
                if (itm.Fqn == null) return ability;
                ability.Add(new XElement("Fqn", itm.Fqn),
                        new XAttribute("Id", itm.Id),
                        new XElement("Name", itm.Name),
                        new XElement("Description", itm.Description));

                if (verbose && !overrideVerbose)
                {
                    /*ability.Element("Name").RemoveAll(); //removes base text to replace with localized variants.
                    for (int i = 0; i < localizations.Count; i++)
                    {
                        if (itm.LocalizedName[localizations[i]] != "")
                        {
                            ability.Element("Name").Add(new XElement(localizations[i], itm.LocalizedName[localizations[i]]));
                        }
                    }

                    ability.Element("Description").RemoveAll();
                    for (int i = 0; i < localizations.Count; i++)
                    {
                        if (itm.LocalizedDescription[localizations[i]] != "")
                        {
                            ability.Element("Description").Add(new XElement(localizations[i], itm.LocalizedDescription[localizations[i]]));
                        }
                    } */
                    ability.Element("Name").Add(new XAttribute("Id", itm.NameId));
                    ability.Element("Fqn").Add(new XAttribute("NodeId", itm.NodeId));
                    ability.Element("Description").Add(new XAttribute("Id", itm.DescriptionId));
                    ability.Add(new XElement("IsPassive", itm.IsPassive),
                        new XElement("IsHidden", itm.IsHidden),
                        new XElement("Tokens", (itm.DescriptionTokens ?? new Dictionary<int, Dictionary<string, object>>()).Select(x => TokenToXElement(x))),                    
                        new XElement("Icon", itm.Icon),
                        new XElement("level", itm.Level),
                        new XElement("MinRange", itm.MinRange * 10),
                        new XElement("MaxRange", itm.MaxRange * 10),
                        new XElement("AmmoHeatCost", itm.ApCost),
                        new XElement("EnergyCost", itm.EnergyCost),
                        new XElement("ForceCost", itm.ForceCost),
                        new XElement("ChannelingTime", itm.ChannelingTime),
                        new XElement("CastingTime", itm.CastingTime),
                        new XElement("CooldownTime", itm.Cooldown),
                        new XElement("GCD", itm.GCD),
                        new XElement("OverridesGCD", itm.GcdOverride),
                        new XElement("UsesPushback", itm.Pushback),
                        new XElement("IgnoresAlacrity", itm.IgnoreAlacrity),
                        new XElement("LoSCheck", itm.LineOfSightCheck),
                        new XElement("ModalGroup", itm.ModalGroup),
                        new XElement("SharedCooldown", itm.SharedCooldown),
                        new XElement("TargetArc", itm.TargetArc),
                        new XElement("TargetArcOffset", itm.TargetArcOffset),
                        new XElement("TargetRule", (GomLib.Models.TargetRule)itm.TargetRule));
                }
                ability.Elements().Where(x => x.Value == "0" || x.IsEmpty).Remove();
            }
            return ability;
        }

        public static XElement TokenToXElement(KeyValuePair<int, Dictionary<string, object>> data)
        {
            return new XElement("Token", new XAttribute("Id", data.Key),
                data.Value.Select(x => new XElement(x.Key, x.Value)));
        }
        #endregion
    }
}
