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
        /* code moved to GomLib.Models.Npc.cs */

        //sqlexec("INSERT INTO `npc` (`npc_name`,`npc_nodeid`, `npc_id`, `ClassSpec`, `Codex`, `CompanionOverride`, `Conversation`, `ConversationFqn`, `DifficultyFlags`, `Faction`, `Fqn`, `IsClassTrainer`, `IsVendor`, `LootTableId`, `MaxLevel`, `MinLevel`, `ProfessionTrained`, `Title`, `Toughness`, `VendorPackages`) VALUES ('" + insert_name + "', '" + itm.NodeId + "', '" + itm.Id + "', '" + itm.ClassSpec + "', '" + itm.Codex + "', '" + itm.CompanionOverride + "', '" + itm.Conversation + "', '" + itm.ConversationFqn + "', '" + itm.DifficultyFlags + "', '" + itm.Faction + "', '" + itm.Fqn + "', '" + itm.IsClassTrainer + "', '" + itm.IsVendor + "', '" + itm.LootTableId + "', '" + itm.MaxLevel + "', '" + itm.MinLevel + "', '" + itm.ProfessionTrained + "', '" + insert_title + "', '" + itm.Toughness + "', '" + itm.VendorPackages + "');");

        /* code moved to GomLib.Models.Npc.cs */
        /* code moved to GomLib.Models.Appearances.cs */

        private XElement SortNpcs(XElement npcs)
        {
            //addtolist("Sorting Npcs");
            npcs.ReplaceNodes(npcs.Elements("Npc")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn"))
                .ThenBy(x => (string)x.Element("Name"))
                .ThenBy(x => (string)x.Attribute("Id")));
            return npcs;
        }
    }
}
