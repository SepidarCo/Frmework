using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.EntityFramework
{
    public interface ICrud<T> : IRead<T> where T : class
    {
        void Delete(long id);

        T Update(T t);

        T Create(T t);

        T Upsert(T t);
    }
}
