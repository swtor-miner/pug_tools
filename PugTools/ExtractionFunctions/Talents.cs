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
        private string TalentDataFromFqnList(IEnumerable<GomLib.GomObject> itmList)
        {
            double i = 0;
            string n = Environment.NewLine;

            var txtFile = new StringBuilder();
            var talLoad = new GomLib.ModelLoader.TalentLoader(currentDom);
            foreach (var gomItm in itmList)
            {
                GomLib.Models.Talent itm = new GomLib.Models.Talent();
                talLoad.Load(itm, gomItm);

                addtolist2("Talent Title: " + itm.Name);

                txtFile.Append("------------------------" + n);
                txtFile.Append("Talent Title: " + itm.Name + n);
                txtFile.Append("Talent NodeId: " + itm.NodeId + n);
                txtFile.Append("Talent Id: " + itm.Id + n);
                //txtFile.Append("------------------------" + n );
                //txtFile.Append("Talent INFO" + n );
                txtFile.Append("  Fqn: " + itm.Fqn + n);
                txtFile.Append("------------------------" + n + n);
                i++;
            }
            addtolist("The Talent list has been generated there are " + i + " Talents");
            return txtFile.ToString();
        }

        private XElement SortTalents(XElement talents)
        {
            //addtolist("Sorting Talents");
            talents.ReplaceNodes(talents.Elements("Talent")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn"))
                .ThenBy(x => (string)x.Element("Title"))
                .ThenBy(x => (string)x.Element("ID")));
            return talents;
        }

        /* code moved to GomLib.Models.Talent.cs */
    }
}
