using Microsoft.AspNetCore.StaticFiles;
using Minio;
using Minio.Exceptions;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace minio_uploader
{
    public static class UploadUtils
    {
        public static async Task<(bool, string)> UploadMinio(MinioClient client, MinioConfig config, string file)
        {
            try
            {
                // Make a bucket on the server, if not already present.
                if (!await client.BucketExistsAsync(config.bucketName))
                    await client.MakeBucketAsync(config.bucketName);

                // Make sure the fileName is legal. Trim Chinese, TrimStart '-'
                var objectName = Path.GetFileName(file);
                objectName = objectName.TrimStart('-');
                var reg = new Regex(@"[\u4e00-\u9fa5]");
                objectName = reg.Replace(objectName, "");
                objectName = PathFormat(objectName, config.pathFormat);

                // Upload a file to bucket.
                await client.PutObjectAsync(config.bucketName, objectName, file, ParseContentType(file));
                return (true, $"http{(config.withSSL ? "s" : "")}://{config.endpoint}/{config.bucketName}/{objectName}");
            }
            catch (MinioException e)
            {
                return (false, $"{e.Message}");
            }
        }

        public static string PathFormat(string originFileName, string pathFormat)
        {
            if (string.IsNullOrWhiteSpace(pathFormat))
                pathFormat = "{filename}{rand:6}";

            if (pathFormat.StartsWith('/'))
                pathFormat = pathFormat[1..];

            var invalidPattern = new Regex(@"[\\\/\:\*\?\042\<\>\|]");
            originFileName = invalidPattern.Replace(originFileName, "");

            string extension = Path.GetExtension(originFileName);
            string filename = Path.GetFileNameWithoutExtension(originFileName);

            pathFormat = pathFormat.Replace("{filename}", filename);
            pathFormat = new Regex(@"\{rand(\:?)(\d+)\}", RegexOptions.Compiled).Replace(pathFormat, match =>
            {
                var digit = 6;
                if (match.Groups.Count > 2)
                {
                    digit = Convert.ToInt32(match.Groups[2].Value);
                }
                var rand = new Random();
                return rand.Next((int)Math.Pow(10, digit), (int)Math.Pow(10, digit + 1)).ToString();
            });

            pathFormat = pathFormat.Replace("{time}", DateTime.Now.Ticks.ToString());
            pathFormat = pathFormat.Replace("{yyyy}", DateTime.Now.Year.ToString());
            pathFormat = pathFormat.Replace("{yy}", (DateTime.Now.Year % 100).ToString("D2"));
            pathFormat = pathFormat.Replace("{mm}", DateTime.Now.Month.ToString("D2"));
            pathFormat = pathFormat.Replace("{dd}", DateTime.Now.Day.ToString("D2"));
            pathFormat = pathFormat.Replace("{hh}", DateTime.Now.Hour.ToString("D2"));
            pathFormat = pathFormat.Replace("{ii}", DateTime.Now.Minute.ToString("D2"));
            pathFormat = pathFormat.Replace("{ss}", DateTime.Now.Second.ToString("D2"));

            return pathFormat + extension;
        }

        public static string ParseContentType(string file)
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.TryGetContentType(file, out var contentType);
            return contentType;
        }
    }
}
