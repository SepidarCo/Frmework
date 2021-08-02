using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public interface IEntityValidator<T> where T : class
    {
        void Validate(T t);

        bool IsValid(T t);
    }
}
