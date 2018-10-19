using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour {

	public Text infoText;
	public Text participantCode;

	public float trialDuration = 4.0f;
	public int numberOfTrials = 8;

	private int trialCounter;

	private bool onTrial;
	private bool timeOut;

	List<Vector3> playerPathPositions;
	List<Vector3> platformPathPositions;

	public GameObject infoPanel;
	public GameObject player;
	public GameObject platform;
	public List<GameObject> landMarks;
	private AudioSource audioNotification;


	public delegate void PlatformFound();
    public static event PlatformFound OnPlatformFound;

	// Use this for initialization
	void Start () {
		audioNotification = GetComponent<AudioSource>();
		infoText.text = "";
		trialCounter = numberOfTrials;
		StartBlock();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            if (OnPlatformFound != null)
            {
                OnPlatformFound();
            }
        }

		if(onTrial && !timeOut){
			playerPathPositions.Add(player.transform.position);
		}

	}

	void OnEnable(){
		PlatformController.OnPlatformReached += OnPlatformReached;
		PlatformMover.OnPositionSelected += OnPositionSelected;
		OnPlatformFound += StartPlacementTask;
	}

	void OnPlatformReached(){		
		
		audioNotification.Play();
		// infoPanel.SetActive(true);		
		// infoText.text = "Congratulations you have found the platform!\nPress the \"Y\" key to continue.";
		// GetComponent<PlayerController>().enabled = false;

		if(AreThereMoreTrials()){
			StartTrial();						
		}else{
			StartPlacementTask();
		}			
	}

	void StartPlacementTask(){
		player.transform.position = new Vector3(0,30,0);
		player.transform.Rotate(90,0,0);
		platform.transform.position = new Vector3(0,0,0);
		RotateLandMarks();	
		infoText.text = "Please use the joystick to place the platform where it was.\nPress the \"A\" key to confirm.";
		platform.GetComponent<PlatformMover>().enabled = true;
	}

	void RotateLandMarks(){
		for(int i=0; i<landMarks.Count; i++){
			landMarks[i].transform.Rotate(0,0,90);
			if(landMarks[i].transform.position.x < 0){
				landMarks[i].transform.position += Vector3.right;
			}else if(landMarks[i].transform.position.x > 0){
				landMarks[i].transform.position -= Vector3.right;
			}else if(landMarks[i].transform.position.z > 0){
				landMarks[i].transform.position -= Vector3.forward;
			}else {
				landMarks[i].transform.position += Vector3.forward;
			}
		}	

	}

	void OnPositionSelected(){
		infoText.text = "Thank you for placing the platform. \nYou have finished the experiment.";
		platform.GetComponent<PlatformMover>().enabled = false;
	}

	void StartBlock(){
		PositionPlatform();

		if(AreThereMoreTrials()){
			StartTrial();
			
		}else{
			StartPlacementTask();
		}

	}

	private bool AreThereMoreTrials()
    {
        if(trialCounter > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	void StartTrial(){
		trialCounter--;
		InsertPlayer();		
	}

	void InsertPlayer(){
		
	}
	

}
