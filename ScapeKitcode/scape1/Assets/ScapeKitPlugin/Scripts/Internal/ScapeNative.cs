//  <copyright file="ScapeNative.cs" company="Scape Technologies Limited">
//
//  ScapeClientIOS.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
  using System;
  using System.Runtime.InteropServices;

	internal class ScapeNative
	{

    #if UNITY_IPHONE && !UNITY_EDITOR
        internal const string LibscapeBoxLibName = "__Internal";
    #elif UNITY_EDITOR_LINUX
        internal const string LibscapeBoxLibName = "scapebox_c";
    #else
        /// <summary>
        /// name of libscapebox native lib
        /// </summary>
        internal const string LibscapeBoxLibName = "libscapebox_c";
    #endif

    [DllImport(LibscapeBoxLibName)]
    internal static extern long _cellIdForWgs(double latitude, double longitude, int s2CellLevel);
    [DllImport(LibscapeBoxLibName)]
    internal static extern double _metersBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2);
    [DllImport(LibscapeBoxLibName)]
    internal static extern double _angleBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2);
    [DllImport(LibscapeBoxLibName)]
    internal static extern void _wgsToLocal(double latitude, double longitude, double altitude, long cellId, double[] result);
    [DllImport(LibscapeBoxLibName)]
    internal static extern void _localToWgs(double x, double y, double z, long cellId, double[] result);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void onScapeSessionErrorDelegate(int errorStatus, [MarshalAs(UnmanagedType.LPStr)] string errorMessage);
    
    [StructLayout(LayoutKind.Sequential)]
		internal struct scape_measurements {
			public double timestamp; 
			public double latitude; 
			public double longitude; 
			public double heading; 
			public double orientationX; 
			public double orientationY; 
			public double orientationZ; 
			public double orientationW; 
			public double rawHeightEstimate; 
			public double confidenceScore; 
			public int measurementsStatus;
		}

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void onScapeMeasurementsUpdatedDelegate(scape_measurements sm);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void onScapeMeasurementsRequestedDelegate(int value);

    [DllImport(LibscapeBoxLibName)]
    internal static extern void citf_setSessionCallbacks( IntPtr client,
                    [MarshalAs(UnmanagedType.FunctionPtr)] onScapeMeasurementsRequestedDelegate req,
                    [MarshalAs(UnmanagedType.FunctionPtr)] onScapeSessionErrorDelegate err,
                    [MarshalAs(UnmanagedType.FunctionPtr)] onScapeMeasurementsUpdatedDelegate mes
                    );

    [DllImport(LibscapeBoxLibName)]
    internal static extern void citf_getMeasurements(IntPtr client);
    [DllImport(LibscapeBoxLibName)]
    internal static extern void citf_setYChannelPtr(IntPtr client, IntPtr pointer, int width, int height);
    [DllImport(LibscapeBoxLibName)]
    internal static extern void citf_setCameraIntrinsics(IntPtr client, double xFocalLength, double yFocalLength, double xPrincipalPoint, double yPrincipalPoint);

    [DllImport(LibscapeBoxLibName)]
    internal static extern void citf_setLogConfig(IntPtr debug, int log_level, int log_output);
    [DllImport(LibscapeBoxLibName)]
    internal static extern void citf_mockGPSCoordinates(IntPtr debug, double latitude, double longitude);
    [DllImport(LibscapeBoxLibName)]
    internal static extern void citf_saveImages(IntPtr debug, bool save);


    [DllImport(LibscapeBoxLibName)]
    internal static extern IntPtr citf_createClient(string api_key, int with_debug);

    [DllImport(LibscapeBoxLibName)]
    internal static extern IntPtr citf_getDebugSession(IntPtr client);

	internal struct motion_measurements {

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public double[] acceleration;
		public double accelerationTimeStamp;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public double[] userAcceleration;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public double[] gyro;
		public double gyroTimestamp;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public double[] magnetometer;
		public double magnetometerTimestamp;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public double[] gravity;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public double[] attitude;
	}

  	internal delegate void onAquireMotionMeasurementsDelegate(ref motion_measurements mm);
    
	internal struct location_measurements {
		public double timestamp;
		public double latitude;
		public double longitude;
		public double coordinatesAccuracy;
		public double altitude;
		public double altitudeAccuracy;
		public double heading;
		public double headingAccuracy;
		public long course;
		public long speed;
	}

    internal delegate void onAquireLocationMeasurementsDelegate(ref location_measurements lm);

    internal struct device_info {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] platform;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] model;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] os;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] os_version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] api_version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] write_directory;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] sdk_version;
    }

    [DllImport(LibscapeBoxLibName)]
    internal static extern void citf_setDeviceInfo(IntPtr client, ref device_info di);

    [DllImport(LibscapeBoxLibName)]
    internal static extern void citf_setClientStateCallbacks( IntPtr client, 
                        [MarshalAs(UnmanagedType.FunctionPtr)] onAquireMotionMeasurementsDelegate motion, 
                        [MarshalAs(UnmanagedType.FunctionPtr)] onAquireLocationMeasurementsDelegate loc);

    [DllImport(LibscapeBoxLibName)]
    internal static extern void citf_destroyClient(IntPtr client);

    [DllImport(LibscapeBoxLibName)]
    internal static extern void citf_log(int logLevel, string tag, string msg);
	}
}