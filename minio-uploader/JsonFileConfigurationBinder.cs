using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace minio_uploader
{
    /// <summary>
    /// 参考：如何将IConfigurationRoot或IConfigurationSection转换为JObject/JSON https://www.it1352.com/2188682.html
    /// </summary>
    public static class JsonFileConfigurationBinder
    {
        public static T Get<T>(this IConfiguration configuration)
        {
            if (!configuration.GetChildren().Any() && configuration is IConfigurationSection section)
                return (T)Convert.ChangeType(section.Value, typeof(T), CultureInfo.InvariantCulture);

            var dict = new Dictionary<string, object>();

            foreach (var child in configuration.GetChildren())
            {
                if (!child.GetChildren().Any() && child is IConfigurationSection childSection)
                {
                    dict.Add(child.Key, childSection.Value);
                }
                else
                {
                    dict.Add(child.Key, Get<object>(child));
                }
            }

            var options = new JsonSerializerOptions();
            options.Converters.Add(new BooleanConverter());

            var result = JsonSerializer.Serialize(dict);
            return JsonSerializer.Deserialize<T>(result, options);
        }
    }

    /// <summary>
    /// 参考：.Net Core 5.0 Json序列化和反序列化 | System.Text.Json 的json序列化和反序列化  https://www.cnblogs.com/tianma3798/p/14090008.html 
    /// </summary>
    public class BooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return bool.Parse(reader.GetString() ?? "False");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
