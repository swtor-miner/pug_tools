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
        #region Decorations
        
        /* code moved to GomLib.Models.Decoration.cs */

        private XElement SortDecorations(XElement decorations)
        {
            //addtolist("Sorting Decoration Entries");
            decorations.ReplaceNodes(decorations.Elements("Decoration")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Attribute("Id"))
                .ThenBy(x => (string)x.Element("Name")));
            return decorations;
        }

        /* code moved to GomLib.Models.Decoration.cs */
        #endregion

        #region Apartments
        /* code moved to GomLib.Models.Stronghold.cs */

        private XElement SortStrongholds(XElement strongholds)
        {
            //addtolist("Sorting Decoration Entries");
            strongholds.ReplaceNodes(strongholds.Elements("Stronghold")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Attribute("Id"))
                .ThenBy(x => (string)x.Element("Name")));
            return strongholds;
        }

        /* code moved to GomLib.Models.Stronghold.cs */
        #endregion

        #region Conquests

        /* code moved to GomLib.Models.Stronghold.cs */

        /*private static string ConquestObjectivesToSCSV(KeyValuePair<Dictionary<string, Dictionary<ulong, GomLib.Models.Planet>>, Dictionary<long, GomLib.Models.ConquestObjective>> itm)
        {
            string objectives = "";
            if (itm.RepeatableObjectivesList != null)
                objectives = ObjectivesToSCSV(itm.ActivePlanets, itm.RepeatableObjectivesList);
            else
                objectives = "";
            string secondObjectives = "";
            if (itm.OneTimeObjectivesList != null)
                secondObjectives = ObjectivesToSCSV(itm.ActivePlanets, itm.OneTimeObjectivesList);
            else
                secondObjectives = "";
            string conquestDates = "Event Order/Start Time: None Listed!";
            if (itm.ActiveData != null)
            {
                var data = String.Join(
                    String.Format("{0};;", Environment.NewLine),
                    itm.ActiveData.OrderBy(x => x.ActualOrderNum).Select(x => String.Format("Event Order/Start Time: {0} - {1} EST", x.ActualOrderNum, x.StartTime.ToString())));
                conquestDates = data;
            }
            //WriteFile(String.Join(Environment.NewLine, itm.Name, conquestDates, Environment.NewLine), "conquestPost.txt", true);
            return String.Join(Environment.NewLine,
                String.Format("Active Planets;Objective Points;{0};Task Description;Required Task Total", String.Join(";", itm.Keys.ToList())),
                //String.Format("Invasion Bonus:;;{0}", String.Join(";", itm.ActivePlanets.Select(y => itm.ActivePlanetObjects[y.Key].InvasionBonus.Replace("Invasion Bonus - ", "")).ToList())),
                String.Format("Repeatable Objectives:;;{0}{1}{2}",
                    String.Join(";", itm.ActivePlanets.Select(y => itm.ActivePlanetObjects[y.Key].InvasionBonus.Replace("Invasion Bonus - ", "")).ToList()),
                    Environment.NewLine,
                    objectives),
                "",
                String.Format("One Time Objectives: {0}{1}", Environment.NewLine, secondObjectives),
                Environment.NewLine
                );
        }*/

        /* code moved to GomLib.Models.Stronghold.cs */

        private XElement SortConquests(XElement strongholds)
        {
            //addtolist("Sorting Decoration Entries");
            strongholds.ReplaceNodes(strongholds.Elements("Conquest")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Attribute("Id"))
                .ThenBy(x => (string)x.Element("Name")));
            return strongholds;
        }

        /* code moved to GomLib.Models.Stronghold.cs */

        #endregion

    }
}
