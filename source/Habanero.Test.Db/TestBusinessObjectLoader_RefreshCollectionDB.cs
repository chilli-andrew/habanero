using System;
using Habanero.BO;
using Habanero.DB;
using Habanero.Test.BO;
using Habanero.Test.BO.BusinessObjectLoader;
using NUnit.Framework;

namespace Habanero.Test.DB
{
    [TestFixture]
    public class TestBusinessObjectLoader_RefreshCollectionDB :
        TestBusinessObjectLoader_RefreshCollection
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetupTest()
        {
            base.SetupTest();
            ContactPersonTestBO.DeleteAllContactPeople();
        }

        #endregion

        protected override void DeleteEnginesAndCars()
        {
            Engine.DeleteAllEngines();
            Car.DeleteAllCars();
        }

        public TestBusinessObjectLoader_RefreshCollectionDB()
        {
            new TestUsingDatabase().SetupDBConnection();
        }

        protected override void SetupDataAccessor()
        {
            BORegistry.DataAccessor = new DataAccessorDB();
        }

        [Test]
        public void TestRefreshCollectionRefreshesNonDirtyObjects()
        {
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorDB();
            ContactPersonTestBO.DeleteAllContactPeople();

            ContactPersonTestBO.LoadDefaultClassDef();
            BusinessObjectCollection<ContactPersonTestBO> col = new BusinessObjectCollection<ContactPersonTestBO>();

            ContactPersonTestBO cp1 = ContactPersonTestBO.CreateSavedContactPerson();
            BusinessObjectManager.Instance.ClearLoadedObjects();

            ContactPersonTestBO.CreateSavedContactPerson();
            ContactPersonTestBO.CreateSavedContactPerson();
            col.LoadAll();
            string newSurname = Guid.NewGuid().ToString();
            cp1.Surname = newSurname;
            cp1.Save();
            ContactPersonTestBO secondInstanceOfCP1 = col.Find(cp1.ContactPersonID);

            //--------------------Assert Preconditions----------
            Assert.IsFalse(col.Contains(cp1));
            Assert.AreEqual(3, col.Count);
            Assert.AreEqual(newSurname, cp1.Surname);
            Assert.AreNotSame(secondInstanceOfCP1, cp1);
            Assert.AreNotEqual(newSurname, secondInstanceOfCP1.Surname);
            Assert.IsFalse(cp1.Status.IsDirty);
            //---------------Execute Test ----------------------
            BORegistry.DataAccessor.BusinessObjectLoader.Refresh(col);

            //---------------Test Result -----------------------
            Assert.AreEqual(3, col.Count);
            Assert.AreNotSame(secondInstanceOfCP1, cp1);
            Assert.AreEqual(newSurname, secondInstanceOfCP1.Surname);
        }

        [Ignore("Not implemented for DB as parametrized class defs are implemented in a different way (via afterload and updateobjectbeforepersisting)")]
        public override void Test_Refresh_W_ParametrizedClassDef_Typed()
        {

        }

        [Ignore("Not implemented for DB as parametrized class defs are implemented in a different way (via afterload and updateobjectbeforepersisting)")]
        public override void Test_Refresh_W_ParametrizedClassDef_Untyped() { }
    }
}