using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

namespace GomLib
{
    public class GomObject : DomType, IEquatable<GomObject>
    {
        internal ulong ClassId { get; set; }
        public DomClass DomClass { get; internal set; }

        public List<DomClass> GlommedClasses { get; internal set; }

        internal int DataLength { get; set; }
        internal byte[] DataBuffer { get; set; }

        public int Offset20 { get; set; }
        public short NumGlommed { get; set; }
        public short Offset26 { get; set; }
        public int ObjectSizeInFile { get; set; }
        public short Offset2C { get; set; }
        public short Offset2E { get; set; }
        public byte Offset30 { get; set; }
        public byte Offset31 { get; set; }

        /// <summary>Adler32 Checksum</summary>
        public long Checksum { get; set; }

        public int Zeroes { get; private set; }

        public int InstanceType { get; internal set; }
        public int NumFields { get; internal set; }

        private GomObjectData _data;
        public GomObjectData Data { get { if (!IsLoaded) { this.Load(); } return _data; } }

        internal bool IsCompressed { get; set; }
        internal int NodeDataOffset { get; set; }
        private bool IsLoaded { get; set; }

        private bool isUnloaded;

        private void SetIsUnloaded(bool value)
        {
            IsUnloaded = value;
        }

        public int DecompressedLength { get; set; }
        //public byte[] FirstBytes { get; set; }

        public Dictionary<string, SortedSet<ulong>> References { get; set; }
        public Dictionary<ulong, Dictionary<string, SortedSet<ulong>>> ProtoReferences { get; set; }
        public Dictionary<ulong, string> FullReferences { get; set; }
        public bool IsUnloaded { get => isUnloaded; set => isUnloaded = value; }

        public override void Link(DataObjectModel dom)
        {
            this.Dom_ = dom;
            base.Link(dom);
            DomClass = Dom_.Get<DomClass>(ClassId);
        }

        public XElement Print()
        {
            if (!IsLoaded) Load();

            //writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            string domClass = DomClass.ToString();
            if (domClass == "")
                domClass = "None";
            XElement root = new XElement(domClass, new XAttribute("Id", Id), //writer.WriteLine("<{0}>", domClass);
                new XAttribute("Name", Name)); //writer.WriteLine("  <node name=\"{0}\" nodeid=\"{1}\">", Name, Id);
            if (!String.IsNullOrEmpty(Description))
            {
                root.Add(new XElement("Description", Description)); //writer.WriteLine("<description text=\"{0}\" />", Description);
            }

            //writer.WriteLine("Compressed Length: 0x{0:X}", DataLength);
            //writer.WriteLine("Offs 0x14: 0x{0:X}", Offset20);
            //writer.WriteLine("NumGlommed: 0x{0:X}", NumGlommed);
            //writer.WriteLine("Offs 0x22: 0x{0:X}", Offset26);
            //writer.WriteLine("ObjSizeInFile: 0x{0:X}", ObjectSizeInFile);
            //writer.WriteLine("Offs 0x28: 0x{0:X}", Offset2C);
            //writer.WriteLine("Offs 0x2A: 0x{0:X}", Offset2E);
            //writer.WriteLine("Offs 0x2C: 0x{0:X}", Offset30);
            //writer.WriteLine("Offs 0x2D: 0x{0:X}", Offset31);
            //writer.WriteLine("Zeroes: {0}", Zeroes);

            //writer.Write("Data: ");
            //for (var i = 0; i < 0xF; i++)
            //{
            //    writer.Write("{0:X2}", FirstBytes[i]);
            //}
            //writer.WriteLine();

            var dataDict = Data;
            if (dataDict != null)
            {
                foreach (var kvp in dataDict.Dictionary.Skip(3))
                {
                    root.Add(PrintVal(kvp.Key, kvp.Value));
                }
            }
            //writer.WriteLine("  </node>");
            //writer.WriteLine("</{0}>", domClass);
            return root;
        }

        private XElement PrintVal(string key, object val)
        {
            XElement element;
            string verifiedKey = key;
            //if (System.Text.RegularExpressions.Regex.IsMatch(key, @"^\-?\d"))
            //verifiedKey = "Field_" + key;
            if (val is System.Collections.IList)
            {
                System.Collections.IList valList = val as System.Collections.IList;
                element = new XElement("List", new XAttribute("Id", verifiedKey)); //writer.WriteLine("{0}<List Id=\"{1}\">", tabs, verifiedKey);
                for (var i = 0; i < valList.Count; i++)
                {
                    element.Add(PrintVal(i.ToString(), valList[i]));
                }
                //writer.WriteLine("{0}</List>", tabs);
            }
            else if (val is IDictionary<object, object>)
            {
                var valDict = val as IDictionary<object, object>;
                element = new XElement("IList", new XAttribute("Id", verifiedKey)); //writer.WriteLine("{0}<IList Id=\"{1}\">", tabs, verifiedKey);
                foreach (var valKvp in valDict)
                {
                    var value = valKvp.Value;
                    if (value != null)
                    {
                        if (value.GetType() == typeof(string))
                        {
                            string stringValue = null;
                            if (value != null)
                            {
                                stringValue = System.Security.SecurityElement.Escape(value.ToString()).Replace("\r\n", "&#xA;");
                            }
                            element.Add(PrintVal(valKvp.Key.ToString(), stringValue));
                        }
                        else element.Add(PrintVal(valKvp.Key.ToString(), valKvp.Value));
                    }
                    else element.Add(PrintVal(valKvp.Key.ToString(), value));
                }
                //writer.WriteLine("{0}</IList>", tabs);
            }
            else if (val is GomObjectData)
            {
                var valDict = (val as GomObjectData).Dictionary.Skip(3);
                element = new XElement("Class", new XAttribute("Id", verifiedKey)); //writer.WriteLine("{0}<Class Id=\"{1}\">", tabs, verifiedKey);
                foreach (var valKvp in valDict)
                {
                    var value = valKvp.Value;
                    if (value != null)
                    {
                        if (value.GetType() == typeof(string))
                        {
                            string stringValue = null;
                            if (value != null)
                            {
                                stringValue = System.Security.SecurityElement.Escape(value.ToString()).Replace("\r\n", "&#xA;");
                            }
                            element.Add(PrintVal(valKvp.Key.ToString(), stringValue));
                        }
                        else element.Add(PrintVal(valKvp.Key.ToString(), value));
                    }
                    else element.Add(PrintVal(valKvp.Key.ToString(), value));
                }
                //writer.WriteLine("{0}</Class>", tabs);
            }
            else
            {
                string stringValue = null;
                if (val != null)
                {
                    stringValue = System.Security.SecurityElement.Escape(val.ToString()).Replace("\r\n", "&#xA;");
                }
                if (stringValue == " ") stringValue = "";
                element = new XElement("Node", new XAttribute("Id", verifiedKey), stringValue); //writer.WriteLine("{0}<node Id=\"{1}\" value=\"{2}\" />", tabs, verifiedKey, stringValue);
            }
            return element;
        }

        public int FindReferences()
        {
            try
            {
                if (!IsLoaded) Load();

                var references = new List<ulong>();

                var dataDict = Data;
                if (dataDict != null)
                {
                    foreach (var kvp in dataDict.Dictionary)
                    {
                        references.AddRange(FindReferencedVal(kvp.Key, kvp.Value));
                    }
                }
                foreach (ulong refe in references.Distinct().ToList())
                {
                    if (refe != this.Id)
                    {
                        if (Dom_.DomTypeMap.ContainsKey(refe))
                        {
                            if (Dom_.DomTypeMap[refe].GetType() == typeof(GomObject))
                            {
                                GomObject gObj = Dom_.DomTypeMap[refe] as GomObject;
                                if (gObj.FullReferences == null) gObj.FullReferences = new Dictionary<ulong, string>();
                                if (!gObj.FullReferences.ContainsKey(this.Id)) gObj.FullReferences.Add(this.Id, this.Name);

                                if (this.FullReferences == null) this.FullReferences = new Dictionary<ulong, string>();
                                if (!this.FullReferences.ContainsKey(gObj.Id)) this.FullReferences.Add(gObj.Id, gObj.Name);
                            }
                        }
                    }
                }
                return references.Count;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        private List<ulong> FindReferencedVal(string key, object val)
        {
            List<ulong> references = new List<ulong>();

            if (key.StartsWith("1614") && key.Length == 20) { references.Add(Convert.ToUInt64(key)); }
            ulong unsignedLong;
            Int64.TryParse(key, out long signedLong);
            unsignedLong = Convert.ToUInt64(String.Format("{0:x8}", signedLong), 16);
            string unsignedString = unsignedLong.ToString();
            if (unsignedString.StartsWith("1614") && unsignedString.Length == 20) { references.Add(unsignedLong); }

            if (val is System.Collections.IList)
            {
                System.Collections.IList valList = val as System.Collections.IList;
                for (var i = 0; i < valList.Count; i++)
                {
                    references.AddRange(FindReferencedVal(i.ToString(), valList[i]));
                }
            }
            else if (val is IDictionary<object, object>)
            {
                var valDict = val as IDictionary<object, object>;
                foreach (var valKvp in valDict)
                {
                    references.AddRange(FindReferencedVal(valKvp.Key.ToString(), valKvp.Value));
                }
            }
            else if (val is GomObjectData)
            {
                var valDict = (val as GomObjectData).Dictionary;
                foreach (var valKvp in valDict)
                {
                    references.AddRange(FindReferencedVal(valKvp.Key.ToString(), valKvp.Value));
                }
            }
            else if (val is GomObject)
            {
                var valDict = (val as GomObject).Data.Dictionary;
                references.Add((val as GomObject).Id);
                foreach (var valKvp in valDict)
                {
                    references.AddRange(FindReferencedVal(valKvp.Key.ToString(), valKvp.Value));
                }
            }
            else
            {
                /*var type = val.GetType().ToString();
                if (type != "GomLib.DomClass"
                    && !type.Contains("Int32")
                    && !type.Contains("Int64")
                    && !type.Contains("System.Single")
                    && !type.Contains("GomLib.ScriptEnum")
                    && !type.Contains("System.Boolean")
                    && !type.Contains("System.String"))
                {
                    string breakhere = "";
                }*/
                if (val != null)
                {
                    if (val.ToString().Length == 20)
                    {
                        if (val.ToString().StartsWith("1614") && val.ToString().Length == 20) { references.Add(Convert.ToUInt64(val)); }
                        Int64.TryParse(val.ToString(), out long valParsed);
                        ulong valU = Convert.ToUInt64(String.Format("{0:x8}", valParsed), 16);
                        if (valU.ToString().StartsWith("1614"))
                        {
                            references.Add(valU);
                        }
                    }
                }
            }

            return references;
        }

        public void Load()
        {
            if (IsLoaded) { return; }
            //if (this.Name == "chrPaidPermissionDefsTablePrototype") { return; } //bandaid, need to probe this failure. Probed. Was a 0xD0 variable length int error.
            //if (IsUnloaded) { throw new InvalidOperationException("Cannot reload object once it's unloaded"); } //Fuck you yes I can reload it.

            if ((NumGlommed > 0) || (ObjectSizeInFile > 0))
            {
                byte[] buffer = GetRawUncompressedNode();

                // Load data from decompressed buffer
                using (var ms = new System.IO.MemoryStream(buffer))
                using (var br = new GomBinaryReader(ms, Dom_))
                {
                    //System.IO.File.WriteAllBytes("j://tempfile.txt", buffer);
                    ms.Position = Zeroes;
                    this.GlommedClasses = new List<DomClass>();
                    for (var glomIdx = 0; glomIdx < NumGlommed; glomIdx++)
                    {
                        var glomClassId = br.ReadUInt64();
                        var glomClass = Dom_.Get<DomClass>(glomClassId);
                        this.GlommedClasses.Add(glomClass);
                    }

                    this._data = Dom_.scriptObjectReader.ReadObject(this.DomClass, br, Dom_);
                }
                //if(this.Id == 16141050636868461855)
                //{
                //    using (var stream = System.IO.File.OpenRead("j:\\16141050636868461855.uncompressed"))
                //    using (var ms = new System.IO.MemoryStream(DataBuffer))
                //    {
                //        //using (var istream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(ms, new ICSharpCode.SharpZipLib.Zip.Compression.Deflater()))
                //        //{
                //        //    stream.CopyTo(istream);
                //            using (var output = System.IO.File.OpenWrite("j:\\16141050636868461855.compressed"))
                //            {
                //                ms.Position = 0;
                //                ms.WriteTo(output);
                //            }
                //        //}

                //    }
                //}
                //FirstBytes = buffer.Take(0xF).ToArray();
            }

            //this.DataBuffer = null; // Since we're loaded, we don't need to hold on to the compressed data anymore // Why the fuck shouldn't I unload the damn object and reclaim the memory. This idiocy prevented me from reloading it.
            IsLoaded = true;
        }

        public byte[] GetRawUncompressedNode()
        {
            byte[] buffer;
            if (IsCompressed)
            {
                int dataLen = 8 * NumGlommed + ObjectSizeInFile;
                int maxLen = dataLen + 8;
                buffer = new byte[maxLen];

                // Decompress DataBuffer
                using (var ms = new System.IO.MemoryStream(DataBuffer))
                using (var istream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(ms, new ICSharpCode.SharpZipLib.Zip.Compression.Inflater(false)))
                {
                    int readBytes = istream.Read(buffer, 0, maxLen);
                    Zeroes = readBytes - dataLen;
                    //istream.Read(buffer, 0, 0xF);
                }
            }
            else
            {
                string path = String.Format("/resources/systemgenerated/prototypes/{0}.node", this.Id);
                TorLib.File protoFile = Dom_._assets.FindFile(path);
                using (var fs = protoFile.Open())
                using (var br = new GomBinaryReader(fs, Encoding.UTF8, Dom_))
                {
                    br.ReadBytes(NodeDataOffset);
                    buffer = br.ReadBytes(ObjectSizeInFile);
                    Zeroes = 0;
                }
            }

            return buffer;
        }

        public void Unload()
        {
            this._data = null;
            this.GlommedClasses = new List<DomClass>();
            IsLoaded = false;
            SetIsUnloaded(true);
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= ClassId.GetHashCode();
            if (Name != null) hash ^= Name.GetHashCode();
            hash ^= DataLength.GetHashCode();
            hash ^= Checksum.GetHashCode();
            return hash;
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is GomObject other)) return false;

            return Equals(other);
        }

        public bool Equals(GomObject other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (Name != other.Name)
                return false;
            if (ClassId != other.ClassId)
                return false;
            if (DataLength != other.DataLength)
                return false;
            if (Checksum != 0)
                if (Checksum != other.Checksum)
                    return false;



            return true;

        }
    }
}
