using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAppX.Helper
{
    public class ApiCaller
    {
        public static async Task<ApiResponse> Get(string url, string authId = null)
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(authId))
                {
                    client.DefaultRequestHeaders.Add("Authorization", authId);
                }
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Response = await response.Content.ReadAsStringAsync() };
                }
                else
                {
                    return new ApiResponse { ErrorMessage = response.ReasonPhrase };
                }
            }
        }
    }

    public class ApiResponse
    {
        public bool IsSuccessful => ErrorMessage == null;
        public string ErrorMessage { get; set; }
        public string Response { get; set; }
    }
}
