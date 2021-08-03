using Microsoft.EntityFrameworkCore;
using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace Sepidar.EntityFramework
{
    public abstract class Repository<T> : ViewRepository<T>, ICrud<T> where T : class
    {
        string bulkOperationTempTablePrefix = "TempBulkOperation";
        static object lockToken = new object();

        public Repository(DbContext dbContext)
            : base(dbContext)
        {
        }

        public void OnBufferFlushError(Action<object[]> handler)
        {
            lock (lockToken)
            {
                BufferManager.RegisterBufferFlushErrorHandler(BufferKey(), handler);
            }
        }

        public virtual void Delete(long id)
        {
            var entity = Get(id);
            if (entity.IsNull())
            {
                return;
            }
            var entry = context.Entry(entity);
            
            entry.State = EntityState.Deleted;
            context.SaveChanges();
        }

        public virtual void Delete(T t)
        {
            var entity = GetIfExists(t);
            if (entity.IsNotNull())
            {
                var entry = context.Entry(entity);
                entry.State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public virtual string TypeName
        {
            get
            {
                return typeof(T).Name;
            }
        }

        public virtual void BufferToFlush(T t)
        {
            lock (lockToken)
            {
                BufferManager.AddToBuffer(BufferKey(), t, FlushBuffer);
            }
        }

        public void FlushBuffer()
        {
            var entities = BufferManager.Get(BufferKey()).Select(i => (T)i).ToList();
            if (entities.Count == 0)
            {
                return;
            }

            BulkInsert(entities);

            BufferManager.Empty(BufferKey());
        }

        public string BufferKey()
        {
            return typeof(T).FullName;
        }

        public virtual T Create(T t)
        {
            if (t.IsNull())
            {
                throw new FrameworkException("{0} is null".Fill(typeof(T).Name));
            }
            try
            {
                dbset.Add(t);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                SqlExceptionHelper.HandleSqlException(ex, TypeName);
            }
            return t;
        }

        public virtual T Update(T t)
        {
            DbSet<T> _dbset = context.Set<T>();
            _dbset.Attach(t);
            context.Entry(t).State = EntityState.Modified;
            context.SaveChanges();

            return t;
//            using (var _context = (Activator.CreateInstance(context.GetType()) as DbContext))
//            {
//                DbSet<T> _dbset = _context.Set<T>();
//                _dbset.Attach(t);
//                _context.Entry(t).State = EntityState.Modified;
//                _context.SaveChanges();
//            }
//            return t;
        }

        public T Upsert(T t)
        {
            T _temp = Get(ExistenceFilter(t));
            if (_temp.IsNull())
            {
                return Create(t);
            }
            else
            {
                _temp = _temp.GetNewValues<T>(t, "Id");
                t.GetType().GetProperty("Id").SetValue(t, _temp.GetType().GetProperty("Id").GetValue(_temp));
                return Update(_temp);
            }
        }

        public void BulkInsert(List<T> entities)
        {
            if (entities.Count == 0)
            {
                return;
            }
            var tempTableName = GenerateTempTableName();
            BulkInsert(entities, TableName ?? tempTableName);
        }

        private string GenerateTempTableName()
        {
            var tempTableName = "dbo.{0}{1}{2}".Fill(bulkOperationTempTablePrefix, typeof(T).Name, Guid.NewGuid().ToString().Replace("-", ""));
            return tempTableName;
        }

        public void DuplicateTableIfItsTemp(string targetTableName)
        {
            if (!targetTableName.Contains(bulkOperationTempTablePrefix))
            {
                return;
            }
            var dbConnection = context.Database.GetDbConnection();
            Sql.Database.Open(dbConnection.ConnectionString).Run(TempTableCreationScript(targetTableName));
        }

        private string BulkInsert(List<T> entities, string tableName)
        {
            if (entities.Count == 0)
            {
                return tableName;
            }
            var table = ConfigureDataTable();
            foreach (var entity in entities)
            {
                AddRecord(table, entity);
            }
            DuplicateTableIfItsTemp(tableName);
            BulkInsert(table, tableName, SqlBulkCopyOptions.Default);
            RestoreConstratints();
            return tableName;
        }

        public void BulkUpdate(List<T> entities)
        {
            if (entities.Count == 0)
            {
                return;
            }
            var tempTableName = GenerateTempTableName();
            BulkInsert(entities, tempTableName);
            var query = @"
merge {0} as t
using {1} as s
on ({2})
when not matched by target
    then insert {3}
when matched
    then update set {4};
".Fill(TableName, tempTableName, BulkUpdateComparisonKey, BulkUpdateInsertClause, BulkUpdateUpdateClause);
            var dbConnection = context.Database.GetDbConnection();
            Sql.Database.Open(dbConnection.ConnectionString).Run(query);
            Sql.Database.Open(dbConnection.ConnectionString).Run("drop table {0}".Fill(tempTableName));
        }

        private void RestoreConstratints()
        {
            if (Config.HasSetting("IgnoreRestoringConstraintsAfterBulkInsert"))
            {
                return;
            }
            var dbConnection = context.Database.GetDbConnection();
            Sql.Database.Open(dbConnection.ConnectionString).Run("alter table {0} with check check constraint all".Fill(TableName));
        }

        public virtual string TableName
        {
            get
            {
                throw new FrameworkException("Override TableName property");
            }
        }

        public virtual void AddRecord(DataTable table, T entity)
        {
            throw new FrameworkException("You need to override AddRecord method");
        }

        public virtual void AddColumnMappings(SqlBulkCopy bulkOperator)
        {
        }

        public virtual DataTable ConfigureDataTable()
        {
            throw new FrameworkException("You need to override ConfigureDataTable method");
        }

        public void BulkInsert(DataTable table, string destinationTableFullyQualifiedName, SqlBulkCopyOptions options = SqlBulkCopyOptions.KeepIdentity)
        {
            if (table.Rows.Count == 0)
            {
                return;
            }
            var dbConnection = context.Database.GetDbConnection();
            string connectionString = dbConnection.ConnectionString;
            using (SqlBulkCopy bulkOperator = new SqlBulkCopy(connectionString, options))
            {
                AddColumnMappings(bulkOperator);
                bulkOperator.BulkCopyTimeout = 10 /* min */ * 60 /* seconds */;
                bulkOperator.NotifyAfter = 50;
                bulkOperator.DestinationTableName = destinationTableFullyQualifiedName;
                bulkOperator.WriteToServer(table);
            }
        }

        public virtual string BulkUpdateComparisonKey
        {
            get
            {
                throw new FrameworkException("Override BulkUpdateComparisonKey property");
            }
        }

        public virtual string BulkUpdateInsertClause
        {
            get
            {
                throw new FrameworkException("Override BulkUpdateInsertClause property");
            }
        }

        public virtual string BulkUpdateUpdateClause
        {
            get
            {
                throw new FrameworkException("Override BulkUpdateUpdateClause property");
            }
        }

        public virtual string TempTableCreationScript(string tempTableName)
        {
            throw new FrameworkException("Override TempTableCreationScript method");
        }

        public void BulkDelete(List<T> items)
        {
            var type = typeof(T).GetProperties().Single(i => i.Name == "Id");
            using (var transaction = new TransactionScope())
            {
                foreach (var item in items)
                {
                    var id = (long)type.GetValue(item);
                    Delete(id);
                }
                transaction.Complete();
            }
        }
    }
}
