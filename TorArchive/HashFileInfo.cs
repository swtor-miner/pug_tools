using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorLib
{
    public class HashFileInfo
    {
        public bool IsNamed;
        public string Source;
        public string FileName;
        public string Extension;
        public string Directory;
        public State FileState;
        public File file;

        public enum State
        {
            New,
            Modified,
            Unchanged
        }

        public HashFileInfo(uint ph, uint sh, File file)
        {
            if (ph == 0 && sh == 0 && file == null)
                return;
            FileInfo info = file.FileInfo;
            nsHashDictionary.HashData data = HashDictionaryInstance.Instance.dictionary.SearchHashList(ph, sh);
            this.file = file;
            this.Source = file.Archive.FileName.ToString().Split('\\').Last();
            if ((data != null) && (data.filename != ""))
            {
                this.IsNamed = true;
                this.FileName = data.filename;
                this.Extension = this.FileName.Split('.').Last();
                string[] temp = this.FileName.Split('/');
                this.Directory = String.Join("/", temp.Take(temp.Length - 1));
                this.FileName = temp.Last();

                if (info.CRC != data.crc)
                {
                    this.FileState = HashFileInfo.State.Modified;
                    HashDictionaryInstance.Instance.dictionary.UpdateCRC(info.ph, info.sh, info.CRC);
                }
                else if (info.CRC == data.crc)
                {
                    this.FileState = HashFileInfo.State.Unchanged;
                }
                //this.Archive.directories.Add(this.Directory);
            }
            else
            {
                this.IsNamed = false;
                this.Directory = "/" + this.Source;
                this.Extension = FileExtension.Instance.GuessExtension(file);

                if (data == null)
                {
                    this.FileState = HashFileInfo.State.New;
                    //this.FileName = info.Checksum + "_" + String.Format(info.FileId.ToString();
                    this.FileName = string.Format("{0:X8}", info.Checksum) + "_" + string.Format("{0:X16}", (long)info.FileId);
                    HashDictionaryInstance.Instance.dictionary.AddHash(info.ph, info.sh, "", info.CRC);
                }
                else if (info.CRC != data.crc)
                {
                    this.FileState = HashFileInfo.State.Modified;
                    HashDictionaryInstance.Instance.dictionary.UpdateCRC(info.ph, info.sh, info.CRC);
                }
                else if (info.CRC == data.crc)
                {
                    this.FileState = HashFileInfo.State.Unchanged;
                }
                if (this.FileName == null)
                {
                    this.FileName = string.Format("{0:X8}", info.Checksum) + "_" + string.Format("{0:X16}", (long)info.FileId);
                }
            }
        }
    }
}
