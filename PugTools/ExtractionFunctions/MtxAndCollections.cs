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
using TorLib;
using GomLib;
using GomLib.Models;

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

        #region New Cartel Market Images

        public void FindNewMtxImages()
        {
            LoadData();

            var lib = currentAssets.libraries.Where(x => x.Name.Contains("main_gfx_assets")).Single();
            string path = lib.Location;
            if (!lib.Loaded)
                lib.Load();

            Dictionary<string, string> names = MtxIcons();
            HashDictionaryInstance hashData = HashDictionaryInstance.Instance;
            if (!hashData.Loaded)
            {
                hashData.Load();
            }
            hashData.dictionary.CreateHelpers();

            addtolist2("Extracting new Cartel Images");
            foreach (var arch in lib.archives)
            {
                foreach (var file in arch.Value.files)
                {
                    HashFileInfo hashInfo = new HashFileInfo(file.FileInfo.ph, file.FileInfo.sh, file);

                    if (hashInfo.FileState == HashFileInfo.State.New && hashInfo.Extension == "dds")
                    {
                        if (file != null)
                        {
                            DevIL.ImageImporter imp = new DevIL.ImageImporter();

                            DevIL.Image dds;
                            using (MemoryStream iconStream = (MemoryStream)file.OpenCopyInMemory())
                                dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, iconStream);

                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                if (dds.Width >= 400 && dds.Height >= 400) // needs cropped
                                {
                                    DevIL.ImageExporter exp = new DevIL.ImageExporter();
                                    exp.SaveImageToStream(dds, DevIL.ImageType.Png, outputStream); //save DDS to stream in PNG format

                                    string filename = hashInfo.FileName;
                                    if (!hashInfo.IsNamed)
                                    {
                                        if (names.ContainsKey(filename))
                                            filename = names[filename];
                                    }
                                    addtolist2(filename);
                                    WriteFile(outputStream, String.Format("/MtxImages/{0}.png", filename));
                                }
                            }
                        }
                    }
                }
            }
            EnableButtons();
        }

        private Dictionary<string, string> MtxIcons()
        {
            Dictionary<object, object> mtxDataProto = new Dictionary<object, object>();
            GomObject dataObject = currentDom.GetObject("mtxStorefrontInfoPrototype");
            if (dataObject != null) //fix to ensure old game assets don't throw exceptions.
            {
                mtxDataProto = dataObject.Data.Get<Dictionary<object, object>>("mtxStorefrontData");
                dataObject.Unload();
            }

            var mtxIds = mtxDataProto.Keys;

            Dictionary<object, object> colDataProto = new Dictionary<object, object>();
            dataObject = currentDom.GetObject("colCollectionItemsPrototype");
            if (dataObject != null) //fix to ensure old game assets don't throw exceptions.
            {
                colDataProto = dataObject.Data.Get<Dictionary<object, object>>("colCollectionItemsData");
                dataObject.Unload();
            }
            var colIds = colDataProto.Keys;

            Dictionary<string, string> icons = new Dictionary<string, string>();

            ClearProgress();
            addtolist2("Loading filenames...");

            var i = 0;
            var count = mtxIds.Count + colIds.Count;
            foreach (var id in mtxIds)
            {
                progressUpdate(i, count);
                object curData;
                mtxDataProto.TryGetValue(id, out curData);
                GomLib.Models.PseudoGameObject curObj = LoadProtoObject("MtxStoreFronts", currentDom, id, curData);
                string filename = String.Format("/resources/gfx/mtxstore/{0}_400x400.dds", ((MtxStorefrontEntry)curObj).Icon);
                if (!icons.ContainsValue(filename)) icons.Add(FileNameToHash(filename), filename);
                i++;
            }
            foreach (var id in colIds)
            {
                progressUpdate(i, count);
                object curData;
                colDataProto.TryGetValue(id, out curData);
                GomLib.Models.PseudoGameObject curObj = LoadProtoObject("Collections", currentDom, id, curData);
                string filename = String.Format("/resources/gfx/mtxstore/{0}.dds", ((Collection)curObj).Icon);
                if (!icons.ContainsValue(filename)) icons.Add(FileNameToHash(filename), filename);
                i++;
            }
            ClearProgress();
            addtolist2("Done.");

            return icons;
        }

        public string FileNameToHash(string filename)
        {
            FileId id = FileId.FromFilePath(filename);
            return String.Format("{0}_{1}", id.ph, id.sh);
        }
        #endregion
    }
}
