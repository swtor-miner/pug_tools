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
        /* code moved to GomLib.Models.Quest.cs */
        /* code moved to GomLib.Models.QuestItem.cs */
        /* code moved to GomLib.Models.QuestBranch.cs */
        /* code moved to GomLib.Models.QuestStep.cs */
        /* code moved to GomLib.Models.QuestTask.cs */

        private XElement SortQuests(XElement quests)
        {
            //addtolist("Sorting Quests");
            quests.ReplaceNodes(quests.Elements("Quest")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn"))
                .ThenBy(x => (string)x.Attribute("Id")));
            return quests;
        }
    }
}