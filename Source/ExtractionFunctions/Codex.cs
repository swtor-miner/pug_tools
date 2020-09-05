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

namespace PugTools
{
    public partial class Tools
    {
        private XElement SortCodices(XElement codices)
        {
            //addtolist("Sorting Codex Entries");
            codices.ReplaceNodes(codices.Elements("Codex")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn"))
                .ThenBy(x => (string)x.Element("Title"))
                .ThenBy(x => (string)x.Element("ID")));
            return codices;
        }

        /* code moved to GomLib.Models.Codex.cs */
    }
}
