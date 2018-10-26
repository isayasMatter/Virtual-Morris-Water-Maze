using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class MainManager : MonoBehaviour {

	public Text infoText;
	public GameObject infoPanel;
	public GameObject player;
	public GameObject platform;
	public GameObject platformParent;
	public GameObject mainCamera;

	public GameObject cameraParent;
	
	public GameObject environment;
	public AudioClip timeOutClip;
	public AudioClip foundPlatformClip;
	public List<GameObject> landMarks;
	

	public string participantID;
	public int trialID;
	public int blockID;
	public bool FOVRestricted;


	public float trialDuration = 60.0f;
	public float delayBetweenTrials = 10.0f;
	public int numberOfTrials = 4;
	public int trialCounter;
	private bool onTrial;
	private bool onPlacementTask;
	private bool timeOut;
	public static bool onPlatform;


	public int numberOfBlocks = 2;
	public int placementTaskelevation = 100;
	private int blockCounter;


	private Trial currentTrial;
	private Vector3 trialInsertionPoint;
	List<Vector3> trajectoryPositions;
	List<Vector3> yawData;
	List<Vector3> platformTrajectoryPositions;
	private ExperimentPosition[] insertionPoints;
	private ExperimentPosition[] platformPositions;
	private Blocks[] blockMappings;
	private int[] insertionOrder;
	
	
	private AudioSource audioNotification;


	public delegate void PlatformFound();
    public static event PlatformFound OnPlatformFound;

	 
	public float startTime;    // timer started counting

	System.DateTime trialStartTime; 
    public bool stopTimer;		// is the trial finished?
    public float elapsed; 	// time elapsed since start of trial


    public delegate void TimeOut();
    public static event TimeOut OnTimeOut;


	// Use this for initialization
	void Start () {
		onTrial =  false;
		audioNotification = GetComponent<AudioSource>();

		TextAsset insPointsFile = Resources.Load("Text/InsertionPoints") as TextAsset;	
		insertionPoints = JsonHelper.FromJson<ExperimentPosition>(insPointsFile.text);

		TextAsset platformPositionsFile = Resources.Load("Text/PlatformPositions") as TextAsset;	
		platformPositions = JsonHelper.FromJson<ExperimentPosition>(platformPositionsFile.text);

		TextAsset blockMapppingsFile = Resources.Load("Text/Blocks") as TextAsset;	
		blockMappings = JsonHelper.FromJson<Blocks>(blockMapppingsFile.text);		

		trajectoryPositions = new List<Vector3>();
		yawData = new List<Vector3>();
		platformTrajectoryPositions = new List<Vector3>();

		infoText.text = "";	
		blockCounter = numberOfBlocks;	
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

		if (!stopTimer)
        {
            float currentTime = Time.time;            

            elapsed = currentTime - startTime;
            if ((elapsed > trialDuration) && (OnTimeOut != null)){
				stopTimer = true;
                OnTimeOut();   
			}                      
        }

		if (onTrial && Time.frameCount % 10 == 0)
        {
            trajectoryPositions.Add(transform.position);
			yawData.Add(mainCamera.transform.rotation.eulerAngles);
        }

		if (onPlacementTask && Time.frameCount % 10 == 0)
        {
            platformTrajectoryPositions.Add(platform.transform.position);
        }

	}

	void OnEnable(){
		PlatformController.OnPlatformReached += OnPlatformReached;
		PlatformMover.OnPositionSelected += OnPositionSelected;
		OnPlatformFound += StartPlacementTask;
		OnTimeOut += trialTimeOut;		
	}

	void OnDisable(){
		PlatformController.OnPlatformReached -= OnPlatformReached;
		PlatformMover.OnPositionSelected -= OnPositionSelected;
		OnPlatformFound -= StartPlacementTask;
		OnTimeOut -= trialTimeOut;		
	}

	void OnPlatformReached(){	
		onPlatform = true;		

		infoPanel.SetActive(false);
		infoText.text = "";

		if(onTrial){
			endTrial();
		}

		audioNotification.clip = foundPlatformClip;
		audioNotification.Play();

		platform.SetActive(true);

		GetComponent<PlayerController>().enabled = false;

		Vector3 newPos = platform.transform.position;
		newPos.y=0;
		transform.position = newPos;
		
		iTween.MoveBy(cameraParent, iTween.Hash("y",3, "easetype", iTween.EaseType.easeInOutSine, "time", 1));	
		

		if(AreThereMoreTrials()){			
			Invoke("StartTrial", delayBetweenTrials);
									
		}else{
			Invoke("StartPlacementTask", delayBetweenTrials);			
		}		
			
	}

	void StartPlacementTask(){	
		onPlacementTask = true;
		platform.SetActive(true);
		player.transform.position = new Vector3(0, placementTaskelevation, 0);
		player.transform.LookAt(platform.transform);
		platformParent.transform.position = new Vector3(0,0,0);
		//RotateLandMarks();	
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
		onPlacementTask = false;
		infoPanel.SetActive(true);
		infoText.text = "Thank you for placing the platform.";
		platform.GetComponent<PlatformMover>().enabled = false;

		//write data to file
		PlacementData placement = new PlacementData();
		placement.participantID = participantID;		
		placement.blockID = blockID;
		placement.trajectoryPositions = platformTrajectoryPositions;		
		placement.export();

		Invoke("StartBlock", delayBetweenTrials);		
	}

	void StartBlock(){
		trialCounter = numberOfTrials;
		blockID = numberOfBlocks - blockCounter;

		if(AreThereMoreBlocks()){
			platform.transform.localPosition = Vector3.zero;
			PositionPlatform();

			if(AreThereMoreTrials()){
				StartTrial();			
			}else{
				StartPlacementTask();
			}
		}else{
			infoPanel.SetActive(true);
			infoText.text = "This is the end of the experiment. Thank you for participating.";
		}
		
		blockCounter-=1;

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

	private bool AreThereMoreBlocks()
    {
        if(blockCounter > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	void StartTrial(){

		Debug.Log("started trial: " + trialCounter);		

		//fading transition effect
		Fade();	

		//ground camera	
		cameraParent.transform.localPosition = Vector3.zero;

		//hide information text
		infoPanel.SetActive(false);
		infoText.text = "";
		
		//reset trajectory
		trajectoryPositions.Clear();
		yawData.Clear();

		//hide platform
		platform.SetActive(false);

		//enable player movement
		platform.GetComponent<PlatformMover>().enabled = false;
		GetComponent<PlayerController>().enabled = true;
		

		//position player 
		InsertPlayer();	
		transform.rotation = Quaternion.LookRotation(transform.position - Vector3.zero);		
		onPlatform = false;

		onTrial = true;	
		startTrialTimer();
		trialID = numberOfTrials - trialCounter;
		trialCounter-=1;		
	}

	void InsertPlayer(){
		//read position from file and insert player into next position
		int insertionPt = blockMappings[blockCounter-1].trials[trialCounter-1];
		int x = insertionPoints[insertionPt-1].posX;
		int z = insertionPoints[insertionPt-1].posZ;
		Vector3 newPosition = new Vector3(x,0.5f,z);
		transform.position = newPosition;
		trialInsertionPoint = newPosition;
	}

	void PositionPlatform(){
		int x = platformPositions[blockCounter-1].posX;
		int z = platformPositions[blockCounter-1].posZ;

		Vector3 newPosition = new Vector3(x,0,z);
		platformParent.transform.position = newPosition;				
	}	

	void trialTimeOut(){
		audioNotification.clip = timeOutClip;
		audioNotification.Play();
		endTrial();
		infoPanel.SetActive(true);		
		infoText.text = "Your alloted time is over. \nSwim to the platform.";
		platform.SetActive(true);
	}

	void endTrial(){
		
		StopTrialTimer();
		onTrial = false;

		Trajectory traj = new Trajectory();
		traj.participantID = participantID;
		traj.trialID = trialID;
		traj.blockID = blockID;
		traj.trajectoryPositions = trajectoryPositions;
		traj.yawData = yawData;		
		traj.export();

		Trial trial = new Trial();
		trial.participantID = participantID;
		trial.trialID = trialID;
		trial.blockID = blockID;
		trial.FOVCondition = (FOVRestricted?"RY":"RN");
		trial.startTime = trialStartTime;
		trial.endTime = trialStartTime.Add(new System.TimeSpan(0,0,0,(int)elapsed));
		trial.completionTime = elapsed;
		trial.platformLocation = platform.transform.position;
		trial.insertionPoint = trialInsertionPoint;
		trial.export();
	}
	 void startTrialTimer(){
        startTime = Time.time;
        stopTimer = false;   
		trialStartTime = System.DateTime.Now;     
        elapsed = 0f;
    }

	void StopTrialTimer(){
		stopTimer = true;			
	}	

	void Fade(){
		SteamVR_Fade.Start(Color.black, 0);
		SteamVR_Fade.Start(Color.clear, 2);
	}

}
