using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace smms_uploader
{
    public static class UploadUtils
    {
        public static (bool, string) UploadSmms(string file)
        {
            var client = new RestClient("https://sm.ms") { Timeout = -1 };
            var resp = client.Execute(new RestRequest(), Method.GET);
            var request = new RestRequest("api/v2/upload?inajax=1", Method.POST);
            var cookies = resp.Cookies.Select(c => $"{c.Name}={c.Value}");
            request.AddHeader("cookie", string.Join("; ", cookies));
            // request.AddHeader("cookie", "PHPSESSID=tsitoe2aoo7t4ettf6ant3ln16; SM_FC=hSIGWqEQEkD4gutgpGzNGo43nUr97uKNODuyUQbsKe6P; _pk_ref.2.dae2=%5B%22%22%2C%22%22%2C1609136616%2C%22https%3A%2F%2Flink.zhihu.com%2F%3Ftarget%3Dhttps%3A%2F%2Fsm.ms%2F%22%5D; _pk_id.2.dae2=80aaff6274f52275.1604134690.; _pk_ses.2.dae2=1; gznotes-visited=true");
            // request.AddFile("smfile", @"C:/Users/Qing/Desktop/1bc2d4ee89b59f4abad37c8f113753d5.png");
            request.AddFile("smfile", file);
            request.AddParameter("file_id", "0");
            var response = client.Execute(request);
            // Console.WriteLine(response.Content);
            var smResult = JsonConvert.DeserializeObject<SmResult>(response.Content);
            if (smResult.Success)
            {
                // Console.WriteLine(smResult.Data.Url);
                return (true, smResult.Data.Url);
            }
            else
            {
                if (!string.IsNullOrEmpty(smResult.Images))
                {
                    // Console.WriteLine(smResult.Images);
                    return (true, smResult.Images);
                }
                else
                {
                    // Console.WriteLine(smResult.Error);
                    return (false, smResult.Error);
                }
            }
        }
    }

    /// <summary>
    /// When Error:
    /// {"success":false,"code":"image_repeated","error":"Image upload repeated limit, this image exists at:https:\/\/i.loli.net\/2021\/01\/12\/DHQmxwbU3PStfMk.jpg","images":"https:\/\/i.loli.net\/2021\/01\/12\/DHQmxwbU3PStfMk.jpg","RequestId":"3A3BA266-F74E-4A1A-AC5C-D5150E77E88B"}
    /// When Success:
    /// {"success":true,"code":"success","message":"Upload success.","data":{"file_id":0,"width":542,"height":478,"filename":"1bc2d4ee89b59f4abad37c8f113753d5_waifu2x_2x_3n_png.png","storename":"V7B63mK4bp5OYWq.png","size":224035,"path":"\/2021\/01\/12\/V7B63mK4bp5OYWq.png","hash":"UTJidk7GvuABH8LYeRtahfy2SO","url":"https:\/\/i.loli.net\/2021\/01\/12\/V7B63mK4bp5OYWq.png","delete":"https:\/\/sm.ms\/delete\/UTJidk7GvuABH8LYeRtahfy2SO","page":"https:\/\/sm.ms\/image\/V7B63mK4bp5OYWq"},"RequestId":"B45316A4-13D6-47B6-A29A-00B634446AA3"}
    /// </summary>
    public class SmResult
    {
        public bool Success { get; set; }
        public string Code { get; set; }

        public string Error { get; set; }
        public string Images { get; set; }

        public string Message { get; set; }
        public Data Data { get; set; }

        public string RequestId { get; set; }
    }

    public class Data
    {
        [JsonProperty("file_id")]
        public int FileId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        [JsonProperty("filename")]
        public string FileName { get; set; }
        [JsonProperty("storename")]
        public string StoreName { get; set; }
        public int Size { get; set; }
        public string Path { get; set; }
        public string Hash { get; set; }
        public string Url { get; set; }
        public string Delete { get; set; }
        public string Page { get; set; }
    }
}
