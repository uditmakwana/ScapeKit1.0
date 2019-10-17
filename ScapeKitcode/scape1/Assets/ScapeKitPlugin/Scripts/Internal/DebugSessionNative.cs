//  <copyright file="DebugSessionNative.cs" company="Scape Technologies Limited">
//
//  DebugSessionNative.cs
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

    public sealed class DebugSessionNative : DebugSession 
    {
        private IntPtr nativePtr = IntPtr.Zero;

        public DebugSessionNative(IntPtr np) 
        {
            nativePtr = np;
        }

        public override void SetLogConfig(LogLevel level, LogOutput output) 
        {
            ScapeNative.citf_setLogConfig(nativePtr, (int)level, (int)output);
        }
        public override void MockGPSCoordinates(double latitude, double longitude) 
        {
            ScapeNative.citf_mockGPSCoordinates(nativePtr, latitude, longitude);
        }
        public override void SaveImages(bool save)
        {
            ScapeNative.citf_saveImages(nativePtr, save);
        }

    }
}