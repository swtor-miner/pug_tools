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
            string changed = "";
            if (sql)
            {
                sqlTransactionsInitialize(initTable["Abilities"].InitBegin, initTable["Abilities"].InitEnd);

                AbilityDataFromFqnListToSQL(itmList);

                sqlTransactionsFlush();
            }
            else
            {
                if(chkBuildCompare.Checked)
                {
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
