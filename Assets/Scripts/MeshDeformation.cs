using UnityEngine;
using System.Collections;

public class MeshDeformation : MonoBehaviour {
	
	public float normalisation = 0.0025f;	//Normalisation resistant (1.0f = instant 0.0f = no normalisation)
	public float meshResistance = 1.0f;		//resistance to manipulation (0.0f = no deformation)

	public float maxDeformation = 10.0f;		//maximum deformation
	
	#region changed when using Gauss
	/*
	public float radiusStrong = 0.2f;	//Radius of strong manipulatet Vertices
	private float radiusMedium;			//Radius of medium manipulatet Vertices	
	private float radiusLow;			//Radius of low manipulatet Vertices
	
	public float radiusStrongToMedium = 2.0f;	//Strong * radiusfactor = Medium
	public float radiusMediumToLow = 1.5f; 		//Medium * radiusfactor = Low
	
	public float factorStrong = 7.5f;	//Factor of Strong Manipulation
	public float factorMedium = 5.0f;	//Factor of Medium Manipulation
	public float factorLow = 2.5f;		//Factor of Low Manipulation
	*/
	#endregion
	
	public float InvSquareRoot2PI;	//1/square root of 2*PI
	
	private UnityEngine.Mesh mesh;			//Mesh from GameObject
	private MeshCollider newMeshCollider;  //Mesh Collider
	
	public Vector3[] newVertices;			//Vertices for Mesh manipulation
	private Vector3[] startVertices;		//Vertices of the original Meshposition
	private Vector3[] deformationVector;	//Vector of Vertice deformation

		
	
	// Use this for initialization
	void Start () 
	{
		mesh = GetComponent<MeshFilter>().mesh;
		newMeshCollider = GetComponent(typeof(MeshCollider)) as MeshCollider; 
		
		startVertices = mesh.vertices;	//Original Mesh vertices
		newVertices = mesh.vertices;	//Mesh vertices for manipulation
		
		deformationVector = mesh.vertices;	//not perfect
		
		for(int i=0; i < startVertices.Length; i++)
		{	
			deformationVector[i] = Vector3.zero;
			//Debug.Log(deformationVector[i]);
		}
		
		//Check meshResistance area
		if(meshResistance < 0.0f) meshResistance = 0.0f;
		if(meshResistance > 1.0f) meshResistance = 1.0f;

		//Check normalisation area
		if(normalisation < 0.0f) normalisation = 0.0f;
		if(normalisation > 1.0f) normalisation = 1.0f;
		
		//Check maxDeformation
		if(maxDeformation < 0.0f) maxDeformation = 0.0f;		
		
		#region changed when using Gauss
		/*
		//Check factors are positiv
		if(radiusStrongToMedium < 0.0f) radiusStrongToMedium = 0.0f;
		if(radiusMediumToLow < 0.0f) radiusMediumToLow = 0.0f;
		
		if(radiusStrong < 0.0f) radiusStrong = 0.0f;
		radiusMedium = radiusStrongToMedium * radiusStrong;
		radiusLow = radiusMediumToLow * radiusMedium;
		*/
		#endregion
		
		InvSquareRoot2PI = (1/(Mathf.Sqrt((2*Mathf.PI))));
		
		Debug.Log("Rotation: "+ transform.rotation);
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
			//Debug.Log ("ID: "+ i +"Distance: "+ Vector3.Distance(Vector3.zero ,deformationVector[i]));
			
			//Reduce deformation to maximum defoermation
			if(Vector3.Distance(Vector3.zero ,deformationVector[i]) > maxDeformation)
			{
				float factor = maxDeformation/(Vector3.Distance(Vector3.zero ,deformationVector[i]));
				deformationVector[i].x = deformationVector[i].x * factor;
				deformationVector[i].y = deformationVector[i].y * factor;
				deformationVector[i].z = deformationVector[i].z * factor;
				
				//Debug.Log ("ID: "+ i +"Changed Distance: "+ Vector3.Distance(Vector3.zero ,deformationVector[i]));
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
		float SigmaFactor = firstMagnitude;
		//float SigmaFactor = 3.0f;
		float reducesDistance;
		
				
		if(meshResistance > 0)
		{
			for(int i=0; i < deformationVector.Length; i++)
			{
				distance = Vector3.Distance(firstContact, (transform.TransformDirection(newVertices[i]) + transform.position));
				
				#region Gauss
				//force of collision is not integrated
				reducesDistance= distance/SigmaFactor;
			
				
				//	Debug.Log("ID: "+ i + " Reduced Distance: " + reducesDistance + " Deformation: "+ (InvSquareRoot2PI * Mathf.Exp(-0.5f * (reducesDistance*reducesDistance))) );
				
				if(reducesDistance <= 2.0f)	
				{
					
					
				//	Debug.Log("Reduced!");
					
					deformationVector[i].y -=  ((InvSquareRoot2PI * Mathf.Exp(-0.5f * (reducesDistance*reducesDistance))) * meshResistance);
					//deformationVector[i].y -=  (factorStrong * meshResistance);
					
					deformation = true;
					//Debug.Log ("ID: "+ i +" Strong");
					//Debug.Log ("ID: "+ i +" Strong: "+ (factorStrong * meshResistance));
					//Debug.Log (deformationVector[i]);
					
				}
				
				
				
				
				#endregion
				
				
				
				
				
				
				#region changed when using Gauss (comment)
				/*
				//check collisionarea
				if(distance <= radiusStrong)
				{
					//testing
					//deformationVector[i].y -= factorStrong * meshResistance;
					
					deformationVector[i].y -=  (factorStrong * meshResistance);
					
					deformation = true;
					//Debug.Log ("ID: "+ i +" Strong");
					//Debug.Log ("ID: "+ i +" Strong: "+ (factorStrong * meshResistance));
					//Debug.Log (deformationVector[i]);
					
				}
				else if(distance <= radiusMedium)
				{
					//testing
					deformationVector[i].y  -=  (factorMedium * meshResistance);
					
					deformation = true;
					//Debug.Log ("ID: "+ i +" Medium");
				}
				else if(distance <= radiusLow)
				{
					//testing
					deformationVector[i].y  -=  (factorLow * meshResistance);
					
					deformation = true;
					//Debug.Log ("ID: "+ i +" Low");
				}
				*/
				#endregion
				
				
				
				//ToDo:
				
				//check collison direction
				//check collisionforce (mass / speed(force)/ ...)
			}
			
			if(deformation) Deformation();			
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{		
		
		Debug.Log("normalized: "+ collision.relativeVelocity.normalized);
		Debug.Log("magnitude: "+ collision.relativeVelocity.magnitude);
		
			
		Manipulation(collision.contacts[0].point, collision.contacts[0].normal, collision.relativeVelocity.magnitude);
		Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.green,2);
		Debug.DrawRay(collision.contacts[0].point, collision.relativeVelocity.normalized, Color.red,2);
		
		Debug.Log("First Contact Point: " + collision.contacts[0].point);
		Debug.Log("First Contact normal: "+ collision.contacts[0].normal);
		//Debug.Log("rotate normal: "+ collision.contacts[0].normal);
		
		
		
	}
	
}
