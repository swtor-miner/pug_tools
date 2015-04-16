using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class SchematicVariation: PseudoGameObject, IEquatable<SchematicVariation>
    {
        public ulong SchemId { get; set; }
        public List<ModPackage> VariationPackages { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return Equals(obj as SchematicVariation);
        }

        public bool Equals(SchematicVariation varObj)
        {
            if (varObj == null)
                return false;
            if (ReferenceEquals(this, varObj))
                return true;

            if (this.SchemId != varObj.SchemId)
                return false;
            if (this.Name != varObj.Name)
                return false;
            if (this.VariationPackages.Count != varObj.VariationPackages.Count)
                return false;
            if (!this.VariationPackages.SequenceEqual(varObj.VariationPackages))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = SchemId.GetHashCode();
            hash ^= Name.GetHashCode();
            foreach(ModPackage pack in VariationPackages)
            {
                hash ^= pack.GetHashCode();
            }

            return hash;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement baseVariationElem = new XElement("ItemVariations", new XAttribute("Id", SchemId));
            baseVariationElem.Add(new XElement("BaseName", Name),
                new XElement("VariationCount", VariationPackages.Count));

            XElement variationsElem = new XElement("Variations");
            foreach (ModPackage variation in VariationPackages)
            {
                variationsElem.Add(variation.ToXElement());
            }
            baseVariationElem.Add(variationsElem);

            return baseVariationElem;
        }
    }
}
