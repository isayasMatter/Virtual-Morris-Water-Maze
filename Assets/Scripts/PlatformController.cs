using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlatformController : MonoBehaviour {

    public delegate void PlatformReached();
    public static event PlatformReached OnPlatformReached;

	void OnTriggerEnter(Collider col)
    {        
        if(OnPlatformReached != null && col.gameObject.tag == "Player")
        {
            if(!MainManager.onPlatform){
                OnPlatformReached();
                Debug.Log("Platform reached");
            }            
        }
        
    }
}
