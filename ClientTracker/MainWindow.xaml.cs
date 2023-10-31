/*
 * Copyright (c) 2013-present, The Eye Tribe. 
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the LICENSE file in the root directory of this source tree. 
 *
 */
using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using EyeTribe.Controls.Calibration;
using EyeTribe.Controls.Cursor;
using EyeTribe.Controls.TrackBox;
using EyeTribe.ClientSdk.Data;
using System.Windows.Interop;
using EyeTribe.ClientSdk;
using MessageBox = System.Windows.MessageBox;

namespace ClientTracker
{
    public partial class MainWindow : IConnectionStateListener
    {
        private Screen activeScreen = Screen.PrimaryScreen;
        private CursorControl cursorControl;

        private bool isCalibrated;

        private string host = "127.0.0.1";
        private int port = 9999; 

        // private Gazepoint gazepoint = new GazePoint();
        private SocketManager socket = new SocketManager();
        public MainWindow()
        {
            InitializeComponent();
            this.ContentRendered += (sender, args) => InitClient();
            // this.KeyDown += MainWindow_KeyDown;
        }

        public void OnConnectionStateChanged(bool isConnected)
        {
            //throw new NotImplementedException();
        }

        private void InitClient()
        {
            // Activate/connect client
            socket.Connect(host, port);
            // GazeManager.Instance.Activate(GazeManager.ApiVersion.VERSION_1_0, GazeManager.ClientMode.Push);

            // Listen for changes in connection to server
            // GazeManager.Instance.AddConnectionStateListener(this);

            // // Fetch current status
            // OnConnectionStateChanged(GazeManager.Instance.IsActivated);

            // // Add a fresh instance of the trackbox in case we reinitialize the client connection.
            // TrackingStatusGrid.Children.Clear();
            // TrackingStatusGrid.Children.Add(new TrackBoxStatus());

            // UpdateState();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            GazeManager.Instance.Deactivate();
        }
    }
}
