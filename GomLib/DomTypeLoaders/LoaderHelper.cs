using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.DomTypeLoaders
{
    static class LoaderHelper
    {
        internal static void ParseShared(GomBinaryReader reader, DomType dom)
        {
            long offset = reader.BaseStream.Position;

            //reader.BaseStream.Position = 0x8;
            //dom.Id = reader.ReadUInt64();
            reader.BaseStream.Position = 0x14;
            short nameOffset = reader.ReadInt16();
            short descOffset = reader.ReadInt16();

            reader.BaseStream.Position = nameOffset;
            dom.Name = reader.ReadNullTerminatedString();

            reader.BaseStream.Position = descOffset;
            dom.Description = reader.ReadNullTerminatedString();

            reader.BaseStream.Position = offset;
        }
    }
}
