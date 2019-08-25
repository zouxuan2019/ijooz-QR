using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CoreAPI
{
    public class httpclientService
    {
        public static async Task<object> PostCallAPI(string url, object jsonObject)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonObject.ToString(), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(HttpUtility.UrlEncode(url), content);
                    if (response != null)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<object>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
                // myCustomLogger.LogException(ex);
            }
            return null;
        }

        public static async Task<object> GetCallAPI(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(HttpUtility.UrlEncode(url));
                    if (response != null)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(jsonString);
                        return JsonConvert.DeserializeObject<object>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
                // myCustomLogger.LogException(ex);
            }
            return null;
        }
    }
}
