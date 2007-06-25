using System;
using Habanero.Bo.ClassDefinition;
using Habanero.Bo;
using Habanero.Db;
using Habanero.Base;
using Habanero.Test;
using NMock;
using NUnit.Framework;

namespace Habanero.Test.Bo
{
    /// <summary>
    /// Summary description for TestBoMapper.
    /// </summary>
    [TestFixture]
    public class TestBoMapper : TestUsingDatabase
    {
        private ClassDef itsClassDef;
        private ClassDef itsRelatedClassDef;

        [TestFixtureSetUp]
        public void SetupTestFixture()
        {
            this.SetupDBConnection();
        }

        [Test]
        public void TestGetPropertyValueToDisplay()
        {
            ClassDef.GetClassDefCol.Clear();
            itsClassDef = MyBo.LoadClassDefWithLookup();
            MyBo bo1 = (MyBo) itsClassDef.CreateNewBusinessObject();
            bo1.SetPropertyValue("TestProp2", "s1");
            BOMapper mapper = new BOMapper(bo1);
            Assert.AreEqual("s1", mapper.GetPropertyValueToDisplay("TestProp2"));
        }

        [Test]
        public void TestGetPropertyValueWithDot()
        {
            Mock mockDbConnection = new DynamicMock(typeof (IDatabaseConnection));
            IDatabaseConnection connection = (IDatabaseConnection) mockDbConnection.MockInstance;

            Mock relColControl = new DynamicMock(typeof (IRelationshipCol));
            IRelationshipCol mockRelCol = (IRelationshipCol) relColControl.MockInstance;

            ClassDef.GetClassDefCol.Clear();
            itsClassDef = MyBo.LoadClassDefWithRelationship();
            itsRelatedClassDef = MyRelatedBo.LoadClassDef();
            MyBo bo1 = (MyBo) itsClassDef.CreateNewBusinessObject(connection);
            MyRelatedBo relatedBo = (MyRelatedBo) itsRelatedClassDef.CreateNewBusinessObject();
            Guid myRelatedBoGuid = new Guid(relatedBo.ID.GetObjectId().Substring(3, 38));
            bo1.SetPropertyValue("RelatedID", myRelatedBoGuid);
            relatedBo.SetPropertyValue("MyRelatedTestProp", "MyValue");
            BOMapper mapper = new BOMapper(bo1);
            bo1.Relationships = mockRelCol;

            relColControl.ExpectAndReturn("GetRelatedBusinessObject", relatedBo, new object[] {"MyRelationship"});

            mockDbConnection.ExpectAndReturn("GetConnection", DatabaseConnection.CurrentConnection.GetConnection(),
                                             new object[] {});
            Assert.AreEqual("MyValue", mapper.GetPropertyValueToDisplay("MyRelationship.MyRelatedTestProp"));
        }

        [Test, ExpectedException(typeof (RelationshipNotFoundException))]
        public void TestGetPropertyValueWithDot_IncorrectRelationshipName()
        {
            ClassDef.GetClassDefCol.Clear();
            itsClassDef = MyBo.LoadClassDefWithRelationship();
            itsRelatedClassDef = MyRelatedBo.LoadClassDef();
            MyBo bo1 = (MyBo) itsClassDef.CreateNewBusinessObject();
            BOMapper mapper = new BOMapper(bo1);
            Assert.AreEqual(null, mapper.GetPropertyValueToDisplay("MyIncorrectRelationship.MyRelatedTestProp"));
        }

        [Test]
        public void TestGetPropertyValueWithDotNoValue()
        {
            Mock mockDbConnection = new DynamicMock(typeof (IDatabaseConnection));
            IDatabaseConnection connection = (IDatabaseConnection) mockDbConnection.MockInstance;

            Mock relColControl = new DynamicMock(typeof (IRelationshipCol));
            IRelationshipCol mockRelCol = (IRelationshipCol) relColControl.MockInstance;

            ClassDef.GetClassDefCol.Clear();
            itsClassDef = MyBo.LoadClassDefWithRelationship();
            itsRelatedClassDef = MyRelatedBo.LoadClassDef();
            MyBo bo1 = (MyBo) itsClassDef.CreateNewBusinessObject(connection);
            MyRelatedBo relatedBo = (MyRelatedBo) itsRelatedClassDef.CreateNewBusinessObject();
//			Guid myRelatedBoGuid = new Guid(relatedBo.ID.GetObjectId().Substring(3, 38));
//			bo1.SetPropertyValue("RelatedID", myRelatedBoGuid);
            relatedBo.SetPropertyValue("MyRelatedTestProp", "MyValue");
            BOMapper mapper = new BOMapper(bo1);
            bo1.Relationships = mockRelCol;

            relColControl.ExpectAndReturn("GetRelatedBusinessObject", null, new object[] {"MyRelationship"});

            mockDbConnection.ExpectAndReturn("GetConnection", DatabaseConnection.CurrentConnection.GetConnection(),
                                             new object[] {});
            Assert.AreEqual(null, mapper.GetPropertyValueToDisplay("MyRelationship.MyRelatedTestProp"));
        }

        public void TestVirtualPropertyValue()
        {
            Mock mockDbConnection = new DynamicMock(typeof (IDatabaseConnection));
            IDatabaseConnection connection = (IDatabaseConnection) mockDbConnection.MockInstance;

            ClassDef.GetClassDefCol.Clear();
            itsClassDef = MyBo.LoadDefaultClassDef();
            MyBo bo1 = (MyBo) itsClassDef.CreateNewBusinessObject(connection);

            BOMapper mapper = new BOMapper(bo1);
            Assert.AreEqual("MyNameIsMyBo", mapper.GetPropertyValueToDisplay("-MyName-"));
        }
    }
}