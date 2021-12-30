using System.Collections.Generic;

namespace Sepidar.BaseApi
{
    public class MultiPartResult<T> where T : class, new()
    {
        public T Model { get; set; }

        public List<BlobFile> Files { get; set; }

        public Dictionary<string, object> Rest { get; set; }
    }
}
