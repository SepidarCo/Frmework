using Sepidar.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sepidar.EntityFramework
{
    public interface IRead<T> where T : class
    {
        IQueryable<T> GetList(Expression<Func<T, bool>> filter);

        ListResult<T> GetList(ListOptions listOptions);

        IQueryable<T> All { get; }

        T Get(long id);

        T Get(Expression<Func<T, bool>> filter);

        T GetIfExists(T t);

        bool Exists(Expression<Func<T, bool>> filter);

        bool Exists(T t);

        Expression<Func<T, bool>> ExistenceFilter(T t);

        Expression<Func<T, bool>> KeySelector { get; }
    }
}
