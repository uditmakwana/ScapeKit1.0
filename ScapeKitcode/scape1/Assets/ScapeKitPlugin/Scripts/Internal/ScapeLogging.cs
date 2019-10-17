//  <copyright file="ScapeLogging.cs" company="Scape Technologies Limited">
//
//  ScapeLogging.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    
    public static class ScapeLogging
    {
        public static void LogDebug(string message = "", string tag = "SCKUnity")
        {
            if(ScapeClient.Instance.IsStarted()) 
            {
                ScapeNative.citf_log((int)LogLevel.LOG_DEBUG, tag, message);
            }
            else {
                Debug.Log(tag + " [Debug] : " + message);
            }
        }
        public static void LogError(string message = "", string tag = "SCKUnity")
        {
            if(ScapeClient.Instance.IsStarted()) 
            {
                ScapeNative.citf_log((int)LogLevel.LOG_ERROR, tag, message);
            }
            else {
                Debug.Log(tag + " [Error] : " + message);
            }
        }
    }
}