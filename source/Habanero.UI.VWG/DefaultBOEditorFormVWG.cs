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
using System.Reflection;
using Gizmox.WebGUI.Forms;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.UI.Base;
using log4net;


namespace Habanero.UI.VWG
{
    /// <summary>
    /// Provides a form used to edit business objects.  This form will usually
    /// be constructed using a UI Form definition provided in the class definitions.
    /// The appropriate UI definition is typically set in the constructor.
    /// </summary>
    public class DefaultBOEditorFormVWG : FormVWG, IDefaultBOEditorForm
    {
        private readonly PostObjectPersistingDelegate _action;
        private static readonly ILog log = LogManager.GetLogger("Habanero.UI.Forms.DefaultBOEditorFormVWG");
        private readonly string _uiDefName;
        private readonly IButtonGroupControl _buttons;
        protected BusinessObject _bo;
        private readonly IControlFactory _controlFactory;
        private readonly IPanel _boPanel;
        protected IPanelFactoryInfo _panelFactoryInfo;

        public DefaultBOEditorFormVWG(BusinessObject bo, string name, IControlFactory controlFactory, PostObjectPersistingDelegate action):this(bo, name, controlFactory)
        {
            _action = action;
            
        }

        public DefaultBOEditorFormVWG(BusinessObject bo, string uiDefName, IControlFactory controlFactory)
        {
            _bo = bo;
            _controlFactory = controlFactory;
            _uiDefName = uiDefName;

            BOMapper mapper = new BOMapper(bo);

            UIForm def;
            if (_uiDefName.Length > 0)
            {
                UIDef uiMapper = mapper.GetUIDef(_uiDefName);
                if (uiMapper == null)
                {
                    throw new NullReferenceException("An error occurred while " +
                                                     "attempting to load an object editing form.  A possible " +
                                                     "cause is that the class definitions do not have a " +
                                                     "'form' section for the class, under the 'ui' " +
                                                     "with the name '" + _uiDefName + "'.");
                }
                def = uiMapper.GetUIFormProperties();
            }
            else
            {
                UIDef uiMapper = mapper.GetUIDef();
                if (uiMapper == null)
                {
                    throw new NullReferenceException("An error occurred while " +
                                                     "attempting to load an object editing form.  A possible " +
                                                     "cause is that the class definitions do not have a " +
                                                     "'form' section for the class.");
                }
                def = uiMapper.GetUIFormProperties();
            }
            if (def == null)
            {
                throw new NullReferenceException("An error occurred while " +
                                                 "attempting to load an object editing form.  A possible " +
                                                 "cause is that the class definitions do not have a " +
                                                 "'form' section for the class.");
            }

            IPanelFactory factory = new PanelFactory(_bo, def, _controlFactory);
            _panelFactoryInfo = factory.CreatePanel();
            _boPanel = _panelFactoryInfo.Panel;
            _buttons = _controlFactory.CreateButtonGroupControl();
            _buttons.AddButton("&Cancel", CancelButtonHandler);
            IButton okbutton = _buttons.AddButton("&OK", OKButtonHandler);
            okbutton.NotifyDefault(true);
            this.AcceptButton = (ButtonVWG)okbutton;
            this.Load += delegate { FocusOnFirstControl(); };

            Text = def.Title;
            SetupFormSize(def);
            MinimizeBox = false;
            MaximizeBox = false;
            //this.ControlBox = false;

            CreateLayout();
            OnResize(new EventArgs());
        }

        private void FocusOnFirstControl()
        {
            IControlHabanero controlToFocus = _panelFactoryInfo.FirstControlToFocus;
            MethodInfo focusMethod = controlToFocus.GetType().
                GetMethod("Focus", BindingFlags.Instance | BindingFlags.Public);
            if (focusMethod != null)
            {
                focusMethod.Invoke(controlToFocus, new object[] { });
            }
        }

        //private void DefaultBOEditorForm_Load(object sender, EventArgs e)
        //{
        //    if (_panelFactoryInfo.ControlMappers.BusinessObject == null && _bo != null)
        //    {
        //        _panelFactoryInfo.ControlMappers.BusinessObject = _bo;
        //    }
        //}

        protected virtual void SetupFormSize(UIForm def)
        {
            int width = def.Width;
            int minWidth = _boPanel.Width +
                           Margin.Left + Margin.Right;
            if (width < minWidth)
            {
                width = minWidth;
            }
            int height = def.Height;
            int minHeight = _boPanel.Height + _buttons.Height +
                            Margin.Top + Margin.Bottom;
            if (height < minHeight)
            {
                height = minHeight;
            }
            Height = height;
            Width = width;
        }

        ///// <summary>
        ///// Constructor as before, but sets the uiDefName to an empty string,
        ///// which uses the ui definition without a specified name attribute
        ///// </summary>
        ///// <param name="bo">The business object to represent</param>
        //public DefaultBOEditorFormVWG(BusinessObject bo)
        //    : this(bo, "", null)
        //{
        //}

        /// <summary>
        /// Returns the panel object being managed
        /// </summary>
        protected IPanel BoPanel
        {
            get { return _boPanel; }
        }

        /// <summary>
        /// Sets up the layout of the panel and buttons
        /// </summary>
        protected virtual void CreateLayout()
        {
            BorderLayoutManager borderLayoutManager = new BorderLayoutManagerVWG(this, _controlFactory);
            borderLayoutManager.AddControl(BoPanel, BorderLayoutManager.Position.Centre);
            borderLayoutManager.AddControl(Buttons, BorderLayoutManager.Position.South);
        }

        /// <summary>
        /// A handler to respond when the "Cancel" button has been pressed.
        /// Any unsaved edits are cancelled and the dialog is closed.
        /// </summary>
        /// <param name="sender">The object that notified of the event</param>
        /// <param name="e">Attached arguments regarding the event</param>
        private void CancelButtonHandler(object sender, EventArgs e)
        {
            _panelFactoryInfo.ControlMappers.BusinessObject = null;
            _bo.Restore();
            //DialogResult = Gizmox.WebGUI.Forms.DialogResult.Cancel;
            DialogResult = Base.DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// A handler to respond when the "OK" button has been pressed.
        /// All changes are committed to the database and the dialog is closed.
        /// </summary>
        /// <param name="sender">The object that notified of the event</param>
        /// <param name="e">Attached arguments regarding the event</param>
        private void OKButtonHandler(object sender, EventArgs e)
        {
            try
            {
                _panelFactoryInfo.ControlMappers.ApplyChangesToBusinessObject();
                TransactionCommitter committer = CreateSaveTransaction();
                committer.CommitTransaction();


                //TODO_Port: Removed by peter
                //if (_boPanel.Controls[0] is TabControl)
                //{
                //    //Console.Out.WriteLine("tabcontrol found.");
                //    TabControl tabControl = (TabControl)_boPanel.Controls[0];
                //    foreach (TabPage page in tabControl.TabPages)
                //    {
                //        foreach (Panel panel in page.Controls)
                //        {
                //            foreach (Control control in panel.Controls)
                //            {
                //                //Console.Out.WriteLine(control.GetType().Name);
                //                if (control is EditableGrid)
                //                {
                //                    //Console.Out.WriteLine("EditableGrid found.");
                //                    ((EditableGrid)control).SaveChanges();
                //                }
                //            }
                //        }
                //    }
                //}

                DialogResult = Base.DialogResult.OK;
                Close();
                if (_action != null)
                {
                    _action(this._bo);
                }
                _panelFactoryInfo.ControlMappers.BusinessObject = null;
            }
            catch (Exception ex)
            {
                log.Error(ExceptionUtilities.GetExceptionString(ex, 0, true));
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "There was a problem saving for the following reason(s):",
                                                          "Saving Problem");
            }
        }

        /// <summary>
        /// Returns a transaction object, preparing the database connection and
        /// specifying which object to update
        /// </summary>
        /// <returns>Returns the transaction object</returns>
        protected virtual TransactionCommitter CreateSaveTransaction()
        {
            TransactionCommitterDB committer = new TransactionCommitterDB();
            committer.AddBusinessObject(_bo);
            return committer;
        }

        /// <summary>
        /// Gets the button control for the buttons in the form
        /// </summary>
        public IButtonGroupControl Buttons
        {
            get { return _buttons; }
        }

        /// <summary>
        /// Gets or sets the dialog result that indicates what action was
        /// taken to close the form
        /// </summary>
        public Base.DialogResult DialogResult
        {
            get { return (Base.DialogResult) base.DialogResult; }
            set { base.DialogResult = (Gizmox.WebGUI.Forms.DialogResult) value; }
        }

        /// <summary>
        /// Gets the object containing all information related to the form, including
        /// its controls, mappers and business object
        /// </summary>
        public IPanelFactoryInfo PanelFactoryInfo
        {
            get { return _panelFactoryInfo; }
        }

        /// <summary>
        /// Gets the collection of controls contained within the control
        /// </summary>
        IControlCollection IControlHabanero.Controls
        {
            get { return new ControlCollectionVWG(this.Controls); }
        }

        /// <summary>
        /// Forces the form to invalidate its client area and
        /// immediately redraw itself and any child controls
        /// </summary>
        public void Refresh()
        {
            // do nothing
        }

        /// <summary>
        /// Gets or sets the current multiple document interface (MDI) parent form of this form
        /// </summary>
        IFormHabanero IFormHabanero.MdiParent
        {
            get { throw new NotImplementedException(); }
            set { this.MdiParent = (Form)value; }
        }

        /// <summary>
        /// Pops the form up in a modal dialog.  If the BO is successfully edited and saved, returns true,
        /// else returns false.
        /// </summary>
        /// <returns>True if the edit was a success, false if not</returns>
        bool IDefaultBOEditorForm.ShowDialog()
        {
            if (this.ShowDialog() == (Gizmox.WebGUI.Forms.DialogResult) Base.DialogResult.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}