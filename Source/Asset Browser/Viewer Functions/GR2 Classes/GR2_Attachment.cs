using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileFormats
{
    public class GR2_Attachment
    {
        public uint offsetAttachName = 0;
        public uint offsetAttachBoneName = 0;

        public string attachName = "";
        public string boneName = "";

        public SlimDX.Matrix attach_matrix;

        public GR2_Attachment(BinaryReader br)
        {
            offsetAttachName = br.ReadUInt32();
            offsetAttachBoneName = br.ReadUInt32();
            attach_matrix = File_Helpers.ReadMatrix(br);

            attachName = File_Helpers.ReadString(br, offsetAttachName);
            boneName = File_Helpers.ReadString(br, offsetAttachBoneName);
        }

    }
}
