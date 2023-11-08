
using EyeTribe.ClientSdk;
using EyeTribe.ClientSdk.Data;

namespace Calibration
{
    public class Tracker
    {
        public string name;
        public int port;
        public string host;
        public bool logging = false;
        public GazeManager gazeManager;

        public Tracker()
        {
        }

        public void OnGazeUpdate(GazeData gazeData)
        {
            if (this.logging)
            {
                Trace.TraceInformation(Newtonsoft.Json.JsonConvert.SerializeObject({[this.name] : gazeData}));
                Trace.Flush();
                var x = (int)Math.Round(gazeData.SmoothedCoordinates.X, 0);
                var y = (int)Math.Round(gazeData.SmoothedCoordinates.Y, 0);
                if (x == 0 & y == 0) return;
                // Invoke thread
                //Dispatcher.BeginInvoke(new Action(() => UpdateState(x, y)));
            }
        }
    }
}
