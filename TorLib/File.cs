using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TorLib
{
    public class FileInfo : IDisposable
    {
        public ulong Offset { get; set; }
        public uint HeaderSize { get; set; }
        public uint CompressedSize { get; set; }
        public uint UncompressedSize { get; set; }
        public ulong FileId { get; set; }
        public uint PrimaryHash { get; set; }
        public uint SecondaryHash { get; set; }
        /// <summary>CRC32 checksum</summary>
        public uint Checksum { get; set; }
        public int CRC { get; set; }
        public ushort CompressionMethod { get; set; }
        public bool IsCompressed { get { return this.CompressionMethod != 0; } }
        private bool disposed = false;

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

            }
            disposed = true;
        }

        ~FileInfo()
        {
            Dispose(false);
        }


    }

    /// <summary>A file stored in a .tor archive</summary>
    public class File : IDisposable
    {
        public Archive Archive { get; set; }
        public FileInfo FileInfo { get; set; }
        public string FilePath { get; set; }
        //public string FileName { get; set; }
        //public string Extension { get; set; }
        //public string Source { get; set; }
        //public bool IsNamed { get; set; }
        //public State FileState { get; set; }
        //public string Directory { get; set; }
        //public string ParentDirectory { get; set; }
        private readonly bool disposed = false;

        public File(Archive arch, FileInfo info)
        {
            this.Archive = arch;
            this.FileInfo = info;
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
                FileInfo.Dispose();
            }
        }

        ~File()
        {
            Dispose(false);
        }

        public Stream Open()
        {
            //var archiveStream = this.Archive.OpenStreamAt(this.FileInfo.Offset);
            var archiveStream = this.Archive.OpenStreamAt((long)this.FileInfo.Offset + this.FileInfo.HeaderSize);

            if (this.FileInfo.IsCompressed)
            {
                // Wrap stream in a a sharpziplib inflater
                var inflaterStream = new InflaterInputStream(archiveStream);
                return inflaterStream;
            }
            else
            {
                return archiveStream;
            }
        }

        public Stream OpenCopyInMemory()
        {
            var fs = Open();
            var buffer = new byte[this.FileInfo.UncompressedSize];
            //This will screw up if we have a single file greater than 2.1 GB.. probably not an issue.
            fs.Read(buffer, 0, (int)this.FileInfo.UncompressedSize);
            var memStream = new MemoryStream(buffer)
            {
                //fs.CopyTo(memStream);
                Position = 0
            };
            return memStream;
        }

        public byte[] PeakBytes(int bytes)
        {
            var fs = Open();
            var buffer = new byte[bytes];
            //This will screw up if we have a single file greater than 2.1 GB.. probably not an issue.
            if (this.FileInfo.UncompressedSize < bytes)
                fs.Read(buffer, 0, (int)this.FileInfo.UncompressedSize);
            else
                fs.Read(buffer, 0, bytes);
            return buffer;
        }
    }
}
