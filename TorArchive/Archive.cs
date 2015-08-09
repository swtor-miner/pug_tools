using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TorLib
{
    /// <summary>
    /// Class used to manage a single .tor file
    /// </summary>
    public class Archive : IDisposable
    {
        private Dictionary<ulong, FileInfo> fileLookup = new Dictionary<ulong, FileInfo>();
        public List<File> files = new List<File>();
        //public HashSet<string> directories = new HashSet<string>();

        internal Library Library { get; set; }
        public string FileName { get; set; }
        internal string _StrippedFileName { get; set; }
        public bool Initialized { get; private set; }
        private bool disposed = false;

        /// <summary>
        /// Gets the archive name, minus the "swtor_" or "swtor_test_" and .tor part of the name.
        /// </summary>
        public string StrippedFileName {
            get
            {
                if(_StrippedFileName == null && FileName != null)
                {
                    StrippedFileName = FileName;
                }

                return _StrippedFileName;
            }

            set
            {
                //Remove the directory.
                string fileName = value.Split('/').Last();
                fileName = fileName.Split('\\').Last();

                //Remove swtor_test_
                fileName = fileName.Replace("swtor_", string.Empty);
                fileName = fileName.Replace("test_", string.Empty);

                //Remove .tor
                fileName = fileName.Replace(".tor", string.Empty);

                _StrippedFileName = fileName;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                foreach (var lookup in fileLookup)
                {
                    lookup.Value.Dispose();
                }
                fileLookup.Clear();
                foreach (var file in files)
                {
                    file.Dispose();
                }
                files.Clear();                
                //directories.Clear();
                //Library.Dispose();                
            }
            disposed = true;
        }

        public File FindFile(FileId fileId)
        {
            return FindFile(fileId.AsUInt64());
        }

        public File FindFile(ulong fileId)
        {
            if (!Initialized) { this.Initialize(); }

            FileInfo fileInfo;

            if (!fileLookup.TryGetValue(fileId, out fileInfo))
            {
                return null;
            }

            File file = new File(this, fileInfo);           
            return file;
        }

        internal FileStream OpenStreamAt(long offset)
        {
            var fs = System.IO.File.Open(this.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            fs.Seek(offset, SeekOrigin.Begin);
            return fs;
        }

        internal FileStream OpenStream(FileInfo fileInfo)
        {
            var offset = fileInfo.Offset + fileInfo.HeaderSize;
            return OpenStreamAt((long)offset);
        }

        /// <summary>Load file tables and fill-in fileLookup dictionary</summary>
        private void Initialize()
        {
            this.Initialized = true;

            using (var fs = this.OpenStreamAt(0))
            using (var reader = new BinaryReader(fs))
            {
                int magicNumber = reader.ReadInt32();
                if (magicNumber != 0x50594D) {
                    throw new InvalidOperationException("Wait a minute! " + this.FileName + " isn't a MYP file!");
                }

                fs.Seek(12, SeekOrigin.Begin);
                ulong fileTableOffset = reader.ReadUInt64();
                while (fileTableOffset != 0)
                {
                    fs.Seek((long)fileTableOffset, SeekOrigin.Begin);

                    uint numFiles = reader.ReadUInt32();
                    fileTableOffset = reader.ReadUInt64();

                    for (var i = 0; i < numFiles; i++)
                    {
                        // Read file info blocks
                        FileInfo info = new FileInfo();
                        info.Offset = reader.ReadUInt64();
                        if (info.Offset == 0)
                        {
                            // No file offset, no file -- skip this entry and try the next one
                            fs.Seek(26, SeekOrigin.Current);
                            continue;
                        }


                        info.HeaderSize = reader.ReadUInt32();
                        info.CompressedSize = reader.ReadUInt32();
                        info.UncompressedSize = reader.ReadUInt32();
                        long current_position = reader.BaseStream.Position;
                        info.sh = reader.ReadUInt32();
                        info.ph = reader.ReadUInt32();
                        reader.BaseStream.Seek(current_position, SeekOrigin.Begin);
                        info.FileId = reader.ReadUInt64();
                        info.Checksum = reader.ReadUInt32();
                        info.CompressionMethod = reader.ReadUInt16();
                        info.CRC = (int)info.Checksum;

                        this.files.Add(new File(this, info));
                        this.fileLookup.Add(info.FileId, info);
                    }
                }
            }
        }
    }
}
