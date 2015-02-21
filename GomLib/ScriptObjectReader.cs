using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib
{
    public class ScriptObjectReader
    {
        [Newtonsoft.Json.JsonIgnore]
        DataObjectModel _dom;
        
        public ScriptObjectReader(DataObjectModel dom)
        {
            _dom = dom;
        }

        public void Flush()
        {
            _dom = null;
        }

        public GomObjectData ReadObject(DomClass domClass, GomBinaryReader reader, DataObjectModel dom)
        {
            if (_dom == null) _dom = dom;
            GomObjectData result = new GomObjectData();
            IDictionary<string, object> resultDict = result.Dictionary;
            if (domClass != null)
            {
                resultDict["Script_Type"] = domClass;
            }
            else
            {
                resultDict["Script_Type"] = null;
            }

            resultDict["Script_TypeId"] = reader.ReadNumber();

            int numFields = (int)reader.ReadNumber();
            resultDict["Script_NumFields"] = numFields;

            ulong fieldId = 0;
            for (var i = 0; i < numFields; i++)
            {
                fieldId += reader.ReadNumber();
                DomField field = _dom.Get<DomField>(fieldId);
                GomType fieldType = null;
                if (field == null)
                {
                    // No idea what kind of field this is, so we'll skip it but we still need to read the data..
                    fieldType = _dom.gomTypeLoader.Load(reader, _dom, false);
                    if (fieldType == null)
                        continue;
                }
                else
                {
                    fieldType = field.GomType;

                    // Confirm the type matches
                    if (!field.ConfirmType(reader))
                    {
                        Console.WriteLine("Unexpected field type for field " + field.Name);
                        reader.BaseStream.Position = reader.BaseStream.Position - 1;
                        //throw new InvalidOperationException("Unexpected field type for field " + field.Name);
                    }
                }

                // Read in the data
                if (_dom == null)
                {
                    string pausehere = "";
                }
                object fieldValue = fieldType.ReadData(_dom, reader);

                // Save data to resulting script object
                string fieldName = null;
                if ((field != null) && (!String.IsNullOrEmpty(field.Name)))
                {
                    fieldName = field.Name;
                }
                else
                {
                    fieldName = _dom.GetStoredTypeName(fieldId);
                    if (fieldName == null)
                    {
                        if (!_dom.UnNamedMap.ContainsKey(fieldId))
                            _dom.UnNamedMap.Add(fieldId, new HashSet<string>());
                        if (domClass != null)
                            _dom.UnNamedMap[fieldId].Add(domClass.ToString());
                        //fieldName = String.Format("field_{0:X8}", fieldId);
                        fieldName = fieldId.ToString();
                    }
                }

                resultDict.Add(fieldName, fieldValue);
            }

            return result;
        }
    }
}
