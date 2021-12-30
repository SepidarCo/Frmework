using Sepidar.Framework;
using System.Reflection.Metadata;

namespace Sepidar.BaseApi
{
    public class BlobFile
    {
        public Blob Blob { get; set; }

        public string Type { get; set; }

        public MediaType? MediaType { get; set; }

        public string FieldName { get; set; }
    }
}
