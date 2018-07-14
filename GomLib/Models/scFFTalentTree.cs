using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class scFFTalentTree : IEquatable<scFFTalentTree>
    {
        [JsonIgnore]
        public Ability Ability { get; set; }
        public string AbilityB62Id { get { if(Ability != null) return Ability.Id.ToMaskedBase62(); return null; } }
        public Dictionary<int, Dictionary<int, List<object>>> Tree { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            scFFTalentTree stt = obj as scFFTalentTree;
            if (stt == null) return false;

            return Equals(stt);
        }

        public bool Equals(scFFTalentTree stt)
        {
            if (stt == null) return false;

            if (ReferenceEquals(this, stt)) return true;

            if (this.Ability != null)
            {
                if (stt.Ability != null)
                {
                    if (!this.Ability.Equals(stt.Ability))
                        return false;
                }
                else
                    return false;
            }
            else if (stt.Ability != null)
                return false;
            if (this.Tree != null)
            {
                if (stt.Tree != null)
                {
                    if (!this.Tree.Keys.SequenceEqual(stt.Tree.Keys))
                        return false;
                    foreach (var kvp in this.Tree)
                    {
                        Dictionary<int, List<object>> prevValue;
                        stt.Tree.TryGetValue(kvp.Key, out prevValue);
                        foreach (var innerKvp in kvp.Value)
                        {
                            List<object> prevObj;
                            prevValue.TryGetValue(innerKvp.Key, out prevObj);
                            if (!innerKvp.Value[0].Equals(prevObj[0]))
                                return false;
                            if (innerKvp.Value[1] != null)
                            {
                                if (prevObj[1] != null)
                                {
                                    if (innerKvp.Value[1].GetType() == typeof(ScriptEnum))
                                    {
                                        if (((ScriptEnum)innerKvp.Value[1]).Value != ((ScriptEnum)prevObj[1]).Value)
                                            return false;
                                    }
                                    else
                                    {
                                        throw new TypeAccessException();
                                    }
                                }
                                else
                                    return false;
                            }
                            else if (prevObj[1] != null)
                                return false;
                        }
                    }
                }
                else
                    return false;
            }
            else if (stt.Tree != null)
                return false;
            return true;
        }
    }
}
