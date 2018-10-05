using UnityEngine;
using System.Collections;

public class WaterPlaneEditor : MonoBehaviour {
    void Update() {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int i = 0;
		float largest = 0.0f;
		Vector3 largestVertex = Vector3.up;
        while (i < vertices.Length) {
            Vector3 vertex = vertices[i];
			if (vertex.z > largest){
				largest = vertex.z;
				largestVertex = vertex;
			}
            i++;
        }
        Debug.Log("Biggest vertex: " + largestVertex.ToString());
    }
	
}