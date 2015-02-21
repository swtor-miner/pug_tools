using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nsHashDictionary;

namespace TorLib
{
    public class FileExtension
    {
        private static readonly FileExtension instance = new FileExtension();
        public Dictionary<string, string> file_types = new Dictionary<string, string>();
        public Dictionary<string, string> xml_types = new Dictionary<string, string>();        

        FileExtension()
        {
            this.file_types.Add("CWS", "swf");
            this.file_types.Add("CFX", "gfx");
            this.file_types.Add("PROT", "node");
            this.file_types.Add("GAWB", "gr2");
            this.file_types.Add("SCPT", "scpt");
            this.file_types.Add("FACE", "fxe");
            this.file_types.Add("PK", "zip");
            this.file_types.Add("lua", "lua");
            this.file_types.Add("DDS", "dds");
            this.file_types.Add("XSM", "xsm");
            this.file_types.Add("XAC", "xac");
            this.file_types.Add("8BPS", "8bps");
            this.file_types.Add("bdLF", "db");
            this.file_types.Add("gsLF", "geom");
            this.file_types.Add("idLF", "diffuse");
            this.file_types.Add("psLF", "specular");
            this.file_types.Add("amLF", "mask");
            this.file_types.Add("ntLF", "tint");
            this.file_types.Add("lgLF", "glow");
            this.file_types.Add("Gamebry", "nif");
            this.file_types.Add("WMPHOTO", "lmp");
            this.file_types.Add("BKHD", "bnk");
            this.file_types.Add("AMX", "amx");
            this.file_types.Add("OLCB", "clo");
            this.file_types.Add("PNG", "png");
            this.file_types.Add("; Zo", "zone.txt");
            this.file_types.Add("RIFF", "riff");
            this.file_types.Add("WAVE", "wav");
            this.file_types.Add("\0\0\0\0", "zero.txt");

            this.xml_types.Add("<Material>", "mat");
            this.xml_types.Add("<TextureObject", "tex");
            this.xml_types.Add("<manifest>", "manifest");
            this.xml_types.Add("<\0n\0o\0d\0e\0W\0C\0l\0a\0s\0s\0e\0s\0", "fxspec");
            this.xml_types.Add("<\0A\0p\0p\0e\0a\0r\0a\0n\0c\0e", "epp");
            this.xml_types.Add("<ClothData>", "clo");
            this.xml_types.Add("<v>", "not");
            this.xml_types.Add("<Rules>", "rul");
            this.xml_types.Add("<SurveyInstance>", "svy");
            this.xml_types.Add("<DataTable>", "tbl");
            this.xml_types.Add("<TextureObject xmlns", "tex");
            this.xml_types.Add("<EnvironmentMaterial", "emt");
        }      

        public static FileExtension Instance
        {
            get
            {
                return instance;
            }
        }

        public string GuessExtension(File file)
        {
            var fs = file.Open();
            var bytes = new byte[200];
            if (file.FileInfo.Checksum < bytes.Length)
                bytes = new byte[file.FileInfo.CompressedSize];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)fs.ReadByte();
            }
            string str = "";
            str = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            char[] separator = new char[] { ',' };
            int length = str.Split(separator, 10).Length;
            string str2 = Encoding.ASCII.GetString(bytes, 0, 4);
            string str3 = "txt";

            if (((bytes[0] == 0x01) && (bytes[1] == 0x00)) && (bytes[2] == 0x00))
            {
                return "stb";
            }
            if (((bytes[0] == 0x02) && (bytes[1] == 0x00)) && (bytes[2] == 0x00))
            {
                return "mph";
            }
            if (((bytes[0] == 0x21) && (bytes[1] == 0x0d)) && ((bytes[2] == 0x0a) && (bytes[3] == 0x21)))
            {
                string str5 = Encoding.ASCII.GetString(bytes, 0, 64);
                if (str5.IndexOf("Particle Specification") >= 0)
                {
                    return "prt";
                }
                else
                    return "dat";
            }
            if (((bytes[0] == 0) && (bytes[1] == 1)) && (bytes[2] == 0))
            {
                return "ttf";
            }
            if (((bytes[0] == 10) && (bytes[1] == 5)) && ((bytes[2] == 1) && (bytes[3] == 8)))
            {
                return "pcx";
            }
            if (((bytes[0] == 0x38) && (bytes[1] == 0x03)) && ((bytes[2] == 0x00) && (bytes[3] == 0x00)))
            {
                return "spt";
            }

            if (length >= 10)
            {
                str3 = "csv";
            }


            foreach (var item in file_types)
            {
                if (str2.IndexOf(item.Key) >= 0)
                {
                    if (item.Key == "RIFF")
                    {
                        if (Encoding.ASCII.GetString(bytes, 8, 4).IndexOf("WAVE") >= 0)
                        {
                            return "wav";
                        }
                    }
                    else if (item.Key == "lua")
                    {
                        if (str.IndexOf("lua") > 50)
                            continue;
                    }
                    else if (item.Key == "\0\0\0\0")
                    {
                        if (bytes[0x0b] == 0x41)
                        {
                            return "jba";
                        }
                    }
                    return item.Value;
                }
            }

            if (str2.IndexOf("<") >= 0)
            {
                string str4 = Encoding.ASCII.GetString(bytes, 0, 64);
                foreach (var item in xml_types)
                {
                    if (str4.IndexOf(item.Key) >= 0)
                        return item.Value;
                }
                return "xml";
            }

            string str6;
            if(bytes.Length < 128)
                str6 = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            else
                str6 = Encoding.ASCII.GetString(bytes, 0, 128);
            if (str6.IndexOf("[SETTINGS]") >= 0 && str6.IndexOf("gr2") >= 0)
            {
                return "dyc";
            }

            if (str.IndexOf("cnv_") >= 1 && str.IndexOf(".wem") >= 1)
            {
                return "acb";
            }

            return str3;
        }
    }
}
