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
        Dictionary<object, object> mapAreasDataObjectList = new Dictionary<object, object>();
        StringTable strTable;

        private readonly DataObjectModel _dom;
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
            mapAreasDataObjectList.TryGetValue((object)id, out object obj);
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
            area.Dom_ = _dom;
            _ = obj.Dictionary;
            //if (objAsDict.ContainsKey("mapAreasDataDisplayNameId") && objAsDict.ContainsKey("mapAreasDataDefaultZoneName"))
            //{
            area.DisplayNameId = obj.ValueOrDefault<long>("mapAreasDataDisplayNameId", 0);
            //area.Id = (int)(area.DisplayNameId & 0x7FFFFFFF);
            //area.CommentableId = Guid.NewGuid();
            area.Name = strTable.GetText(area.DisplayNameId, "MapArea." + area.DisplayNameId);
            area.LocalizedName = strTable.GetLocalizedText(area.DisplayNameId, "MapArea." + area.DisplayNameId);
            area.AreaId = obj.ValueOrDefault<ulong>("mapAreasDataAreaId", 0);

            var proto = _dom.GetObjectNoLoad("mapAreasDataProto");
            if (proto != null && proto.ProtoReferences != null)
            {
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
            for (int i = 0; i < count; i += 2)
            {
                var key = elements.ElementAt(i).Value;
                ulong.TryParse(key, out ulong k);
                var value = elements.ElementAt(i + 1);
                var node = value.Element("node");
                if (node != null)
                {
                    MapNoteData mpn = new MapNoteData(area.Dom_).Load(node);
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
                    MapPage page = new MapPage
                    {
                        Area = area,
                        Guid = mapPage.ValueOrDefault<long>("mapPageGUID", 0)
                    };
                    page.Id = (int)(page.Guid & 0x7FFFFFFF);
                    if (page.Id == 40003)
                    {
                        // string sdf = "";
                    }
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
                    //if (String.IsNullOrWhiteSpace(page.Name))
                    //{
                    //    string sdfin = "";
                    //    if (mapPage.Dictionary.ContainsKey("locTextRetrieverMap"))
                    //    {
                    //        var descLookupData = (GomObjectData)(mapPage.Get<Dictionary<object, object>>("locTextRetrieverMap")[-2761358831308646330]);
                    //        var stringId = descLookupData.Get<long>("strLocalizedTextRetrieverStringID");
                    //        var bucket = descLookupData.Get<string>("strLocalizedTextRetrieverBucket");
                    //        var checkname = strTable.GetText(stringId, bucket);
                    //        string sdf = "";
                    //    }
                    //}
                    page.LocalizedName = strTable.GetLocalizedText(page.Guid, "MapPage." + page.MapName);

                    page.ExplorationId = Int64.Parse(mapPage.ValueOrDefault<object>("mapExplorationId", 0).ToString());
                    page.MapFowRadius = mapPage.ValueOrDefault<float>("mapFowRadius", 0f);

                    pageLookup[page.SId] = page;
                    area.MapPages.Add(page);
                }

                foreach (var p in area.MapPages)
                {
                    if (p.ParentId == 0) continue; // MapPage has no parent (this is a world map)

                    if (pageLookup.TryGetValue(p.ParentId, out MapPage parent))
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

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (gom is null)
            {
                throw new ArgumentNullException(nameof(gom));
            }
            // No references to load
        }
    }
}
