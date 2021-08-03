using Sepidar.Normalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sepidar.EntityFramework
{
    public class PersianInterceptor //: IDbCommandInterceptor
    {
        /* 
         * todo: use cases might be:
         * creating a monitoring for detecting and diagnosing long-running database operations
         * Maybe this can help in multinenancy
        */
        //public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        //{
        //}

        //public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        //{
        //    SafeEncode(command);
        //}

        //public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        //{
        //}

        //public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        //{
        //    SafeEncode(command);
        //}

        //public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        //{
        //}

        //public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        //{
        //    SafeEncode(command);
        //}

        private static void SafeEncode(DbCommand command)
        {
            command.CommandText = command.CommandText.SafePersianEncode();
            command.Parameters.OfType<DbParameter>().ToList().ForEach(i =>
            {
                var stringTypes = new DbType[]
                {
                    DbType.AnsiString,
                    DbType.AnsiStringFixedLength,
                    DbType.String,
                    DbType.StringFixedLength
                };
                if (stringTypes.Contains(i.DbType))
                {
                    i.Value = (object)i.Value.ToString().SafePersianEncode();
                }
            });
        }
    }
}
