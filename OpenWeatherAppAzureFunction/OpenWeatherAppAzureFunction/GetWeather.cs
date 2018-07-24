using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenWeatherMap_MunichWeather.model;
using OpenWeatherMap_MunichWeather.Models;

namespace OpenWeatherMap_MunichWeather
{
    public static class GetWeather
    {
        private static readonly HttpClient _http = new HttpClient();
        
        /** Azure function that is executed every 15 minutes to call a public Weather API
         * Once the data is loaded and parsed into the desired format it is passed along to a BlobStorage for further processing
         */
        [FunctionName("GetWeather")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, TraceWriter log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                             .SetBasePath(context.FunctionAppDirectory)
                             .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                             .AddEnvironmentVariables()
                             .Build();

            string apiKey = config["ApiKey"]; //Robert Schlaeger API Key for OpenWeatherMap

            HttpResponseMessage result = await _http.GetAsync("http://api.openweathermap.org/data/2.5/weather?id=3220838&units=metric&lang=de&APPID=" + apiKey);
            var resOb = JsonConvert.DeserializeObject<OpenWeatherMapsModel>(await result.Content.ReadAsStringAsync());

            log.Info(ParseExternalModel(resOb).City);

            //TODO: Implement Parsing
            //TODO: Implement Connection to Blob Storage and dropping of the parsed data
            

        }
        
        /** Parse the external model to fit into our Weather model
         * @param model The model to pass into
         */
        static private ChandelierModel ParseExternalModel(OpenWeatherMapsModel model)
        {
            //TODO: implement logic here
            ChandelierModel result = new ChandelierModel();
            result.City = model.name.Split(' ')[1];
            result.CloudStatus = model.clouds.all;
            result.Temperature = model.main.temp;
            return result;
        }


    }
}
