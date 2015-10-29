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

                {"ach.", "Achievement"},
                {"abl.", "Ability"},
                {"cdx.", "Codex"},
                {"itm.", "Item"},
                {"nco.", "Companion" },
                {"npc.", "Npc" },
                {"qst.", "Mission"},
                {"tal.", "Talent"},
                {"sche", "Schematic"},
                //{"apn.", true},
                //*{"cnv.", true},
                ///*{"dec.", true},*/
                /*{"apt.", true},
                {"ipp.",true},
                {"npp.",true}*/
            };
            bool frLoaded = currentAssets.loadedFileGroups.Contains("fr-fr");
            bool deLoaded = currentAssets.loadedFileGroups.Contains("de-de");
            for (int f = 0; f < gameObjects.Count; f++)
            {
                var gameObj = gameObjects.ElementAt(f);
                ClearProgress();
                var gomList = currentDom.GetObjectsStartingWith(gameObj.Key).Where(x => !x.Name.Contains("/"));
                var count = gomList.Count();
                int i = 0;
                addtolist2(String.Format("Checking {0}", gameObj.Key));
                List<GomLib.Models.Tooltip> iList = new List<GomLib.Models.Tooltip>();
                //List<GomLib.Models.Tooltip> frList = new List<GomLib.Models.Tooltip>();
                //List<GomLib.Models.Tooltip> deList = new List<GomLib.Models.Tooltip>();
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
                        //if (frLoaded)
                        //{
                        //    GomLib.Models.Tooltip.language = "frMale";
                        //    t = new GomLib.Models.Tooltip(gom.Id, currentDom);
                        //    var table = currentDom.stringTable.Find("str.gui.alignment");
                        //    frList.Add(t);
                        //    GomLib.Models.Tooltip.language = "enMale";
                        //}
                        //if (deLoaded)
                        //{
                        //    GomLib.Models.Tooltip.language = "deMale";
                        //    t = new GomLib.Models.Tooltip(gom.Id, currentDom);
                        //    deList.Add(t);
                        //    GomLib.Models.Tooltip.language = "enMale";
                        //}

                    }
                    i++;
                }
                //ObjectListAsSql(gameObj.Value, "Tooltip", iList);
                CreatCompressedOutput(gameObj.Value, iList, "en-us");
                if (frLoaded)
                    CreatCompressedOutput(gameObj.Value, iList, "fr-fr");
                if (deLoaded)
                    CreatCompressedOutput(gameObj.Value, iList, "de-de");
            }

            bool failed = false;
            //Dictionary<string, string> protoGameObjects = new Dictionary<string, string>
            //{
            //    {"mtxStorefrontInfoPrototype", "mtxStorefrontData"},
            //    {"colCollectionItemsPrototype", "colCollectionItemsData"},
            //    {"chrCompanionInfo_Prototype", "chrCompanionInfoData"},
            //    {"scFFShipsDataPrototype", "scFFShipsData"},
            //    {"wevConquestInfosPrototype", "wevConquestTable"},
            //    {"achCategoriesTable_Prototype", "achCategoriesData"}
            //};
            Dictionary<string, string[]> protoGameObjects = new Dictionary<string, string[]>
            {
                //{"Discipline", new string[] {"ablPackagePrototype", "classDisciplinesTable"}},
            };

            for (int f = 0; f < protoGameObjects.Count; f++)
            {
                var gameObj = protoGameObjects.ElementAt(f);
                Dictionary<object, object> currentDataProto = new Dictionary<object, object>();
                GomObject currentDataObject = currentDom.GetObject(gameObj.Value[0]);
                if (currentDataObject != null) //fix to ensure old game assets don't throw exceptions.
                {
                    currentDataProto = currentDataObject.Data.Get<Dictionary<object, object>>(gameObj.Value[1]);
                    currentDataObject.Unload();
                }

                ClearProgress();
                var count = currentDataProto.Count();
                int i = 0;
                addtolist2(String.Format("Checking {0}", gameObj.Key));
                List<GomLib.Models.Tooltip> iList = new List<GomLib.Models.Tooltip>();
                foreach (var gom in currentDataProto)
                {
                    progressUpdate(i, count);
                    GomLib.Models.PseudoGameObject itm = GomLib.Models.PseudoGameObject.LoadFromProtoName(gameObj.Key, currentDom, gom.Key, (GomObjectData)gom.Value);
                    bool okToOutput = true;
                    if (chkBuildCompare.Checked)
                    {
                        var itm2 = GomLib.Models.PseudoGameObject.LoadFromProtoName(gameObj.Key, previousDom, gom.Key, (GomObjectData)gom.Value);
                        if (itm2 != null)
                        {
                            if (itm.GetHashCode() == itm2.GetHashCode())
                                okToOutput = false;
                        }
                    }

                    if (okToOutput)
                    {
                        GomLib.Models.Tooltip t = new GomLib.Models.Tooltip(itm);
                        /*if (itm.GetType() == typeof(GomLib.Models.Item)){
                            OutputIcon(((GomLib.Models.Item)itm).Icon, "TORC");
                        }*/
                        //if(((GomLib.Models.Schematic)t.obj).NameId != 0)
                        //WriteFile(t.Base62Id + ";" + ((GomLib.Models.Schematic)t.obj).Fqn + ";" + ((GomLib.Models.Schematic)t.obj).MissionFaction + ";" + ((GomLib.Models.Schematic)t.obj).Name + Environment.NewLine, "schematics.txt", true);
                        iList.Add(t);
                    }
                    i++;
                }

                CreatCompressedOutput(gameObj.Key, iList, "en-us");
            }
            OutputDiscIcons();
            EnableButtons();
        }

        public void CreatCompressedOutput(string xmlRoot, IEnumerable<GomLib.Models.Tooltip> itmList, string language)
        {
            string file = String.Format("tooltips\\{0}tips({1}).zip", xmlRoot, language);
            WriteFile("", file, false);
            HashSet<string> iconNames = new HashSet<string>();
            using (var compressStream = new MemoryStream())
            {
                //create the zip in memory
                using (var zipArchive = new ZipArchive(compressStream, ZipArchiveMode.Create, true))
                {
                    string compressedFolder = "tooltips/html/";
                    switch (language)
                    {
                        case "fr-fr":
                            compressedFolder = "tooltips/html/fr-fr/";
                            GomLib.Models.Tooltip.language = "frMale";
                            break;
                        case "de-de":
                            compressedFolder = "tooltips/html/de-de/";
                            GomLib.Models.Tooltip.language = "deMale";
                            break;
                    }
                    foreach (var t in itmList)
                    {
                        var torcEntry = zipArchive.CreateEntry(String.Format("{0}{1}.torctip", compressedFolder, t.Base62Id), CompressionLevel.Fastest);
                        using (StreamWriter writer = new StreamWriter(torcEntry.Open(), Encoding.UTF8)) //old method. Race conditions led to Central Directory corruption.
                            writer.Write(t.HTML);
                        /*using (MemoryStream htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(t.HTML ?? ""))) //see if this solves the Central Directory corruption.
                        {
                            using (var html = torcEntry.Open())
                                htmlStream.WriteTo(html);
                        }*/
                        if (language != "en-us") continue;
                        string icon = "";
                        string secondaryicon = "";
                        if (t.obj != null)
                        {
                            switch (t.obj.GetType().ToString())
                            {
                                case "GomLib.Models.Item":
                                    icon = String.Format("icons/{0}", ((GomLib.Models.Item)t.obj).Icon);
                                    secondaryicon = String.Format("icons/{0}", ((GomLib.Models.Item)t.obj).RepublicIcon);
                                    break;
                                case "GomLib.Models.Ability":
                                    icon = String.Format("icons/{0}", ((GomLib.Models.Ability)t.obj).Icon);
                                    break;
                                case "GomLib.Models.Quest":
                                    icon = String.Format("codex/{0}", ((GomLib.Models.Quest)t.obj).Icon);
                                    break;
                                case "GomLib.Models.Talent":
                                    icon = String.Format("icons/{0}", ((GomLib.Models.Talent)t.obj).Icon);
                                    break;
                                case "GomLib.Models.Achievement":
                                    icon = String.Format("icons/{0}", ((GomLib.Models.Achievement)t.obj).Icon);
                                    break;
                                case "GomLib.Models.Codex":
                                    //break;
                                    icon = String.Format("codex/{0}", ((GomLib.Models.Codex)t.obj).Image);
                                    break;
                                case "GomLib.Models.NewCompanion":
                                    //break;
                                    icon = String.Format("portraits/{0}", ((GomLib.Models.NewCompanion)t.obj).Icon);
                                    break;
                            }
                        }
                        if (t.pObj != null)
                        {
                            switch (t.pObj.GetType().ToString())
                            {
                                case "GomLib.Models.Discipline":
                                    icon = String.Format("icons/{0}", ((GomLib.Models.Item)t.obj).Icon);
                                    break;
                            }
                        }
                        if (!String.IsNullOrEmpty(icon))
                        {
                            if (iconNames.Contains(icon))
                                continue;
                            else
                                iconNames.Add(icon);
                            IconParamter[] parms = new IconParamter[1];
                            if (icon.StartsWith("portraits/"))
                                parms[0] = IconParamter.PngFormat;
                            using (MemoryStream iconStream = GetIcon(icon, parms))
                            {
                                if (iconStream != null)
                                {
                                    ZipArchiveEntry iconEntry;
                                    if (icon.StartsWith("codex/"))
                                    {
                                        iconEntry = zipArchive.CreateEntry(String.Format("codex/{0}.jpg", GetIconFilename(icon)), CompressionLevel.Fastest);
                                    }
                                    else if(icon.StartsWith("portraits/"))
                                        iconEntry = zipArchive.CreateEntry(String.Format("portraits/{0}.png", GetIconFilename(icon)), CompressionLevel.Fastest);
                                    else
                                        iconEntry = zipArchive.CreateEntry(String.Format("icons/{0}.jpg", GetIconFilename(icon)), CompressionLevel.Fastest);
                                    using (var a = iconEntry.Open())
                                        iconStream.WriteTo(a);
                                    //using (Writer writer = new BinaryWriter(iconEntry.Open()))
                                    //writer.(iconStream);
                                }
                            }
                            if (icon.StartsWith("codex/"))
                            {
                                using (MemoryStream iconStream = GetIcon(icon, true))
                                {
                                    if (iconStream != null)
                                    {
                                        ZipArchiveEntry iconEntry;
                                        iconEntry = zipArchive.CreateEntry(String.Format("codex/{0}_thumb.jpg", GetIconFilename(icon)), CompressionLevel.Fastest);
                                        using (var a = iconEntry.Open())
                                            iconStream.WriteTo(a);
                                    }
                                }
                            }
                            else if (icon.StartsWith("portraits/"))
                            {
                                using (MemoryStream iconStream = GetIcon(icon, true, IconParamter.PngFormat))
                                {
                                    if (iconStream != null)
                                    {
                                        ZipArchiveEntry iconEntry;
                                        iconEntry = zipArchive.CreateEntry(String.Format("portraits/{0}_thumb.png", GetIconFilename(icon)), CompressionLevel.Fastest);
                                        using (var a = iconEntry.Open())
                                            iconStream.WriteTo(a);
                                    }
                                }
                            }
                        }
                        if (!String.IsNullOrEmpty(secondaryicon))
                        {
                            if (iconNames.Contains(secondaryicon))
                                continue;
                            else
                                iconNames.Add(secondaryicon);
                            using (MemoryStream iconStream = GetIcon(secondaryicon))
                            {
                                if (iconStream != null)
                                {
                                    ZipArchiveEntry iconEntry;
                                    if (icon.StartsWith("codex/"))
                                    {
                                        iconEntry = zipArchive.CreateEntry(String.Format("codex/{0}.jpg", GetIconFilename(secondaryicon)), CompressionLevel.Fastest);
                                    }
                                    else
                                        iconEntry = zipArchive.CreateEntry(String.Format("icons/{0}.jpg", GetIconFilename(secondaryicon)), CompressionLevel.Fastest);
                                    using (var a = iconEntry.Open())
                                        iconStream.WriteTo(a);
                                    //using (Writer writer = new BinaryWriter(iconEntry.Open()))
                                    //writer.(iconStream);
                                }
                            }
                            if (icon.StartsWith("codex/"))
                            {
                                using (MemoryStream iconStream = GetIcon(secondaryicon, true))
                                {
                                    if (iconStream != null)
                                    {
                                        ZipArchiveEntry iconEntry;
                                        iconEntry = zipArchive.CreateEntry(String.Format("codex/{0}_thumb.jpg", GetIconFilename(secondaryicon)), CompressionLevel.Fastest);
                                        using (var a = iconEntry.Open())
                                            iconStream.WriteTo(a);
                                    }
                                }
                            }
                        }
                    }
                }
                compressStream.Position = 0;
                WriteFile(compressStream, file);
            }
            GomLib.Models.Tooltip.language = "enMale";
        }

        private MemoryStream GetIcon(string icon, params IconParamter[] encodingParams)
        {
            return GetIcon(icon, false, encodingParams);
        }
        private MemoryStream GetIcon(string icon, bool generateThumb, params IconParamter[] encodingParams)
        {
            if (icon == null) return null;
            using (var file = currentDom._assets.FindFile(String.Format("/resources/gfx/{0}.dds", icon)))
            {
                if (file == null) return null;
                var filename = String.Join("_", file.FileInfo.ph, file.FileInfo.sh);
                return GetIcon(filename, file, generateThumb, encodingParams);
            }
        }

        private MemoryStream GetIcon(string filename, TorLib.File file, bool generateThumb, params IconParamter[] encodingParams)
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
            ImageCodecInfo encoder;
            if (encodingParams.Contains(IconParamter.PngFormat))
            {
                myparams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                encoder = GetEncoder(ImageFormat.Png);
            }
            else
            {
                myparams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                encoder = GetEncoder(ImageFormat.Jpeg);
            }

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
                croppedIconBM.Save(outputStream, encoder, myparams); //Bitmap to PNG Stream
            }
            else
            {
                var iconData = dds.GetImageData(0);

                //System.Drawing.Bitmap iconBM = new System.Drawing.Bitmap(iconData.Width, iconData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                //for (int k = 0; k < iconData.Height * iconData.Width; k++) // loop through image data
                //{
                //    Color iconPixel = Color.FromArgb(iconData.Data[k * 4 + 3], // copy pixel values
                //                iconData.Data[k * 4 + 0],
                //                iconData.Data[k * 4 + 1],
                //                iconData.Data[k * 4 + 2]);

                //    iconBM.SetPixel(k % iconData.Width, (int)k / iconData.Width, iconPixel); //save pixel in new bitmap
                //}

                //iconBM.Save(outputStream, jpgEncoder, myparams); //Bitmap to PNG Stream
                if (encodingParams.Contains(IconParamter.PngFormat))
                {
                    
                    if (generateThumb)
                    {
                        dds.Resize(dds.Width / 4, dds.Height / 4, 4, DevIL.SamplingFilter.ScaleLanczos3, true);
                    }
                        exp.SaveImageToStream(dds, DevIL.ImageType.Png, outputStream);
                }
                else
                {
                    using (MemoryStream taco = new MemoryStream())
                    {

                        exp.SaveImageToStream(dds, DevIL.ImageType.Bmp, taco); //save DDS to stream in jpg format
                        System.Drawing.Bitmap iconBM = new System.Drawing.Bitmap(taco);
                        if (generateThumb)
                        {
                            Bitmap resized = new Bitmap(iconBM, new System.Drawing.Size(iconBM.Width / 4, iconBM.Height / 4));
                            resized.Save(outputStream, encoder, myparams);
                        }
                        else
                            iconBM.Save(outputStream, encoder, myparams); //Bitmap to JPG Stream
                    }
                }
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
                    using (var file2 = currentDom._assets.FindFile(String.Format("/resources/gfx/icons/{0}.dds", icon)))
                    {
                        if (file2 == null)
                            return "";
                        return String.Join("_", file2.FileInfo.ph, file2.FileInfo.sh);
                    }
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
        public void OutputDiscIcons()
        {
            Dictionary<string, string[]> protoGameObjects = new Dictionary<string, string[]>
            {
                {"Discipline", new string[] {"ablPackagePrototype", "classDisciplinesTable"}},
            };

            for (int f = 0; f < protoGameObjects.Count; f++)
            {
                var gameObj = protoGameObjects.ElementAt(f);
                Dictionary<object, object> currentDataProto = new Dictionary<object, object>();
                GomObject currentDataObject = currentDom.GetObject(gameObj.Value[0]);
                if (currentDataObject != null) //fix to ensure old game assets don't throw exceptions.
                {
                    currentDataProto = currentDataObject.Data.Get<Dictionary<object, object>>(gameObj.Value[1]);
                    currentDataObject.Unload();
                }

                ClearProgress();
                var count = currentDataProto.Count();
                int i = 0;
                addtolist2(String.Format("Checking {0}", gameObj.Key));
                List<GomLib.Models.Discipline> iList = new List<GomLib.Models.Discipline>();

                foreach (var gom in currentDataProto)
                {
                    progressUpdate(i, count);
                    var discData = (List<GomObjectData>)((List<object>)gom.Value).ConvertAll(x => (GomObjectData)x);

                    foreach (var disc in discData)
                    {
                        GomLib.Models.Discipline dis = new GomLib.Models.Discipline();
                        currentDom.disciplineLoader.Load(dis, disc);
                        iList.Add(dis);
                    }
                    
                    i++;
                }
                WriteFile("", "disciplineIcons.zip", false);
                HashSet<string> iconNames = new HashSet<string>();
                using (var compressStream = new MemoryStream())
                {
                    //create the zip in memory
                    using (var zipArchive = new ZipArchive(compressStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var t in iList)
                        {
                            string icon = "icons/" + t.Icon;
                            
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
                    WriteFile(compressStream, "disciplineIcons.zip");
                }
                
            }
        }
    }
    public enum IconParamter
    {
        PngFormat,
    }
}
