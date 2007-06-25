using System;
using Habanero.Bo.ClassDefinition;
using Habanero.Bo.Loaders;
using Habanero.Bo;
using Habanero.Base;
using Habanero.Test;
using NUnit.Framework;

namespace Habanero.Test.Bo.Loaders
{
    /// <summary>
    /// Summary description for TestXmlPropertyLoader.
    /// </summary>
    [TestFixture]
    public class TestXmlPropertyLoader
    {
        private XmlPropertyLoader itsLoader;

        [SetUp]
        public void SetupTest()
        {
            itsLoader = new XmlPropertyLoader();
            ClassDef.GetClassDefCol.Clear();
        }

        [Test]
        public void TestSimpleProperty()
        {
            PropDef def = itsLoader.LoadProperty(@"<propertyDef name=""TestProp"" />");
            Assert.AreEqual("TestProp", def.PropertyName, "Property name should be same as that specified in xml");
            Assert.AreEqual(typeof (string), def.PropertyType,
                            "Property type should be the default as defined in the dtd");
            Assert.AreEqual(null, def.DefaultValue, "The default defaultValue is null");
            Assert.AreEqual("TestProp", def.FieldName,
                            "The field name should be the same as the property name by default");
        }


        [Test]
        public void TestPropertyWithType()
        {
            PropDef def = itsLoader.LoadProperty(@"<propertyDef name=""TestProp"" type=""Int32"" />");
            Assert.AreEqual(typeof (int), def.PropertyType, "Property type should be same as that specified in xml");
        }

        [Test]
        public void TestPropertyWithReadWriteRule()
        {
            PropDef def = itsLoader.LoadProperty(@"<propertyDef name=""TestProp"" readWriteRule=""ReadOnly"" />");
            Assert.AreEqual(PropReadWriteRule.ReadOnly, def.ReadWriteRule,
                            "Property read write rule should be same as that specified in xml");
        }

        [Test]
        public void TestPropertyWithDefaultValue()
        {
            PropDef def = itsLoader.LoadProperty(@"<propertyDef name=""TestProp"" defaultValue=""TestValue"" />");
            Assert.AreEqual("TestValue", def.DefaultValue, "Default value should be same as that specified in xml");
        }

        [Test]
        public void TestPropertyWithGuidDefaultValue()
        {
            PropDef def =
                itsLoader.LoadProperty(
                    @"<propertyDef name=""TestProp"" type=""Guid"" defaultValue=""{38373667-B06A-40c5-B4CE-299CE925E121}"" />");
            Assert.AreEqual(new Guid("{38373667-B06A-40c5-B4CE-299CE925E121}"), def.DefaultValue,
                            "Default value should be same as that specified in xml");
        }

        [Test]
        public void TestPropertyWithDatabaseFieldName()
        {
            PropDef def =
                itsLoader.LoadProperty(@"<propertyDef name=""TestProp"" databaseFieldName=""TestFieldName"" />");
            Assert.AreEqual("TestFieldName", def.FieldName, "Field Name should be the same as that specified in xml");
        }

        [Test]
        public void TestPropertyWithPropRule()
        {
            PropDef def =
                itsLoader.LoadProperty(
                    @"<propertyDef name=""TestProp""><rule name=""StringRule""><add key=""min"" value=""8""/><add key=""max"" value=""8"" /></rule></propertyDef>");
            Assert.AreEqual("PropRuleString", def.PropRule.GetType().Name);
        }

        [Test]
        public void TestPropertyWithDatabaseLookupListSource()
        {
            PropDef def =
                itsLoader.LoadProperty(
                    @"<propertyDef name=""TestProp""><databaseLookupListSource sqlString=""Source"" /></propertyDef>");
            Assert.AreSame(typeof (DatabaseLookupListSource), def.LookupListSource.GetType(),
                           "LookupListSource should be of type DatabaseLookupListSource but is of type " +
                           def.LookupListSource.GetType().Name);
            DatabaseLookupListSource source = (DatabaseLookupListSource) def.LookupListSource;
            Assert.AreEqual("Source", source.SqlString, "LookupListSource should be the same as that specified in xml");
            Assert.IsNull(source.ClassDef);
        }

        [Test]
        public void TestDatabaseLookupListSourceWithClassDef()
        {
        	ClassDef.GetClassDefCol.Clear();
            XmlClassLoader loader = new XmlClassLoader();

            ClassDef classDef = MyBo.LoadDefaultClassDef();

			PropDef def =
                itsLoader.LoadProperty(
                    @"<propertyDef name=""TestProp""><databaseLookupListSource sqlString=""Source"" className=""MyBo"" assemblyName=""Habanero.Test"" /></propertyDef>");
            DatabaseLookupListSource source = (DatabaseLookupListSource) def.LookupListSource;
            Assert.IsNotNull(source.ClassDef);
            Assert.AreEqual(classDef.ClassName, source.ClassDef.ClassName);
        }


        [Test]
        public void TestPropertyWithSimpleLookupListSource()
        {
            PropDef def =
                itsLoader.LoadProperty(
                    @"
					<propertyDef name=""TestProp"">
						<simpleLookupListSource>
							<stringGuidPair string=""s1"" guid=""{C2887FB1-7F4F-4534-82AB-FED92F954783}"" />
							<stringGuidPair string=""s2"" guid=""{B89CC2C9-4CBB-4519-862D-82AB64796A58}"" />
						</simpleLookupListSource>
					</propertyDef>");
            Assert.AreSame(typeof (SimpleLookupListSource), def.LookupListSource.GetType(),
                           "LookupListSource should be of type SimpleLookupListSource");
            SimpleLookupListSource source = (SimpleLookupListSource) def.LookupListSource;
            Assert.AreEqual(2, source.GetLookupList().Count, "LookupList should have two stringguidpairs");
        }
    }
}