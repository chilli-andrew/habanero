//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using Gizmox.WebGUI.Forms;
using Habanero.Base;
using Habanero.BO;
using Habanero.UI.Base;
using DialogResult=Gizmox.WebGUI.Forms.DialogResult;

namespace Habanero.UI.WebGUI
{
    public class ReadOnlyGridControlGiz : ControlGiz, IReadOnlyGridControl, ISupportInitialize
    {
        #region Delegates

        public delegate void RefreshGridDelegate();

        #endregion

        private readonly IReadOnlyGridButtonsControl _buttons;

        private readonly IControlFactory _controlFactory;
        private readonly IFilterControl _filterControl;
        private readonly ReadOnlyGridGiz _grid;
        private readonly IGridInitialiser _gridInitialiser;
        private string _additionalSearchCriteria;
        private IBusinessObjectCreator _businessObjectCreator;
        private IBusinessObjectDeletor _businessObjectDeletor;
        private IBusinessObjectEditor _businessObjectEditor;
        private string _orderBy;

        public ReadOnlyGridControlGiz() : this(GlobalUIRegistry.ControlFactory)
        {
        }

        /// <summary>
        /// Constructor to initialise a new grid
        /// </summary>
        public ReadOnlyGridControlGiz(IControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
            _filterControl = _controlFactory.CreateFilterControl();
            _grid = new ReadOnlyGridGiz();
            _buttons = _controlFactory.CreateReadOnlyGridButtonsControl();
            _gridInitialiser = new GridInitialiser(this, _controlFactory);
            InitialiseButtons();
            InitialiseFilterControl();

            BorderLayoutManager borderLayoutManager = new BorderLayoutManagerGiz(this, _controlFactory);
            borderLayoutManager.AddControl(_grid, BorderLayoutManager.Position.Centre);
            borderLayoutManager.AddControl(_buttons, BorderLayoutManager.Position.South);
            borderLayoutManager.AddControl(_filterControl, BorderLayoutManager.Position.North);
            FilterMode = FilterModes.Filter;
            _grid.Name = "GridControl";
        }

        #region IReadOnlyGridControl Members

        /// <summary>
        /// initiliase the grid to the with the 'default' UIdef.
        /// </summary>
        public void Initialise(IClassDef classDef)
        {
            _gridInitialiser.InitialiseGrid(classDef);
        }

        public void Initialise(IClassDef classDef, string uiDefName)
        {
            if (classDef == null) throw new ArgumentNullException("classDef");
            if (uiDefName == null) throw new ArgumentNullException("uiDefName");

            ClassDef = classDef;
            UiDefName = uiDefName;

            _gridInitialiser.InitialiseGrid(ClassDef, uiDefName);
        }


        /// <summary>
        /// Returns the grid object held. This property can be used to
        /// access a range of functionality for the grid
        /// (eg. myGridWithButtons.Grid.AddBusinessObject(...)).
        /// </summary>
        public IGridBase Grid
        {
            get { return _grid; }
        }

        /// <summary>
        /// Gets or sets the single selected business object (null if none are selected)
        /// denoted by where the current selected cell is
        /// </summary>
        public IBusinessObject SelectedBusinessObject
        {
            get { return _grid.SelectedBusinessObject; }
            set { _grid.SelectedBusinessObject = value; }
        }

        /// <summary>
        /// Returns the button control held. This property can be used
        /// to access a range of functionality for the button control
        /// (eg. myGridWithButtons.Buttons.AddButton(...)).
        /// </summary>
        public IReadOnlyGridButtonsControl Buttons
        {
            get { return _buttons; }
        }

        public IBusinessObjectEditor BusinessObjectEditor
        {
            get { return _businessObjectEditor; }
            set { _businessObjectEditor = value; }
        }

        public IBusinessObjectCreator BusinessObjectCreator
        {
            get { return _businessObjectCreator; }
            set { _businessObjectCreator = value; }
        }

        public IBusinessObjectDeletor BusinessObjectDeletor
        {
            get { return _businessObjectDeletor; }
            set { _businessObjectDeletor = value; }
        }

        public string UiDefName
        {
            get { return _grid.GridBaseManager.UiDefName; }
            set { _grid.GridBaseManager.UiDefName = value; }
        }

        public IClassDef ClassDef
        {
            get { return _grid.GridBaseManager.ClassDef; }
            set { _grid.GridBaseManager.ClassDef = value; }
        }

        public IFilterControl FilterControl
        {
            get { return _filterControl; }
        }

        public bool IsInitialised
        {
            get { return _gridInitialiser.IsInitialised; }
        }

        public FilterModes FilterMode
        {
            get { return _filterControl.FilterMode; }
            set { _filterControl.FilterMode = value; }
        }

        /// <summary>
        /// Gets and sets the default order by clause used for loading the grid when the <see cref="IReadOnlyGridControl.FilterMode"/>
        /// is Search see <see cref="FilterModes"/>
        /// </summary>
        public string OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = value; }
        }

        /// <summary>
        /// Gets and sets the standard search criteria used for loading the grid when the <see cref="IReadOnlyGridControl.FilterMode"/>
        /// is Search see <see cref="FilterModes"/>. This search criteria will be And (ed) to any search criteria returned
        /// by the FilterControl.
        /// </summary>
        public string AdditionalSearchCriteria
        {
            get { return _additionalSearchCriteria; }
            set { _additionalSearchCriteria = value; }
        }


        public void SetBusinessObjectCollection(IBusinessObjectCollection boCollection)
        {
            if (boCollection == null)
            {
                _grid.SetBusinessObjectCollection(null);
                Buttons.Enabled = false;
                FilterControl.Enabled = false;
                return;
            }
            if (ClassDef == null)
            {
                Initialise(boCollection.ClassDef);
            }
            else
            {
                if (ClassDef != boCollection.ClassDef)
                {
                    throw new ArgumentException(
                        "You cannot call set collection for a collection that has a different class def than is initialised");
                }
            }
            if (BusinessObjectCreator is DefaultBOCreator)
            {
                BusinessObjectCreator = new DefaultBOCreator(boCollection);
            }
            if (BusinessObjectCreator == null) BusinessObjectCreator = new DefaultBOCreator(boCollection);
            if (BusinessObjectEditor == null) BusinessObjectEditor = new DefaultBOEditor(_controlFactory);
            if (BusinessObjectDeletor == null) BusinessObjectDeletor = new DefaultBODeletor();

            _grid.SetBusinessObjectCollection(boCollection);

            Buttons.Enabled = true;
            FilterControl.Enabled = true;
        }

        /// <summary>
        /// Initialises the grid based with no classDef. This is used where the columns are set up manually.
        /// A typical case of when you would want to set the columns manually would be when the grid
        ///  requires alternate columns e.g. images to indicate the state of the object or buttons/links.
        /// The grid must already have at least one column added. At least one column must be a column with the name
        /// "ID" This column is used to synchronise the grid with the business objects.
        /// </summary>
        /// <exception cref="GridBaseInitialiseException"> in the case where the columns have not already been defined for the grid</exception>
        public void Initialise()
        {
            _gridInitialiser.InitialiseGrid();
        }

        #endregion

        #region ISupportInitialize Members

        ///<summary>
        ///Signals the object that initialization is starting.
        ///</summary>
        ///
        public void BeginInit()
        {
            ((ISupportInitialize) Grid).BeginInit();
        }

        ///<summary>
        ///Signals the object that initialization is complete.
        ///</summary>
        ///
        public void EndInit()
        {
            ((ISupportInitialize) Grid).EndInit();
        }

        #endregion

        private void InitialiseFilterControl()
        {
            _filterControl.Filter += _filterControl_OnFilter;
        }

        private void _filterControl_OnFilter(object sender, EventArgs e)
        {
            Grid.CurrentPage = 1;
            if (FilterMode == FilterModes.Search)
            {
                BusinessObjectCollection<BusinessObject> collection =
                    new BusinessObjectCollection<BusinessObject>(ClassDef);
                string searchClause = _filterControl.GetFilterClause().GetFilterClauseString("%", "'");
                if (!string.IsNullOrEmpty(AdditionalSearchCriteria))
                {
                    if (!string.IsNullOrEmpty(searchClause))
                    {
                        searchClause += " AND ";
                    }
                    searchClause += AdditionalSearchCriteria;
                }
                collection.Load(searchClause, OrderBy);
                SetBusinessObjectCollection(collection);
            }
            else
            {
                Grid.ApplyFilter(_filterControl.GetFilterClause());
            }
        }

        private void InitialiseButtons()
        {
            _buttons.AddClicked += Buttons_AddClicked;
            _buttons.EditClicked += Buttons_EditClicked;
            _buttons.DeleteClicked += Buttons_DeleteClicked;
            _buttons.Name = "ButtonControl";
        }

        private void Buttons_DeleteClicked(object sender, EventArgs e)

        {
            try
            {
                if (Grid.GetBusinessObjectCollection() == null)
                {
                    throw new GridDeveloperException("You cannot call delete since the grid has not been set up");
                }
                IBusinessObject selectedBo = SelectedBusinessObject;

                if (selectedBo != null)
                {
                    MessageBox.Show("Are you certain you want to delete the object '" + selectedBo + "'",
                                    "Delete Object", MessageBoxButtons.YesNo,
                                    delegate(object msgBoxSender, EventArgs e1)
                                        {
                                            if (((Form) msgBoxSender).DialogResult == DialogResult.Yes)
                                            {
                                                try
                                                {
                                                    _grid.SelectedBusinessObject = null;
                                                    _businessObjectDeletor.DeleteBusinessObject(selectedBo);
                                                }
                                                catch (Exception ex)
                                                {
                                                    try
                                                    {
                                                        selectedBo.Restore();
                                                        _grid.SelectedBusinessObject = selectedBo;
                                                    }
                                                    catch (Exception)
                                                    {
                                                        //Do nothing
                                                    }
                                                    GlobalRegistry.UIExceptionNotifier.Notify(ex,
                                                                                              "There was a problem deleting",
                                                                                              "Problem Deleting");
                                                }
                                            }
                                        });
                }
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "There was a problem deleting", "Problem Deleting");
            }
        }

        private void Buttons_EditClicked(object sender, EventArgs e)
        {
            try
            {
                if (Grid.GetBusinessObjectCollection() == null)
                {
                    throw new GridDeveloperException("You cannot call edit since the grid has not been set up");
                }
                IBusinessObject selectedBo = SelectedBusinessObject;
                if (selectedBo != null)
                {
                    if (_businessObjectEditor != null)
                    {
                        _businessObjectEditor.EditObject(selectedBo, UiDefName, delegate { Grid.RefreshGrid(); });
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error trying to edit an item");
            }
        }

        private void Buttons_AddClicked(object sender, EventArgs e)
        {
            try
            {
                if (Grid.GetBusinessObjectCollection() == null)
                {
                    throw new GridDeveloperException("You cannot call add since the grid has not been set up");
                }
                IBusinessObject newBo;
                if (_businessObjectCreator == null)
                {
                    throw new GridDeveloperException(
                        "You cannot call add as there is no business object creator set up for the grid");
                }
                newBo = _businessObjectCreator.CreateBusinessObject();
                if (_businessObjectEditor != null && newBo != null)
                {
                    _businessObjectEditor.EditObject(newBo, UiDefName,
                                                     delegate(IBusinessObject bo) { Grid.SelectedBusinessObject = bo; });
                }
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error trying to add an item");
            }
        }
    }
}