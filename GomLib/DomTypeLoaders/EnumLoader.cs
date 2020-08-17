using System.Collections.Generic;

namespace GomLib.DomTypeLoaders
{
    class EnumLoader : IDomTypeLoader
    {
        public int SupportedType { get { return (int)DomTypes.Enum; } }

        public DomType Load(GomBinaryReader reader)
        {
            DomEnum result = new DomEnum();
            LoaderHelper.ParseShared(reader, result);

            reader.BaseStream.Position = 0x12;
            var nameOffset = reader.ReadInt16();

            reader.BaseStream.Position = 0x18;
            var numVals = reader.ReadInt16();
            var valOffset = reader.ReadInt16();

            // Read in names
            reader.BaseStream.Position = nameOffset;
            for (var i = 0; i < numVals; i++)
            {
                var name = reader.ReadNullTerminatedString();
                result.AddName(name);
            }

            // Read in values
            reader.BaseStream.Position = valOffset;
            for (var i = 0; i < numVals; i++)
            {
                var val = reader.ReadInt16();
                result.AddValue(val);
            }

            return result;
        }
    }
}
