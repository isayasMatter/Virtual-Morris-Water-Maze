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
	public GameObject environment;		//all objects except player

	public GameObject poolWall;
	public List<Texture> LandMarkTextures;
	public AudioClip timeOutClip;		
	public AudioClip foundPlatformClip;
	public List<GameObject> landMarks;
	

	public string participantID;
	public string sessionNumber;
	public int trialID;
	public int blockID;	


	public float trialDuration = 60.0f;
	public float delayBetweenTrials = 6.0f;
	public int numberOfTrials = 6;
	public int numberOfBlocks = 6;

	public int trialCounter;
	private bool onTrial;
	private bool onPlacementTask;
	private bool timeOut;
	public static bool onPlatform;
	private bool onTraining;

	public int numberOfLandMarks = 4;
	private int textureCounter = 0;
	
	public int placementTaskelevation = 100;
	private int blockCounter;

	private int[] fovBlockOrder = {5,3,2,6,4,1};
	private int[] noFovBlockOrder = {1,5,4,2,3,6};

	private Trial currentTrial;
	private Vector3 trialInsertionPoint;
	List<Vector3> trajectoryPositions;
	List<float> trajectoryTimeStamps;
	List<Vector3> yawData;
	List<Vector3> platformTrajectoryPositions;
	List<float> platformTrajectoryTimeStamps;
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

		participantID = ExperimentSettings.ParticipantID;
		sessionNumber = ExperimentSettings.SessionNumber;

		TextAsset insPointsFile = Resources.Load("Text/InsertionPoints") as TextAsset;	
		insertionPoints = JsonHelper.FromJson<ExperimentPosition>(insPointsFile.text);

		TextAsset platformPositionsFile = Resources.Load("Text/PlatformPositions") as TextAsset;	
		platformPositions = JsonHelper.FromJson<ExperimentPosition>(platformPositionsFile.text);

		TextAsset blockMapppingsFile = Resources.Load("Text/Blocks") as TextAsset;	
		blockMappings = JsonHelper.FromJson<Blocks>(blockMapppingsFile.text);		

		trajectoryPositions = new List<Vector3>();
		trajectoryTimeStamps = new List<float>();
		yawData = new List<Vector3>();
		platformTrajectoryPositions = new List<Vector3>();
		platformTrajectoryTimeStamps = new List<float>();

		infoText.text = "";			

		if(ExperimentSettings.OnTraining){
			onTraining = true;
			numberOfBlocks = 1;	
			numberOfTrials = 3;
			trialDuration = 20;		
		}

		blockCounter = numberOfBlocks;	
		StartBlock();			
	}
	
	// Update is called once per frame
	void Update () {

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
			trajectoryTimeStamps.Add(Time.time);
        }

		if (onPlacementTask && Time.frameCount % 10 == 0)
        {
            platformTrajectoryPositions.Add(platform.transform.position);
			platformTrajectoryTimeStamps.Add(Time.time);
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
		player.transform.eulerAngles = new Vector3(90,0,0);	

		float cameraY = mainCamera.transform.localEulerAngles.y;	
		Debug.Log(cameraY);	
		cameraParent.transform.Rotate(0,(360-cameraY),0);

		platformParent.transform.position = new Vector3(0,0,0);
		//RotateLandMarks();	
		infoText.text = "Please use the joystick to place the platform where it was.\nPress the \"A\" key to confirm.";
		//platform.GetComponent<Rigidbody>().isKinematic = false;
		platform.GetComponent<Collider>().isTrigger = false;
	
		platform.GetComponent<PlatformMover>().enabled = true;
	}

	/**
	Rotates landmarks for better view during placement task.
	 */
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

	/**
	Changes the texture (the images) in the landmarks at the start of every block
	 */
	void SetLandMarkTexture(){
		if(!onTraining){
			for(int i=0; i<landMarks.Count; i++){
				Renderer m_Renderer;
				m_Renderer =landMarks[i].GetComponent<Renderer>();
				m_Renderer.material.SetTexture("_MainTex", LandMarkTextures[i+textureCounter]);
			}
			textureCounter +=4;
		}else{
			for(int i=0; i<landMarks.Count; i++){
				Renderer m_Renderer;
				m_Renderer =landMarks[i].GetComponent<Renderer>();
				m_Renderer.material.SetTexture("_MainTex", LandMarkTextures[i*4]);
			}
		}
		
	}

	void OnPositionSelected(){
		onPlacementTask = false;
		infoPanel.SetActive(true);
		infoText.text = "Thank you for placing the platform.";
		platform.GetComponent<PlatformMover>().enabled = false;
		
		platform.GetComponent<Collider>().isTrigger = true;

		if(!onTraining){
		//write data to file
			PlacementData placement = new PlacementData();
			placement.participantID = participantID;	
			placement.sessionNumber = sessionNumber;	
			placement.blockID = blockID;
			placement.trajectoryPositions = platformTrajectoryPositions;
			placement.trajectoryTimeStamps = platformTrajectoryTimeStamps;		
			placement.export();
		}

		Invoke("StartBlock", delayBetweenTrials);		
	}

	void StartBlock(){
		trialCounter = numberOfTrials;
		blockID = numberOfBlocks - blockCounter;
		

		if(AreThereMoreBlocks()){
			//get the next order from a predefined list
			if(ExperimentSettings.FovRestriction) blockID = fovBlockOrder[blockID];
			else blockID = noFovBlockOrder[blockID];

			Debug.Log("started block: " + blockID);

			//reset platform position to orignal local position (with respect to parent)
			platform.transform.localPosition = Vector3.zero;

			//reset camera rotation to 0 (with respect to parent)
			cameraParent.transform.localRotation = Quaternion.Euler(0,0,0);

			//position the platform and set landmark textures
			PositionPlatform();
			SetLandMarkTexture();

			if(AreThereMoreTrials()){
				StartTrial();			
			}else{
				StartPlacementTask();
			}
		}else{
			infoPanel.SetActive(true);
			if(!onTraining){
				infoText.text = "This is the end of the experiment, please inform the experiment conductor.";
			}else{
				infoText.text = "This is the end of the training, please inform the experiment conductor.";
			}
			
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
		int insertionPt = blockMappings[blockID-1].trials[trialCounter-1];
		float x = insertionPoints[insertionPt-1].posX;
		float z = insertionPoints[insertionPt-1].posZ;

		Vector3 newPosition = new Vector3(x,0.5f,z);
		
		transform.position = newPosition;
		trialInsertionPoint = newPosition;
	}

	void PositionPlatform(){
		float x = platformPositions[blockID-1].posX;
		float z = platformPositions[blockID-1].posZ;

		Vector3 newPosition = new Vector3(x,0,z);

		if(onTraining){
			platformParent.transform.position = Vector3.zero;
		}else{
			platformParent.transform.position = newPosition;	
		}
					
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

		if(!onTraining){
			Trajectory traj = new Trajectory();
			traj.participantID = participantID;
			traj.sessionNumber = sessionNumber;
			traj.trialID = trialID;
			traj.blockID = blockID;
			traj.trajectoryPositions = trajectoryPositions;
			traj.trajectoryTimeStamps = trajectoryTimeStamps;
			traj.yawData = yawData;		
			traj.export();

			Trial trial = new Trial();
			trial.participantID = participantID;
			trial.sessionNumber = sessionNumber;
			trial.trialID = trialID;
			trial.blockID = blockID;
			trial.FOVCondition = (ExperimentSettings.FovRestriction?"RY":"RN");
			trial.startTime = trialStartTime;
			trial.endTime = trialStartTime.Add(new System.TimeSpan(0,0,0,(int)elapsed));
			trial.completionTime = elapsed;
			trial.platformLocation = platform.transform.position;
			trial.insertionPoint = trialInsertionPoint;
			trial.export();
		}
		
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
