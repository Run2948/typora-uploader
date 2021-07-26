using System.Text.Json.Serialization;

namespace minio_uploader
{
    public class UploaderConfig
    {
        [JsonPropertyName("minio-uploader")]
        public MinioConfig MinioConfig { get; set; }

        public static UploaderConfig InitMinio()
        {
            return new UploaderConfig()
            {
                MinioConfig = new MinioConfig()
                {
                    endpoint = "127.0.0.1:9000",
                    accessKey = "minioadmin",
                    secretKey = "minioadmin",
                    bucketName = "images",
                    pathFormat = "{yyyy}/{mm}/{dd}/{time}{rand:6}",
                    withSSL = false
                }
            };
        }
    }

    public class MinioConfig
    {
        public string endpoint { get; set; }
        public string accessKey { get; set; }
        public string secretKey { get; set; }
        public bool withSSL { get; set; }
        public string bucketName { get; set; }
        public string pathFormat { get; set; }
    }
}
