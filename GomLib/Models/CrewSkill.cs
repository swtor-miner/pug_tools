using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class CrewSkill
    {
        public Dictionary<Profession, CrewSkill> fqnMap;
        public StringTable nameTable;

        [Newtonsoft.Json.JsonIgnore]
        DataObjectModel _dom;

        public CrewSkill(DataObjectModel dom)
        {
            _dom = dom;
            fqnMap = new Dictionary<Profession, CrewSkill>();
            nameTable = _dom.stringTable.Find("str.prf.professions");
        }

        public Profession Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public ProfessionCategory SchematicType { get; set; }

        //public CrewSkill FromProfession(Profession prf)
        //{
        //    CrewSkill result;
        //    if (!fqnMap.TryGetValue(prf, out result))
        //    {
        //        var row = TorLib.Tables.ProfessionInfo.FindRowBySpec(prf);
        //        if (row == null) { return null; }

        //        result = FromRow(row);
        //    }

        //    return result;
        //}

        //public CrewSkill FromRow(TorLib.Tables.ProfessionInfo.Row row)
        //{
        //    if (fqnMap.ContainsKey(row.profession)) { return fqnMap[row.profession]; }

        //    CrewSkill result = new CrewSkill();
        //    result.Id = row.profession;
        //    result.Icon = row.icon;
        //    Icons.Add(result.Icon);
        //    result.Name = nameTable.GetText(row.name_id, row.profession.ToString());
        //    result.SchematicType = row.category.ToProfessionCategory();

        //    fqnMap.Add(result.Id, result);
        //    return result;
        //}
    }
}
