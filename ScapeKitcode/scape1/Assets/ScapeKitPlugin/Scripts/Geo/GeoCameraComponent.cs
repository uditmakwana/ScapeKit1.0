//  <copyright file="GeoCameraComponent.cs" company="Scape Technologies Limited">
//
//  GeoCameraComponent.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// GeoCameraComponent. Can be added to any camera component to apply the Scape functionality too. 
    /// When a ScapeMeasurement is returned to the ScapeSessionManager that updates the GeoCmeraComponent.
    /// The GeoCameraComponent inserts a new GameObject above the camera it is attached to.
    /// The GameObject's transform takes the camera into a globally correct world space.
    /// In that world space the z axis is always true north and the x axis east. The world position of the camera
    /// is relative to the center of the S2Cell (and the scene's origin) found in the GeoAnchorManager.
    /// </summary>
    public class GeoCameraComponent : MonoBehaviour
    {
        /// <summary>
        /// The worldTransformObject is used to convert the local camera's space (typically set by ARKit/Core)
        /// to the world space. That is a space relative to the S2 cell being used as the point of origin for the scene
        /// and the direction being relative to north.
        /// The world transform object is made the camera's parent transform.
        /// </summary>
        private static GameObject worldTransformObject;

        /// <summary>
        /// Use the AR system to find a ground plane and use that to determine camera's height in 
        /// the real world. The height measurement returned from scape is not reliable
        /// </summary> 
        [SerializeField]
        private GroundTrackerARFoundation groundTracker;

        /// <summary>
        /// The camera position at which point scape measurements last were taken
        /// </summary>
        private Vector3 positionAtScapeMeasurements;

        /// <summary>
        /// The camera rotation at which point scape measurements last were taken
        /// </summary>
        private Vector3 rotationAtScapeMeasurements;

        /// <summary>
        /// The camera's position relative to the root s2cell (it's intended world position, as established by VPS)
        /// calculated at the point a scape measurement is returned, mainly for debugging purposes
        /// </summary>
        private Vector3 cameraS2Position;

        /// <summary>
        /// worldTransformDirection. The rotation component of the worldTransform object.
        /// </summary>
        private float worldTransformDirection = 0.0f;

        /// <summary>
        /// worldTransformPosition, The position component of the worldTransform object
        /// </summary>
        private Vector3 worldTransformPosition = new Vector3(0.0f, 0.0f, 0.0f);

        /// <summary>
        /// used to update worldTransform in Unity main thread
        /// </summary>
        private bool updateWorldTransform = false;

        /// <summary>
        /// time at last measurements update
        /// </summary>
        private float updateStartTime = 0.0f;

        /// <summary>
        /// previous direction value. Zero denotes unset
        /// </summary>
        private float previousYDirection = 0.0f;

        /// <summary>
        /// previous position value. Zero denotes unset
        /// </summary>
        private Vector3 previousPosition = new Vector3(0.0f, 0.0f, 0.0f);

        /// <summary>
        /// the currently set direction of the worldTransform
        /// </summary>
        private float lerpDirection = 0.0f;

        /// <summary>
        /// the currently set position of the worldTransform
        /// </summary>
        private Vector3 lerpPosition = new Vector3(0.0f, 0.0f, 0.0f);

        /// <summary>
        /// Update world transform object over some time 
        /// to make smoother transition between world position updates.
        /// </summary>
        [SerializeField]
        private float updateTransformTime = 0.0f;

        /// <summary>
        /// The S2Cell id for the GeoCamera, this should be the same as the one calculated for GeoAnchorManager.
        /// </summary>
        private long s2CellId = 0;

        /// <summary>
        /// Gets the world transform. This is the transform that takes the camera
        /// from ar space to "real world" space.
        /// The inverse of this transform can be used to take an object position in world space 
        /// and covert it back to ar space for use with ar specific functions eg Ar.Session.RayCast
        /// Additionally an object in world space prior to a scape measurement, can be converted to the "new"
        /// world space after a scape measurement is returned by multiplying it's transform with this one
        /// (or by parenting itself to this one)
        /// </summary>
        public static Transform WorldTransform
        {
            get 
            { 
                return worldTransformObject.transform; 
            }

            private set 
            {
            }
        }

        /// <summary>
        /// used to save the camera's position and orientation at the point the Scape Measurements are taken
        /// </summary>
        public void HoldCameraPose()
        {
            positionAtScapeMeasurements = transform.localPosition;

            rotationAtScapeMeasurements = transform.localRotation.eulerAngles;
        }

        /// <summary>
        /// create camera parent at start
        /// </summary>
        public void Start() 
        {
            SetupCameraParent();

            ScapeClient.Instance.ScapeSession.ScapeMeasurementsEvent += OnScapeMeasurementsEvent;
            ScapeClient.Instance.ScapeSession.ScapeMeasurementsRequested += OnScapeMeasurementsRequested;
        }

        /// <summary>
        /// update root on main thread
        /// </summary>
        public void Update()
        {
            UpdateWorldTransform();
        }

        /// <summary>
        /// called by ScapeSessionComponent. 
        /// Here the scape measurement is used to update the world transform object 
        /// in order to position and orient the camera with respect to the scene's origin.
        /// </summary>
        /// <param name="coordinates">
        /// GPS Coordinates given by scape measurements
        /// </param>
        /// <param name="heading">
        /// The compass heading given by scape measurements
        /// </param>        
        /// <param name="altitude">
        /// The the altitude from the ground the camera is at. This is currently supplied by 
        /// ARKit/Core using the GroundTracker
        /// </param>        
        public void SynchronizeARCamera(LatLng coordinates, float heading, float altitude) 
        {
            if (groundTracker) 
            {
                bool success = false;
                float height = groundTracker.GetGroundHeight(out success);
                if (success) 
                {
                    altitude = -height;
                }
                else 
                {
                    ScapeLogging.LogError(message: "groundTracker.getHeight not found before ScapeMeasurement, falling back to Scape's RawMeasurementEstimate");
                }
            }

            ScapeLogging.LogDebug(message: "SynchronizeARCamera() LatLngCoordinates = " + ScapeUtils.CoordinatesToString(coordinates));

            ScapeLogging.LogDebug(message: "SynchronizeARCamera() ARHeading = " + rotationAtScapeMeasurements.y);
            ScapeLogging.LogDebug(message: "SynchronizeARCamera() ARPosition = " + positionAtScapeMeasurements.ToString());
            
            if (s2CellId == 0) 
            {
                FindS2CellId(coordinates);
            }

            // the Unity position the camera should be in, that is it's position relative to the S2 cell based on it's
            // gps coordinates
            cameraS2Position = ScapeUtils.WgsToLocal(
                                                    coordinates.Latitude, 
                                                    coordinates.Longitude, 
                                                    altitude, 
                                                    s2CellId);
            
            // the world transform direction corrects the camera's Heading to be relative to North.
            worldTransformDirection = heading - rotationAtScapeMeasurements.y;

            if (worldTransformDirection < 0.0) 
            {
                worldTransformDirection += 360.0f;
            }

            ScapeLogging.LogDebug(message: "SynchronizeARCamera() worldTransformDirectionYAngle = " + worldTransformDirection);
            
            Vector3 positionAtScapeMeasurementsRotated = Quaternion.AngleAxis(worldTransformDirection, Vector3.up) * positionAtScapeMeasurements;

            // the world transform position corrects the camera's final position after applying the direction correction
            worldTransformPosition = cameraS2Position - positionAtScapeMeasurementsRotated;
            ScapeLogging.LogDebug(message: "SynchronizeARCamera() worldTransformPosition = " + worldTransformPosition.ToString());

            if (updateWorldTransform)
            {
                previousYDirection = lerpDirection;
                previousPosition = lerpPosition;
            }

            updateWorldTransform = true;

            updateStartTime = Time.time;
        }

        /// <summary>
        /// Synchronize camera from scape measurements
        /// </summary>
        /// <param name="scapeMeasurements">
        /// The scapeMesurements struct returned from the API
        /// </param>
        private void OnScapeMeasurementsEvent(ScapeMeasurements scapeMeasurements)
        {
            if (scapeMeasurements.MeasurementsStatus == ScapeMeasurementStatus.ResultsFound) 
            {
                SynchronizeARCamera(
                                    scapeMeasurements.LatLng, 
                                    (float)scapeMeasurements.Heading, 
                                    (float)scapeMeasurements.RawHeightEstimate);
            }
        }

        /// <summary>
        /// At the point the core requests the event (this can be later than the client requesting it due
        /// to the time taken to acquire and process the image), the camera records it's current position in
        /// AR space.
        /// </summary>
        /// <param name="timestamp">
        /// The timestamp at which the measurement is being taken, ignored here
        /// </param>
        private void OnScapeMeasurementsRequested(double timestamp)
        {
            HoldCameraPose();
        }

        /// <summary>
        /// create the world tranform object and insert it above the camera object in the scene
        /// </summary>
        private void SetupCameraParent()
        {
            var cameraParent = transform.parent;

            worldTransformObject = new GameObject();

            transform.SetParent(worldTransformObject.transform, false);

            if (cameraParent) 
            {
                worldTransformObject.transform.SetParent(cameraParent.transform, false);
            }
        }

        /// <summary>
        /// update the world transform object having been given new scape measurements
        /// </summary>
        private void UpdateWorldTransform() 
        {
            if (updateWorldTransform) 
            {
                ScapeLogging.LogDebug(message: "GeoCameraComponent::UpdateWorldTransform()");

                float elapsed = Time.time - updateStartTime;

                float lerpDirection = worldTransformDirection;
                Vector3 lerpPosition = worldTransformPosition;
                if (elapsed >= updateTransformTime || previousYDirection == 0.0f) 
                {
                    previousYDirection = worldTransformDirection;
                    previousPosition = worldTransformPosition;
                    updateWorldTransform = false;
                }
                else 
                {
                    float lerp = elapsed / updateTransformTime;
                    lerpDirection = Mathf.Lerp(previousYDirection, worldTransformDirection, lerp);
                    lerpPosition = Vector3.Lerp(previousPosition, worldTransformPosition, lerp);
                }

                worldTransformObject.transform.rotation = Quaternion.AngleAxis(lerpDirection, Vector3.up);
                worldTransformObject.transform.position = lerpPosition;

                PrintError();
            }
        }

        /// <summary>
        /// print some debug output for logging purposes
        /// </summary>
        private void PrintError() 
        {
            ScapeLogging.LogDebug(message: "CameraS2Position = " + cameraS2Position.ToString());
            ScapeLogging.LogDebug(message: "CameraCWPosition = " + transform.position.ToString());
        }

        /// <summary>
        /// Identify which S2Cell will be used for the root. 
        /// </summary>
        /// <param name="latLng">
        /// the LatLng coordinates from the first scape measurement
        /// </param>
        private void FindS2CellId(LatLng latLng) 
        {
            s2CellId = ScapeUtils.CellIdForWgs(latLng.Latitude, latLng.Longitude, ScapeUtils.S2CellLevel);
        }
    }
}