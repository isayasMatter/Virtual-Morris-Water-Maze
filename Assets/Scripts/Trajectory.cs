using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class Trajectory {

	public string participantID;
    public string sessionNumber;
    public int blockID;
	public int trialID;	
	public List<Vector3> trajectoryPositions;
    public List<float> trajectoryTimeStamps;
    public List<Vector3> yawData;

	public void export(){
		StringBuilder sb = new StringBuilder();
        sb.Append(Application.dataPath);
        sb.Append(Path.AltDirectorySeparatorChar).Append("ExperimentData").Append(Path.AltDirectorySeparatorChar);
        sb.Append("P").Append(participantID).Append("_");
        sb.Append("B").Append(blockID).Append("_");
        sb.Append("T").Append(trialID);        
        sb.Append(".csv");
       

        StreamWriter writer = new StreamWriter(sb.ToString());

        sb = new StringBuilder();

        sb.Append("ParticipantID,");
		sb.Append("BlockID,");
		sb.Append("trialID,");
        sb.Append("time-stamp,");
        sb.Append("position-x,position-z,");
        sb.Append("head-yaw-x,head-yaw-y,head-yaw-z\n");
        
        int index =0;
        foreach(Vector3 point in trajectoryPositions)        
        {
            sb.Append(participantID).Append(",");
		    sb.Append(blockID).Append(",");
		    sb.Append(trialID).Append(",");
            sb.Append(trajectoryTimeStamps[index]).Append(",");
            sb.Append(point.x).Append(",").Append(point.z).Append(",");
            sb.Append(yawData[index].x).Append(",").Append(yawData[index].y).Append(",").Append(yawData[index].z).Append("\n");
            index++;
        }

        writer.Write(sb);
        writer.Close();
	}
}


