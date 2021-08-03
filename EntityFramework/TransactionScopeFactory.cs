using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Sepidar.EntityFramework
{
    public class TransactionScopeFactory
    {
        public static TransactionScope CreateTrasnactionScope(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = isolationLevel,
                Timeout = TransactionManager.MaximumTimeout
            };

            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }
    }
}