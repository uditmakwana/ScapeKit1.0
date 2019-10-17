//  <copyright file="ScapeClient.cs" company="Scape Technologies Limited">
//
//  ScapeClient.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.XR.ARFoundation;

    #if PLATFORM_ANDROID
    using UnityEngine.Android;
    #endif
    
    /// <summary>
    /// class encapsualting all scapekit functionality
    /// </summary>
    public class ScapeClient : MonoBehaviour
    {
        /// <summary>
        /// static instance of the client
        /// </summary>
        private static ScapeClient instance = null;

        /// <summary>
        /// the filename containing the scapeAPIkey
        /// </summary>
        private static string apikeyFileName = "ScapeAPIKey";

        /// <summary>
        /// the filepath containing the scapeAPIkey file
        /// </summary>
        private static string resPath = "Assets/Resources/";

        /// <summary>
        /// The interface to the underlying native lib
        /// </summary>
        private ScapeClientNative nativeClient = null;

        /// <summary>
        /// The ScapeSession object
        /// THe underlying implementation of the ScapeSession obj is only initialized
        /// when ScapeClient Impl is,
        /// however the events can be registered too before then.
        /// </summary>
        private ScapeSession scapeSession = new ScapeSession();

        /// <summary>
        /// the scape api key.
        /// This should be entered through the unity gui
        /// Unity Menu SCapekit -> Account  
        /// </summary>
        private string apiKey = "XXX";     
        
        /// <summary>
        /// theCamera, main camera object of Unity scene.
        /// Must be set in order to use scape measurements with AR Camera
        /// </summary>
        [SerializeField]
        private ARCameraManager arCameraManager;

        /// <summary>
        /// A scriptable object which can be optionally added to provide 
        /// debug support options
        /// </summary>
        [SerializeField]
        private ScapeDebugConfig debugConfig;

        /// <summary>
        /// Gets static instance of the client
        /// </summary>
        public static ScapeClient Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Gets the instance of the ScapeSession
        /// There is always a ScapeSession available, even before the underlying
        /// native client has been started. 
        /// </summary>
        public ScapeSession ScapeSession
        {
            get
            {
                return scapeSession;
            } 
        }

        /// <summary>
        /// Gets the instance of the DebugSession
        /// A debug session is only available after the native client has been started
        /// and if a ScapeDebugConfig was applied to this ScapeClient
        /// </summary>
        public DebugSession DebugSession
        {
            get 
            {
                return nativeClient.DebugSession;
            }
        }

        /// <summary>
        /// Gets or sets the DebugConfig
        /// </summary>
        public ScapeDebugConfig DebugConfig
        {
            get
            {
                return debugConfig;
            }

            set
            {
                debugConfig = value;
            }
        }

        /// <summary>
        /// Gets or sets the apiKey
        /// </summary>
        protected virtual string ApiKey
        {
            get
            {
                return this.apiKey;
            }

            set
            {
                this.apiKey = value;
            }
        }

        /// <summary>
        /// save the api key to the specific file in resources folder
        /// </summary>
        /// <param name="apiKey">
        /// the apikey as string
        /// </param>
        public static void SaveApiKeyToResource(string apiKey) 
        {
            try
            {
                if (apiKey.Length == 0) 
                {
                    return;
                }

                if (!Directory.Exists(resPath))
                {
                    Directory.CreateDirectory(resPath);
                }
                
                using (StreamWriter writer = new StreamWriter(resPath + apikeyFileName + ".txt", false))
                {
                    writer.WriteLine(apiKey.Trim());
                }
            }
            catch (Exception e)
            {
                ScapeLogging.LogError(message: "Failed to save apikey to '" + resPath + "'");
            }
        }

        /// <summary>
        /// used at runtime to retrieve the api key for the ScapeClient
        /// </summary>
        /// <returns>
        /// returns the apikey if found
        /// </returns>
        public static string RetrieveKeyFromResources()
        {
            try
            {
            #if UNITY_EDITOR
                using (StreamReader streamReader = new StreamReader(resPath + apikeyFileName + ".txt")) 
                {
                    string apiKey = streamReader.ReadLine();
                
                    return apiKey;
                }

            #else
                string apiKey = Resources.Load<TextAsset>(apikeyFileName).ToString();

                return apiKey;
            #endif
            }
            catch (Exception ex)
            {
                ScapeLogging.LogError("Exception retrieving apikey: " + ex.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// init permissions before scene load
        /// ensures location works first time
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitPermissions()
        {
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }
#endif
        }

        /// <summary>
        /// A public function to request ScapeMeasurements anytime
        /// </summary>
        public void TakeMeasurements() 
        {
            Instance.ScapeSession.GetMeasurements();
        }

        /// <summary>
        /// create c interface on awake
        /// </summary>
        public void Awake()
        {
            if (instance != null) 
            {
                Debug.Log("ERROR: There should only be one ScapeClient in a scene!");
                return;
            }
            
            instance = this;
            nativeClient = new ScapeClientNative();
        }

        /// <summary>
        /// start c core 
        /// </summary>
        public void Start()
        {
            nativeClient.StartClient(RetrieveKeyFromResources(), debugConfig != null);

            if (debugConfig != null)
            {
                debugConfig.ConfigureDebugSession(nativeClient.DebugSession);
            }

            scapeSession.SetNative(nativeClient.ScapeSessionNative);

            if (arCameraManager != null)
            {
                scapeSession.SetCameraManager(arCameraManager);
            }
        }

        /// <summary>
        /// update ScapeSession in main thread
        /// </summary>
        public void Update() 
        {
            if (nativeClient != null)
            {
                nativeClient.Update();
            }
            
            scapeSession.Update();
        }

        /// <summary>
        /// terminate client on scene
        /// </summary>
        public void OnDestroy()
        {
            nativeClient.Terminate();

            instance = null;
        }

        /// <summary>
        /// returnws whether the client has started
        /// </summary>
        /// <returns>
        /// boolean value
        /// </returns>
        public bool IsStarted()
        {
            if (nativeClient != null)
            {
                return nativeClient.IsStarted();
            }

            return false;
        }
    }
}
