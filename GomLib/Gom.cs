using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib
{
    public static class Gom
    {
        //public static dynamic Browser { get; private set; }

        //static Gom()
        //{
        //    Browser = new System.Dynamic.ExpandoObject();
        //}

        //internal static void AddObject(GomObject obj)
        //{
        //    // CURRENTLY DISABLED!
        //    return;
        //    // CURRENTLY DISABLED!

        //    string name = obj.Name;
        //    string[] path = name.Split('.');
        //    int numDirs = path.Length - 1;
        //    IDictionary<string, object> browserDir = Browser as IDictionary<string, object>;
        //    for (var i = 0; i < numDirs; i++)
        //    {
        //        object dirObj = null;
        //        dynamic dirExpando = null;
        //        if (!browserDir.TryGetValue(path[i], out dirObj))
        //        {
        //            dirExpando = new System.Dynamic.ExpandoObject();
        //            browserDir.Add(path[i], dirExpando);
        //            browserDir = dirExpando as IDictionary<string, object>;
        //        }
        //        else
        //        {
        //            browserDir = dirObj as IDictionary<string, object>;
        //        }

        //        if (browserDir == null)
        //        {
        //            Console.WriteLine("Cannot place {0} into the Gom Browser", obj.Name);
        //            return;
        //        }
        //    }

        //    object fileContainer;
        //    Dictionary<string, GomObject> files;
        //    if (!browserDir.TryGetValue("Files", out fileContainer))
        //    {
        //        files = new Dictionary<string, GomObject>();
        //        browserDir.Add("Files", files);
        //    }
        //    else
        //    {
        //        files = fileContainer as Dictionary<string, GomObject>;
        //    }
        //    files.Add(obj.Name, obj);

        //    //if (browserDir.ContainsKey(path[numDirs]))
        //    //{
        //    //    Console.WriteLine("Cannot place {0} into the Gom Browser", obj.Name);
        //    //}
        //    //else
        //    //{
        //    //    browserDir.Add(path[numDirs], obj);
        //    //}
        //}
    }
}
