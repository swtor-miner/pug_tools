using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace GomLib
{
    public enum TypedValueType : byte
    {
        Null = 0x0,
        Int8 = 0xB0,
        Int16 = 0xB1,
        Int24 = 0xB2,
        Int32 = 0xB3,
        Int40 = 0xB4,
        Int48 = 0xB5,
        Int56 = 0xB6,
        Int64 = 0xB7,
        FieldType = 0x80,
        BizarroFieldType = 0x81,
        UnicodeString = 0x84,
        CString = 0x85,
        Array = 0x86,
        Array2 = 0x88
    }

    public class TypedValue
    {
        public object Value { get; private set; }
        public TypedValueType ValueType { get; private set; }

        public TypedValue(TypedValueType valueType)
        {
            ValueType = valueType;
        }

        public void Parse(GomBinaryReader reader)
        {
            switch (ValueType)
            {
                case TypedValueType.Null:
                    Value = null;
                    break;
                case TypedValueType.Int8:
                case TypedValueType.Int16:
                case TypedValueType.Int24:
                case TypedValueType.Int32:
                case TypedValueType.Int40:
                case TypedValueType.Int48:
                case TypedValueType.Int56:
                case TypedValueType.Int64:
                    {
                        int numBytesToRead = ((byte)ValueType) - 0xB0 + 1;
                        Value = reader.ReadVariableWidthUInt64(numBytesToRead);
                    }
                    break;
                case TypedValueType.UnicodeString:
                    {
                        int strLen = reader.ReadByte();
                        if ((strLen >= 0xB0) && (strLen <= 0xB7))
                        {
                            strLen = (int)reader.ReadVariableWidthUInt64(strLen - 0xB0 + 1);
                        }

                        byte[] charData = reader.ReadBytes(strLen * 2);
                        Value = Encoding.Unicode.GetString(charData);
                    }
                    break;
                case TypedValueType.CString:
                    {
                        int strLen = reader.ReadByte();
                        if ((strLen >= 0xB0) && (strLen <= 0xB7))
                        {
                            strLen = (int)reader.ReadVariableWidthUInt64(strLen - 0xB0 + 1);
                        }

                        byte[] charData = reader.ReadBytes(strLen);
                        Value = Encoding.UTF8.GetString(charData);
                    }
                    break;
                case TypedValueType.Array:
                case TypedValueType.Array2:
                    {
                        int arrayLen = reader.ReadByte();
                        if ((arrayLen >= 0xB0) && (arrayLen <= 0xB7))
                        {
                            arrayLen = (int)reader.ReadVariableWidthUInt64(arrayLen - 0xB0 + 1);
                        }

                        List<TypedValue> arrayValues = new List<TypedValue>(arrayLen);
                        for (var i = 0; i < arrayLen; i++)
                        {
                            var val = reader.ReadTypedValue();
                            arrayValues.Add(val);
                        }
                        Value = arrayValues;

                        // Array type 2 has a trailing byte for some reason
                        if (ValueType == TypedValueType.Array2)
                        {
                            reader.ReadByte();
                        }
                    }
                    break;
                case TypedValueType.BizarroFieldType:
                case TypedValueType.FieldType:
                    {
                        Value = reader.ReadGomType();
                    }
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unexpected TypedValue Type: {0:X}", ValueType));
            }
        }

        public ulong AsNumber()
        {
            if (ValueType == TypedValueType.Null)
            {
                return 0;
            }

            if (((byte)ValueType < 0xB0) || ((byte)ValueType > 0xB7))
            {
                throw new InvalidOperationException("Value is not a number");
            }

            return (ulong)Value;
        }

        public string AsString()
        {
            if (ValueType == TypedValueType.Null)
            {
                return string.Empty;
            }

            return Value.ToString();
        }

        public IEnumerable<TypedValue> AsEnumerable()
        {
            if ((ValueType != TypedValueType.Array) && (ValueType != TypedValueType.Array2))
            {
                throw new InvalidOperationException("Value is not enumerable");
            }

            return (IEnumerable<TypedValue>)Value;
        }

        public GomType AsGomType()
        {
            if ((ValueType != TypedValueType.FieldType) && (ValueType != TypedValueType.BizarroFieldType))
            {
                throw new InvalidOperationException("Value is not a GomType");
            }

            return (GomType)Value;
        }
    }
}
