//---------------------------------------------------------------------------------
// Copyright (C) 2007 Chillisoft Solutions
// 
// This file is part of Habanero Standard.
// 
//     Habanero Standard is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Habanero Standard is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with Habanero Standard.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using Habanero.BO;
using Habanero.Base;
using Habanero.Util;
using NUnit.Framework;
using BusinessObject=Habanero.BO.BusinessObject;

namespace Habanero.Test.BO.Loaders
{
    /// <summary>
    /// Summary description for TestXmlLoader.
    /// </summary>
    [TestFixture]
    public class TestXmlClassLoader
    {
        private XmlClassLoader loader;

        [SetUp]
        public void SetupTest()
        {
            loader = new XmlClassLoader();
            ClassDef.ClassDefs.Clear();
        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException), "An invalid node 'class1' was encountered when loading the class definitions.")]
        public void TestInvalidXmlFormatWrongRootElement()
        {
            loader.LoadClass("<class1 name=\"TestClass\" assembly=\"Habanero.Test.BO.Loaders\" />");
        }

        //[
        //    Test,
        //        ExpectedException(typeof (UnknownTypeNameException),
        //            "The type TestClassNoExist does not exist in assembly Habanero.Test.BO.Loaders")]
        //public void TestLoadingNonExistantClass()
        //{
        //    ClassDef def = loader.LoadClass("<classDef name=\"TestClassNoExist\" assembly=\"Habanero.Test.BO.Loaders\" />");
        //    Assert.IsNull( def.)
        //}

        //[
        //    Test,
        //        ExpectedException(typeof (UnknownTypeNameException),
        //            "The assembly Habanero.Test.BO.NoExist could not be found")]
        //public void TestLoadingNonExistantAssembly()
        //{
        //    loader.LoadClass("<classDef name=\"TestClass\" assembly=\"Habanero.Test.BO.NoExist\" />");
        //}

        [Test, ExpectedException(typeof(XmlException))]
        public void TestClassWithNoAssembly()
        {
            ClassDef def = loader.LoadClass(@"
                <class name=""TestClass"" >
                    <property  name=""TestProp"" />
                    <primaryKey>
                        <prop name=""TestProp""/>
                    </primaryKey>
				</class>
			");
        }

        [Test, ExpectedException(typeof(XmlException))]
        public void TestClassWithNoClassname()
        {
            ClassDef def = loader.LoadClass(@"
                <class assembly=""Habanero.Test.BO.Loaders"">
                    <property  name=""TestProp"" />
                    <primaryKey>
                        <prop name=""TestProp""/>
                    </primaryKey>
				</class>
			");
        }

        [Test]
        public void TestNoTableName()
        {
            ClassDef def =
                loader.LoadClass(
                    @"
				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
                    <property  name=""TestProp"" />
                    <primaryKey>
                        <prop name=""TestProp""/>
                    </primaryKey>
				</class>
			");
            Assert.AreEqual("TestClass", def.TableName);
        }

        [Test]
        public void TestTableName()
        {
            ClassDef def =
                loader.LoadClass(
                    @"
				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" table=""myTable"">
                    <property  name=""TestProp"" />
                    <primaryKey>
                        <prop name=""TestProp""/>
                    </primaryKey>
				</class>
			");
            Assert.AreEqual("myTable", def.TableName);
        }

//        [Test]
//        public void TestSupportsSynchronisation()
//        {
//            ClassDef def =
//                loader.LoadClass(
//                    @"
//				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" supportsSynchronising=""true"">
//                    <property  name=""TestProp"" />
//                    <primaryKey>
//                        <prop name=""TestProp""/>
//                    </primaryKey>
//				</class>
//			");
//            Assert.IsTrue(def.SupportsSynchronising);
//        }


//        [Test]
//        public void TestSupportsSynchronisationDefault()
//        {
//            ClassDef def =
//                loader.LoadClass(
//                    @"
//				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
//                    <property  name=""TestProp"" />
//                    <primaryKey>
//                        <prop name=""TestProp""/>
//                    </primaryKey>
//				</class>
//			");
//            Assert.IsFalse(def.SupportsSynchronising);
//        }

        [Test]
        public void TestTwoPropClass()
        {
            ClassDef def =
                loader.LoadClass(
                    @"
				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
					<property  name=""TestProp"" />
					<property  name=""TestProp2"" />
                    <primaryKey>
                        <prop name=""TestProp""/>
                    </primaryKey>
				</class>
			");
            Assert.AreEqual(2, def.PropDefcol.Count);
            Assert.AreEqual("TestClass", def.ClassName);
        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
        public void TestClassWithNoProps()
        {
            ClassDef def = loader.LoadClass(
                @"<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
				</class>
			");
        }

        [Test]
        public void TestClassWithPrimaryKeyDef()
        {
            ClassDef def =
                loader.LoadClass(
                    @"
				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
					<property  name=""TestProp"" />
					<primaryKey>
						<prop name=""TestProp"" />
					</primaryKey>
				</class>
			");
            Assert.IsNotNull(def.PrimaryKeyDef);
            Assert.AreEqual(1, def.PrimaryKeyDef.Count);
        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
        public void TestClassWithNoPrimaryKeyException()
        {
            ClassDef def = loader.LoadClass(
                @"<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
				    <property  name=""TestProp"" />					
				</class>");
        }

        [Test]
        public void TestClassWithInheritanceAndNoPrimaryKey()
        {
            ClassDef def = loader.LoadClass(
                @"<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
                    <superClass class=""SomeTestClass"" assembly=""Habanero.Test.BO.Loaders"" />
					<property  name=""TestProp"" />
				</class>
			");
            Assert.IsNull(def.PrimaryKeyDef);
            Assert.IsNotNull(def.SuperClassDef);
        }

        [Test, ExpectedException(typeof (InvalidXmlDefinitionException))]
        public void TestClassWithMoreThanOnePrimaryKeyDef()
        {
            loader.LoadClass(
                @"
				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
					<property  name=""TestProp"" />
					<property  name=""TestProp2"" />
					<primaryKey>
						<prop name=""TestProp"" />
					</primaryKey>
					<primaryKey>
						<prop name=""TestProp2"" />
					</primaryKey>
				</class>
			");
        }

        // Not reaching the exception I was trying to cover (line 254 of XmlClassLoader)
//        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
//        public void TestClassWithInvalidPrimaryKeyDef()
//        {
//            loader.LoadClass(@"
//				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
//					<property name=""TestProp"" />
//					<primaryKey/>
//				</class>
//			");
//        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
        public void TestClassWithPrimaryKeyAndNoProps()
        {
            ClassDef def = loader.LoadClass(
                @"<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
					<property  name=""TestProp"" />
					<primaryKey/>
				</class>
			");
        }

        [Test]
        public void TestClassWithKeyDefs()
        {
            ClassDef def =
                loader.LoadClass(
                    @"
				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
					<property  name=""TestProp"" />
					<property  name=""TestProp2"" />
					<property  name=""TestProp3"" />
					<key>
						<prop name=""TestProp"" />
					</key>
					<key>
						<prop name=""TestProp2"" />
						<prop name=""TestProp3"" />
					</key>
                    <primaryKey>
                        <prop name=""TestProp""/>
                    </primaryKey>
				</class>
			");
            Assert.AreEqual(2, def.KeysCol.Count);
        }

        [Test]
        public void TestClassWithSingleRelationship()
        {
            ClassDef def =
                loader.LoadClass(
                    @"
				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
					<property  name=""TestProp"" />
                    <primaryKey>
                        <prop name=""TestProp""/>
                    </primaryKey>
					<relationship 
						name=""TestRelationship"" 
						type=""single"" 
						relatedClass=""TestRelatedClass"" 
						relatedAssembly=""Habanero.Test.BO.Loaders""
					>
						<relatedProperty property=""TestProp"" relatedProperty=""TestRelatedProp"" />
					</relationship>
				</class>
			");
            RelationshipDefCol relDefCol = def.RelationshipDefCol;
            Assert.AreEqual(1, relDefCol.Count, "There should be one relationship def from the given xml definition");
            Assert.IsNotNull(relDefCol["TestRelationship"],
                             "'TestRelationship' should be the name of the relationship created");
        }

        [Test]
        public void TestClassWithSuperClass()
        {
            ClassDef.ClassDefs.Clear();
            ClassDef.LoadClassDefs(
                new XmlClassDefsLoader(
                    @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
						</class>
					</classes>",
                                 new DtdLoader()));
            ClassDef def =
                loader.LoadClass(
                    @"
				<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"">
					<superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" />
					<property  name=""TestProp"" />
                    <primaryKey>
                        <prop name=""TestProp""/>
                    </primaryKey>
				</class>
			");
            Assert.IsNotNull(def.SuperClassDef);
            //ClassDef parentDef = ClassDef.ClassDefs[typeof(TestClass)];
            ClassDef parentDef = ClassDef.ClassDefs["Habanero.Test.BO.Loaders", "TestClass"];
            ClassDef superClassDef = def.SuperClassDef.SuperClassClassDef;
            Assert.AreSame(parentDef, superClassDef);
        }

        [Test]
        public void TestClassWithUIDef()
        {
            ClassDef def =
                loader.LoadClass(
                    @"
				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
					<property  name=""TestProp"" />
					<property  name=""TestProp2"" />
					<primaryKey>
						<prop name=""TestProp"" />
					</primaryKey>
					<ui>
						<form>
							<tab name=""testtab"">
								<columnLayout>
									<field label=""Test Prop"" property=""TestProp"" />
								</columnLayout>
							</tab>
						</form>
					</ui>
				</class>
			");
            UIDef uiDef = def.UIDefCol["default"];
            Assert.IsNotNull(uiDef);
            Assert.IsNotNull(uiDef.UIForm);
            Assert.IsNull(uiDef.UIGrid);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestEmptyXmlStringException()
        {
            ClassDef def = loader.LoadClass("");
        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
        public void TestInvalidClassElementException()
        {
            ClassDef def = loader.LoadClass(@"
				<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"">
					<property  name=""TestProp"" />
					<primaryKey>
						<prop name=""TestProp"" />
					</primaryKey>
                    <invalid/>
				</class>
			");
        }
    }

    public class TestClass : BusinessObject
    {
        protected override ClassDef ConstructClassDef()
        {
            throw new NotImplementedException();
        }
    }

    public class TestRelatedClass : BusinessObject
    {
        protected override ClassDef ConstructClassDef()
        {
            throw new NotImplementedException();
        }
    }
}