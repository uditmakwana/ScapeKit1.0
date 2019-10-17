//  <copyright file="ScapeSessionDetails.cs" company="Scape Technologies Limited">
//
//  ScapeSessionDetails.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An enum to control differing levels of logoutput
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// the LOG_OFF
        /// </summary>
        LOG_OFF,

        /// <summary>
        /// the LOG_VERBOSE
        /// </summary>
        LOG_VERBOSE,

        /// <summary>
        /// the LOG_DEBUG
        /// </summary>
        LOG_DEBUG,

        /// <summary>
        /// the LOG_INFO
        /// </summary>
        LOG_INFO,

        /// <summary>
        /// the LOG_WARN
        /// </summary>
        LOG_WARN,

        /// <summary>
        /// the LOG_ERROR
        /// </summary>
        LOG_ERROR
    }

    /// <summary>
    /// An enum to control destination of logoutput
    /// </summary>
    public enum LogOutput
    {
        /// <summary>
        /// the NO_OUPUT
        /// </summary>
        NO_OUPUT = 0,

        /// <summary>
        /// the CONSOLE
        /// </summary>
        CONSOLE = 1 << 0,

        /// <summary>
        /// the FILE
        /// </summary>
        FILE = 1 << 1,

        /// <summary>
        /// the OVERLAY
        /// </summary>
        OVERLAY = 1 << 2,

        /// <summary>
        /// the NETWORK (NOT IMPLMENTED!)
        /// </summary>
        NETWORK = 1 << 3,

        /// <summary>
        /// the CONSOLE_FILE
        /// </summary>
        CONSOLE_FILE = CONSOLE | FILE,

        /// <summary>
        /// the ALL_OUPUT
        /// </summary>
        ALL_OUPUT = 0 | CONSOLE | FILE | OVERLAY | NETWORK,
    }

    /// <summary>
    /// An enum to state Scape Measurement Status of returned results
    /// </summary>
    public enum ScapeMeasurementStatus
    {
        /// <summary>
        /// the NoResults, the VPS failed to locate the image sent to it.
        /// </summary>
        NoResults,

        /// <summary>
        /// the UnavailableArea. The area is not available to the VPS.
        /// </summary>
        UnavailableArea,

        /// <summary>
        /// the ResultsFound. A successful response.
        /// </summary>
        ResultsFound,

        /// <summary>
        /// the InternalError. An unspecified error has occurred using VPS.
        /// This does not indicate a permanent problem.
        /// </summary>
        InternalError
    }

    /// <summary>
    /// An enum to state Scape Session Status
    /// </summary>
    public enum ScapeSessionState
    {
        /// <summary>
        /// the NoError
        /// </summary>
        NoError,

        /// <summary>
        /// the LocationSensorsError
        /// </summary>
        LocationSensorsError,

        /// <summary>
        /// the MotionSensorsError
        /// </summary>
        MotionSensorsError,

        /// <summary>
        /// the ImageSensorsError
        /// </summary>
        ImageSensorsError,

        /// <summary>
        /// the LockingPositionError
        /// </summary>
        LockingPositionError,

        /// <summary>
        /// the AuthenticationError
        /// </summary>
        AuthenticationError,

        /// <summary>
        /// the NetworkError
        /// </summary>
        NetworkError,

        /// <summary>
        /// the UnexpectedError
        /// </summary>
        UnexpectedError
    }

    /// <summary>
    /// A class representing a World Coordinate.
    /// Scape uses the [World Geodetic System: WGS 84](https://en.wikipedia.org/wiki/World_Geodetic_System), same as GPS.
    /// </summary>
    [Serializable]
    public struct LatLng
    {
        /// <summary>
        /// the latitude in degrees
        /// </summary>
        public double Latitude;
     
        /// <summary>
        /// the longitude in degrees
        /// </summary>
        public double Longitude;
    }

    /// <summary>
    /// A class representing a quaternion orientation
    /// </summary>
    [Serializable]
    public struct ScapeOrientation
    {
        /// <summary>
        /// the y
        /// </summary>
        public double Y;

        /// <summary>
        /// the w
        /// </summary>
        public double W;

        /// <summary>
        /// the z
        /// </summary>
        public double Z;

        /// <summary>
        /// the x
        /// </summary>
        public double X;
    }

    /// <summary>
    /// An enum to state Scape Session Error
    /// </summary>
    [Serializable]
    public struct ScapeSessionError
    {
        /// <summary>
        /// the state
        /// </summary>
        public ScapeSessionState State;

        /// <summary>
        /// the message
        /// </summary>
        public string Message;
    }

    /// <summary>
    /// the data structure returned from Scape's Visual Positioning Service in response to an image query. 
    /// </summary>
    [Serializable]
    public struct ScapeMeasurements
    {
        /// <summary>
        /// the timestamp referring to the point the image was taken
        /// </summary>
        public double Timestamp;

        /// <summary>
        /// the coordinates established for the camera which took the image 
        /// </summary>
        public LatLng LatLng;

        /// <summary>
        /// the compass heading of the source camera
        /// </summary>
        public double Heading;

        /// <summary>
        /// the orientation of the source camera
        /// </summary>
        public ScapeOrientation Orientation;

        /// <summary>
        /// the rawHeightEstimate for the camera. Scape measures height from the ground, not sea level,
        /// </summary>
        public double RawHeightEstimate;

        /// <summary>
        /// the confidenceScore. A value between 0-5 indicating the confidence for which the VPS has located the image. 
        /// </summary>
        public double ConfidenceScore;

        /// <summary>
        /// the measurementsStatus. An enum state referring to the result of the query.
        /// </summary>
        public ScapeMeasurementStatus MeasurementsStatus;
    }

    /// <summary>
    /// the data structure returned from device's motion measurements via ScapeKit
    /// </summary>
    [Serializable]
    public struct MotionMeasurements
    {
        /// <summary>
        /// the acceleration
        /// </summary>
        public List<double> Acceleration;

        /// <summary>
        /// the accelerationTimeStamp
        /// </summary>
        public double AccelerationTimeStamp;

        /// <summary>
        /// the double
        /// </summary>
        public List<double> UserAcceleration;

        /// <summary>
        /// the double
        /// </summary>
        public List<double> Gyro;

        /// <summary>
        /// the gyroTimestamp
        /// </summary>
        public double GyroTimestamp;

        /// <summary>
        /// the double
        /// </summary>
        public List<double> Magnetometer;

        /// <summary>
        /// the magnetometerTimestamp
        /// </summary>
        public double MagnetometerTimestamp;

        /// <summary>
        /// the double
        /// </summary>
        public List<double> Gravity;

        /// <summary>
        /// the double
        /// </summary>
        public List<double> Attitude;
    }

    /// <summary>
    /// a data structure representing the status of the device's motion measurements
    /// </summary>
    [Serializable]
    public struct MotionSessionDetails
    {
        /// <summary>
        /// the measurements
        /// </summary>
        public MotionMeasurements Measurements;

        /// <summary>
        /// the errorMessage
        /// </summary>
        public string ErrorMessage;
    }

    /// <summary>
    /// the data structure returned from device's location measurements (GPS), via ScapeKit
    /// </summary>
    [Serializable]
    public struct LocationMeasurements
    {
        /// <summary>
        /// the timestamp
        /// </summary>
        public double Timestamp;

        /// <summary>
        /// the latlng
        /// </summary>
        public LatLng LatLng;

        /// <summary>
        /// the coordinatesAccuracy
        /// </summary>
        public double CoordinatesAccuracy;

        /// <summary>
        /// the altitude
        /// </summary>
        public double Altitude;

        /// <summary>
        /// the altitudeAccuracy
        /// </summary>
        public double AltitudeAccuracy;

        /// <summary>
        /// the heading
        /// </summary>
        public double Heading;

        /// <summary>
        /// the headingAccuracy
        /// </summary>
        public double HeadingAccuracy;

        /// <summary>
        /// the course
        /// </summary>
        public long Course;

        /// <summary>
        /// the speed
        /// </summary>
        public long Speed;
    }

    /// <summary>
    /// a data structure representing the status of the device's location measurements
    /// </summary>
    [Serializable]
    public struct LocationSessionDetails
    {
        /// <summary>
        /// the measurements
        /// </summary>
        public LocationMeasurements Measurements;

        /// <summary>
        /// the errorMessage
        /// </summary>
        public string ErrorMessage;
    }
}