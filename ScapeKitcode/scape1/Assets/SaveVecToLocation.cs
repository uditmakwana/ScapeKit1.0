using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScapeKitUnity;
public class SaveVecToLocation : MonoBehaviour
{
   public LatLng cubelatlong;

    // Start is called before the first frame update
    void OnEnable()
    {

       

      
        cubelatlong = ScapeUtils.LocalToWgs(transform.position, GeoAnchorManager.Instance.S2CellId);

        PlayerPrefs.SetFloat("cubelatlon0", (float)cubelatlong.Latitude);
        PlayerPrefs.SetFloat("cubelatlon0", (float)cubelatlong.Longitude);

        PlayerPrefs.SetString("cubelat", cubelatlong.ToString());


        print(PlayerPrefs.GetString("cubelat"));
    }

   
}
