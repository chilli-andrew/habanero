using System.Collections;
using System.Collections.Generic;
using System.Text;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Test.BO.RelatedBusinessObjectCollection;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Test.BO.BusinessObjectCollection
{
    [TestFixture]
    public class TestRelatedBOCol_Aggregation
    {
        private readonly TestUtilsRelated util = new TestUtilsRelated();

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
        }

        [SetUp]
        public void SetupTest()
        {
            ClassDef.ClassDefs.Clear();
            BORegistry.DataAccessor = new DataAccessorInMemory();
            ContactPersonTestBO.LoadClassDefOrganisationTestBORelationship_MultipleReverse();
            OrganisationTestBO.LoadDefaultClassDef_PreventAddChild();
        }

        [TearDown]
        public void TearDownTest()
        {
            TestUtil.WaitForGC();
        }

        [Test]
        public void Test_AddMethod_AddPersistedChild()
        {
            //�	An already persisted tyre can be added to a car 
            //---------------Set up test pack-------------------
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> aggregateRelationship = GetAggregateRelationship(organisationTestBO, out cpCol);
            ContactPersonTestBO contactPerson = ContactPersonTestBO.CreateSavedContactPerson();
            util.RegisterForAddedAndRemovedEvents(cpCol);

            //---------------Assert Precondition----------------
            util.AssertAllCollectionsHaveNoItems(cpCol);

            //---------------Execute Test ----------------------
            cpCol.Add(contactPerson);

            //---------------Test Result -----------------------
            util.AssertAddedEventFired();
            util.AssertOneObjectInCurrentAndAddedCollection(cpCol);
            Assert.AreSame(contactPerson.Organisation, aggregateRelationship.OwningBO);
        }

        private MultipleRelationship<ContactPersonTestBO> GetAggregateRelationship(OrganisationTestBO organisationTestBO, out BusinessObjectCollection<ContactPersonTestBO> cpCol) {
            MultipleRelationship<ContactPersonTestBO> aggregateRelationship = organisationTestBO.Relationships.GetMultiple<ContactPersonTestBO>("ContactPeople");
            RelationshipDef relationshipDef = (RelationshipDef) aggregateRelationship.RelationshipDef;
            relationshipDef.RelationshipType = RelationshipType.Aggregation;
            cpCol = aggregateRelationship.BusinessObjectCollection;
            return aggregateRelationship;
        }


        [Test]
        public void Test_AddMethod_AddNewChild()
        {
            //�(In habanero a new tyre can be added to a car).
            //---------------Set up test pack-------------------
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> aggregateRelationship = GetAggregateRelationship(organisationTestBO, out cpCol);
            ContactPersonTestBO contactPerson = ContactPersonTestBO.CreateUnsavedContactPerson();
            util.RegisterForAddedAndRemovedEvents(cpCol);

            //---------------Assert Precondition----------------
            util.AssertAllCollectionsHaveNoItems(cpCol);

            //---------------Execute Test ----------------------
            cpCol.Add(contactPerson);

            //---------------Test Result -----------------------
            util.AssertAddedEventFired();
            util.AssertOneObjectInCurrentAndCreatedCollection(cpCol);
            Assert.AreSame(contactPerson.Organisation, aggregateRelationship.OwningBO);
        }


        [Test]
        public void Test_ResetParent_PersistedChild()
        {
            //The Tyre may however be transferred from one car to another. 
            //---------------Set up test pack-------------------
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> aggregateRelationship = GetAggregateRelationship(organisationTestBO, out cpCol);
            ContactPersonTestBO contactPerson = cpCol.CreateBusinessObject();
            contactPerson.Surname = TestUtil.CreateRandomString();
            contactPerson.FirstName = TestUtil.CreateRandomString();
            contactPerson.Save();

            OrganisationTestBO alternateOrganisationTestBO = OrganisationTestBO.CreateSavedOrganisation();


            //---------------Assert Precondition----------------
            util.AssertOneObjectInCurrentPersistedCollection(cpCol);
            Assert.IsFalse(contactPerson.Status.IsNew);
            Assert.AreSame(contactPerson.Organisation, organisationTestBO);
            // Assert.AreEqual(0, cpAltCol.Count);

            //---------------Execute Test ----------------------
            contactPerson.Organisation = alternateOrganisationTestBO;

            //---------------Test Result -----------------------
            Assert.AreEqual(0, cpCol.Count);
            Assert.IsFalse(cpCol.Contains(contactPerson));
            util.AssertOneObjectInRemovedAndPersisted(cpCol);
            MultipleRelationship<ContactPersonTestBO> relationship = alternateOrganisationTestBO.Relationships.GetMultiple<ContactPersonTestBO>("ContactPeople");
            BusinessObjectCollection<ContactPersonTestBO> cpAltCol = relationship.BusinessObjectCollection; 
            Assert.AreSame(contactPerson.Organisation, relationship.OwningBO);
            Assert.AreSame(alternateOrganisationTestBO, contactPerson.Organisation);
            util.AssertOneObjectInCurrentAndAddedCollection(cpAltCol);
        }

        [Test]
        public void Test_ResetParent_NewChild_ReverseRelationship_Loaded()
        {
            //The Tyre may however be transferred from one car to another. 
            //---------------Set up test pack-------------------
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> aggregateRelationship = GetAggregateRelationship(organisationTestBO, out cpCol);
            ContactPersonTestBO contactPerson = ContactPersonTestBO.CreateUnsavedContactPerson(TestUtil.CreateRandomString(), TestUtil.CreateRandomString());
            util.RegisterForAddedAndRemovedEvents(cpCol);

            //---------------Assert Precondition----------------
            util.AssertAllCollectionsHaveNoItems(cpCol);

            //---------------Execute Test ----------------------
            contactPerson.Organisation = organisationTestBO;

            //---------------Test Result -----------------------
            Assert.AreEqual(contactPerson.OrganisationID, organisationTestBO.OrganisationID);
            util.AssertOneObjectInCurrentAndCreatedCollection(cpCol);
            Assert.IsTrue(cpCol.Contains(contactPerson));
            util.AssertAddedEventFired();
        }

        [Test]
        public void Test_SetParentNull()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO contactPerson = ContactPersonTestBO.CreateUnsavedContactPerson(TestUtil.CreateRandomString(), TestUtil.CreateRandomString());

            //---------------Assert Precondition----------------
            Assert.IsNull(contactPerson.Organisation);

            //---------------Execute Test ----------------------
            contactPerson.Organisation = null;

            //---------------Test Result -----------------------
            Assert.IsNull(contactPerson.Organisation);
        }

        [Test]
        public void Test_RemoveMethod()
        {
            //A tyre can be removed from its car. 
            //---------------Set up test pack-------------------
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> aggregateRelationship = GetAggregateRelationship(organisationTestBO, out cpCol);
            ContactPersonTestBO contactPerson = ContactPersonTestBO.CreateUnsavedContactPerson(TestUtil.CreateRandomString(), TestUtil.CreateRandomString());
            contactPerson.OrganisationID = organisationTestBO.OrganisationID;
            contactPerson.Save();
            cpCol.LoadAll();
            util.RegisterForAddedAndRemovedEvents(cpCol);

            //---------------Assert Precondition----------------
            util.AssertOneObjectInCurrentPersistedCollection(cpCol);

            //---------------Execute Test ----------------------
            cpCol.Remove(contactPerson);

            //---------------Test Result -----------------------
            util.AssertOneObjectInRemovedAndPersisted(cpCol);
            util.AssertRemovedEventFired();
        }


        [Test]
        public void Test_ResetParent_NewChild_SetToNull()
        {
            //A tyre can be removed from its car.   This test is removing via the reverse relationship
            //---------------Set up test pack-------------------
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> aggregateRelationship = GetAggregateRelationship(organisationTestBO, out cpCol);
            ContactPersonTestBO contactPerson = ContactPersonTestBO.CreateUnsavedContactPerson(TestUtil.CreateRandomString(), TestUtil.CreateRandomString());
            contactPerson.OrganisationID = organisationTestBO.OrganisationID;
            contactPerson.Save();
            cpCol.LoadAll();
            util.RegisterForAddedAndRemovedEvents(cpCol);

            //---------------Assert Precondition----------------
            util.AssertOneObjectInCurrentPersistedCollection(cpCol);

            //---------------Execute Test ----------------------

            contactPerson.Organisation = null;

            //---------------Test Result -----------------------
            Assert.IsNull(contactPerson.Organisation);
            util.AssertOneObjectInRemovedAndPersisted(cpCol);
            util.AssertRemovedEventFired();
        }





    }
}