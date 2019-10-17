//  <copyright file="ScapeOrientationExtensions.cs" company="Scape Technologies Limited">
//
//  ScapeOrientationExtensions.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System.Collections;
    using ScapeKitUnity;
    using UnityEngine;

    /// <summary>
    /// Some utils to help working with ScapeKit Orientation
    /// </summary>
    public static class ScapeOrientationExtensions
    {       
        /// <summary>
        /// function get ToQuaternion
        /// </summary>
        /// <param name="o">
        /// input ScapeOrientation
        /// </param>
        /// <returns>
        /// returns unity Quaternion 
        /// </returns>
        public static Quaternion ToQuaternion(this ScapeOrientation o)
        {
            // Convert the right handed coordinates to left handed coordinates used by Unity (NED --> NWD)
            return new Quaternion(
                                (float)o.X,
                                (float)-o.Y,
                                (float)o.Z,
                                (float)-o.W);
        }
        
        /// <summary>
        /// function get Yaw
        /// </summary>
        /// <param name="o">
        /// input ScapeOrientation
        /// </param>
        /// <returns>
        /// returns Yaw in radians
        /// </returns>
        public static float Yaw(this ScapeOrientation o)
        {
            var q = o.ToQuaternion();
            return Mathf.Atan2(2.0f * ((q.y * q.z) + (q.w * q.x)), (q.w * q.w) - (q.x * q.x) - (q.y * q.y) + (q.z * q.z));
        }
        
        /// <summary>
        /// function get Pitch
        /// </summary>
        /// <param name="o">
        /// input ScapeOrientation
        /// </param>
        /// <returns>
        /// returns Pitch in radians
        /// </returns>
        public static float Pitch(this ScapeOrientation o)
        {
            var q = o.ToQuaternion();
            return Mathf.Asin(-2.0f * ((q.x * q.z) - (q.w * q.y)));
        }
        
        /// <summary>
        /// function get Roll
        /// </summary>
        /// <param name="o">
        /// input ScapeOrientation
        /// </param>
        /// <returns>
        /// returns Roll in radians
        /// </returns>
        public static float Roll(this ScapeOrientation o)
        {
            var q = o.ToQuaternion();
            return Mathf.Atan2(2.0f * ((q.x * q.y) + (q.w * q.z)), (q.w * q.w) + (q.x * q.x) - (q.y * q.y) - (q.z * q.z));
        }
        
        /// <summary>
        /// function get ToTrueScapeHeading
        /// </summary>
        /// <param name="q">
        /// input ScapeOrientation
        /// </param>
        /// <returns>
        /// returns ToTrueScapeHeading in degrees
        /// </returns>
        public static float ToTrueScapeHeading(this ScapeOrientation q)
        {
            Quaternion globalOrientationCamToNwd = q.ToQuaternion();

            // Project the camera outward facing z-axis into the NWD-frame
            Vector3 unitZ = new Vector3(0, 0, 1);
            Vector3 cameraDirectionInNwd = globalOrientationCamToNwd * unitZ;

            // The heading is the angle between the projection of z onto the xy-plane in the NWD-frame
            float heading = Mathf.Atan2(cameraDirectionInNwd.y, cameraDirectionInNwd.x);

            // Heading is measured from [0, 2*pi], not [-pi, pi]
            if (heading < 0.0f) 
            {
                heading += 2 * Mathf.PI;
            }

            // Convert from radians to degrees and return
            float trueHeading = heading * 180 / Mathf.PI;

            return trueHeading;
        }
    }
}
