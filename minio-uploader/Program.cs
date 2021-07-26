using Microsoft.Extensions.Configuration;
using Minio;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace minio_uploader
{
    class Program
    {
        static async Task Main(string[] args)
        {
#if DEBUG
            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
#endif
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Typora", @"conf");
            if (!File.Exists(Path.Combine(basePath, "conf.uploader.json")))
            {
                await File.WriteAllTextAsync(Path.Combine(basePath, "conf.uploader.json"), JsonSerializer.Serialize(UploaderConfig.InitMinio(), new JsonSerializerOptions()
                {
                    WriteIndented = true
                }));
            }
#if DEBUG
            var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"1bc2d4ee89b59f4abad37c8f113753d5.png");
#else
            if (!args.Any())
            {
                Console.WriteLine("Upload Interrupted: ");
                Console.WriteLine("No picture is recognized.");
                return;
            }

            var file = args.FirstOrDefault();
            if (!File.Exists(file))
            {
                Console.WriteLine("Upload Interrupted: ");
                Console.WriteLine("No picture is recognized.");
                return;
            }
#endif
            try
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile(Path.Combine(basePath, "conf.uploader.json"), false, true);
                var configRoot = builder.Build();
                var config = configRoot.GetSection("minio-uploader").Get<MinioConfig>();
#if DEBUG
                Console.WriteLine("\n********************* MinIO Config *********************\n");
                Console.WriteLine($"\tendpoint: {config.endpoint}");
                Console.WriteLine($"\taccessKey: {config.accessKey}");
                Console.WriteLine($"\tsecretKey: {config.secretKey}");
                Console.WriteLine($"\twithSSL: {config.withSSL}");
                Console.WriteLine($"\tbucketName: {config.bucketName}");
                Console.WriteLine($"\tpathFormat: {config.pathFormat}");
                Console.WriteLine("\n*********************************************************\n");
#endif
                var minio = new MinioClient(config.endpoint, config.accessKey, config.accessKey);
                if (config.withSSL) minio = minio.WithSSL();
#if DEBUG
                minio.SetTraceOn();
#endif
                var (success, result) = await UploadUtils.UploadMinio(minio, config, file);
                Console.WriteLine($"Upload {(success ? "Success" : "Error")}: ");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload Error: ");
                Console.WriteLine(ex.Message);
            }
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
