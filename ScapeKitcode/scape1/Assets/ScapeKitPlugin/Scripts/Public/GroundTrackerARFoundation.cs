//  <copyright file="GroundTrackerARFoundation.cs" company="Scape Technologies Limited">
//
//  GroundTrackerARFoundation.cs
//  ScapeKitUnity
//
//  Created by nick on 6/9/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.XR.ARFoundation;
    using UnityEngine.XR.ARSubsystems;

    /// <summary>
    /// A class for monitoring plane tracking using ar platforms
    /// This class simply looks for horizontal planes.
    /// The first horizontal plane it finds is assumed to be the ground.
    /// This naive approach works for scenarios where the user is in a flat open space.
    /// </summary>
    public class GroundTrackerARFoundation : MonoBehaviour
    {
        /// <summary>
        /// Action triggered upon finding ground plane
        /// </summary>
        [SerializeField]
        private UnityEvent onGroundPlaneFound = new UnityEvent();

        /// <summary>
        /// boolean indicating whether the first plane has yet been found.
        /// </summary>
        private bool haveArPlanes = false;

        /// <summary>
        /// the height of the camera from the ground.
        /// </summary>
        private float groundHeight = 0.0f;

        /// <summary>
        /// The component must be attached to a a game object holding a ARPlaneManager
        /// </summary>
        private ARPlaneManager arPlaneManager;

        /// <summary>
        /// THe update function polls the ARPlaneManager until it finds a horizontal surface
        /// </summary>
        public void Update() 
        {
            if (arPlaneManager == null) 
            {
                arPlaneManager = GetComponent<ARPlaneManager>();
            }

            if (haveArPlanes) 
            {
                return;
            } 

            if (arPlaneManager) 
            {
                foreach (var plane in arPlaneManager.trackables)
                {
                    if (plane.alignment == PlaneAlignment.HorizontalUp) 
                    {
                        groundHeight = plane.gameObject.transform.position.y; 

                        haveArPlanes = true;

                        onGroundPlaneFound.Invoke();

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// retrieves the ground height if it has been found
        /// </summary>
        /// <param name="success">
        /// will be set to true if the ground has already been found
        /// </param>
        /// <returns>
        /// returns the height from the ground
        /// </returns>
        public float GetGroundHeight(out bool success)
        {
            success = haveArPlanes;

            return groundHeight;
        } 
    }
}