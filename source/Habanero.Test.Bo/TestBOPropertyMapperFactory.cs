#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
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
#endregion
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Test.BO
{
// ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestBOPropertyMapperFactory
    {
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            ClassDef.ClassDefs.Clear();
            GlobalRegistry.UIExceptionNotifier = new RethrowingExceptionNotifier();
            BORegistry.DataAccessor = new DataAccessorInMemory();
            BORegistry.BusinessObjectManager = new BusinessObjectManagerNull();
            AddressTestBO.LoadDefaultClassDef();
            ContactPersonTestBO.LoadClassDefOrganisationTestBORelationship_MultipleReverse();
            OrganisationTestBO.LoadDefaultClassDef();
        }
        [SetUp]
        public void SetupTest()
        {

        }

        [Test]
        public void Test_Create_WhenDirectProp_ShouldCreateBOPropMapper()
        {
            //---------------Set up test pack-------------------
            const string propName = "Surname";
            var bo = new ContactPersonTestBO();
            //---------------Assert Precondition----------------
            Assert.IsTrue(bo.Props.Contains(propName));
            //---------------Execute Test ----------------------
            IBOPropertyMapper propMapper = BOPropMapperFactory.CreateMapper(bo, propName);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<BOPropertyMapper>(propMapper);
            Assert.AreEqual(propName, propMapper.PropertyName);
            Assert.AreSame(bo, propMapper.BusinessObject);
        }

        [Test]
        public void Test_Create_WhenRelatedProp_ShouldCreateBOPropMapper()
        {
            //---------------Set up test pack-------------------
            const string propName = "Organisation.OrganisationID";
            var bo = new ContactPersonTestBO();
            //---------------Assert Precondition----------------
            Assert.IsTrue(bo.Relationships.Contains("Organisation"));
            //---------------Execute Test ----------------------
            IBOPropertyMapper propMapper = BOPropMapperFactory.CreateMapper(bo, propName);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<BOPropertyMapper>(propMapper);
            Assert.AreEqual(propName, propMapper.PropertyName);
            Assert.AreSame(bo, propMapper.BusinessObject);
        }

        [Test]
        public void Test_Create_WhenReflectiveProp_ShouldCreateReflectivePropMapper()
        {
            //---------------Set up test pack-------------------
            const string propName = "-ReflectiveProp-";
            const string propNameWithoutDash = "ReflectiveProp";
            var bo = new ContactPersonTestBO();
            //---------------Assert Precondition----------------
            var propertyInfo = ReflectionUtilities.GetPropertyInfo(bo.GetType(), propNameWithoutDash);
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            IBOPropertyMapper propMapper = BOPropMapperFactory.CreateMapper(bo, propName);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<ReflectionPropertyMapper>(propMapper);
            Assert.AreEqual(propNameWithoutDash, propMapper.PropertyName);
            Assert.AreSame(bo, propMapper.BusinessObject);
        }
        [Test]
        public void Test_Create_WhenReflectiveProp_WhenNotDefinedWithDash_ShouldCreateReflectivePropMapper()
        {
            //---------------Set up test pack-------------------
            const string propName = "ReflectiveProp";
            var bo = new ContactPersonTestBO();
            //---------------Assert Precondition----------------
            var propertyInfo = ReflectionUtilities.GetPropertyInfo(bo.GetType(), propName);
            Assert.IsNotNull(propertyInfo);
            //---------------Execute Test ----------------------
            IBOPropertyMapper propMapper = BOPropMapperFactory.CreateMapper(bo, propName);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<ReflectionPropertyMapper>(propMapper);
            Assert.AreEqual(propName, propMapper.PropertyName);
            Assert.AreSame(bo, propMapper.BusinessObject);
        }
        [Test]
        public void Test_Create_WhenReflectiveProp_WhenWhenReflectivePropNotExists_ShouldCreateReflectivePropMapper()
        {
            //---------------Set up test pack-------------------
            const string propName = "NonExistentReflectiveProp";
            var bo = new ContactPersonTestBO();
            //---------------Assert Precondition----------------
            var propertyInfo = ReflectionUtilities.GetPropertyInfo(bo.GetType(), propName);
            Assert.IsNull(propertyInfo);
            //---------------Execute Test ----------------------
            try
            {
                BOPropMapperFactory.CreateMapper(bo, propName);
                Assert.Fail("Expected to throw an InvalidPropertyException");
            }
                //---------------Test Result -----------------------
            catch (InvalidPropertyException ex)
            {
                StringAssert.Contains("The property 'NonExistentReflectiveProp' on 'ContactPersonTestBO' cannot be found", ex.Message);
            }

        }

    }
}