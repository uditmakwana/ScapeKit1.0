

namespace ScapeKitUnity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using AOT;

    internal sealed class ScapeClientNative
    {

    	IntPtr scapeClientPtr = IntPtr.Zero;
    	IntPtr debugSessionPtr = IntPtr.Zero;

        private int isDebugEnabled = 0;

        private static LocationInfo lastInfo;
        private static bool haveLocation = false;
        
        /// <summary>
        /// instance of the ScapeSession
        /// </summary>
        private ScapeSessionNative scapeSessionNative = null;

        /// <summary>
        /// instance of the DebugSession
        /// </summary>
        private DebugSession debugSession = null;  

        public ScapeSessionNative ScapeSessionNative 
        {
        	get 
        	{
        		return this.scapeSessionNative;
        	}
        }

        public DebugSession DebugSession 
        {
            get
            {
                return debugSession;
            }
        }

        public void StartClient(string apiKey, bool isDebugEnabled)
        {
        	if(this.scapeClientPtr == IntPtr.Zero)
        	{
                int isDebugInt = 0;
                if(isDebugEnabled) {
                    isDebugInt = 1;
                }
                
	        	this.scapeClientPtr = ScapeNative.citf_createClient(apiKey, isDebugInt);


	        	if(this.IsStarted()) 
                {
                    this.scapeSessionNative = new ScapeSessionNative(this.scapeClientPtr);

	        		ScapeNative.citf_setClientStateCallbacks(this.scapeClientPtr, 
                        onAquireMotionMeasurements, 
                        onAquireLocationMeasurements);

	        		if(isDebugEnabled) 
                    {
                        this.debugSessionPtr = ScapeNative.citf_getDebugSession(this.scapeClientPtr);

                        debugSession = new DebugSessionNative(debugSessionPtr);
	        		}

                    setDeviceInfo();

                    Input.location.Start();
	        	}
        	} 
        }

        public bool IsStarted() 
        {
        	return this.scapeClientPtr != IntPtr.Zero;
        }

        public void Terminate() 
        {	
        	if (this.IsStarted())
        	{
        		ScapeNative.citf_destroyClient(this.scapeClientPtr);
        	}
        }

        byte[] stringToFixedByteArray(string str, int max_size)
        {
            int copy_size = str.Length < max_size ? str.Length : max_size;
            byte[] result = new byte[max_size];
            Encoding.UTF8.GetBytes(str, 0, copy_size, result, 0);

            return result;
        }

        private void setDeviceInfo()
        {
            ScapeNative.device_info di = new ScapeNative.device_info();

            di.id = stringToFixedByteArray(SystemInfo.deviceUniqueIdentifier, 256);
            di.platform = stringToFixedByteArray(Enum.GetName(typeof(RuntimePlatform), Application.platform), 256);
            di.model = stringToFixedByteArray(SystemInfo.deviceModel, 256);
            di.os = stringToFixedByteArray(SystemInfo.operatingSystem, 256);
            di.os_version = stringToFixedByteArray(Enum.GetName(typeof(OperatingSystemFamily), SystemInfo.operatingSystemFamily), 256);
            di.write_directory = stringToFixedByteArray(Application.persistentDataPath, 256);
            di.sdk_version = stringToFixedByteArray(ScapeKitVersionVars.version + "." + ScapeKitVersionVars.build, 256);
            ScapeNative.citf_setDeviceInfo(this.scapeClientPtr, ref di);
        }
        
        [MonoPInvokeCallback (typeof(ScapeNative.onAquireMotionMeasurementsDelegate))]
		static void onAquireMotionMeasurements(ref ScapeNative.motion_measurements mm)
		{
		}

        [MonoPInvokeCallback (typeof(ScapeNative.onAquireLocationMeasurementsDelegate))]
  		static void onAquireLocationMeasurements(ref ScapeNative.location_measurements lm)
  		{
            if(haveLocation) 
            {
                lm.longitude = lastInfo.longitude;
                lm.latitude = lastInfo.latitude;
            }
            else 
            {
                ScapeLogging.LogError("ScapeClientNative::onAquireLocationMeasurements called but Unity has not retrieved location measurements from device yet. Have Location Permissions been allowed?");

                lm.longitude = 0.0;
                lm.latitude = 0.0;
            }
  		}

        public void Update()
        {
            if (this.IsStarted())
            {
                if (Input.location.status == LocationServiceStatus.Running) 
                {
                    lastInfo = Input.location.lastData;
                    haveLocation = true;
                }
            }
        }
    }
}