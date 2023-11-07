using System;
using Newtonsoft.Json.Linq;

namespace ClientTracker
{
    internal class Packet
    {
        public string time = DateTime.UtcNow.Ticks.ToString();
        public string Category = string.Empty;
        public string Request = string.Empty;
        public string StatusCode = string.Empty;
        //public JToken Values;
        public string Values = string.Empty;
        public string RawData = string.Empty;

        public Packet() { }
        }
}