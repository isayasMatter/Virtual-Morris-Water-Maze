using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExperimentSettings {
	private static bool onTraining, fovRestriction;
	private static string participantID, sessionNumber;
	
	public static bool OnTraining{
		get{
			return onTraining;
		}
		set{
			onTraining = value;
		}
	}

	public static bool FovRestriction{
		get{
			return fovRestriction;
		}
		set{
			fovRestriction = value;			
		}
	}

	public static string ParticipantID{
		get{
			return participantID;
		}
		set{
			participantID = value;
		}
	}
	
	public static string SessionNumber{
		get{
			return sessionNumber;
		}
		set{
			sessionNumber = value;
		}
	}
}
