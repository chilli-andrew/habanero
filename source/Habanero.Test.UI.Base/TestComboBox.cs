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
using Habanero.UI.Base;
using Habanero.UI.VWG;
using Habanero.UI.Win;
using NUnit.Framework;

namespace Habanero.Test.UI.Base
{
    /// <summary>
    /// This test class tests the base inherited methods of the ComboBox class.
    /// </summary>
    public class TestBaseMethodsWin_ComboBox : TestBaseMethods.TestBaseMethodsWin
    {
        [STAThread]
        protected override IControlHabanero CreateControl()
        {
            return GetControlFactory().CreateComboBox();
        }
    }

    /// <summary>
    /// This test class tests the base inherited methods of the ComboBox class.
    /// </summary>
    public class TestBaseMethodsVWG_ComboBox : TestBaseMethods.TestBaseMethodsVWG
    {
        protected override IControlHabanero CreateControl()
        {
            return GetControlFactory().CreateComboBox();
        }
    }

    /// <summary>
    /// This test class tests the ComboBox class.
    /// </summary>
    [TestFixture]
    public abstract class TestComboBox
    {
        
        protected IComboBox CreateComboBox()
        {
            return GetControlFactory().CreateComboBox();
        }
        
        protected abstract IControlFactory GetControlFactory();

        protected abstract string GetUnderlyingAutoCompleteSourceToString(IComboBox controlHabanero);

        protected void AssertAutoCompleteSourcesSame(IComboBox comboBox)
        {
            AutoCompleteSource AutoCompleteSource = comboBox.AutoCompleteSource;
            string AutoCompleteSourceToString = GetUnderlyingAutoCompleteSourceToString(comboBox);
            Assert.AreEqual(AutoCompleteSource.ToString(), AutoCompleteSourceToString);
        }

        public class TestComboBoxWin : TestComboBox
        {
            protected override IControlFactory GetControlFactory()
            {
                return new ControlFactoryWin();
            }

            protected override string GetUnderlyingAutoCompleteSourceToString(IComboBox controlHabanero)
            {
                System.Windows.Forms.ComboBox control = (System.Windows.Forms.ComboBox)controlHabanero;
                return control.AutoCompleteSource.ToString();
            }

            [Ignore("Need to figure out how to run tests in STAThread for this test")]
            public override void TestConversion_AutoCompleteSource_None()
            {
                base.TestConversion_AutoCompleteSource_None();
            }

            [Ignore("Need to figure out how to run tests in STAThread for this test")]
            public override void TestConversion_AutoCompleteSource_AllSystemSources()
            {
                base.TestConversion_AutoCompleteSource_AllSystemSources();
            }

            [Ignore("Need to figure out how to run tests in STAThread for this test")]
            public override void TestConversion_AutoCompleteSource_AllUrl()
            {
                base.TestConversion_AutoCompleteSource_AllUrl();
            }

            [Ignore("Need to figure out how to run tests in STAThread for this test")]
            public override void TestConversion_AutoCompleteSource_CustomSource()
            {
                base.TestConversion_AutoCompleteSource_CustomSource();
            }

            [Ignore("Need to figure out how to run tests in STAThread for this test")]
            public override void TestConversion_AutoCompleteSource_FileSystem()
            {
                base.TestConversion_AutoCompleteSource_FileSystem();
            }

            [Ignore("Need to figure out how to run tests in STAThread for this test")]
            public override void TestConversion_AutoCompleteSource_FileSystemDirectories()
            {
                base.TestConversion_AutoCompleteSource_FileSystemDirectories();
            }

            [Ignore("Need to figure out how to run tests in STAThread for this test")]
            public override void TestConversion_AutoCompleteSource_HistoryList()
            {
                base.TestConversion_AutoCompleteSource_HistoryList();
            }

            [Ignore("Need to figure out how to run tests in STAThread for this test")]
            public override void TestConversion_AutoCompleteSource_ListItems()
            {
                base.TestConversion_AutoCompleteSource_ListItems();
            }

            [Ignore("Need to figure out how to run tests in STAThread for this test")]
            public override void TestConversion_AutoCompleteSource_RecentlyUsedList()
            {
                base.TestConversion_AutoCompleteSource_RecentlyUsedList();
            }
        }

        public class TestComboBoxVWG : TestComboBox
        {
            protected override IControlFactory GetControlFactory()
            {
                return new ControlFactoryVWG();
            }

            protected override string GetUnderlyingAutoCompleteSourceToString(IComboBox controlHabanero)
            {
                Gizmox.WebGUI.Forms.ComboBox control = (Gizmox.WebGUI.Forms.ComboBox)controlHabanero;
                return control.AutoCompleteSource.ToString();
            }
        }


        [Test]
        public virtual void TestConversion_AutoCompleteSource_None()
        {
            //---------------Set up test pack-------------------
            IComboBox control = CreateComboBox();
            //-------------Assert Preconditions -------------
            //---------------Execute Test ----------------------
            control.AutoCompleteSource = AutoCompleteSource.None;
            //---------------Test Result -----------------------
            Assert.AreEqual(AutoCompleteSource.None, control.AutoCompleteSource);
            AssertAutoCompleteSourcesSame(control);
        }

        [Test]
        public virtual void TestConversion_AutoCompleteSource_AllSystemSources()
        {
            //---------------Set up test pack-------------------
            IComboBox control = CreateComboBox();
            //-------------Assert Preconditions -------------
            //---------------Execute Test ----------------------
            control.AutoCompleteSource = AutoCompleteSource.AllSystemSources;
            //---------------Test Result -----------------------
            Assert.AreEqual(AutoCompleteSource.AllSystemSources, control.AutoCompleteSource);
            AssertAutoCompleteSourcesSame(control);
        }

        [Test]
        public virtual void TestConversion_AutoCompleteSource_AllUrl()
        {
            //---------------Set up test pack-------------------
            IComboBox control = CreateComboBox();
            //-------------Assert Preconditions -------------
            //---------------Execute Test ----------------------
            control.AutoCompleteSource = AutoCompleteSource.AllUrl;
            //---------------Test Result -----------------------
            Assert.AreEqual(AutoCompleteSource.AllUrl, control.AutoCompleteSource);
            AssertAutoCompleteSourcesSame(control);
        }

        [Test]
        public virtual void TestConversion_AutoCompleteSource_CustomSource()
        {
            //---------------Set up test pack-------------------
            IComboBox control = CreateComboBox();
            //-------------Assert Preconditions -------------
            //---------------Execute Test ----------------------
            control.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //---------------Test Result -----------------------
            Assert.AreEqual(AutoCompleteSource.CustomSource, control.AutoCompleteSource);
            AssertAutoCompleteSourcesSame(control);
        }

        [Test]
        public virtual void TestConversion_AutoCompleteSource_FileSystem()
        {
            //---------------Set up test pack-------------------
            IComboBox control = CreateComboBox();
            //-------------Assert Preconditions -------------
            //---------------Execute Test ----------------------
            control.AutoCompleteSource = AutoCompleteSource.FileSystem;
            //---------------Test Result -----------------------
            Assert.AreEqual(AutoCompleteSource.FileSystem, control.AutoCompleteSource);
            AssertAutoCompleteSourcesSame(control);
        }

        [Test]
        public virtual void TestConversion_AutoCompleteSource_FileSystemDirectories()
        {
            //---------------Set up test pack-------------------
            IComboBox control = CreateComboBox();
            //-------------Assert Preconditions -------------
            //---------------Execute Test ----------------------
            control.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;
            //---------------Test Result -----------------------
            Assert.AreEqual(AutoCompleteSource.FileSystemDirectories, control.AutoCompleteSource);
            AssertAutoCompleteSourcesSame(control);
        }

        [Test]
        public virtual void TestConversion_AutoCompleteSource_HistoryList()
        {
            //---------------Set up test pack-------------------
            IComboBox control = CreateComboBox();
            //-------------Assert Preconditions -------------
            //---------------Execute Test ----------------------
            control.AutoCompleteSource = AutoCompleteSource.HistoryList;
            //---------------Test Result -----------------------
            Assert.AreEqual(AutoCompleteSource.HistoryList, control.AutoCompleteSource);
            AssertAutoCompleteSourcesSame(control);
        }

        [Test]
        public virtual void TestConversion_AutoCompleteSource_ListItems()
        {
            //---------------Set up test pack-------------------
            IComboBox control = CreateComboBox();
            //-------------Assert Preconditions -------------
            //---------------Execute Test ----------------------
            control.AutoCompleteSource = AutoCompleteSource.ListItems;
            //---------------Test Result -----------------------
            Assert.AreEqual(AutoCompleteSource.ListItems, control.AutoCompleteSource);
            AssertAutoCompleteSourcesSame(control);
        }

        [Test]
        public virtual void TestConversion_AutoCompleteSource_RecentlyUsedList()
        {
            //---------------Set up test pack-------------------
            IComboBox control = CreateComboBox();
            //-------------Assert Preconditions -------------
            //---------------Execute Test ----------------------
            control.AutoCompleteSource = AutoCompleteSource.RecentlyUsedList;
            //---------------Test Result -----------------------
            Assert.AreEqual(AutoCompleteSource.RecentlyUsedList, control.AutoCompleteSource);
            AssertAutoCompleteSourcesSame(control);
        }

    }
}