using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartExperimentOnClick : MonoBehaviour {

	public InputField participantID;
	public InputField sessionNumber;
	public Toggle fovRestriction;
	public Toggle sessionType;

	public Text errorText;
	// Use this for initialization
	public void LoadExperiment(int sceneIndex){
		string pID = participantID.text;
		string sNo = sessionNumber.text;
		bool fovR = fovRestriction.isOn;
		bool sesT = sessionType.isOn;

		if((pID != "") && (sNo != "")){
			ExperimentSettings.ParticipantID = pID;
			ExperimentSettings.SessionNumber = sNo;
			ExperimentSettings.FovRestriction = fovR;
			ExperimentSettings.ExperimentType = sessionType;

			SceneManager.LoadScene(sceneIndex);
		}else{
			errorText.gameObject.SetActive(true);
		}
		
	}
}
