using UnityEngine;
using System.Collections;

public class MeshConstruction : MonoBehaviour {
	
/*	public GameObject NewObject;	//GameObject for the New Object
	
	public Vector3 ObjectCenter;		//Center of the new Object
	public Vector3 ObjectRotation;		//Rotation of the new Object
	private Quaternion RotationQuat;	//Rotation Quaternion of the new Object
	
	public float ObjectHeight = 1.0f;		//Height of the new Objec
	public float ObjectWidth = 1.0f;		//Width of the new Object
	//public float ObjectDepth = 1.0f;		//Depth of the new Object
	
	public float MaxObjectMeshHeight = 1.0f;	//Max Mech section Height
	private float MeshHeight;
	public float MaxObjectMeshWidth  = 1.0f;	//Max Mesh section Width
	private float MeshWidth;
	
	
	private int SectionHeight;
	private int SectionWidth;
	

	private Vector3[][] newVertices;		//Vertices for Mesh creation
	//private Vector3[][] newVertices;		//Vertices for Mesh creation
	
	
	private UnityEngine.Mesh newMesh;			//Mesh from GameObject
	private MeshCollider newMeshCollider;	//Mesh Collider	
	
	
	
	// Use this for initialization
	void Start () 
	{
		
		if(ObjectWidth > 0.0f && ObjectHeight > 0.0f)
		{
			CreateObject();
			CreateMeshSections();
			
			
		#region Region Debug
		//Debug.Log ("New Mesh: "+ newMesh);
		//Debug.Log ("New Mesh Collider: "+ newMeshCollider);
		
		Debug.Log ("Sections %1: "+ ((ObjectWidth / MaxObjectMeshWidth) % 1));
		Debug.Log ("Sections: "+ (ObjectWidth / MaxObjectMeshWidth));
		Debug.Log ("Sections(int): "+ (int)(ObjectWidth / MaxObjectMeshWidth));
		Debug.Log ("Sections(int+1): "+ (int)((ObjectWidth / MaxObjectMeshWidth) +1));
		#endregion
		}	
		else
		{
			#region Region Debug
			Debug.Log ("No GameObejct created");
			#endregion
			
		}
			

	}
	
	void CreateObject()
	{
		NewObject = new GameObject();
				
		newMesh = NewObject.AddComponent<MeshFilter>().mesh;
		newMeshCollider = NewObject.AddComponent(typeof(MeshCollider)) as MeshCollider; 
		
		NewObject.transform.position = ObjectCenter;
		RotationQuat = Quaternion.Euler(ObjectRotation);
		NewObject.transform.rotation = RotationQuat;
	}
	
		
	void CreateMeshSections()
	{
		CalculateMeshSectionSize();
	
		float HalfMeshHeight = ObjectHeight / 2;
		float HalfMeshWidth = ObjectWidth / 2;
		
		//newVertices = new Vector3[(SectionHeight * SectionWidth)];
		newVertices = new Vector3[SectionHeight][SectionWidth];
		
		for(int i = 0; i < SectionHeight; i++)
		{
			for(int j = 0; j < SectionWidth; j++)
			{
				//newVertices[i][j] = new Vector3(((i*MeshHeight) - HalfMeshHeight),((j * MeshWidth) - HalfMeshWidth), ObjectCenter.z);
				newVertices[i][j].x = ((i*MeshHeight) - HalfMeshHeight);
				newVertices[i][j].y = ((j * MeshWidth) - HalfMeshWidth);
				newVertices[i][j].z = ObjectCenter.z;
				
					
					
				#region Region Debug
				Debug.Log("Vertice[" + i + "][" + j + "]: " + newVertices[i][j]);
				#endregion
			}
		}
		
		
		//ObjectWidth
		//MaxObjectMeshWidth
		//MeshWidth
		
		#region Region Debug
		#endregion
		
	}
	
	void CalculateMeshSectionSize()
	{
		
		if((ObjectHeight / MaxObjectMeshHeight) % 1 == 0.0f)
		{
			SectionHeight = (int)(ObjectHeight / MaxObjectMeshHeight);
		}
		else
		{
			SectionHeight = (int)(ObjectHeight / MaxObjectMeshHeight) + 1;
		}
		MeshHeight = ObjectHeight / SectionHeight;
		
		if((ObjectWidth / MaxObjectMeshWidth) % 1 == 0.0f)
		{
			SectionWidth = (int)(ObjectWidth / MaxObjectMeshWidth);
		}
		else
		{
			SectionWidth = (int)(ObjectWidth / MaxObjectMeshWidth) + 1;
		}
		MeshWidth = ObjectWidth / SectionWidth;
		
	}
	
	
	 */
	
	
	
	
	
	
	// Update is called once per frame
	void Update () 
	{
	
	}


}
