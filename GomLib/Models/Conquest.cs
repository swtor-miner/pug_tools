using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;

namespace GomLib.Models
{
    public class Conquest : PseudoGameObject, IEquatable<Conquest>
    {
        public long NameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long DescId { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public string Icon { get; set; }
        public long ParticipateGoal { get; set; }
        public List<long> RepeatableObjectivesIdList { get; set; }
        public List<ConquestObjective> RepeatableObjectivesList { get; set; }
        public List<long> OneTimeObjectiveIdList { get; set; }
        public List<ConquestObjective> OneTimeObjectivesList { get; set; }
        public string DesignName { get; set; }
        public Dictionary<ulong, bool> ActivePlanets { get; set; }
        public Dictionary<ulong, Planet> ActivePlanetObjects { get; set; }
        public List<Planet> RepublicActivePlanets { get; set; }
        public List<Planet> ImperialActivePlanets { get; set; }
        public List<ConquestData> ActiveData { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Conquest itm = obj as Conquest;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(Conquest itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.ActivePlanets != null)
            {
                if (itm.ActivePlanets == null)
                    return false;
                foreach (var kvp in this.ActivePlanets)
                {
                    var prev = false;
                    itm.ActivePlanets.TryGetValue(kvp.Key, out prev);
                    if (!kvp.Value.Equals(prev))
                        return false;
                }
            }
            if (this.DescId != itm.DescId)
                return false;
            if (this.Description != itm.Description)
                return false;
            if (this.DesignName != itm.DesignName)
                return false;
            if (this.Icon != itm.Icon)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.RepeatableObjectivesList != null)
            {
                if (itm.RepeatableObjectivesList == null)
                    return false;
                if (!this.RepeatableObjectivesList.SequenceEqual(itm.RepeatableObjectivesList))
                    return false;
            }
            if (this.OneTimeObjectivesList != null)
            {
                if (itm.OneTimeObjectivesList == null)
                    return false;
                if (!this.OneTimeObjectivesList.SequenceEqual(itm.OneTimeObjectivesList))
                    return false;
            }
            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedDescription, itm.LocalizedDescription))
                return false; 
            if (!ssComp.Equals(this.LocalizedName, itm.LocalizedName))
                return false; 
            
            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.RepeatableObjectivesList != null)
            {
                if (itm.RepeatableObjectivesList == null)
                    return false;
                if (!this.RepeatableObjectivesList.SequenceEqual(itm.RepeatableObjectivesList))
                    return false;
            }
            if (this.ParticipateGoal != itm.ParticipateGoal)
                return false;
            if (this.OneTimeObjectivesList != null)
            {
                if (itm.OneTimeObjectivesList == null)
                    return false;
                if (!this.OneTimeObjectivesList.SequenceEqual(itm.OneTimeObjectivesList))
                    return false;
            }
            else if (itm.OneTimeObjectivesList != null)
                return false;
            if (this.ActiveData != null)
            {
                if (itm.ActiveData == null)
                    return false;
                if (!this.ActiveData.SequenceEqual(itm.ActiveData))
                    return false;
            }
            else if (itm.ActiveData != null)
                return false;
            return true;
        }
    }

    public class ConquestObjective : IEquatable<ConquestObjective>
    {
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom;
        public Dictionary<Achievement, Dictionary<Planet, float>> ObjectiveList { get; set; }
        public long Id { get; set; }

        public ConquestObjective()
        {
            ObjectiveList = new Dictionary<Achievement, Dictionary<Planet, float>>();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ConquestObjective itm = obj as ConquestObjective;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(ConquestObjective itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.Id != itm.Id)
                return false;

            var ufComp = new DictionaryComparer<ulong, float>();
            if (this.ObjectiveList != null)
            {
                if (itm.ObjectiveList == null)
                    return false;
                if (this.ObjectiveList.Count != itm.ObjectiveList.Count)
                    return false;
                foreach (var kvp in this.ObjectiveList)
                {
                    var curDict = kvp.Value.ToDictionary(x => x.Key.Id, x => x.Value);
                    Dictionary<Planet, float> prev;
                    itm.ObjectiveList.TryGetValue(kvp.Key, out prev);
                    if (prev != null)
                    {
                        var prevDict = kvp.Value.ToDictionary(x => x.Key.Id, x => x.Value);
                        if (!ufComp.Equals(curDict, prevDict))
                            return false; 
                    }
                }
            }

            return true;
        }
    }

    public class Planet : GameObject, IEquatable<Planet>
    {
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom;
        public long NameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long DescId { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public string Icon { get; set; }
        public ulong Id { get; set; }
        public Dictionary<ulong, object> ExitList { get; set; }
        public ulong PrimaryAreaId { get; set; }
        public long TransportCost { get; set; }
        public long OrbtSupportCost { get; set; }
        public ulong OrbtSupportAblId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal Ability _OrbtSupportAbility { get; set; }
        public Ability OrbtSupportAbility
        {
            get
            {
                if (_OrbtSupportAbility == null)
                {
                    _OrbtSupportAbility = _dom.abilityLoader.Load(OrbtSupportAblId);
                }
                return _OrbtSupportAbility;
            }
        }
        public Dictionary<string, string> LocalizedInvasionBonus { get; set; }
        public string InvasionBonus { get; set; }
        public long InvasionBonusId { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Planet itm = obj as Planet;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(Planet itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.DescId != itm.DescId)
                return false;
            if (this.Description != itm.Description)
                return false;
            if (this.Icon != itm.Icon)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.InvasionBonus != itm.InvasionBonus)
                return false;
            if (this.InvasionBonusId != itm.InvasionBonusId)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedDescription, itm.LocalizedDescription))
                return false;
            if (!ssComp.Equals(this.LocalizedName, itm.LocalizedName))
                return false;
            if (!ssComp.Equals(this.LocalizedInvasionBonus, itm.LocalizedInvasionBonus))
                return false;

            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            if (this.OrbtSupportAblId != itm.OrbtSupportAblId)
                return false;
            if (this.OrbtSupportCost != itm.OrbtSupportCost)
                return false;
            if (this.PrimaryAreaId != itm.PrimaryAreaId)
                return false;
            if (this.ExitList != null)
            {
                if (itm.ExitList == null)
                    return false;
                if (this.ExitList.Count != itm.ExitList.Count)
                    return false;
                foreach(var kvp in this.ExitList)
                {
                    object prev;
                    itm.ExitList.TryGetValue(kvp.Key, out prev);
                    if (kvp.Value != prev)
                        return false;
                }
            }
            if (this.TransportCost != itm.TransportCost)
                return false;
            return true;
        }
    }

    public class ConquestData : IEquatable<ConquestData>
    {
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom;
        public long Id { get; set; }
        public ulong GuildQstId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal Quest _GuildQst { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Quest GuildQst
        {
            get
            {
                if (_GuildQst == null)
                {
                    _GuildQst = _dom.questLoader.Load(GuildQstId);
                }
                return _GuildQst;
            }
        }
        public ulong PersonalQstId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal Quest _PersonalQst { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Quest PersonalQst
        {
            get
            {
                if (_PersonalQst == null)
                {
                    _PersonalQst = _dom.questLoader.Load(PersonalQstId);
                }
                return _PersonalQst;
            }
        }
        public int OrderId { get; set; }
        public long RawStartTime { get; set; }
        public DateTime StartTime { get; set; }
        public int ActualOrderNum { get; set; }
        

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ConquestData itm = obj as ConquestData;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(ConquestData itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.GuildQstId != itm.GuildQstId)
                return false;
            if (this.PersonalQstId != itm.PersonalQstId)
                return false;
            if (this.OrderId != itm.OrderId)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.RawStartTime != itm.RawStartTime)
                return false;
            if (this.ActualOrderNum != itm.ActualOrderNum)
                return false;

            return true;
        }
    }
}
