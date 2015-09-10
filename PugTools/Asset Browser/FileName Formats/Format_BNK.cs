using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tor_tools
{
    class Format_BNK
    {
        
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;
        public int found;

        public Format_BNK(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void parseBNK(Stream fileStream, string fullFileName)
        {   
            BinaryReader br = new BinaryReader(fileStream);                                    
            FileFormat_BNK bnk = new FileFormat_BNK(br);            
            if (bnk.hirc != null)
            {
                if (bnk.hirc.numObject != 0)
                {
                    foreach (var obj in bnk.hirc.objects)
                    {
                        if (obj.type == 2)
                        {
                            if (obj.embed != 0)
                            {
                                if (obj.audio_id != 0)
                                    fileNames.Add("/resources/bnk2/streamed/" + obj.audio_id + ".wem");
                                if (obj.audio_source_id != 0)
                                    fileNames.Add("/resources/bnk2/streamed/" + obj.audio_source_id + ".wem");
                            }
                        }
                        else if (obj.type == 11)
                        {
                            if (obj.audio_id != 0)
                                fileNames.Add("/resources/bnk2/streamed/" + obj.audio_id + ".wem");
                            if (obj.audio_source_id != 0)
                                fileNames.Add("/resources/bnk2/streamed/" + obj.audio_source_id + ".wem");
                        }
                    }
                }
            }

            if (bnk.stid != null)
            {
                if (bnk.stid.numSoundBanks != 0)
                {
                    foreach (var obj in bnk.stid.soundBanks)
                    {
                        fileNames.Add("/resources/bnk2/" + obj.name + ".bnk");
                        fileNames.Add("/resources/en-us/bnk2/" + obj.name + ".bnk");
                    }
                }
            }
        }            
        

        public void WriteFile()
        {
            if (!System.IO.Directory.Exists(this.dest + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.dest + "\\File_Names");
            found = this.fileNames.Count();
            if (this.fileNames.Count > 0)
            {
                System.IO.StreamWriter outputNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_file_names.txt", false);
                foreach (string file in fileNames)
                {
                    outputNames.Write(file.Replace("\\", "/") + "\r\n");
                }
                outputNames.Close();
                fileNames.Clear();
            }

            if (this.errors.Count > 0)
            {
                System.IO.StreamWriter outputErrors = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_error_list.txt", false);
                foreach (string error in errors)
                {
                    outputErrors.Write(error + "\r\n");
                }
                outputErrors.Close();
                errors.Clear();
            }
        }
    }

    public class FileFormat_BNK
    {
        public FileFormat_BNK_BKHD bkhd;        
        public FileFormat_BNK_DIDX didx;        
        public FileFormat_BNK_DATA data;        
        public FileFormat_BNK_HIRC hirc;        
        public FileFormat_BNK_STID stid;        

        public FileFormat_BNK(BinaryReader br, bool loadWEMs = false)
        {   
            char[] section_header;

            while (br.BaseStream.Position != br.BaseStream.Length)
            {   
                section_header = br.ReadChars(4);

                string header_str = string.Join("", section_header);
                switch (header_str)
                {
                    case "BKHD":
                        this.bkhd = new FileFormat_BNK_BKHD(br);                        
                        break;
                    case "DIDX":
                        this.didx = new FileFormat_BNK_DIDX(br);                        
                        break;
                    case "DATA":
                        this.data = new FileFormat_BNK_DATA(br);                        
                        break;
                    case "HIRC":
                        this.hirc = new FileFormat_BNK_HIRC(br);                        
                        break;
                    case "STID":
                        this.stid = new FileFormat_BNK_STID(br);                        
                        break;
                    default:                        
                        uint length = br.ReadUInt32();
                        br.BaseStream.Seek(length, SeekOrigin.Current);
                        break;
                }
            }

            if (loadWEMs)
            {
                if (this.didx != null && this.data != null)
                {
                    foreach (WEM_File wem in this.didx.wems)
                    {
                        br.BaseStream.Seek(this.data.offset + 4, SeekOrigin.Begin);
                        br.BaseStream.Seek(wem.offset, SeekOrigin.Current);
                        wem.data = br.ReadBytes((int)wem.length);
                    }                    
                }
            }
        }
    }

    public class FileFormat_BNK_BKHD
    {
        public uint length;
        public uint version;
        public uint id;
        public long offset;

        public FileFormat_BNK_BKHD(BinaryReader br)
        {
            this.offset = br.BaseStream.Position;
            this.length = br.ReadUInt32();
            this.version = br.ReadUInt32();
            this.id = br.ReadUInt32();
            br.ReadUInt32();
            br.ReadUInt32();
            br.BaseStream.Seek(this.length - 0x10, SeekOrigin.Current);
        }
    }

    public class FileFormat_BNK_DIDX
    {
        public uint length;
        public long offset;
        public List<WEM_File> wems = new List<WEM_File>();

        public FileFormat_BNK_DIDX(BinaryReader br)
        {
            this.offset = br.BaseStream.Position;
            this.length = br.ReadUInt32();
            int intFileCount = (int)this.length / 12;
            for (int intCount = 0; intCount < intFileCount; intCount++)
            {
                WEM_File wem = new WEM_File(br);
                this.wems.Add(wem);
            }
        }
    }

    public class FileFormat_BNK_DATA
    {
        public uint length;
        public long offset;

        public FileFormat_BNK_DATA(BinaryReader br)
        {
            this.offset = br.BaseStream.Position;
            this.length = br.ReadUInt32();
            br.BaseStream.Seek(this.length, SeekOrigin.Current);
        }
    }

    public class FileFormat_BNK_HIRC
    {
        public uint length;
        public uint numObject;
        public long offset;
        public List<FileFormat_BNK_HIRC_Object> objects = new List<FileFormat_BNK_HIRC_Object>();

        public FileFormat_BNK_HIRC(BinaryReader br)
        {
            this.length = br.ReadUInt32();
            this.numObject = br.ReadUInt32();
            for (int intCount = 0; intCount < this.numObject; intCount++)
            {
                FileFormat_BNK_HIRC_Object obj = new FileFormat_BNK_HIRC_Object(br);
                this.objects.Add(obj);
            }
        }
    }

    public class FileFormat_BNK_HIRC_Object
    {
        //All Objects
        public byte type;
        public uint length;
        public uint id;

        //SoundFX / Music Tracks
        public uint embed;
        public uint audio_id;
        public uint audio_source_id;

        //Music Segment
        public List<uint> audioIDs;

        //Events
        public uint numEvents;
        public List<uint> eventActions;

        //Event Action        
        public uint actionObjectID;

        public FileFormat_BNK_HIRC_Object(BinaryReader br)
        {
            this.type = br.ReadByte();
            this.length = br.ReadUInt32();
            this.id = br.ReadUInt32();

            switch (this.type)
            {
                case 2:
                    br.ReadBytes(4);
                    this.embed = br.ReadUInt32();
                    this.audio_id = br.ReadUInt32();
                    this.audio_source_id = br.ReadUInt32();
                    if (this.embed == 0)
                    {
                        br.ReadUInt32(); // offset
                        br.ReadUInt32(); // length
                    }
                    br.ReadByte();
                    if (this.embed == 0)
                        br.BaseStream.Seek((this.length - 29), SeekOrigin.Current);
                    else
                        br.BaseStream.Seek((this.length - 21), SeekOrigin.Current);
                    break;                    
                case 3:
                    byte scope = br.ReadByte();
                    byte type = br.ReadByte();
                    this.actionObjectID = br.ReadUInt32();
                    br.BaseStream.Seek((this.length - 10), SeekOrigin.Current);                 

                    // Disable this for now
                    /*
                    br.ReadByte();
                    byte numParam = br.ReadByte();
                    List<byte> adtlParam = new List<byte>();
                    int numBytes = 17;
                    for (int c = 0; c < numParam; c++ )
                    {
                        adtlParam.Add(br.ReadByte());
                        numBytes++;
                    }
                    foreach(byte param in adtlParam)                    
                    {
                        if (param == 0x0E || param == 0x0F)
                        {
                            br.ReadUInt32();
                            numBytes += 4;
                        }
                        else
                        {
                            br.ReadSingle();
                            numBytes += 4;
                        }
                    }
                    br.ReadByte();
                    numBytes += 1;
                    if(type == 0x12)
                    {
                        uint state_group_id = br.ReadUInt32();
                        uint state_id = br.ReadUInt32();
                        numBytes += 8;
                    }
                    else if(type == 0x19)
                    {
                        uint switch_group_id = br.ReadUInt32();
                        uint switch_id = br.ReadUInt32();
                        numBytes += 8;
                    }*/                    
                    break;
                case 4:
                    this.numEvents = br.ReadUInt32();
                    if (numEvents > 0)
                    {
                        eventActions = new List<uint>();
                        for (int count = 0; count < this.numEvents; count++)
                        {
                            eventActions.Add(br.ReadUInt32());
                        }
                    }
                    break;
                case 10:
                    long before = br.BaseStream.Position;
                    FileFormat_BNK_HIRC_SoundStruct soundStruct = new FileFormat_BNK_HIRC_SoundStruct(br);                    
                    uint numChild = br.ReadUInt32();
                    if (numChild > 0)
                    {
                        this.audioIDs = new List<uint>();
                        for (int count = 0; count < numChild; count++)
                        {
                            this.audioIDs.Add(br.ReadUInt32());
                        } 
                    }
                    long after = br.BaseStream.Position;
                    long diff = (after - before) + 4;                    
                    br.BaseStream.Seek((this.length - diff), SeekOrigin.Current);                    
                    break;
                case 11:
                    br.ReadBytes(8);
                    br.ReadBoolean();
                    br.ReadBytes(3);
                    this.audio_source_id = br.ReadUInt32();
                    this.audio_id = br.ReadUInt32();
                    br.BaseStream.Seek((this.length - 24), SeekOrigin.Current);
                    break;
                default:
                    //Skipping other HIRC Types
                    br.BaseStream.Seek((this.length - 4), SeekOrigin.Current);
                    break;
            }
        }
    }

    public class FileFormat_BNK_HIRC_SoundStruct
    {

        public FileFormat_BNK_HIRC_SoundStruct(BinaryReader br)
        {
            //bool override
            br.ReadBoolean();       

            //number of effects
            int numEffects = br.ReadByte();

            if (numEffects > 0)
            {
                //bit mask
                br.ReadByte();


                for (int count = 0; count < numEffects; count++)
                {
                    //effect index
                    br.ReadByte();

                    //effect id
                    br.ReadUInt32();

                    //unknown
                    br.ReadBytes(2);
                }
            }
            //id of output bus
            br.ReadUInt32();            

            //id of parent object
            br.ReadUInt32();

            //override playback priority
            br.ReadBoolean();

            //offset priority
            br.ReadBoolean();

            //number of additional paramaters
            int numParam = br.ReadByte();

            if (numParam > 0)
            {
                for (int count = 0; count < numParam; count++)
                {
                    br.ReadByte();
                }

                for (int count = 0; count < numParam; count++)
                {
                    br.ReadUInt32();
                }
            }

            //unknown
            br.ReadByte();

            //positioning section included
            bool positioning = br.ReadBoolean();

            if (positioning)
            {
                //type 00 = 2d, 01 = 3d                
                byte position_type = br.ReadByte();

                if (position_type == 0)
                {
                    br.ReadBoolean();
                }
                else if (position_type == 1)
                {
                    //type of source                    
                    uint position_source = br.ReadUInt32();

                    //id of attenuation object
                    br.ReadUInt32();

                    //enable spatial
                    br.ReadBoolean();

                    if (position_source == 2)
                    {
                        br.ReadUInt32(); //play type
                        br.ReadBoolean(); //loop?
                        br.ReadUInt32(); //transition time
                        br.ReadBoolean(); //follow listener orientation
                    }
                    else if (position_source == 3)
                    {
                        br.ReadBoolean(); //update at each frame
                    }
                }
            }

            br.ReadBoolean(); //overrite game defined aux sends
            br.ReadBoolean(); //use game defined aux sends
            br.ReadBoolean(); //override user aux sends            
            bool user_def_aux_sends = br.ReadBoolean(); //use user aux sends
            if (user_def_aux_sends)
            {
                br.ReadUInt32(); //id aux bus 0
                br.ReadUInt32(); //id aux bus 1
                br.ReadUInt32(); //id aux bus 2
                br.ReadUInt32(); //id aux bus 3
            }
            
            bool unknown = br.ReadBoolean(); //unknown playback limit

            if (unknown)
            {
                br.ReadByte(); //priority equal
                br.ReadByte(); //limit reached
                br.ReadUInt16(); //limit instances
            }
            br.ReadByte(); //how limit instances
            br.ReadByte(); //virtual voice behave
            br.ReadBoolean(); //override plaback limit
            br.ReadBoolean(); //override virtual voice            
            uint state_groups = br.ReadUInt32(); //number state groups
            if (state_groups > 0)
            {
                for (int count = 0; count < state_groups; count++)
                {
                    br.ReadUInt32(); //state group id
                    br.ReadByte(); //change occurs at                                        
                    ushort custom = br.ReadUInt16();  //number of custom setting states
                    if (custom > 0)
                    {
                        for (int count2 = 0; count2 < custom; count2++)
                        {
                            br.ReadUInt32(); //id state object
                            br.ReadUInt32(); //id object contains settings
                        }
                    }
                }
            }            
            ushort rtpc = br.ReadUInt16();//number of rtpc

            if (rtpc > 0)
            {
                for (int count = 0; count < rtpc; count++)
                {

                    br.ReadUInt32(); //id of game param                    
                    br.ReadUInt32(); //y-axis type                    
                    br.ReadUInt32(); //unknown                    
                    br.ReadByte(); //unkown                                        
                    byte points = br.ReadByte(); //number of points
                    br.ReadByte(); //unknown
                    if (points > 0)
                    {
                        for (int count2 = 0; count2 < points; count2++)
                        {
                            br.ReadUInt32(); //float x                            
                            br.ReadUInt32(); //float y                            
                            br.ReadUInt32(); //share of curve
                        }
                    }
                }
            }                        
        }
    }

    public class FileFormat_BNK_STID
    {
        public uint length;
        public uint unknown;
        public uint numSoundBanks;
        public List<FileFormat_BNK_STID_SoundBank> soundBanks = new List<FileFormat_BNK_STID_SoundBank>();

        public FileFormat_BNK_STID(BinaryReader br)
        {
            this.length = br.ReadUInt32();
            this.unknown = br.ReadUInt32();
            this.numSoundBanks = br.ReadUInt32();
            for (int intCount = 0; intCount < this.numSoundBanks; intCount++)
            {
                FileFormat_BNK_STID_SoundBank obj = new FileFormat_BNK_STID_SoundBank(br);
                this.soundBanks.Add(obj);
            }
        }
    }

    public class FileFormat_BNK_STID_SoundBank
    {
        public uint id;
        public byte nameLength;
        public char[] nameTemp;
        public string name;

        public FileFormat_BNK_STID_SoundBank(BinaryReader br)
        {
            this.id = br.ReadUInt32();
            this.nameLength = br.ReadByte();
            this.nameTemp = br.ReadChars(this.nameLength);
            this.name = String.Join("", this.nameTemp);
        }
    }
}
