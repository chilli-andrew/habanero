using System;
using System.Collections.Generic;
using System.Text;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;

namespace Habanero.Test.General
{
	/// <summary>
	/// Class that tests different aspects of deletion from a Business object perspective
	/// </summary>
	[TestFixture]
	public class TestDeletion : TestUsingDatabase
	{
		private ContactPerson _person;
		private Address _address1;
		private Address _address2;
			
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			this.SetupDBConnection();
		}

		[SetUp]
		public void SetupTest()
		{
			ClassDef.ClassDefs.Clear();
			_person = new ContactPerson();
			_person.FirstName = "Joe";
			_person.Surname = "Soap";
			_person.DateOfBirth = new DateTime(2001,01,01);
			_person.Save();
			_address1 = new Address();
			_address1.AddressLine1 = "1 Test Road";
			_address1.AddressLine2 = "Test Suburb";
			_address1.AddressLine3 = "Test City";
			_address1.AddressLine4 = "Test Country";
			_address1.Relationships.SetRelatedObject("ContactPerson", _person);
			_address1.Save();
			_address2 = new Address();
			_address2.AddressLine1 = "2 Test Road";
			_address2.AddressLine2 = "Test Suburb";
			_address2.AddressLine3 = "Test City";
			_address2.AddressLine4 = "Test Country";
			_address2.Relationships.SetRelatedObject("ContactPerson", _person);
			_address2.Save();
		}

		[TestFixtureTearDown]
		public void TearDownTest()
		{
			Address.DeleteAllAddresses();
			ContactPerson.DeleteAllContactPeople();
		}

		[Test]
		public void TestCascadeDelete()
		{
			Assert.AreEqual(2, _person.Addresses.Count);
			_person.Delete();
			_person.Save();
			Assert.IsTrue(_person.State.IsDeleted);
			Assert.AreEqual(0, _person.Addresses.Count);
		}
	}
}
