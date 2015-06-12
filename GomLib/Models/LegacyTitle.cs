using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class LegacyTitle : PseudoGameObject, IEquatable<LegacyTitle>
    {
        //lgcLegacyTitlesTablePrototype - lgcLegacyTitlesData - class lookup

        public long LegacyTitleId { get; set; }       //lgcLegacyTitleId - long : 4611686347671207007        
        public long LegacyTitleStringId { get; set; }  //lgcLegacyTitleString - long lookup : 4611686347671207002 - str.pc.legacytitle - 2693215077531907
        public string LegacyTitleString { get; set; }  
       
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            LegacyTitle title = obj as LegacyTitle;
            if (title == null) return false;

            return Equals(title);
        }

        public bool Equals(LegacyTitle title)
        {
            if (title == null) return false;

            if (ReferenceEquals(this, title)) return true;

            if (this.LegacyTitleId != title.LegacyTitleId)
                return false;
            if (this.LegacyTitleStringId != title.LegacyTitleStringId)
                return false;
            if (this.LegacyTitleString != title.LegacyTitleString)
                return false;            

            return true;
        }
    }
}
