using System;
using System.Data;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using Habanero.BO;
using Habanero.DB;
using Habanero.Base;
using Habanero.Test;
using NMock;
using NUnit.Framework;
using Rhino.Mocks;
using BusinessObject=Habanero.BO.BusinessObject;

namespace Habanero.Test.BO
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>   
    //[TestFixture] 
    public abstract class TestBusinessObjectCollectionDataProvider : TestUsingDatabase
    {
        protected XmlClassLoader itsLoader;
        protected ClassDef itsClassDef;
        protected BusinessObjectCollection<BusinessObject> itsCollection;
        protected DataTable itsTable;
        protected BusinessObject itsBo1;
        protected BusinessObject itsBo2;
        protected BusinessObject itsRelatedBo;
        protected IDataSetProvider itsProvider;

        protected Mock itsDatabaseConnectionMockControl;
        protected IDatabaseConnection itsConnection;

        [TestFixtureSetUp]
        public void SetupTestFixture()
        {
            this.SetupDBConnection();
            ClassDef.ClassDefs.Clear();
            itsLoader = new XmlClassLoader();
            itsClassDef = MyBO.LoadClassDefWithLookup();
        }

        [SetUp]
        public void SetupTest()
        {
			itsDatabaseConnectionMockControl = new DynamicMock(typeof (IDatabaseConnection));
			itsConnection = (IDatabaseConnection) itsDatabaseConnectionMockControl.MockInstance;
            itsCollection = new BusinessObjectCollection<BusinessObject>(itsClassDef);
            itsBo1 = itsClassDef.CreateNewBusinessObject(itsConnection);
            itsBo1.SetPropertyValue("TestProp", "bo1prop1");
            itsBo1.SetPropertyValue("TestProp2", "s1");
            itsCollection.Add(itsBo1);
            itsBo2 = itsClassDef.CreateNewBusinessObject(itsConnection);
            itsBo2.SetPropertyValue("TestProp", "bo2prop1");
            itsBo2.SetPropertyValue("TestProp2", "s2");
            itsCollection.Add(itsBo2);
        	SetupSaveExpectation();
            itsBo1.Save();
            //itsBo1.Save();
            itsProvider = CreateDataSetProvider(itsCollection);
            BOMapper mapper = new BOMapper(itsCollection.SampleBo);
            itsTable = itsProvider.GetDataTable(mapper.GetUIDef().GetUIGridProperties());
            itsDatabaseConnectionMockControl.Verify();
        }

        protected abstract IDataSetProvider CreateDataSetProvider(BusinessObjectCollection<BusinessObject> col);

		protected void SetupSaveExpectation()
		{
			itsDatabaseConnectionMockControl.ExpectAndReturn("GetConnection",
				DatabaseConnection.CurrentConnection.GetConnection());
			itsDatabaseConnectionMockControl.ExpectAndReturn("GetConnection",
				DatabaseConnection.CurrentConnection.GetConnection());
			itsDatabaseConnectionMockControl.ExpectAndReturn("ExecuteSql",
				1, new object[] { null, null });
		}

        [Test]
        public void TestCorrectNumberOfRows()
        {
            Assert.AreEqual(2, itsTable.Rows.Count);
        }

        [Test]
        public void TestCorrectNumberOfColumns()
        {
            Assert.AreEqual(3, itsTable.Columns.Count);
        }

        [Test]
        public void TestCorrectColumnNames()
        {
            Assert.AreEqual("ID", itsTable.Columns[0].Caption);
            Assert.AreEqual("ID", itsTable.Columns[0].ColumnName);

            Assert.AreEqual("Test Prop", itsTable.Columns[1].Caption);
            Assert.AreEqual("TestProp", itsTable.Columns[1].ColumnName);
            Assert.AreEqual("Test Prop 2", itsTable.Columns[2].Caption);
            Assert.AreEqual("TestProp2", itsTable.Columns[2].ColumnName);
        }

        [Test]
        public void TestCorrectRowValues()
        {
            DataRow row1 = itsTable.Rows[0];
            DataRow row2 = itsTable.Rows[1];
            Assert.AreEqual("bo1prop1", row1["TestProp"]);
            Assert.AreEqual(itsBo1.ID.ToString(), row1["ID"]);
            Assert.AreEqual("s1", row1["TestProp2"]);
            Assert.AreEqual("bo2prop1", row2["TestProp"]);
            Assert.AreEqual("s2", row2["TestProp2"]);
        }

        [Test]
        public void TestLookupListPopulated()
        {
            Object prop = itsTable.Columns["TestProp2"].ExtendedProperties["LookupList"];
            Assert.AreSame(typeof (SimpleLookupList), prop.GetType());
        }


        //		[Test]
        //		public void TestUpdateBusinessObjectUpdatesRow() {
        //			 itsBo1.SetPropertyValue("TestProp", "bo1prop1updated");
        //			Assert.AreEqual("bo1prop1updated", _table.Rows[0]["TestProp"]);
        //		}
    }
}