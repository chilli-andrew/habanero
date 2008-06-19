using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.DB;

namespace Habanero.BO
{
    public class BusinessObjectLoaderDB : IBusinessObjectLoader
    {
        private readonly DataStoreInMemory _dataStoreInMemory;
        private readonly IDatabaseConnection _databaseConnection;

        public BusinessObjectLoaderDB(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
            _dataStoreInMemory = new DataStoreInMemory();
        }

        public T GetBusinessObject<T>(IPrimaryKey key) where T : class, IBusinessObject, new()
        {
            T foundBO = _dataStoreInMemory.Find<T>(key);
            if (foundBO != null) return foundBO;
            return GetBusinessObject<T>(Criteria.FromPrimaryKey(key));
        }

        public T GetBusinessObject<T>(Criteria criteria) where T : class, IBusinessObject, new()
        {
            T foundBO = _dataStoreInMemory.Find<T>(criteria);
            if (foundBO != null) return foundBO;
            SelectQuery selectQuery = QueryBuilder.CreateSelectQuery(ClassDef.ClassDefs[typeof (T)]);
            SelectQueryDB selectQueryDB = new SelectQueryDB(selectQuery);
            ISqlStatement statement = selectQueryDB.CreateSqlStatement();
            using (IDataReader dr = _databaseConnection.LoadDataReader(statement))
            {
                try
                {
                    if (dr.Read())
                    {
                        return LoadBOFromReader<T>(dr, selectQueryDB);
                    }
                }
                finally
                {
                    if (dr != null && !dr.IsClosed)
                    {
                        dr.Close();
                    }
                }
            }
            return null;
        }

        public BusinessObjectCollection<T> GetBusinessObjectCollection<T>(Criteria criteria)
            where T : class, IBusinessObject, new()
        {
            BusinessObjectCollection<T> col = new BusinessObjectCollection<T>();
            col.SelectQuery.Criteria = criteria;
            Refresh(col);
            return col;
        }

        public void Refresh<T>(BusinessObjectCollection<T> collection) where T : class, IBusinessObject, new()
        {
            SelectQueryDB selectQuery = new SelectQueryDB(collection.SelectQuery);
            ISqlStatement statement = selectQuery.CreateSqlStatement();
            BusinessObjectCollection<T> oldCol = collection.Clone();
            collection.Clear();
            using (IDataReader dr = _databaseConnection.LoadDataReader(statement))
            {
                try
                {
                    while (dr.Read())
                    {
                        T loadedBo = LoadBOFromReader<T>(dr, selectQuery);
                        if (oldCol.Contains(loadedBo)) collection.AddInternal(loadedBo);
                        else collection.Add(loadedBo);
                    }
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }
        }

        public RelatedBusinessObjectCollection<T> GetRelatedBusinessObjectCollection<T>(IRelationship relationship)
            where T : class, IBusinessObject, new()
        {
            RelatedBusinessObjectCollection<T> relatedCol = new RelatedBusinessObjectCollection<T>(relationship);
            Criteria relationshipCriteria = Criteria.FromRelationship(relationship);
            GetBusinessObjectCollection<T>(relationshipCriteria).ForEach(delegate(T obj) { relatedCol.Add(obj); });
            relatedCol.SelectQuery.Criteria = relationshipCriteria;
            return relatedCol;
        }

        public BusinessObjectCollection<T> GetBusinessObjectCollection<T>(Criteria criteria, OrderCriteria orderCriteria) where T : BusinessObject, new()
        {
            BusinessObjectCollection<T> col = new BusinessObjectCollection<T>();
            col.SelectQuery.Criteria = criteria;
            col.SelectQuery.OrderCriteria = orderCriteria;
            Refresh(col);
            return col;
        }


        private T LoadBOFromReader<T>(IDataRecord dr, ISelectQuery selectQuery)
            where T : class, IBusinessObject, new()
        {
            IBusinessObject ibo = new T();
            BusinessObject bo = (BusinessObject) ibo;
            int i = 0;
            foreach (QueryField field in selectQuery.Fields.Values)
            {
                bo.Props[field.PropertyName].InitialiseProp(dr[i++]);
            }
            T loadedBo = _dataStoreInMemory.Find<T>(bo.PrimaryKey);
            if (loadedBo == null)
            {
                if (BusinessObject.AllLoadedBusinessObjects().ContainsKey(bo.PrimaryKey.GetObjectId()))
                    ibo = (T) BusinessObject.AllLoadedBusinessObjects()[bo.PrimaryKey.GetObjectId()].Target;
                else
                {
                    //todo: add ibo to the allloadedbusinessobjectscol.
                }
                _dataStoreInMemory.Add(ibo);
                loadedBo = (T) ibo;
            } 
            return loadedBo;
        }
    }
}