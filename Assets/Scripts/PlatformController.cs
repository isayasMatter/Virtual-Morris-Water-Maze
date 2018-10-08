using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlatformController : MonoBehaviour {

    public delegate void PlatformReached();
    public static event PlatformReached OnPlatformReached;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter(Collider col)
    {
         Debug.Log("Platform reached " + col.gameObject.tag);
        if(OnPlatformReached != null && col.gameObject.tag == "Player")
        {
            OnPlatformReached();
            Debug.Log("Platform reached");
        }
        
    }
}
