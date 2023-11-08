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
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Build.Framework;
using System.Diagnostics;

namespace Calibration
{
    public partial class MainWindow : IConnectionStateListener
    {
        private Screen activeScreen = Screen.PrimaryScreen;

        private string host = "127.0.0.1";
        private int[] ports = {8008, 9009}; //8008 should be the left one

        bool isCalibrated = false;

        private static int maxTrackers = 2;

        private Tracker[] trackers = new Tracker[maxTrackers];
        private Tracker currentTracker;
        private TrackBoxStatus trackbox;

        public MainWindow()
        {
            InitializeComponent();
            this.ContentRendered += (sender, args) => InitClient();
            this.KeyDown += MainWindow_KeyDown;
        }

        #region Components initalization

        private void DeactivateTracker(Tracker tracker)
        {
            tracker.gazeManager.Deactivate();
            tracker.gazeManager.RemoveConnectionStateListener(this);
        }

        private void ActivateTracker(Tracker tracker)
        {
            // Activate/connect client
            // Listen for changes in connection to server
            tracker.gazeManager.Activate(GazeManager.ApiVersion.VERSION_1_0, tracker.host, tracker.port);
            tracker.gazeManager.AddConnectionStateListener(this);
        }


        private Tracker InitTrackerClient(int INDEX_TRACKER)
        {
            Tracker _tracker = new Tracker();
            _tracker.name = "Tracker " + INDEX_TRACKER;
            _tracker.host = host;
            _tracker.port = ports[INDEX_TRACKER];
            _tracker.gazeManager = GazeManager.Instance;

            return _tracker;
        }

        private void InitTrackerStatus()
        {
            // Add a fresh instance of the trackbox in case we reinitialize the client connection.
            trackbox = new TrackBoxStatus();
            TrackingStatusGrid.Children.Clear();
            TrackingStatusGrid.Children.Add(trackbox);
        }

        private void InitClient()
        {
            activeScreen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
            Console.Out.WriteLine("##########################################################");
            Console.Out.WriteLine(activeScreen);
            Console.Out.WriteLine(activeScreen.Bounds.Size);

            for (int i = 0; i < maxTrackers; i++)
                trackers[i] = InitTrackerClient(i);

            currentTracker = trackers[0];
            ActivateTracker(currentTracker);
            InitTrackerStatus();

            UpdateTrackerInfo(
                ((ComboBoxItem)TrackerSelected.SelectedItem).Content.ToString(),
                ports[TrackerSelected.SelectedIndex],
                currentTracker.gazeManager.IsActivated
            );


            // Fetch current status
            OnConnectionStateChanged(currentTracker.gazeManager.IsActivated);

            UpdateState();
        }

        #endregion

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e == null)
                return;

            switch (e.Key)
            {
                // Start calibration on hitting "C"
                case Key.C:
                    ButtonCalibrateClicked(this, null);
                    break;

                case Key.L:
                    ButtonLogClicked(this, null);
                    break;

                // Turn cursor control off on hitting Escape
                case Key.Escape:
                    currentTracker.logging = false;

                    UpdateState();
                    break;
            }
        }

        #region Gaze API Handlers
        private void WindowClosed(object sender, EventArgs e)
        {
            currentTracker.gazeManager.Deactivate();
        }
        private void Calibrate()
        {
            // Update screen to calibrate where the window currently is
            activeScreen = Screen.FromHandle(new WindowInteropHelper(this).Handle);

            // Initialize and start the calibration
            CalibrationRunner calRunner = new CalibrationRunner(activeScreen, activeScreen.Bounds.Size, 16);
            calRunner.OnResult += calRunner_OnResult;
            calRunner.Start();
        }

        private void calRunner_OnResult(object sender, CalibrationRunnerEventArgs e)
        {
            // Invoke on UI thread since we are accessing UI elements
            if (RatingText.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(new MethodInvoker(() => calRunner_OnResult(sender, e)));
                return;
            }

            // Show calibration results rating
            if (e.Result == CalibrationRunnerResult.Success)
            {
                isCalibrated = true;
                UpdateState();
            }
            else
                MessageBox.Show(this, "Calibration failed, please try again");
        }

        public void OnConnectionStateChanged(bool IsActivated)
        {
            // The connection state listener detects when the connection to the EyeTribe server changes
            if (btnCalibrate.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(new MethodInvoker(() => OnConnectionStateChanged(IsActivated)));
                return;
            }

            if (!IsActivated)
                currentTracker.gazeManager.Deactivate();

            UpdateTrackerInfo(currentTracker.name, currentTracker.port, currentTracker.gazeManager.IsActivated);
            UpdateState();
        }

        public void OnGazeUpdate(GazeData gazeData)
        {
            if (currentTracker.logging)
            {
                Trace.TraceInformation(Newtonsoft.Json.JsonConvert.SerializeObject(gazeData));
                Trace.Flush();
                var x = (int)Math.Round(gazeData.SmoothedCoordinates.X, 0);
                var y = (int)Math.Round(gazeData.SmoothedCoordinates.Y, 0);
                if (x == 0 & y == 0) return;
                // Invoke thread
                //Dispatcher.BeginInvoke(new Action(() => UpdateState(x, y)));
            }
        }

        #endregion

        #region UI 

        public void SelectTracker(object sender, EventArgs e){
            if (currentTracker != null)
            {
                DeactivateTracker(currentTracker);
                currentTracker = InitTrackerClient(TrackerSelected.SelectedIndex);
                ActivateTracker(currentTracker);
                InitTrackerStatus();
            }

            if (btnCalibrate!= null)
                OnConnectionStateChanged(currentTracker.gazeManager.IsActivated);
        }

        public void UpdateTrackerInfo(string trackerName, int trackerPort, bool status)
        {
            TrackerName.Text = trackerName;
            TrackerPort.Text = trackerPort.ToString();s
            TrackerStatus.Text = status ? "Active" : "Inactive";

            if (currentTracker.gazeManager.LastCalibrationResult != null)
                RatingText.Text = RatingFunction(currentTracker.gazeManager.LastCalibrationResult);
            else
                RatingText.Text = "Calibration not available";
        }

        private void ButtonCalibrateClicked(object sender, RoutedEventArgs e)
        {
            // Check connectivitiy status
            // API needs to be active to start calibrating
            if (!currentTracker.gazeManager.IsActivated)
            {
                currentTracker.gazeManager.Activate(GazeManager.ApiVersion.VERSION_1_0, currentTracker.host, currentTracker.port);
                UpdateState();
            }
            else if (currentTracker.gazeManager.IsActivated)
                Calibrate();
            else
                UpdateState(); // show reconnect
        }

        private void ButtonLogClicked(object sender, RoutedEventArgs e)
        {
            if (currentTracker.gazeManager.IsCalibrated == false)
                return;
            currentTracker.logging = !currentTracker.logging; // Toggle on/off

            UpdateState();
        }

        // private void ButtonLogAllClicked(object sender, RoutedEventArgs e)
        // {
        //     // check that all trackers are calibrated
        //     foreach(Tracker _tracker in trackers){
        //         if (!_tracker.gazeManager.IsCalibrated)
        //             return;
        //     }

        //     // if so, start clients and logging
        //     foreach(Tracker _tracker in trackers){
        //         if (!_tracker.gazeManager.IsActivated){
        //             ActivateTracker(_tracker);
        //             currentTracker.logging = !currentTracker.logging; // Toggle on/off
        //         }
        //     }

        //     UpdateState();
        // }

        private void UpdateState()
        {
            Console.Out.WriteLine("tracker status : " + currentTracker.gazeManager.IsActivated + " calibrated : " + currentTracker.gazeManager.IsCalibrated);
            // No connection
            if (currentTracker.gazeManager.IsActivated == false)
            {
                btnCalibrate.Content = "Connect";
                btnLog.Content = "";
                RatingText.Text = "";
                return;
            }

            if (currentTracker.gazeManager.IsCalibrated == false)
            {
                btnCalibrate.Content = "Calibrate";
            }
            else
            {
                btnCalibrate.Content = "Recalibrate";

                // Set mouse-button label
                btnLog.Content = "Log On";

                if (currentTracker.logging)
                    btnLog.Content = "Log Off";

                if (currentTracker.gazeManager.LastCalibrationResult != null)
                    RatingText.Text = RatingFunction(currentTracker.gazeManager.LastCalibrationResult);
            }
        }

        private string RatingFunction(CalibrationResult result)
        {
            if (result == null)
                return "";

            double accuracy = result.AverageErrorDegree;

            if (accuracy < 0.5)
                return "Calibration Quality: PERFECT";

            if (accuracy < 0.7)
                return "Calibration Quality: GOOD";

            if (accuracy < 1)
                return "Calibration Quality: MODERATE";

            if (accuracy < 1.5)
                return "Calibration Quality: POOR";

            return "Calibration Quality: REDO";
        }

        #endregion
    }
}