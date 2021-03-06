﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Habanero.Base.Data
{
    public interface IQueryResultSorter
    {
        void Sort(IQueryResult queryResult, IOrderCriteria orderCriteria);
    }

    public class QueryResultSorter : IQueryResultSorter
    {

        public void Sort(IQueryResult queryResult, IOrderCriteria orderCriteria)
        {
            queryResult.Rows.Sort(new RowComparer(orderCriteria, queryResult));
        }

        private class RowComparer : IComparer<IQueryResultRow>
        {
            private readonly IOrderCriteria _orderCriteria;
            private readonly IQueryResult _queryResult;

            public RowComparer(IOrderCriteria orderCriteria, IQueryResult queryResult)
            {
                _orderCriteria = orderCriteria;
                _queryResult = queryResult;
            }

            public int Compare(IQueryResultRow x, IQueryResultRow y)
            {
                foreach (var orderField in _orderCriteria.Fields)
                {
                    var propertyName = orderField.PropertyName;
                    var fieldIndex = _queryResult.Fields.First(field => field.PropertyName == propertyName).Index;
                    var result = ((IComparable)x.Values[fieldIndex]).CompareTo(y.Values[fieldIndex]);
                    if (orderField.SortDirection == SortDirection.Descending) result = -result;
                    if (result != 0) return result;
                }
                return 0;
            }
        }
    }
}