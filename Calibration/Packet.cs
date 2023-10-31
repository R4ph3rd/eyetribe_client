namespace Calibration {
    public class Packet
    {
        public string time = DateTime.UtcNow.Ticks.ToString();
        public string category = string.Empty;
        public string request = string.Empty;
        public string statuscode = string.Empty;
        public string values = string.Empty;
        public string rawData = string.Empty;

        public Packet() { }
    }
}