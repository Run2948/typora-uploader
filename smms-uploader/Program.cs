using System;
using System.IO;
using System.Linq;

namespace smms_uploader
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            var file = @"C:/Users/Qing/Desktop/1bc2d4ee89b59f4abad37c8f113753d5.png";
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
            var (success, result) = UploadUtils.UploadSmms(file);

            Console.WriteLine($"Upload {(success ? "Success" : "Error")}:");
            Console.WriteLine(result);
        }
    }
}
