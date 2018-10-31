using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExperimentSettings {
	private static bool experimentType, fovRestriction;
	private static string participantID, sessionNumber;
	
	public static bool ExperimentType{
		get{
			return experimentType;
		}
		set{
			experimentType = value;
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
