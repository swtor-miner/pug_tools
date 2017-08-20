using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class WonkavatorPackage: PseudoGameObject
    {
        long DisplayNameId { get; set; }

    }

    public class WonkavatorDestination
    {
        long NameId { get; set; }
        long DestinationId { get; set; }
        ulong DestinationGUID { get; set; }
    }
}
