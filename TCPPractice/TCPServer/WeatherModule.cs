using System;
using System.Collections.Generic;
using System.Text;

namespace TCPServer
{
    public class WeatherResponse
    {
        public TemperatureInfo Main { get; set; }
        public string Name { get; set; }
    }

    public class TemperatureInfo
    {
        public float Temp { get; set; }
    }
}
