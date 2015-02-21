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
        #region Collections
        public void getCollect()
        {
            Clearlist2();

            LoadData();
            var collectionDataProto = currentDom.GetObject("colCollectionItemsPrototype").Data.Get<Dictionary<object, object>>("colCollectionItemsData");
            //double ttl = collectionDataProto.Count();
            bool append = false;
            bool addedChanged = false;
            string changed = "";
            if(chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            var filename = changed + "Collections.xml";
            if(outputTypeName == "Text")
            {
                filename = changed + "Collections.txt";
                string generatedContent = CollectionDataFromFqnList(collectionDataProto);
                WriteFile(generatedContent, filename, append);
            }
            else
            {
                //if (addedChanged)
                    ProcessProtoData("Collections", "colCollectionItemsPrototype", "colCollectionItemsData"); //collectionDataProto);
                /*else
                {
                    XDocument xmlContent = new XDocument(CollectionDataFromFqnListAsXElement(collectionDataProto, addedChanged));
                    WriteFile(xmlContent, filename, append);
                }*/
            }

            //MessageBox.Show("The Collection lists has been generated there are " + ttl + " Collection Entries");
            if (MessageBox.Show("Output Icons?", "Icon Output", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (!System.IO.Directory.Exists(Config.ExtractPath + "MtxIcons\\")) { System.IO.Directory.CreateDirectory(Config.ExtractPath + "MtxIcons\\"); }
                this.currentAssets.icons.SaveMtxTo(Config.ExtractPath + "MtxIcons\\");
            }

            EnableButtons();
        }

        private string CollectionDataFromFqnList(Dictionary<object, object> collectionDataProto)
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
        }

        private XElement CollectionDataFromFqnListAsXElement(Dictionary<object, object> collectionDataProto, bool addedChangedOnly)
        {
            double i = 0;
            XElement collections = new XElement("Collections");
            foreach (var collection in collectionDataProto)
            {
                GomLib.Models.Collection col = new GomLib.Models.Collection();
                currentDom.collectionLoader.Load(col, (long)collection.Key, (GomLib.GomObjectData)collection.Value);

                addtolist2("Collection Title: " + col.Name);

                XElement collectionXE = CollectionToXElement(col);
                collections.Add(collectionXE); //add collectionXE to collections
                i++;
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Collection Entries to the loaded Patch");
                XElement addedItems = FindChangedEntries(collections, "Collections", "Collection");
                addedItems = SortCollections(addedItems);
                addtolist("The Collection list has been generated there are " + addedItems.Elements("Collection").Count() + " new/changed Collections");
                collections = null;
                return addedItems;
            }

            collections = SortCollections(collections);

            addtolist("The Collection list has been generated there are " + i + " Collections");
            return collections;
        }

        private XElement SortCollections(XElement collections)
        {
            //addtolist("Sorting Collection Entries");
            collections.ReplaceNodes(collections.Elements("Collection")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Attribute("Id"))
                .ThenBy(x => (string)x.Element("Name")));
            return collections;
        }

        private XElement CollectionToXElement(GomLib.Models.Collection col) //split to see if it was necessary to loop through linked collections. Didn't seem like it.
        {
            XElement collectionXE = new XElement("Collection");

            collectionXE.Add(new XElement("Name", col.Name),
                new XElement("RarityDescription", col.RarityDesc),
                new XAttribute("Id", col.Id));

            if (col.unknowntext != "") { collectionXE.Add(new XElement("Unknown", col.unknowntext)); }
            else { collectionXE.Add(new XElement("Unknown")); }

            collectionXE.Add(new XElement("Requirements"),
                new XElement("Icon", col.Icon));
            int b = 0;
            foreach (var bullet in col.BulletPoints)
            {
                collectionXE.Add(new XElement("Info", bullet, new XAttribute("Id", b)));
                b++;
            }

            XElement grantedItems = new XElement("GrantedItems");
            foreach (var grantedItem in col.ItemList)
            {
                grantedItems.Add(ConvertToXElement(grantedItem, true));
            }
            collectionXE.Add(grantedItems);

            XElement grantedAbilities = new XElement("GrantedAbilities");
            foreach (var grantedAbility in col.AbilityList)
            {
                grantedAbilities.Add(ConvertToXElement(grantedAbility, true));
            }
            collectionXE.Add(grantedAbilities);

            XElement alternateUnlocks = new XElement("AlternateUnlocks");
            foreach (var altUnlock in col.AlternateUnlocksMap)
            {
                XElement alternate = ConvertToXElement(altUnlock.Key, col._dom, true);
                foreach (var baseAlternate in altUnlock.Value)
                {
                    alternate.Add(new XElement("AlternateUnlockFor", ConvertToXElement(baseAlternate, col._dom, true)));
                }
                alternateUnlocks.Add(alternate);
            }
            collectionXE.Add(alternateUnlocks);

            return collectionXE;
        }
        #endregion

        #region MTX
        public void getMtx()
        {
            Clearlist2();
            
            LoadData();
            var MtxStoreFrontData = currentDom.GetObject("mtxStorefrontInfoPrototype").Data.Get<Dictionary<object, object>>("mtxStorefrontData");
            //double ttl = mtxStorefrontDataProto.Count();
            bool append = false;
            bool addedChanged = false;
            string changed = "";
            if(chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            var filename = changed + "MtxStoreFronts.xml";
            if(outputTypeName == "Text")
            {
                filename = changed + "MtxStoreFronts.txt";
                string generatedContent = MtxStoreFrontDataFromFqnList(MtxStoreFrontData);
                WriteFile(generatedContent, filename, append);
            }
            else
            {
                //if (addedChanged)
                    ProcessProtoData("MtxStoreFronts", "mtxStorefrontInfoPrototype", "mtxStorefrontData"); //MtxStoreFrontData);
                /*else
                {
                    XDocument xmlContent = new XDocument(MtxStoreFrontDataFromFqnListAsXElement(MtxStoreFrontData, addedChanged));
                    WriteFile(xmlContent, filename, append);
                }*/
            }

            //MessageBox.Show("The MtxStoreFront lists has been generated there are " + ttl + " MtxStoreFront Entries");
            if (MessageBox.Show("Output Icons?", "Icon Output", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (!System.IO.Directory.Exists(Config.ExtractPath + "MtxIcons\\")) { System.IO.Directory.CreateDirectory(Config.ExtractPath + "MtxIcons\\"); }
                this.currentAssets.icons.SaveMtxTo(Config.ExtractPath + "MtxIcons\\");
            }
            EnableButtons();
        }

        private string MtxStoreFrontDataFromFqnList(Dictionary<object, object> mtxStorefrontDataProto)
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
        }

        private XElement MtxStoreFrontDataFromFqnListAsXElement(Dictionary<object, object> mtxStorefrontDataProto, bool addedChangedOnly)
        {
            double i = 0;
            XElement mtxStorefrontEntries = new XElement("MtxStoreFronts");
            foreach (var mtxStorefrontEntry in mtxStorefrontDataProto)
            {
                GomLib.Models.MtxStorefrontEntry col = new GomLib.Models.MtxStorefrontEntry();
                currentDom.mtxStorefrontEntryLoader.Load(col, (long)mtxStorefrontEntry.Key, (GomLib.GomObjectData)mtxStorefrontEntry.Value);

                addtolist2("MtxStoreFront Title: " + col.Name);

                XElement mtxStorefrontEntryXE = MtxStoreFrontToXElement(col);
                mtxStorefrontEntries.Add(mtxStorefrontEntryXE); //add mtxStorefrontEntrieXE to mtxStorefrontEntries
                i++;
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current MtxStoreFront Entries to the loaded Patch");
                XElement addedItems = FindChangedEntries(mtxStorefrontEntries, "MtxStoreFronts", "MtxStoreFront");
                //addedItems = SortCollections(addedItems);
                addtolist("The MtxStoreFront list has been generated there are " + addedItems.Elements("MtxStoreFront").Count() + " new/changed MtxStoreFronts");
                mtxStorefrontEntries = null;
                return addedItems;
            }

            //mtxStorefrontEntries = SortCollections(mtxStorefrontEntries);

            addtolist("The MtxStoreFront list has been generated there are " + i + " MtxStoreFronts");
            return mtxStorefrontEntries;
        }

        private static XElement MtxStoreFrontToXElement(GomLib.Models.MtxStorefrontEntry col) //split to see if it was necessary to loop through linked mtxStorefrontEntries. Didn't seem like it.
        {
            XElement mtxStorefrontEntryXE = new XElement("MtxStoreFront");

            mtxStorefrontEntryXE.Add(new XElement("Name", col.Name),
                new XElement("RarityDescription", col.RarityDesc),
                new XAttribute("Id", col.Id),
                new XElement("Cost", col.FullPriceCost),
                new XElement("DiscountCost", col.DiscountCost),
                new XElement("IsAccountUnlock", col.IsAccountUnlock),
                new XElement("UnknownBool", col.unknownBool2));

            if (col.unknowntext != "") { mtxStorefrontEntryXE.Add(new XElement("Unknown", col.unknowntext)); }
            else { mtxStorefrontEntryXE.Add(new XElement("Unknown")); }

            mtxStorefrontEntryXE.Add(new XElement("Requirements"),
                new XElement("Icon", col.Icon));
            int b = 0;
            foreach (var bullet in col.BulletPoints)
            {
                mtxStorefrontEntryXE.Add(new XElement("Info", bullet, new XAttribute("Id", b)));
                b++;
            }
            
            return mtxStorefrontEntryXE;
        }

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

        #region Comparison

        
        #endregion
    }
}
