//  <copyright file="ScapeDebugConfig.cs" company="Scape Technologies Limited">
//
//  ScapeDebugConfig.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Holds settings that are used to run the scape session with debugging support.
    /// </summary>
    [CreateAssetMenu(
        fileName = "ScapeDebugConfig", menuName = "ScapeKit/ScapeDebugConfig")]
    public class ScapeDebugConfig : ScriptableObject
    {
        /// <summary>
        /// LogLevel, controls the amount of logging that gets output
        /// </summary>
        [SerializeField]
        private LogLevel logLevel = LogLevel.LOG_ERROR;
        
        /// <summary>
        /// logOutput, controls where the logging gets output
        /// </summary>
        [SerializeField]
        private LogOutput logOutput = LogOutput.CONSOLE;

        /// <summary>
        /// if used will override the coordinates returned from location sessions
        /// </summary>
        [SerializeField]
        private bool debugMockGPS = false;

        /// <summary>
        /// The Mock Longitude in degrees
        /// </summary>
        [SerializeField]
        private LatLng mockGPSLatLng;

        /// <summary> 
        /// If used scape client will not be used and these results will be immediately returned instead
        /// </summary>
        [SerializeField]
        private bool mockScapeResults = false;

        /// <summary>
        /// The Mock Scape Longitude in degrees
        /// </summary>
        [SerializeField]
        private LatLng mockScapeLatLng;

        /// <summary>
        /// The mocked compass heading 
        /// </summary>
        [SerializeField]
        private double mockScapeHeading = 0.0;

        /// <summary>
        /// When using mock scape measurements it is more realistic to have the measurements
        /// return after some delay
        /// </summary>
        [SerializeField]
        private float mockScapeMeasurementsDelay = 3.0f;

        /// <summary> 
        /// If used images sent to scape's back end are also saved to local device storage
        /// </summary>
        [SerializeField]
        private bool saveImages = false;
        
        /// <summary>
        /// Gets or sets mockScapeMeasurementsDelay
        /// </summary>
        public float MockScapeMeasurementsDelay
        {
            get { return mockScapeMeasurementsDelay; }

            protected set { mockScapeMeasurementsDelay = value; }
        }

        /// <summary>
        /// Functions returns whether the scape results are to be mocked 
        /// </summary>
        /// <returns>
        /// value of mockScapeResults
        /// </returns>
        public bool MockScapeResults() 
        {
            return mockScapeResults;
        }

        /// <summary>
        /// Apply the debug config options to the DebugSession object 
        /// </summary>
        /// <param name="debugSession">
        /// pass in a debugSession to be configured by this config object
        /// </param>
        public void ConfigureDebugSession(DebugSession debugSession) 
        {
            debugSession.SetLogConfig(logLevel, logOutput);

            if (debugMockGPS) 
            {
                debugSession.MockGPSCoordinates(mockGPSLatLng.Latitude, mockGPSLatLng.Longitude);
            }
            
            if (saveImages)
            {
                debugSession.SaveImages(saveImages);
            }
        }

        /// <summary>
        /// Function returns a mocked ScapeResults object.
        /// </summary>
        /// <returns>
        /// A ScapeResults object same type as that returned from ScapeKit client
        /// </returns>
        public ScapeMeasurements GetMockScapeMeasurements() 
        {
            return new ScapeMeasurements 
            {
                Timestamp = ((DateTimeOffset)System.DateTime.Now).ToUnixTimeSeconds(),
                LatLng = mockScapeLatLng,
                Heading = mockScapeHeading,
                Orientation = new ScapeOrientation(),
                RawHeightEstimate = 0.0,
                ConfidenceScore = 5,
                MeasurementsStatus = ScapeMeasurementStatus.ResultsFound
            };
        }
    }
}