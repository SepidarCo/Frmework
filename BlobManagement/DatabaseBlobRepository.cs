using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepidar.BlobManagement
{
    public class DatabaseBlobRepository : BlobRepository<DatabaseBlobStorage>
    {
        public override string Add(Blob blob)
        {
            using (var context = new BlobDataContext())
            {
                context.Blobs.Add(blob);
                context.SaveChanges();
                return blob.Id.ToString();
            }
        }

        public override Blob Get(string token)
        {
            Guid guid;
            if (Guid.TryParse(token, out guid))
            {
                using (var context = new BlobDataContext())
                {
                    var result = context.Blobs.FirstOrDefault(x => x.Id == guid);
                    return result;
                }
            }
            return null;
        }

        public override Blob GetByName(string name)
        {
            using (var context = new BlobDataContext())
            {
                var blob = context.Blobs.FirstOrDefault(x => x.Name == name);
                return blob;
            }
        }

        public override void Remove(string token)
        {
            Guid guid;
            if (Guid.TryParse(token, out guid))
            {
                using (var context = new BlobDataContext())
                {
                    var file = context.Blobs.FirstOrDefault(x => x.Id == guid);
                    if (file.IsNotNull())
                    {
                        context.Blobs.Remove(file);
                        context.SaveChanges();
                    }
                }
            }
        }

        public List<Blob> List()
        {
            using (var context = new BlobDataContext())
            {
                return context.Blobs.ToList();
            }
        }

        public List<Blob> GetByIds(List<string> tokens)
        {
            var guids = tokens.Select(i => Guid.Parse(i)).ToList();
            using (var context = new BlobDataContext())
            {
                var files = context.Blobs.Where(x => guids.Contains(x.Id)).ToList();
                return files;
            }
        }
    }
}
