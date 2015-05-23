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
using System.IO.Compression;
using System.Drawing.Imaging;

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
                //{"ach.", true},
                //{"abl.", "Ability"},
                /*{"apn.", true},
                {"cdx.", true},
                {"cnv.", true},
                {"npc.", true},*/
                //{"qst.", "Mission"},
                //{"tal.", true},*/
                //{"sche", "Schematics"},
                /*{"dec.", true},*/
                {"itm.", "Item"}//,
                /*{"apt.", true},
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
                List<GomLib.Models.Tooltip> iList = new List<GomLib.Models.Tooltip>();
                foreach (var gom in gomList)
                {
                    progressUpdate(i, count);
                    
                    bool okToOutput = true;
                    if (chkBuildCompare.Checked)
                    {
                        var itm = new GomLib.Models.GameObject().Load(gom);
                        var itm2 = new GomLib.Models.GameObject().Load(gom.Id, previousDom);
                        if (itm2 != null)
                        {
                            if (itm.GetHashCode() == itm2.GetHashCode())
                                okToOutput = false;
                        }
                    }

                    if (okToOutput)
                    {
                        GomLib.Models.Tooltip t = new GomLib.Models.Tooltip(gom.Id, currentDom);
                        /*if (itm.GetType() == typeof(GomLib.Models.Item)){
                            OutputIcon(((GomLib.Models.Item)itm).Icon, "TORC");
                        }*/
                        //if(((GomLib.Models.Schematic)t.obj).NameId != 0)
                            //WriteFile(t.Base62Id + ";" + ((GomLib.Models.Schematic)t.obj).Fqn + ";" + ((GomLib.Models.Schematic)t.obj).MissionFaction + ";" + ((GomLib.Models.Schematic)t.obj).Name + Environment.NewLine, "schematics.txt", true);
                        iList.Add(t);
                    }
                    i++;
                }
                //ObjectListAsSql(gameObj.Value, "Tooltip", iList);
                CreatCompressedOutput(gameObj.Value, gameObj.Value, iList);
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

        public void CreatCompressedOutput(string prefix, string xmlRoot, IEnumerable<GomLib.Models.Tooltip> itmList)
        {
            WriteFile("", xmlRoot + "torctips.zip", false);
            HashSet<string> iconNames = new HashSet<string>();
            using (var compressStream = new MemoryStream())
            {
                //create the zip in memory
                using (var zipArchive = new ZipArchive(compressStream, ZipArchiveMode.Create, true))
                {
                    foreach (var t in itmList)
                    {
                        var torcEntry = zipArchive.CreateEntry(String.Format("tooltips/html/{0}.torctip", t.Base62Id), CompressionLevel.Fastest);
                        using (StreamWriter writer = new StreamWriter(torcEntry.Open())) //old method. Race conditions led to Central Directory corruption.
                            writer.Write(t.HTML);
                        /*using (MemoryStream htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(t.HTML ?? ""))) //see if this solves the Central Directory corruption.
                        {
                            using (var html = torcEntry.Open())
                                htmlStream.WriteTo(html);
                        }*/

                        string icon = "";
                        //if (t.obj.GetType() == typeof(GomLib.Models.Item))
                            //icon = String.Format("icons/{0}", ((GomLib.Models.Item)t.obj).Icon);
                        if (t.obj.GetType() == typeof(GomLib.Models.Ability))
                            icon = String.Format("icons/{0}", ((GomLib.Models.Ability)t.obj).Icon);
                        if (t.obj.GetType() == typeof(GomLib.Models.Quest))
                            icon = String.Format("codex/{0}", ((GomLib.Models.Quest)t.obj).Icon);
                        if (!String.IsNullOrEmpty(icon))
                        {
                            if (iconNames.Contains(icon))
                                continue;
                            else
                                iconNames.Add(icon);
                            using (MemoryStream iconStream = GetIcon(icon))
                            {
                                if (iconStream != null)
                                {
                                    var iconEntry = zipArchive.CreateEntry(String.Format("icons/{0}.jpg", GetIconFilename(icon)), CompressionLevel.Fastest);
                                    using (var a = iconEntry.Open())
                                        iconStream.WriteTo(a);
                                    //using (Writer writer = new BinaryWriter(iconEntry.Open()))
                                    //writer.(iconStream);
                                }
                            }
                        }
                    }
                }

                compressStream.Position = 0;
                WriteFile(compressStream, xmlRoot + "torctips.zip");
            }

            
        }

        private MemoryStream GetIcon(string icon)
        {
            if (icon == null) return null;
            using (var file = currentDom._assets.FindFile(String.Format("/resources/gfx/{0}.dds", icon)))
            {
                if (file == null) return null;
                var filename = String.Join("_", file.FileInfo.ph, file.FileInfo.sh);
                return GetIcon(filename, file);
            }
        }

        private MemoryStream GetIcon(string filename, TorLib.File file)
        {
            if (file == null) return null;
            /*using (*/
            MemoryStream outputStream = new MemoryStream();//)
            //{
                DevIL.ImageImporter imp = new DevIL.ImageImporter();

                DevIL.Image dds;
                using (MemoryStream iconStream = (MemoryStream)file.OpenCopyInMemory())
                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, iconStream);
                var myparams = new EncoderParameters(1);
                myparams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                DevIL.ImageExporter exp = new DevIL.ImageExporter();
                if (dds.Width == 52 && dds.Height == 52) // needs cropped
                {
                    var iconData = dds.GetImageData(0);

                    System.Drawing.Bitmap iconBM = new System.Drawing.Bitmap(iconData.Width, iconData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    for (int k = 0; k < iconData.Height * iconData.Width; k++) // loop through image data
                    {
                        Color iconPixel = Color.FromArgb(iconData.Data[k * 4 + 3], // copy pixel values
                                    iconData.Data[k * 4 + 0],
                                    iconData.Data[k * 4 + 1],
                                    iconData.Data[k * 4 + 2]);

                        iconBM.SetPixel(k % iconData.Width, (int)k / iconData.Width, iconPixel); //save pixel in new bitmap
                    }

                    Bitmap croppedIconBM = iconBM.Clone(new Rectangle(0, 0, 50, 50), iconBM.PixelFormat); // crop Bitmap
                    //croppedIconBM.Save(outputStream, System.Drawing.Imaging.ImageFormat.Png); //Bitmap to PNG Stream
                    croppedIconBM.Save(outputStream, jpgEncoder, myparams); //Bitmap to PNG Stream
                }
                else
                {
                    var iconData = dds.GetImageData(0);

                    System.Drawing.Bitmap iconBM = new System.Drawing.Bitmap(iconData.Width, iconData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    for (int k = 0; k < iconData.Height * iconData.Width; k++) // loop through image data
                    {
                        Color iconPixel = Color.FromArgb(iconData.Data[k * 4 + 3], // copy pixel values
                                    iconData.Data[k * 4 + 0],
                                    iconData.Data[k * 4 + 1],
                                    iconData.Data[k * 4 + 2]);

                        iconBM.SetPixel(k % iconData.Width, (int)k / iconData.Width, iconPixel); //save pixel in new bitmap
                    }
                    
                    iconBM.Save(outputStream, jpgEncoder, myparams); //Bitmap to PNG Stream
                    //exp.SaveImageToStream(dds, DevIL.ImageType.Png, outputStream); //save DDS to stream in PNG format
                }

                //WriteFile(outputStream, String.Format("/{0}/Images/{1}.png", directory, filename));
                return outputStream;
            //}
        }

        private void OutputIcon(string icon, string directory)
        {
            if (icon == null) return;
            using (var file = currentDom._assets.FindFile(String.Format("/resources/gfx/icons/{0}.dds", icon)))
            {
                if (file == null) return;
                var filename = String.Join("_", file.FileInfo.ph, file.FileInfo.sh);
                OutputIcon(filename, file, directory);
            }
        }

        private void OutputIcon(string filename, TorLib.File file, string directory)
        {
            if (!File.Exists(String.Format("{0}{1}/{2}/Images/{3}.dds", Config.ExtractPath, prefix, directory, filename)))
            {
                if (file != null)
                {
                    DevIL.ImageImporter imp = new DevIL.ImageImporter();

                    DevIL.Image dds;
                    using (MemoryStream iconStream = (MemoryStream)file.OpenCopyInMemory())
                        dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, iconStream);

                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        DevIL.ImageExporter exp = new DevIL.ImageExporter();
                        if (dds.Width == 52 && dds.Height == 52) // needs cropped
                        {
                            var iconData = dds.GetImageData(0);

                            System.Drawing.Bitmap iconBM = new System.Drawing.Bitmap(iconData.Width, iconData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                            for (int k = 0; k < iconData.Height * iconData.Width; k++) // loop through image data
                            {
                                Color iconPixel = Color.FromArgb(iconData.Data[k * 4 + 3], // copy pixel values
                                            iconData.Data[k * 4 + 0],
                                            iconData.Data[k * 4 + 1],
                                            iconData.Data[k * 4 + 2]);

                                iconBM.SetPixel(k % iconData.Width, (int)k / iconData.Width, iconPixel); //save pixel in new bitmap
                            }

                            Bitmap croppedIconBM = iconBM.Clone(new Rectangle(0, 0, 50, 50), iconBM.PixelFormat); // crop Bitmap
                            croppedIconBM.Save(outputStream, System.Drawing.Imaging.ImageFormat.Png); //Bitmap to PNG Stream
                        }
                        else
                        {
                            
                            exp.SaveImageToStream(dds, DevIL.ImageType.Png, outputStream); //save DDS to stream in PNG format
                        }

                        WriteFile(outputStream, String.Format("/{0}/Images/{1}.png", directory, filename));
                    }
                }
            }
        }

        private string GetIconFilename(string icon)
        {
            if (icon == null) return "";
            using (var file = currentDom._assets.FindFile(String.Format("/resources/gfx/{0}.dds", icon)))
            {
                if (file == null)
                    return "";
                return String.Join("_", file.FileInfo.ph, file.FileInfo.sh);
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
