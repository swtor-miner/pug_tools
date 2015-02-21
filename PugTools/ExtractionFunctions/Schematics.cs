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
        public void getSchematics()
        {
            Clearlist2();

            LoadData();
            var itmList = currentDom.GetObjectsStartingWith("schem.");
            double ttl = itmList.Count();
            bool append = false;
            bool addedChanged = false;
            string changed = "";
            if(chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            var filename = changed + "Schematics.xml";
            if(outputTypeName == "Text")
            {
                filename = changed + "Schematics.txt";
                string generatedContent = SchematicDataFromFqnList(itmList);
                WriteFile(generatedContent, filename, append);
            }
            else
            {
                if (addedChanged)
                {
                    ProcessGameObjects("schem", "Schematics");
                }
                else
                {
                    XDocument xmlContent = new XDocument(SchematicDataFromFqnListAsXElement(itmList, addedChanged));
                    WriteFile(xmlContent, filename, append);
                }
            }

            //MessageBox.Show("The Talent lists has been generated there are " + ttl + " Talents");
            EnableButtons();
        }

        private string SchematicDataFromFqnList(IEnumerable<GomLib.GomObject> itmList)
        {
            double i = 0;
            string n = Environment.NewLine;

            
            string replaceWith = "  ";
            var sortedItmList = itmList.OrderBy(x => x.Name);
            Dictionary<string, List<string>> schematics = new Dictionary<string, List<string>>();
            foreach (var gomItm in sortedItmList)
            {
                GomLib.Models.Schematic itm = new GomLib.Models.Schematic();
                currentDom.schematicLoader.Load(itm, gomItm);

                addtolist2("Schematic: " + itm.Name);
                
                var txt = new StringBuilder();

                txt.Append(String.Format("* {0}{1}", itm.Item.Name, n)); //itm.Item.Description.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith), n));
                List<string> reqs = new List<string>();
                foreach(var kvp in itm.Materials)
                {
                    var mat = itm._dom.itemLoader.Load(kvp.Key);
                    reqs.Add(String.Format("{0}x {1}", kvp.Value, mat.Name)); //, mat.Description));
                }
                if (reqs.Count > 0)
                    txt.Append(String.Format(" * {0}{1}", String.Join(n + " * ", reqs), n));
                string crewSkill = itm.CrewSkillId.ToString();
                if (!schematics.ContainsKey(crewSkill))
                {
                    schematics.Add(crewSkill, new List<string>());
                }
                schematics[crewSkill].Add(txt.ToString());
                i++;
            }
            var outputTxt = new StringBuilder();
            foreach (var kvp in schematics)
            {
                var list = kvp.Value;
                list.Sort();
                outputTxt.Append(String.Format("##{0}{1}{2}{3}{4}", kvp.Key, n, n, String.Join("", list),n,n));
            }
            addtolist("The Schematic list has been generated; there are " + i + " Schematics");
            return outputTxt.ToString();
        }

        private XElement SchematicDataFromFqnListAsXElement(IEnumerable<GomLib.GomObject> itmList, bool addedChangedOnly)
        {
            double i = 0;
            XElement schematics = new XElement("Schematics");
            foreach (var gomItm in itmList)
            {
                GomLib.Models.Schematic itm = new GomLib.Models.Schematic();
                currentDom.schematicLoader.Load(itm, gomItm);

                addtolist2("Schematic: " + itm.Name + " - " + itm.Fqn);

                XElement schem = ConvertToXElement(itm);
                schem.Add(ReferencesToXElement(gomItm.References));
                schematics.Add(schem); //add talent to talents
                i++;
            }

            /*if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Talents to the loaded Patch");
                XElement addedItems = FindChangedEntries(schematics, "Talents", "Talent");
                addedItems = SchematicTalents(addedItems);
                addtolist("The Talent list has been generated there are " + addedItems.Elements("Talent").Count() + " new/changed Talents");
                schematics = null;
                return addedItems;
            }*/

            schematics = SortSchematics(schematics);

            addtolist("The Talent list has been generated there are " + i + " Talents");
            return schematics;
        }

        private XElement SortSchematics(XElement schematics)
        {
            //addtolist("Sorting Talents");
            schematics.ReplaceNodes(schematics.Elements("Schematic")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn")));
            return schematics;
        }

        public XElement SchematicToXElement(GomObject gomItm)
        {
            return SchematicToXElement(gomItm, false);
        }

        public XElement SchematicToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                GomLib.Models.Schematic itm = new GomLib.Models.Schematic();
                currentDom.schematicLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                return SchematicToXElement(itm, overrideVerbose);
            }
            return null;
        }

        public XElement SchematicToXElement(GomLib.Models.Schematic itm) 
        {
            return SchematicToXElement(itm, false);
        }

        public XElement SchematicToXElement(GomLib.Models.Schematic itm, bool overrideVerbose)
        {
            XElement schem = new XElement("Schematic");
            if (itm == null) return schem;

            schem.Add(new XElement("Fqn", itm.Fqn),
                new XAttribute("Id", itm.NodeId),
                new XElement("CrewSkill", itm.CrewSkillId));
            
            if (itm.Name == "")
            {
                //if (itm.ItemId == 0)
                    //throw new IndexOutOfRangeException();
                var crftItm = itm._dom.itemLoader.Load(itm.ItemId);
                var crafted = ConvertToXElement(crftItm, true);
                if (crafted != null)
                {
                    crafted.Name = "CraftedItem";
                    schem.Add(crafted);
                    if (ExportICONS)
                    {
                        OutputSchematicIcon(crftItm.Icon);
                    }
                }

                XElement mats = new XElement("RequiredMaterials");
                if (itm.Materials != null)
                {
                    foreach (var kvp in itm.Materials)
                    {
                        mats.Add(RequiredMatToXElement(itm._dom, kvp.Value, kvp.Key));
                    }
                    schem.Add(mats);
                }
                XElement resmats = new XElement("ResearchReturnedMaterials");
                if (itm.Research1 != null)
                    resmats.Add(RequiredMatToXElement(itm._dom, itm.ResearchQuantity1, itm.Research1.NodeId));
                if (itm.Research2 != null)
                    resmats.Add(RequiredMatToXElement(itm._dom, itm.ResearchQuantity2, itm.Research2.NodeId));
                if (itm.Research3 != null)
                    resmats.Add(RequiredMatToXElement(itm._dom, itm.ResearchQuantity3, itm.Research3.NodeId));
                schem.Add(resmats);
            }
            else
            {
                schem.Add(new XElement("Name", itm.Name),
                    new XElement("MissionDescription", itm.MissionDescription));
            }


            if (verbose)
            {
                if (itm.Name == "")
                {

                }
                else
                {
                    schem.Add(new XElement("Yield", itm.MissionYieldDescription),
                        new XElement("IsUnlockable", itm.MissionUnlockable),
                        new XElement("Cost", itm.MissionCost),
                        new XElement("Alignment", String.Format("Light(normal/crit): {0}/{1}, Dark(normal/crit): {2}/{3}", itm.MissionLight, itm.MissionLightCrit, itm.MissionDark, itm.MissionDarkCrit)));
                }
                schem.Add(new XElement("SkillRanges",
                    new XElement("Orange", itm.SkillOrange),
                    new XElement("Yellow", itm.SkillYellow),
                    new XElement("Green", itm.SkillGreen),
                    new XElement("Grey", itm.SkillGrey)));
            }

            return schem;
        }

        private XElement RequiredMatToXElement(DataObjectModel dom, int i, ulong id)
        {
            var itm = dom.itemLoader.Load(id);
            var mat = ConvertToXElement(itm, true);
            mat.Add(new XAttribute("Quantity", i), new XElement("Icon", itm.Icon));
            if (ExportICONS)
            {
                OutputSchematicIcon(itm.Icon);
            }
            return mat;
        }

        private void OutputSchematicIcon(string icon)
        {
            if (icon == null) return;
            icon = icon.Replace("'", "");
            if (!File.Exists(String.Format("{0}{1}/icons/{2}.dds", Config.ExtractPath, prefix, icon)))
            {
                var stream = currentDom._assets.FindFile(String.Format("/resources/gfx/icons/{0}.dds", icon));
                if (stream != null)
                    WriteFile((MemoryStream)stream.OpenCopyInMemory(), String.Format("/icons/{0}.dds", icon));
            }
        }
    }
}
