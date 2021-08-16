//using Sepidar.Framework.Extensions;
//using System;
//using System.Diagnostics;
//using System.IO;
//using NReco.VideoConverter;
//using Sepidar.BlobManagement;
//using Sepidar.Framework;

//namespace Sepidar.BlobManagement
//{
//    public class VideoJob
//    {
//        public static string CreateAndInsertVideoThumbnail(Blob blob, BlobStorageType blobStorageType = BlobStorageType.Disk, float? frameTime = null, bool getFileNameFromBlobName = true)
//        {
//            var tempFile = Path.GetTempFileName();
//            try
//            {
//                using (var fileStream = File.OpenWrite(tempFile))
//                {
//                    var blobStream = new MemoryStream(blob.Content);
//                    blobStream.CopyTo(fileStream);
//                }

//                var ffMpeg = new FFMpegConverter();
//                using (var thumbJpegStream = new MemoryStream())
//                {
//                    if (frameTime.IsNull())
//                        frameTime = 2;

//                    ffMpeg.GetVideoThumbnail(tempFile, thumbJpegStream, frameTime);

//                    string fileName;
//                    if (getFileNameFromBlobName)
//                    {
//                        fileName = string.Format("{0} -Thumbnail", blob.Name);
//                    }
//                    else
//                    {
//                        fileName = "file.jpg";
//                    }

//                    var thumbnailBlob = new Blob
//                    {
//                        Name = fileName,
//                        Content = thumbJpegStream.ToArray()
//                    };

//                    if (blobStorageType == BlobStorageType.Disk)
//                    {
//                        var blobRepository = new DiskBlobRepository();
//                        blobRepository.Add(thumbnailBlob);
//                    }
//                    else if (blobStorageType == BlobStorageType.Database)
//                    {
//                        var blobRepository = new DatabaseBlobRepository();
//                        blobRepository.Add(thumbnailBlob);

//                    }

//                    return thumbnailBlob.Id.ToString();
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new FrameworkException("Cannot Generate Thumbnail for Video", ex);
//            }
//            finally
//            {
//                File.Delete(tempFile);
//            }
//        }

//        public static string ConvertToMP4(Blob blob, float? seek = null, BlobStorageType blobStorageType = BlobStorageType.Disk, int? videoWidth = null, int? videoHeigth = null)
//        {
//            var tempFile = Path.GetTempFileName();
//            try
//            {
//                using (var fileStream = File.OpenWrite(tempFile))
//                {
//                    var blobStream = new MemoryStream(blob.Content);
//                    blobStream.CopyTo(fileStream);
//                }

//                var ffMpeg = new FFMpegConverter();
//                using (var outputStream = new MemoryStream())
//                {
//                    var settings = new ConvertSettings();
//                    if (seek.IsNull())
//                    {
//                        if (videoWidth.IsNotNull() && videoHeigth.IsNotNull())
//                        {
//                            settings.SetVideoFrameSize((int)videoWidth, (int)videoHeigth);
//                        }
//                        ffMpeg.ConvertMedia(tempFile, Format.mp4, outputStream, Format.mp4, settings);

//                    }
//                    else
//                    {
//                        settings.Seek = seek;
//                        ffMpeg.ConvertMedia(tempFile, Format.mp4, outputStream, Format.mp4, settings);
//                    }

//                    var mp3Blob = new Blob
//                    {
//                        Name = blob.Name,
//                        Content = outputStream.ToArray()
//                    };

//                    if (blobStorageType == BlobStorageType.Disk)
//                    {
//                        var blobRepository = new DiskBlobRepository();
//                        blobRepository.Add(mp3Blob);
//                    }
//                    else if (blobStorageType == BlobStorageType.Database)
//                    {
//                        var blobRepository = new DatabaseBlobRepository();
//                        blobRepository.Add(mp3Blob);

//                    }
//                    return mp3Blob.Id.ToString();
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new FrameworkException("Cannot Convert to MP4", ex);
//            }
//            finally
//            {
//                File.Delete(tempFile);
//            }
//        }

//        public static Blob GetThumbnail(Blob blob)
//        {
//            var tempFile = Path.GetTempFileName();
//            try
//            {
//                using (var fileStream = File.OpenWrite(tempFile))
//                {
//                    var blobStream = new MemoryStream(blob.Content);
//                    blobStream.CopyTo(fileStream);
//                }

//                var ffMpeg = new FFMpegConverter();
//                using (var thumbJpegStream = new MemoryStream())
//                {
//                    ffMpeg.GetVideoThumbnail(tempFile, thumbJpegStream, 2);
//                    var thumbnailBlob = new Blob
//                    {
//                        Name = string.Format("{0}-Thumbnail", blob.Name),
//                        Content = thumbJpegStream.ToArray()
//                    };
//                    return thumbnailBlob;
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new FrameworkException("Cannot Generate Thumbnail for Video", ex);
//            }
//            finally
//            {
//                File.Delete(tempFile);
//            }
//        }

//        public static Blob MergeVideoesAndAudio(string firstVideoPath, string audioPath, string secondVideoPath = null, string[] scaleParameters = null, string workingDirectory = null)
//        {
//            var tempFile = workingDirectory + @"\Temp\" + Guid.NewGuid() + ".mp4";
//            try
//            {
//                var outputFilePath = tempFile;
//                string command;
//                if (secondVideoPath.IsNotNull())
//                {
//                    Logger.LogInfo("Merging two videos with an audio. FirstVideoPath:{0} SecondVideoPath:{1} AudioPath:{2}".Fill(firstVideoPath, secondVideoPath, audioPath));
//                    command = MergeVideosAndAudiosCommand(firstVideoPath, secondVideoPath, audioPath, scaleParameters, outputFilePath);
//                }
//                else
//                {
//                    Logger.LogInfo("Merging a video with an audio. VideoPath:{0} AudioPath:{1}".Fill(firstVideoPath, audioPath));
//                    command = MergeVideoAndAudioCommand(firstVideoPath, audioPath, outputFilePath);
//                }

//                var info = new ProcessStartInfo
//                {
//                    WindowStyle = ProcessWindowStyle.Normal,
//                    FileName = "cmd.exe",
//                    Arguments = @"/c " + command,
//                    WorkingDirectory = workingDirectory
//                };

//                var process = new Process
//                {
//                    StartInfo = info
//                };
//                process.Start();

//                process.WaitForExit();

//                var blob = new Blob
//                {
//                    Name = "file.mp4",
//                    Content = File.ReadAllBytes(tempFile)
//                };

//                if (secondVideoPath.IsNotNull())
//                {
//                    Logger.LogSuccess("Merging two videos with an audio done successfully. FirstVideoPath:{0} SecondVideoPath:{1} AudioPath:{2}".Fill(firstVideoPath, secondVideoPath, audioPath));
//                }
//                else
//                {
//                    Logger.LogSuccess("Merging a video with an audio done successfully. VideoPath:{0} AudioPath:{1}".Fill(firstVideoPath, audioPath));
//                }

//                return blob;
//            }
//            catch (Exception ex)
//            {
//                throw new FrameworkException("Cannot Merge Videoes And Audio", ex);
//            }
//            finally
//            {
//                File.Delete(tempFile);
//            }
//        }

//        private static string MergeVideosAndAudiosCommand(string firstVideoPath, string secondVideoPath, string audioPath, string[] scaleParameters, string outputFilePath)
//        {
//            string command = @"ffmpeg -i {0} -i {1} -i {2}".Fill(firstVideoPath, secondVideoPath, audioPath);

//            if (scaleParameters.IsNotNull())
//            {

//                command += @" -filter_complex ""[0:v]scale={0}:{1}[left]; [1:v]scale={0}:{1}[right]; [left][right]hstack[v];".Fill(scaleParameters[0], scaleParameters[1]);
//            }
//            else
//            {
//                command += @" -filter_complex ""[0:v][1:v]hstack[v];";
//            }
//            command += @"[0:a][1:a][2:a]amerge=inputs=3[a]""";
//            command += @" -map ""[v]"" -map ""[a]"" -shortest {0}".Fill(outputFilePath);
//            return command;
//        }

//        public static string MergeVideoAndAudioCommand(string videoPath, string audioPath, string outputFilePath)
//        {
//            //            string command = @"ffmpeg -i {0} -i {1} -c:v copy -c:a aac -strict experimental -map 0:v:0 -map 1:a:0 {2}".Fill(videoPath, audioPath, outputFilePath);
//            string command = @"ffmpeg -i {0} -i {1} -filter_complex [0:a][1:a]amerge[a] -map [a] -c:v copy -map 0:v:0 {2}".Fill(videoPath, audioPath, outputFilePath);
//            return command;
//        }
//    }
//}
