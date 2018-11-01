using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartExperimentOnClick : MonoBehaviour {

	public InputField participantID;
	public InputField sessionNumber;
	public Toggle fovRestriction;
	public Toggle onTraining;

	public Text errorText;
	// Use this for initialization
	public void LoadExperiment(int sceneIndex){
		string pID = participantID.text;
		string sNo = sessionNumber.text;
		bool fovR = fovRestriction.isOn;
		bool onT = onTraining.isOn;

		if((pID != "") && (sNo != "")){
			ExperimentSettings.ParticipantID = pID;
			ExperimentSettings.SessionNumber = sNo;
			ExperimentSettings.FovRestriction = fovR;
			ExperimentSettings.OnTraining = onT;

			SceneManager.LoadScene(sceneIndex);
		}else{
			errorText.gameObject.SetActive(true);
		}
		
	}
}
