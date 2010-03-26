// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2010 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
using Habanero.UI.Base;
using Habanero.UI.VWG;
using Habanero.UI.Win;
using NUnit.Framework;

namespace Habanero.Test.UI.Base
{
    /// <summary>
    /// This test class tests the base inherited methods of the TextBox class.
    /// </summary>
    [TestFixture]
    public class TestBaseMethodsWin_TextBox : TestBaseMethods.TestBaseMethodsWin
    {
        protected override IControlHabanero CreateControl()
        {
            return GetControlFactory().CreateTextBox();
        }
    }

    /// <summary>
    /// This test class tests the base inherited methods of the TextBox class.
    /// </summary>
    [TestFixture]
    public class TestBaseMethodsVWG_TextBox : TestBaseMethods.TestBaseMethodsVWG
    {
        protected override IControlHabanero CreateControl()
        {
            return GetControlFactory().CreateTextBox();
        }
    }

    /// <summary>
    /// This test class tests the TextBox class.
    /// </summary>
    public abstract class TestTextBox
    {
        protected abstract IControlFactory GetControlFactory();

        [TestFixture]
        public class TestTextBoxWin : TestTextBox
        {
            protected override IControlFactory GetControlFactory()
            {
                return new ControlFactoryWin();
            }

            [Test]
            public void TestScrollBars_Vertical()
            {
                //---------------Set up test pack-------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Execute Test ----------------------
                textBox.ScrollBars = ScrollBars.Vertical;
                //---------------Test Result -----------------------
                Assert.AreEqual((int)System.Windows.Forms.ScrollBars.Vertical, (int)textBox.ScrollBars);
                Assert.AreEqual(ScrollBars.Vertical, textBox.ScrollBars);
                //---------------Tear Down -------------------------
            }
            [Test]
            public void TestScrollBars_Horizontal()
            {
                //---------------Set up test pack-------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Execute Test ----------------------
                textBox.ScrollBars = ScrollBars.Horizontal;
                //---------------Test Result -----------------------
                Assert.AreEqual((int)System.Windows.Forms.ScrollBars.Horizontal, (int)textBox.ScrollBars);
                Assert.AreEqual(ScrollBars.Horizontal, textBox.ScrollBars);
                //---------------Tear Down -------------------------
            }
            [Test]
            public void TestScrollBars_None()
            {
                //---------------Set up test pack-------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Execute Test ----------------------
                textBox.ScrollBars = ScrollBars.None;
                //---------------Test Result -----------------------
                Assert.AreEqual((int)System.Windows.Forms.ScrollBars.None, (int)textBox.ScrollBars);
                Assert.AreEqual(ScrollBars.None, textBox.ScrollBars);
                //---------------Tear Down -------------------------
            }
            [Test]
            public void TestScrollBars_Both()
            {
                //---------------Set up test pack-------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Execute Test ----------------------
                textBox.ScrollBars = ScrollBars.Both;
                //---------------Test Result -----------------------
                Assert.AreEqual((int)System.Windows.Forms.ScrollBars.Both, (int)textBox.ScrollBars);
                Assert.AreEqual(ScrollBars.Both, textBox.ScrollBars);
                //---------------Tear Down -------------------------
            }

        }

        [TestFixture]
        public class TestTextBoxVWG : TestTextBox
        {
            protected override IControlFactory GetControlFactory()
            {
                return new ControlFactoryVWG();
            }
            [Test]
            public void TestScrollBars_Vertical()
            {
                //---------------Set up test pack-------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Execute Test ----------------------
                textBox.ScrollBars = ScrollBars.Vertical;
                //---------------Test Result -----------------------
                Assert.AreEqual((int)Gizmox.WebGUI.Forms.ScrollBars.Vertical, (int)textBox.ScrollBars);
                Assert.AreEqual(ScrollBars.Vertical, textBox.ScrollBars);
                //---------------Tear Down -------------------------
            }
            [Test]
            public void TestScrollBars_Horizontal()
            {
                //---------------Set up test pack-------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Execute Test ----------------------
                textBox.ScrollBars = ScrollBars.Horizontal;
                //---------------Test Result -----------------------
                Assert.AreEqual((int)Gizmox.WebGUI.Forms.ScrollBars.Horizontal, (int)textBox.ScrollBars);
                Assert.AreEqual(ScrollBars.Horizontal, textBox.ScrollBars);
                //---------------Tear Down -------------------------
            }
            [Test]
            public void TestScrollBars_None()
            {
                //---------------Set up test pack-------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Execute Test ----------------------
                textBox.ScrollBars = ScrollBars.None;
                //---------------Test Result -----------------------
                Assert.AreEqual((int)Gizmox.WebGUI.Forms.ScrollBars.None, (int)textBox.ScrollBars);
                Assert.AreEqual(ScrollBars.None, textBox.ScrollBars);
                //---------------Tear Down -------------------------
            }
            [Test]
            public void TestScrollBars_Both()
            {
                //---------------Set up test pack-------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Execute Test ----------------------
                textBox.ScrollBars = ScrollBars.Both;
                //---------------Test Result -----------------------
                Assert.AreEqual((int)Gizmox.WebGUI.Forms.ScrollBars.Both, (int)textBox.ScrollBars);
                Assert.AreEqual(ScrollBars.Both, textBox.ScrollBars);
                //---------------Tear Down -------------------------
            }

            [Test]
            public void Test_defaultTextAlignment()
            {
                //---------------Set up test pack-------------------
               
                //---------------Execute Test ----------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Test Result -----------------------
                Assert.AreEqual(HorizontalAlignment.Left, textBox.TextAlign);
                //---------------Tear Down -------------------------
            }

            [Test]
            public void Test_setTextAlignment_Left()
            {
                //---------------Set up test pack-------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Execute Test ----------------------
                textBox.TextAlign = HorizontalAlignment.Left;
                //---------------Test Result -----------------------
                Assert.AreEqual(HorizontalAlignment.Left, textBox.TextAlign);
                //---------------Tear Down -------------------------
            }

            [Test]
            public void Test_setTextAlignment_Center()
            {
                //---------------Set up test pack-------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Execute Test ----------------------
                textBox.TextAlign = HorizontalAlignment.Center;
                //---------------Test Result -----------------------
                Assert.AreEqual(HorizontalAlignment.Center, textBox.TextAlign);
                //---------------Tear Down -------------------------
            }

            [Test]
            public void Test_setTextAlignment_Right()
            {
                //---------------Set up test pack-------------------
                ITextBox textBox = GetControlFactory().CreateTextBox();
                //---------------Execute Test ----------------------
                textBox.TextAlign = HorizontalAlignment.Right;
                //---------------Test Result -----------------------
                Assert.AreEqual(HorizontalAlignment.Right,textBox.TextAlign);
                //---------------Tear Down -------------------------
            }
        }

   

    }
}
