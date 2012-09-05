//Created by Max Merchel

using UnityEngine;
using System.Collections;

public class MeshDeformation : MonoBehaviour {
	
	public float normalisation = 0.0025f;	//Normalisation resistant (1.0f = instant 0.0f = no normalisation)
	public float meshResistance = 1.0f;		//resistance to manipulation (0.0f = no deformation)

	public float maxDeformation = 10.0f;		//maximum deformation
	
	public float Deepfactor = 2.0f;					//Factor for collision deforation deepness
	public float SigmaManipulationFactor = 3.0f;	// Factor for collision radius (not 0.0f)
	private float InvSquareRoot2PI;					// 1/square root of 2*PI
	
	private UnityEngine.Mesh mesh;			//Mesh from GameObject
	private MeshCollider newMeshCollider;  //Mesh Collider
	
	public Vector3[] newVertices;			//Vertices for Mesh manipulation
	private Vector3[] startVertices;		//Vertices of the original Meshposition
	private Vector3[] deformationVector;	//Vector of Vertice deformation
	
	private Transform curTransform;
	private Quaternion Rotation;		//Rotation of the object as Quaternion
	private Vector3 ObjectRotation;		//Rotation of the Object as Vector 3 (Euler)

	public void Init()
	{
		Start ();
	}
	
	// Use this for initialization
	void Start () 
	{
		mesh = GetComponent<MeshFilter>().mesh;
		newMeshCollider = GetComponent(typeof(MeshCollider)) as MeshCollider; 
		
		curTransform = GetComponent<Transform>();
		
		Rotation = curTransform.rotation;
		
		
		startVertices = mesh.vertices;	//Original Mesh vertices
		newVertices = mesh.vertices;	//Mesh vertices for manipulation
		
		deformationVector = mesh.vertices;	//not perfect
		
		for(int i=0; i < startVertices.Length; i++)
		{	
			deformationVector[i] = Vector3.zero;
			#region Region Debug
			//Debug.Log(deformationVector[i]);
			#endregion
		}
		
		//Check meshResistance area
		if(meshResistance < 0.0f) meshResistance = 0.0f;
		if(meshResistance > 1.0f) meshResistance = 1.0f;

		//Check normalisation area
		if(normalisation < 0.0f) normalisation = 0.0f;
		if(normalisation > 1.0f) normalisation = 1.0f;
		
		//Check maxDeformation
		if(maxDeformation < 0.0f) maxDeformation = 0.0f;		
		
		//Check SigmaManipulationFactor
		if(SigmaManipulationFactor < 0.001f) SigmaManipulationFactor = 0.010f;	
		
		InvSquareRoot2PI = (1/(Mathf.Sqrt((2*Mathf.PI))));
		
		#region Region Debug
		//Debug.Log("Rotation: "+ Rotation);
		//Debug.DrawRay(curTransform.position, mesh.normals[3],Color.blue,15.5f);
		#endregion

	
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		BacktoNormal();
		mesh.vertices = newVertices;
		mesh.RecalculateBounds();
	}
	
	
	
	void Deformation()
	{
		
		for(int i=0; i < startVertices.Length; i++)
		{
			#region Region Debug
			//Debug.Log ("ID: "+ i +"Distance: "+ Vector3.Distance(Vector3.zero ,deformationVector[i]));
			#endregion
			
			//Reduce deformation to maximum defoermation
			if(Vector3.Distance(Vector3.zero ,deformationVector[i]) > maxDeformation)
			{
				float factor = maxDeformation/(Vector3.Distance(Vector3.zero ,deformationVector[i]));
				deformationVector[i].x = deformationVector[i].x * factor;
				deformationVector[i].y = deformationVector[i].y * factor;
				deformationVector[i].z = deformationVector[i].z * factor;
				
				#region Region Debug
				//Debug.Log ("ID: "+ i +"Changed Distance: "+ Vector3.Distance(Vector3.zero ,deformationVector[i]));
				#endregion
				
			}
			
			newVertices[i] = startVertices[i] + deformationVector[i];
		}
		
				
		mesh.vertices = newVertices;			
		mesh.RecalculateBounds();
		newMeshCollider.sharedMesh = mesh;	
		
		//Not good, but the mesh typ must change to work correct 
		newMeshCollider.smoothSphereCollisions = true;
		newMeshCollider.smoothSphereCollisions = false;
		
	}
	
	void BacktoNormal()
	{
		bool deformation = false;
		
		if(normalisation > 0.0f)
		{
			for(int i=0; i < deformationVector.Length; i++)
			{
				if(deformationVector[i] != Vector3.zero)
				{
					deformationVector[i] -= deformationVector[i] * normalisation * meshResistance;	
					deformation = true;
				}
			}
			
			if(deformation) Deformation();			
		}
	}
	
	void Manipulation(Vector3 firstContact, Vector3 firstContactDirection, float firstMagnitude)
	{
		bool deformation = false;
		
		float distance;
		float SigmaFactor = firstMagnitude/SigmaManipulationFactor;
		float reducesDistance;
		
				
		if(meshResistance > 0)
		{
			for(int i=0; i < deformationVector.Length; i++)
			{
				distance = Vector3.Distance(firstContact, (transform.TransformDirection(newVertices[i]) + transform.position));
				
				#region Region Gauss
				reducesDistance= distance/SigmaFactor*SigmaManipulationFactor;
			
				#region Region Debug
				//	Debug.Log("ID: "+ i + " Reduced Distance: " + reducesDistance + " Deformation: "+ (InvSquareRoot2PI * Mathf.Exp(-0.5f * (reducesDistance*reducesDistance))) );
				#endregion
				
				if(reducesDistance <= 2.0f)	
				{		
					
					//deformationVector[i].y -=  ((InvSquareRoot2PI * Mathf.Exp(-0.5f * (reducesDistance*reducesDistance))) * meshResistance * Deepfactor);
					if(Rotation != new Quaternion(0.0f,0.0f,0.0f,1.0f))
					{
					deformationVector[i] += Vector3.Cross(firstContactDirection, ObjectRotation.normalized)* ((InvSquareRoot2PI * Mathf.Exp(-0.5f * (reducesDistance*reducesDistance))) * meshResistance * Deepfactor);
					}
					else
					{
					deformationVector[i] += firstContactDirection * ((InvSquareRoot2PI * Mathf.Exp(-0.5f * (reducesDistance*reducesDistance))) * meshResistance * Deepfactor);
					}
					deformation = true;
					
					#region Region Debug
					//Debug.Log("Deformation!");
					//Debug.Log ("ID: "+ i);
					//Debug.Log ("ID: "+ i +" Deformation: "+ ((InvSquareRoot2PI * Mathf.Exp(-0.5f * (reducesDistance*reducesDistance))) * meshResistance));
					//Debug.Log ("Deformation Vector: " + deformationVector[i]);
					#endregion
				}
				
				
				#endregion //Gauss
			}
			
			if(deformation) Deformation();			
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{		
		Rotation = curTransform.rotation;
		ObjectRotation = Rotation.eulerAngles;
		
		Manipulation(collision.contacts[0].point, collision.contacts[0].normal, collision.relativeVelocity.magnitude);
		
		#region Region Debug
		//Debug.Log("Vector normal: "+ ObjectRotation.normalized);
		//Debug.Log("Contact normal: "+ collision.contacts[0].normal);
		
		//Debug.Log("Rotation: " + Rotation);
		//Debug.Log("Object Rotation: " + ObjectRotation);
		//Debug.Log("normalized: "+ collision.relativeVelocity.normalized);

		//Debug.Log("Vector3 Kreuz: "+ Vector3.Cross(collision.contacts[0].normal, ObjectRotation.normalized));
		//Debug.DrawRay(collision.contacts[0].point, ObjectRotation.normalized, Color.red,2);
		//Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.yellow,2);
		//Debug.DrawRay(collision.contacts[0].point, Vector3.Cross(collision.contacts[0].normal, ObjectRotation.normalized), Color.green,2);
		
		//Debug.DrawRay(collision.contacts[0].point, Vector3.Cross(new Vector3(collision.contacts[0].normal.x,0.0f,0.0f), ObjectRotation.normalized), Color.red,2);
		//Debug.DrawRay(collision.contacts[0].point, Vector3.Cross(new Vector3(0.0f,collision.contacts[0].normal.y,0.0f), ObjectRotation.normalized), Color.green,2);
		//Debug.DrawRay(collision.contacts[0].point, Vector3.Cross(new Vector3(0.0f,0.0f,collision.contacts[0].normal.z), ObjectRotation.normalized), Color.blue,2);
		//Debug.DrawRay(collision.contacts[0].point, Vector3.Cross(collision.contacts[0].normal, ObjectRotation.normalized), Color.white,2);
					

		//Debug.Log("magnitude: "+ collision.relativeVelocity.magnitude);	
		//Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.white,2);
		//Debug.DrawRay(collision.contacts[0].point, collision.relativeVelocity.normalized, Color.red,2);
		
		//Debug.Log("First Contact Point: " + collision.contacts[0].point);
		//Debug.Log("First Contact normal: "+ collision.contacts[0].normal);
		//Debug.Log("rotate normal: "+ collision.contacts[0].normal);
		#endregion
		
	}
	
}
