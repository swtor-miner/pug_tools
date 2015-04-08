using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class SetBonusLoader
    {
        DataObjectModel _dom;
        StringTable strTable;
        public Dictionary<object, object> SetBonusEntryData;

        public SetBonusLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            strTable = null;
            SetBonusEntryData = new Dictionary<object,object>();
        }

        public Models.SetBonusEntry Load(long id)
        {
            if (SetBonusEntryData.Count == 0)
            {
                SetBonusEntryData = _dom.GetObject("itmSetBonusesPrototype").Data.Get<Dictionary<object, object>>("itmSetBonuses");
            }
            object setData = new object();
            SetBonusEntryData.TryGetValue(id, out setData);

            SetBonusEntry set = new SetBonusEntry();
            return Load(set, id, (GomObjectData)setData);
        }

        public Models.SetBonusEntry Load(Models.SetBonusEntry setEntry, long Id, GomObjectData objData)
        {
            if (objData == null)
                return setEntry;
            if (setEntry == null)
                return null;

            setEntry.Id = Id;
            setEntry._dom = _dom;
            setEntry.Prototype = "itmSetBonusesPrototype";
            setEntry.ProtoDataTable = "itmSetBonuses";

            if(strTable == null)
            {
                strTable = _dom.stringTable.Find("str.gui.itm.setbonuses");
            }
            //The base id to use for finding the real id with an offset.
            //First id in the file take 1.
            long baseId = strTable.data.Keys.First() - 1;

            long nameOffset = objData.ValueOrDefault<long>("itmSetBonusDisplayName", 0);
            
            //What is the second value supposed to be for?
            setEntry.Name = strTable.GetText(baseId + nameOffset, string.Empty);
            setEntry.LocalizedNameStrings = strTable.GetLocalizedText(baseId + nameOffset, string.Empty);

            setEntry.MaxItemCount = objData.ValueOrDefault<long>("itmSetBonusItemCount", 0);

            Dictionary<long, Models.Ability> setAblsByNum = new Dictionary<long, Models.Ability>();
            Dictionary<object, object> setAblData = objData.ValueOrDefault<Dictionary<object, object>>("itmSetBonusBonuses", new Dictionary<object, object>());
            foreach(KeyValuePair<object, object> kvp in setAblData)
            {
                long setNum = (long)kvp.Key;
                
                ulong abilityNodeId = (ulong)kvp.Value;
                Models.Ability abl = _dom.abilityLoader.Load(abilityNodeId);

                setAblsByNum.Add(setNum, abl);
            }
            setEntry.BonusAbilityByNum = setAblsByNum;

            List<Models.Item> setSourceItmList = new List<Models.Item>();
            Dictionary<object, object> setSources = objData.ValueOrDefault<Dictionary<object, object>>("itmSetBonusSetItems", new Dictionary<object, object>());
            foreach(KeyValuePair<object, object> kvp in setSources)
            {
                ulong itmNodeId = (ulong)kvp.Key;
                setSourceItmList.Add(_dom.itemLoader.Load(itmNodeId));
            }
            setEntry.Sources = setSourceItmList;

            return setEntry;
        }
    }
}
