using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using GomLib;

namespace tor_tools
{
    class Format_CNV
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public HashSet<string> animNames = new HashSet<string>();
        public HashSet<string> fxSpecNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;

        public Format_CNV(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void ParseCNVNodes(List<GomObject> cnvNodes)
        {
            foreach (GomObject obj in cnvNodes)
            {
                string under = obj.Name.ToLower().ToString().Replace('.', '_');
                string slash = obj.Name.ToLower().ToString().Replace('.', '/');
                string stb = "/resources/en-us/str/" + slash + ".stb";
                string acb = "/resources/en-us/bnk2/" + under + ".acb";
                string fxe = "/resources/en-us/fxe/" + slash + ".fxe";
                fileNames.Add(stb);
                fileNames.Add(acb);
                fileNames.Add(fxe);

                //Check for alien vo files.
                if(obj.Name.StartsWith("cnv.alien_vo"))
                {
                    fileNames.Add("/resources/bnk2/" + under + ".acb");
                }
                if (obj.Data.Dictionary.ContainsKey("cnvActionList"))
                {
                    var actionData = obj.Data.Get<List<object>>("cnvActionList");
                    if (actionData != null)
                    {
                        foreach (string action in actionData)
                        {
                            if (action.Contains("stg."))
                                continue;
                            animNames.Add(action.Split('.').Last().ToLower());
                        }                                                
                    }
                }

                if (obj.Data.Dictionary.ContainsKey("cnvActiveVFXList"))
                {
                    var vfxData = obj.Data.Get<Dictionary<object, object>>("cnvActiveVFXList");
                    if (vfxData != null)
                    {
                        foreach (KeyValuePair<object, object> kvp in vfxData)
                        {
                            List<object> value = (List<object>)kvp.Value;
                            if (value.Count > 0)
                            {
                                foreach (string vfx in value)
                                {
                                    fxSpecNames.Add(vfx.ToLower());
                                }                                                                
                            }
                        }                                                
                    }
                }
                obj.Unload();
            }
        }

        public void WriteFile()
        {
            if (!System.IO.Directory.Exists(this.dest + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.dest + "\\File_Names");
            if (this.fileNames.Count > 0)
            {
                System.IO.StreamWriter outputNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_file_names.txt", false);
                foreach (string file in fileNames)
                {
                    outputNames.Write(file.Replace("\\", "/") + "\r\n");
                }
                outputNames.Close();
                fileNames.Clear(); 
            }

            if (this.animNames.Count > 0)
            {
                System.IO.StreamWriter outputAnimNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_anim_names.txt", false);
                foreach (string file in animNames)
                {
                    outputAnimNames.Write(file.Replace("\\", "/") + "\r\n");
                }
                outputAnimNames.Close();
                animNames.Clear();
            }

            if (this.fxSpecNames.Count > 0)
            {
                System.IO.StreamWriter outputfxSpecNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_fxspec_names.txt", false);
                foreach (string file in fxSpecNames)
                {
                    outputfxSpecNames.Write(file.Replace("\\", "/") + "\r\n");
                }
                outputfxSpecNames.Close();
                fxSpecNames.Clear();
            }
           
            if(this.errors.Count > 0)
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
