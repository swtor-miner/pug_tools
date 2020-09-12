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
                resultDict.Add("Script_Type", domClass);
            }
            resultDict.Add("Script_TypeId", reader.ReadNumber());

            int numFields = (int)reader.ReadNumber();
            resultDict.Add("Script_NumFields", numFields);

            ulong fieldId = 0;
            for (var i = 0; i < numFields; i++)
            {
                fieldId += reader.ReadNumber();
                DomField field = _dom.Get<DomField>(fieldId);
                GomType fieldType;
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
                /*if (_dom == null)//obsolete debugging code
                {
                    string pausehere = "";
                }*/
                object fieldValue = fieldType.ReadData(_dom, reader);
                string domClassNullCheck = ((object)domClass ?? "Unknown").ToString();

                // Save data to resulting script object
                string fieldName;
                if ((field != null) && (!string.IsNullOrEmpty(field.Name)))
                {
                    fieldName = field.Name;
                    if (!_dom.NamedMap.ContainsKey(domClassNullCheck))
                        _dom.NamedMap.Add(domClassNullCheck, new HashSet<string>());
                    if (domClass != null)
                        _dom.NamedMap[domClassNullCheck].Add(fieldName);
                }
                else
                {
                    fieldName = _dom.GetStoredTypeName(fieldId);
                    if (fieldName == null)
                    {
                        if (!_dom.UnNamedMap.ContainsKey(domClassNullCheck))
                            _dom.UnNamedMap.Add(domClassNullCheck, new HashSet<ulong>());
                        if (domClass != null)
                            _dom.UnNamedMap[domClassNullCheck].Add(fieldId);
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
