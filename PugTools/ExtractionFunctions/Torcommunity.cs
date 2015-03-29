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
        public void getTorc()
        {
            Clearlist2();

            LoadData();

            //getTooltips();

            //getAuctionCats();

            getitemIds();
            
            EnableButtons();
        }

        public void getAuctionCats()
        {
            ClearProgress();
            LoadData();
            GomLib.Models.AuctionCategory.Load(currentDom);
            var gomList = GomLib.Models.AuctionCategory.AuctionCategoryList;
            var count = gomList.Count();
            int i = 0;
            WriteFile("", "aucCats.txt", false);
            foreach (var gom in gomList)
            {
                progressUpdate(i, count);
                WriteFile(String.Join(Environment.NewLine, Environment.NewLine + gom.Value.Name + " (" + gom.Value.Id + ")", String.Join(Environment.NewLine, gom.Value.SubCategories.Select(x => "  " + x.Value.Name + " (" + x.Value.Id + ")").ToList())), "aucCats.txt", true);
                i++;
            }
        }

        public void getitemIds()
        {
            ClearProgress();
            LoadData();
            var gomList = currentDom.GetObjectsStartingWith("itm.");
            var count = gomList.Count();
            int i = 0;
            WriteFile("", "itemIds.txt", false);
            foreach (var gom in gomList)
            {
                
                progressUpdate(i, count);
                var itm = currentDom.itemLoader.Load(gom);
                WriteFile(String.Format("{0}: http://torcommunity.com/db/{1}{2}", itm.Name, itm.Base62Id, Environment.NewLine), "itemIds.txt", true);
                i++;
            }
        }
    }
}
