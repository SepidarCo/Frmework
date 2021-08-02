using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public class FrameworkException : ApplicationException
    {
        public FrameworkException(string message)
         : base(message)
        {

        }

        public FrameworkException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
