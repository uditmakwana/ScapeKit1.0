//  <copyright file="ScapeSimpleUI.cs" company="Scape Technologies Limited">
//
//  ScapeSimpleUI.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System.Collections;
    using System.Collections.Generic;
    using ScapeKitUnity;
    using UnityEngine;
    #if UNITY_ANDROID && !UNITY_EDITOR
    using UnityEngine.Android;
    #endif
    using UnityEngine.UI;

    /// <summary>
    /// ScapeSimpleUI, a class for managing a Scape session using a simple gui
    /// </summary>
    public class ScapeSimpleUI : MonoBehaviour
    {
        /// <summary>
        /// A text field to write the printout from scape too
        /// </summary>
        [SerializeField]
        private Text textField;

        /// <summary>
        /// The button to trigger get measurements
        /// </summary>
        [SerializeField]
        private Button button;

        /// <summary>
        /// The Loading Circles to show localisation processing
        /// </summary>
        [SerializeField]
        private GameObject loadingCircles;

       /// <summary>
       /// set button enabled in main thread
       /// </summary>
       private bool setButtonEnabled = true;

        /// <summary>
        /// The text to be updated
        /// </summary>
        private string newText;

        /// <summary>
        /// a boolean to signal a text update on the main thread
        /// </summary>
        private bool updateText = false;

        /// <summary>
        /// a boolean to activate loading circles
        /// </summary>
        private bool showLoadingCicles = false;

        /// <summary>
        /// At start register the callbacks to the scape client session events
        /// </summary>
        public void Start()
        {
            InitScape();
        }

        /// <summary>
        /// if the text has an update do it here on main thread
        /// </summary>
        public void Update()
        {
            if (updateText) 
            {
                textField.text = newText;
                updateText = false;
            }

            if (button.interactable != setButtonEnabled)
            {
               button.interactable = setButtonEnabled;
            }

            if (loadingCircles.active != showLoadingCicles) 
            {  
                loadingCircles.SetActive(showLoadingCicles);
            }
        }

        /// <summary>
        /// register for scape callbacks
        /// </summary>
        private void InitScape()
        {   
            // Register callbacks
            ScapeClient.Instance.ScapeSession.ScapeMeasurementsRequested += ScapeMeasurementsRequested;
            ScapeClient.Instance.ScapeSession.ScapeMeasurementsEvent += OnScapeMeasurementsEvent;
            ScapeClient.Instance.ScapeSession.ScapeSessionErrorEvent += OnScapeSessionError;
        }

        /// <summary>
        /// this call is connected to the GetMeasurements signal,
        /// when that get's called (potentially from anywhere), we reset the text
        /// </summary>
        /// <param name="ts">
        /// the timestamp for when the request was made
        /// </param>
        private void ScapeMeasurementsRequested(double ts)
        {
            newText = newText + "\nFetching...";
            showLoadingCicles = true;
            updateText = true;
            setButtonEnabled = false;
        }

        /// <summary>
        /// on scape measurements result, print the result to the Text box.
        /// </summary>
        /// <param name="scapeMeasurements">
        /// scapeMeasurements from scape system
        /// </param>
        private void OnScapeMeasurementsEvent(ScapeMeasurements scapeMeasurements)
        {
            if (scapeMeasurements.MeasurementsStatus == ScapeMeasurementStatus.ResultsFound)
            {
                // Use the scape scape position
                newText = "OnScapeMeasurementsEvent:\n" +
                    "timestamp: " + scapeMeasurements.Timestamp + "\n" + 
                    "coordinates: " + scapeMeasurements.LatLng.Longitude + " " + scapeMeasurements.LatLng.Latitude + "\n" + 
                    "heading: " + scapeMeasurements.Heading + "\n" +  
                    "rawHeightEstimate: " + scapeMeasurements.RawHeightEstimate + "\n" + 
                    "confidenceScore: " + scapeMeasurements.ConfidenceScore + "\n" + 
                    "measurementsStatus: " + scapeMeasurements.MeasurementsStatus + "\n\n";
                
                updateText = true;
                setButtonEnabled = true;
                showLoadingCicles = false;
            }

            ScapeLogging.LogDebug("ScapeSimpleUI::OnScapeMeasurementsEvent()");
        }

        /// <summary>
        /// on error print the error
        /// </summary>
        /// <param name="scapeDetails">
        /// scapeDetails from scape system
        /// </param>
        private void OnScapeSessionError(ScapeSessionError scapeDetails)
        {
            // print a ScapeSessionError
            newText = "OnScapeSessionError:\n" + scapeDetails.State + "\n" + scapeDetails.Message + "\n";
            updateText = true;
            showLoadingCicles = false;

            // try again on failure
            ScapeClient.Instance.TakeMeasurements();
        }
    }
}
