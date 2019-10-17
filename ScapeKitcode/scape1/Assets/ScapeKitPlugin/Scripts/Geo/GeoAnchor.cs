//  <copyright file="GeoAnchor.cs" company="Scape Technologies Limited">
//
//  GeoAnchor.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using UnityEngine;
 
    /// <summary>
    /// When attached to a [GameObject](https://docs.unity3d.com/ScriptReference/GameObject.html) 
    /// the root transform of that object will be repositioned according to it's latitude and longitude coordinates 
    /// to be globally correct with respect to the origin of the scene.
    /// When a GeoAnchored component starts it registers itself with the GeoAnchorManager and 
    /// it sets it's [GameObject](https://docs.unity3d.com/ScriptReference/GameObject.html)
    /// to inactive by default. This means the update function will not be called on it.
    /// The object will only become active after a successful ScapeMeasurement comes in.
    /// The GeoAnchorManager receives the ScapeMeasuremnt and calls the "OriginEvent" function 
    /// on all GeoAnchor's through the use of an Event.
    /// Any scene containing one or more GeoAnchor components must have a GeoAnchorManager component attached to an active 
    /// GameObject somewhere in the scene.
    /// </summary>
    public class GeoAnchor : MonoBehaviour, IGeoOrigin
    {
        /// <summary>
        /// The latLng coordinates that anchor the game object this component is attached to 
        /// to a real world location
        /// </summary>
        [SerializeField]
        private LatLng latLng;

        /// <summary>
        /// The height from the ground in meters.
        /// </summary>
        [SerializeField]
        private double altitude = 0.0;

        /// <summary>
        /// The maximum distance from the scene's origin the object will be instantiated.
        /// This is evaluated at the point a scape measurement comes is applied to the scene.
        /// </summary>
        [SerializeField]
        private double maxDistance = 1000.0;

        /// <summary>
        /// A string value holding a copy of the GameObject's name, this component is attached to.
        /// Used for logging purposes.
        /// </summary>
        private string gameObjectName;

        /// <summary>
        /// Keeps track of whether the object has been instantiated yet.
        /// </summary>
        private bool isInstantiated = false;

        /// <summary>
        /// Gets or sets the LatLng. 
        /// If setting on an active object, this will potentially update the GameObject's localPosition.
        /// This requires a conversion from LatLng coords to Unity coords, which may incur a performance penalty. 
        /// </summary>
        public LatLng LatLng 
        {
            get
            { 
                return latLng; 
            }
            
            set 
            { 
                latLng = value; 
                CalculateLocalCoordinates();
            }
        }

        /// <summary>
        /// Gets or sets altitude
        /// </summary>
        public double Altitude
        {
            get 
            { 
                return altitude; 
            }
            
            set 
            { 
                altitude = value;
                CalculateLocalCoordinates();
            }
        }

        /// <summary>
        /// Gets or sets maxDistance
        /// </summary>
        public double MaxDistance
        {
            get { return maxDistance; }
            set { maxDistance = value; }
        }

        /// <summary>
        /// Returns a boolean value indicating whether this object is less than the
        /// maximum distance from the Scene's center as specified by maxDistance
        /// </summary>
        /// <returns>True if the object should be instantiated.</returns>
        public bool WithinMaxDistance() 
        {
            return this.gameObject.transform.localPosition.magnitude < maxDistance;
        }

        /// <summary>
        /// Upon Start up the GeoAnchor component will deactivate the [gameobject](https://docs.unity3d.com/ScriptReference/GameObject.html) untill a scape measurement
        /// arrives, in order to position it's place in the Unity scene. 
        /// </summary>
        public void Start() 
        {       
            gameObjectName = this.gameObject.name;

            this.gameObject.SetActive(false);
            
            GeoAnchorManager.Instance.RegisterGeoInterface(this);
        }

        /// <summary>
        /// called form amin thread, used to continually try to find an anchor point 
        /// if one is wanted
        /// </summary>
        public void Update() 
        {
        }

        /// <summary>
        /// The OriginEvent function is connected to the GeoAnchorManager's GeoOriginEvent action
        /// which is triggered when the ScapeSessionObject receives a successful scape measurement.
        /// At this point the object calculates it's position in the Unity scene and is set to active.
        /// Due to the update to the GameObject's transform, it must be ensured that this function is
        /// only called on the main thread.
        /// </summary>
        public void OriginEvent() 
        {
            ScapeLogging.LogDebug(message: "GeoAnchor::OriginEvent() " + gameObjectName);

            isInstantiated = true;

            CalculateLocalCoordinates();

            if (WithinMaxDistance()) 
            {
                this.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// The game object calculates it's position in the Unity scene by comparing its World coordinate to the center
        /// of the unique S2 cell defined in the scene's GeoAnchorManager object.
        /// </summary>
        private void CalculateLocalCoordinates() 
        {
            if (!isInstantiated)
            {
                return;
            }

            ScapeLogging.LogDebug(message: "GeoAnchor::GetWorldCoordinates() WorldCoords = " + latLng.Latitude + ", " + latLng.Longitude);

            Vector3 scenePos = ScapeUtils.WgsToLocal(latLng.Latitude, latLng.Longitude, altitude, GeoAnchorManager.Instance.S2CellId);

            ScapeLogging.LogDebug(message: "GeoAnchor::GetWorldCoordinates() ScenePos = " + scenePos.ToString());
            
            this.gameObject.transform.localPosition = scenePos;
        }
    } 
}