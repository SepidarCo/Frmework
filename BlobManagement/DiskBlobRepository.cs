using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sepidar.BlobManagement
{
    public class DiskBlobRepository : BlobRepository<DiskBlobStorage>
    {
        public override string Add(Blob blob)
        {
            if (!Path.HasExtension(blob.Name))
            {
                throw new FrameworkException(
                    "Blob name {0} should have an extention to be stored on disk".Fill(blob.Name));
            }

            if (blob.Content.IsNull())
            {
                throw new FrameworkException("Blob content is null");
            }

            // file path is like connection string here
            // todo: how to save to the proper server here? for now, I will save to one simple location on the current disk
            if (blob.Id.IsNull() || blob.Id == Guid.Empty)
            {
                blob.Id = Guid.NewGuid();
            }

            var firstCharacter = blob.Id.ToString().First().ToString();
            var serverNumber = Convert.ToInt32(firstCharacter, 16);
            var unc = GetDiskPath(blob.Id.ToString(), Path.GetExtension(blob.Name));
            unc = Environment.ExpandEnvironmentVariables(unc);
            if (!Directory.Exists(Path.GetDirectoryName(unc)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(unc));
            }

            File.WriteAllBytes(unc, blob.Content);
            return blob.Id.ToString();

        }

        public override Blob Get(string token)
        {
            var path = Path.GetDirectoryName(GetDiskPath(token, ""));
            var files = Directory.GetFiles(path, "file.*", SearchOption.TopDirectoryOnly);
            if (files.Count() == 0)
            {
                throw new FrameworkException("No blob is found with the given toke '{0}'".Fill(token));
            }

            var file = files.First();
            var blob = new Blob();
            blob.Id = Guid.Parse(token);
            blob.Name = "{0}.{1}".Fill(token, Path.GetExtension(file).Replace(".", ""));
            blob.Content = File.ReadAllBytes(file);
            return blob;
        }

        public override Blob GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public override void Remove(string token)
        {
            var firstCharacter = token.First().ToString();
            var serverNumber = Convert.ToInt32(firstCharacter, 16);
            var unc = @"{0}\{1}".Fill(Config.DiskBlobsRoot, GetDiskPath(token));
            unc = Environment.ExpandEnvironmentVariables(unc);
            if (Directory.Exists(Path.GetDirectoryName(unc)))
            {
                Directory.Delete(Path.GetDirectoryName(unc), true);
            }
        }

        public static string GetDiskPath(string token, string extension)
        {
            extension = extension.Replace(".", "");
            var unc = @"{0}\{1}file.{2}".Fill(Config.DiskBlobsRoot, GetDiskPath(token.ToString()), extension);
            return Environment.ExpandEnvironmentVariables(unc);
        }

        private static string GetDiskPath(string token)
        {
            token = token.ToLower();
            token = token.ReplaceAll(@"[^0-9a-z]", "", RegexOptions.None);
            var path = Regex.Replace(token, @"([0-9a-z])", @"$1\", RegexOptions.None);
            path = "{0}".Fill(path);
            return Environment.ExpandEnvironmentVariables(path);
        }

        public static string GetUrlPath(string token, string extension)
        {
            extension = extension.Replace(".", "");
            var path = GetDiskPath(token);
            path = "{0}/file.{1}".Fill(path, extension);
            path = path.ReplaceAll(@"\\", @"/", RegexOptions.None);
            return path;
        }
    }
}