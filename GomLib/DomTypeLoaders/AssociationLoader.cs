namespace GomLib.DomTypeLoaders
{
    class AssociationLoader : IDomTypeLoader
    {
        public int SupportedType { get { return (int)DomTypes.Association; } }

        public DomType Load(GomBinaryReader reader)
        {
            DomAssociation result = new DomAssociation();
            LoaderHelper.ParseShared(reader, result);
            return result;
        }
    }
}
