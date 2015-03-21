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
        public void getTooltips()
        {
            Clearlist2();

            LoadData();
            Dictionary<string, string> gameObjects = new Dictionary<string, string>
            {
                /*{"ach.", true},
                {"abl.", true},
                {"apn.", true},
                {"cdx.", true},
                {"cnv.", true},
                {"npc.", true},
                {"qst.", true},
                {"tal.", true},
                {"sche", true},
                {"dec.", true},*/
                {"itm.", "Item"}/*,
                {"apt.", true},
                {"apc.", true},
                {"class.",true},
                {"ipp.",true},
                {"npp.",true}*/
            };
            for (int f = 0; f < gameObjects.Count; f++)
            {
                var gameObj = gameObjects.ElementAt(f);
                ClearProgress();
                var gomList = currentDom.GetObjectsStartingWith(gameObj.Key).Where(x => !x.Name.Contains("/"));
                var count = gomList.Count();
                int i = 0;
                addtolist2(String.Format("Checking {0}", gameObj.Key));
                List<object> iList = new List<object>();
                foreach (var gom in gomList)
                {
                    progressUpdate(i, count);
                    var itm = new GomLib.Models.GameObject().Load(gom);
                    bool okToOutput = true;
                    if (chkBuildCompare.Checked)
                    {
                        var itm2 = new GomLib.Models.GameObject().Load(gom);
                        if (itm == null) continue;
                        if (itm.GetHashCode() == itm2.GetHashCode())
                            okToOutput = false;
                    }

                    if (okToOutput)
                    {
                        GomLib.Models.Tooltip t = new GomLib.Models.Tooltip(itm);
                        iList.Add(t);
                    }
                    i++;
                }
                ObjectListAsSql(gameObj.Value, "Tooltip", iList);
            }

            /*addtolist("Verifying GameObject Hashes");
            bool failed = false;
            Dictionary<string, string> protoGameObjects = new Dictionary<string, string>
            {
                {"mtxStorefrontInfoPrototype", "mtxStorefrontData"},
                {"colCollectionItemsPrototype", "colCollectionItemsData"},
                {"chrCompanionInfo_Prototype", "chrCompanionInfoData"},
                {"scFFShipsDataPrototype", "scFFShipsData"},
                {"wevConquestInfosPrototype", "wevConquestTable"},
                {"achCategoriesTable_Prototype", "achCategoriesData"}
            };

            for (int f = 0; f < protoGameObjects.Count; f++)
            {
                var gameObj = protoGameObjects.ElementAt(f);
                Dictionary<object, object> currentDataProto = new Dictionary<object, object>();
                GomObject currentDataObject = currentDom.GetObject(gameObj.Key);
                if (currentDataObject != null) //fix to ensure old game assets don't throw exceptions.
                {
                    currentDataProto = currentDataObject.Data.Get<Dictionary<object, object>>(gameObj.Value);
                    currentDataObject.Unload();
                }

                ClearProgress();
                var count = currentDataProto.Count();
                int i = 0;
                addtolist2(String.Format("Checking {0}", gameObj.Key));
                bool localfail = false;
                foreach (var gom in currentDataProto)
                {
                    progressUpdate(i, count);
                    var itm = GomLib.Models.PseudoGameObject.LoadFromProtoName(gameObj.Key, currentDom, gom.Key, (GomObjectData)gom.Value);
                    var itm2 = GomLib.Models.PseudoGameObject.LoadFromProtoName(gameObj.Key, currentDom, gom.Key, (GomObjectData)gom.Value);
                    if (itm == null) continue;
                    if (itm.GetHashCode() != itm2.GetHashCode())
                    {
                        addtolist2(String.Format("Failed: {0}", gameObj.Key));
                        failed = true;
                        localfail = true;
                        break; //break inner loop
                    }
                    i++;
                }
                if (!localfail)
                    addtolist2(String.Format("Passed: {0}", gameObj.Key));
            }
            completeString = "Passed.";
            if (failed)
                completeString = "Failed.";
            addtolist(completeString);*/

            EnableButtons();
        }

    }
}
