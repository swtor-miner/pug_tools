using System;
using System.Collections.Generic;
using System.Linq;
using GomLib;

namespace PugTools
{
    class NodeListItem
    {
        public object name;
        public string displayName;
        public string type;
        public object value;
        public string displayValue;

        public List<NodeListItem> children = new List<NodeListItem>();

        // Sort of hacky to avoid changing all the existing stuff.
        public NodeListItem(object name, object value, GomType type = null)
        {
            this.name = name;

            if (name is ulong potNodeID)
            {
                DataObjectModel currentDom = DomHandler.Instance.GetCurrentDOM();
                if (currentDom.DomTypeMap.TryGetValue(potNodeID, out DomType nodeLookup))
                {
                    displayName = potNodeID.ToString() + "  (" + nodeLookup.Name.ToString() + ")";
                }
                else
                {
                    displayName = name.ToString();
                }

                NodeListItemSetup(name.ToString(), value, type);
            }
            else
            {
                displayName = name.ToString();
                NodeListItemSetup(name.ToString(), value, type);
            }
        }

        public NodeListItem(string name, object value, GomType type = null)
        {
            this.name = name;
            displayName = name;
            NodeListItemSetup(name, value, type);
        }
        private void NodeListItemSetup(string name, object value, GomType type = null)
        {
            try
            {
                this.value = value;
                // Console.WriteLine(name);
            }
            catch (Exception) { }

            if (type != null)
            {
                try
                {
                    if (type.TypeId == GomTypeId.Map)
                    {
                        this.type = type.ToString();
                        var map = (GomLib.GomTypes.Map)type;
                        var keyType = map.KeyType;
                        var valueType = map.ValueType;

                        Dictionary<object, object> objDict = (Dictionary<object, object>)this.value;
                        foreach (var item in objDict)
                        {
                            NodeListItem child = new NodeListItem(item.Key, item.Value, valueType);
                            if (displayName == "locTextRetrieverMap")
                            {
                                GomObjectData currentValue = (GomObjectData)item.Value;
                                long stringId = currentValue.ValueOrDefault<long>("strLocalizedTextRetrieverStringID", 0);
                                string stringBucket = currentValue.ValueOrDefault<string>("strLocalizedTextRetrieverBucket", null);

                                if (stringId != 0 && stringBucket != null)
                                {
                                    DataObjectModel currentDom = DomHandler.Instance.GetCurrentDOM();
                                    string currentStringValue = currentDom.stringTable.TryGetString(stringBucket, stringId);
                                    NodeListItem stringChild = new NodeListItem("String Value", currentStringValue);
                                    child.children.Add(stringChild);
                                }
                            }
                            children.Add(child);
                        }
                        displayValue = "";

                    }
                    else if (type.TypeId == GomTypeId.List)
                    {
                        this.type = type.ToString();
                        var listType = (GomLib.GomTypes.List)type;
                        var valueType = listType.ContainedType;
                        List<object> list = (List<object>)this.value;
                        int count = 0;
                        foreach (var item in list)
                        {
                            NodeListItem child = new NodeListItem(count.ToString(), item, valueType);
                            children.Add(child);
                            count++;
                        }
                        displayValue = "";
                    }
                    else if (type.TypeId == GomTypeId.EmbeddedClass)
                    {
                        this.type = type.ToString();
                        GomObjectData obj = (GomObjectData)this.value;
                        foreach (KeyValuePair<string, object> objItem in obj.Dictionary)
                        {
                            if (objItem.Key.Contains("Script_"))
                                continue;

                            DomClass classLookup = (DomClass)obj.Dictionary["Script_Type"];
                            DomField fieldLookup = classLookup.Fields.Find(x => x.Name == objItem.Key.ToString());

                            if (fieldLookup == null)
                            {
                                try
                                {
                                    if (!ulong.TryParse(objItem.Key, out ulong id))
                                    {
                                        fieldLookup = classLookup.Fields.Find(x => x.Id == id);
                                    }
                                }
                                catch (Exception) { }
                            }

                            if (fieldLookup != null)
                            {
                                NodeListItem child = new NodeListItem(objItem.Key.ToString(), objItem.Value, fieldLookup.GomType);
                                children.Add(child);
                            }
                            else
                            {
                                NodeListItem child = new NodeListItem(objItem.Key.ToString(), objItem.Value, null);
                                children.Add(child);
                            }
                        }
                    }
                    else if (type.TypeId == GomTypeId.Vec3)
                    {
                        List<float> list = (List<float>)this.value;
                        displayValue = "(" + string.Join(", ", list.ToArray()) + ")";
                        this.type = type.ToString();
                    }
                    else if (type.TypeId == GomTypeId.UInt64)
                    {
                        DataObjectModel currentDom = DomHandler.Instance.GetCurrentDOM();
                        currentDom.DomTypeMap.TryGetValue((ulong)this.value, out DomType nodeLookup);
                        if (nodeLookup != null)
                        {
                            displayValue = value.ToString() + "  (" + nodeLookup.Name.ToString() + ")";
                        }
                        else
                        {
                            displayValue = value.ToString();
                        }
                        this.type = type.ToString();
                    }
                    else
                    {
                        this.type = type.ToString();
                        if (this.value != null && displayValue == null)
                        {
                            displayValue = value.ToString();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("caught exception");
                    Console.WriteLine("exception pause here" + ex.ToString());
                }
            }
            else
            {
                object value1 = this.value;
                if (this.value is Dictionary<object, object> objDict)
                {
                    foreach (var item in objDict)
                    {
                        NodeListItem child = new NodeListItem(item.Key.ToString(), item.Value);
                        if (displayName == "locTextRetrieverMap")
                        {
                            GomObjectData currentValue = (GomObjectData)item.Value;
                            long stringId = currentValue.ValueOrDefault<long>("strLocalizedTextRetrieverStringID", 0);
                            string stringBucket = currentValue.ValueOrDefault<string>("strLocalizedTextRetrieverBucket", null);

                            if (stringId != 0 && stringBucket != null)
                            {
                                DataObjectModel currentDom = DomHandler.Instance.GetCurrentDOM();
                                string currentStringValue = currentDom.stringTable.TryGetString(stringBucket, stringId);
                                NodeListItem stringChild = new NodeListItem("String Value", currentStringValue);
                                child.children.Add(stringChild);
                            }
                        }
                        child.type = GetType(item.Key.ToString());
                        children.Add(child);
                    }
                    displayValue = "";
                }
                else if (this.value is Dictionary<string, string> stringDict)
                {
                    foreach (var item in stringDict)
                    {
                        NodeListItem child = new NodeListItem(item.Key.ToString(), item.Value)
                        {
                            type = GetType(item.Key.ToString())
                        };
                        children.Add(child);
                    }
                    displayValue = "";
                }
                else if (this.value is GomObjectData obj)
                {
                    foreach (KeyValuePair<string, object> objItem in obj.Dictionary)
                    {
                        NodeListItem child = new NodeListItem(objItem.Key.ToString(), objItem.Value)
                        {
                            type = GetType(objItem.Key.ToString())
                        };
                        children.Add(child);
                    }
                }
                else if (this.value is List<object>)
                {
                    List<object> list = (List<object>)this.value;
                    foreach (var item in list)
                    {
                        NodeListItem child = new NodeListItem("", item)
                        {
                            type = GetType(item.ToString())
                        };
                        children.Add(child);
                    }
                    displayValue = "";
                }
                else if (value1 is List<string>)
                {
                    List<string> list = (List<string>)this.value;
                    foreach (var item in list)
                    {
                        NodeListItem child = new NodeListItem("", item)
                        {
                            type = GetType(item.ToString())
                        };
                        children.Add(child);
                    }
                    if (children.Count() == 0)
                    {
                        if (this.name != null && displayValue == "")
                            displayValue = name;
                    }
                    else
                        displayValue = "";
                }
                else if (this.value is DEP_Entry entry)
                {
                    if (entry.Dependencies.Count() > 0)
                    {
                        foreach (string dependency in entry.Dependencies)
                        {
                            NodeListItem child = new NodeListItem("", dependency);
                            children.Add(child);
                        }
                    }
                }
                else
                {
                    if (this.value != null)
                    {
                        if (displayValue == null)
                            displayValue = value.ToString();
                    }
                }
            }
        }

        public static void ResetTreeListViewColumns(BrightIdeasSoftware.TreeListView tlv)
        {
            BrightIdeasSoftware.OLVColumn olvColumn1 = new BrightIdeasSoftware.OLVColumn();
            BrightIdeasSoftware.OLVColumn olvColumn2 = new BrightIdeasSoftware.OLVColumn();

            olvColumn1.AspectName = "name";
            olvColumn1.CellPadding = null;
            olvColumn1.Text = "Name";
            olvColumn1.Width = 167;

            olvColumn2.AspectName = "displayValue";
            olvColumn2.CellPadding = null;
            olvColumn2.Text = "Value";
            olvColumn2.Width = 230;

            tlv.Columns.Clear();
            tlv.Columns.Add(olvColumn1);
            tlv.Columns.Add(olvColumn2);
        }


        public string GetType(string item)
        {
            DataObjectModel currentDom = DomHandler.Instance.GetCurrentDOM();
            currentDom.nodeLookup[typeof(DomField)].TryGetValue(item.ToString(), out DomType fieldLookup);
            string type;
            if (fieldLookup != null)
            {
                DomField fieldType = (DomField)fieldLookup;
                type = fieldType.GomType.ToString();
            }
            else
            {
                type = "Unknown";
            }
            return type;
        }
    }
}
