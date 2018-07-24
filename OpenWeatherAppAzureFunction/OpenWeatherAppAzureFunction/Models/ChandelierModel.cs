using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWeatherMap_MunichWeather.Models
{
    class ChandelierModel
    {
        public string City { get; set; }
        public string Description { get; set; }
        public double Temperature { get; set; }
        public double RainProbability { get; set; }
        public double  WindSpeed { get; set; }
        public int CloudStatus { get; set; }

    }
}
