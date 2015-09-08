using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace tor_tools
{
    public class View_BNK
    {
        public List<WEM_File> wems = new List<WEM_File>();
        public List<FileFormat_BNK_STID_SoundBank> soundBanks = new List<FileFormat_BNK_STID_SoundBank>();

        public View_BNK(BinaryReader br)
        {
            FileFormat_BNK bnk = new FileFormat_BNK(br, true);
            if (bnk.didx != null && bnk.didx.wems.Count() > 0)
            {
                this.wems = bnk.didx.wems;
            }
            if(bnk.stid != null && bnk.stid.soundBanks.Count > 0)
            {
                soundBanks = bnk.stid.soundBanks;
            }
        }
    }
}
