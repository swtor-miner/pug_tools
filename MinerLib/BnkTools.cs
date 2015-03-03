using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MinerLib
{
    public class BnkTools
    {
        // http://wiki.xentax.com/index.php?title=Wwise_SoundBank_(*.bnk) // format specification - If anyone's written an extractor based on this, I sure couldn't find it.

        public static TorLib.Assets _assets;

        BnkTools(TorLib.Assets assets)
        {
            _assets = assets;
        }

        public void ExtractTestBnk()
        {
            string bnk = "/resources/bnk2/location_act_1_ships_data.bnk"; //temporary for testing
            var file = _assets.FindFile(bnk.Replace("_data.", "_media."));
            if (file != null)
            {
                ReadBnkStream(file, "c:\\swtor\\bnk\\");
            }
        }

        private void ReadBnkStream(TorLib.File file, string outputFolder)
        {
            using (var inFile = file.Open())
            using (var outFile = System.IO.File.Open(outputFolder + file.FilePath.Substring(file.FilePath.LastIndexOf("/") + 1) , FileMode.Create, System.IO.FileAccess.Write))
            {
                var fBuffer = new byte[file.FileInfo.UncompressedSize];
                for (int i = 0; i < file.FileInfo.UncompressedSize; i++)
                {
                    fBuffer[i] = (byte)inFile.ReadByte();
                }
                var memStream = new MemoryStream(fBuffer);
                memStream.CopyTo(outFile);
            }
            using (var fileStream = file.OpenCopyInMemory())
            {
                bool foundSTID = false;
                var FR = new List<uint[]>();
                var stringIds = new Dictionary<uint, string>();
                long dataOffset = 0;
                long hircOffset = 0;
                string SI = "";
                byte[] buffer;
                var length = fileStream.Length;
                while (fileStream.Position < length)
                {
                    buffer = fileStream.ReadBuffer(8);
                    string word = System.Text.Encoding.UTF8.GetString(buffer,0,4);
                    uint size = BitConverter.ToUInt32(buffer, 4);
                    Console.WriteLine(word + ": " + size);
                    if (word == "BKHD")
                    {
                        fileStream.Seek(size, SeekOrigin.Current);
                    }
                    else if (word == "DIDX")
                    {
                        ReadDIDX(fileStream.ReadBuffer((int)size), FR, (size / 12));
                    }
                    else if (word == "STID")
                    {
                        foundSTID = true;
                        ReadSTID(fileStream.ReadBuffer((int)size), stringIds, size); /*Bioware split off the HIRC and STID sections into a separate file to make them harder to read, or because
                                                              * they ran out of address space using 32-bit uints as pointers. I think the latter happened.*/
                    }
                    else if (word == "DATA")
                    {
                        dataOffset = fileStream.Position;
                        fileStream.Seek(size, SeekOrigin.Current); //don't need to process this now.
                    }
                    else if (word == "HIRC")
                    {
                        hircOffset = fileStream.Position;
                        fileStream.Seek(size, SeekOrigin.Current); //need to add code to process this.
                    }
                }
                if (!foundSTID)
                {
                    var dataFile = _assets.FindFile(file.FilePath.Replace("_media.", "_data."));
                    using (var inFile = dataFile.Open())
                    using (var outFile = System.IO.File.Open(outputFolder + dataFile.FilePath.Substring(file.FilePath.LastIndexOf("/") + 1), FileMode.Create, System.IO.FileAccess.Write))
                    {
                        var fBuffer = new byte[dataFile.FileInfo.UncompressedSize];
                        for (int i = 0; i < dataFile.FileInfo.UncompressedSize; i++)
                        {
                            fBuffer[i] = (byte)inFile.ReadByte();
                        }
                        var memStream = new MemoryStream(fBuffer);
                        memStream.CopyTo(outFile);
                    }
                    using (var dataFileStream = dataFile.OpenCopyInMemory())
                    {
                        length = dataFileStream.Length;
                        while (dataFileStream.Position < length)
                        {
                            buffer = dataFileStream.ReadBuffer(8);
                            string word = System.Text.Encoding.UTF8.GetString(buffer, 0, 4);
                            uint size = BitConverter.ToUInt32(buffer, 4);
                            Console.WriteLine(word + ": " + size);
                            if (word == "BKHD")
                            {
                                fileStream.Seek(size, SeekOrigin.Current);
                            }
                            else if (word == "STID")
                            {
                                foundSTID = true;
                                ReadSTID(dataFileStream.ReadBuffer((int)size), stringIds, size); /*Bioware split off the HIRC and STID sections into a separate file to make them harder to read, or because
                                                              * they ran out of address space using 32-bit uints as pointers. I think the latter happened.*/
                            }
                            else if (word == "HIRC")
                            {
                                hircOffset = dataFileStream.Position;
                                dataFileStream.Seek(size, SeekOrigin.Current);
                            }
                        }
                    }
                }
                
                // extract files
                if ((dataOffset > 0) && (FR.Count() > 0)) //this is a duplication of the bnkextr code to test the code above. We can add more functions later.
                {
                    if (!System.IO.Directory.Exists(outputFolder)) { System.IO.Directory.CreateDirectory(outputFolder); }
                    for (var i = 0; i < FR.Count; i++)
                    {
                        string filename = stringIds.First().Value + '.' + FR[i][0] + ".wav";
                        
                        /*If ParamCount > 1 Then //I didn't put the other byteswap code in, but we probably should.
                         * Begin
                         * FR[I].Size:=swap32(FR[I].Size);
                         * FR[I].Offs:=swap32(FR[I].Offs);
                         * End;*/

                        fileStream.Seek(dataOffset + FR[i][1], SeekOrigin.Begin);
                        var fileContent = fileStream.ReadBuffer((int)FR[i][2]);

                        using (System.IO.BinaryWriter binWriter = new BinaryWriter(File.Open(outputFolder + filename, FileMode.Create)))
                        {
                            binWriter.Write(fileContent);
                        }
                    }
                }

            }
        }

        private void ReadSTID(byte[] p1, Dictionary<uint, string> stringIds, uint size)
        {
            MemoryStream stream = new MemoryStream(p1);
            var number1 = stream.ReadBuffer(4); // Always 1 - maybe add check?
            uint numStringIds = BitConverter.ToUInt32(stream.ReadBuffer(4), 0); // bnkextr fucks this up. This is the number of stringIds
            for (var i = 1; i <= numStringIds; i++)
            {
                uint soundBankId = BitConverter.ToUInt32(stream.ReadBuffer(4), 0);
                var nameLength = stream.ReadByte();
                var name = System.Text.Encoding.Default.GetString(stream.ReadBuffer(nameLength));
                stringIds.Add(soundBankId, name);
            }
        }

        private void ReadDIDX(byte[] p1, List<uint[]>FR, uint p2)
        {
            MemoryStream stream = new MemoryStream(p1);
            for (var i = 0; i < p2; i++)
            {
                var subArray = stream.ReadBuffer(12);
                FR.Add(new uint[] { BitConverter.ToUInt32(subArray, 0), BitConverter.ToUInt32(subArray, 4), BitConverter.ToUInt32(subArray, 8) });
            }
        }

    }

    public static class BnkToolsHelpers
    {
        #region Helpers
        public static byte[] ReadBuffer(this Stream stream, int length)
        {
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        public static T[] SubArray<T>(this T[] data, int start, int end)
        {
            int length = end - start + 1;
            T[] result = new T[length];
            Array.Copy(data, start, result, 0, length);
            return result;
        }

        public static string ToHexString(this byte[] array)
        {
            return BitConverter.ToString(array).Replace("-", "");
        }

        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
        #endregion
    }
}
