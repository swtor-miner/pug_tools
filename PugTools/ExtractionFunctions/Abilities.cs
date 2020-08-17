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
