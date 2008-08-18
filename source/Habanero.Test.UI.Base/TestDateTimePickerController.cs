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
using System.Windows.Forms;
using Habanero.UI;
using Habanero.UI.Base;
using Habanero.UI.WebGUI;
using Habanero.UI.Win;
using NUnit.Framework;


namespace Habanero.Test.UI.Base
{
    public abstract class TestDateTimePickerController //:TestBase
    {
        protected abstract IControlFactory GetControlFactory();

        //[TestFixture]
        //public class TestDateTimePickerControllerWin : TestDateTimePickerController
        //{
        //    protected override IControlFactory GetControlFactory()
        //    {
        //        return new ControlFactoryWin();
        //    }

            
        //}


        //public class TestDateTimePickerControllerGiz : TestDateTimePickerController
        //{
        //    protected override IControlFactory GetControlFactory()
        //    {
        //        return new ControlFactoryGizmox();
        //    }
        //    //There are not tests for Giz since this functionality is very specific to windows
        //}

        [SetUp]
        public void SetupTest()
        {
            //Runs every time that any testmethod is executed
            //base.SetupTest();
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.
        }

        [TearDown]
        public void TearDownTest()
        {
            //runs every time any testmethod is complete
            //base.TearDownTest();
        }

        private IDateTimePicker CreateDateTimePicker()
        {
            return GetControlFactory().CreateDateTimePicker();
        }

//        private DateTimePickerController GetDateTimePickerController(IDateTimePicker dateTimePicker)
//        {
////            return new DateTimePickerController(GetControlFactory(), dateTimePicker);
//        }

        //-----------------------------------TO BE MOVED TO GIZ AS WELL----------------------------
        

        

        //[Test]
        //public void TestCreateDateTimePickerController()
        //{
        //    //---------------Set up test pack-------------------
        //    IDateTimePicker dateTimePicker = CreateDateTimePicker();
        //    //---------------Execute Test ----------------------
        //    DateTimePickerController dateTimePickerController = GetDateTimePickerController(dateTimePicker);

        //    //---------------Test Result -----------------------
        //    Assert.IsNotNull(dateTimePickerController);
        //    Assert.IsNotNull(dateTimePickerController.DateTimePicker);
        //    //Assert.IsTrue(dateTimePicker.Controls.Count > 0);
        //    //---------------Tear Down   -----------------------
        //}



        


        //[Test]
        //public void TestSetDateTimePickerValueViaController()
        //{
        //    IDateTimePicker dateTimePicker = CreateDateTimePicker();
        //    DateTimePickerController dateTimePickerController = GetDateTimePickerController(dateTimePicker);
        //    DateTime testDate = new DateTime(2007, 01, 01, 01, 01, 01);
        //    //---------------Execute Test ----------------------
        //    dateTimePickerController.Value = testDate;

        //    //---------------Test Result -----------------------
        //    Assert.AreEqual(testDate, dateTimePicker.Value);
        //    Assert.AreEqual(dateTimePickerController.Value, dateTimePicker.Value);
        //    //---------------Tear Down -------------------------          
        //}

        //-----------------------------------For Windows Only----------------------------
        

        //[Test]
        //public void TestSetPickerControllerValue()
        //{
        //    //---------------Set up test pack-------------------
        //    IDateTimePicker dateTimePicker = CreateDateTimePicker();
        //    DateTimePickerController dateTimePickerController = GetDateTimePickerController(dateTimePicker);
        //    DateTime sampleDate = new DateTime(2001, 01, 01, 01, 01, 01);
        //    //---------------Execute Test ----------------------
        //    dateTimePicker.Value = sampleDate;
        //    //---------------Test Result -----------------------
        //    Assert.AreEqual(sampleDate, dateTimePickerController.Value);
        //    IControlChilli nullDisplayControl = dateTimePicker.Controls[0];
        //    Assert.IsFalse(nullDisplayControl.Visible,
        //                   "Null display value control should not be visible when there is a value.");
        //}


        

        //[Test]
        //public void TestSetNullThenControllerValue()
        //{
        //    TestSetNullValue();
        //    TestSetControllerValue();
        //}

        

        

        //[Test]
        //public void TestSetControllerValueFiresEvent()
        //{
        //    IDateTimePicker dateTimePicker = CreateDateTimePicker();
        //    DateTimePickerController dateTimePickerController = GetDateTimePickerController(dateTimePicker);
        //    DateTime sampleDate = new DateTime(2003, 03, 03, 03, 03, 03);
        //    bool isFired = false;
        //    EventHandler handleValueChanged = delegate { isFired = true; };
        //    dateTimePickerController.ValueChanged += handleValueChanged;
        //    dateTimePickerController.Value = sampleDate;
        //    Assert.IsTrue(isFired, "The ValueChanged event should have fired after setting the value.");
        //    isFired = false;
        //    dateTimePickerController.Value = null;
        //    Assert.IsTrue(isFired, "The ValueChanged event should have fired after setting the value to null.");
        //    dateTimePickerController.ValueChanged -= handleValueChanged;
        //}

        //[Test]
        //public void TestSetPickerValueFiresEvent()
        //{
        //    IDateTimePicker dateTimePicker = CreateDateTimePicker();
        //    DateTimePickerController dateTimePickerController = GetDateTimePickerController(dateTimePicker);
        //    DateTime sampleDate = new DateTime(2004, 04, 04, 04, 04, 04);
        //    bool isFired = false;
        //    EventHandler handleValueChanged = delegate { isFired = true; };
        //    dateTimePickerController.ValueChanged += handleValueChanged;
        //    dateTimePicker.Value = sampleDate;
        //    Assert.IsTrue(isFired, "The ValueChanged event should have fired after setting the value.");
        //    dateTimePickerController.ValueChanged -= handleValueChanged;
        //}

    }
}