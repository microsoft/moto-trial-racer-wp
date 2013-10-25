/**
 * Copyright (c) 2011 Nokia Corporation and/or its subsidiary(-ies).
 * All rights reserved.
 *
 * For the applicable distribution terms see the license text file included in
 * the distribution.
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Devices;
using Microsoft.Devices.Sensors;

namespace MotoTrialRacer
{
    /// <summary>
    /// This class handles the acceleration sensor data of the device
    /// </summary>
    public class RotationData
    {
#if WINDOWS_PHONE
        private Accelerometer accelerometer = new Accelerometer();
#endif
        private static object threadLock = new object();
        public bool device = (Microsoft.Devices.Environment.DeviceType == DeviceType.Device);
        public float xRot, yRot;

        /// <summary>
        /// Enables handling rotation data. Creates a new acceleration sensor data handler.
        /// If called using an emulator, won't do anything.
        /// </summary>
        public void EnableRotation()
        {
#if WINDOWS_PHONE
            if (device)
            {
                try
                {
                    accelerometer.ReadingChanged +=
                        new EventHandler<AccelerometerReadingEventArgs>(sensor_ReadingChanged);
                    accelerometer.Start();
                }
                catch (Microsoft.Devices.Sensors.AccelerometerFailedException)
                {
                }
            }
#endif
        }

#if WINDOWS_PHONE
        /// <summary>
        /// A callback function for Accelerometer to notify that the sensor reading has changed.
        /// This method reads the acceleration along the x axis, then scales and clamp it between
        /// 0 and -30.
        /// </summary>
        /// <param name="sender">The caller of this function</param>
        /// <param name="e">the parameter wich holds the data of the sensor</param>
        private void sensor_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            lock (threadLock)
            {
                if (e.X < -0.6)
                {
                    xRot = -30.0f;
                    return;
                }
                if (e.X > 0)
                {
                    xRot = 0;
                    return;
                }
                xRot = (float)e.X / 0.6f * 30.0f;
            }
        }
#endif
    }
}
