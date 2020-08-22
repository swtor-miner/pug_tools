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
            this.offsetAttachName = br.ReadUInt32();
            this.offsetAttachBoneName = br.ReadUInt32();
            this.attach_matrix = File_Helpers.ReadMatrix(br);

            this.attachName = File_Helpers.ReadString(br, this.offsetAttachName);
            this.boneName = File_Helpers.ReadString(br, this.offsetAttachBoneName);
        }

    }
}
