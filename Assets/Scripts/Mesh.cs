using UnityEngine;
using System.Collections;

public class Mesh : MonoBehaviour 
{
	
	public float normalisation;		//Float for Normalisation resistant (1 = instant 0 = no normalisation)

	public UnityEngine.Mesh mesh;	//Mesh from GameObject
	
	public Vector3[] newVertices;	//Vertices for Mesh manipulation
	private Vector3[] Vertices;		//Vertices of the Mesh
		
	// Use this for initialization
	void Start () 
	{
		mesh = GetComponent<MeshFilter>().mesh;
		
		Vertices = mesh.vertices;	//Original Mesh vertices
		newVertices = mesh.vertices;	//Mesh vertices for manipulation
		
		//Check normalisation area
		if(normalisation < 0.0f) normalisation = 0.0f;
		if(normalisation > 1.0f) normalisation = 1.0f;

	}
	
	// Update is called once per frame
	void Update () 
	{
		Normalisation();
		
		mesh.vertices = newVertices;	
	}
	
	void Normalisation()
	{
		for(int i=0; i < newVertices.Length; i++)
		{
			if(newVertices[i].x != Vertices[i].x)
			{
				newVertices[i].x -= ((newVertices[i].x - Vertices[i].x)* normalisation);
			}
			if(newVertices[i].y != Vertices[i].y)
			{
				newVertices[i].y -= ((newVertices[i].y - Vertices[i].y)* normalisation);
			}
			if(newVertices[i].z != Vertices[i].z)
			{
				newVertices[i].z -= ((newVertices[i].z - Vertices[i].z)* normalisation);
			}
			
		}
		
	}
	
	void OnCollisionEnter(Collision collision)
	{
		
	}
}
