using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sepidar.Framework
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>()
        {
            Expression<Func<T, bool>> result = f => true;
            return result;
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            Expression<Func<T, bool>> result = f => false;
            return result;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> firstExpression, Expression<Func<T, bool>> secondExpression)
        {
            var invokedExpression = Expression.Invoke(secondExpression, firstExpression.Parameters.Cast<Expression>());
            var result = Expression.Lambda<Func<T, bool>>(Expression.OrElse(firstExpression.Body, invokedExpression), firstExpression.Parameters);
            return result;
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> firstExpression, Expression<Func<T, bool>> secondExpression)
        {
            var invokedExpression = Expression.Invoke(secondExpression, firstExpression.Parameters.Cast<Expression>());
            var result = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(firstExpression.Body, invokedExpression), firstExpression.Parameters);
            return result;
        }
    }
}
