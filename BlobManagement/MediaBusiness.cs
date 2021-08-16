using Microsoft.Extensions.Configuration;
using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.IO;
using System.Linq;

namespace Sepidar.BlobManagement
{
    public class MediaBusiness : IMediaBusiness
    {
        private IConfiguration configuration;

        public MediaBusiness(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Uri MakeUrlFromFileToken(string token, long? mediaType)
        {
            Uri result = null;

            if (token.IsSomething())
            {
                string fileExtension = string.Empty;

                if (mediaType == 0)
                {
                    fileExtension = ".jpg";
                }
                else if (mediaType == 1)
                {
                    fileExtension = ".mp3";
                }
                else if (mediaType == 2)
                {
                    fileExtension = ".mp4";
                }

                Uri hostName = new Uri(configuration["Settings:AdminUrl"]);
                var filePath = "media/" + token;

                result = new Uri(hostName, filePath + fileExtension);
            }

            return result;
        }

        public Uri MakeUrlFromFileTokenSsl(string token, long? mediaType)
        {
            Uri result = null;

            if (token.IsSomething())
            {
                string fileExtension = string.Empty;

                if (mediaType == 0)
                {
                    fileExtension = ".jpg";
                }
                else if (mediaType == 1)
                {
                    fileExtension = ".mp3";
                }
                else if (mediaType == 2)
                {
                    fileExtension = ".mp4";
                }

                Uri hostName = new Uri(configuration["Settings:SiteUrl"]);
                var filePath = "media/" + token;

                result = new Uri(hostName, filePath + fileExtension);
            }

            return result;
        }

        public string Add(Blob blob)
        {
            if (!Path.HasExtension(blob.Name))
            {
                throw new FrameworkException("Blob name {0} should have an extention to be stored on disk".Fill(blob.Name));
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

            var extention = Path.GetExtension(blob.Name);

            string unc;
            if (extention == ".jpeg" || extention == ".png")
            {
                unc = GetDiskPath(blob.Id.ToString(), ".jpg");
            }
            else
            {
                unc = GetDiskPath(blob.Id.ToString(), Path.GetExtension(blob.Name));
            }

            unc = Environment.ExpandEnvironmentVariables(unc);

            if (!Directory.Exists(Path.GetDirectoryName(unc)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(unc));
            }

            File.WriteAllBytes(unc, blob.Content);
            return blob.Id.ToString();

        }

        public string GetDiskPath(string token, string extension)
        {
            extension = extension.Replace(".", "");
            var unc = @"{0}\{1}.{2}".Fill(Config.DiskBlobsRoot, token, extension);
            return Environment.ExpandEnvironmentVariables(unc);
        }
    }
}
