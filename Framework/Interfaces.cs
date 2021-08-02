using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sepidar.Framework
{
    // Based on ISP (Interface Segregation Principal), I break IManager into three interfaces, namely called IManager, IValidator, and IDefaultProvider.
    public interface IManager<T> where T : class
    {
        IQueryable<T> GetList(Expression<Func<T, bool>> filter);

        IQueryable<T> All { get; }

        T Get(int id);

        T Get(Expression<Func<T, bool>> filter);

        void Delete(int id);

        // todo: Create a delete method which takes a query. Read more at http://stackoverflow.com/questions/869209/bulk-deleting-in-linq-to-entities

        void Update(T t);

        void Create(T t);

        //T Empty { get; }

        //T Default { get; }

        ///// <summary>
        ///// Implementers should throw a kind of validation exception, in case the instance is not a valid instance.
        ///// </summary>
        ///// <param name="instance">The object which should be validated</param>
        //void Validate(T instance);
    }

    public interface IValidator<T> where T : class
    {
        /// <summary>
        /// Implementers should throw a kind of validation exception, in case the instance is not a valid instance.
        /// </summary>
        /// <param name="instance">The object which should be validated</param>
        void Validate(T instance);
    }

    public interface IInstaceProvider<T> where T : class
    {
        T Empty { get; }

        T Default { get; }
    }

    public interface IWebManager<T> : IManager<T> where T : class
    {
        T ExtractFromRequest();
    }

    //public interface IRouteRegistrar
    //{
    //    void RegisterRoutes(RouteCollection routes);
    //}

    public interface IMenuBuilder
    {
        List<MenuItem> Build();
    }
}