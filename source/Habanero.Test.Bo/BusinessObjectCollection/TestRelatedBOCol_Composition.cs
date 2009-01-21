using System;
using System.Collections;
using System.Collections.Generic;
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
    public class TestRelatedBOCol_Composition
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

        //   TODO: remove option to do contactPerson.OrganisationID = xcsd.

        [Test]
        public void Test_AddMethod_AddPersistedChild()
        {
            //An already persisted invoice line cannot be added to an Invoice 
            //(In Habanero a new invoice line can be added to an invoice). 
            //---------------Set up test pack-------------------
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> compositionRelationship = GetCompositionRelationship(out cpCol);
            ContactPersonTestBO contactPerson = ContactPersonTestBO.CreateSavedContactPerson();

            //---------------Assert Precondition----------------
            util.AssertAllCollectionsHaveNoItems(cpCol);

            //---------------Execute Test ----------------------
            try
            {
                cpCol.Add(contactPerson);
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                AssertMessageIsCorrect(ex, compositionRelationship.RelationshipDef.RelatedObjectClassName, "added", compositionRelationship.RelationshipName);
            }
            catch (Exception ex)
            {
                Assert.Fail("HabaneroDeveloperException not thrown. Exception Thrown was : " + ex.Message);
            }
        }

        private MultipleRelationship<ContactPersonTestBO> GetCompositionRelationship(out BusinessObjectCollection<ContactPersonTestBO> cpCol) {
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            return GetCompositionRelationship(out cpCol, organisationTestBO);
        }

        private MultipleRelationship<ContactPersonTestBO> GetCompositionRelationship(out BusinessObjectCollection<ContactPersonTestBO> cpCol, OrganisationTestBO organisationTestBO)
        {
            MultipleRelationship<ContactPersonTestBO> compositionRelationship =
                organisationTestBO.Relationships.GetMultiple<ContactPersonTestBO>("ContactPeople");
            RelationshipDef relationshipDef = (RelationshipDef)compositionRelationship.RelationshipDef;
            relationshipDef.RelationshipType = RelationshipType.Composition;
            cpCol = compositionRelationship.BusinessObjectCollection;
            return compositionRelationship;
        }


        [Test]
        public void Test_AddMethod_AddNewChild()
        {
            //An new invoice line can be added to an Invoice 
            //(In Habanero a new invoice line can be added to an invoice). 
            //---------------Set up test pack-------------------
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> compositionRelationship = GetCompositionRelationship(out cpCol);
            ContactPersonTestBO myBO = ContactPersonTestBO.CreateUnsavedContactPerson(TestUtil.CreateRandomString(), TestUtil.CreateRandomString());
            util.RegisterForAddedAndRemovedEvents(cpCol);

            //---------------Assert Precondition----------------
            util.AssertAllCollectionsHaveNoItems(cpCol);

            //---------------Execute Test ----------------------
            cpCol.Add(myBO);

            //---------------Test Result -----------------------
            util.AssertAddedEventFired();
            util.AssertOneObjectInCurrentAndCreatedCollection(cpCol);
            Assert.AreSame(myBO.Organisation, compositionRelationship.OwningBO);
        }

        [Test]
        public void Test_ResetParent_PersistedChild()
        {
            //An already persisted invoice line cannot be added to an Invoice 
            // This rule must also be implemented for the reverse relationship.
            //---------------Set up test pack-------------------
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> compositionRelationship = GetCompositionRelationship(out cpCol, organisationTestBO);
            ContactPersonTestBO contactPerson = ContactPersonTestBO.CreateSavedContactPerson();
            contactPerson.Surname = TestUtil.CreateRandomString();
            contactPerson.FirstName = TestUtil.CreateRandomString();
            contactPerson.Save();

            OrganisationTestBO alternateOrganisationTestBO = OrganisationTestBO.CreateSavedOrganisation();

            //---------------Assert Precondition----------------
            util.AssertAllCollectionsHaveNoItems(cpCol);
            Assert.IsFalse(contactPerson.Status.IsNew);

            //---------------Execute Test ----------------------
            try
            {
                contactPerson.Organisation = alternateOrganisationTestBO;
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains("The " + compositionRelationship.RelationshipDef.RelatedObjectClassName, ex.Message);
                StringAssert.Contains("could not be added since the " + compositionRelationship.RelationshipName + " relationship is set up as ", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("HabaneroDeveloperException not thrown. Exception Thrown was : " + ex.Message);
            }
        }
        [Test]
        public void Test_SetParentNull_PersistedChild()
        {
            //An already persisted invoice line cannot be added to an Invoice 
            // This rule must also be implemented for the reverse relationship.
            //---------------Set up test pack-------------------
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> compositionRelationship = GetCompositionRelationship(out cpCol, organisationTestBO);
            ContactPersonTestBO contactPerson = cpCol.CreateBusinessObject();
            contactPerson.Surname = TestUtil.CreateRandomString();
            contactPerson.FirstName = TestUtil.CreateRandomString();
            contactPerson.Save();

            //---------------Assert Precondition----------------
            util.AssertOneObjectInCurrentPersistedCollection(cpCol);
            Assert.IsFalse(contactPerson.Status.IsNew);
            Assert.AreSame(contactPerson.Organisation, organisationTestBO);

            //---------------Execute Test ----------------------
            try
            {
                contactPerson.Organisation = null;
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains("The " + compositionRelationship.RelationshipDef.RelatedObjectClassName, ex.Message);
                StringAssert.Contains("could not be removed since the " + compositionRelationship.RelationshipName + " relationship is set up as ", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("HabaneroDeveloperException not thrown. Exception Thrown was : " + ex.Message);
            }
        }

        [Test]
        public void Test_ResetParent_NewChild_ReverseRelationship_Loaded()
        {
            //An new invoice line can be added to an Invoice 
            //(In Habanero a new invoice line can be added to an invoice). 
            // This rule is also be applied for the reverse relationship
            // In this case the organisation can be set for myBO since myBO has never
            //   been associated with am organisation.
            //---------------Set up test pack-------------------
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> compositionRelationship = GetCompositionRelationship(out cpCol, organisationTestBO);
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
            //An new invoice line can be added to an Invoice 
            //(In Habanero a new invoice line can be added to an invoice). 
            // This rule is also be applied for the reverse relationship
            // In this case the organisation can be set to null for myBO since myBO has never
            //   been associated with am organisation.
            //---------------Set up test pack-------------------
            ContactPersonTestBO myBO = ContactPersonTestBO.CreateUnsavedContactPerson(TestUtil.CreateRandomString(), TestUtil.CreateRandomString());

            //---------------Assert Precondition----------------
            Assert.IsNull(myBO.Organisation);

            //---------------Execute Test ----------------------
            myBO.Organisation = null;

            //---------------Test Result -----------------------
            Assert.IsNull(myBO.Organisation);
        }

        [Test]
        public void Test_RemoveMethod()
        {
            //An invoice line cannot be removed from an Invoice.
            //---------------Set up test pack-------------------
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> compositionRelationship = GetCompositionRelationship(out cpCol);
            ContactPersonTestBO contactPerson = ContactPersonTestBO.CreateUnsavedContactPerson(TestUtil.CreateRandomString(), TestUtil.CreateRandomString());
            contactPerson.OrganisationID = organisationTestBO.OrganisationID;
            contactPerson.Save();
            cpCol.LoadAll();
            util.RegisterForAddedAndRemovedEvents(cpCol);

            //---------------Assert Precondition----------------
            util.AssertOneObjectInCurrentPersistedCollection(cpCol);
            Assert.IsFalse((bool)ReflectionUtilities.GetPrivatePropertyValue(cpCol, "Loading"));

            //---------------Execute Test ----------------------
            try
            {
                cpCol.Remove(contactPerson);
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains("The " + compositionRelationship.RelationshipDef.RelatedObjectClassName, ex.Message);
                StringAssert.Contains("could not be removed since the " + compositionRelationship.RelationshipName + " relationship is set up as ", ex.Message);
                StringAssert.Contains("RemoveChildAction.Prevent", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("HabaneroDeveloperException not thrown. Exception Thrown was : " + ex.Message);
            }
        }

        [Test]
        public void Test_ResetParent_NewChild_SetToNull()
        {
            //An new invoice line can be added to an Invoice 
            //(In Habanero a new invoice line can be added to an invoice). 
            // This rule is also be applied for the reverse relationship
            // In this case the organisation can not be set to null for myBO since myBO has
            //   been associated with am organisation.
            //---------------Set up test pack-------------------
            OrganisationTestBO organisationTestBO = OrganisationTestBO.CreateSavedOrganisation();
            BusinessObjectCollection<ContactPersonTestBO> cpCol;
            MultipleRelationship<ContactPersonTestBO> compositionRelationship = GetCompositionRelationship(out cpCol);
            ContactPersonTestBO contactPerson = ContactPersonTestBO.CreateUnsavedContactPerson(TestUtil.CreateRandomString(), TestUtil.CreateRandomString());
            contactPerson.Organisation = (OrganisationTestBO)compositionRelationship.OwningBO;
            util.RegisterForAddedAndRemovedEvents(cpCol);

            //---------------Assert Precondition----------------
            util.AssertOneObjectInCurrentAndCreatedCollection(cpCol);
            util.AssertAddedEventNotFired();
            util.AssertRemovedEventNotFired();

            //---------------Execute Test ----------------------
            try
            {
                contactPerson.Organisation = null;
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains("The " + compositionRelationship.RelationshipDef.RelatedObjectClassName, ex.Message);
                StringAssert.Contains("could not be removed since the " + compositionRelationship.RelationshipName + " relationship is set up as ", ex.Message);
                StringAssert.Contains("RemoveChildAction.Prevent", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("HabaneroDeveloperException not thrown. Exception Thrown was : " + ex.Message);
            }
        }

        #region Utils

        private void AssertMessageIsCorrect(HabaneroDeveloperException ex, string relatedObjectClassName, string operation, string relationshipName)
        {
            StringAssert.Contains("The " + relatedObjectClassName, ex.Message);
            StringAssert.Contains("could not be " + operation + " since the " + relationshipName + " relationship is set up as ", ex.Message);
        }


        #endregion

    }
}