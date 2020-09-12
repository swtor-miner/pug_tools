using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace PugTools
{
    class Format_DAT
    {

        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public HashSet<string> animFileNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;
        public string filename;
        private readonly HashSet<string> checkKeys = new HashSet<string>(new string[] { ".NormalMap2", ".NormalMap1", ".SurfaceMap", ".RampMap", ".Falloff", ".IlluminationMap", ".FxSpecName", ".EnvironmentMap", ".Intensity", ".PortalTarget", ".Color", ".gfxMovieName", ".DiffuseColor", ".ProjectionTexture" });
        public HashSet<string> portalTargets = new HashSet<string>();
        readonly Dictionary<uint, DatTypeId> Properties = new Dictionary<uint, DatTypeId>();

        public HashSet<string> CheckKeys => checkKeys;

        public Format_DAT(string dest, string ext)
        {
            this.dest = dest;
            extension = ext;
        }

        public void ParseDAT(Stream fileStream, string fullFileName, AssetBrowser form)
        {
            if (form is null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            filename = fullFileName;
            bool oldFormat = true;
            BinaryReader binReader = new BinaryReader(fileStream);
            int header = binReader.ReadInt32();
            if (header == 24)
            {
                oldFormat = false;
                //fileStream.Position = 4;
                char c = binReader.ReadChar();
                StringBuilder formatter = new StringBuilder();
                while (c != '\0')
                {
                    formatter.Append(c);
                    c = binReader.ReadChar();
                }
                string format = formatter.ToString();
                switch (format)
                {
                    case "ROOM_DAT_BINARY_FORMAT_":
                        ParseRoomDAT(binReader);
                        break;
                    case "AREA_DAT_BINARY_FORMAT_":
                        ParseAreaDAT(binReader);
                        break;
                    default:
                        break;
                }
            }

            if (oldFormat)
            {
                if (fileStream.CanSeek)
                    fileStream.Position = 0;
                else
                {
                    // string soin = ""; //dunno what to do here
                }
                StreamReader reader = new StreamReader(fileStream);
                string stream_line;
                List<string> stream_lines = new List<string>();
                while ((stream_line = reader.ReadLine()) != null)
                {
                    stream_lines.Add(stream_line.TrimStart());
                }
                reader.Close();
                if (stream_lines.Any(x => x.Contains("! Area Specification")))
                    ParseAreaDAT(stream_lines);
                else if (stream_lines.Any(x => x.Contains("! Room Specification")))
                    ParseRoomDAT(stream_lines);
                else if (stream_lines.Any(x => x.Contains("! Character Specification")))
                    ParseCharacterDAT(stream_lines);
                else
                    //throw new Exception("Unknown DAT Specification" + stream_lines[1]);
                    Console.WriteLine("Unknown DAT Specification" + stream_lines[1]);
            }
        }

        #region New Format Readers
        public void ParseAreaDAT(BinaryReader br)
        {
            br.BaseStream.Position = 0x1C; //Skip room header

            uint roomOffset = br.ReadUInt32();
            uint assetsOffset = br.ReadUInt32();
            _ = br.ReadUInt32();
            uint schemesOffset = br.ReadUInt32();
            uint terTexOffset = br.ReadUInt32();
            uint DydTexOffset = br.ReadUInt32();
            _ = br.ReadUInt32();
            _ = br.ReadUInt32();

            uint guidOffset = br.ReadUInt32();
            _ = br.ReadBytes(0x16); //Always (01 00) repeating

            br.BaseStream.Position = guidOffset;
            ulong areaGuid = br.ReadUInt64();

            string areaID = null;	//areaGuid not usually the correct ID in the file path

            if (filename.Contains("/resources/world/areas"))
            {
                areaID = filename.Replace("/resources/world/areas/", "").Replace("/area.dat", "");
                fileNames.Add("/resources/world/areas/" + areaID + "/mapnotes.not");
            }

            //Rooms
            br.BaseStream.Position = roomOffset;
            uint numRooms = br.ReadUInt32();
            for (uint i = 0; i < numRooms; i++)
            {
                uint nameLength = br.ReadUInt32();
                string room = ReadString(br, nameLength).ToLower();
                if (areaID != null)
                    fileNames.Add(string.Format("/resources/world/areas/{0}/{1}.dat", areaID, room));
                else
                    fileNames.Add(string.Format("/resources/world/areas/{0}/{1}.dat", areaGuid, room));
            }

            //Assets
            br.BaseStream.Position = assetsOffset;
            uint numAssets = br.ReadUInt32();
            for (uint i = 0; i < numAssets; i++)
            {
                _ = br.ReadUInt64();
                uint nameLength = br.ReadUInt32();
                string assetName = ReadString(br, nameLength);
                if (assetName.Contains(':') || assetName.Contains('#'))
                    continue;
                fileNames.Add("/resources" + assetName.ToLower().Replace("\\", "/"));
            }

            //Paths

            //Schemes
            br.BaseStream.Position = schemesOffset;
            uint numSchemes = br.ReadUInt32();
            for (uint i = 0; i < numSchemes; i++)
            {
                uint nameLength = br.ReadUInt32();
                _ = ReadString(br, nameLength);
                uint schemeLength = br.ReadUInt32();
                string scheme = ReadString(br, schemeLength);
                if (scheme.Contains("/"))
                {
                    int idx = 0;
                    while ((idx = scheme.IndexOf('/', idx)) != -1)
                    {
                        int end = scheme.IndexOf('|', idx);
                        int len = end - idx;
                        string final = scheme.Substring(idx, len).ToLower();
                        fileNames.Add(string.Format("/resources{0}.tex", final));
                        fileNames.Add(string.Format("/resources{0}.dds", final));
                        fileNames.Add(string.Format("/resources{0}.tiny.dds", final));
                        idx = end;
                    }
                }
            }

            //TERRAINTEXTURES
            br.BaseStream.Position = terTexOffset;
            uint numTerTex = br.ReadUInt32();
            for (uint i = 0; i < numTerTex; i++)
            {
                _ = br.ReadUInt64();
                uint nameLength = br.ReadUInt32();
                string terTexName = ReadString(br, nameLength);

                fileNames.Add(string.Format("/resources/art/shaders/materials/{0}.mat", terTexName.ToLower()));
                fileNames.Add(string.Format("/resources/art/shaders/environmentmaterials/{0}.emt", terTexName.ToLower()));
            }

            //TERRAINTEXTURES
            br.BaseStream.Position = DydTexOffset;
            uint numDydTex = br.ReadUInt32();
            for (uint i = 0; i < numDydTex; i++)
            {
                _ = br.ReadUInt32();
                uint nameLength = br.ReadUInt32();
                string terTexName = ReadString(br, nameLength);

                fileNames.Add(string.Format("/resources/art/shaders/materials/{0}.mat", terTexName.ToLower()));
                fileNames.Add(string.Format("/resources/art/shaders/environmentmaterials/{0}.emt", terTexName.ToLower()));
            }

            //DYDCHANNELPARAMS

            //SETTINGS
        }

        private static string ReadString(BinaryReader br, uint length)
        {
            long curpos = br.BaseStream.Position;
            long endpos = curpos + length;
            char c = br.ReadChar();
            StringBuilder builder = new StringBuilder();
            while (c != '\0' && br.BaseStream.Position < endpos)
            {
                builder.Append(c);
                c = br.ReadChar();
            }
            return builder.ToString();
        }

        public void ParseRoomDAT(BinaryReader br)
        {
            HashSet<string> fxspecs = new HashSet<string>();
            HashSet<string> textures = new HashSet<string>();

            br.BaseStream.Position = 0x1C; //Skip room header

            uint instanceOffset = br.ReadUInt32();
            _ = br.ReadUInt32();
            _ = br.ReadUInt32();
            _ = br.ReadUInt64(); //Always 281479271743491 : (03 00 01 00 01 00 01 00)

            uint fileNameLength = br.ReadUInt32();
            string filename = ReadString(br, fileNameLength);
            fileNames.Add(string.Format("/resources{0}", filename));

            string area = filename.Remove(filename.LastIndexOf('/') + 1);
            fileNames.Add(string.Format("/resources{0}", area + "area.dat"));
            fileNames.Add(string.Format("/resources{0}", area + "mapnotes.not"));

            //Instances
            br.BaseStream.Position = instanceOffset;
            uint numInstances = br.ReadUInt32();
            for (uint i = 0; i < numInstances; i++)
            {
                uint instanceHeader = br.ReadUInt32();
                if (instanceHeader != 0xABCD1234)  // 0x3412CDAB
                {
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                    // string sdifn = "";
                }

                _ = br.ReadByte();
                _ = br.ReadUInt64();
                _ = br.ReadUInt64();
                _ = br.ReadByte();
                uint numProperties = br.ReadUInt32();
                uint propteriesLength = br.ReadUInt32();

                long startOffset = br.BaseStream.Position;
                _ = br.ReadByte();
                try
                {
                    for (uint p = 0; p < numProperties; p++)
                    {
                        DatTypeId type = (DatTypeId)br.ReadByte();
                        uint propertyId = br.ReadUInt32();

                        if (!Properties.ContainsKey(propertyId))
                        {
                            Properties.Add(propertyId, type);
                        }
                        else if (Properties[propertyId] != type)
                        {
                            //throw new IndexOutOfRangeException();
                            DatTypeId oldtype = Properties[propertyId];
                        }
                        object o = null;
                        switch (type)
                        {
                            case DatTypeId.Boolean:

                                byte b = br.ReadByte();
                                if (b > 1)
                                    throw new IndexOutOfRangeException();
                                bool boo = Convert.ToBoolean(b);
                                o = boo;
                                break;
                            case DatTypeId.Unknown1:
                                int ival = br.ReadInt32();
                                if (ival > 1)
                                    o = ival;
                                break;
                            //case DatTypeId.Unknown2:
                            //    break;
                            case DatTypeId.UInt32:
                                uint val = br.ReadUInt32();
                                o = val;
                                break;
                            case DatTypeId.Single:
                                float flo = br.ReadSingle();
                                o = flo;
                                break;
                            case DatTypeId.UInt64:
                                ulong lval = br.ReadUInt64();
                                o = lval;
                                break;
                            case DatTypeId.Vector3:
                                List<float> vec3 = new List<float>
                                {
                                    br.ReadSingle(),
                                    br.ReadSingle(),
                                    br.ReadSingle()
                                };
                                o = vec3;
                                break;
                            case DatTypeId.Unknown7:
                                //byte[] bytes = br.ReadBytes(16);
                                List<float> vec4 = new List<float>
                                {
                                    br.ReadSingle(),
                                    br.ReadSingle(),
                                    br.ReadSingle(),
                                    br.ReadSingle()
                                };
                                o = vec4;
                                break;
                            case DatTypeId.String:
                                uint strlen = br.ReadUInt32();
                                StringBuilder str = new StringBuilder((int)strlen);
                                char c1 = br.ReadChar();
                                char c2 = br.ReadChar();
                                uint charsRead = 1;
                                while (c1 != '\0' && c1 != '\0' && charsRead < strlen)
                                {
                                    str.Append(c1);
                                    if (c2 != '\0')
                                        throw new IndexOutOfRangeException();
                                    c1 = br.ReadChar();
                                    c2 = br.ReadChar();
                                    charsRead++;
                                }
                                o = str.ToString();

                                if (!string.IsNullOrWhiteSpace((string)o))
                                {
                                    switch (propertyId)
                                    {
                                        case 3261558584:    // FxSpecName
                                            fxspecs.Add((string)o);
                                            break;
                                        case 2393024011:    // spnAnimation or spnNpcIdleAnimationName
                                            animFileNames.Add(((string)o).ToLower());
                                            break;
                                        case 964697786:     // Tag
                                            fileNames.Add("/resources" + area + (string)o + ".dat");
                                            textures.Add((area + (string)o));
                                            break;

                                        //Skip Start
                                        case 2957064701:    // PortalTarget
                                        case 4255290973:    // rgnVolumeData
                                        case 669968511:     // rgnCharacteristics
                                        case 3166688232:    // ParentMapTag
                                        case 1768825245:    // tesselation 
                                        case 240466284:     // resolution
                                        case 3106719576:    // StopEvent 
                                        case 948461446:     // PlayEvent
                                        case 466906898:     // rgnRespawnMedCenter
                                        case 3160985587:    // Intensity
                                        case 384379389:     // Range
                                        case 3430452781:    // FxRespawnDelay
                                        case 3629101973:    // TriggerParam
                                        case 273365031:     // Speed
                                        case 2335395941:    // Path
                                        case 713588192:     // spnTagFromEncounter                                        
                                        case 1467655203:    // wtrVertexData
                                        case 113668568:     // DepthTexture
                                        case 3060549674:    // spnPhaseInstanceName
                                        case 773762347:     // name
                                        case 3235228203:    // FxMaxSpawnDistance
                                        case 446782081:     // DiffuseColor
                                        case 2793072227:    // Color
                                        case 3179516067:    // TriggerScript
                                        case 3424594045:    // JointFlags
                                        case 3084732969:    // Deformation_X
                                        case 3084732970:    // Deformation_Y
                                        case 4158591558:    // Divisions
                                        case 3839584892:    // LightningWidth
                                        case 2857627687:    // DeltaRotation3D                                        
                                        case 3522231145:    // LeafTinting
                                        case 4012120889:    // GlossColor
                                        case 489737334:     // LODFactor
                                        case 3402983087:    // BoneName
                                        case 4268140818:    // some type of color
                                        case 3069428699:    // regionEdgeData
                                        case 2766070679:    // DeepColor
                                        case 3671420588:    // FogColor1
                                        case 3671420589:    // FogColor2
                                        case 1620832956:    // FogColorSky
                                            break;
                                        //Skip End

                                        case 999479220:     // Falloff
                                        case 1820631501:    // IlluminationMap
                                        case 1117554570:    // RampMap
                                        case 1412492047:    // SurfaceMap 
                                        case 2545768381:    // NormalMap2
                                        case 2545768380:    // NormalMap1
                                        case 2829380834:    // gfxMovieName
                                        case 3003166540:    // ProjectionTexture                                            
                                        default:
                                            textures.Add((string)o);
                                            break;
                                    }
                                }
                                break;
                            case DatTypeId.Data:
                                uint datalen = br.ReadUInt32();
                                br.BaseStream.Position += datalen;
                                break;
                            default:
                                long curpos = br.BaseStream.Position; //this is for debugging new formats found
                                byte[] bities = br.ReadBytes(32);
                                br.BaseStream.Position = curpos;
                                throw new IndexOutOfRangeException();
                        }
                        //break;

                    }

                }
                catch (Exception)
                {
                    br.BaseStream.Position = startOffset + propteriesLength;
                }

            }

            foreach (var fxs in fxspecs)
            {
                fileNames.Add(string.Format("/resources/art/fx/fxspec/{0}.fxspec", fxs.ToLower()).Replace("\\", "/").Replace("//", "/").Replace(".fxspec.fxspec", ".fxspec"));
            }
            foreach (var tex in textures)
            {
                string file = ("/resources/" + tex.ToLower()).Replace("\\", "/").Replace("//", "/").Replace(".dds", "");
                fileNames.Add(string.Format("{0}.dds", file));
                fileNames.Add(string.Format("{0}.tiny.dds", file));
                fileNames.Add(string.Format("{0}.tex", file));
            }
        }
        #endregion
        #region Old Format Readers
        public void ParseAreaDAT(List<string> lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            return;
        }
        public void ParseRoomDAT(List<string> lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            return;
        }
        public void ParseCharacterDAT(List<string> lines)
        {
            List<string> sectionNames = new List<string>(new string[] { "[PARTS]" });

            lines.RemoveAt(0);
            string skeleton_name = lines[0].Split(new string[] { "for " }, StringSplitOptions.None).Last().Trim();
            fileNames.Add("/resources/art/dynamic/spec/" + skeleton_name + ".gr2");

            Dictionary<string, string> parts = new Dictionary<string, string>();

            string current = "";

            foreach (string line in lines)
            {
                if (sectionNames.Contains(line))
                    current = line;
                else
                {
                    if (line.Contains(':') || line.Contains('#'))
                        continue;
                    switch (current)
                    {
                        case "[PARTS]":
                            if (line == "")
                                continue;
                            string[] split = line.Split('=');
                            if (!parts.ContainsKey(split[0]))
                            {
                                parts.Add(split[0], split[1]);
                            }
                            break;
                        default:
                            break;
                    }

                }
            }

            if (parts.ContainsKey("Model"))
            {
                fileNames.Add("/resources/art/dynamic/spec/" + parts["Model"]);
                fileNames.Add("/resources/art/dynamic/spec/" + parts["Model"].Replace(".dyc", ".dat"));
                fileNames.Add("/resources/art/dynamic/spec/" + parts["Model"].Replace(".dyc", ".mag"));
            }

            if (parts.ContainsKey("AnimMetadataFqn"))
            {
                string[] temp = parts["AnimMetadataFqn"].Split(',');
                TorLib.Assets assets = TorLib.AssetHandler.Instance.GetCurrentAssets();
                foreach (string item in temp)
                {
                    string tempName = "/resources/" + item.Replace('\\', '/').Replace("//", "/");
                    fileNames.Add(tempName);
                    if (parts.ContainsKey("AnimNetworkFolder"))
                    {
                        string netfold = string.Format("/resources/{0}", parts["AnimNetworkFolder"].Replace('\\', '/').Replace("//", "/"));
                        var file = assets.FindFile(tempName);
                        if (file != null)
                        {
                            try
                            {
                                using (var fileStream = file.OpenCopyInMemory())
                                {
                                    var doc = XDocument.Load(fileStream);

                                    XElement aamElement = doc.Element("aam");
                                    if (aamElement != null)
                                    {
                                        var actionElement = aamElement.Element("actions");
                                        if (actionElement != null)
                                        {
                                            var actionList = actionElement.Elements("action");

                                            foreach (var action in actionList)
                                            {
                                                var actionName = action.Attribute("name").Value;
                                                if (action.Attribute("actionProvider") != null)
                                                {
                                                    var actionProvider = action.Attribute("actionProvider").Value + ".mph";
                                                    animFileNames.Add(netfold + actionProvider);
                                                    animFileNames.Add(netfold + actionProvider + ".amx");
                                                }
                                                if (action.Attribute("animName") != null)
                                                {
                                                    var animationName = action.Attribute("animName").Value;
                                                    if (actionName != animationName)
                                                    {
                                                        animationName += ".jba";

                                                        animFileNames.Add(netfold + animationName);
                                                    }
                                                }
                                                actionName += ".jba";
                                                animFileNames.Add(netfold + actionName);
                                            }
                                        }

                                        XElement networkElem = aamElement.Element("networks");
                                        if (networkElem != null)
                                        {
                                            var networkList = networkElem.Descendants("literal");
                                            foreach (var network in networkList)
                                            {
                                                var fqnName = network.Attribute("fqn").Value;
                                                if (fqnName != null)
                                                {
                                                    animFileNames.Add(netfold + fqnName);
                                                    animFileNames.Add(netfold + fqnName + ".amx");
                                                }
                                            }
                                        }
                                        var inputElement = aamElement.Element("inputs");
                                        if (inputElement != null)
                                        {
                                            var inputList = inputElement.Elements("input").Descendants("value");
                                            foreach (var input in inputList)
                                            {
                                                var fqnName = input.Attribute("name").Value;
                                                if (fqnName != null)
                                                {
                                                    animFileNames.Add(netfold + fqnName);
                                                    animFileNames.Add(netfold + fqnName + ".amx");
                                                    animFileNames.Add(netfold + fqnName + ".jba");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                errors.Add("File: " + tempName);
                                errors.Add(ex.Message + ":");
                                errors.Add(ex.StackTrace);
                                errors.Add("");
                            }
                        }
                    }
                }
            }

            if (parts.ContainsKey("AnimLibraryFqn"))
            {
                string tempName = "/resources/" + parts["AnimLibraryFqn"];
                fileNames.Add(tempName.Replace('\\', '/').Replace("//", "/"));
            }

            if (parts.ContainsKey("AnimShareMetadataFqn"))
            {
                string tempName = "/resources/" + parts["AnimShareMetadataFqn"];
                fileNames.Add(tempName.Replace('\\', '/').Replace("//", "/"));
            }

            /*** Disabled - Enable to find new keys that have slashes
            HashSet<string> animKeys = new HashSet<string>(new string[] { "AnimShareMetadataFqn", "AnimLibraryFqn", "AnimMetadataFqn", "Model", "AnimNetworkFolder" });
            foreach (var part in parts)
            {
                if (animKeys.Contains(part.Key))
                    continue;
                if (part.Value.Contains('\\'))
                {
                    Console.WriteLine(part.Key.ToString());
                }

                if (part.Value.Contains('/'))
                {
                    Console.WriteLine(part.Key.ToString());
                }              
            }
            ***/
        }
        #endregion

        public void WriteFile()
        {
            if (!Directory.Exists(dest + "\\File_Names"))
                Directory.CreateDirectory(dest + "\\File_Names");
            if (fileNames.Count > 0)
            {
                StreamWriter outputNames = new StreamWriter(dest + "\\File_Names\\" + extension + "_file_names.txt", false);
                foreach (string file in fileNames)
                {
                    outputNames.WriteLine(file.Replace("\\", "/"));
                }
                outputNames.Close();
                fileNames.Clear();
            }

            if (animFileNames.Count > 0)
            {
                StreamWriter outputAnimNames = new StreamWriter(dest + "\\File_Names\\" + extension + "_anim_file_names.txt", false);
                foreach (string file in animFileNames)
                {
                    outputAnimNames.WriteLine(file.Replace("\\", "/"));
                }
                outputAnimNames.Close();
                animFileNames.Clear();
            }

            if (errors.Count > 0)
            {
                StreamWriter outputErrors = new StreamWriter(dest + "\\File_Names\\" + extension + "_error_list.txt", false);
                foreach (string error in errors)
                {
                    outputErrors.WriteLine(error);
                }
                outputErrors.Close();
                errors.Clear();
            }
        }
    }

    public enum DatTypeId : byte
    {
        Boolean = 0x00,
        Unknown1 = 0x01,
        Unknown2 = 0x02,
        UInt32 = 0x03, // may be Int32
        Single = 0x04,
        UInt64 = 0x05,
        Vector3 = 0x06,
        Unknown7 = 0x07,
        String = 0x08,
        Data = 0x09
    }
}
