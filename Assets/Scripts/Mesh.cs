using UnityEngine;
using System.Collections;

public class Mesh : MonoBehaviour 
{
	
	public float normalisation = 0.0025f;	//Normalisation resistant (1 = instant 0 = no normalisation)
	public float meshResistance = 1.0f;		//resistance to manipulation
	
	public float radiusStrong = 0.2f;	//Radius of strong manipulatet Vertices
	private float radiusMedium;			//Radius of medium manipulatet Vertices		//Set Private if it work
	private float radiusLow;			//Radius of low manipulatet Vertices
	
	public float radiusStrongToMedium = 2.0f;	//Strong * radiusfactor = Medium
	public float radiusMediumToLow = 1.5f; 		//Medium * radiusfactor = Low
	
	public float factorStrong = 20.0f;	//Factor of Strong Manipulation
	public float factorMedium = 10.0f;	//Factor of Medium Manipulation
	public float factorSLow = 5.0f;		//Factor of Low Manipulation
		
	private UnityEngine.Mesh mesh;		//Mesh from GameObject
	
	public Vector3[] newVertices;	//Vertices for Mesh manipulation
	private Vector3[] Vertices;		//Vertices of the original Meshposition
	
	public Vector3 firstContact;			//Position of first collision poin
	
	
	// Use this for initialization
	void Start () 
	{
		mesh = GetComponent<MeshFilter>().mesh;
		
		Vertices = mesh.vertices;	//Original Mesh vertices
		newVertices = Vertices;	//Mesh vertices for manipulation
		
		//Check normalisation area
		if(normalisation < 0.0f) normalisation = 0.0f;
		if(normalisation > 1.0f) normalisation = 1.0f;
		
		//Check factors are positiv
		if(radiusStrongToMedium < 0.0f) radiusStrongToMedium = 0.0f;
		if(radiusMediumToLow < 0.0f) radiusMediumToLow = 0.0f;
		
		if(radiusStrong < 0.0f) radiusStrong = 0.0f;
		radiusMedium = radiusStrongToMedium * radiusStrong;
		radiusLow = radiusMediumToLow * radiusMedium;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		Normalisation();
		//Manipulation();	//Only on Collison (Test Mode)
		mesh.vertices = newVertices;	
	}
	
	//transform the Vertices back to the origenal Position
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
			
			/* 
			//Collision area
			Debug.DrawRay(firstContact, new Vector3(radiusLow,0,0), Color.green);
			Debug.DrawRay(firstContact, new Vector3(-radiusLow,0,0), Color.yellow);
			Debug.DrawRay(firstContact, new Vector3(0,radiusLow,0), Color.green);
			Debug.DrawRay(firstContact, new Vector3(0,-radiusLow,0), Color.yellow);
			Debug.DrawRay(firstContact, new Vector3(0, 0,radiusLow), Color.green);
			Debug.DrawRay(firstContact, new Vector3(0, 0,-radiusLow), Color.yellow);
			*/
			
			
			
		}
		
	}
	
	
	//do not work
	void OnCollisionEnter(Collision collision)
	{		
		firstContact = collision.contacts[0].point;		
		Manipulation();
	}
	
	
	void Manipulation()
	{
		
		for(int i=0; i < newVertices.Length; i++)
		{
		//	newVertices[i].y -= radiusMedium * meshResistance;
			
			//sphere area needed
			//Cube area are used
			if(
			((newVertices[i].x >= firstContact.x - radiusStrong - transform.position.x) && (newVertices[i].x <= firstContact.x + radiusStrong - transform.position.x)) &&
			((newVertices[i].y >= firstContact.y - radiusStrong - transform.position.y) && (newVertices[i].y <= firstContact.y + radiusStrong - transform.position.y)) &&
			((newVertices[i].z >= firstContact.z - radiusStrong - transform.position.z) && (newVertices[i].z <= firstContact.z + radiusStrong - transform.position.z)))
			{
				newVertices[i].y -= factorStrong * meshResistance;
			}
			else if(
			((newVertices[i].x >= firstContact.x - radiusMedium - transform.position.x) && (newVertices[i].x <= firstContact.x + radiusMedium - transform.position.x)) &&
			((newVertices[i].y >= firstContact.y - radiusMedium - transform.position.y) && (newVertices[i].y <= firstContact.y + radiusMedium - transform.position.y)) &&
			((newVertices[i].z >= firstContact.z - radiusMedium - transform.position.z) && (newVertices[i].z <= firstContact.z + radiusMedium - transform.position.z)))
			{
				newVertices[i].y -= factorMedium * meshResistance;
			}
			else if(
			((newVertices[i].x >= firstContact.x - radiusLow - transform.position.x) && (newVertices[i].x <= firstContact.x + radiusLow - transform.position.x)) &&
			((newVertices[i].y >= firstContact.y - radiusLow - transform.position.y) && (newVertices[i].y <= firstContact.y + radiusLow - transform.position.y)) &&
			((newVertices[i].z >= firstContact.z - radiusLow - transform.position.z) && (newVertices[i].z <= firstContact.z + radiusLow - transform.position.z))
				)
			{
				newVertices[i].y -= factorSLow * meshResistance;		
			}		
			
		}
		
	}

}