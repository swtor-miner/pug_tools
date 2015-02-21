using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace GomLib.DomTypeLoaders
{
    class InstanceLoader : IDomTypeLoader
    {
        private static HashSet<string> outDirs = new HashSet<string>();
        private static ICSharpCode.SharpZipLib.Checksums.Adler32 adlerCalc = new ICSharpCode.SharpZipLib.Checksums.Adler32();

        public int SupportedType { get { return (int)DomTypes.Instance; } }

        public DomType Load(GomBinaryReader reader)
        {
            GomObject result = new GomObject();
            LoaderHelper.ParseShared(reader, result);

            reader.BaseStream.Position = 0x12;
            short headerEnd = reader.ReadInt16();

            reader.BaseStream.Position = 0x18;
            //result.Offset20 = reader.ReadInt32();

            result.ClassId = reader.ReadUInt64(); // Offset 0x18

            result.Offset20 = reader.ReadInt32(); // 0x20

            result.NumGlommed = reader.ReadInt16(); // 0x24
            result.Offset26 = reader.ReadInt16(); // 0x26
            result.ObjectSizeInFile = reader.ReadInt32(); // 0x28
            result.Offset2C = reader.ReadInt16(); // 0x2C
            result.Offset2E = reader.ReadInt16(); // 0x2E
            result.Offset30 = reader.ReadByte();  // 0x30
            result.Offset31 = reader.ReadByte();  // 0x31

            reader.BaseStream.Position = headerEnd;

            if (headerEnd != reader.BaseStream.Length)
            {
                //InflaterInputStream istream = new InflaterInputStream(reader.BaseStream);
                //string outFileName = result.Name;
                //outFileName = outFileName.Replace('/', '_');
                //outFileName = outFileName.Replace('.', '\\');
                //outFileName = outFileName + ".node";
                //string path = System.IO.Path.Combine("c:\\code\\swtorfiles\\prototypes\\", outFileName);
                //if (path.Contains("\\con\\"))
                //{
                //    path = path.Replace("\\con\\", "\\_con\\");
                //}
                ////path = String.Format("c:\\code\\swtorfiles\\prototypes\\{1}", result.Id, result.Name);
                //string dir = System.IO.Path.GetDirectoryName(path);

                //if (!outDirs.Contains(dir))
                //{
                //    try
                //    {
                //        System.IO.Directory.CreateDirectory(dir);
                //        outDirs.Add(dir);
                //    }
                //    catch (System.IO.IOException)
                //    {
                //        Console.WriteLine("Cannot create directory {0} for {1}", dir, path);
                //        return result;
                //    }
                //}
                
                //using (var fs = System.IO.File.Open(path, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                //{
                //    ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(istream, fs, outBuff);
                //}

                // Copy the compressed data to the instance
                int compressedLength = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                var buff = reader.ReadBytes(compressedLength);

                adlerCalc.Update(buff);
                result.Checksum = adlerCalc.Value;
                adlerCalc.Reset();

                result.IsCompressed = true;
                result.DataLength = compressedLength;
                result.DataBuffer = buff;
            }
            else
            {
                result.DataLength = 0;
            }

            // Gom.AddObject(result);

            return result;
        }
    }
}
