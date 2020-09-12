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
        private readonly object lockObject = new object();

        private readonly Dictionary<ulong, MetadataEntry> metadataLookup = new Dictionary<ulong, MetadataEntry>();
        private readonly Dictionary<ulong, string> duplicateDict = new Dictionary<ulong, string>();
        public Dictionary<int, Archive> archives = new Dictionary<int, Archive>();

        public string Name { get; private set; }
        public string Location { get; private set; }
        public bool Loaded { get; private set; }
        private bool disposed = false;

        protected bool IsPtr { get; private set; }

        public Dictionary<ulong, string> DuplicateDict => duplicateDict;

        //public Library(string name)
        //{
        //    this.Name = name;
        //    this.Location = Directory.GetCurrentDirectory();
        //}

        public Library(string name, string dir)
        {
            Name = name;
            Location = dir;

            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["usePtrData"], out bool isPtr);
            IsPtr = isPtr;
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
            if (Loaded)
            {
                return;
            }

            lock (lockObject)
            {
                if (Loaded)
                {
                    //Check again just in case.
                    return;
                }

                Archive archive = null;
                bool hasFile;
                string basePath = Location;
                for (var i = 1; ; i++)
                {
                    string filePath = null;
                    //string filePath = Path.Combine(basePath, String.Format("assets_{0}_{1}.tor", this.Name, i));
                    if (IsPtr)
                    {
                        filePath = Path.Combine(basePath, string.Format("swtor_test_{0}_{1}.tor", Name, i));
                        // Console.WriteLine("WARNING: USING PTR DATA");
                    }
                    else
                    {
                        filePath = Path.Combine(basePath, string.Format("swtor_{0}_{1}.tor", Name, i));
                    }

                    hasFile = System.IO.File.Exists(filePath);

                    if (!hasFile)
                    {
                        filePath = Path.Combine(basePath, string.Format("red_{0}.tor", Name, i));
                        hasFile = System.IO.File.Exists(filePath);
                        if (!hasFile)
                        {
                            if (archive == null)
                            {
                                // Can't find a single file for this library?! Something is quite wrong with this.
                                throw new InvalidOperationException("Cannot find any files for library named " + Name + " in " + Location);
                            }

                            // What is currently in 'archive' is the last archive in this library -- we need to get metadata from it!
                            File metadataFile = archive.FindFile(FileId.FromFilePath("metadata.bin"));
                            if (metadataFile == null)
                            {
                                throw new InvalidOperationException("Cannot Load metadata.bin for this library from " + archive.FileName);
                            }

                            LoadMetadataFromFile(metadataFile);

                            break;
                        }
                    }

                    archive = new Archive
                    {
                        Library = this,
                        FileName = filePath
                    };
                    archives[i] = archive;
                }

                Loaded = true;
                return;
            }
        }

        //public File FindFileByFqn(string fqn, string ext)
        //{
        //    // string filePath = String.Format("/server/{0}.{1}", fqn.Replace('.', '/'), ext);
        //    string filePath = String.Format("/resources/server/{0}.{1}", fqn.Replace('.', '/'), ext);
        //    return FindFile(filePath);
        //}

        public File FindFile(string path)
        {
            if (!Loaded)
            {
                Load();
            }

            // path = String.Format("/resources{0}", path.Replace('\\', '/'));

            FileId fileId = FileId.FromFilePath(path);

            if (!metadataLookup.TryGetValue(fileId.AsUInt64(), out MetadataEntry metadata))
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

                    //Considerably faster than throwing exceptions.
                    ulong fid = fileId.AsUInt64();
                    if (!metadataLookup.ContainsKey(fid))
                    {
                        metadataLookup.Add(fid, entry);
                    }
                }
            }
        }
    }
}
