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
using MessageBox = System.Windows.Forms.MessageBox;

namespace tor_tools
{
    public partial class Tools
    {

        #region Txt
        #endregion

        #region Deprecated

        /* private XElement ShipDataFromPrototypeAsXElement(Dictionary<object, object> shipProto, bool addedChangedOnly)
        {
            double i = 0;
            double e = 0;
            int h = 0;
            XElement ships = new XElement("Ships");
            foreach (var shipEntry in shipProto)
            {
                GomLib.Models.scFFShip ship = new GomLib.Models.scFFShip();
                currentDom.scFFShipLoader.Load(ship, (long)shipEntry.Key, (GomLib.GomObjectData)shipEntry.Value);
                addtolist2("Ship Name: " + ship.Name);

                XElement shipElement = ShipToXElement(ship);
                XDocument xmlContent = new XDocument(shipElement);
                string path = "\\Ships\\";
                if (!System.IO.Directory.Exists(Config.ExtractPath + prefix + path)) { System.IO.Directory.CreateDirectory(Config.ExtractPath + prefix + path); }
                if (ship.Faction != "") path += ship.Faction + "_";
                else path += "Not_Flagged\\";
                WriteFile(xmlContent, path + ship.Name.Replace(" ", "_") + ".xml", false);
                ships.Add(shipElement);

                if (ship.IsAvailable) e++;
                if (ship.IsHidden) h++;
                if (ship.Name != null)
                {
                    string name = ship.Name.Replace("'", " ");
                }
                //sqlexec("INSERT INTO `ships` (`ship_idc`, `quest_name`, `quest_nodeid`, `quest_id`, `IsBonus`, `BonusShareable`, `Branches`, `CanAbandon`, `Category`, `CategoryId`, `Classes`, `Difficulty`, `Fqn`, `Icon`, `IsClassQuest`, `IsHidden`, `IsRepeatable`, `Items`, `RequiredLevel`, `XpLevel`) VALUES (NULL, '" + name + "', '" + ship.NodeId + "', '" + ship.Id + "', '" + ship.IsBonus + "', '" + ship.BonusShareable + "', '" + ship.Branches + "', '" + ship.CanAbandon + "', '" + ship.Category + "', '" + ship.CategoryId + "', '" + ship.Classes + "', '" + ship.Difficulty + "', '" + ship.Fqn + "', '" + ship.Icon + "', '" + ship.IsClassQuest + "', '" + ship.IsHidden + "', '" + ship.IsRepeatable + "', '" + ship.Items + "', '" + ship.RequiredLevel + "', '" + ship.XpLevel + "');");
                i++;
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Ships to the loaded Patch");

                XElement addedItems = FindChangedEntries(ships, "Ships", "Ship");
                addedItems = SortShips(addedItems);
                addtolist("The Ship list has been generated there are " + addedItems.Elements("Ship").Count() + " new/changed Ships");
                ships = null;
                return addedItems;
            }

            ships = SortShips(ships);
            addtolist("The Ship list has been generated there are " + i + " Ships");
            addtolist(e + " Ships are available.");
            addtolist(h + " Ships are hidden.");
            return ships;
        }*/
        #endregion
        #region XML

        private XElement SortShips(XElement ships)
        {
            //addtolist("Sorting Ships");
            ships.ReplaceNodes(ships.Elements("Ship")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Attribute("Id"))
                .ThenBy(x => (string)x.Element("Name"))
                .ThenBy(x => (string)x.Element("Category")));
            return ships;
        }

        /* code moved to GomLib.Models.scFFShip.cs */

        #endregion

        # region Misc
        #endregion
    }
}
