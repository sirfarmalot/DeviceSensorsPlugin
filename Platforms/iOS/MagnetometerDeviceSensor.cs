﻿using CoreMotion;
using Foundation;
using Plugin.DeviceSensors.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.DeviceSensors.Platforms.iOS
{
    public class MagnetometerDeviceSensor : IDeviceSensor<VectorReading>
    {
        CMMotionManager _motionManager;
        VectorReading _lastReading;

        public MagnetometerDeviceSensor(CMMotionManager motionManager)
        {
            _motionManager = motionManager;
        }

        public bool IsSupported => _motionManager.MagnetometerAvailable;

        public bool IsActive => _motionManager.MagnetometerActive;

        public VectorReading LastReading
        {
            get
            {
                VectorReading lastReading = null;
                var data = _motionManager.MagnetometerData;
                if (data != null)
                {
                    lastReading = new VectorReading(data.MagneticField.X, data.MagneticField.Y, data.MagneticField.Z);
                }

                return lastReading;
            }
        }

        public int ReadingInterval { get { return Convert.ToInt32(_motionManager.MagnetometerUpdateInterval); } set { _motionManager.MagnetometerUpdateInterval = value; } }

        public event EventHandler<DeviceSensorReadingEventArgs<VectorReading>> OnReadingChanged;

        public void StartReading(int reportInterval = -1)
        {
            if (reportInterval > 0)
            {
                _motionManager.MagnetometerUpdateInterval = reportInterval;
            }
            _motionManager.StartMagnetometerUpdates(NSOperationQueue.CurrentQueue, OnMagnetoMeterChanged);
        }

        public void StopReading()
        {
            _motionManager.StopMagnetometerUpdates();
        }


        /// <summary>
        /// Raises the magnetometer changed event.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="error">Error.</param>
        void OnMagnetoMeterChanged(CMMagnetometerData data, NSError error)
        {
            OnReadingChanged?.Invoke(this, new DeviceSensorReadingEventArgs<VectorReading>(new VectorReading(data.MagneticField.X, data.MagneticField.Y, data.MagneticField.Z)));
        }
    }
}
