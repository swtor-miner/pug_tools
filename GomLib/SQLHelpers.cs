using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace GomLib
{
    public static class SQLHelpers
    {
        public static List<string> GetPropertyValues(object obj, List<SQLProperty> properties)
        {
            List<string> results = new List<string>();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            foreach (var property in properties)
            {
                var x = obj.GetType().GetProperty(property.PropertyName, System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty);
                if (x == null)
                    
                    continue;
                if (property.JsonSerialize)
                {
                    results.Add(sqlSani(JsonConvert.SerializeObject(x.GetValue(obj, null), settings)));
                }
                else
                {
                    var val = x.GetValue(obj, null) ?? "";
                    var valType = val.GetType().ToString();
                    switch (valType)
                    {
                        case "System.Boolean": //this is convert the Boolean values to 0/1 instead of true/false string
                            var intval = Convert.ToInt32(val);
                            results.Add(sqlSani(intval.ToString()));
                            break;
                        default:
                            results.Add(sqlSani(val.ToString()).ToString());
                            break;
                    }
                }
            }
            return results;
        }

        public static string ToSQL(object obj, SQLData sql, string patchVersion)
        {
            string s = "', '";
            string value = String.Format("('{0}', '', '{0}', '{1}', '{2}')", sqlSani(patchVersion), String.Join(s, GetPropertyValues(obj, sql.SQLProperties)), obj.GetHashCode());
            return value;
        }

        public static string sqlSani(string str)
        {
            if (str == null) return "";
            return MySql.Data.MySqlClient.MySqlHelper.EscapeString(str);
        }
        public static Dictionary<string, string> SimpleTagLocalizedDict(this Dictionary<string, string> dict)
        {
            if (dict == null) return new Dictionary<string, string>();
            Dictionary<string, string> returnVal = new Dictionary<string,string>();
            foreach (var val in dict)
            {
                switch (val.Key)
                {
                    case "enMale": returnVal.Add("1", val.Value); break;
                    //case "enFemale": returnVal.Add("2", val.Value); break; //this is never used.
                    case "frMale": returnVal.Add("3", val.Value); break;
                    case "frFemale": returnVal.Add("4", val.Value); break;
                    case "deMale": returnVal.Add("5", val.Value); break;
                    case "deFemale": returnVal.Add("6", val.Value); break;
                }
            }
            return returnVal;
        }
    }

    public class SQLData
    {
        public SQLData() { }

        public SQLData(List<SQLProperty> sqlProp)
        {
            SQLProperties = sqlProp;
        }

        public List<SQLProperty> SQLProperties { get; set; }

        public static List<SQLProperty> BaseProperties = new List<SQLProperty>
        {
            new SQLProperty("GetHashCode()", "Hash", "int(11) NOT NULL")
        };
    }

    public class SQLProperty
    {
        public SQLProperty(string name, string propName, string type)
        {
            SetValues(name, propName, type);
            IsPrimaryKey = false;
        }

        public SQLProperty(string name, string propName, string type, bool isPriKey)
        {
            SetValues(name, propName, type);
            IsPrimaryKey = isPriKey;
        }

        public SQLProperty(string name, string propName, string type, bool isPriKey, bool jSerialize)
        {
            SetValues(name, propName, type);
            IsPrimaryKey = isPriKey;
            JsonSerialize = jSerialize;
        }
        public SQLProperty(string name, string propName, string type, params SQLPropSetting[] settings)
        {
            SetValues(name, propName, type);
            IsPrimaryKey = settings.Contains(SQLPropSetting.PrimaryKey);
            JsonSerialize = settings.Contains(SQLPropSetting.JsonSerialize);
            AddIndex = settings.Contains(SQLPropSetting.AddIndex);
        }

        public string Name;
        public string PropertyName;
        public string Type;
        public bool IsPrimaryKey = false;
        public bool JsonSerialize = false;
        public bool AddIndex = false;

        internal void SetValues(string name, string propName, string type)
        {
            Name = name;
            PropertyName = propName;
            Type = type;
        }
    }
    public enum SQLPropSetting
    {
        PrimaryKey,
        JsonSerialize,
        AddIndex
    }
    #region Base62
    public static class EncodingExtensions
    {
        private static string Base62CodingSpace = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Convert a the last 5 bytes of a ulong value to a Base62 string
        /// </summary>
        /// <param name="original">ulong</param>
        /// <returns>Base62 string</returns>
        public static string ToMaskedBase62(this ulong id)
        {
            ulong maskedId = (ulong)(id & 0xFFFFFFFFFF);
            List<byte> maskedBytes = BitConverter.GetBytes(maskedId).ToList();
            maskedBytes.RemoveRange(5, 3);
            return maskedBytes.ToArray().ToBase62();
        }

        /// <summary>
        /// Convert a the last 5 bytes of a ulong value to a Base62 string
        /// </summary>
        /// <param name="original">ulong</param>
        /// <returns>Base62 string</returns>
        public static string ToMaskedBase62(this long id)
        {
            //long maskedId = (long)(id & 0xFFFFFFFFFF);
            //List<byte> maskedBytes = BitConverter.GetBytes(maskedId).ToList();
            //maskedBytes.RemoveRange(5, 3);
            //return maskedBytes.ToArray().ToBase62();
            return ((ulong)id).ToMaskedBase62();
        }

        /// <summary>
        /// Convert a byte array
        /// </summary>
        /// <param name="original">Byte array</param>
        /// <returns>Base62 string</returns>
        public static string ToBase62(this byte[] original)
        {
            StringBuilder sb = new StringBuilder();
            BitStream stream = new BitStream(original);         // Set up the BitStream
            byte[] read = new byte[1];                          // Only read 6-bit at a time
            while (true)
            {
                read[0] = 0;
                int length = stream.Read(read, 0, 6);           // Try to read 6 bits
                if (length == 6)                                // Not reaching the end
                {
                    if ((int)(read[0] >> 3) == 0x1f)            // First 5-bit is 11111
                    {
                        sb.Append(Base62CodingSpace[61]);
                        stream.Seek(-1, SeekOrigin.Current);    // Leave the 6th bit to next group
                    }
                    else if ((int)(read[0] >> 3) == 0x1e)       // First 5-bit is 11110
                    {
                        sb.Append(Base62CodingSpace[60]);
                        stream.Seek(-1, SeekOrigin.Current);
                    }
                    else                                        // Encode 6-bit
                    {
                        sb.Append(Base62CodingSpace[(int)(read[0] >> 2)]);
                    }
                }
                else if (length == 0)                           // Reached the end completely
                {
                    break;
                }
                else                                            // Reached the end with some bits left
                {
                    // Padding 0s to make the last bits to 6 bit
                    sb.Append(Base62CodingSpace[(int)(read[0] >> (int)(8 - length))]);
                    break;
                }
            }
            if (sb.Length > 7) { return sb.ToString().Substring(0, 7); } //didn't realize that some of these were longer than 7
            return sb.ToString();
        }

        /// <summary>
        /// Convert a Base62 string to byte array
        /// </summary>
        /// <param name="base62">Base62 string</param>
        /// <returns>Byte array</returns>
        public static byte[] FromBase62(this string base62)
        {
            // Character count
            int count = 0;

            // Set up the BitStream
            BitStream stream = new BitStream(base62.Length * 6 / 8);

            foreach (char c in base62)
            {
                // Look up coding table
                int index = Base62CodingSpace.IndexOf(c);

                // If end is reached
                if (count == base62.Length - 1)
                {
                    // Check if the ending is good
                    int mod = (int)(stream.Position % 8);
                    if (mod == 0)
                        throw new InvalidDataException("an extra character was found");

                    if ((index >> (8 - mod)) > 0)
                        throw new InvalidDataException("invalid ending character was found");

                    stream.Write(new byte[] { (byte)(index << mod) }, 0, 8 - mod);
                }
                else
                {
                    // If 60 or 61 then only write 5 bits to the stream, otherwise 6 bits.
                    if (index == 60)
                    {
                        stream.Write(new byte[] { 0xf0 }, 0, 5);
                    }
                    else if (index == 61)
                    {
                        stream.Write(new byte[] { 0xf8 }, 0, 5);
                    }
                    else
                    {
                        stream.Write(new byte[] { (byte)index }, 2, 6);
                    }
                }
                count++;
            }

            // Dump out the bytes
            byte[] result = new byte[stream.Position / 8];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(result, 0, result.Length * 8);
            return result;
        }
    }

    /// <summary>
    /// Utility that read and write bits in byte array
    /// </summary>
    public class BitStream : Stream
    {
        private byte[] Source { get; set; }

        /// <summary>
        /// Initialize the stream with capacity
        /// </summary>
        /// <param name="capacity">Capacity of the stream</param>
        public BitStream(int capacity)
        {
            this.Source = new byte[capacity];
        }

        /// <summary>
        /// Initialize the stream with a source byte array
        /// </summary>
        /// <param name="source"></param>
        public BitStream(byte[] source)
        {
            this.Source = source;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Bit length of the stream
        /// </summary>
        public override long Length
        {
            get { return Source.Length * 8; }
        }

        /// <summary>
        /// Bit position of the stream
        /// </summary>
        public override long Position { get; set; }

        /// <summary>
        /// Read the stream to the buffer
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset bit start position of the stream</param>
        /// <param name="count">Number of bits to read</param>
        /// <returns>Number of bits read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // Temporary position cursor
            long tempPos = this.Position;
            tempPos += offset;

            // Buffer byte position and in-byte position
            int readPosCount = 0, readPosMod = 0;

            // Stream byte position and in-byte position
            long posCount = tempPos >> 3;
            int posMod = (int)(tempPos - ((tempPos >> 3) << 3));

            while (tempPos < this.Position + offset + count && tempPos < this.Length)
            {
                // Copy the bit from the stream to buffer
                if ((((int)this.Source[posCount]) & (0x1 << (7 - posMod))) != 0)
                {
                    buffer[readPosCount] = (byte)((int)(buffer[readPosCount]) | (0x1 << (7 - readPosMod)));
                }
                else
                {
                    buffer[readPosCount] = (byte)((int)(buffer[readPosCount]) & (0xffffffff - (0x1 << (7 - readPosMod))));
                }

                // Increment position cursors
                tempPos++;
                if (posMod == 7)
                {
                    posMod = 0;
                    posCount++;
                }
                else
                {
                    posMod++;
                }
                if (readPosMod == 7)
                {
                    readPosMod = 0;
                    readPosCount++;
                }
                else
                {
                    readPosMod++;
                }
            }
            int bits = (int)(tempPos - this.Position - offset);
            this.Position = tempPos;
            return bits;
        }

        /// <summary>
        /// Set up the stream position
        /// </summary>
        /// <param name="offset">Position</param>
        /// <param name="origin">Position origin</param>
        /// <returns>Position after setup</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case (SeekOrigin.Begin):
                    {
                        this.Position = offset;
                        break;
                    }
                case (SeekOrigin.Current):
                    {
                        this.Position += offset;
                        break;
                    }
                case (SeekOrigin.End):
                    {
                        this.Position = this.Length + offset;
                        break;
                    }
            }
            return this.Position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write from buffer to the stream
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset">Offset start bit position of buffer</param>
        /// <param name="count">Number of bits</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            // Temporary position cursor
            long tempPos = this.Position;

            // Buffer byte position and in-byte position
            int readPosCount = offset >> 3, readPosMod = offset - ((offset >> 3) << 3);

            // Stream byte position and in-byte position
            long posCount = tempPos >> 3;
            int posMod = (int)(tempPos - ((tempPos >> 3) << 3));

            while (tempPos < this.Position + count && tempPos < this.Length)
            {
                // Copy the bit from buffer to the stream
                if ((((int)buffer[readPosCount]) & (0x1 << (7 - readPosMod))) != 0)
                {
                    this.Source[posCount] = (byte)((int)(this.Source[posCount]) | (0x1 << (7 - posMod)));
                }
                else
                {
                    this.Source[posCount] = (byte)((int)(this.Source[posCount]) & (0xffffffff - (0x1 << (7 - posMod))));
                }

                // Increment position cursors
                tempPos++;
                if (posMod == 7)
                {
                    posMod = 0;
                    posCount++;
                }
                else
                {
                    posMod++;
                }
                if (readPosMod == 7)
                {
                    readPosMod = 0;
                    readPosCount++;
                }
                else
                {
                    readPosMod++;
                }
            }
            this.Position = tempPos;
        }
    }
    #endregion
}
