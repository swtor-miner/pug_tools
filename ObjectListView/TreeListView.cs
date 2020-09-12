/*
 * TreeListView - A listview that can show a tree of objects in a column
 *
 * Author: Phillip Piper
 * Date: 23/09/2008 11:15 AM
 *
 * Change log:
 * 2014-02-23  JPP  - Added Reveal() method to show a deeply nested models.
 * 2014-02-05  JPP  - Fix bug where refreshing a non-root item would collapse all expanded children of that item
 * 2014-02-01  JPP  - ClearObjects() now actually, you know, clears objects :)
 *                  - Corrected bug where Expanded event was being raised twice.
 *                  - RebuildChildren() no longer checks if CanExpand is true before rebuilding.
 * 2014-01-16  JPP  - Corrected an off-by-1 error in hit detection, which meant that clicking in the last 16 pixels
 *                    of an items label was being ignored.
 * 2013-11-20  JPP  - Moved event triggers into Collapse() and Expand() so that the events are always triggered.
 *                  - CheckedObjects now includes objects that are in a branch that is currently collapsed
 *                  - CollapseAll() and ExpandAll() now trigger cancellable events
 * 2013-09-29  JPP  - Added TreeFactory to allow the underlying Tree to be replaced by another implementation.
 * 2013-09-23  JPP  - Fixed long standing bug where RefreshObject() would not work on root objects
 *                    which overrode Equals()/GetHashCode().
 * 2013-02-23  JPP  - Added HierarchicalCheckboxes. When this is true, the checkedness of a parent
 *                    is an synopsis of the checkedness of its children. When all children are checked,
 *                    the parent is checked. When all children are unchecked, the parent is unchecked.
 *                    If some children are checked and some are not, the parent is indeterminate.
 * v2.6
 * 2012-10-25  JPP  - Circumvent annoying bug in ListView control where changing
 *                    selection would leave artefacts on the control.
 * 2012-08-10  JPP  - Don't trigger selection changed events during expands
 * 
 * v2.5.1
 * 2012-04-30  JPP  - Fixed bug where CheckedObjects would return model objects that had been filtered out.
 *                  - Allow any column to render the tree, not just column 0 (still not sure about this one)
 * v2.5.0
 * 2011-04-20  JPP  - Added ExpandedObjects property and RebuildAll() method.
 * 2011-04-09  JPP  - Added Expanding, Collapsing, Expanded and Collapsed events.
 *                    The ..ing events are cancellable. These are only fired in response
 *                    to user actions.
 * v2.4.1
 * 2010-06-15  JPP  - Fixed bug in Tree.RemoveObjects() which resulted in removed objects
 *                    being reported as still existing.
 * v2.3
 * 2009-09-01  JPP  - Fixed off-by-one error that was messing up hit detection
 * 2009-08-27  JPP  - Fixed bug when dragging a node from one place to another in the tree
 * v2.2.1
 * 2009-07-14  JPP  - Clicks to the left of the expander in tree cells are now ignored.
 * v2.2
 * 2009-05-12  JPP  - Added tree traverse operations: GetParent and GetChildren.
 *                  - Added DiscardAllState() to completely reset the TreeListView.
 * 2009-05-10  JPP  - Removed all unsafe code
 * 2009-05-09  JPP  - Fixed bug where any command (Expand/Collapse/Refresh) on a model
 *                    object that was once visible but that is currently in a collapsed branch
 *                    would cause the control to crash.
 * 2009-05-07  JPP  - Fixed bug where RefreshObjects() would fail when none of the given
 *                    objects were present/visible.
 * 2009-04-20  JPP  - Fixed bug where calling Expand() on an already expanded branch confused
 *                    the display of the children (SF#2499313)
 * 2009-03-06  JPP  - Calculate edit rectangle on column 0 more accurately
 * v2.1
 * 2009-02-24  JPP  - All commands now work when the list is empty (SF #2631054)
 *                  - TreeListViews can now be printed with ListViewPrinter
 * 2009-01-27  JPP  - Changed to use new Renderer and HitTest scheme
 * 2009-01-22  JPP  - Added RevealAfterExpand property. If this is true (the default),
 *                    after expanding a branch, the control scrolls to reveal as much of the
 *                    expanded branch as possible.
 * 2009-01-13  JPP  - Changed TreeRenderer to work with visual styles are disabled
 * v2.0.1
 * 2009-01-07  JPP  - Made all public and protected methods virtual 
 *                  - Changed some classes from 'internal' to 'protected' so that they
 *                    can be accessed by subclasses of TreeListView.
 * 2008-12-22  JPP  - Added UseWaitCursorWhenExpanding property
 *                  - Made TreeRenderer public so that it can be subclassed
 *                  - Added LinePen property to TreeRenderer to allow the connection drawing 
 *                    pen to be changed 
 *                  - Fixed some rendering issues where the text highlight rect was miscalculated
 *                  - Fixed connection line problem when there is only a single root
 * v2.0
 * 2008-12-10  JPP  - Expand/collapse with mouse now works when there is no SmallImageList.
 * 2008-12-01  JPP  - Search-by-typing now works.
 * 2008-11-26  JPP  - Corrected calculation of expand/collapse icon (SF#2338819)
 *                  - Fixed ugliness with dotted lines in renderer (SF#2332889)
 *                  - Fixed problem with custom selection colors (SF#2338805)
 * 2008-11-19  JPP  - Expand/collapse now preserve the selection -- more or less :)
 *                  - Overrode RefreshObjects() to rebuild the given objects and their children
 * 2008-11-05  JPP  - Added ExpandAll() and CollapseAll() commands
 *                  - CanExpand is no longer cached
 *                  - Renamed InitialBranches to RootModels since it deals with model objects
 * 2008-09-23  JPP  Initial version
 *
 * TO DO:
 * 
 * Copyright (C) 2006-2013 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip_piper@bigfoot.com.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A TreeListView combines an expandable tree structure with list view columns.
    /// </summary>
    /// <remarks>
    /// <para>To support tree operations, two delegates must be provided:</para>
    /// <list type="table">
    /// <item>
    /// <term>
    /// CanExpandGetter
    /// </term> 
    /// <description>
    /// This delegate must accept a model object and return a boolean indicating
    /// if that model should be expandable. 
    /// </description>
    /// </item>
    /// <item>
    /// <term>
    /// ChildrenGetter
    /// </term> 
    /// <description>
    /// This delegate must accept a model object and return an IEnumerable of model
    /// objects that will be displayed as children of the parent model. This delegate will only be called
    /// for a model object if the CanExpandGetter has already returned true for that model.
    /// </description>
    /// </item>
    /// <item>
    /// <term>
    /// ParentGetter
    /// </term> 
    /// <description>
    /// This delegate must accept a model object and return the parent model. 
    /// This delegate will only be called when HierarchicalCheckboxes is true OR when Reveal() is called. 
    /// </description>
    /// </item>
    /// </list>
    /// <para>
    /// The top level branches of the tree are set via the Roots property. SetObjects(), AddObjects() 
    /// and RemoveObjects() are interpreted as operations on this collection of roots.
    /// </para>
    /// <para>
    /// To add new children to an existing branch, make changes to your model objects and then
    /// call RefreshObject() on the parent.
    /// </para>
    /// <para>The tree must be a directed acyclic graph -- no cycles are allowed. Put more mundanely, 
    /// each model object must appear only once in the tree. If the same model object appears in two
    /// places in the tree, the control will become confused.</para>
    /// </remarks>
    public partial class TreeListView : VirtualObjectListView
    {
        /// <summary>
        /// Make a default TreeListView
        /// </summary>
        public TreeListView()
        {
            OwnerDraw = true;
            View = View.Details;
            CheckedObjectsMustStillExistInList = false;

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            RegenerateTree();
            TreeColumnRenderer = new TreeRenderer();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            // This improves hit detection even if we don't have any state image
            SmallImageList = new ImageList();
        }

        //------------------------------------------------------------------------------------------
        // Properties

        /// <summary>
        /// This is the delegate that will be used to decide if a model object can be expanded.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual CanExpandGetterDelegate CanExpandGetter
        {
            get { return TreeModel.CanExpandGetter; }
            set { TreeModel.CanExpandGetter = value; }
        }

        /// <summary>
        /// Gets whether or not this listview is capabale of showing groups
        /// </summary>
        [Browsable(false)]
        public override bool CanShowGroups
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// This is the delegate that will be used to fetch the children of a model object
        /// </summary>
        /// <remarks>This delegate will only be called if the CanExpand delegate has 
        /// returned true for the model object.</remarks>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ChildrenGetterDelegate ChildrenGetter
        {
            get { return TreeModel.ChildrenGetter; }
            set { TreeModel.ChildrenGetter = value; }
        }

        /// <summary>
        /// This is the delegate that will be used to fetch the parent of a model object
        /// </summary>
        /// <returns>The parent of the given model, or null if the model doesn't exist or 
        /// if the model is a root</returns>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ParentGetterDelegate ParentGetter
        {
            get { return parentGetter; }
            set { parentGetter = value; }
        }
        private ParentGetterDelegate parentGetter;

        /// <summary>
        /// Get or set the collection of model objects that are checked.
        /// When setting this property, any row whose model object isn't
        /// in the given collection will be unchecked. Setting to null is
        /// equivilent to unchecking all.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property returns a simple collection. Changes made to the returned
        /// collection do NOT affect the list. This is different to the behaviour of
        /// CheckedIndicies collection.
        /// </para>
        /// <para>
        /// When getting CheckedObjects, the performance of this method is O(n) where n is the number of checked objects.
        /// When setting CheckedObjects, the performance of this method is O(n) where n is the number of checked objects plus
        /// the number of objects to be checked.
        /// </para>
        /// <para>
        /// If the ListView is not currently showing CheckBoxes, this property does nothing. It does
        /// not remember any check box settings made.
        /// </para>
        /// </remarks>
        public override IList CheckedObjects
        {
            get
            {
                return base.CheckedObjects;
            }
            set
            {
                ArrayList objectsToRecalculate = new ArrayList(CheckedObjects);
                if (value != null)
                    objectsToRecalculate.AddRange(value);

                base.CheckedObjects = value;

                if (HierarchicalCheckboxes)
                    RecalculateHierarchicalCheckBoxGraph(objectsToRecalculate);
            }
        }

        /// <summary>
        /// Gets or sets the model objects that are expanded.
        /// </summary>
        /// <remarks>
        /// <para>This can be used to expand model objects before they are seen.</para>
        /// <para>
        /// Setting this does *not* force the control to rebuild
        /// its display. You need to call RebuildAll(true).
        /// </para>
        /// </remarks>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable ExpandedObjects
        {
            get
            {
                return TreeModel.mapObjectToExpanded.Keys;
            }
            set
            {
                TreeModel.mapObjectToExpanded.Clear();
                if (value != null)
                {
                    foreach (object x in value)
                        TreeModel.SetModelExpanded(x, true);
                }
            }
        }

        /// <summary>
        /// Gets or  sets the filter that is applied to our whole list of objects.
        /// TreeListViews do not currently support whole list filters.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IListFilter ListFilter
        {
            get { return null; }
            set
            {
                Debug.Assert(value == null, "TreeListView do not support ListFilters");
            }
        }

        /// <summary>
        /// Gets or sets whether this tree list view will display hierarchical checkboxes.
        /// Hierarchical checkboxes is when a parent's "checkedness" is calculated from
        /// the "checkedness" of its children. If all children are checked, the parent
        /// will be checked. If all children are unchecked, the parent will also be unchecked.
        /// If some children are checked and others are not, the parent will be indeterminate.
        /// </summary>
        [Category("ObjectListView"),
         Description("Show hierarchical checkboxes be enabled?"),
         DefaultValue(false)]
        public virtual bool HierarchicalCheckboxes
        {
            get { return hierarchicalCheckboxes; }
            set
            {
                if (hierarchicalCheckboxes == value)
                    return;

                hierarchicalCheckboxes = value;
                CheckBoxes = value;
                if (value)
                    TriStateCheckBoxes = false;
            }
        }
        private bool hierarchicalCheckboxes;

        /// <summary>
        /// Gets or sets the collection of root objects of the tree
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IEnumerable Objects
        {
            get { return Roots; }
            set { Roots = value; }
        }

        /// <summary>
        /// Gets the collection of objects that will be considered when creating clusters
        /// (which are used to generate Excel-like column filters)
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IEnumerable ObjectsForClustering
        {
            get
            {
                for (int i = 0; i < TreeModel.GetObjectCount(); i++)
                    yield return TreeModel.GetNthObject(i);
            }
        }

        /// <summary>
        /// After expanding a branch, should the TreeListView attempts to show as much of the 
        /// revealed descendents as possible.
        /// </summary>
        [Category("ObjectListView"),
         Description("Should the parent of an expand subtree be scrolled to the top revealing the children?"),
         DefaultValue(true)]
        public bool RevealAfterExpand
        {
            get { return revealAfterExpand; }
            set { revealAfterExpand = value; }
        }
        private bool revealAfterExpand = true;

        /// <summary>
        /// The model objects that form the top level branches of the tree.
        /// </summary>
        /// <remarks>Setting this does <b>NOT</b> reset the state of the control.
        /// In particular, it does not collapse branches.</remarks>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual IEnumerable Roots
        {
            get { return TreeModel.RootObjects; }
            set
            {
                TreeColumnRenderer = TreeColumnRenderer;
                TreeModel.RootObjects = value ?? new ArrayList();
                UpdateVirtualListSize();
            }
        }

        /// <summary>
        /// Make sure that at least one column is displaying a tree. 
        /// If no columns is showing the tree, make column 0 do it.
        /// </summary>
        protected virtual void EnsureTreeRendererPresent(TreeRenderer renderer)
        {
            if (Columns.Count == 0)
                return;

            foreach (OLVColumn col in Columns)
            {
                if (col.Renderer is TreeRenderer)
                {
                    col.Renderer = renderer;
                    return;
                }
            }

            // No column held a tree renderer, so give column 0 one
            OLVColumn columnZero = GetColumn(0);
            columnZero.Renderer = renderer;
            columnZero.WordWrap = columnZero.WordWrap;
        }

        /// <summary>
        /// Gets or sets the renderer that will be used to draw the tree structure.
        /// Setting this to null resets the renderer to default.
        /// </summary>
        /// <remarks>If a column is currently rendering the tree, the renderer
        /// for that column will be replaced. If no column is rendering the tree,
        /// column 0 will be given this renderer.</remarks>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual TreeRenderer TreeColumnRenderer
        {
            get { return treeRenderer ?? (treeRenderer = new TreeRenderer()); }
            set
            {
                treeRenderer = value ?? new TreeRenderer();
                EnsureTreeRendererPresent(treeRenderer);
            }
        }
        private TreeRenderer treeRenderer;

        /// <summary>
        /// This is the delegate that will be used to create the underlying Tree structure
        /// that the TreeListView uses to manage the information about the tree.
        /// </summary>
        /// <remarks>
        /// <para>The factory must not return null. </para>
        /// <para>
        /// Most users of TreeListView will never have to use this delegate.
        /// </para>
        /// </remarks>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeFactoryDelegate TreeFactory
        {
            get { return treeFactory; }
            set { treeFactory = value; }
        }
        private TreeFactoryDelegate treeFactory;

        /// <summary>
        /// Should a wait cursor be shown when a branch is being expanded?
        /// </summary>
        /// <remarks>When this is true, the wait cursor will be shown whilst the children of the 
        /// branch are being fetched. If the children of the branch have already been cached, 
        /// the cursor will not change.</remarks>
        [Category("ObjectListView"),
        Description("Should a wait cursor be shown when a branch is being expaned?"),
        DefaultValue(true)]
        public virtual bool UseWaitCursorWhenExpanding
        {
            get { return useWaitCursorWhenExpanding; }
            set { useWaitCursorWhenExpanding = value; }
        }
        private bool useWaitCursorWhenExpanding = true;

        /// <summary>
        /// Gets the model that is used to manage the tree structure
        /// </summary>
        /// <remarks>
        /// Don't mess with this property unless you really know what you are doing.
        /// If you don't already know what it's for, you don't need it.</remarks>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Tree TreeModel
        {
            get { return treeModel; }
            protected set { treeModel = value; }
        }
        private Tree treeModel;

        //------------------------------------------------------------------------------------------
        // Accessing

        /// <summary>
        /// Return true if the branch at the given model is expanded
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool IsExpanded(object model)
        {
            Branch br = TreeModel.GetBranch(model);
            return (br != null && br.IsExpanded);
        }

        //------------------------------------------------------------------------------------------
        // Commands

        /// <summary>
        /// Collapse the subtree underneath the given model
        /// </summary>
        /// <param name="model"></param>
        public virtual void Collapse(object model)
        {
            if (GetItemCount() == 0)
                return;

            OLVListItem item = ModelToItem(model);
            TreeBranchCollapsingEventArgs args = new TreeBranchCollapsingEventArgs(model, item);
            OnCollapsing(args);
            if (args.Canceled)
                return;

            IList selection = SelectedObjects;
            int index = TreeModel.Collapse(model);
            if (index >= 0)
            {
                UpdateVirtualListSize();
                SelectedObjects = selection;
                if (index < GetItemCount())
                    RedrawItems(index, GetItemCount() - 1, false);
                OnCollapsed(new TreeBranchCollapsedEventArgs(model, item));
            }
        }

        /// <summary>
        /// Collapse all subtrees within this control
        /// </summary>
        public virtual void CollapseAll()
        {
            if (GetItemCount() == 0)
                return;

            TreeBranchCollapsingEventArgs args = new TreeBranchCollapsingEventArgs(null, null);
            OnCollapsing(args);
            if (args.Canceled)
                return;

            IList selection = SelectedObjects;
            int index = TreeModel.CollapseAll();
            if (index >= 0)
            {
                UpdateVirtualListSize();
                SelectedObjects = selection;
                RedrawItems(index, GetItemCount() - 1, false);
                OnCollapsed(new TreeBranchCollapsedEventArgs(null, null));
            }
        }

        /// <summary>
        /// Remove all items from this list
        /// </summary>
        /// <remark>This method can safely be called from background threads.</remark>
        public override void ClearObjects()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(ClearObjects));
            else
            {
                Roots = null;
                DiscardAllState();
            }
        }

        /// <summary>
        /// Collapse all roots and forget everything we know about all models
        /// </summary>
        public virtual void DiscardAllState()
        {
            CheckStateMap.Clear();
            RebuildAll(false);
        }

        /// <summary>
        /// Expand the subtree underneath the given model object
        /// </summary>
        /// <param name="model"></param>
        public virtual void Expand(object model)
        {
            if (GetItemCount() == 0)
                return;

            // Give the world a chance to cancel the expansion
            OLVListItem item = ModelToItem(model);
            TreeBranchExpandingEventArgs args = new TreeBranchExpandingEventArgs(model, item);
            OnExpanding(args);
            if (args.Canceled)
                return;

            // Remember the selection so we can put it back later
            IList selection = SelectedObjects;

            // Expand the model first
            int index = TreeModel.Expand(model);
            if (index < 0)
                return;

            // Update the size of the list and restore the selection
            UpdateVirtualListSize();
            using (SuspendSelectionEventsDuring())
                SelectedObjects = selection;

            // Redraw the items that were changed by the expand operation
            RedrawItems(index, GetItemCount() - 1, false);

            OnExpanded(new TreeBranchExpandedEventArgs(model, item));

            if (RevealAfterExpand && index > 0)
            {
                // TODO: This should be a separate method
                BeginUpdate();
                try
                {
                    int countPerPage = NativeMethods.GetCountPerPage(this);
                    int descedentCount = TreeModel.GetVisibleDescendentCount(model);
                    // If all of the descendents can be shown in the window, make sure that last one is visible.
                    // If all the descendents can't fit into the window, move the model to the top of the window
                    // (which will show as many of the descendents as possible)
                    if (descedentCount < countPerPage)
                    {
                        EnsureVisible(index + descedentCount);
                    }
                    else
                    {
                        TopItemIndex = index;
                    }
                }
                finally
                {
                    EndUpdate();
                }
            }
        }

        /// <summary>
        /// Expand all the branches within this tree recursively.
        /// </summary>
        /// <remarks>Be careful: this method could take a long time for large trees.</remarks>
        public virtual void ExpandAll()
        {
            if (GetItemCount() == 0)
                return;

            // Give the world a chance to cancel the expansion
            TreeBranchExpandingEventArgs args = new TreeBranchExpandingEventArgs(null, null);
            OnExpanding(args);
            if (args.Canceled)
                return;

            IList selection = SelectedObjects;
            int index = TreeModel.ExpandAll();
            if (index < 0)
                return;

            UpdateVirtualListSize();
            using (SuspendSelectionEventsDuring())
                SelectedObjects = selection;
            RedrawItems(index, GetItemCount() - 1, false);
            OnExpanded(new TreeBranchExpandedEventArgs(null, null));
        }

        /// <summary>
        /// Completely rebuild the tree structure
        /// </summary>
        /// <param name="preserveState">If true, the control will try to preserve selection and expansion</param>
        public virtual void RebuildAll(bool preserveState)
        {
            int previousTopItemIndex = preserveState ? TopItemIndex : -1;

            RebuildAll(
                preserveState ? SelectedObjects : null,
                preserveState ? ExpandedObjects : null,
                preserveState ? CheckedObjects : null);

            if (preserveState)
                TopItemIndex = previousTopItemIndex;
        }

        /// <summary>
        /// Completely rebuild the tree structure
        /// </summary>
        /// <param name="selected">If not null, this list of objects will be selected after the tree is rebuilt</param>
        /// <param name="expanded">If not null, this collection of objects will be expanded after the tree is rebuilt</param>
        /// <param name="checkedObjects">If not null, this collection of objects will be checked after the tree is rebuilt</param>
        protected virtual void RebuildAll(IList selected, IEnumerable expanded, IList checkedObjects)
        {
            // Remember the bits of info we don't want to forget (anyone ever see Memento?)
            IEnumerable roots = Roots;
            CanExpandGetterDelegate canExpand = CanExpandGetter;
            ChildrenGetterDelegate childrenGetter = ChildrenGetter;

            try
            {
                BeginUpdate();

                // Give ourselves a new data structure
                RegenerateTree();

                // Put back the bits we didn't want to forget
                CanExpandGetter = canExpand;
                ChildrenGetter = childrenGetter;
                if (expanded != null)
                    ExpandedObjects = expanded;
                Roots = roots;
                if (selected != null)
                    SelectedObjects = selected;
                if (checkedObjects != null)
                    CheckedObjects = checkedObjects;
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Unroll all the ancestors of the given model and make sure it is then visible.
        /// </summary>
        /// <remarks>This works best when a ParentGetter is installed.</remarks>
        /// <param name="modelToReveal">The object to be revealed</param>
        /// <param name="selectAfterReveal">If true, the model will be selected and focused after being revealed</param>
        /// <returns>True if the object was found and revealed. False if it was not found.</returns>
        public virtual void Reveal(object modelToReveal, bool selectAfterReveal)
        {
            // Collect all the ancestors of the model
            ArrayList ancestors = new ArrayList();
            foreach (object ancestor in GetAncestors(modelToReveal))
                ancestors.Add(ancestor);

            // Arrange them from root down to the model's immediate parent
            ancestors.Reverse();
            try
            {
                BeginUpdate();
                foreach (object ancestor in ancestors)
                    Expand(ancestor);
                EnsureModelVisible(modelToReveal);
                if (selectAfterReveal)
                    SelectObject(modelToReveal, true);
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Update the rows that are showing the given objects
        /// </summary>
        public override void RefreshObjects(IList modelObjects)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { RefreshObjects(modelObjects); });
                return;
            }
            // There is no point in refreshing anything if the list is empty
            if (GetItemCount() == 0)
                return;

            // Remember the selection so we can put it back later
            IList selection = SelectedObjects;

            // We actually need to refresh the parents.
            // Refreshes on root objects have to be handled differently
            ArrayList updatedRoots = new ArrayList();
            Hashtable modelsAndParents = new Hashtable();
            foreach (object model in modelObjects)
            {
                if (model == null)
                    continue;
                object parent = GetParent(model);
                if (parent == null)
                {
                    updatedRoots.Add(model);
                }
                else
                {
                    modelsAndParents[model] = true;
                    modelsAndParents[parent] = true;
                }
            }

            // Updates to root level objects are treated like updates on a normal OLV
            if (updatedRoots.Count > 0)
                base.RefreshObjects(updatedRoots);

            // Refresh each object, remembering where the first update occured
            int firstChange = int.MaxValue;
            foreach (object model in modelsAndParents.Keys)
            {
                if (model != null)
                {
                    int index = TreeModel.RebuildChildren(model);
                    if (index >= 0)
                        firstChange = Math.Min(firstChange, index);
                }
            }

            // If we didn't refresh any objects, don't do anything else
            if (firstChange >= GetItemCount())
                return;

            ClearCachedInfo();
            UpdateVirtualListSize();
            SelectedObjects = selection;

            // Redraw everything from the first update to the end of the list
            RedrawItems(firstChange, GetItemCount() - 1, false);
        }

        /// <summary>
        /// Change the check state of the given object to be the given state.
        /// </summary>
        /// <remarks>
        /// If the given model object isn't in the list, we still try to remember
        /// its state, in case it is referenced in the future.</remarks>
        /// <param name="modelObject"></param>
        /// <param name="state"></param>
        /// <returns>True if the checkedness of the model changed</returns>
        protected override bool SetObjectCheckedness(object modelObject, CheckState state)
        {
            // If the checkedness of the given model changes AND this tree has 
            // hierarchical checkboxes, then we need to update the checkedness of 
            // its children, and recalculate the checkedness of the parent (recursively)
            if (!base.SetObjectCheckedness(modelObject, state))
                return false;

            if (!HierarchicalCheckboxes)
                return true;

            // Give each child the same checkedness as the model

            CheckState? checkedness = GetCheckState(modelObject);
            if (!checkedness.HasValue || checkedness.Value == CheckState.Indeterminate)
                return true;

            foreach (object child in GetChildrenWithoutExpanding(modelObject))
            {
                SetObjectCheckedness(child, checkedness.Value);
            }

            ArrayList args = new ArrayList
            {
                modelObject
            };
            RecalculateHierarchicalCheckBoxGraph(args);

            return true;
        }


        private IEnumerable GetChildrenWithoutExpanding(object model)
        {
            Branch br = TreeModel.GetBranch(model);
            if (br == null || !br.CanExpand)
                return new ArrayList();

            return br.Children;
        }

        /// <summary>
        /// Toggle the expanded state of the branch at the given model object
        /// </summary>
        /// <param name="model"></param>
        public virtual void ToggleExpansion(object model)
        {
            _ = ModelToItem(model);
            if (IsExpanded(model))
            {
                Collapse(model);
            }
            else
            {
                Expand(model);
            }
        }

        //------------------------------------------------------------------------------------------
        // Commands - Tree traversal

        /// <summary>
        /// Return whether or not the given model can expand.
        /// </summary>
        /// <param name="model"></param>
        /// <remarks>The given model must have already been seen in the tree</remarks>
        public virtual bool CanExpand(object model)
        {
            Branch br = TreeModel.GetBranch(model);
            return (br != null && br.CanExpand);
        }

        /// <summary>
        /// Return the model object that is the parent of the given model object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <remarks>The given model must have already been seen in the tree.</remarks>
        public virtual object GetParent(object model)
        {
            Branch br = TreeModel.GetBranch(model);
            return br == null || br.ParentBranch == null ? null : br.ParentBranch.Model;
        }

        /// <summary>
        /// Return the collection of model objects that are the children of the 
        /// given model as they exist in the tree at the moment.
        /// </summary>
        /// <param name="model"></param>
        /// <remarks>
        /// <para>
        /// This method returns the collection of children as the tree knows them. If the given
        /// model has never been presented to the user (e.g. it belongs to a parent that has
        /// never been expanded), then this method will return an empty collection.</para>
        /// <para>
        /// Because of this, if you want to traverse the whole tree, this is not the method to use.
        /// It's better to traverse the your data model directly.
        /// </para>
        /// <para>
        /// If the given model has not already been seen in the tree or
        /// if it is not expandable, an empty collection will be returned.
        /// </para>
        /// </remarks>
        public virtual IEnumerable GetChildren(object model)
        {
            Branch br = TreeModel.GetBranch(model);
            if (br == null || !br.CanExpand)
                return new ArrayList();

            br.FetchChildren();

            return br.Children;
        }

        //------------------------------------------------------------------------------------------
        // Delegates

        /// <summary>
        /// Delegates of this type are use to decide if the given model object can be expanded
        /// </summary>
        /// <param name="model">The model under consideration</param>
        /// <returns>Can the given model be expanded?</returns>
        public delegate bool CanExpandGetterDelegate(object model);

        /// <summary>
        /// Delegates of this type are used to fetch the children of the given model object
        /// </summary>
        /// <param name="model">The parent whose children should be fetched</param>
        /// <returns>An enumerable over the children</returns>
        public delegate IEnumerable ChildrenGetterDelegate(object model);

        /// <summary>
        /// Delegates of this type are used to fetch the parent of the given model object.
        /// </summary>
        /// <param name="model">The child whose parent should be fetched</param>
        /// <returns>The parent of the child or null if the child is a root</returns>
        public delegate object ParentGetterDelegate(object model);

        /// <summary>
        /// Delegates of this type are used to create a new underlying Tree structure.
        /// </summary>
        /// <param name="view">The view for which the Tree is being created</param>
        /// <returns>A subclass of Tree</returns>
        public delegate Tree TreeFactoryDelegate(TreeListView view);

        //------------------------------------------------------------------------------------------
        #region Implementation

        /// <summary>
        /// Handle a left button down event
        /// </summary>
        /// <param name="hti"></param>
        /// <returns></returns>
        protected override bool ProcessLButtonDown(OlvListViewHitTestInfo hti)
        {
            // Did they click in the expander?
            if (hti.HitTestLocation == HitTestLocation.ExpandButton)
            {
                PossibleFinishCellEditing();
                ToggleExpansion(hti.RowObject);
                return true;
            }

            return base.ProcessLButtonDown(hti);
        }

        /// <summary>
        /// Create a OLVListItem for given row index
        /// </summary>
        /// <param name="itemIndex">The index of the row that is needed</param>
        /// <returns>An OLVListItem</returns>
        /// <remarks>This differs from the base method by also setting up the IndentCount property.</remarks>
        public override OLVListItem MakeListViewItem(int itemIndex)
        {
            OLVListItem olvItem = base.MakeListViewItem(itemIndex);
            Branch br = TreeModel.GetBranch(olvItem.RowObject);
            if (br != null)
                olvItem.IndentCount = br.Level;
            return olvItem;
        }

        /// <summary>
        /// Reinitialize the Tree structure
        /// </summary>
        protected virtual void RegenerateTree()
        {
            TreeModel = TreeFactory == null ? new Tree(this) : TreeFactory(this);
            Trace.Assert(TreeModel != null);
            VirtualListDataSource = TreeModel;
        }

        /// <summary>
        /// Recalculate the state of the checkboxes of all the items in the given list
        /// and their ancestors.
        /// </summary>
        /// <remarks>This only makes sense when HierarchicalCheckboxes is true.</remarks>
        /// <param name="toCheck"></param>
        protected virtual void RecalculateHierarchicalCheckBoxGraph(IList toCheck)
        {
            if (toCheck == null || toCheck.Count == 0)
                return;

            // Avoid recursive calculations
            if (isRecalculatingHierarchicalCheckBox)
                return;

            try
            {
                isRecalculatingHierarchicalCheckBox = true;
                foreach (object ancestor in CalculateDistinctAncestors(toCheck))
                    RecalculateSingleHierarchicalCheckBox(ancestor);
            }
            finally
            {
                isRecalculatingHierarchicalCheckBox = false;
            }

        }
        private bool isRecalculatingHierarchicalCheckBox;

        /// <summary>
        /// Recalculate the hierarchy state of the given item and its ancestors
        /// </summary>
        /// <remarks>This only makes sense when HierarchicalCheckboxes is true.</remarks>
        /// <param name="modelObject"></param>
        protected virtual void RecalculateSingleHierarchicalCheckBox(object modelObject)
        {

            if (modelObject == null)
                return;

            // Only branches have calculated checkstates. Leaf node checkedness is not calculated
            if (!CanExpand(modelObject))
                return;

            // Set the checkedness of the given model based on the state of its children.
            CheckState? aggregate = null;
            foreach (object child in GetChildren(modelObject))
            {
                CheckState? checkedness = GetCheckState(child);
                if (!checkedness.HasValue)
                    continue;

                if (aggregate.HasValue)
                {
                    if (aggregate.Value != checkedness.Value)
                    {
                        aggregate = CheckState.Indeterminate;
                        break;
                    }
                }
                else
                    aggregate = checkedness;
            }

            base.SetObjectCheckedness(modelObject, aggregate ?? CheckState.Indeterminate);
        }

        /// <summary>
        /// Yield the unique ancestors of the given collection of objects.
        /// The order of the ancestors is guaranteed to be deeper objects first.
        /// Roots will always be last.
        /// </summary>
        /// <param name="toCheck"></param>
        /// <returns>Unique ancestors of the given objects</returns>
        protected virtual IEnumerable CalculateDistinctAncestors(IList toCheck)
        {

            if (toCheck.Count == 1)
            {
                foreach (object ancestor in GetAncestors(toCheck[0]))
                {
                    yield return ancestor;
                }
            }
            else
            {
                // WARNING - Clever code

                // Example: Calculate ancestors of A, B, X and Y
                // A and B are  children of P, child of GP, child of Root
                // X and Y are  children of Q, child of GP, child of Root

                // Build a list of all ancestors of all objects we need to check
                ArrayList allAncestors = new ArrayList();
                foreach (object child in toCheck)
                {
                    foreach (object ancestor in GetAncestors(child))
                    {
                        allAncestors.Add(ancestor);
                    }
                }

                // allAncestors = { P, GP, Root, P, GP, Root, Q, GP, Root, Q, GP, Root }

                ArrayList uniqueAncestors = new ArrayList();
                Dictionary<object, bool> alreadySeen = new Dictionary<object, bool>();
                allAncestors.Reverse();
                foreach (object ancestor in allAncestors)
                {
                    if (!alreadySeen.ContainsKey(ancestor))
                    {
                        alreadySeen[ancestor] = true;
                        uniqueAncestors.Add(ancestor);
                    }
                }

                // uniqueAncestors = { Root, GP, Q, P }

                uniqueAncestors.Reverse();
                foreach (object x in uniqueAncestors)
                    yield return x;
            }
        }

        /// <summary>
        /// Return all the ancestors of the given model
        /// </summary>
        /// <remarks>
        /// <para>
        /// This uses ParentGetter if possible.
        /// </para>
        /// <para>If the given model is a root OR if the model doesn't exist, the collection will be empty</para>
        /// </remarks>
        /// <param name="model">The model whose ancestors should be calculated</param>
        /// <returns>Return a collection of ancestors of the given model.</returns>
        protected virtual IEnumerable GetAncestors(object model)
        {
            ParentGetterDelegate parentGetterDelegate = ParentGetter ?? GetParent;

            object parent = parentGetterDelegate(model);
            while (parent != null)
            {
                yield return parent;
                parent = parentGetterDelegate(parent);
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------
        #region Event handlers

        /// <summary>
        /// The application is idle and a SelectionChanged event has been scheduled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void HandleApplicationIdle(object sender, EventArgs e)
        {
            base.HandleApplicationIdle(sender, e);

            // There is an annoying redraw bug on ListViews that use indentation and
            // that have full row select enabled. When the selection reduces to a subset
            // of previously selected rows, or when the selection is extended using
            // shift-pageup/down, then the space occupied by the identation is not
            // invalidated, and hence remains highlighted.
            // Ideally we'd want to know exactly which rows were selected or deselected
            // and then invalidate just the indentation region of those rows,
            // but that's too much work. So just redraw the control.
            // Actually... the selection issues show just slightly for non-full row select
            // controls as well. So, always redraw the control after the selection
            // changes.
            Invalidate();
        }

        /// <summary>
        /// Decide if the given key event should be handled as a normal key input to the control?
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool IsInputKey(Keys keyData)
        {
            // We want to handle Left and Right keys within the control
            Keys key = keyData & Keys.KeyCode;
            if (key == Keys.Left || key == Keys.Right)
                return true;

            return base.IsInputKey(keyData);
        }

        /// <summary>
        /// Handle the keyboard input to mimic a TreeView.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Was the key press handled?</returns>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!(FocusedItem is OLVListItem focused))
            {
                base.OnKeyDown(e);
                return;
            }

            object modelObject = focused.RowObject;
            Branch br = TreeModel.GetBranch(modelObject);

            switch (e.KeyCode)
            {
                case Keys.Left:
                    // If the branch is expanded, collapse it. If it's collapsed,
                    // select the parent of the branch.
                    if (br.IsExpanded)
                        Collapse(modelObject);
                    else
                    {
                        if (br.ParentBranch != null && br.ParentBranch.Model != null)
                            SelectObject(br.ParentBranch.Model, true);
                    }
                    e.Handled = true;
                    break;

                case Keys.Right:
                    // If the branch is expanded, select the first child.
                    // If it isn't expanded and can be, expand it.
                    if (br.IsExpanded)
                    {
                        List<Branch> filtered = br.FilteredChildBranches;
                        if (filtered.Count > 0)
                            SelectObject(filtered[0].Model, true);
                    }
                    else
                    {
                        if (br.CanExpand)
                            Expand(modelObject);
                    }
                    e.Handled = true;
                    break;
            }

            base.OnKeyDown(e);
        }

        #endregion

        //------------------------------------------------------------------------------------------
        // Support classes

        /// <summary>
        /// A Tree object represents a tree structure data model that supports both 
        /// tree and flat list operations as well as fast access to branches.
        /// </summary>
        /// <remarks>If you create a subclass of Tree, you must install it in the TreeListView
        /// via the TreeFactory delegate.</remarks>
        public class Tree : IVirtualListDataSource, IFilterableDataSource
        {
            /// <summary>
            /// Create a Tree
            /// </summary>
            /// <param name="treeView"></param>
            public Tree(TreeListView treeView)
            {
                this.treeView = treeView;
                trunk = new Branch(null, this, null)
                {
                    IsExpanded = true
                };
            }

            //------------------------------------------------------------------------------------------
            // Properties

            /// <summary>
            /// This is the delegate that will be used to decide if a model object can be expanded.
            /// </summary>
            public CanExpandGetterDelegate CanExpandGetter
            {
                get { return canExpandGetter; }
                set { canExpandGetter = value; }
            }
            private CanExpandGetterDelegate canExpandGetter;

            /// <summary>
            /// This is the delegate that will be used to fetch the children of a model object
            /// </summary>
            /// <remarks>This delegate will only be called if the CanExpand delegate has 
            /// returned true for the model object.</remarks>
            public ChildrenGetterDelegate ChildrenGetter
            {
                get { return childrenGetter; }
                set { childrenGetter = value; }
            }
            private ChildrenGetterDelegate childrenGetter;


            /// <summary>
            /// Get or return the top level model objects in the tree
            /// </summary>
            public IEnumerable RootObjects
            {
                get { return trunk.Children; }
                set
                {
                    trunk.Children = value;
                    foreach (Branch br in trunk.ChildBranches)
                        br.RefreshChildren();
                    RebuildList();
                }
            }

            /// <summary>
            /// What tree view is this Tree the model for?
            /// </summary>
            public TreeListView TreeView
            {
                get { return treeView; }
            }

            //------------------------------------------------------------------------------------------
            // Commands

            /// <summary>
            /// Collapse the subtree underneath the given model
            /// </summary>
            /// <param name="model">The model to be collapsed. If the model isn't in the tree,
            /// or if it is already collapsed, the command does nothing.</param>
            /// <returns>The index of the model in flat list version of the tree</returns>
            public virtual int Collapse(object model)
            {
                Branch br = GetBranch(model);
                if (br == null || !br.IsExpanded)
                    return -1;

                // Remember that the branch is collapsed, even if it's currently not visible
                if (!br.Visible)
                {
                    br.Collapse();
                    return -1;
                }

                int count = br.NumberVisibleDescendents;
                br.Collapse();

                // Remove the visible descendents from after the branch itself
                int index = GetObjectIndex(model);
                objectList.RemoveRange(index + 1, count);
                RebuildObjectMap(index + 1);
                return index;
            }

            /// <summary>
            /// Collapse all branches in this tree
            /// </summary>
            /// <returns>Return the index of the first root that was not collapsed</returns>
            public virtual int CollapseAll()
            {
                foreach (Branch br in trunk.ChildBranches)
                {
                    if (br.IsExpanded)
                        br.Collapse();
                }
                RebuildList();
                return 0;
            }

            /// <summary>
            /// Expand the subtree underneath the given model object
            /// </summary>
            /// <param name="model">The model to be expanded.</param> 
            /// <returns>The index of the model in flat list version of the tree</returns>
            /// <remarks>
            /// If the model isn't in the tree,
            /// if it cannot be expanded or if it is already expanded, the command does nothing.
            /// </remarks>
            public virtual int Expand(object model)
            {
                Branch br = GetBranch(model);
                if (br == null || !br.CanExpand || br.IsExpanded)
                    return -1;

                // Remember that the branch is expanded, even if it's currently not visible
                if (!br.Visible)
                {
                    br.Expand();
                    return -1;
                }

                int index = GetObjectIndex(model);
                InsertChildren(br, index + 1);
                return index;
            }

            /// <summary>
            /// Expand all branches in this tree
            /// </summary>
            /// <returns>Return the index of the first branch that was expanded</returns>
            public virtual int ExpandAll()
            {
                trunk.ExpandAll();
                Sort(lastSortColumn, lastSortOrder);
                return 0;
            }

            /// <summary>
            /// Return the Branch object that represents the given model in the tree
            /// </summary>
            /// <param name="model">The model whose branches is to be returned</param>
            /// <returns>The branch that represents the given model, or null if the model
            /// isn't in the tree.</returns>
            public virtual Branch GetBranch(object model)
            {
                if (model == null)
                    return null;

                mapObjectToBranch.TryGetValue(model, out Branch br);
                return br;
            }

            /// <summary>
            /// Return the number of visible descendents that are below the given model.
            /// </summary>
            /// <param name="model">The model whose descendent count is to be returned</param>
            /// <returns>The number of visible descendents. 0 if the model doesn't exist or is collapsed</returns>
            public virtual int GetVisibleDescendentCount(object model)
            {
                Branch br = GetBranch(model);
                if (br == null || !br.IsExpanded)
                    return 0;
                else
                    return br.NumberVisibleDescendents;
            }

            /// <summary>
            /// Rebuild the children of the given model, refreshing any cached information held about the given object
            /// </summary>
            /// <param name="model"></param>
            /// <returns>The index of the model in flat list version of the tree</returns>
            public virtual int RebuildChildren(object model)
            {
                Branch br = GetBranch(model);
                if (br == null || !br.Visible)
                    return -1;

                int count = br.NumberVisibleDescendents;

                // Remove the visible descendents from after the branch itself
                int index = GetObjectIndex(model);
                if (count > 0)
                    objectList.RemoveRange(index + 1, count);
                if (br.CanExpand && br.IsExpanded)
                {
                    br.RefreshChildren();
                    InsertChildren(br, index + 1);
                }
                return index;
            }

            //------------------------------------------------------------------------------------------
            // Implementation

            /// <summary>
            /// Is the given model expanded?
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            internal bool IsModelExpanded(object model)
            {
                // Special case: model == null is the container for the roots. This is always expanded
                if (model == null)
                    return true;
                mapObjectToExpanded.TryGetValue(model, out bool isExpanded);
                return isExpanded;
            }

            /// <summary>
            /// Remember whether or not the given model was expanded
            /// </summary>
            /// <param name="model"></param>
            /// <param name="isExpanded"></param>
            internal void SetModelExpanded(object model, bool isExpanded)
            {
                if (model == null) return;

                if (isExpanded)
                    mapObjectToExpanded[model] = true;
                else
                    mapObjectToExpanded.Remove(model);
            }

            /// <summary>
            /// Insert the children of the given branch into the given position
            /// </summary>
            /// <param name="br">The branch whose children should be inserted</param>
            /// <param name="index">The index where the children should be inserted</param>
            protected virtual void InsertChildren(Branch br, int index)
            {
                // Expand the branch
                br.Expand();
                br.Sort(GetBranchComparer());

                // Insert the branch's visible descendents after the branch itself
                objectList.InsertRange(index, br.Flatten());
                RebuildObjectMap(index);
            }

            /// <summary>
            /// Rebuild our flat internal list of objects.
            /// </summary>
            protected virtual void RebuildList()
            {
                objectList = ArrayList.Adapter(trunk.Flatten());
                List<Branch> filtered = trunk.FilteredChildBranches;
                if (filtered.Count > 0)
                {
                    filtered[0].IsFirstBranch = true;
                    filtered[0].IsOnlyBranch = (filtered.Count == 1);
                }
                RebuildObjectMap(0);
            }

            /// <summary>
            /// Rebuild our reverse index that maps an object to its location
            /// in the filteredObjectList array.
            /// </summary>
            /// <param name="startIndex"></param>
            protected virtual void RebuildObjectMap(int startIndex)
            {
                if (startIndex == 0)
                    mapObjectToIndex.Clear();
                for (int i = startIndex; i < objectList.Count; i++)
                    mapObjectToIndex[objectList[i]] = i;
            }

            /// <summary>
            /// Create a new branch within this tree
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="model"></param>
            /// <returns></returns>
            internal Branch MakeBranch(Branch parent, object model)
            {
                Branch br = new Branch(parent, this, model);

                // Remember that the given branch is part of this tree.
                mapObjectToBranch[model] = br;
                return br;
            }

            //------------------------------------------------------------------------------------------

            #region IVirtualListDataSource Members

            /// <summary>
            /// 
            /// </summary>
            /// <param name="n"></param>
            /// <returns></returns>
            public virtual object GetNthObject(int n)
            {
                return objectList[n];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public virtual int GetObjectCount()
            {
                return trunk.NumberVisibleDescendents;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public virtual int GetObjectIndex(object model)
            {
                if (model != null && mapObjectToIndex.TryGetValue(model, out int index))
                    return index;
                else
                    return -1;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="first"></param>
            /// <param name="last"></param>
            public virtual void PrepareCache(int first, int last)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <param name="first"></param>
            /// <param name="last"></param>
            /// <param name="column"></param>
            /// <returns></returns>
            public virtual int SearchText(string value, int first, int last, OLVColumn column)
            {
                return AbstractVirtualListDataSource.DefaultSearchText(value, first, last, column, this);
            }

            /// <summary>
            /// Sort the tree on the given column and in the given order
            /// </summary>
            /// <param name="column"></param>
            /// <param name="order"></param>
            public virtual void Sort(OLVColumn column, SortOrder order)
            {
                lastSortColumn = column;
                lastSortOrder = order;

                // TODO: Need to raise an AboutToSortEvent here

                // Sorting is going to change the order of the branches so clear
                // the "first branch" flag
                foreach (Branch b in trunk.ChildBranches)
                    b.IsFirstBranch = false;

                trunk.Sort(GetBranchComparer());
                RebuildList();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected virtual BranchComparer GetBranchComparer()
            {
                if (lastSortColumn == null)
                    return null;

                return new BranchComparer(new ModelObjectComparer(
                    lastSortColumn,
                    lastSortOrder,
                    treeView.SecondarySortColumn ?? treeView.GetColumn(0),
                    treeView.SecondarySortColumn == null ? lastSortOrder : treeView.SecondarySortOrder));
            }

            /// <summary>
            /// Add the given collection of objects to the roots of this tree
            /// </summary>
            /// <param name="modelObjects"></param>
            public virtual void AddObjects(ICollection modelObjects)
            {
                ArrayList newRoots = EnumerableToArray(treeView.Roots, true);
                foreach (object x in modelObjects)
                    newRoots.Add(x);
                SetObjects(newRoots);
            }

            /// <summary>
            /// Remove all of the given objects from the roots of the tree.
            /// Any objects that is not already in the roots collection is ignored.
            /// </summary>
            /// <param name="modelObjects"></param>
            public virtual void RemoveObjects(ICollection modelObjects)
            {
                ArrayList newRoots = new ArrayList();
                foreach (object x in treeView.Roots)
                    newRoots.Add(x);
                foreach (object x in modelObjects)
                {
                    newRoots.Remove(x);
                    mapObjectToIndex.Remove(x);
                }
                SetObjects(newRoots);
            }

            /// <summary>
            /// Set the roots of this tree to be the given collection
            /// </summary>
            /// <param name="collection"></param>
            public virtual void SetObjects(IEnumerable collection)
            {
                // We interpret a SetObjects() call as setting the roots of the tree
                treeView.Roots = collection;
            }

            /// <summary>
            /// Update/replace the nth object with the given object
            /// </summary>
            /// <param name="index"></param>
            /// <param name="modelObject"></param>
            public void UpdateObject(int index, object modelObject)
            {
                ArrayList newRoots = EnumerableToArray(treeView.Roots, false);
                if (index < newRoots.Count)
                    newRoots[index] = modelObject;
                SetObjects(newRoots);
            }

            #endregion

            #region IFilterableDataSource Members

            /// <summary>
            /// 
            /// </summary>
            /// <param name="modelFilter"></param>
            /// <param name="listFilter"></param>
            public void ApplyFilters(IModelFilter modelFilter, IListFilter listFilter)
            {
                this.modelFilter = modelFilter;
                this.listFilter = listFilter;
                RebuildList();
            }

            /// <summary>
            /// Is this list currently being filtered?
            /// </summary>
            internal bool IsFiltering
            {
                get
                {
                    return treeView.UseFiltering && (modelFilter != null || listFilter != null);
                }
            }

            /// <summary>
            /// Should the given model be included in this control?
            /// </summary>
            /// <param name="model">The model to consider</param>
            /// <returns>True if it will be included</returns>
            internal bool IncludeModel(object model)
            {
                if (!treeView.UseFiltering)
                    return true;

                if (modelFilter == null)
                    return true;

                return modelFilter.Filter(model);
            }

            #endregion

            //------------------------------------------------------------------------------------------
            // Private instance variables

            private OLVColumn lastSortColumn;
            private SortOrder lastSortOrder;
            private readonly Dictionary<object, Branch> mapObjectToBranch = new Dictionary<object, Branch>();
            internal Dictionary<object, bool> mapObjectToExpanded = new Dictionary<object, bool>();
            private readonly Dictionary<object, int> mapObjectToIndex = new Dictionary<object, int>();
            private ArrayList objectList = new ArrayList();
            private readonly TreeListView treeView;
            private readonly Branch trunk;

            /// <summary>
            /// 
            /// </summary>
            protected IModelFilter modelFilter;
            /// <summary>
            /// 
            /// </summary>
            protected IListFilter listFilter;
        }

        /// <summary>
        /// A Branch represents a sub-tree within a tree
        /// </summary>
        public class Branch
        {
            /// <summary>
            /// Indicators for branches
            /// </summary>
            [Flags]
            public enum BranchFlags
            {
                /// <summary>
                /// FirstBranch of tree
                /// </summary>
                FirstBranch = 1,

                /// <summary>
                /// LastChild of parent
                /// </summary>
                LastChild = 2,

                /// <summary>
                /// OnlyBranch of tree
                /// </summary>
                OnlyBranch = 4
            }

            #region Life and death

            /// <summary>
            /// Create a Branch
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="tree"></param>
            /// <param name="model"></param>
            public Branch(Branch parent, Tree tree, object model)
            {
                ParentBranch = parent;
                Tree = tree;
                Model = model;
            }

            #endregion

            #region Public properties

            //------------------------------------------------------------------------------------------
            // Properties

            /// <summary>
            /// Get the ancestor branches of this branch, with the 'oldest' ancestor first.
            /// </summary>
            public virtual IList<Branch> Ancestors
            {
                get
                {
                    List<Branch> ancestors = new List<Branch>();
                    if (ParentBranch != null)
                        ParentBranch.PushAncestors(ancestors);
                    return ancestors;
                }
            }

            private void PushAncestors(IList<Branch> list)
            {
                // This is designed to ignore the trunk (which has no parent)
                if (ParentBranch != null)
                {
                    ParentBranch.PushAncestors(list);
                    list.Add(this);
                }
            }

            /// <summary>
            /// Can this branch be expanded?
            /// </summary>
            public virtual bool CanExpand
            {
                get
                {
                    if (Tree.CanExpandGetter == null || Model == null)
                        return false;
                    else
                        return Tree.CanExpandGetter(Model);
                }
            }

            /// <summary>
            /// Gets or sets our children
            /// </summary>
            public List<Branch> ChildBranches
            {
                get { return childBranches; }
                set { childBranches = value; }
            }
            private List<Branch> childBranches = new List<Branch>();

            /// <summary>
            /// Get/set the model objects that are beneath this branch
            /// </summary>
            public virtual IEnumerable Children
            {
                get
                {
                    ArrayList children = new ArrayList();
                    foreach (Branch x in ChildBranches)
                        children.Add(x.Model);
                    return children;
                }
                set
                {
                    ChildBranches.Clear();

                    TreeListView treeListView = Tree.TreeView;
                    CheckState? checkedness = null;
                    if (treeListView != null && treeListView.HierarchicalCheckboxes)
                        checkedness = treeListView.GetCheckState(Model);
                    foreach (object x in value)
                    {
                        AddChild(x);

                        // If the tree view is showing hierarchical checkboxes, then
                        // when a child object is first added, it has the same checkedness as this branch
                        if (checkedness.HasValue && checkedness.Value == CheckState.Checked)
                            treeListView.SetObjectCheckedness(x, checkedness.Value);
                    }
                }
            }

            private void AddChild(object model)
            {
                Branch br = Tree.GetBranch(model);
                if (br == null)
                    br = Tree.MakeBranch(this, model);
                else
                {
                    br.ParentBranch = this;
                    br.Model = model;
                    br.ClearCachedInfo();
                }
                ChildBranches.Add(br);
            }

            /// <summary>
            /// Gets a list of all the branches that survive filtering
            /// </summary>
            public List<Branch> FilteredChildBranches
            {
                get
                {
                    if (!IsExpanded)
                        return new List<Branch>();

                    if (!Tree.IsFiltering)
                        return ChildBranches;

                    List<Branch> filtered = new List<Branch>();
                    foreach (Branch b in ChildBranches)
                    {
                        if (Tree.IncludeModel(b.Model))
                            filtered.Add(b);
                        else
                        {
                            // Also include this branch if it has any filtered branches (yes, its recursive)
                            if (b.FilteredChildBranches.Count > 0)
                                filtered.Add(b);
                        }
                    }
                    return filtered;
                }
            }

            /// <summary>
            /// Gets or set whether this branch is expanded
            /// </summary>
            public bool IsExpanded
            {
                get { return Tree.IsModelExpanded(Model); }
                set { Tree.SetModelExpanded(Model, value); }
            }

            /// <summary>
            /// Return true if this branch is the first branch of the entire tree
            /// </summary>
            public virtual bool IsFirstBranch
            {
                get
                {
                    return ((flags & BranchFlags.FirstBranch) != 0);
                }
                set
                {
                    if (value)
                        flags |= BranchFlags.FirstBranch;
                    else
                        flags &= ~BranchFlags.FirstBranch;
                }
            }

            /// <summary>
            /// Return true if this branch is the last child of its parent
            /// </summary>
            public virtual bool IsLastChild
            {
                get
                {
                    return ((flags & BranchFlags.LastChild) != 0);
                }
                set
                {
                    if (value)
                        flags |= BranchFlags.LastChild;
                    else
                        flags &= ~BranchFlags.LastChild;
                }
            }

            /// <summary>
            /// Return true if this branch is the only top level branch
            /// </summary>
            public virtual bool IsOnlyBranch
            {
                get
                {
                    return ((flags & BranchFlags.OnlyBranch) != 0);
                }
                set
                {
                    if (value)
                        flags |= BranchFlags.OnlyBranch;
                    else
                        flags &= ~BranchFlags.OnlyBranch;
                }
            }

            /// <summary>
            /// Gets the depth level of this branch
            /// </summary>
            public int Level
            {
                get
                {
                    if (ParentBranch == null)
                        return 0;
                    else
                        return ParentBranch.Level + 1;
                }
            }

            /// <summary>
            /// Gets or sets which model is represented by this branch
            /// </summary>
            public object Model
            {
                get { return model; }
                set { model = value; }
            }
            private object model;

            /// <summary>
            /// Return the number of descendents of this branch that are currently visible
            /// </summary>
            /// <returns></returns>
            public virtual int NumberVisibleDescendents
            {
                get
                {
                    if (!IsExpanded)
                        return 0;

                    List<Branch> filtered = FilteredChildBranches;
                    int count = filtered.Count;
                    foreach (Branch br in filtered)
                        count += br.NumberVisibleDescendents;
                    return count;
                }
            }

            /// <summary>
            /// Gets or sets our parent branch
            /// </summary>
            public Branch ParentBranch
            {
                get { return parentBranch; }
                set { parentBranch = value; }
            }
            private Branch parentBranch;

            /// <summary>
            /// Gets or sets our overall tree
            /// </summary>
            public Tree Tree
            {
                get { return tree; }
                set { tree = value; }
            }
            private Tree tree;

            /// <summary>
            /// Is this branch currently visible? A branch is visible
            /// if it has no parent (i.e. it's a root), or its parent
            /// is visible and expanded.
            /// </summary>
            public virtual bool Visible
            {
                get
                {
                    if (ParentBranch == null)
                        return true;
                    else
                        return ParentBranch.IsExpanded && ParentBranch.Visible;
                }
            }

            #endregion

            #region Commands

            //------------------------------------------------------------------------------------------
            // Commands

            /// <summary>
            /// Clear any cached information that this branch is holding
            /// </summary>
            public virtual void ClearCachedInfo()
            {
                Children = new ArrayList();
                alreadyHasChildren = false;
            }

            /// <summary>
            /// Collapse this branch
            /// </summary>
            public virtual void Collapse()
            {
                IsExpanded = false;
            }

            /// <summary>
            /// Expand this branch
            /// </summary>
            public virtual void Expand()
            {
                if (CanExpand)
                {
                    IsExpanded = true;
                    FetchChildren();
                }
            }

            /// <summary>
            /// Expand this branch recursively
            /// </summary>
            public virtual void ExpandAll()
            {
                Expand();
                foreach (Branch br in ChildBranches)
                {
                    if (br.CanExpand)
                        br.ExpandAll();
                }
            }

            /// <summary>
            /// Fetch the children of this branch.
            /// </summary>
            /// <remarks>This should only be called when CanExpand is true.</remarks>
            public virtual void FetchChildren()
            {
                if (alreadyHasChildren)
                    return;

                alreadyHasChildren = true;

                if (Tree.ChildrenGetter == null)
                    return;

                Cursor previous = Cursor.Current;
                try
                {
                    if (Tree.TreeView.UseWaitCursorWhenExpanding)
                        Cursor.Current = Cursors.WaitCursor;
                    Children = Tree.ChildrenGetter(Model);
                }
                finally
                {
                    Cursor.Current = previous;
                }
            }

            /// <summary>
            /// Collapse the visible descendents of this branch into list of model objects
            /// </summary>
            /// <returns></returns>
            public virtual IList Flatten()
            {
                ArrayList flatList = new ArrayList();
                if (IsExpanded)
                    FlattenOnto(flatList);
                return flatList;
            }

            /// <summary>
            /// Flatten this branch's visible descendents onto the given list.
            /// </summary>
            /// <param name="flatList"></param>
            /// <remarks>The branch itself is <b>not</b> included in the list.</remarks>
            public virtual void FlattenOnto(IList flatList)
            {
                Branch lastBranch = null;
                foreach (Branch br in FilteredChildBranches)
                {
                    lastBranch = br;
                    br.IsLastChild = false;
                    flatList.Add(br.Model);
                    if (br.IsExpanded)
                        br.FlattenOnto(flatList);
                }
                if (lastBranch != null)
                    lastBranch.IsLastChild = true;
            }

            /// <summary>
            /// Force a refresh of all children recursively
            /// </summary>
            public virtual void RefreshChildren()
            {
                if (!IsExpanded || !CanExpand)
                    return;

                ClearCachedInfo();
                FetchChildren();
                foreach (Branch br in ChildBranches)
                    br.RefreshChildren();
            }

            /// <summary>
            /// Sort the sub-branches and their descendents so they are ordered according
            /// to the given comparer.
            /// </summary>
            /// <param name="comparer">The comparer that orders the branches</param>
            public virtual void Sort(BranchComparer comparer)
            {
                if (ChildBranches.Count == 0)
                    return;

                if (comparer != null)
                    ChildBranches.Sort(comparer);

                foreach (Branch br in ChildBranches)
                    br.Sort(comparer);
            }

            #endregion


            //------------------------------------------------------------------------------------------
            // Private instance variables

            private bool alreadyHasChildren = false;
            private BranchFlags flags;
        }

        /// <summary>
        /// This class sorts branches according to how their respective model objects are sorted
        /// </summary>
        public class BranchComparer : IComparer<Branch>
        {
            /// <summary>
            /// Create a BranchComparer
            /// </summary>
            /// <param name="actualComparer"></param>
            public BranchComparer(IComparer actualComparer)
            {
                this.actualComparer = actualComparer;
            }

            /// <summary>
            /// Order the two branches
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(Branch x, Branch y)
            {
                return actualComparer.Compare(x.Model, y.Model);
            }

            private readonly IComparer actualComparer;
        }

    }
}
