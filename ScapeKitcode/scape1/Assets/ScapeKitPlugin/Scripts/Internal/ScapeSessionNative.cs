
namespace ScapeKitUnity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using Unity.Collections;
    using UnityEngine.XR.ARFoundation;
    using UnityEngine.XR.ARSubsystems;
    using AOT;

    internal sealed class ScapeSessionNative 
    {
        private ARCameraManager cameraManager;
        private static ScapeSession frontEnd;

        private bool measurementsRequested = false;
        private bool gotIntrinsics = false;

        private XRCameraImage currentXRImage;
        private float requestTime;

        private const int ScapeImgWidth = 640;
        private const int ScapeImgHeight = 480;

        private byte[] imageBufferCopy = null;
        private IntPtr imagePtr = IntPtr.Zero;
        private GCHandle handle;

        public void GetMeasurements() 
        {
            measurementsRequested = true;
            //next image ready will trigger the GetMeasurements

            requestTime = Time.time;
            ScapeLogging.LogDebug("ScapeSessionNative::GetMeasurements() requested " + Time.time);
        }
        
        public void GetMeasurements(ScapeSession.ARImage image)
        { 
            ScapeNative.citf_setYChannelPtr(this.scapeClient, image.YPixelBuffer, image.Width, image.Height);
            ScapeNative.citf_setCameraIntrinsics(this.scapeClient, 
                                    image.XFocalLength, 
                                    image.YFocalLength, 
                                    image.XPrincipalPoint, 
                                    image.YPrincipalPoint);

            ScapeNative.citf_getMeasurements(this.scapeClient);
        }

        public void Terminate()
        {
            FreeHandle();
        }

        private void TryGetIntrinsics(float scaleX, float scaleY)
        {
            XRCameraIntrinsics intrinsics = new XRCameraIntrinsics();

            if(cameraManager.TryGetIntrinsics(out intrinsics))
            {
                ScapeNative.citf_setCameraIntrinsics(this.scapeClient, 
                    intrinsics.focalLength.x * scaleX,
                    intrinsics.focalLength.y * scaleY,
                    intrinsics.principalPoint.x * scaleX,
                    intrinsics.principalPoint.y * scaleY);
                
                ScapeLogging.LogDebug("setCameraIntrinsics " + "\n" +
                    "focalLength.x = " + (intrinsics.focalLength.x * scaleX) + "\n" + 
                    "focalLength.y = " + (intrinsics.focalLength.y * scaleY) + "\n" + 
                    "principalPoint.x = " + (intrinsics.principalPoint.x * scaleX) + "\n" + 
                    "principalPoint.y = " + (intrinsics.principalPoint.y * scaleY) + "\n");

                gotIntrinsics = true;
            }
        }

        public void SetCameraManager(ARCameraManager arCameraManager)
        {
            cameraManager = arCameraManager;
            cameraManager.frameReceived += OnCameraFrameReceived;
        }

        public void SetFrontEnd(ScapeSession in_frontEnd)
        {
            frontEnd = in_frontEnd;
        }

        private void FreeHandle()
        {
            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }

        private void CopyImageBuffer(NativeArray<byte> imageBuffer)
        {   
            if(imageBufferCopy == null || imageBufferCopy.Length != imageBuffer.Length)
            {
                FreeHandle();

                imageBufferCopy = new byte[imageBuffer.Length];
                handle = GCHandle.Alloc(imageBufferCopy, GCHandleType.Pinned);
                imagePtr = handle.AddrOfPinnedObject();

                ScapeLogging.LogDebug("CopyImageBuffer() " + (Time.time - requestTime));
            }

            imageBuffer.CopyTo(imageBufferCopy);
        }

        private void ProcessImage(AsyncCameraImageConversionStatus status, 
                        XRCameraImageConversionParams conversionParams, 
                        NativeArray<byte> imageBuffer) 
        {
            if (status != AsyncCameraImageConversionStatus.Ready)
            {
                // attempt to call getMeasurements with empty image will compel SDK core to report error
                // triggering session to request another
                ScapeNative.citf_setYChannelPtr(this.scapeClient, IntPtr.Zero, 0, 0);
                ScapeNative.citf_getMeasurements(this.scapeClient);
                return;
            }

            CopyImageBuffer(imageBuffer);

            ScapeNative.citf_setYChannelPtr(this.scapeClient, imagePtr, ScapeImgWidth, ScapeImgHeight);
            ScapeNative.citf_getMeasurements(this.scapeClient);

            ScapeLogging.LogDebug("citf_getMeasurements() " + (Time.time - requestTime));
        }

        private void GetImageAsync()
        {
            currentXRImage = new XRCameraImage();

            if ( cameraManager.TryGetLatestImage(out currentXRImage) ) 
            {
                currentXRImage.ConvertAsync(new XRCameraImageConversionParams
                {
                    inputRect = new RectInt(0, 0, currentXRImage.width, currentXRImage.height),
                    outputDimensions = new Vector2Int(ScapeImgWidth, ScapeImgHeight),
                    outputFormat = TextureFormat.R8

                }, ProcessImage);

                currentXRImage.Dispose();

                ScapeLogging.LogDebug("GetImageAsync() " + (Time.time - requestTime));
                measurementsRequested = false;
            }
        }

    	private IntPtr scapeClient;

    	internal ScapeSessionNative(IntPtr client) 
    	{
    		this.scapeClient = client;

    		ScapeNative.citf_setSessionCallbacks(this.scapeClient, 
    			onScapeMeasurementsRequestedNative,
    			onScapeSessionErrorNative,
    			onScapeMeasurementsUpdatedNative
    		);
    	}

        private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
        {
            if (measurementsRequested) 
            {
                GetImageAsync();
            }

        }

        [MonoPInvokeCallback (typeof(ScapeNative.onScapeMeasurementsRequestedDelegate))]
        static void onScapeMeasurementsRequestedNative(int timestamp)
        {
        	frontEnd.OnScapeMeasurementsRequested(timestamp);
        }

        [MonoPInvokeCallback (typeof(ScapeNative.onScapeMeasurementsUpdatedDelegate))]
        static void onScapeMeasurementsUpdatedNative(ScapeNative.scape_measurements sm)
        {
    		ScapeMeasurements scapeMeasurements;
			
			scapeMeasurements.Timestamp = sm.timestamp;
			scapeMeasurements.LatLng = new LatLng() {
				Latitude = sm.latitude, 
				Longitude = sm.longitude 
			};

			scapeMeasurements.Heading = sm.heading;
			scapeMeasurements.Orientation = new ScapeOrientation() {
				X = sm.orientationX, 
				Y = sm.orientationY, 
				Z = sm.orientationZ, 
				W = sm.orientationW
			};
			scapeMeasurements.RawHeightEstimate = sm.rawHeightEstimate;
			scapeMeasurements.ConfidenceScore = sm.confidenceScore;
			scapeMeasurements.MeasurementsStatus = (ScapeMeasurementStatus)sm.measurementsStatus;

            frontEnd.OnScapeMeasurementsEvent(scapeMeasurements);
        }

        [MonoPInvokeCallback (typeof(ScapeNative.onScapeSessionErrorDelegate))]
        static void onScapeSessionErrorNative(int errorStatus, [MarshalAs(UnmanagedType.LPStr)] string errorMessage)
        {
    		frontEnd.OnScapeSessionErrorEvent(new ScapeSessionError() {
    			State = (ScapeSessionState)errorStatus, 
    			Message = errorMessage
    		});
        }
    }
}