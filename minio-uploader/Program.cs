﻿using Minio;
using System;
using System.IO;
#if !DEBUG
using System.Linq;
#endif
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
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);
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

            if (
                "-h" == file ||
                "--help" == file
                )
            {
                Console.WriteLine("Open config directory in file explorer: ");
                Console.WriteLine("\texplorer \"%AppData%\\Typora\\conf\\\"");
                return;
            }

            if (!File.Exists(file))
            {
                Console.WriteLine("Upload Interrupted: ");
                Console.WriteLine("No picture is recognized.");
                return;
            }
#endif
            try
            {
                var builder = new JsonFileConfigurationBinder();
                builder
                    .SetBasePath(basePath)
                    .AddJsonFile("conf.uploader.json");
                var config = builder.Build();
                var minioConfig = config.MinioConfig;
                //var withSSL = config.MinioConfig.withSSL;
#if DEBUG
                Console.WriteLine("\n********************* MinIO Config *********************\n");
                Console.WriteLine($"\tendpoint: {minioConfig.endpoint}");
                Console.WriteLine($"\taccessKey: {minioConfig.accessKey}");
                Console.WriteLine($"\tsecretKey: {minioConfig.secretKey}");
                Console.WriteLine($"\twithSSL: {minioConfig.withSSL}");
                Console.WriteLine($"\tbucketName: {minioConfig.bucketName}");
                Console.WriteLine($"\tpathFormat: {minioConfig.pathFormat}");
                Console.WriteLine("\n*********************************************************\n");
#endif
                var minio = new MinioClient(minioConfig.endpoint, minioConfig.accessKey, minioConfig.accessKey);
                if (minioConfig.withSSL) minio = minio.WithSSL();
#if DEBUG
                minio.SetTraceOn();
#endif
                var (success, result) = await UploadUtils.UploadMinio(minio, minioConfig, file);
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
