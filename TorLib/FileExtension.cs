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
            file_types.Add("CWS", "swf");
            file_types.Add("CFX", "gfx");
            file_types.Add("PROT", "node");
            file_types.Add("GAWB", "gr2");
            file_types.Add("SCPT", "scpt");
            file_types.Add("FACE", "fxe");
            file_types.Add("PK", "zip");
            file_types.Add("lua", "lua");
            file_types.Add("DDS", "dds");
            file_types.Add("XSM", "xsm");
            file_types.Add("XAC", "xac");
            file_types.Add("8BPS", "8bps");
            file_types.Add("bdLF", "db");
            file_types.Add("gsLF", "geom");
            file_types.Add("idLF", "diffuse");
            file_types.Add("psLF", "specular");
            file_types.Add("amLF", "mask");
            file_types.Add("ntLF", "tint");
            file_types.Add("lgLF", "glow");
            file_types.Add("Gamebry", "nif");
            file_types.Add("WMPHOTO", "lmp");
            file_types.Add("BKHD", "bnk");
            file_types.Add("AMX", "amx");
            file_types.Add("OLCB", "clo");
            file_types.Add("PNG", "png");
            file_types.Add("; Zo", "zone.txt");
            file_types.Add("RIFF", "riff");
            file_types.Add("WAVE", "wav");
            file_types.Add("\0\0\0\0", "zero.txt");

            xml_types.Add("<Material>", "mat");
            xml_types.Add("<TextureObject", "tex");
            xml_types.Add("<manifest>", "manifest");
            xml_types.Add("<\0n\0o\0d\0e\0W\0C\0l\0a\0s\0s\0e\0s\0", "fxspec");
            xml_types.Add("<\0A\0p\0p\0e\0a\0r\0a\0n\0c\0e", "epp");
            xml_types.Add("<ClothData>", "clo");
            xml_types.Add("<v>", "not");
            xml_types.Add("<Rules>", "rul");
            xml_types.Add("<SurveyInstance>", "svy");
            xml_types.Add("<DataTable>", "tbl");
            xml_types.Add("<TextureObject xmlns", "tex");
            xml_types.Add("<EnvironmentMaterial", "emt");
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
            fs.Read(bytes, 0, bytes.Length);
            fs.Dispose();

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
            if (((bytes[0] == 0x18) && (bytes[1] == 0x00)) && ((bytes[2] == 0x00) && (bytes[3] == 0x00)))
            {
                string strCheckDAT = Encoding.ASCII.GetString(bytes, 4, 22);
                if(strCheckDAT == "AREA_DAT_BINARY_FORMAT" || strCheckDAT == "ROOM_DAT_BINARY_FORMAT")
                    return "dat";
            }

            string str = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            string str2 = Encoding.ASCII.GetString(bytes, 0, 4);
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

            int length = str.Split(new char[] { ',' }, 10).Length;
            if (length >= 10)
            {
                return "csv";
            }
            else
            {
                return "txt";
            }
        }
    }
}
