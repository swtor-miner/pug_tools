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
        public File File
        {
            get
            {
                if (_FileRef != null && _FileRef.TryGetTarget(out File tmpObj))
                {
                    return tmpObj;
                }

                return null;
            }

            set
            {
                _FileRef = new WeakReference<File>(value);
            }
        }

        private WeakReference<File> _FileRef;

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

            nsHashDictionary.HashData data = HashDictionaryInstance.Instance.dictionary.SearchHashList(ph, sh, file.Archive.StrippedFileName);
            _FileRef = new WeakReference<File>(file);
            Source = file.Archive.FileName.ToString().Split('\\').Last();
            if (data != null && data.filename.Length > 0)
            {
                IsNamed = true;
                FileName = data.filename;
                Extension = FileName.Split('.').Last();
                string[] temp = FileName.Split('/');
                Directory = string.Join("/", temp.Take(temp.Length - 1));
                FileName = temp.Last();

                if (info.CRC != data.crc)
                {
                    FileState = State.Modified;
                    HashDictionaryInstance.Instance.dictionary.UpdateCRC(info.PrimaryHash, info.SecondaryHash, info.CRC, file.Archive.StrippedFileName);
                }
                else if (info.CRC == data.crc)
                {
                    FileState = State.Unchanged;
                }
                //this.Archive.directories.Add(this.Directory);
            }
            else
            {
                IsNamed = false;
                Directory = "/" + Source;
                Extension = FileExtension.Instance.GuessExtension(file);

                if (data == null)
                {
                    FileState = State.New;
                    //this.FileName = info.Checksum + "_" + String.Format(info.FileId.ToString();
                    FileName = string.Format("{0:X8}", info.Checksum) + "_" + string.Format("{0:X16}", info.FileId);
                    HashDictionaryInstance.Instance.dictionary.AddHash(info.PrimaryHash, info.SecondaryHash, "", info.CRC, file.Archive.StrippedFileName);
                }
                else if (info.CRC != data.crc)
                {
                    FileState = State.Modified;
                    HashDictionaryInstance.Instance.dictionary.UpdateCRC(info.PrimaryHash, info.SecondaryHash, info.CRC, file.Archive.StrippedFileName);
                }
                else if (info.CRC == data.crc)
                {
                    FileState = State.Unchanged;
                }
                if (FileName == null)
                {
                    FileName = string.Format("{0:X8}", info.Checksum) + "_" + string.Format("{0:X16}", info.FileId);
                }
            }
        }
    }
}
