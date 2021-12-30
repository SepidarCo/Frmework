using System;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Sepidar.BlobManagement;

namespace Sepidar.BaseApi
{
    public class BlobController : GeneralController
    {
        [HttpGet]
        public IActionResult Stream(string token)
        {
            var path = DiskBlobRepository.GetDiskPath(token, "");
//            var path = DiskBlobRepository.GetDiskPath(token);
            path = Environment.ExpandEnvironmentVariables(path);

            var bytes = System.IO.File.ReadAllBytes(path);
            var memoryStream = new MemoryStream(bytes);
            return new FileStreamResult(memoryStream, new MediaTypeHeaderValue("video/mp4").MediaType);
        }
    }
}
