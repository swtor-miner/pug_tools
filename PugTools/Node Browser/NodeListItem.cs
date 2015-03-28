using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GomLib;

namespace tor_tools
{
    class NodeListItem
    {   
        public string name;
        public string type;
        public object value;
        public string displayValue;

        public List<NodeListItem> children = new List<NodeListItem>();

        public NodeListItem(string name, object value, GomType type = null)
        {
            try
            {
                this.name = name;
                this.value = value;
                //Console.WriteLine(name);
            } catch (Exception) { }            

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
                            NodeListItem child = new NodeListItem(item.Key.ToString(), item.Value, valueType);
                            if (this.name == "locTextRetrieverMap")
                            {
                                GomObjectData currentValue = (GomObjectData)item.Value;
                                long stringId = currentValue.ValueOrDefault<long>("strLocalizedTextRetrieverStringID", 0);
                                string stringBucket = currentValue.ValueOrDefault<string>("strLocalizedTextRetrieverBucket", null);

                                if (stringId != 0 && stringBucket != null)
                                {
                                    DataObjectModel currentDom = DomHandler.Instance.getCurrentDOM();
                                    string currentStringValue = currentDom.stringTable.TryGetString(stringBucket, stringId);
                                    NodeListItem stringChild = new NodeListItem("String Value", currentStringValue);
                                    child.children.Add(stringChild);
                                }
                            }                            
                            children.Add(child);
                        }
                        this.displayValue = "";

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
                        this.displayValue = "";
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
                                    ulong id;
                                    if (!ulong.TryParse(objItem.Key, out id))
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
                    else if(type.TypeId == GomTypeId.Vec3)
                    {
                        List<Single> list = (List<Single>)this.value;
                        this.displayValue = "(" + String.Join(", ", list.ToArray()) + ")"; 
                        this.type = type.ToString();
                    }
                    else if (type.TypeId == GomTypeId.UInt64)
                    {
                        DataObjectModel currentDom = DomHandler.Instance.getCurrentDOM();
                        DomType nodeLookup = null;
                        currentDom.DomTypeMap.TryGetValue((ulong)this.value, out nodeLookup);
                        if (nodeLookup != null)
                        {
                            this.displayValue = value.ToString() + "  (" + nodeLookup.Name.ToString() + ")";
                        }
                        else
                        {
                            this.displayValue = value.ToString();
                        }
                        this.type = type.ToString();
                    }
                    else
                    {
                        this.type = type.ToString();
                        if (this.value != null && displayValue == null)
                        {
                            this.displayValue = value.ToString();
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("caught exception");
                    Console.WriteLine("exception pause here" + e.ToString());
                }
            }
            else
            {
                if (this.value is Dictionary<object, object>)
                {
                    Dictionary<object, object> objDict = (Dictionary<object, object>)this.value;
                    foreach (var item in objDict)
                    {
                        NodeListItem child = new NodeListItem(item.Key.ToString(), item.Value);
                        if (this.name == "locTextRetrieverMap")
                        {
                            GomObjectData currentValue = (GomObjectData)item.Value;
                            long stringId = currentValue.ValueOrDefault<long>("strLocalizedTextRetrieverStringID", 0);
                            string stringBucket = currentValue.ValueOrDefault<string>("strLocalizedTextRetrieverBucket", null);

                            if (stringId != 0 && stringBucket != null)
                            {
                                DataObjectModel currentDom = DomHandler.Instance.getCurrentDOM();
                                string currentStringValue = currentDom.stringTable.TryGetString(stringBucket, stringId);
                                NodeListItem stringChild = new NodeListItem("String Value", currentStringValue);
                                child.children.Add(stringChild);
                            }
                        }
                        child.type = getType(item.Key.ToString());
                        children.Add(child);
                    }
                    this.displayValue = "";
                }
                else if (this.value is Dictionary<string, string>)
                {
                    Dictionary<string, string> stringDict = (Dictionary<string, string>)this.value;
                    foreach (var item in stringDict)
                    {
                        NodeListItem child = new NodeListItem(item.Key.ToString(), item.Value);
                        child.type = getType(item.Key.ToString());
                        children.Add(child);
                    }
                    this.displayValue = "";
                }
                else if (this.value is GomObjectData)
                {
                    GomObjectData obj = (GomObjectData)this.value;
                    foreach (KeyValuePair<string, object> objItem in obj.Dictionary)
                    {
                        NodeListItem child = new NodeListItem(objItem.Key.ToString(), objItem.Value);
                        child.type = getType(objItem.Key.ToString());
                        children.Add(child);
                    }                    
                }
                else if (this.value is List<object>)
                {
                    List<object> list = (List<object>)this.value;
                    foreach (var item in list)
                    {
                        NodeListItem child = new NodeListItem("", item);
                        child.type = getType(item.ToString());
                        children.Add(child);
                    }
                    this.displayValue = "";                    
                }
                else if (this.value is List<string>)
                {
                    List<string> list = (List<string>)this.value;
                    foreach (var item in list)
                    {
                        NodeListItem child = new NodeListItem("", item);
                        child.type = getType(item.ToString());
                        children.Add(child);
                    }
                    if (children.Count() == 0)
                    {
                        if (this.name != null && this.displayValue == "")
                            this.displayValue = name;
                    }
                    else
                        this.displayValue = "";                    
                }
                else if (this.value is DEP_Entry)
                {
                    DEP_Entry entry = (DEP_Entry)this.value;
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
                            this.displayValue = value.ToString();
                    }
                }
            }
         }

        public static void resetTreeListViewColumns(BrightIdeasSoftware.TreeListView tlv)
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


        public string getType(string item)
        {
            string type = "";
            DomType fieldLookup = null;
            DataObjectModel currentDom = DomHandler.Instance.getCurrentDOM();
            currentDom.nodeLookup[typeof(DomField)].TryGetValue(item.ToString(), out fieldLookup);
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
