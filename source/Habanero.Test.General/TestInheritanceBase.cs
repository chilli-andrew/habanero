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

using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.DB;
using NUnit.Framework;

namespace Habanero.Test.General
{
    public abstract class TestInheritanceBase : TestUsingDatabase
    {
        protected BusinessObject objCircle;
        protected SqlStatementCollection itsInsertSql;
        protected SqlStatementCollection itsUpdateSql;
        protected SqlStatementCollection itsDeleteSql;
        protected SqlStatement selectSql;
        protected string strID;

        public void SetupTest()
        {
            ClassDef.ClassDefs.Clear();

            this.SetupDBConnection();
            SetupInheritanceSpecifics();
            objCircle = new Circle();
            SetStrID();
            objCircle.SetPropertyValue("ShapeName", "MyShape");
            objCircle.SetPropertyValue("Radius", 10);
            itsInsertSql = objCircle.GetInsertSql();
            itsUpdateSql = objCircle.GetUpdateSql();
            itsDeleteSql = objCircle.GetDeleteSql();
            selectSql = new SqlStatement(DatabaseConnection.CurrentConnection);
            selectSql.Statement.Append(objCircle.SelectSqlStatement(selectSql));
        }

        public void SetupTestWithoutPrimaryKey()
        {
            ClassDef.ClassDefs.Clear();
            this.SetupDBConnection();
            SetupInheritanceSpecifics();
            objCircle = new CircleNoPrimaryKey();
            SetStrID();
            objCircle.SetPropertyValue("ShapeName", "MyShape");
            objCircle.SetPropertyValue("Radius", 10);
            itsInsertSql = objCircle.GetInsertSql();
            itsUpdateSql = objCircle.GetUpdateSql();
            itsDeleteSql = objCircle.GetDeleteSql();
            selectSql = new SqlStatement(DatabaseConnection.CurrentConnection);
            selectSql.Statement.Append(objCircle.SelectSqlStatement(selectSql));
        }

        protected abstract void SetupInheritanceSpecifics();
        protected abstract void SetStrID();
    }
}