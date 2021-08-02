using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Sepidar.Framework
{
    public class ImageHelper
    {
        //public static string MakeImageThumbnailAndGetToken(int maxWidth, int maxHeight, Blob imageBlob, BlobStorage blobStorage = BlobStorage.Database)
        //{
        //    Blob thumbnailBlob;
        //    MemoryStream memoryStream = MakeImageThumbnail(maxWidth, maxHeight, imageBlob.Content);
        //    thumbnailBlob = new Blob();
        //    thumbnailBlob.Content = memoryStream.ToArray();
        //    var splitedImageBlobName = imageBlob.Name.Split('.').ToList();
        //    thumbnailBlob.Name = splitedImageBlobName.First() + "Thumbnail" + '.' + splitedImageBlobName.Last();

        //    new BlobRepository().Add(thumbnailBlob, blobStorage);

        //    return thumbnailBlob.Id.ToString();
        //}

        //public static MemoryStream MakeImageThumbnail(int? maxWidth, int? maxHeight, byte[] bytes)
        //{
        //    var stream = new MemoryStream(bytes);
        //    var image = Image.FromStream(stream);

        //    maxWidth = maxWidth ?? image.Width;
        //    maxHeight = maxHeight ?? image.Height;
        //    if (image.Width < maxWidth)
        //        maxWidth = image.Width;

        //    decimal imageRatio = (decimal)image.Width / (decimal)image.Height;
        //    decimal outputWidth = maxWidth.Value;
        //    int outputHeight = Convert.ToInt32(outputWidth / imageRatio);

        //    var thumbnail = image.GetThumbnailImage(Convert.ToInt32(outputWidth), outputHeight, () => false, IntPtr.Zero);
        //    var memoryStream = new MemoryStream((byte[])new ImageConverter().ConvertTo(thumbnail, typeof(byte[])));
        //    //thumbnail.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
        //    return memoryStream;
        //}
    }
}
