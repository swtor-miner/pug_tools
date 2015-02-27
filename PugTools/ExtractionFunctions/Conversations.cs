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
        /* code moved to GomLib.Models.Conversation.cs */

        private XElement SortConversations(XElement items)
        {
            //addtolist("Sorting Items");
            items.ReplaceNodes(items.Elements("Conversation")
                .OrderBy(x => (string)x.Attribute("Fqn")));
            return items;
        }
    }
}
