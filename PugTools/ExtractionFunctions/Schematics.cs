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

        #region Deprecated
        /*private string SchematicDataFromFqnList(IEnumerable<GomLib.GomObject> itmList)
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
        }*/
        #endregion

        private XElement SortSchematics(XElement schematics)
        {
            //addtolist("Sorting Talents");
            schematics.ReplaceNodes(schematics.Elements("Schematic")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn")));
            return schematics;
        }

        /* code moved to GomLib.Models.Schematic.cs */
        
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
