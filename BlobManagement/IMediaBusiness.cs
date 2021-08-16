using System;

namespace Sepidar.BlobManagement
{
    public interface IMediaBusiness
    {
        Uri MakeUrlFromFileToken(string token, long? mediaType);

        Uri MakeUrlFromFileTokenSsl(string token, long? mediaType);

        string Add(Blob blob);

        string GetDiskPath(string token, string extension);
    }
}
