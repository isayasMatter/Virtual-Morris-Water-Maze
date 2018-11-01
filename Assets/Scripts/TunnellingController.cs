using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TunnellingController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(ExperimentSettings.FovRestriction){
			GetComponent<Tunnelling>().enabled = true;
		}else{
			GetComponent<Tunnelling>().enabled = false;
		}
	}
}
