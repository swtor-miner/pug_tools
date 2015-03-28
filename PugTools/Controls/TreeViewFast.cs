using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using tor_tools;

namespace TreeViewFast.Controls
{

    public class TreeViewFast : TreeView
    {
        private readonly Dictionary<string, TreeNode> _treeNodes = new Dictionary<string, TreeNode>();

        /// <summary>
        /// Load the TreeView with items.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="items">Collection of items</param>
        /// <param name="getId">Function to parse Id value from item object</param>
        /// <param name="getParentId">Function to parse parentId value from item object</param>
        /// <param name="getDisplayName">Function to parse display name value from item object. This is used as node text.</param>
        public void LoadItems<T>(IEnumerable<T> items, Func<T, string> getId, Func<T, string> getParentId, Func<T, string> getDisplayName)
        {
            // Clear view and internal dictionary
            Nodes.Clear();
            _treeNodes.Clear();
                       

            // Load internal dictionary with nodes
            foreach (var item in items)
            {
                var id = getId(item);
                var displayName = getDisplayName(item);
                var node = new TreeNode { Name = id.ToString(), Text = displayName, Tag = item };                
                _treeNodes.Add(getId(item), node);                
            }

            // Create hierarchy and load into view
            foreach (var id in _treeNodes.Keys)
            {
                var node = GetNode(id);
                var obj = (T)node.Tag;
                var parentId = getParentId(obj);

                if (parentId != "")
                {
                    var parentNode = GetNode(parentId);
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    Nodes.Add(node);
                }
            }
        }

        /// <summary>
        /// Get a handle to the object collection.
        /// This is convenient if you want to search the object collection.
        /// </summary>
        public IQueryable<T> GetItems<T>()
        {
            return _treeNodes.Values.Select(x => (T)x.Tag).AsQueryable();
        }

        /// <summary>
        /// Retrieve TreeNode by Id.
        /// Useful when you want to select a specific node.
        /// </summary>
        /// <param name="id">Item id</param>
        public TreeNode GetNode(string id)
        {
            return _treeNodes[id];
        }

        /// <summary>
        /// Retrieve item object by Id.
        /// Useful when you want to get hold of object for reading or further manipulating.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="id">Item id</param>
        /// <returns>Item object</returns>
        public T GetItem<T>(string id)
        {
            return (T)GetNode(id).Tag;
        }


        /// <summary>
        /// Get parent item.
        /// Will return NULL if item is at top level.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="id">Item id</param>
        /// <returns>Item object</returns>
        public T GetParent<T>(string id) where T : class
        {
            var parentNode = GetNode(id).Parent;
            return parentNode == null ? null : (T)Parent.Tag;
        }

        /// <summary>
        /// Retrieve descendants to specified item.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="id">Item id</param>
        /// <param name="deepLimit">Number of generations to traverse down. 1 means only direct children. Null means no limit.</param>
        /// <returns>List of item objects</returns>
        public List<T> GetDescendants<T>(string id, int? deepLimit = null)
        {
            var node = GetNode(id);
            var enumerator = node.Nodes.GetEnumerator();
            var items = new List<T>();

            if (deepLimit.HasValue && deepLimit.Value <= 0)
                return items;

            while (enumerator.MoveNext())
            {
                // Add child
                var childNode = (TreeNode)enumerator.Current;
                var childItem = (T)childNode.Tag;
                items.Add(childItem);

                // If requested add grandchildren recursively
                var childDeepLimit = deepLimit.HasValue ? deepLimit.Value - 1 : (int?)null;
                if (!deepLimit.HasValue || childDeepLimit > 0)
                {
                    var childId = childNode.Name.ToString();
                    var descendants = GetDescendants<T>(childId, childDeepLimit);
                    items.AddRange(descendants);
                }
            }
            return items;
        }

        internal void LoadItems<T1>(Dictionary<string, TreeListItem> testDict, Func<TreeListItem, string> getId, Func<TreeListItem, string> getParentId, Func<TreeListItem, string> getDisplayName)
        {
            // Clear view and internal dictionary
            Nodes.Clear();
            _treeNodes.Clear();

            List<string> keys = testDict.Keys.ToList();
            keys.Sort(delegate(string x, string y)
            {
                TreeListItem tx = testDict[x];
                TreeListItem ty = testDict[y];
                
                if (tx.hashInfo.file == null && ty.hashInfo.file != null)
                    return -1;
                else if (tx.hashInfo.file != null && ty.hashInfo.file == null)
                    return 1;
                else if (tx.hashInfo.file == null && ty.hashInfo.file == null)
                    return string.Compare(x, y);
                else
                    return string.Compare(x, y);
            });

            // Load internal dictionary with nodes
            foreach (var key in keys)
            {
                var item = testDict[key];
                var id = getId(item);
                var displayName = getDisplayName(item);
                var node = new TreeNode { Name = id.ToString(), Text = displayName, Tag = item };
                if (item.hashInfo.file != null)
                {
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                }
                else
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                }
                _treeNodes.Add(getId(item), node);
            }

            // Create hierarchy and load into view
            foreach (var id in _treeNodes.Keys)
            {
                var node = GetNode(id);
                var obj = (tor_tools.TreeListItem)node.Tag;
                var parentId = getParentId(obj);

                if (parentId != "")
                {
                    var parentNode = GetNode(parentId);
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    Nodes.Add(node);
                }
            }            
        }

        internal void LoadItems<T1>(Dictionary<string, tor_tools.TreeListItem> assetDict, Func<tor_tools.TreeListItem, string> getId, Func<tor_tools.TreeListItem, string> getParentId, Func<tor_tools.TreeListItem, string> getDisplayName, string filter, string type)
        {
            // Clear view and internal dictionary
            Nodes.Clear();
            _treeNodes.Clear();

            List<string> keys = assetDict.Keys.ToList();
            keys.Sort();

            // Load internal dictionary with nodes
            foreach (var key in keys)
            {
                var item = assetDict[key];
                var id = getId(item);
                var displayName = getDisplayName(item);
                var node = new TreeNode { Name = id.ToString(), Text = displayName, Tag = item };
                if (item.hashInfo.file != null)
                {
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                }
                else
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                }
                _treeNodes.Add(getId(item), node);
            }

            // Create hierarchy and load into view
            foreach (var id in _treeNodes.Keys)
            {
                var node = GetNode(id);
                var obj = (tor_tools.TreeListItem)node.Tag;
                var parentId = getParentId(obj);

                if (parentId != "")
                {
                    var parentNode = GetNode(parentId);
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    Nodes.Add(node);
                }
            }
        }

        internal void LoadItems<T1>(Dictionary<string, tor_tools.NodeAsset> assetDict, Func<tor_tools.NodeAsset, string> getId, Func<tor_tools.NodeAsset, string> getParentId, Func<tor_tools.NodeAsset, string> getDisplayName)
        {
            // Clear view and internal dictionary
            Nodes.Clear();
            _treeNodes.Clear();

            List<string> keys = assetDict.Keys.ToList();            
            keys.Sort(delegate(string x, string y)
            {
                NodeAsset tx = assetDict[x];
                NodeAsset ty = assetDict[y];
                
                if ((tx.obj == null && tx.dynObject == null && tx.objData == null) && (ty.obj != null || ty.dynObject != null || ty.objData != null))
                    return -1;
                else if ((tx.obj != null || tx.dynObject != null || tx.objData != null) && (ty.obj == null && ty.dynObject == null && ty.objData == null))
                    return 1;
                else if ((tx.obj == null && tx.dynObject == null && tx.objData == null) && (ty.obj == null && ty.dynObject == null && ty.objData == null))
                    return string.Compare(x, y);
                else
                    return string.Compare(tx.Id, ty.Id);
            });


            // Load internal dictionary with nodes
            foreach (var key in keys)
            {
                var item = assetDict[key];
                var id = getId(item);
                var displayName = getDisplayName(item);
                var node = new TreeNode { Name = id.ToString(), Text = displayName, Tag = item };
                
                if (item.obj != null || item.dynObject != null || item.objData != null)
                {
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                }
                else
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                }
                _treeNodes.Add(getId(item), node);
            }

            // Create hierarchy and load into view
            foreach (var id in _treeNodes.Keys)
            {
                var node = GetNode(id);
                var obj = (tor_tools.NodeAsset)node.Tag;
                var parentId = getParentId(obj);

                if (parentId != "")
                {
                    var parentNode = GetNode(parentId);
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    Nodes.Add(node);
                }
            }
        }

        internal void LoadItems<T1>(Dictionary<string, ArchTreeListItem> testDict, Func<ArchTreeListItem, string> getId, Func<ArchTreeListItem, string> getParentId, Func<ArchTreeListItem, string> getDisplayName)
        {
            // Clear view and internal dictionary
            Nodes.Clear();
            _treeNodes.Clear();

            List<string> keys = testDict.Keys.ToList();
            keys.Sort(delegate(string x, string y)
            {
                ArchTreeListItem tx = testDict[x];
                ArchTreeListItem ty = testDict[y];               

                return string.Compare(x, y);
            });

            // Load internal dictionary with nodes
            foreach (var key in keys)
            {
                var item = testDict[key];
                var id = getId(item);
                var displayName = getDisplayName(item);
                var node = new TreeNode { Name = id.ToString(), Text = displayName, Tag = item };
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;                
                _treeNodes.Add(getId(item), node);
            }

            // Create hierarchy and load into view
            foreach (var id in _treeNodes.Keys)
            {
                var node = GetNode(id);
                var obj = (tor_tools.ArchTreeListItem)node.Tag;
                var parentId = getParentId(obj);

                if (parentId != "")
                {
                    var parentNode = GetNode(parentId);
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    Nodes.Add(node);
                }
            }
        }
    }
}
