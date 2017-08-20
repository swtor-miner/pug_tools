using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class MapNoteLoader
    {
        const long NameLookupKey = -2761358831308646330;

        private StringTable taxiTerminals;

        Dictionary<ulong, Models.MapNote> idMap;
        Dictionary<string, Models.MapNote> nameMap;
        Dictionary<ulong, Models.WonkavatorPackage> wnkPackages;

        DataObjectModel _dom;

        public MapNoteLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            taxiTerminals = null;
            idMap = new Dictionary<ulong, Models.MapNote>();
            nameMap = new Dictionary<string, Models.MapNote>();
        }

        public string ClassName
        {
            get { return "mpnMapNoteInfo"; }
        }

        public Models.MapNote Load(ulong nodeId)
        {
            Models.MapNote result;
            if (idMap.TryGetValue(nodeId, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            Models.MapNote mpn = new Models.MapNote();
            return Load(mpn, obj);
        }

        public Models.MapNote Load(string fqn)
        {
            Models.MapNote result;
            if (nameMap.TryGetValue(fqn, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            Models.MapNote mpn = new Models.MapNote();
            return Load(mpn, obj);
        }

        public Models.MapNote Load(GomObject obj)
        {
            Models.MapNote mpn = new Models.MapNote();
            return Load(mpn, obj);
        }

        public Models.GameObject CreateObject()
        {
            return new Models.MapNote();
        }

        public Models.MapNote Load(Models.MapNote mpn, GomObject obj)
        {
            if (obj == null) { return null; }
            if (mpn == null) { return null; }

            if (taxiTerminals == null)
            {
                taxiTerminals = _dom.stringTable.Find("str.tax.terminals");
            }

            mpn.Fqn = obj.Name;
            mpn.Id = obj.Id;
            mpn.References = obj.References;
            if (mpn.References != null && mpn.References.ContainsKey("usedByArea"))
            {
                if (mpn.References["usedByArea"].Count > 1)
                {
                    var sfoin = "";
                }
            }

            mpn.AssetID = obj.Data.Get<long>("mpnAssetID");
            //mpn.Id = (ulong)(mpnAssetID >> 32);

            var textLookup = obj.Data.Get<Dictionary<object,object>>("locTextRetrieverMap");
            var nameLookupData = (GomObjectData)textLookup[NameLookupKey];
            mpn.Name = _dom.stringTable.TryGetString(mpn.Fqn, nameLookupData);
            mpn.LocalizedName = _dom.stringTable.TryGetLocalizedStrings(mpn.Fqn, nameLookupData);

            //mpn.Icon = MapNoteIconExtensions.ToMapNoteIcon(obj.Data.Get<string>("mpnIconAsset"));
            mpn.Icon = obj.Data.Get<string>("mpnIconAsset");
            if (obj.Data.ContainsKey("mpnConditionEType")) { mpn.Condition = MapNoteConditionExtensions.ToMapNoteCondition(obj.Data.Get<object>("mpnConditionEType").ToString()); }
            if (obj.Data.ContainsKey("mpnHuntingRadius")) { mpn.HuntingRadius = _dom.data.huntingRadius.GetRadius(obj.Data.Get<long>("mpnHuntingRadius")); }
            if (obj.Data.ContainsKey("mpnHuntingRadiusBonus")) { mpn.BonusHuntingRadius = _dom.data.huntingRadius.GetRadius(obj.Data.Get<long>("mpnHuntingRadiusBonus")); }

            if (obj.Data.ContainsKey("mpnMapLink"))
            {
                var mapLink = obj.Data.Get<GomObjectData>("mpnMapLink");
                ulong areaId = mapLink.Get<ulong>("mapLinkAreaId");
                long mId = mapLink.ValueOrDefault<long>("mapLinkMapNameSId");
                long sId = mapLink.ValueOrDefault<long>("mapLinkSubmapNameSId");

                mpn.MapLink = new MapLink(areaId, mId, sId);
                //GetLinkedMapId(mpn, areaId, sId);
            }

            // if (obj.Data.ContainsKey("mpnMetadataFullFQN") && mpn.Icon != MapNoteIcon.Taxi && mpn.Icon != MapNoteIcon.Wonkavator) { Console.WriteLine(mpn.Fqn); }
            //if (obj.Data.ContainsKey("mpnMetadataInt")) { mpn.WonkaPackageId = obj.Data.Get<long>("mpnMetadataInt"); }
            //if (obj.Data.ContainsKey("mpnMetadataID")) { mpn.WonkaDestinationId = (long)obj.Data.Get<ulong>("mpnMetadataID"); }
            //0x2F63800000000

            switch(mpn.Icon) {
                case "Taxi":
                    if (obj.Data.ContainsKey("mpnMetadataFullFQN"))
                    {
                        ulong taxiNodeId = obj.Data.Get<ulong>("mpnMetadataFullFQN");
                        var taxiNode = _dom.GetObject(taxiNodeId);
                        if (taxiNode != null)
                        {
                            long taxNameId = taxiNode.Data.ValueOrDefault<long>("taxNameId", -1);
                            long taxFaction = taxiNode.Data.ValueOrDefault<long>("taxFaction", 0);
                            mpn.Name = taxiTerminals.GetText(0x7D60500000000 + taxNameId, mpn.Fqn);
                            mpn.LocalizedName = taxiTerminals.GetLocalizedText(0x7D60500000000 + taxNameId, mpn.Fqn);
                            mpn.Faction = _dom.factionData.ToFaction(taxFaction);
                        }
                    }
                    break;
                case "Wonkavator":
                    ulong unknownNodeId = obj.Data.ValueOrDefault<ulong>("mpnMetadataFullFQN");
                    //var unknownNode = _dom.GetObject(unknownNodeId);
                    
                    break;
            }

            return mpn;
        }

        ///// <summary>Retrieve the ID for the Map this map note should link to</summary>
        ///// <param name="mpn">The map note that links to a map</param>
        ///// <param name="areaId">Area ID of the linked map</param>
        ///// <param name="sId">SId of the linked map</param>
        //private void GetLinkedMapId(MapNote mpn, ulong areaId, long sId)
        //{
        //    switch (areaId)
        //    {
        //        case 4611686055156531852:
        //        case 4611686048166971304:
        //        case 4611686048166971255:
        //        case 4611686038902870211:
        //            {
        //                Console.WriteLine("{0} links to map not in mapAreasProto", mpn.Fqn);
        //                return;
        //            }
        //    }

        //    string mapDataPath = String.Format("world.areas.{0}.mapdata", areaId);
        //    var mapDataObj = _dom.GetObject(mapDataPath);
        //    if (mapDataObj != null)
        //    {
        //        List<object> mapPages = (List<object>)mapDataObj.Data.ValueOrDefault<List<object>>("mapDataContainerMapDataList", null);

        //        if (mapPages != null)
        //        {
        //            foreach (GomObjectData mapPage in mapPages)
        //            {
        //                if (sId == mapPage.ValueOrDefault<long>("mapNameSId", 0))
        //                {
        //                    var guid = mapPage.ValueOrDefault<long>("mapPageGUID", 0);
        //                    mpn.LinkedMapId = (int)(guid & 0x7FFFFFFF);
        //                    return;
        //                }
        //            }
        //        }
        //    }
        //}

        public void LoadObject(Models.GameObject loadMe, GomObject obj)
        {
            GomLib.Models.MapNote mpn = (Models.MapNote)loadMe;
            Load(mpn, obj);
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            // No references to load
        }
    }
}
