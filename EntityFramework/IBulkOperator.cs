using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Sepidar.EntityFramework
{
    public interface IBulkOperator<T>
    {
        void BulkInsert(List<T> entities);

        void AddRecord(DataTable table, T entity);

        DataTable ConfigureDataTable();

        string TableName { get; }
    }
}
