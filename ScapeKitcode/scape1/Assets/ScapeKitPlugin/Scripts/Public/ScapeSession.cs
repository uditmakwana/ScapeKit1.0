//  <copyright file="ScapeSession.cs" company="Scape Technologies Limited">
//
//  ScapeSession.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.XR.ARFoundation;
    
    /// <summary>
    /// A class to handle the scape session request and response
    /// </summary>
    public class ScapeSession
    {
        /// <summary>
        /// flag to prevent multiple requests to scapemeasurements
        /// A request to scapeMeasuremnts is guaranteed to always return
        /// a signal to OnScapeMeasurementsEvent or OnScapeSessionErrorEvent.
        /// </summary>
        private bool scapeMeasurementInProgress = false;

        /// <summary>
        /// Flag to tell ScapeMeasurementsRequested needs to be clled from main thread
        /// </summary>
        private bool doScapeMeasurementsRequested;

        /// <summary>
        /// the last double returned from core implementation
        /// </summary>
        private double lastScapeMeasurementsRequested;

        /// <summary>
        /// Flag to tell ScapeSessionErrorEvent needs to be clled from main thread
        /// </summary>
        private bool doScapeSessionErrorEvent;

        /// <summary>
        /// the last ScapeSessionError returned from core implementation
        /// </summary>
        private ScapeSessionError lastScapeSessionError;

        /// <summary>
        /// Flag to tell DeviceLocationMeasurementsEvent needs to be clled from main thread
        /// </summary>
        private bool doDeviceLocationMeasurementsEvent;

        /// <summary>
        /// the last LocationMeasurements returned from core implementation
        /// </summary>
        private LocationMeasurements lastLocationMeasurements;

        /// <summary>
        /// Flag to tell DeviceMotionMeasurementsEvent needs to be clled from main thread
        /// </summary>
        private bool doDeviceMotionMeasurementsEvent;

        /// <summary>
        /// the last MotionMeasurements returned from core implementation
        /// </summary>
        private MotionMeasurements lastMotionMeasurements;

        /// <summary>
        /// Flag to tell ScapeMeasurementsEvent needs to be clled from main thread
        /// </summary>
        private bool doScapeMeasurementsEvent;

        /// <summary>
        /// the last ScapeMeasurements returned from core implementation
        /// </summary>
        private ScapeMeasurements lastScapeMeasurements;

        /// <summary>
        /// the native implementation of ScapeSession, supplied by the ScapeClient after it has started
        /// </summary>
        private ScapeSessionNative scapeSessionNative = null;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ScapeSession" /> class
        /// </summary>
        internal ScapeSession()
        {
        }

        /// <summary>
        /// An event that is triggered when a SCape Measurement has been requested
        /// </summary>
        public event Action<double> ScapeMeasurementsRequested;
         
        /// <summary>
        /// An event that is returned when an error occurs getting a ScapeMeasurement
        /// </summary>
        public event Action<ScapeSessionError> ScapeSessionErrorEvent;
        
        /// <summary>
        /// An event that is triggered when the devices location are taken
        /// </summary>
        public event Action<LocationMeasurements> DeviceLocationMeasurementsEvent;
        
        /// <summary>
        /// An event that is triggered when the devices motion are taken
        /// </summary>
        public event Action<MotionMeasurements> DeviceMotionMeasurementsEvent;
        
        /// <summary>
        /// An event that is triggered when a ScapeMeasurement has been returned
        /// </summary>
        public event Action<ScapeMeasurements> ScapeMeasurementsEvent;

        /// <summary>
        /// The ARCameraManager is needed to acquire the frame to send to scapekit's backend 
        /// </summary>
        /// <param name="arCameraManager">
        /// The arCameraManager usually attached to the ARCamera
        /// </param>
        public virtual void SetCameraManager(ARCameraManager arCameraManager)
        {
            this.scapeSessionNative.SetCameraManager(arCameraManager);
        }

        /// <summary>
        /// calls teh appropriate event on the main thread having been reviously received
        /// from the Scapekit core plugin
        /// </summary>
        public void Update()
        {
            if (this.doScapeMeasurementsRequested) 
            {
                this.doScapeMeasurementsRequested = false;
                this.ScapeMeasurementsRequested(this.lastScapeMeasurementsRequested);
            }

            if (this.doScapeSessionErrorEvent) 
            {
                this.doScapeSessionErrorEvent = false;
                this.ScapeSessionErrorEvent(this.lastScapeSessionError);
            }

            if (this.doDeviceLocationMeasurementsEvent) 
            {
                this.doDeviceLocationMeasurementsEvent = false;
                this.DeviceLocationMeasurementsEvent(this.lastLocationMeasurements);
            }

            if (this.doDeviceMotionMeasurementsEvent) 
            {
                this.doDeviceMotionMeasurementsEvent = false;
                this.DeviceMotionMeasurementsEvent(this.lastMotionMeasurements);
            }

            if (this.doScapeMeasurementsEvent) 
            {
                this.doScapeMeasurementsEvent = false;
                this.ScapeMeasurementsEvent(this.lastScapeMeasurements);
            }
        }

        /// <summary>
        /// The public function to request a ScapeMeasurement using the given image details
        /// </summary>
        /// <param name="image">
        /// the image to be sent to the Scape back end
        /// </param>
        public void GetMeasurements(ScapeSession.ARImage image)
        {
            if (this.scapeMeasurementInProgress) 
            {
                ScapeLogging.LogError("GetMeasuremnts ignored, scapeMeasurements already in progress");
                return;
            }

            if (this.scapeSessionNative != null) 
            {
                this.scapeMeasurementInProgress = true;
                this.scapeSessionNative.GetMeasurements(image);
            }
            else 
            {
                ScapeLogging.LogError("GetMeasurements called before scapeSessionNative initialized");
            }
        }

        /// <summary>
        /// The public function to request a ScapeMeasurement.
        /// </summary>
        public void GetMeasurements()
        {
            if (this.scapeMeasurementInProgress) 
            {
                ScapeLogging.LogError("GetMeasuremnts ignored, scapeMeasurements already in progress");
                return;
            }

            if (this.scapeSessionNative != null) 
            {
                this.scapeMeasurementInProgress = true;
                this.scapeSessionNative.GetMeasurements();
            }
            else 
            {
                ScapeLogging.LogError("GetMeasurements called before scapeSessionNative initialized");
            }
        }

        /// <summary>
        /// set the native impl 
        /// </summary>
        /// <param name="in_native">
        /// The native implementation is passed in from the ScapeClient
        /// </param>
        internal void SetNative(ScapeSessionNative in_native)
        {
            this.scapeSessionNative = in_native;
            this.scapeSessionNative.SetFrontEnd(this);
        }

        /// <summary>
        /// Destroy the ScapeSession object
        /// </summary>
        internal void Terminate()
        {
        }

        /// <summary>
        /// An internal function to trigger the ScapeMeasurementsRequested from the underlying implementation
        /// </summary>
        /// <param name="arg">
        /// The timestamp at which the measurement was taken 
        /// </param>
        internal virtual void OnScapeMeasurementsRequested(double arg)
        {
            if (this.ScapeMeasurementsRequested != null) 
            {
                this.doScapeMeasurementsRequested = true;
                this.lastScapeMeasurementsRequested = arg;
            }
        }

        /// <summary>
        /// An internal function to trigger the ScapeSessionErrorEvent from the underlying implementation
        /// </summary>
        /// <param name="arg">
        /// The ScapeSessionError returned from the ScapeSession 
        /// </param>
        internal virtual void OnScapeSessionErrorEvent(ScapeSessionError arg)
        {
            this.scapeMeasurementInProgress = false;

            if (this.ScapeSessionErrorEvent != null) 
            {
                this.doScapeSessionErrorEvent = true;
                this.lastScapeSessionError = arg;
            }
        }

        /// <summary>
        /// An internal function to trigger the DeviceLocationMeasurementsEvent from the underlying implementation
        /// </summary>
        /// <param name="arg">
        /// The LocationMeasurements returned from the ScapeSession 
        /// </param>
        internal virtual void OnDeviceLocationMeasurementsEvent(LocationMeasurements arg)
        {
            if (this.DeviceLocationMeasurementsEvent != null) 
            {
                this.doDeviceLocationMeasurementsEvent = true;
                this.lastLocationMeasurements = arg;
            }
        }

        /// <summary>
        /// An internal function to trigger the DeviceMotionMeasurementsEvent from the underlying implementation
        /// </summary>
        /// <param name="arg">
        /// The MotionMeasurements returned from the ScapeSession 
        /// </param>
        internal virtual void OnDeviceMotionMeasurementsEvent(MotionMeasurements arg)
        {
            if (this.DeviceMotionMeasurementsEvent != null) 
            {
                this.doDeviceMotionMeasurementsEvent = true;
                this.lastMotionMeasurements = arg;
            }
        }
        
        /// <summary>
        /// An internal function to trigger the ScapeMeasurementsEvent from the underlying implementation
        /// </summary>
        /// <param name="sm">
        /// The ScapeMeasurements returned from the ScapeSession 
        /// </param>
        internal virtual void OnScapeMeasurementsEvent(ScapeMeasurements sm) 
        {
            this.scapeMeasurementInProgress = false;
            
            if (this.ScapeMeasurementsEvent != null) 
            {
                this.doScapeMeasurementsEvent = true;
                this.lastScapeMeasurements = sm;
            }
        }

        /// <summary>
        /// A struct to hold all information pertaining to an image to be sent to the backend
        /// </summary>
        public struct ARImage 
        {
            /// <summary>
            /// the image Width
            /// </summary>
            public int Width;

            /// <summary>
            /// the image Height
            /// </summary>
            public int Height;

            /// <summary>
            /// the image YPixelBuffer, a pointer to a single byte array representation of the grey scale image.
            /// The buffer should be exactly Width*Height in size bytes.
            /// </summary>
            public IntPtr YPixelBuffer;

            /// <summary>
            /// the image XFocalLength
            /// </summary>
            public float XFocalLength;

            /// <summary>
            /// the image YFocalLength
            /// </summary>
            public float YFocalLength;

            /// <summary>
            /// the image XPrincipalPoint
            /// </summary>
            public float XPrincipalPoint;

            /// <summary>
            /// the image YPrincipalPoint
            /// </summary>
            public float YPrincipalPoint;

            /// <summary>
            /// the image IsAvailable
            /// </summary>
            public bool IsAvailable;
        }
    }
}
