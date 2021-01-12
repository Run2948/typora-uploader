using System;
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
                Console.WriteLine("Upload Error: There is no any files.");
                return;
            }

            var file = args.FirstOrDefault();
#endif
            var (success, result) = UploadUtils.UploadSmms(file);

            if (success)
            {
                Console.WriteLine("Upload Success:");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"Upload Error: {result}");
            }
        }
    }
}
