//  <copyright file="MonoBehaviourSingleton.cs" company="Scape Technologies Limited">
//
//  MonoBehaviourSingleton.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using UnityEngine;
    using System.Collections;
    
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T BehaviourInstance
        {
            get
            {
                if (instance == null)
                {
                    GameObject coreGameObject = new GameObject(typeof(T).Name);

                    instance = coreGameObject.AddComponent<T>();
                }

                return instance;
            }
        }

        private static T instance;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = GetComponent<T>();
                DontDestroyOnLoad(this);
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }
        }
    }
}
