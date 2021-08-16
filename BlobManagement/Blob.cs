using System;

namespace Sepidar.BlobManagement
{
    public class Blob
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public byte[] Content { get; set; }
    }
}
