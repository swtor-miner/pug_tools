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
        public List<WEM_File> wems;

        public List<WEM_File> parseBNK(BinaryReader br)
        {
            FileFormat_BNK bnk = new FileFormat_BNK(br, true);
            if (bnk.didx.wems.Count() > 0)
            {
                this.wems = bnk.didx.wems;
            }
            return wems;
        }
    }
}
