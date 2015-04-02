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

            //getitemIds();

            torheadscanner();
            
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

        public void torheadscanner()
        {
            //http://www.torhead.com/item/ace+in+the+hole
            //http://www.torhead.com/item/schematic:+[artifact]+microfilament+skill+d-device
            //http://www.torhead.com/item/agent's-birthright-headgear

            ClearProgress();
            LoadData();
            var gomList = currentDom.GetObjectsStartingWith("itm.");
            var count = gomList.Count();
            int i = 0;
            Dictionary<string, GomLib.Models.Item> itemlist = new Dictionary<string, GomLib.Models.Item>();
            foreach (var gom in gomList)
            {
                progressUpdate(i, count);
                var itm = currentDom.itemLoader.Load(gom);
                if (itemlist.ContainsKey(itm.Name))
                {
                    //add code here to prioritize items based on quality/item level
                }
                else if(itm.Name != null && itm.Name != ""){
                    itemlist.Add(itm.Name, itm);
                }
                i++;
            }
            addtolist("item list created in memory, scanning torhead");
            addtolist(String.Format("Found {0} unique names, full scan", itemlist.Count));
            TimeSpan t = TimeSpan.FromSeconds(itemlist.Count/5);

            string answer = String.Format("will take {0:D2}h:{1:D2}m:{2:D2}s",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);
            addtolist(answer);
            Dictionary<string, string> nametourlmap = new Dictionary<string, string>();
            WriteFile("", "torheadurls.txt", false);
            WriteFile("", "badtorheadurls.txt", false);
            WriteFile("", "torheadurlserrors.txt", false);

            ClearProgress();
            count = itemlist.Count();
            i = 0;
            EnableButtons();
            foreach (var kvp in itemlist)
            {
                progressUpdate(i, count);
                string url = String.Format("http://www.torhead.com/item/{0}", kvp.Key.Replace(" ", "+"));
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.CreateHttp(url);
                req.AllowAutoRedirect = false;
                try
                {
                    var response = (HttpWebResponse)req.GetResponse();
                    if (response.StatusCode != HttpStatusCode.InternalServerError)
                    {
                        string loc = response.GetResponseHeader("Location");
                        nametourlmap.Add(kvp.Value.Base62Id, loc);
                        WriteFile(String.Format("{0},{1},http://www.torhead.com{2}{3}", kvp.Value.Base62Id, url, loc, Environment.NewLine), "torheadurls.txt", true);
                        addtolist2(url);
                    }
                    else
                        WriteFile(String.Format("{0},{1}{2}", kvp.Value.Base62Id, url, Environment.NewLine), "badtorheadurls.txt", true);
                }
                catch (WebException ex)
                {
                    WriteFile(String.Format("{0},{1}{2}", kvp.Value.Base62Id, url, Environment.NewLine), "torheadurlserrors.txt", true);
                    addtolist(String.Format("Error: {0}", ex.Message));
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        addtolist(String.Format("Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode));
                        addtolist(String.Format("Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription));
                    }
                }
                i++;
                Thread.Sleep(200);
            }
            addtolist("Done scanning torhead!");
        }
    }
}
