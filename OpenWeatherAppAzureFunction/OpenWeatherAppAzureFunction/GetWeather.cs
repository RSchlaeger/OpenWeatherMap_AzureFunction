using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenWeatherMap_MunichWeather.model;
using OpenWeatherMap_MunichWeather.Models;
using OpenWeatherMapSharp;
using OpenWeatherMapSharp.Enums;
using OpenWeatherMapSharp.Utils;
using OpenWeatherMapSharp.Models;
using Microsoft.WindowsAzure.Storage.Blob;

namespace OpenWeatherMap_MunichWeather
{
    public static class GetWeather
    {
        private static readonly HttpClient _http = new HttpClient();
        
        /** Azure function that is executed every 15 minutes to call a public Weather API
         * Once the data is loaded and parsed into the desired format it is passed along to a BlobStorage for further processing
         */
        [FunctionName("GetWeather")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, TraceWriter log, ExecutionContext context, [Blob("cache/weatherData.json",System.IO.FileAccess.Write)]CloudBlockBlob blob)
        {
            var config = new ConfigurationBuilder()
                             .SetBasePath(context.FunctionAppDirectory)
                             .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                             .AddEnvironmentVariables()
                             .Build();

            string apiKey = config["ApiKey"]; //Robert Schlaeger API Key for OpenWeatherMap
            OpenWeatherMapService weatherService = new OpenWeatherMapService(apiKey);

            OpenWeatherMapServiceResponse<WeatherRoot> res = await weatherService.GetWeatherAsync("München", LanguageCode.DE);
            log.Info(res.Response.CityId.ToString());

            var outputJson = JsonConvert.SerializeObject(res.Response);
            blob.Properties.ContentType = "application/json";
            await blob.UploadTextAsync(outputJson);

            //TODO: Implement Parsing
           


        }
        
    }
}
