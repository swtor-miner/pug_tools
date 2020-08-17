using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib
{
    public class GomObjectData// : System.Dynamic.DynamicObject
    {
        private readonly Dictionary<string, object> data = new Dictionary<string, object>();

        public IDictionary<string, object> Dictionary { get { return data; } }

        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        //public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        //{
        //    string key = binder.Name;
        //    if (!data.TryGetValue(key, out result))
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        //{
        //    string key = binder.Name;

        //    data[key] = value;

        //    return true;
        //}

        public T Get<T>(string key)
        {
            return (T)data[key];
        }

        public T ValueOrDefault<T>(string key, T defaultValue)
        {
            if (ContainsKey(key))
            {
                return (T)data[key];
            }
            else
            {
                return defaultValue;
            }
        }
        public T ValueOrDefault<T>(string key)
        {
            if (ContainsKey(key))
            {
                return (T)data[key];
            }
            else
            {
                return default;
            }
        }
    }
}
