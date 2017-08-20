using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;
using System.Xml.Linq;
using System.IO;

namespace GomLib.ModelLoader
{
    public class AreaLoader
    {
        const long NameLookupKey = -2761358831308646330;
        Dictionary<object, object> mapAreasDataObjectList = new Dictionary<object, object>();
        StringTable strTable;

        private DataObjectModel _dom;
        public AreaLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            strTable = null;
            mapAreasDataObjectList = new Dictionary<object, object>();
        }

        public string ClassName
        {
            get { return "mapAreasDataObject"; }
        }

        public Area Load(ulong id)
        {
            if (mapAreasDataObjectList.Count == 0)
            {
                var mapAreasDataProto = _dom.GetObject("mapAreasDataProto");
                if (mapAreasDataProto != null)
                    mapAreasDataObjectList = mapAreasDataProto.Data.ValueOrDefault<Dictionary<object, object>>("mapAreasDataObjectList", new Dictionary<object, object>());
            }
            object obj;
            mapAreasDataObjectList.TryGetValue((object)id, out obj);
            return Load(new Area(), obj as GomObjectData);
        }
        public Models.Area Load(Models.Area area, GomObjectData obj)
        {
            if (obj == null) { return area; }
            if (area == null) { return null; }

            if (strTable == null)
            {
                strTable = _dom.stringTable.Find("str.sys.worldmap");
            }
            area._dom = _dom;

            IDictionary<string, object> objAsDict = obj.Dictionary;
            //if (objAsDict.ContainsKey("mapAreasDataDisplayNameId") && objAsDict.ContainsKey("mapAreasDataDefaultZoneName"))
            //{
            area.DisplayNameId = obj.ValueOrDefault<long>("mapAreasDataDisplayNameId", 0);
            //area.Id = (int)(area.DisplayNameId & 0x7FFFFFFF);
            //area.CommentableId = Guid.NewGuid();
            area.Name = strTable.GetText(area.DisplayNameId, "MapArea." + area.DisplayNameId);
            area.LocalizedName = strTable.GetLocalizedText(area.DisplayNameId, "MapArea." + area.DisplayNameId);
            area.AreaId = obj.ValueOrDefault<ulong>("mapAreasDataAreaId", 0);

            var proto = _dom.GetObjectNoLoad("mapAreasDataProto");
            if (proto != null && proto.ProtoReferences != null) {
                if (proto.ProtoReferences.ContainsKey(area.AreaId))
                    area.References = proto.ProtoReferences[area.AreaId];
            }
            area.Id = (long)area.AreaId; //(long)(area.AreaId >> 32);
            if (area.Id == 0)
                area.Id = (long)area.AreaId;
            area.ZoneName = obj.ValueOrDefault<string>("mapAreasDataDefaultZoneName", null);

            string mapDataPath = String.Format("world.areas.{0}.mapdata", area.AreaId);
            var mapDataObj = _dom.GetObject(mapDataPath);
            if (mapDataObj != null)
            {
                LoadMapdata(area, mapDataObj);

                area.FowGroupStringIds = new Dictionary<ulong, long>();
                var mapDataContainerFowGroupList = mapDataObj.Data.ValueOrDefault<Dictionary<object, object>>("mapDataContainerFowGroupList", null);
                if (mapDataContainerFowGroupList != null)
                {
                    foreach (var kvp in mapDataContainerFowGroupList)
                    {
                        ulong fowId = (ulong)(kvp.Key);
                        long stringId = ((GomObjectData)kvp.Value).ValueOrDefault<long>("mapFowGroupGUID"); //"str.sys.worldmap"
                        area.FowGroupStringIds.Add(fowId, stringId);
                    }
                }
            }
            string mapNotePath = String.Format("/resources/world/areas/{0}/mapnotes.not", area.AreaId);
            var mapNoteObj = _dom._assets.FindFile(mapNotePath);
            if (mapNoteObj != null)
            {
                LoadMapNotes(area, mapNoteObj);
            }

            //area.Assets = LoadAssets(area.AreaId);
            area.AreaDat = _dom.areaDatLoader.Load(area.AreaId);
            /*}
            else
            {
                area.Id = 0;
            }*/

            return area;
        }

        private void LoadMapNotes(Models.Area area, TorLib.File file)
        {
            string xml = "";
            using (var reader = new StreamReader(file.OpenCopyInMemory()))
            {
                xml = reader.ReadToEnd();
            }
            xml = xml.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;lt;", "<").Replace("&amp;gt;", ">").Replace("&amp;apos;", "'").Replace("\0", "");
            XDocument notes = XDocument.Parse(xml);
            var elements = notes.Root.Element("e").Element("v").Elements();
            Dictionary<ulong, MapNoteData> mapNotes = new Dictionary<ulong, MapNoteData>();
            var count = elements.Count();
            for (int i = 0; i < count; i = i + 2)
            {
                var key = elements.ElementAt(i).Value;
                ulong k;
                ulong.TryParse(key, out k);
                var value = elements.ElementAt(i+1);
                var node = value.Element("node");
                if (node != null)
                {
                    MapNoteData mpn = new MapNoteData(area._dom).Load(node);
                    mapNotes.Add(k, mpn);
                }
                else
                {
                    continue;
                }
            }
            area.MapNotes = mapNotes;
        }

        private void LoadMapdata(Models.Area area, GomObject obj)
        {
            List<object> mapPages = (List<object>)obj.Data.ValueOrDefault<List<object>>("mapDataContainerMapDataList", null);
            Dictionary<long, MapPage> pageLookup = new Dictionary<long, MapPage>();

            if (mapPages != null)
            {
                area.MapPages = new List<MapPage>();
                foreach (GomObjectData mapPage in mapPages)
                {
                    MapPage page = new MapPage();
                    page.Area = area;
                    page.Guid = mapPage.ValueOrDefault<long>("mapPageGUID", 0);
                    page.Id = (int)(page.Guid & 0x7FFFFFFF);
                    var minCoord = mapPage.ValueOrDefault<List<float>>("mapPageMinCoord", null);
                    if (minCoord != null)
                    {
                        page.MinX = minCoord[0];
                        page.MinY = minCoord[1];
                        page.MinZ = minCoord[2];
                    }
                    var maxCoord = mapPage.ValueOrDefault<List<float>>("mapPageMaxCoord", null);
                    if (maxCoord != null)
                    {
                        page.MaxX = maxCoord[0];
                        page.MaxY = maxCoord[1];
                        page.MaxZ = maxCoord[2];
                    }
                    var miniMinCoord = mapPage.ValueOrDefault<List<float>>("mapPageMiniMinCoord", null);
                    if (miniMinCoord != null)
                    {
                        page.MiniMapMinX = miniMinCoord[0];
                        page.MiniMapMinZ = miniMinCoord[2];
                    }
                    var miniMaxCoord = mapPage.ValueOrDefault<List<float>>("mapPageMiniMaxCoord", null);
                    if (miniMaxCoord != null)
                    {
                        page.MiniMapMaxX = miniMaxCoord[0];
                        page.MiniMapMaxZ = miniMaxCoord[2];
                    }
                    page.CalculateVolume();
                    page.ExplorationType = mapPage.ValueOrDefault<ScriptEnum>("mapExplorationType", new ScriptEnum()).ToString();
                    page.MountAllowed = mapPage.ValueOrDefault<bool>("mapMountAllowed", false);
                    page.IsHeroic = mapPage.ValueOrDefault<bool>("mapIsHeroic", false);
                    page.ParentId = mapPage.ValueOrDefault<long>("mapParentNameSId", 0);
                    page.SId = mapPage.ValueOrDefault<long>("mapNameSId", 0);
                    page.MapName = mapPage.ValueOrDefault<string>("mapName", null);

                    string mapImagePath = String.Format("/resources/world/areas/{0}/{1}_r.dds", area.AreaId, page.MapName);
                    page.HasImage = _dom._assets.HasFile(mapImagePath);

                    if (page.HasImage)
                    {
                        if (page.Area.RequiredFiles == null)
                            page.Area.RequiredFiles = new HashSet<string>();
                        page.Area.RequiredFiles.Add(mapImagePath);
                    }

                    _dom._assets.icons.AddMap(area.AreaId, page.MapName);

                    page.Name = strTable.GetText(page.Guid, "MapPage." + page.MapName);
                    page.LocalizedName = strTable.GetLocalizedText(page.Guid, "MapPage." + page.MapName);

                    page.ExplorationId = Int64.Parse(mapPage.ValueOrDefault<object>("mapExplorationId", 0).ToString());
                    page.mapFowRadius = mapPage.ValueOrDefault<float>("mapFowRadius", 0f);

                    pageLookup[page.SId] = page;
                    area.MapPages.Add(page);
                }

                foreach (var p in area.MapPages)
                {
                    if (p.ParentId == 0) continue; // MapPage has no parent (this is a world map)

                    MapPage parent;
                    if (pageLookup.TryGetValue(p.ParentId, out parent))
                    {
                        p.Parent = parent;
                    }
                    else
                    {
                        throw new InvalidOperationException("Unable to find parent map page");
                    }
                }
            }
        }

        private Dictionary<ulong, string> LoadAssets(ulong id)
        {
            Dictionary<ulong, string> assets = new Dictionary<ulong, string>();
            if (id == 0) return assets;
            
            string path = "\\resources\\world\\areas\\" + id.ToString() + "\\";
            string fileName = path + "area.dat";
            //ParseResource(assetList, path + "mapnotes.not");

            //if (assetList.Add(fileName)) addtolist2(fileName);

            var datFile = _dom._assets.FindFile(fileName);
            if (datFile == null) return assets;
            System.Xml.Linq.XDocument doc = new System.Xml.Linq.XDocument();
            List<string> lines = new List<string>();
            using (var fileStream = datFile.OpenCopyInMemory())
            using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
            {
                int x = 0;
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                    x++;
                }
            }

            int line_count = 0;
            int line_skip = 0;

            foreach (string line in lines)
            {
                if (line_skip >= 1)
                {
                    line_count++;
                    line_skip--;
                    continue;
                }

                if (line.Contains("[ROOMS]"))
                {
                    string line_ahead = "";
                    bool stop = false;
                    do
                    {
                        line_skip++;
                        line_ahead = lines[line_count + line_skip];
                        if (line_ahead.Contains("[ASSETS]"))
                        {
                            line_skip--;
                            stop = true;
                        }
                        //else
                            //ParseResource(assetList, path + line_ahead.ToLower().Trim() + ".dat");
                    } while (stop == false);
                }

                if (line.Contains("[ASSETS]"))
                {
                    string line_ahead = "";
                    bool stop = false;
                    do
                    {
                        line_skip++;
                        line_ahead = lines[line_count + line_skip];
                        if (line_ahead.Contains("[PATHS]"))
                        {
                            line_skip--;
                            stop = true;
                        }
                        else
                        {
                            string[] temp = line_ahead.Split('=');
                            ulong assetId;
                            UInt64.TryParse(temp[0], out assetId);

                            assets.Add(assetId, temp[1]);

                        }
                    } while (stop == false);
                }

                /*if (line.Contains("[TERRAINTEXTURES]"))
                {
                    addtolist2("Found Terrain Textures!");
                    string line_ahead = "";
                    bool stop = false;
                    do
                    {
                        line_skip++;
                        line_ahead = lines[line_count + line_skip];
                        if (line_ahead.Contains("[DYDTEXTURES]"))
                        {
                            line_skip--;
                            stop = true;
                        }
                        else
                        {
                            string[] temp = line_ahead.Split(':');
                            if (temp.Length > 2)
                            {
                                string temp1 = temp[1].Trim().Replace('/', '\\');
                                if (!temp1.Contains('\\')) ParseResource(assetList, "/resources/art/shaders/materials/" + temp1 + ".mat");
                                else ParseResource(assetList, temp[2].Trim());
                            }
                        }
                    } while (stop == false);
                }

                if (line.Contains("[DYDTEXTURES]"))
                {
                    addtolist2("Found Textures!");
                    string line_ahead = "";
                    bool stop = false;
                    do
                    {
                        line_skip++;
                        line_ahead = lines[line_count + line_skip];
                        if (line_ahead.Contains("[DYDCHANNELPARAMS]"))
                        {
                            line_skip--;
                            stop = true;
                        }
                        else
                        {
                            string[] temp = line_ahead.Split(':');
                            if (temp[1] != "")
                            {
                                string temp1 = temp[1].Trim().Replace('/', '\\');
                                if (!temp1.Contains('\\')) ParseResource(assetList, "/resources/art/shaders/materials/" + temp1 + ".mat");
                                else ParseResource(assetList, temp[1].Trim());
                            }
                        }
                    } while (stop == false);
                }*/

                line_count++;
            }
            return assets;
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            // No references to load
        }
    }
}
