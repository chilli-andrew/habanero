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

using System.Data;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.DB;
using Habanero.Util;
using Habanero.BO;
using NUnit.Framework;

namespace Habanero.Test.General
{
    /// <summary>
    /// This class tests an inheritance heirarchy of three classes, with the
    /// children both using single table inheritance
    /// </summary>
    [TestFixture]
    public class TestInheritanceHeirarchySingleTable : TestInheritanceHeirarchyBase
    {
        [TestFixtureSetUp]
        public void SetupFixture()
        {
            SetupTestForFilledCircleNoPK();
        }

        protected override void SetupInheritanceSpecifics()
        {
            CircleNoPrimaryKey.GetClassDef().SuperClassDef =
                new SuperClassDef(Shape.GetClassDef(), ORMapping.SingleTableInheritance);
            FilledCircleNoPrimaryKey.GetClassDef().SuperClassDef =
                new SuperClassDef(CircleNoPrimaryKey.GetClassDef(), ORMapping.SingleTableInheritance);
            CircleNoPrimaryKey.GetClassDef().SuperClassDef.Discriminator = "ShapeType";
            FilledCircleNoPrimaryKey.GetClassDef().SuperClassDef.Discriminator = "ShapeType";
        }

        protected override void SetStrID()
        {
            itsFilledCircleId = (string) DatabaseUtil.PrepareValue(itsFilledCircle.GetPropertyValue("ShapeID"));
        }

        [Test]
        public void TestFilledCircleIsUsingSingleTableInheritance()
        {
            Assert.AreEqual(ORMapping.SingleTableInheritance, CircleNoPrimaryKey.GetClassDef().SuperClassDef.ORMapping);
            Assert.AreEqual(ORMapping.SingleTableInheritance, FilledCircleNoPrimaryKey.GetClassDef().SuperClassDef.ORMapping);
        }

        [Test]
        public void TestFilledCircleHasShapeIDAsPrimaryKey()
        {
            try
            {
                itsFilledCircle.ID.Contains("ShapeID");
            }
            catch (HabaneroArgumentException)
            {
                Assert.Fail("An object using SingleTableInheritance should receive its superclass as its primary key.");
            }
        }

        [Test]
        public void TestFilledCircleHasCorrectPropertyNames()
        {
            itsFilledCircle.GetPropertyValue("ShapeName");
            itsFilledCircle.GetPropertyValue("Radius");
            itsFilledCircle.GetPropertyValue("ShapeID");
            itsFilledCircle.GetPropertyValue("Colour");
        }

        [Test, ExpectedException(typeof (InvalidPropertyNameException))]
        public void TestFilledCircleDoesntHaveCircleID()
        {
            itsFilledCircle.GetPropertyValue("CircleID");
            itsFilledCircle.GetPropertyValue("FilledCircleID");
        }


        [Test]
        public void TestCircleSelectSql()
        {
            Assert.AreEqual("SELECT Shape.Colour, Shape.Radius, Shape.ShapeID, Shape.ShapeName FROM Shape WHERE ShapeType = 'FilledCircleNoPrimaryKey' AND ShapeID = ?Param0",
                            itsSelectSql.Statement.ToString(),//.Substring(0, 76),
                            "select statement is incorrect for Single Table inheritance");
        }

        [Test]
        public void TestCircleInsertSql()
        {
            Assert.AreEqual(1, itsInsertSql.Count,
                            "There should only be one insert Sql statement when using Single Table Inheritance.");
            Assert.AreEqual(
                "INSERT INTO Shape (Colour, Radius, ShapeID, ShapeName, ShapeType) VALUES (?Param0, ?Param1, ?Param2, ?Param3, ?Param4)",
                itsInsertSql[0].Statement.ToString(), "Concrete Table Inheritance insert Sql seems to be incorrect.");
            Assert.AreEqual(3, ((IDbDataParameter) itsInsertSql[0].Parameters[0]).Value,
                            "Parameter Colour has incorrect value");
            Assert.AreEqual("MyFilledCircle", ((IDbDataParameter) itsInsertSql[0].Parameters[3]).Value,
                            "Parameter ShapeName has incorrect value");
            Assert.AreEqual(itsFilledCircleId, ((IDbDataParameter) itsInsertSql[0].Parameters[2]).Value,
                            "Parameter ShapeID has incorrect value");
            Assert.AreEqual(10, ((IDbDataParameter) itsInsertSql[0].Parameters[1]).Value,
                            "Parameter Radius has incorrect value");
        }

        [Test]
        public void TestCircleUpdateSql()
        {
            Assert.AreEqual(1, itsUpdateSql.Count,
                            "There should only be one update sql statement when using single table inheritance.");
            Assert.AreEqual(
                "UPDATE Shape SET Colour = ?Param0, Radius = ?Param1, ShapeName = ?Param2 WHERE ShapeID = ?Param3",
                itsUpdateSql[0].Statement.ToString());
            Assert.AreEqual(3, ((IDbDataParameter) itsUpdateSql[0].Parameters[0]).Value,
                            "Parameter Colour has incorrect value");
            Assert.AreEqual("MyFilledCircle", ((IDbDataParameter) itsUpdateSql[0].Parameters[2]).Value,
                            "Parameter ShapeName has incorrect value");
            //Assert.AreEqual(itsFilledCircleId, ((IDbDataParameter) itsUpdateSql[0].Parameters[2]).Value,
            //                "Parameter ShapeID has incorrect value");
            Assert.AreEqual(10, ((IDbDataParameter) itsUpdateSql[0].Parameters[1]).Value,
                            "Parameter Radius has incorrect value");
            Assert.AreEqual(itsFilledCircleId, ((IDbDataParameter) itsUpdateSql[0].Parameters[3]).Value,
                            "Parameter ShapeID has incorrect value");
        }

        [Test]
        public void TestCircleDeleteSql()
        {
            Assert.AreEqual(1, itsDeleteSql.Count,
                            "There should only be one delete sql statement when using single table inheritance.");
            Assert.AreEqual("DELETE FROM Shape WHERE ShapeID = ?Param0", itsDeleteSql[0].Statement.ToString(),
                            "Delete Sql for single table inheritance is incorrect.");
            Assert.AreEqual(itsFilledCircleId, ((IDbDataParameter) itsDeleteSql[0].Parameters[0]).Value,
                            "Parameter ShapeID has incorrect value for delete sql when using Single Table inheritance.");
        }

        // TODO: Would like to separate these tests out later, but needs a structure
        //  change and I'm out of time right now.
        [Test]
        public void TestDatabaseReadWrite()
        {
            // Test inserting & selecting
            Shape shape = new Shape();
            shape.ShapeName = "MyShape";
            shape.Save();

            BusinessObjectCollection<Shape> shapes = new BusinessObjectCollection<Shape>();
            shapes.LoadAll();
            Assert.AreEqual(1, shapes.Count);

            BusinessObjectCollection<CircleNoPrimaryKey> circles = new BusinessObjectCollection<CircleNoPrimaryKey>();
            circles.LoadAll();
            Assert.AreEqual(0, circles.Count);

            BusinessObjectCollection<FilledCircleNoPrimaryKey> filledCircles = new BusinessObjectCollection<FilledCircleNoPrimaryKey>();
            filledCircles.LoadAll();
            Assert.AreEqual(0, filledCircles.Count);

            CircleNoPrimaryKey circle = new CircleNoPrimaryKey();
            circle.Radius = 5;
            circle.ShapeName = "Circle";
            circle.Save();

            shapes.LoadAll();
            Assert.AreEqual(2, shapes.Count);
            Assert.AreEqual("MyShape", shapes[0].ShapeName);
            Assert.AreEqual("Circle", shapes[1].ShapeName);

            circles.LoadAll();
            Assert.AreEqual(1, circles.Count);
            Assert.AreEqual(circles[0].ShapeID, shapes[1].ShapeID);
            Assert.AreEqual(5, circles[0].Radius);
            Assert.AreEqual("Circle", circles[0].ShapeName);

            FilledCircleNoPrimaryKey filledCircle = new FilledCircleNoPrimaryKey();
            filledCircle.Colour = 3;
            filledCircle.Radius = 7;
            filledCircle.ShapeName = "FilledCircle";
            filledCircle.Save();

            shapes.LoadAll();
            Assert.AreEqual(3, shapes.Count);
            Assert.AreEqual("MyShape", shapes[0].ShapeName);
            Assert.AreEqual("Circle", shapes[1].ShapeName);
            Assert.AreEqual("FilledCircle", shapes[2].ShapeName);

            circles.LoadAll();
            Assert.AreEqual(2, circles.Count);
            Assert.AreEqual(circles[1].ShapeID, shapes[2].ShapeID);
            Assert.AreEqual(7, circles[1].Radius);
            Assert.AreEqual("FilledCircle", circles[1].ShapeName);

            filledCircles.LoadAll();
            Assert.AreEqual(1, filledCircles.Count);
            Assert.AreEqual(filledCircles[0].ShapeID, shapes[2].ShapeID);
            Assert.AreEqual(7, filledCircles[0].Radius);
            Assert.AreEqual("FilledCircle", filledCircles[0].ShapeName);
            Assert.AreEqual(3, filledCircles[0].Colour);

            // Test updating
            shape.ShapeName = "MyShapeChanged";
            shape.Save();
            circle.ShapeName = "CircleChanged";
            circle.Radius = 10;
            circle.Save();
            filledCircle.ShapeName = "FilledCircleChanged";
            filledCircle.Radius = 12;
            filledCircle.Colour = 4;
            filledCircle.Save();

            shapes.LoadAll();
            Assert.AreEqual("MyShapeChanged", shapes[0].ShapeName);
            Assert.AreEqual("CircleChanged", shapes[1].ShapeName);
            Assert.AreEqual("FilledCircleChanged", shapes[2].ShapeName);
            circles.LoadAll();
            Assert.AreEqual(10, circles[0].Radius);
            Assert.AreEqual(12, circles[1].Radius);
            Assert.AreEqual("CircleChanged", circles[0].ShapeName);
            Assert.AreEqual("FilledCircleChanged", circles[1].ShapeName);
            filledCircles.LoadAll();
            Assert.AreEqual(4, filledCircles[0].Colour);
            Assert.AreEqual(12, filledCircles[0].Radius);
            Assert.AreEqual("FilledCircleChanged", filledCircles[0].ShapeName);

            // Test deleting
            shape.Delete();
            shape.Save();
            circle.Delete();
            circle.Save();
            filledCircle.Delete();
            filledCircle.Save();

            shapes.LoadAll();
            Assert.AreEqual(0, shapes.Count);
            circles.LoadAll();
            Assert.AreEqual(0, circles.Count);
            filledCircles.LoadAll();
            Assert.AreEqual(0, filledCircles.Count);
        }

        // Provided in case the above test fails and the rows remain in the database
        [TestFixtureTearDown]
        public void TearDown()
        {
            Shape shape = BOLoader.Instance.GetBusinessObject<Shape>(
                "ShapeName = 'MyShape' OR ShapeName = 'MyShapeChanged'");
            if (shape != null)
            {
                shape.Delete();
                shape.Save();
            }

            CircleNoPrimaryKey circle = BOLoader.Instance.GetBusinessObject<CircleNoPrimaryKey>(
                "ShapeName = 'Circle' OR ShapeName = 'CircleChanged'");
            if (circle != null)
            {
                circle.Delete();
                circle.Save();
            }

            FilledCircleNoPrimaryKey filledCircle = BOLoader.Instance.GetBusinessObject<FilledCircleNoPrimaryKey>(
                "ShapeName = 'FilledCircle' OR ShapeName = 'FilledCircleChanged'");
            if (filledCircle != null)
            {
                filledCircle.Delete();
                filledCircle.Save();
            }
        }
    }
}