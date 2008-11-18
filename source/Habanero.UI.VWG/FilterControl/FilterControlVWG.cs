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
using System.Collections;
using System.Collections.Generic;
using Gizmox.WebGUI.Forms;
using Habanero.Base;
using Habanero.UI.Base;

namespace Habanero.UI.VWG
{
    /// <summary>
    /// Manages a group of filter controls that create a filter clause used to limit
    /// which rows of data to show on a DataGridView
    /// </summary>
    public class FilterControlVWG : PanelVWG, IFilterControl
    {
        private readonly IControlFactory _controlFactory;
        private readonly FilterControlManager _filterControlManager;
        private IButton _filterButton;
        private IButton _clearButton;
        public event EventHandler Filter;
        private readonly IGroupBox _gbox;
        private FilterModes _filterMode; //Note all this should move up to windows need to decide buttons etc on win
        private readonly IPanel _controlPanel;

        public FilterControlVWG(IControlFactory controlFactory)
        {

            this.Height = 50;
            _controlFactory = controlFactory;
            _gbox = _controlFactory.CreateGroupBox();
            _controlFactory.CreateBorderLayoutManager(this).AddControl(_gbox, BorderLayoutManager.Position.Centre);
            _gbox.Text = "Filter the Grid";

            BorderLayoutManager layoutManager = controlFactory.CreateBorderLayoutManager(_gbox);
            layoutManager.BorderSize = 20;
            IPanel filterButtonPanel = controlFactory.CreatePanel();
            filterButtonPanel.Height = 50;
            filterButtonPanel.Width = 110;
            CreateFilterButtons(filterButtonPanel);

            layoutManager.AddControl(filterButtonPanel, BorderLayoutManager.Position.West);

            _controlPanel = controlFactory.CreatePanel();
            _controlPanel.Width = this.Width;

            layoutManager.AddControl(_controlPanel, BorderLayoutManager.Position.Centre);
            _filterControlManager = new FilterControlManager(controlFactory, new FlowLayoutManager(_controlPanel, controlFactory));
        }

        //public int CountOfFilterControls()
        //{
        //    return 
        //}

        private void CreateFilterButtons(IPanel filterButtonPanel)
        {
            const int buttonHeight = 20;
            const int buttonWidth = 45;
            _filterButton = CreateFilterButton(buttonWidth, buttonHeight);
            _clearButton = CreateClearButton(buttonWidth, buttonHeight);

            FlowLayoutManager layoutManager = new FlowLayoutManager(filterButtonPanel, _controlFactory);
            layoutManager.AddControl(_filterButton);
            layoutManager.AddControl(_clearButton);
        }

        private IButton CreateClearButton(int buttonWidth, int buttonHeight)
        {
            IButton clearButton = _controlFactory.CreateButton();
            clearButton.Width = buttonWidth;
            clearButton.Height = buttonHeight;
            clearButton.Top = _filterButton.Height + 2;
            clearButton.Text = "Clear";
            clearButton.Click += delegate { ClearFilters(); };
            return clearButton;
        }

        private IButton CreateFilterButton(int buttonWidth, int buttonHeight)
        {
            IButton filterButton = _controlFactory.CreateButton();
            filterButton.Width = buttonWidth;
            filterButton.Height = buttonHeight;
            filterButton.Top = 0;
            filterButton.Text = "Filter";
            filterButton.Click += delegate { FireFilterEvent(); };
            return filterButton;
        }

        /// <summary>
        /// Applies the filter that has been captured.
        /// This allows an external control (e.g. another button click) to be used as the event that causes the filter to fire.
        /// Typically used when the filter controls are being set manually.
        /// </summary>
        public void ApplyFilter()
        {
            FireFilterEvent();
        }

        /// <summary>
        /// The header text that will be set above the filter.  Defaults to 'Filter'.
        /// </summary>
        public string HeaderText
        {
            get { return _gbox.Text; }
            set { _gbox.Text = value; }
        }

        /// <summary>
        /// The number of controls used for filtering that are on the filter control. <see cref="IFilterControl.FilterControls"/>
        /// </summary>
        public int CountOfFilters
        {
            get { return _filterControlManager.CountOfFilters; }
        }

        private void FireFilterEvent()
        {
            if (Filter != null)
            {
                Filter(this, new EventArgs());
            }
        }

        /// <summary>
        /// Adds a TextBox filter in which users can specify text that
        /// a string-value column will be filtered on.  This uses a "like"
        /// operator and accepts any strings that contain the provided clause.
        /// </summary>
        /// <param name="labelText">The label to appear before the control</param>
        /// <param name="propertyName">The business object property on which to filter</param>
        /// <returns>Returns the new TextBox added</returns>
        public ITextBox AddStringFilterTextBox(string labelText, string propertyName)
        {
            ITextBox textBox = _filterControlManager.AddStringFilterTextBox(labelText, propertyName);
            return textBox;
        }

        /// <summary>
        /// Adds a TextBox filter in which users can specify text that
        /// a string-value column will be filtered on.
        /// </summary>
        /// <param name="labelText">The label to appear before the control</param>
        /// <param name="propertyName">The business object property on which to filter</param>
        /// <param name="filterClauseOperator">The operator to use for the filter clause</param>
        /// <returns>Returns the new TextBox added</returns>
        public ITextBox AddStringFilterTextBox(string labelText, string propertyName,
                                               FilterClauseOperator filterClauseOperator)
        {
            return _filterControlManager.AddStringFilterTextBox(labelText, propertyName, filterClauseOperator);
        }

        /// <summary>
        /// Adds a ComboBox filter control
        /// </summary>
        /// <param name="labelText">The label to appear before the control</param>
        /// <param name="propertyName">The business object property on which to filter</param>
        /// <param name="options">The collection of items used to fill the combo box.</param>
        /// <param name="strictMatch">Whether to filter the DataGridView column on a strict match or using a LIKE operator</param>
        /// <returns>Returns the new ComboBox added</returns>
        public IComboBox AddStringFilterComboBox(string labelText, string propertyName, ICollection options,
                                                 bool strictMatch)
        {
            IComboBox comboBox =
                _filterControlManager.AddStringFilterComboBox(labelText, propertyName, options, strictMatch);
            comboBox.Height = new TextBox().Height;
            return comboBox;
        }

        /// <summary>
        /// Adds a CheckBox filter that displays only rows whose boolean value
        /// matches the on-off state of the CheckBox. The column of data must
        /// have "true" or "false" as its values (boolean database fields are
        /// usually converted to true/false string values by the Habanero
        /// object manager).
        /// </summary>
        /// <param name="labelText">The label to appear before the control</param>
        /// <param name="propertyName">The business object property on which to filter</param>
        /// <param name="defaultValue">Whether the CheckBox is checked</param>
        /// <returns>Returns the new CheckBox added</returns>
        public ICheckBox AddBooleanFilterCheckBox(string labelText, string propertyName, bool defaultValue)
        {
            ICheckBox checkBox = _filterControlManager.AddBooleanFilterCheckBox(labelText, propertyName, defaultValue);
            return checkBox;
        }

        /// <summary>
        /// Adds a date-time picker that filters a date column on the date
        /// chosen by the user.  The given operator compares the chosen date
        /// with the date shown in the given column name.
        /// </summary>
        /// <param name="labelText">The label to appear before the control</param>
        /// <param name="propertyName">The business object property on which to filter</param>
        /// <param name="defaultValue">The default date or null</param>
        /// <param name="filterClauseOperator">The operator used to compare
        /// with the date chosen by the user.  The chosen date is on the
        /// right side of the equation.</param>
        /// <param name="nullable">Whether the datetime picker allows null values</param>
        /// <returns>Returns the new DateTimePicker added</returns>
        public IDateTimePicker AddDateFilterDateTimePicker(string labelText, string propertyName, DateTime defaultValue,
                                                           FilterClauseOperator filterClauseOperator, bool nullable)
        {
            IDateTimePicker dtPicker =
                _filterControlManager.AddDateFilterDateTimePicker(labelText, propertyName, filterClauseOperator, nullable,
                                                                  defaultValue);
            return dtPicker;
        }

        /// <summary>
        /// Returns the filter clause as a composite of all the specific
        /// clauses in each filter control in the set
        /// </summary>
        /// <returns>Returns the filter clause</returns>
        public IFilterClause GetFilterClause()
        {
            //if (FilterMode == FilterModes.Search)
            //{
            //    return _filterControlManager.GetFilterClause();
            //}
            return _filterControlManager.GetFilterClause();
        }

        /// <summary>
        /// Returns the filter button that when clicked applies the filter
        /// </summary>
        public IButton FilterButton
        {
            get { return _filterButton; }
        }

        /// <summary>
        /// Gets and sets the FilterMode <see cref="FilterModes"/>, which determines the
        /// behaviour of the filter control
        /// </summary>
        public FilterModes FilterMode
        {
            get { return _filterMode; }
            set
            {
                _filterMode = value;
                if (_filterMode == FilterModes.Filter)
                {
                    _filterButton.Text = "Filter";
                    _gbox.Text = "Filter the Grid";
                }
                else
                {
                    _filterButton.Text = "Search";
                    _gbox.Text = "Search the Grid";
                }
            }
        }

        /// <summary>
        /// Gets the collection of individual filters
        /// </summary>
        public IList FilterControls
        {
            get { return _filterControlManager.FilterControls; }
        }

        public IControlHabanero GetChildControl(string propertyName)
        {
            return this._filterControlManager.GetChildControl(propertyName);
        }

        /// <summary>
        /// Returns the clear button that when clicked clears the filter
        /// </summary>
        public IButton ClearButton
        {
            get { return _clearButton; }
        }

        /// <summary>
        /// Gets the collection of controls contained within the control
        /// </summary>
        IControlCollection IControlHabanero.Controls
        {
            get { return new ControlCollectionVWG(base.Controls); }
        }

        /// <summary>
        /// Clears all the values from the filter and calls <see cref="IFilterControl.ApplyFilter"/>
        /// </summary>
        public void ClearFilters()
        {
            _filterControlManager.ClearFilters();
            FireFilterEvent();
        }

        /// <summary>
        /// Returns the layout manager used to lay the controls out on the filter control panel.
        /// The default layout manager is the FlowLayoutManager.
        /// </summary>
        public LayoutManager LayoutManager
        {
            get { return _filterControlManager.LayoutManager; }
            set { _filterControlManager.LayoutManager = value; }
        }

        /// <summary>
        /// Returns the panel onto which the filter controls will be placed
        /// </summary>
        public IPanel FilterPanel
        {
            get { return _controlPanel; }
        }

        /// <summary>
        /// Adds a DateRangeComboBox filter which provides common date ranges such as "Today" or "This Year",
        /// so that the grid will only show rows having a date property in the given range
        /// </summary>
        /// <param name="labelText">The label to appear before the control</param>
        /// <param name="columnName">The business object property on which to filter</param>
        /// <param name="includeStartDate">Includes all dates that match the start date exactly</param>
        /// <param name="includeEndDate">Includes all dates that match the end date exactly</param>
        /// <returns>Returns the new DateRangeComboBox added</returns>
        public IDateRangeComboBox AddDateRangeFilterComboBox(string labelText, string columnName, bool includeStartDate, bool includeEndDate)
        {
            return AddDateRangeFilterComboBox(labelText, columnName, null, includeStartDate, includeEndDate);

        }

        /// <summary>
        /// Adds a DateRangeComboBox filter which provides common date ranges such as "Today" or "This Year",
        /// so that the grid will only show rows having a date property in the given range
        /// </summary>
        /// <param name="labelText">The label to appear before the control</param>
        /// <param name="columnName">The business object property on which to filter</param>
        /// <param name="options">Provides a specific set of date range options to show</param>
        /// <param name="includeStartDate">Includes all dates that match the start date exactly</param>
        /// <param name="includeEndDate">Includes all dates that match the end date exactly</param>
        /// <returns>Returns the new DateRangeComboBox added</returns>
        public IDateRangeComboBox AddDateRangeFilterComboBox(string labelText, string columnName, List<DateRangeOptions> options, bool includeStartDate, bool includeEndDate)
        {
            return _filterControlManager.AddDateRangeFilterComboBox(labelText, columnName, options, includeStartDate,
                                                                 includeEndDate);
        }

        public IControlHabanero AddCustomFilter(string labelText,string propertyName, FilterControlManager.ICustomFilter customFilter)
        {
            return _filterControlManager.AddCustomFilter(labelText, propertyName, customFilter);
        }
    }
}