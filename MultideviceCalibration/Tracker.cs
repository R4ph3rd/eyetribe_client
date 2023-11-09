
using EyeTribe.ClientSdk;
using EyeTribe.ClientSdk.Data;
using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace Calibration
{
    public class Tracker : IGazeListener, IConnectionStateListener
    {
        public string name;
        public int port;
        public string host;
        private bool logging = false;
        public GazeManager gazeManager;
        public event EventHandler<bool> EmitConnectionStateChanged;

        private string logFilePath;
        private TextWriterTraceListener logFileListener;

        public bool Logging
        {
            get
            {
                return logging;
            }
            set {}
        }

        public Tracker(string _name, string _host, int _port)
        {
            this.name = _name;
            this.host = _host;
            this.port = _port;
            this.gazeManager = GazeManager.Instance;
            this.logFilePath = "./data/" + this.name.Replace(" ", "_") + "-gazeData.log";
        }

        public void DeactivateTracker()
        {
            this.gazeManager.Deactivate();
            this.gazeManager.RemoveConnectionStateListener(this);
        }

        public void ActivateTracker()
        {
            // Activate/connect client
            // Listen for changes in connection to server
            this.gazeManager.Activate(GazeManager.ApiVersion.VERSION_1_0, this.host, this.port);
            this.gazeManager.AddConnectionStateListener(this);
        }

        public void DeactivateGazeListener()
        {
            this.gazeManager.RemoveGazeListener(this);
        }

        public void ActivateGazeListener()
        {
            this.gazeManager.AddGazeListener(this);
        }

        public void OnGazeUpdate(GazeData gazeData)
        {
            if (this.Logging)
            {
                Trace.TraceInformation(JsonConvert.SerializeObject(gazeData)); //{ [this.name] : 
                Trace.Flush(); // Flush the trace to ensure it's written to the log file
                //var x = (int)Math.Round(gazeData.SmoothedCoordinates.X, 0);
                //var y = (int)Math.Round(gazeData.SmoothedCoordinates.Y, 0);
                //if (x == 0 & y == 0) return;
            }
        }

        public void OnConnectionStateChanged(bool IsActivated)
        {
            EmitConnectionStateChanged?.Invoke(this, IsActivated);
        }

        public void ToggleLogging()
        {
            this.logging = !this.logging;

            if (this.logging)
            {
                this.ActivateGazeListener();
                this.logFileListener = new TextWriterTraceListener(logFilePath);
                Trace.Listeners.Add(logFileListener);
            }
            else
            {
                this.DeactivateGazeListener();
                this.logFileListener.Close();
            }
        }
    }
}
