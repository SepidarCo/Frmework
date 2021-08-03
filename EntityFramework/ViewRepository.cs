using Microsoft.EntityFrameworkCore;
using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sepidar.EntityFramework
{
    public class ViewRepository<T> : IRead<T> where T : class
    {
        protected readonly DbContext context;
        protected readonly DbSet<T> dbset;

        //این بخش روپیدا نکردم بعدا برگرد و بهش نگاه کن
        //public List<T> Query(string query)
        //{
        //    try
        //    {
        //        //var dbConnection = context.Database.GetDbConnection();

        //        return context.Database.SqlQuery<T>(query).ToList();

        //        //return context.Database.fromque; 
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex);
        //        throw new FrameworkException("Query method should return a record set that can be casted to type of {0}".Fill(typeof(T).FullName));
        //    }
        //}

        public ViewRepository(DbContext context)
        {
            this.context = context;
            this.dbset = context.Set<T>();
        }

//        static ViewRepository()
//        {
//            Configuration.InstantiateEntityFrameworkConfiguration();
//        }

        public IQueryable<T> GetList(Expression<Func<T, bool>> filter)
        {
            return All.Where(filter);
        }

        public virtual ListResult<T> GetList(ListOptions listOptions)
        {
            try
            {
                var result = All.ApplyListOptionsAndGetTotalCount(listOptions);
                return result;
            }
            catch (Exception ex)
            {
                LogConnectionString();
                throw ex;
            }
        }

        public IQueryable<T> All
        {
            get
            {
                return dbset;
            }
        }

        public T Get(long id)
        {
            try
            {
                return dbset.Find(id);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw ex;
            }
        }

        public bool Exists(Expression<Func<T, bool>> filter)
        {
            return Get(filter).IsNotNull();
        }

        public bool Exists(T t)
        {
            return GetIfExists(t).IsNotNull();
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            try
            {
                return All.FirstOrDefault(filter);
            }
            catch (Exception ex)
            {
                LogConnectionString();
                throw ex;
            }
        }

        public virtual T GetIfExists(T t)
        {
            return Get(ExistenceFilter(t));
        }

        public virtual Expression<Func<T, bool>> ExistenceFilter(T t)
        {
            throw new FrameworkException("ExistenceFilter expression is not implemented for {0}".Fill(typeof(T).Name));
        }

        public virtual Expression<Func<T, bool>> KeySelector
        {
            get
            {
                throw new FrameworkException("KeySelector expression is not implemented for {0}".Fill(typeof(T).Name));
            }
        }

        public virtual Func<T, bool> Key(T t)
        {
            return ExistenceFilter(t).Compile();
        }

        public DbContext Context
        {
            get { return context; }
        }

        public bool HasData
        {
            get
            {
                try
                {
                    return All.Count() > 0;
                }
                catch (Exception ex)
                {
                    LogConnectionString();
                    throw ex;
                }
            }
        }

        private void LogConnectionString()
        {
            var dbConnection = context.Database.GetDbConnection();
            Logger.LogError("Error in connecting to {0}: ".Fill(dbConnection.ConnectionString));
        }

        public T Random()
        {
            try
            {
                return All.OrderBy(i => Guid.NewGuid()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogConnectionString();
                throw ex;
            }
        }
    }
}
