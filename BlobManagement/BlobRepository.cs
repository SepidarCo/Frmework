using System;

namespace Sepidar.BlobManagement
{
    public abstract class BlobRepository<T> where T : class
    {
        public abstract string Add(Blob blob);

        public abstract Blob Get(string token);

        public abstract Blob GetByName(string name);

        public abstract void Remove(string token);
    }
}
