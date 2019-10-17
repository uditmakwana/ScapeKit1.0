//  <copyright file="DebugSession.cs" company="Scape Technologies Limited">
//
//  DebugSession.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    
    /// <summary>
    /// a class to configure various debugging facilities
    /// </summary>
    public abstract class DebugSession 
    {
        /// <summary>
        /// SetLogConfig set the output level and output destination
        /// </summary>
        /// <param name="level">
        /// Denotes different levels of output, any of the following
        /// LOG_OFF
        /// LOG_VERBOSE
        /// LOG_DEBUG
        /// LOG_INFO
        /// LOG_WARN
        /// LOG_ERROR
        /// </param>
        /// <param name="output">
        /// NO_OUPUT
        /// CONSOLE
        /// FILE
        /// OVERLAY
        /// ALL_OUPUT = 0 | CONSOLE | FILE | OVERLAY
        /// </param> 
        public abstract void SetLogConfig(LogLevel level, LogOutput output);

        /// <summary>
        /// MockGPSCoordinates, enter mock gps coordinates to be used in place of devices reported gps
        /// </summary>
        /// <param name="latitude">
        /// latitude value in degrees
        /// </param>
        /// <param name="longitude">
        /// longitude value in degrees
        /// </param>
        public abstract void MockGPSCoordinates(double latitude, double longitude);

        /// <summary>
        /// SaveImages, option toi have all images sesnt to scape backend saved to apps documents folder
        /// </summary>
        /// <param name="save">
        /// turn images saving on or off
        /// </param>
        public abstract void SaveImages(bool save);
    }
}