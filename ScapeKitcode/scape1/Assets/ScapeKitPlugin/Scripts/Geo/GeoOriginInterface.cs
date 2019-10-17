//  <copyright file="GeoOriginInterface.cs" company="Scape Technologies Limited">
//
//  GeoOriginInterface.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    /// <summary>
    /// Any object using the GeoOriginInterface should call the GeoAnchorManager.RegisterGeoInterface
    /// static function. Once registered it will receive the OriginEvent call whenever a scape
    /// measurement is received.
    /// </summary>
    public interface IGeoOrigin
    {
        /// <summary>
        /// The event that is called when a scape measurement arrives
        /// </summary>
        void OriginEvent();
    }
}