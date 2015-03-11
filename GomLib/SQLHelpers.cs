using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GomLib
{
    public static class SQLHelpers
    {
        public static List<string> GetPropertyValues(object obj, List<SQLProperty> properties)
        {
            List<string> results = new List<string>();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            foreach (var property in properties)
            {
                var x = obj.GetType().GetProperty(property.PropertyName);
                if (x == null)
                    continue;
                if(property.JsonSerialize)
                    results.Add(sqlSani(JsonConvert.SerializeObject(x.GetValue(obj), settings)));
                else
                    results.Add(sqlSani((x.GetValue(obj) ?? "").ToString()));
            }
            return results;
        }

        public static string ToSQL(object obj, SQLData sql, string patchVersion)
        {
            string s = "', '";
            string value = String.Format("('{0}', '', '{1}', '{2}')", sqlSani(patchVersion), String.Join(s, GetPropertyValues(obj, sql.SQLProperties)), obj.GetHashCode());
            return value;
        }

        public static string sqlSani(string str)
        {
            if (str == null) return "";
            return MySql.Data.MySqlClient.MySqlHelper.EscapeString(str);
        }
    }

    public class SQLData
    {
        public SQLData() { }

        public SQLData(List<SQLProperty> sqlProp)
        {
            SQLProperties = sqlProp;
        }

        public List<SQLProperty> SQLProperties { get; set; }

        public static List<SQLProperty> BaseProperties = new List<SQLProperty>
        {
            new SQLProperty("GetHashCode()", "Hash", "int(11) NOT NULL")
        };
    }

    public class SQLProperty
    {
        public SQLProperty(string name, string propName, string type)
        {
            SetValues(name, propName, type);
            IsPrimaryKey = false;
        }

        public SQLProperty(string name, string propName, string type, bool isPriKey)
        {
            SetValues(name, propName, type);
            IsPrimaryKey = isPriKey;
        }

        public SQLProperty(string name, string propName, string type, bool isPriKey, bool jSerialize)
        {
            SetValues(name, propName, type);
            IsPrimaryKey = isPriKey;
            JsonSerialize = jSerialize;
        }

        public string Name;
        public string PropertyName;
        public string Type;
        public bool IsPrimaryKey = false;
        public bool JsonSerialize = false;

        internal void SetValues(string name, string propName, string type)
        {
            Name = name;
            PropertyName = propName;
            Type = type;
        }
    }
}
