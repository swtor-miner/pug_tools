using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TorLib
{
    public static class Extensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            foreach (var item in source)
                action(item);

            return source;
        }

        public static void CopyTo(this System.IO.Stream source, System.IO.Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            long totalBytes = 0;
            int bytesRead = 0;

            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
            {
                target.Write(buf, 0, bytesRead);
                totalBytes += bytesRead;
            }
        }

        public static void CopyTo(this System.IO.Stream input, System.IO.Stream output, byte[] buffer)
        {
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public static long AsLong(this XElement el, long defaultValue = 0)
        {
            if (el == null) { return defaultValue; }
            var val = el.Value;
            if (String.IsNullOrEmpty(val))
            {
                return defaultValue;
            }

            long result;
            if (!long.TryParse(val, out result))
            {
                return defaultValue;
            }

            return result;
        }

        public static int AsInt(this XElement el, int defaultValue = 0)
        {
            if (el == null) { return defaultValue; }
            var val = el.Value;
            if (String.IsNullOrEmpty(val))
            {
                return defaultValue;
            }

            int result;
            if (!Int32.TryParse(val, out result))
            {
                return defaultValue;
            }

            return result;
        }

        public static int AsInt(this XAttribute el, int defaultValue = 0)
        {
            if (el == null) { return defaultValue; }
            var val = el.Value;
            if (String.IsNullOrEmpty(val))
            {
                return defaultValue;
            }

            int result;
            if (!Int32.TryParse(val, out result))
            {
                return defaultValue;
            }

            return result;
        }

        public static bool AsBool(this XAttribute el, bool defaultValue = false)
        {
            if (el == null) { return defaultValue; }
            var val = el.Value;
            if (String.IsNullOrEmpty(val))
            {
                return defaultValue;
            }

            bool result;
            if (!Boolean.TryParse(val, out result))
            {
                return defaultValue;
            }

            return result;
        }

        private static Dictionary<int, string> globalIdToFqnMap = new Dictionary<int, string>();
        public static int AsId(this XAttribute el, string fqn, Dictionary<int,string> idToFqnMap = null)
        {
            if (el == null) throw new ArgumentNullException("el");

            var val = el.Value;
            if (String.IsNullOrEmpty(val))
            {
                throw new ArgumentException("Attribute has no content!", "el");
            }

            ulong result;
            if (!ulong.TryParse(val, out result))
            {
                throw new ArgumentException("Attribute is not an integer!", "el");
            }

            if ((result & 0xffffffff) != 0)
            {
                throw new InvalidOperationException("GUID has some of lower 32 bits set for " + fqn);
            }

            int id = (int)(result >> 32);

            //if (idToFqnMap != null)
            //{
            //    if (idToFqnMap.ContainsKey(id))
            //    {
            //        throw new InvalidOperationException(String.Format("Duplicate ID for {0} and {1}", fqn, idToFqnMap[id]));
            //    }
            //    else
            //    {
            //        idToFqnMap.Add(id, fqn);
            //    }
            //}
            
            if (globalIdToFqnMap.ContainsKey(id))
            {
                throw new InvalidOperationException(String.Format("Duplicate ID for {0} and {1}", fqn, globalIdToFqnMap[id]));
            }
            else
            {
                globalIdToFqnMap.Add(id, fqn);
            }

            return id;
        }

        public static long AsLong(this XAttribute el, long defaultValue = 0)
        {
            if (el == null) { return defaultValue; }
            var val = el.Value;
            if (String.IsNullOrEmpty(val))
            {
                return defaultValue;
            }

            long result;
            if (!long.TryParse(val, out result))
            {
                return defaultValue;
            }

            return result;
        }

        public static int? AsNullableInt(this XElement el)
        {
            if (el == null) { return null; }
            var val = el.Value;
            if (String.IsNullOrEmpty(val))
            {
                return null;
            }

            int result;
            if (!Int32.TryParse(val, out result))
            {
                return null;
            }

            return result;
        }

        public static float AsFloat(this XElement el, float defaultValue = 0f)
        {
            if (el == null) { return defaultValue; }
            var val = el.Value;
            if (String.IsNullOrEmpty(val))
            {
                return defaultValue;
            }

            float result;
            if (!Single.TryParse(val, out result))
            {
                return defaultValue;
            }

            return result;
        }

        public static bool AsBool(this XElement el, bool defaultValue = false)
        {
            if (el == null) { return defaultValue; }
            var val = el.Value;
            if (String.IsNullOrEmpty(val))
            {
                return defaultValue;
            }

            if (val.ToUpper() == "TRUE") { return true; }
            if (val.ToUpper() == "FALSE") { return false; }
            return defaultValue;
        }

        public static float AsDuration(this XElement el)
        {
            if (el == null) { return 0; }

            var durString = (string)el;
            if (String.IsNullOrEmpty(durString))
            {
                return 0;
            }

            var parts = durString.Split(':');
            float mult = 1;
            float result = 0;
            foreach (var part in parts.Reverse())
            {
                result += mult * Single.Parse(part);
                mult = mult * 60;
            }

            return result;
        }

        public static int ParseDuration(this string str)
        {
            if (String.IsNullOrEmpty(str)) return 0;

            string[] vals = str.Split(':');
            int mult = 1;
            int seconds = 0;
            for (var i = vals.Length - 1; i >= 0; i--)
            {
                seconds += mult * Int32.Parse(vals[i]);
                mult *= 60;
            }
            return seconds;
        }

        //public static string LocalizedString(this XElement el, StringTable strTable, string fqn = null)
        //{
        //    if (el == null) return null;

        //    string result = null;
        //    // If we have a string table, try to look up a localized string
        //    if (strTable != null)
        //    {
        //        long locId = el.Attribute("LocID").AsLong();
        //        result = strTable.GetText(locId, fqn);
        //        if (result != null) return result;
        //    }

        //    // If we get here, try to just read in the <text> child element
        //    var txtEl = el.Element("text");
        //    result = (string)txtEl;

        //    // If there's no <text> child element, return the text of the passed element
        //    if (result == null)
        //    {
        //        result = (string)el;
        //    }

        //    return result;
        //}
    }
}
