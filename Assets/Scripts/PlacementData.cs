using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class PlacementData {

	public string participantID;	
	public int blockID;
	public List<Vector3> trajectoryPositions;

	public void export(){
		StringBuilder sb = new StringBuilder();
        sb.Append(Application.dataPath);
        sb.Append(Path.AltDirectorySeparatorChar).Append("ExperimentData").Append(Path.AltDirectorySeparatorChar);
        sb.Append("P").Append(participantID).Append("_");
        sb.Append("B").Append(blockID).Append("_");
        sb.Append("Placement");        
        sb.Append(".csv");
       

        StreamWriter writer = new StreamWriter(sb.ToString());

        sb = new StringBuilder();

        sb.Append("ParticipantID,");
		sb.Append("BlockID,");	

        sb.Append("x,z\n");
        
        foreach(Vector3 point in trajectoryPositions)        
        {
            sb.Append(participantID).Append(",");
		    sb.Append(blockID).Append(",");		   
            sb.Append(point.x).Append(",").Append(point.z).Append("\n");
        }

        writer.Write(sb);
        writer.Close();
	}
}


