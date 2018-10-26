using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class Trial {
	public int trialID;
	public string participantID;
	public int blockID;
	public string FOVCondition;	
	public Vector3 platformLocation;
	public Vector3 insertionPoint;
	public System.DateTime startTime;
	public System.DateTime endTime;
	public float completionTime;	

	public void export(){
		StringBuilder sb = new StringBuilder();
        sb.Append(Application.dataPath);
        sb.Append(Path.AltDirectorySeparatorChar).Append("ExperimentData").Append(Path.AltDirectorySeparatorChar);
        sb.Append("trials");        
        sb.Append(".csv");
       
	   	string fileName = sb.ToString();

		sb = new StringBuilder();

        if(!File.Exists(fileName)){			
			//add csv headers			
			sb.Append("ParticipantID,");
			sb.Append("BlockID,");
			sb.Append("trialID,");
			sb.Append("FOVCondition,");	
			sb.Append("StartTime,");	
			sb.Append("EndTime,");
			sb.Append("CompletionTime,");
			sb.Append("PlatformLocation.x,PlatformLocation.z,");
			sb.Append("InsertionPoint.x,InsertionPoint.z\n");
		}else{			
			//add data
			sb.Append(participantID).Append(",");
			sb.Append(blockID).Append(",");
			sb.Append(trialID).Append(",");
			sb.Append(FOVCondition).Append(",");	
			sb.Append(startTime.ToString()).Append(",");	
			sb.Append(endTime.ToString()).Append(",");
			sb.Append(completionTime).Append(",");
			sb.Append(platformLocation.x).Append(",");
			sb.Append(platformLocation.z).Append(",");
			sb.Append(insertionPoint.x).Append(",");
			sb.Append(insertionPoint.z).Append("\n");  

		}

		StreamWriter writer = new StreamWriter(fileName, true);
		writer.Write(sb);
        writer.Close();       
	}
}
