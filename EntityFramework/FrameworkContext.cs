using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.EntityFramework
{
    public class FrameworkContext : DbContext
    {
        public FrameworkContext(string connectionStringName)
           // : base(connectionStringName)
        {


        }

        static FrameworkContext()
        {
            //DbInterception.Add(new SafePersianInterceptor());
        }
    }
}
