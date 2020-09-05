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

namespace PugTools
{
    public partial class Tools
    {
        private XElement SortAchievements(XElement achievements)
        {
            //addtolist("Sorting Achievement Entries");
            achievements.ReplaceNodes(achievements.Elements("Achievement")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn"))
                .ThenBy(x => (string)x.Element("Name"))
                .ThenBy(x => (string)x.Element("Id")));
            return achievements;
        }

        /* code moved to GomLib.Models.Achievement.cs */
    }
}
