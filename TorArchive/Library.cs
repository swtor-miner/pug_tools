using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TorLib
{
    public class MetadataEntry : IDisposable
    {
        /// <summary>FileId this metadata entry points to</summary>
        public FileId FileId { get; set; }

        /// <summary>Which archive this file is located in</summary>
        public byte Archive { get; set; }

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

        ~MetadataEntry()
        {
            Dispose(false);
        }

    }

    /// <summary>Manages multiple Archive files together as a single library</summary>
    public class Library : IDisposable
    {
        // public static Library Current { get; private set; }

        private Dictionary<ulong, MetadataEntry> metadataLookup = new Dictionary<ulong, MetadataEntry>();
        public Dictionary<int, Archive> archives = new Dictionary<int, Archive>();        

        public string Name { get; private set; }
        public string Location { get; private set; }
        public bool Loaded { get; private set; }
        private bool disposed = false;

        protected bool IsPtr { get; private set; }

        //public Library(string name)
        //{
        //    this.Name = name;
        //    this.Location = Directory.GetCurrentDirectory();
        //}

        public Library(string name, string dir)
        {
            this.Name = name;
            this.Location = dir;

            bool isPtr = false;
            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["usePtrData"], out isPtr);
            this.IsPtr = isPtr;
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
                foreach (var meta in metadataLookup)
                {
                    meta.Value.Dispose();
                }                
                metadataLookup.Clear();
                foreach (var arch in archives)
                {
                    arch.Value.Dispose();
                }
                archives.Clear();                
            }
            disposed = true;
            Loaded = false;
        }

        ~Library()
        {
            Dispose(false);
        }

        //public static Library Open(string name, string dir = null)
        //{
        //    if (dir == null)
        //    {
        //        dir = Directory.GetCurrentDirectory();
        //    }

        //    Current = new Library(name, dir);
        //    Current.Load();
        //    return Current;
        //}

        public void Load()
        {
            if (this.Loaded) { return; }

            Archive archive = null;
            bool hasFile;
            string basePath = this.Location;
            for (var i = 1;; i++) {
                string filePath = null;
                //string filePath = Path.Combine(basePath, String.Format("assets_{0}_{1}.tor", this.Name, i));
                if (IsPtr)
                {
                    filePath = Path.Combine(basePath, String.Format("swtor_test_{0}_{1}.tor", this.Name, i));
                    // Console.WriteLine("WARNING: USING PTR DATA");
                }
                else
                {
                    filePath = Path.Combine(basePath, String.Format("swtor_{0}_{1}.tor", this.Name, i));
                }

                hasFile = System.IO.File.Exists(filePath);

                if (!hasFile)
                {
                    if (archive == null)
                    {
                        // Can't find a single file for this library?! Something is quite wrong with this.
                        throw new InvalidOperationException("Cannot find any files for library named " + this.Name + " in " + this.Location);
                    }

                    // What is currently in 'archive' is the last archive in this library -- we need to get metadata from it!
                    File metadataFile = archive.FindFile(FileId.FromFilePath("metadata.bin"));
                    if (metadataFile == null)
                    {
                        throw new InvalidOperationException("Cannot Load metadata.bin for this library from " + archive.FileName);
                    }

                    this.LoadMetadataFromFile(metadataFile);

                    break;
                }

                archive = new Archive();
                archive.Library = this;
                archive.FileName = filePath;
                archives[i] = archive;
            }

            this.Loaded = true;
            return;
        }       

        //public File FindFileByFqn(string fqn, string ext)
        //{
        //    // string filePath = String.Format("/server/{0}.{1}", fqn.Replace('.', '/'), ext);
        //    string filePath = String.Format("/resources/server/{0}.{1}", fqn.Replace('.', '/'), ext);
        //    return FindFile(filePath);
        //}

        public File FindFile(string path)
        {
            if (!this.Loaded) { this.Load(); }

            // path = String.Format("/resources{0}", path.Replace('\\', '/'));

            FileId fileId = FileId.FromFilePath(path);
            MetadataEntry metadata;

            if (!metadataLookup.TryGetValue(fileId.AsUInt64(), out metadata))
            {
                return null;
            }

            Archive archive = archives[metadata.Archive];

            File result = archive.FindFile(fileId);
            if (result != null)
            {
                result.FilePath = path;
            }

            return result;
        }

        private void LoadMetadataFromFile(File metadataFile)
        {
            uint numFiles = metadataFile.FileInfo.UncompressedSize / 32;

            using (var stream = metadataFile.Open())
            {
                var reader = new BinaryReader(stream);

                for (var i = 0; i < numFiles; i++)
                {
                    reader.ReadBytes(16); // Unknown usage.. CRC of some type perhaps?
                    uint ph = reader.ReadUInt32();
                    uint sh = reader.ReadUInt32();
                    byte fileNum = reader.ReadByte();
                    reader.ReadBytes(7); // Unknown usage

                    FileId fileId = new FileId() { ph = ph, sh = sh };
                    MetadataEntry entry = new MetadataEntry()
                    {
                        FileId = fileId,
                        Archive = fileNum
                    };

                    metadataLookup.Add(fileId.AsUInt64(), entry);
                }
            }
        }
    }
}
