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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Habanero.UI.Base;

namespace Habanero.UI.Win
{
    /// <summary>
    /// Provides a multiselector control. The type to be displayed in the 
    /// lists is set by the template type.  The multiselector helps the user to
    /// select from an available list of options.  Unselected options appear on the
    /// left and selected ones appear on the right.  The AvailableOptions consists
    /// of all options, both selected and unselected - no object may appear in the
    /// selected list if it is not also in the AvailableOptions list.  All list
    /// control is managed through the Model object.
    /// </summary>
    public partial class MultiSelectorWin<T> : UserControlWin, IMultiSelector<T>
    {
        private readonly MultiSelectorManager<T> _manager;

        public MultiSelectorWin()
        {
            InitializeComponent();
            _manager = new MultiSelectorManager<T>(this);
            AvailableOptionsListBox.SelectedIndexChanged += delegate
            {
                GetButton(MultiSelectorButton.Select).Enabled = (AvailableOptionsListBox.SelectedIndex != -1);
            };

            SelectedOptionsListBox.SelectedIndexChanged += delegate
            {
                GetButton(MultiSelectorButton.Deselect).Enabled = (SelectedOptionsListBox.SelectedIndex != -1);
            };

            SetDoubleClickEventHandlers();
        }

        /// <summary>
        /// Gets and sets the complete list of options available to go in
        /// either panel
        /// </summary>
        public List<T> AllOptions
        {
            get { return _manager.AllOptions; }
            set
            {
                _manager.AllOptions = value;
                GetButton(MultiSelectorButton.Select).Enabled = false;
            }
        }

        /// <summary>
        /// Gets the ListBox control that contains the available options that
        /// have not been selected
        /// </summary>
        public IListBox AvailableOptionsListBox
        {
            get { return _availableOptionsListbox; }
        }

        /// <summary>
        /// Gets the model that manages the options available or selected
        /// </summary>
        public MultiSelectorModel<T> Model
        {
            get { return _manager.Model; }
        }

        ///<summary>
        /// Gets or sets the list of items already selected (which is a subset of
        /// all available options).  This list typically appears on the right-hand side.
        ///</summary>
        public List<T> SelectedOptions
        {
            get { return _manager.SelectedOptions; }
            set
            {
                _manager.SelectedOptions = value;
                GetButton(MultiSelectorButton.Deselect).Enabled = false;
            }
        }

        /// <summary>
        /// Gets the ListBox control that contains the options that have been
        /// selected from those available
        /// </summary>
        public IListBox SelectedOptionsListBox
        {
            get { return _selectionsListbox; }
        }

        /// <summary>
        /// Gets the button control as indicated by the <see cref="MultiSelectorButton"/> enumeration.
        /// </summary>
        /// <param name="buttonType">The type of button</param>
        /// <returns>Returns a button</returns>
        public IButton GetButton(MultiSelectorButton buttonType)
        {
            switch (buttonType)
            {
                case MultiSelectorButton.Select:
                    return _btnSelect;
              
                case MultiSelectorButton.Deselect:
                    return _btnDeselect;
                case MultiSelectorButton.SelectAll:
                    return _btnSelectAll;
                case MultiSelectorButton.DeselectAll:
                    return _btnDeselectAll;
                default:
                    throw new ArgumentOutOfRangeException("buttonType");
            }
        }

        /// <summary>
        /// Gets a view of the SelectedOptions collection
        /// </summary>
        public ReadOnlyCollection<T> SelectionsView
        {
            //TODO Port: Fix and test this for windows.
            get { return this._manager.SelectionsView; }
        }

        private void SetDoubleClickEventHandlers()
        {
            AvailableOptionsListBox.DoubleClick += _manager.DoSelect;
            SelectedOptionsListBox.DoubleClick += _manager.DoDeselect;
        }
    }
}
