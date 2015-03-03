using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using MinerLib.FileTypes;

namespace MinerLib
{
    public class AcbTools
    {
        /*static void Main(string[] args)
        {
            string directory = System.IO.Directory.GetCurrentDirectory();
            var acbFiles = System.IO.Directory.GetFiles(directory).Where(x => x.EndsWith(".acb"));
            foreach (var acbFile in acbFiles)
            {
                AcbToWem(acbFile);
            }
            Console.WriteLine("Finish converting " + acbFiles.Count() + " acb files");
        }*/

        private static void AcbToWem(TorLib.File file, string outputFolder)
        {
            string acbFile = file.FilePath.Substring(file.FilePath.LastIndexOf("/") + 1);
            string acbDirectory = outputFolder + acbFile.Substring(0, acbFile.LastIndexOf('.'));
            Directory.CreateDirectory(acbDirectory);
            using (var stream = file.OpenCopyInMemory())
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    long numFiles = reader.ReadInt64();
                    for (long i = 0; i < numFiles; i++)
                    {
                        List<byte> byteBuffer = new List<byte>();
                        byte b = reader.ReadByte();
                        // Read until we encounter a null byte
                        while (b != 0)
                        {
                            byteBuffer.Add(b);
                            b = reader.ReadByte();
                        }

                        string filename = Encoding.UTF8.GetString(byteBuffer.ToArray());
                        long fileSize = reader.ReadInt64();
                        long fileOffset = reader.ReadInt64();
                        Console.WriteLine(filename + " - size: " + fileSize + ", offset: " + fileOffset);
                        //Console.ReadLine();
                        var currentPosition = reader.BaseStream.Position;
                        //string filenameNoExt = filename.Substring(0,filename.LastIndexOf('.'));

                        OutputFile(reader, acbDirectory + "\\" + filename, fileSize, fileOffset);
                        //ConvertToOgg(outputDirectory);
                        //FixOgg(outputDirectory.Replace(".wem", ".ogg"));
                        reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
                    }
                }
        }

        public static void OutputFile(System.IO.BinaryReader reader, string filename, long fileSize, long fileOffset)
        {
            using (System.IO.BinaryWriter file = new System.IO.BinaryWriter(System.IO.File.OpenWrite(filename)))
            {
                reader.BaseStream.Seek(fileOffset, System.IO.SeekOrigin.Begin);
                var content = reader.ReadBytes((int)fileSize);
                file.Write(content);
            }
        }

        public static Stream ReturnWem(System.IO.BinaryReader reader, string filename, long fileSize, long fileOffset)
        {
            reader.BaseStream.Seek(fileOffset, System.IO.SeekOrigin.Begin);
            var content = reader.ReadBytes((int)fileSize);
            return new MemoryStream(content);
        }

        /*public static void ConvertToOgg(string filename)
        {
            System.Diagnostics.Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = "ww2ogg.exe";
            ExternalProcess.StartInfo.Arguments = filename + " --pcb packed_codebooks_aoTuV_603.bin";
            ExternalProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            ExternalProcess.Start();
            ExternalProcess.WaitForExit();

            File.Delete(filename);
        }

        public static void FixOgg(string filename)
        {
            System.Diagnostics.Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = "revorb.exe";
            ExternalProcess.StartInfo.Arguments = filename;
            ExternalProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            ExternalProcess.Start();
            ExternalProcess.WaitForExit();

            //File.Delete(filename);
        }*/
    }
}
