using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TorLib;
using GomLib;

namespace tor_tools
{
    class Format_HYD
    {
        public string dest = "";
        public HashSet<string> animFileNames = new HashSet<string>();
        public HashSet<string> vfxFileNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;

        public Format_HYD(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }

        public void ParseHYD(List<GomObject> hydNodes)
        {
            foreach (GomObject obj in hydNodes)
            {
                Dictionary<object, object> hydScriptMap = obj.Data.ValueOrDefault<Dictionary<object, object>>("hydScriptMap", null);
                if (hydScriptMap != null)
                {
                    foreach (var scriptMapItem in (Dictionary<object, object>)hydScriptMap)
                    {
                        var scriptMapItem2 = (GomLib.GomObjectData)scriptMapItem.Value;
                        List<object> hydScriptBlocks = scriptMapItem2.ValueOrDefault<List<object>>("hydScriptBlocks", null);
                        if (hydScriptBlocks != null)
                        {
                            foreach (var hydScriptBlocksItem in (List<object>)hydScriptBlocks)
                            {
                                var hydScriptBlocksItem2 = (GomLib.GomObjectData)hydScriptBlocksItem;
                                List<object> hydActionBlocks = hydScriptBlocksItem2.ValueOrDefault<List<object>>("hydActionBlocks", null);
                                if (hydActionBlocks != null)
                                {
                                    foreach (var hydActionBlocksItem in (List<object>)hydActionBlocks)
                                    {
                                        var hydActionBlocksItem2 = (GomLib.GomObjectData)hydActionBlocksItem;
                                        List<object> hydActions = hydActionBlocksItem2.ValueOrDefault<List<object>>("hydActions", null);
                                        if (hydActions != null)
                                        {
                                            foreach (var hydActionsItem in (List<object>)hydActions)
                                            {
                                                var hydActionsItem2 = (GomLib.GomObjectData)hydActionsItem;
                                                var action = hydActionsItem2.ValueOrDefault<object>("hydAction", "").ToString();
                                                var value = hydActionsItem2.ValueOrDefault<object>("hydValue", "").ToString().ToLower();
                                                if (action.Contains("Animation"))
                                                    animFileNames.Add(value);
                                                else if (action.Contains("VFX"))
                                                    vfxFileNames.Add(value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                obj.Unload();
            }
        }

        public void WriteFile(bool _ = false)
        {
            if (!System.IO.Directory.Exists(this.dest + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.dest + "\\File_Names");
            if (this.animFileNames.Count > 0)
            {
                System.IO.StreamWriter outputAnimFileNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_anim_file_names.txt", false);
                foreach (string item in animFileNames)
                {
                    if (item != "")
                        outputAnimFileNames.WriteLine(item);
                }
                outputAnimFileNames.Close();
                animFileNames.Clear();
            }

            if (this.vfxFileNames.Count > 0)
            {
                System.IO.StreamWriter outputVfxFileNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_fxspec_file_names.txt", false);
                foreach (string item in vfxFileNames)
                {
                    if (item != "")
                    {
                        if (item.Contains("art/"))
                        {
                            string output = "/resources/" + item + ".fxspec";
                            outputVfxFileNames.WriteLine(output.Replace("//", "/").Replace(".fxspec.fxspec", ".fxspec"));
                        }
                        else
                        {
                            string output = "/resources/art/fx/fxspec/" + item + ".fxspec";
                            outputVfxFileNames.WriteLine(output.Replace("//", "/").Replace(".fxspec.fxspec", ".fxspec"));
                        }
                    }
                }
                outputVfxFileNames.Close();
                vfxFileNames.Clear();
            }

            if (this.errors.Count > 0)
            {
                System.IO.StreamWriter outputErrors = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_error_list.txt", false);
                foreach (string error in errors)
                {
                    outputErrors.Write(error + "\r\n");
                }
                outputErrors.Close();
                errors.Clear();
            }
        }
    }
}
