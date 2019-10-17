//  <copyright file="ScapeDebugConfigEditor.cs" company="Scape Technologies Limited">
//
//  ScapeDebugConfigEditor.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

#if UNITY_EDITOR
    [CustomEditor (typeof (ScapeDebugConfig))]
    class ScapeDebugConfigEditor : Editor 
    {
        SerializedProperty logLevelProp;
        SerializedProperty logOutputProp;
        SerializedProperty debugMockGPSProp;
        SerializedProperty mockGPSLongitudeProp;
        SerializedProperty mockGPSLatitudeProp;
        SerializedProperty mockScapeResultsProp;
        SerializedProperty mockScapeLongitudeProp;
        SerializedProperty mockScapeLatitudeProp;
        SerializedProperty mockScapeHeadingProp;
        SerializedProperty mockScapeMeasurementsDelayProp;
        SerializedProperty saveImagesProp;

        public void OnEnable()
        {            
            logLevelProp = serializedObject.FindProperty("logLevel");
            logOutputProp = serializedObject.FindProperty("logOutput");
            debugMockGPSProp = serializedObject.FindProperty("debugMockGPS");
            mockGPSLongitudeProp = serializedObject.FindProperty("mockGPSLatLng.Longitude");
            mockGPSLatitudeProp = serializedObject.FindProperty("mockGPSLatLng.Latitude");
            mockScapeResultsProp = serializedObject.FindProperty("mockScapeResults");
            mockScapeLongitudeProp = serializedObject.FindProperty("mockScapeLatLng.Longitude");
            mockScapeLatitudeProp = serializedObject.FindProperty("mockScapeLatLng.Latitude");
            mockScapeHeadingProp = serializedObject.FindProperty("mockScapeHeading");
            mockScapeMeasurementsDelayProp = serializedObject.FindProperty("mockScapeMeasurementsDelay");
            saveImagesProp = serializedObject.FindProperty("saveImages");
        }

        public override void OnInspectorGUI() 
        {            
            serializedObject.Update();

            var logLevelGUI = new GUIContent("Log Level");
            logLevelGUI.tooltip = "controls the amount of logging that gets output";
            EditorGUILayout.PropertyField(logLevelProp, logLevelGUI);
            var logOutputGUI = new GUIContent("Log Output");
            logOutputGUI.tooltip = "controls where the logging gets output";
            EditorGUILayout.PropertyField(logOutputProp, logOutputGUI);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            var debugMockGPSGUI = new GUIContent("Mock GPS");
            debugMockGPSGUI.tooltip = "If this flag is set, the MockGPS coordinates will override the results of the device's GPS sensors.\n";
            EditorGUILayout.PropertyField(debugMockGPSProp, debugMockGPSGUI);
            if(debugMockGPSProp.boolValue)
            {
                var mockGPSLatitudeGUI = new GUIContent("Mock GPS Latitude");
                mockGPSLatitudeGUI.tooltip = "The Mock Latitude in degrees";
                EditorGUILayout.PropertyField(mockGPSLatitudeProp, mockGPSLatitudeGUI);
                
                var mockGPSLongitudeGUI = new GUIContent("Mock GPS Longitude");
                mockGPSLongitudeGUI.tooltip = "The Mock Longitude in degrees";
                EditorGUILayout.PropertyField(mockGPSLongitudeProp, mockGPSLongitudeGUI);
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            var mockScapeResultsGUI = new GUIContent("Mock Scape Results");
            mockScapeResultsGUI.tooltip = "If used scape client will not be used and these results will be immediately returned instead";
            EditorGUILayout.PropertyField(mockScapeResultsProp, mockScapeResultsGUI);
            if(mockScapeResultsProp.boolValue) 
            {
                var mockScapeLatitudeGUI = new GUIContent("Mock Scape Latitude");
                mockScapeLatitudeGUI.tooltip = "The Mock Scape Latitude in degrees";
                EditorGUILayout.PropertyField(mockScapeLatitudeProp, mockScapeLatitudeGUI);
             
                var mockScapeLongitudeGUI = new GUIContent("Mock Scape Longitude");
                mockScapeLongitudeGUI.tooltip = "The Mock Scape Longitude in degrees";
                EditorGUILayout.PropertyField(mockScapeLongitudeProp, mockScapeLongitudeGUI);

                var mockScapeHeadingGUI = new GUIContent("Mock Scape Heading");
                mockScapeHeadingGUI.tooltip = "The mocked compass heading";
                EditorGUILayout.PropertyField(mockScapeHeadingProp, mockScapeHeadingGUI);

                var mockScapeMeasurementsDelayGUI = new GUIContent("Mock Scape Measurements Delay");
                mockScapeMeasurementsDelayGUI.tooltip = "Delay in seconds. When using mocked scape measurements it is more realistic to have the measurements return after some delay.";
                EditorGUILayout.PropertyField(mockScapeMeasurementsDelayProp, mockScapeMeasurementsDelayGUI);
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            var saveImagesGUI = new GUIContent("Save Images");
            saveImagesGUI.tooltip = "If used images sent to scape's back end are also saved to local device storage";
            EditorGUILayout.PropertyField(saveImagesProp, saveImagesGUI);
        
            serializedObject.ApplyModifiedProperties ();
        }
    }
#endif
}
