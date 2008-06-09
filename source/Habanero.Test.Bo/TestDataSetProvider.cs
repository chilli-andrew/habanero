//---------------------------------------------------------------------------------
// Copyright (C) 2007 Chillisoft Solutions
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
using System.Data;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using NMock;
using NUnit.Framework;

namespace Habanero.Test.BO
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>   
    //[TestFixture] 
    public abstract class TestDataSetProvider : TestUsingDatabase
    {
        protected XmlClassLoader _loader;
        protected ClassDef _classDef;
        protected BusinessObjectCollection<BusinessObject> _collection;
        protected DataTable itsTable;
        protected BusinessObject itsBo1;
        protected BusinessObject itsBo2;
        protected IBusinessObject itsRelatedBo;
        protected IDataSetProvider _dataSetProvider;

        protected Mock itsDatabaseConnectionMockControl;
        protected IDatabaseConnection itsConnection;

        [TestFixtureSetUp]
        public void SetupTestFixture()
        {
            this.SetupDBConnection();
            OrderItem.CreateTable();
        }

        [TestFixtureTearDown]
        public void TearDownFixure()
        {
            OrderItem.DropTable();
        }

        
        public void SetupTest()
        {
            ClassDef.ClassDefs.Clear();
            _loader = new XmlClassLoader();
            _classDef = MyBO.LoadClassDefWithLookup();
            OrderItem.LoadDefaultClassDef();

            TransactionCommitterStub committer = new TransactionCommitterStub();
			itsDatabaseConnectionMockControl = new DynamicMock(typeof (IDatabaseConnection));
			itsConnection = (IDatabaseConnection) itsDatabaseConnectionMockControl.MockInstance;
            _collection = new BusinessObjectCollection<BusinessObject>(_classDef);
            itsBo1 = _classDef.CreateNewBusinessObject(itsConnection);
            itsBo1.SetPropertyValue("TestProp", "bo1prop1");
            itsBo1.SetPropertyValue("TestProp2", "s1");
            _collection.Add(itsBo1);
            itsBo2 = _classDef.CreateNewBusinessObject(itsConnection);
            itsBo2.SetPropertyValue("TestProp", "bo2prop1");
            itsBo2.SetPropertyValue("TestProp2", "s2");
            _collection.Add(itsBo2);
        	SetupSaveExpectation();
            committer.AddBusinessObject(itsBo1);
            committer.CommitTransaction();
            //itsBo1.Save();
            //itsBo1.Save();
            _dataSetProvider = CreateDataSetProvider(_collection);
            BOMapper mapper = new BOMapper((BusinessObject) _collection.SampleBo);
            itsTable = _dataSetProvider.GetDataTable(mapper.GetUIDef().GetUIGridProperties());
            itsDatabaseConnectionMockControl.Verify();
        }

        [TearDown]
        public void TearDown()
        {
//            OrderItem.ClearTable();
        }

        protected abstract IDataSetProvider CreateDataSetProvider(IBusinessObjectCollection col);

		protected void SetupSaveExpectation()
		{
//            itsDatabaseConnectionMockControl.ExpectAndReturn("GetConnection",
//                DatabaseConnection.CurrentConnection.GetConnection());
//            itsDatabaseConnectionMockControl.ExpectAndReturn("GetConnection",
//                DatabaseConnection.CurrentConnection.GetConnection());
//            itsDatabaseConnectionMockControl.ExpectAndReturn("GetConnection",
//    DatabaseConnection.CurrentConnection.GetConnection());
//            itsDatabaseConnectionMockControl.ExpectAndReturn("GetConnection",
//DatabaseConnection.CurrentConnection.GetConnection());
		}

        [Test]
        public void TestCorrectNumberOfRows()
        {
            SetupTest();
            Assert.AreEqual(2, itsTable.Rows.Count);
        }

        [Test]
        public void TestCorrectNumberOfColumns()
        {
            SetupTest();
            Assert.AreEqual(3, itsTable.Columns.Count);
        }

        [Test]
        public void TestCorrectColumnNames()
        {
            SetupTest();
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
            SetupTest();
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
            SetupTest();
            Object prop = itsTable.Columns["TestProp2"].ExtendedProperties["LookupList"];
            Assert.AreSame(typeof (SimpleLookupList), prop.GetType());
        }

        [Test]
        public void TestDateTimeColumnType()
        {
            ClassDef.ClassDefs.Clear();
            MyBO.LoadClassDefWithDateTime();
//            IDataGridViewColumn column;
//            string requiredFormat = "dd.MMM.yyyy";
//            IGridBase gridBase = CreateGridBaseWithDateCustomFormatCol(out column, requiredFormat);
            BusinessObjectCollection<MyBO> col = new BusinessObjectCollection<MyBO>();
            MyBO bo = new MyBO();
            string dateTimeProp = "TestDateTime";
            DateTime expectedDate = DateTime.Now;
            bo.SetPropertyValue(dateTimeProp, expectedDate);

            col.Add(bo);
            IDataSetProvider dataSetProvider = CreateDataSetProvider(col);
            //--------------Assert PreConditions----------------            

            //---------------Execute Test ----------------------
            DataTable dataTable = dataSetProvider.GetDataTable(bo.ClassDef.GetUIDef("default").UIGrid);
            //---------------Test Result -----------------------
            Assert.AreSame(typeof(DateTime), dataTable.Columns[dateTimeProp].DataType);
            Assert.IsInstanceOfType(typeof(DateTime), dataTable.Rows[0][dateTimeProp]);
            //---------------Tear Down -------------------------          
        }

        //		[Test]
        //		public void TestUpdateBusinessObjectUpdatesRow() {
        //			 itsBo1.SetPropertyValue("TestProp", "bo1prop1updated");
        //			Assert.AreEqual("bo1prop1updated", _table.Rows[0]["TestProp"]);
        //		}
    }
}