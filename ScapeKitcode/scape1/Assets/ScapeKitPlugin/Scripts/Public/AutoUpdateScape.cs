//  <copyright file="AutoUpdateScape.cs" company="Scape Technologies Limited">
//
//  AutoUpdateScape.cs
//  ScapeKitUnity
//
//  Created by nick on 6/9/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// A behaviour to request ScapeMeasurements repeatedly
    /// Can be configured to either call new measurements after a specific time interval or after th camera has moved a specific distance
    /// </summary> 
    public class AutoUpdateScape : MonoBehaviour
    {
        /// <summary>
        /// checkCameraPointsUp, only attempts to send image when camera is pointing above horizontal
        /// </summary>
        [SerializeField]
        private bool checkCameraPointsUp = true;

        /// <summary>
        /// TimeoutUpdate, controls when another GetMeasurement gets called due to timeout
        /// </summary>
        [SerializeField]
        private float timeoutUpdate = -1.0f;

        /// <summary>
        /// distanceUpdate, controls when another GetMeasurement gets called due to camera movement
        /// </summary>
        [SerializeField]
        private float distanceUpdate = -1.0f;

        /// <summary>
        /// timeSinceUpdate, counter for timeout
        /// </summary>
        private float timeSinceUpdate = 0.0f;

        /// <summary>
        /// positionAtLastUpdate, used to check distance update
        /// </summary>
        private Vector3 positionAtLastUpdate;
        
        /// <summary>
        /// initialize callbacks to ScapeSession
        /// This can only be called after the client has been started
        /// </summary>
        public void Start()
        {
            ScapeClient.Instance.ScapeSession.ScapeSessionErrorEvent += OnScapeSessionError;
            ScapeClient.Instance.ScapeSession.ScapeMeasurementsEvent += OnScapeMeasurementsEvent;

            positionAtLastUpdate = Camera.main.transform.localPosition;
        }

        /// <summary>
        /// check whether its time for another measurement
        /// </summary>
        public void Update()
        {
            CheckRequiresUpdate();
        }

        /// <summary>
        /// Callback for ScapeMeasurements update
        /// </summary>
        /// <param name="scapeMeasurements">
        /// the information passed from ScapeKit
        /// </param>
        private void OnScapeMeasurementsEvent(ScapeMeasurements scapeMeasurements)
        {
            ResetUpdateVars();
        }

        /// <summary>
        /// Callback for ScapeSessionError update
        /// </summary>
        /// <param name="scapeDetails">
        /// the information passed from ScapeKit
        /// </param>
        private void OnScapeSessionError(ScapeSessionError scapeDetails)
        {
            ResetUpdateVars();
            ScapeClient.Instance.ScapeSession.GetMeasurements();
        }

        /// <summary>
        /// CheckRequiresUpdate, based on time and camera movements
        /// </summary>
        private void CheckRequiresUpdate() 
        {
            bool requiresUpate = false;
            
            if (distanceUpdate > 0.0f && Camera.main) 
            {
                float movement = (Camera.main.transform.localPosition - positionAtLastUpdate).magnitude;
                if (movement > distanceUpdate) 
                {
                    ScapeClient.Instance.ScapeSession.GetMeasurements();
                    ResetUpdateVars();
                    return;
                }
            }

            timeSinceUpdate += Time.deltaTime;

            if (timeoutUpdate > 0.0f && timeSinceUpdate > timeoutUpdate) 
            {
                ScapeClient.Instance.ScapeSession.GetMeasurements();
                ResetUpdateVars();
                return;
            }
        }

        /// <summary>
        /// Set values to begin timing again to count until the next ScapeMeasurements update
        /// should be requested if autoUpdate is set to true; 
        /// </summary>
        private void ResetUpdateVars() 
        {   
            if (Camera.main) 
            {
                positionAtLastUpdate = Camera.main.transform.localPosition;
            }

            timeSinceUpdate = 0;
        }
    }
}