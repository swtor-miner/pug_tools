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
        #region deprecated
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
                    //string generatedContent = AbilityDataFromFqnList(itmList);
                    //WriteFile(generatedContent, filename, append);
                }
                else
                {
                    /*if (addedChanged)*/ ProcessGameObjects("abl.", "Abilities");
                    /*else
                    {
                        XDocument xmlContent = new XDocument(AbilityDataFromFqnListAsXElement(itmList, addedChanged));
                        WriteFile(xmlContent, filename, append);
                    }*/
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
        #endregion

        #region XML

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

        /* code moved to GomLib.Models.AbilityPackage.cs */
                
        /* code moved to GomLib.Models.Ability.cs */
        #endregion
    }
}
