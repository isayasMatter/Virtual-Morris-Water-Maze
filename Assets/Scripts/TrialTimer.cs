using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TrialTimer : MonoBehaviour {    
    
    public float startTime;    
    public float trialTime;
    private bool done;
    public float elapsed;

    public delegate void TimeOut();
    public static event TimeOut OnTimeOut;

    public delegate void StartTimer();
    public static event StartTimer OnStartTimer;

	// Use this for initialization
	void Start () {
        
	}

    void Enable(){
        OnStartTimer += startTrialTimer;
    }
	
    void startTrialTimer(){
        startTime = Time.time;
        done = false;        
        elapsed = 0f;
    }
    
	// Update is called once per frame
	void Update () {
        if (!done)
        {
            float currentTime = Time.time;            

            elapsed = currentTime - startTime;
            if ((elapsed > trialTime) && (OnTimeOut != null))
                OnTimeOut();   
                done = true;         
        }
	}
    
}