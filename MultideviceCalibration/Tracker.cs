
using EyeTribe.ClientSdk;
using EyeTribe.Controls.Calibration;
using EyeTribe.Controls.Cursor;
using EyeTribe.Controls.TrackBox;
using EyeTribe.ClientSdk.Data;
using System.Windows.Interop;
using EyeTribe.ClientSdk;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Controls;

namespace Calibration
{
    public class Tracker
    {
        public string name;
        public int port;
        public string host;
        public GazeManager gazeManager;

        public Tracker()
        {
        }
    }
}
