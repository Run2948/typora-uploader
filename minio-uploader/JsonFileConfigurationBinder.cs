using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace minio_uploader
{
    /// <summary>
    /// 参考：如何将IConfigurationRoot或IConfigurationSection转换为JObject/JSON https://www.it1352.com/2188682.html
    /// </summary>
    public class JsonFileConfigurationBinder
    {
        private string _basePath;
        private string _fileName;

        public JsonFileConfigurationBinder SetBasePath(string basePath)
        {
            _basePath = basePath;
            return this;
        }

        public JsonFileConfigurationBinder AddJsonFile(string fileName)
        {
            _fileName = fileName;
            return this;
        }

        public UploaderConfig Build()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new BooleanConverter());
            options.ReadCommentHandling = JsonCommentHandling.Skip;
            options.AllowTrailingCommas = true;
            var content = File.ReadAllText(Path.Combine(_basePath, _fileName), Encoding.Default);
            return JsonSerializer.Deserialize<UploaderConfig>(content, options);
        }
    }

    /// <summary>
    /// 参考：.Net Core 5.0 Json序列化和反序列化 | System.Text.Json 的json序列化和反序列化  https://www.cnblogs.com/tianma3798/p/14090008.html 
    /// </summary>
    public class BooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.False) return false;
            if (reader.TokenType == JsonTokenType.True) return true;
            return Convert.ToBoolean(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
