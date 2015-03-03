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
using MessageBox = System.Windows.Forms.MessageBox;

namespace tor_tools
{
    public partial class Tools
    {
        public void getItemApps()
        {
            Clearlist2();
            ClearProgress();

            LoadData();
            var itmList = currentDom.GetObjectsStartingWith("ipp.");

            if (chkBuildCompare.Checked) ProcessGameObjects("ipp.", "ItemAppearances");
            else
            {
                int i = 0;
                int count = itmList.Count();
                ClearProgress();
                foreach (var itm in itmList)
                {
                    i++;
                    progressUpdate(i, count);
                    addtolist2(itm.Name);

                    WriteFile(new XDocument(new GomLib.Models.GameObject().ToXElement(itm)), itm.Name.Replace(".", "\\") + ".ipp", false);
                }
            }

            EnableButtons();
        }
        #region SQL
        public string ItemDataFromFqnListToSQL(IEnumerable<GomLib.GomObject> itmList)
        {
            int i = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();

            sqlTransactionsInitialize(initTable["Items"].InitBegin, initTable["Items"].InitEnd);

            var count = itmList.Count();
            foreach (var gomItm in itmList)
            {
                progressUpdate(i, count);
                GomLib.Models.Item itm = new GomLib.Models.Item();
                currentDom.itemLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                addtolist2("ItemName: " + itm.Name);

                AddItemToSQL(itm);
                i++;
            }
            sqlTransactionsFlush();

            addtolist("the item mysql table has been generated there were " + i + " items parsed.");
            ClearProgress();
            return txtFile.ToString();
        }

        private void AddItemToSQL(GomLib.Models.Item itm)
        {
            string value = itm.ToSQL(patchVersion);
            sqlAddTransactionValue(value);
        }
        #endregion
        #region XML

        /* code moved to GomLib.Models.Item.cs */

        private XElement SortItems(XElement items)
        {
            //addtolist("Sorting Items");
            items.ReplaceNodes(items.Elements("Item")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn")));
                //.ThenBy(x => (string)x.Element("Name")));
            return items;
        }

        
        #endregion
    }
}