using UnityEngine;
using System.Collections;

public class Mesh : MonoBehaviour 
{
	
	public Vector3[] newVertices;
	
	public UnityEngine.Mesh mesh;
		
	// Use this for initialization
	void Start () 
	{
		//UnityEngine.Mesh mesh = new UnityEngine.Mesh();
		//GetComponent<MeshFilter>().mesh = mesh;
		mesh = GetComponent<MeshFilter>().mesh;
		
		newVertices = mesh.vertices;

	}
	
	// Update is called once per frame
	void Update () 
	{
		mesh.vertices = newVertices;
	}
}
