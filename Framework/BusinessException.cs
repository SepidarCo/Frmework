using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public class BusinessException : ApplicationException
    {
        public string Code { get; set; }

        public BusinessException(string message)
            : base(message)
        {

        }

        public BusinessException(string message, string code)
            : base(message)
        {
            Code = code;
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public BusinessException(string message, string code, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }
    }
}