using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;
using System.Xml.Linq;

namespace GomLib.ModelLoader
{
    public class StrongholdLoader : IModelLoader
    {
        DataObjectModel _dom;

        public StrongholdLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            //HookList = null;
        }

        //private Dictionary<long, Hook> HookList;

        public string ClassName
        {
            get { return "aptStronghold"; }
        }

        public Models.GameObject CreateObject()
        {
            return new Models.Stronghold();
        }

        private HashSet<string> fields = new HashSet<string>() {
            "decPrevObjRotationX",
            "decPrevObjRotationY",
            "decNameId",
            "decUseItemName",
            "decUnlockingItemId",
            "decDecorationId",
            "decPlcState",
            "decDefaultAnimation",
            "decMaxUnlockLimit",
            "decUnknownUnlockLimit",
            "decUniquePerLegacy",
            "decFactionPlacementRestriction",
            "decCategoryNameId",
            "decSubCategoryNameId",
            "decHookList",
            "decPrevCamHeightOff",
            "decPrevCamDisOff",
            "decRequiredAbilityType",
            "decRequiresAbilityUnlocked",
            "decGuildPurchaseCost"
            };

        public Stronghold Load(Models.GameObject obj, GomObject gom)
        {
            if (gom == null) { return (Stronghold)obj; }
            if (gom == null) { return null; }

            var apt = (Models.Stronghold)obj;

            apt.NodeId = gom.Id;
            apt.Id = gom.Id;
            apt.Fqn = gom.Name;
            apt._dom = _dom;
            apt.References = obj.References;
            var keys = gom.Data.Dictionary.Keys;
            foreach (var key in keys.Skip(3)) //Skip the first three keys as they are internal ones
            {
                /*if (!fields.Contains(key))
                {
                    Console.WriteLine(String.Join(", ", key, dec.Fqn));
                    //if (key != "Script_Type" && key != "Script_TypeId" && key != "Script_NumFields")
                    //throw new IndexOutOfRangeException();
                }*/
            }

            var nameTable = _dom.stringTable.Find("str.apt");

            apt.NameId = gom.Data.ValueOrDefault<long>("aptNameStb", 0);
            apt.Name = nameTable.GetText(apt.NameId, "str.apt");
            apt.LocalizedName = nameTable.GetLocalizedText(apt.NameId, "str.apt");

            apt.DescId = gom.Data.ValueOrDefault<long>("aptDescStb", 0);
            apt.Description = nameTable.GetText(apt.DescId, "str.apt");
            apt.LocalizedDescription = nameTable.GetLocalizedText(apt.DescId, "str.apt");

            apt.PublicIcon = gom.Data.ValueOrDefault<string>("aptPublicIcon", "");
            apt.Icon = gom.Data.ValueOrDefault<string>("aptIcon", "");
            apt.DefaultOccupancy = gom.Data.ValueOrDefault<long>("aptDefaultOccupancy", 0);
            apt.DefaultHooks = gom.Data.ValueOrDefault<long>("aptDefaultHooks", 0);
            apt.PhsId = gom.Data.ValueOrDefault<ulong>("aptPhsId", 0);
            apt.MaxHooks = gom.Data.ValueOrDefault<long>("aptMaxHooks", 0);
            var roomTable = gom.Data.ValueOrDefault<Dictionary<object, object>>("aptRoomTable", new Dictionary<object, object>());

            apt.RoomTable = new Dictionary<long, Room>();
            foreach (var kvp in roomTable)
            {
                Room room = LoadRoom((GomObjectData)kvp.Value);
                apt.RoomTable.Add((long)kvp.Key, room);
            }

            apt.DiscountMtxSFId = gom.Data.ValueOrDefault<long>("aptDiscountMtxSF", 0);
            apt.MtxStoreFrontId = gom.Data.ValueOrDefault<long>("aptMtxStoreFront", 0);
            apt.DefGuildShOcc = gom.Data.ValueOrDefault<long>("aptDefGuildShOcc", 0);
            apt.PlayerShCost = gom.Data.ValueOrDefault<long>("aptPlayerShCost", 0);
            apt.GuildShCost = gom.Data.ValueOrDefault<long>("aptGuildShCost", 0);
            var factionPurchaseRestriction = gom.Data.ValueOrDefault<ScriptEnum>("aptFactionPurchaseRestriction", new ScriptEnum());
            apt.FactionPurchaseRestriction = factionPurchaseRestriction.ToString();
            var type = gom.Data.ValueOrDefault<ScriptEnum>("aptType", new ScriptEnum());
            apt.Type = type.ToString();


            gom.Unload();
            return apt;
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            var itm = (Models.Stronghold)obj;
        }

        public void LoadObject(Models.GameObject obj, GomObject gom)
        {
            Load(obj, gom);
        }

        public Models.Stronghold Load(ulong nodeId)
        {
            GomObject obj = _dom.GetObject(nodeId);
            if (obj == null) { return null; }
            Models.Stronghold itm = new Stronghold();
            return Load(itm, obj);
        }

        public Models.Stronghold Load(string fqn)
        {
            GomObject obj = _dom.GetObject(fqn);
            if (obj == null) { return null; }
            Models.Stronghold itm = new Stronghold();
            return Load(itm, obj);
        }

        public Models.Stronghold Load(GomObject obj)
        {
            if (obj == null) { return null; }
            Models.Stronghold itm = new Stronghold();
            return Load(itm, obj);
        }

        public Room LoadRoom(GomObjectData gom)
        {
            if (gom == null) { return new Room(); }

            var room = new Room();

            room._dom = _dom;

            var nameTable = _dom.stringTable.Find("str.apt");

            room.NameId = gom.ValueOrDefault<long>("aptRoomNameStb", 0);
            room.Name = nameTable.GetText(room.NameId, "str.apt");
            room.LocalizedName = nameTable.GetLocalizedText(room.NameId, "str.apt");

            room.DescId = gom.ValueOrDefault<long>("aptRoomDescStb", 0);
            room.Description = nameTable.GetText(room.DescId, "str.apt");
            room.LocalizedDescription = nameTable.GetLocalizedText(room.DescId, "str.apt");

            room.PlayerShCost = gom.ValueOrDefault<long>("aptRoomPlayerShCost", 0);
            room.DiscountMtxSFId = gom.ValueOrDefault<long>("aptRoomDiscMtxSF", 0);
            room.PlyrShIncDecs = gom.ValueOrDefault<long>("aptRoomPlyrShIncDecs", 0);
            room.PlyrShIncOcc = gom.ValueOrDefault<long>("aptRoomPlyrShIncOcc", 0);
            room.MtxStoreFrontId = gom.ValueOrDefault<long>("aptRoomMtxStoreFront", 0);
            room.GldShIncDecs = gom.ValueOrDefault<long>("aptRoomGldShIncDecs", 0);
            room.GldShIncOcc = gom.ValueOrDefault<long>("aptRoomGldShIncOcc", 0);
            room.GldShCost = gom.ValueOrDefault<long>("aptRoomGldShCost", 0);
            room.Idx = gom.ValueOrDefault<long>("aptRoomIdx", 0);
            room.ReqItmToUnlockId = gom.ValueOrDefault<ulong>("aptReqItmToUnlock", 0);
            room.ReqQty = gom.ValueOrDefault<long>("aptReqQty", 0);

            return room;
        }
    }

}
