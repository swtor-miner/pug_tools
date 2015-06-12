using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class PlayerTitle : PseudoGameObject, IEquatable<PlayerTitle>
    {
        //chrPlayerTitlesTablePrototype - chrPlayerTitlesMapping - class lookup

        public long TitleId { get; set; }             //lgcLegacyTitleId - long : 4611686347671207007        
        public long TitleStringId { get; set; }       //titleDetailStringID - long lookup : 4611686124430185044 - str.pc.title - 2017148570435585
        public string TitleString { get; set; }
        public Dictionary<string, string> LocalizedTitleString { get; set; }
        public ulong TitleCodexNode { get; set; }      //titleCodex - long : 4611686226457330000 - 16140932244844680403
        public bool TitleLegacyPrefix { get; set; }   //titleDetailLegacyPrefix - bool - 4611686124430185045 - true

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            PlayerTitle title = obj as PlayerTitle;
            if (title == null) return false;

            return Equals(title);
        }

        public bool Equals(PlayerTitle title)
        {
            if (title == null) return false;

            if (ReferenceEquals(this, title)) return true;

            if (this.TitleId != title.TitleId)
                return false;
            if (this.TitleStringId != title.TitleStringId)
                return false;
            if (this.TitleString != title.TitleString)
                return false;
            if (this.TitleCodexNode != title.TitleCodexNode)
                return false;
            if (this.TitleLegacyPrefix != title.TitleLegacyPrefix)
                return false;

            return true;
        }

        public override XElement ToXElement()
        {
            XElement titleContainer = null;
            if (this != null)
            {
                titleContainer = new XElement("Title", TitleString);
                titleContainer.Add(new XAttribute("Id", TitleId),
                            new XAttribute("StringId", TitleStringId),
                            new XAttribute("CodexNode", TitleCodexNode),
                            new XAttribute("LegacyPrefix", TitleLegacyPrefix));
            }
            return titleContainer;
        }
    }
}
