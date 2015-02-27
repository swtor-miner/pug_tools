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
using MessageBox = System.Windows.Forms.MessageBox;

namespace tor_tools
{
    public partial class Tools
    {
        #region Deprecated
        
        /*private string CollectionDataFromFqnList(Dictionary<object, object> collectionDataProto)
        {
            double i = 0;
            string n = Environment.NewLine;

            var txtFile = new StringBuilder();
            foreach (var collection in collectionDataProto)
            {
                GomLib.Models.Collection col = new GomLib.Models.Collection();
                currentDom.collectionLoader.Load(col, (long)collection.Key, (GomLib.GomObjectData)collection.Value);

                addtolist2("Name: " + col.Name);

                txtFile.Append("------------------------" + n);
                txtFile.Append("Id: " + col.Id + n); 
                txtFile.Append("Title: " + col.Name + n);
                txtFile.Append("Rarity: " + col.RarityDesc + n);
                txtFile.Append("Unknown: " + col.unknowntext + n);
                txtFile.Append("Icon: " + col.Icon + n); 
                txtFile.Append("Info: " + n);
                foreach (var bullet in col.BulletPoints)
                {
                    txtFile.Append("  " + bullet + n);
                }
                //txtFile.Append("------------------------" + n );
                //txtFile.Append("Collection INFO" + n );
                txtFile.Append("------------------------" + n + n);
                i++;
            }
            addtolist("The Collection list has been generated there are " + i + " Collection Entries");
            return txtFile.ToString();
        }*/

        private XElement SortCollections(XElement collections)
        {
            //addtolist("Sorting Collection Entries");
            collections.ReplaceNodes(collections.Elements("Collection")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Attribute("Id"))
                .ThenBy(x => (string)x.Element("Name")));
            return collections;
        }

        /* code moved to GomLib.Models.Collection.cs */

        /*private string MtxStoreFrontDataFromFqnList(Dictionary<object, object> mtxStorefrontDataProto)
        {
            double i = 0;
            string n = Environment.NewLine;

            var txtFile = new StringBuilder();
            foreach (var mtxStorefrontEntry in mtxStorefrontDataProto)
            {
                GomLib.Models.MtxStorefrontEntry col = new GomLib.Models.MtxStorefrontEntry();
                currentDom.mtxStorefrontEntryLoader.Load(col, (long)mtxStorefrontEntry.Key, (GomLib.GomObjectData)mtxStorefrontEntry.Value);

                addtolist2("Name: " + col.Name);

                txtFile.Append("------------------------" + n);
                txtFile.Append("Id: " + col.Id + n);
                txtFile.Append("Title: " + col.Name + n);
                txtFile.Append("Rarity: " + col.RarityDesc + n);
                txtFile.Append("Unknown: " + col.unknowntext + n);
                txtFile.Append("Icon: " + col.Icon + n);
                txtFile.Append("Info: " + n);
                foreach (var bullet in col.BulletPoints)
                {
                    txtFile.Append("  " + bullet + n);
                }
                //txtFile.Append("------------------------" + n );
                //txtFile.Append("MtxStoreFront INFO" + n );
                txtFile.Append("------------------------" + n + n);
                i++;
            }
            addtolist("The MtxStoreFront list has been generated there are " + i + " MtxStoreFront Entries");
            return txtFile.ToString();
        }*/

        /* code moved to GomLib.Models.MtxStorefrontEntry.cs */


        private XElement SortMtxStoreFronts(XElement mtxStoreFront)
        {
            //addtolist("Sorting MtxStoreFront Entries");
            mtxStoreFront.ReplaceNodes(mtxStoreFront.Elements("MtxStoreFront")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Attribute("Id"))
                .ThenBy(x => (string)x.Element("Name")));
            return mtxStoreFront;
        }
        #endregion
    }
}
