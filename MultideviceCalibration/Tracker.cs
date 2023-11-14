
using EyeTribe.ClientSdk;
using EyeTribe.ClientSdk.Data;
using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Calibration
{
    public class Tracker : IGazeListener, IConnectionStateListener
    {
        public string name;
        public int port;
        public string host;
        private bool logging = false;
        public int FPS = 30;
        public GazeManager gazeManager;
        public event EventHandler<bool> EmitConnectionStateChanged, EmitLoggingChanged;


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
        public bool log = false;

        public Tracker(int indexTracker, string _host, int _port)
        {
            this.name = "Tracker " + indexTracker;
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
                Task.Run(() =>
                {
                    Trace.TraceInformation(JsonConvert.SerializeObject(gazeData));
                    Trace.Flush(); // Flush the trace to ensure it's written to the log file
                });
            }
        }

        public void OnConnectionStateChanged(bool IsActivated)
        {
            EmitConnectionStateChanged?.Invoke(this, IsActivated);
        }

        public void ToggleLogging()
        {
            this.logging = !this.logging;
            EmitLoggingChanged?.Invoke(this, this.logging);

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

        public void ToggleLogging(bool l)
        {
            this.logging = l;
            EmitLoggingChanged?.Invoke(this, this.logging);
            Console.Out.WriteLine("after event bool");

            if (l)
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
