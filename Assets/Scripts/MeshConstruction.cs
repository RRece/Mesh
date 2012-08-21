using UnityEngine;
using System.Collections;

public class MeshConstruction : MonoBehaviour 
{
	
	public GameObject NewObject;					//GameObject for the New Object
	public string ObjectName = "New GameObject";	//Name of the New GameObject
	
	public Vector3 ObjectCenter;		//Center of the new Object
	public Vector3 ObjectRotation;		//Rotation of the new Object
	private Quaternion RotationQuat;	//Rotation Quaternion of the new Object
	
	public float ObjectHeight = 10.0f;		//Height of the new Objec
	public float ObjectWidth = 10.0f;		//Width of the new Object
	//public float ObjectDepth = 10.0f;		//Depth of the new Object
	
	public float MaxObjectMeshHeight = 1.0f;	//Max Mech section Height
	private float MeshHeight;
	public float MaxObjectMeshWidth  = 1.0f;	//Max Mesh section Width
	private float MeshWidth;
	
	
	private int SectionHeight;
	private int SectionWidth;
	

	private Vector3[] newVertices;		//Vertices for Mesh creation
	private Vector2[] newUVs;			//UVs for Mesh creation
	private int[] newTriangles;			//Triangles for Mesh creation
	
	
	private UnityEngine.Mesh newMesh;		//Mesh from GameObject
	private MeshCollider newMeshCollider;	//Mesh Collider	
	private Transform newTransform;			//Transform of the new Object
	
	public bool DestroyAfterConstruction = false; 
	
	// Use this for initialization
	void Start () 
	{
		
		if(ObjectWidth > 0.0f && ObjectHeight > 0.0f)
		{
			CreateObject();	//Funktion
			CreateMesh();	//Funktion
			
			
		#region Region Debug
		//Debug.Log ("New Mesh: "+ newMesh);
		//Debug.Log ("New Mesh Collider: "+ newMeshCollider);
		
		//Debug.Log ("Sections %1: "+ ((ObjectWidth / MaxObjectMeshWidth) % 1));
		//Debug.Log ("Sections: "+ (ObjectWidth / MaxObjectMeshWidth));
		//Debug.Log ("Sections(int): "+ (int)(ObjectWidth / MaxObjectMeshWidth));
		//Debug.Log ("Sections(int+1): "+ (int)((ObjectWidth / MaxObjectMeshWidth) +1));
		#endregion
			if(DestroyAfterConstruction)
			{
				Destroy(this.gameObject);
			}
			
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
		NewObject.name = ObjectName;
		
		NewObject.AddComponent("MeshRenderer");
			
		
		newMesh = NewObject.AddComponent<MeshFilter>().mesh;
		newMeshCollider = NewObject.AddComponent(typeof(MeshCollider)) as MeshCollider; 
		
		newTransform = NewObject.GetComponent<Transform>();
		newTransform.position = ObjectCenter;
		
		RotationQuat = Quaternion.Euler(ObjectRotation);
		newTransform.rotation = RotationQuat;
	}
	
	void CreateMesh()
	{
		CreateMeshSections();	//Funktion
		
		
		newMesh.vertices = newVertices;
		newMesh.uv = newUVs;
		newMesh.triangles = newTriangles;
		
		newMesh.RecalculateBounds();
		newMesh.RecalculateNormals();
		
		newMeshCollider.sharedMesh = newMesh;	
		
		//Not good, but the mesh typ must change to work correct 
		newMeshCollider.smoothSphereCollisions = true;
		newMeshCollider.smoothSphereCollisions = false;

		
		
	}
	
	
	void CreateMeshSections()
	{
		CalculateMeshSectionSize();	//Funktion
	
		float HalfMeshHeight = ObjectHeight / 2;
		float HalfMeshWidth = ObjectWidth / 2;
		
		newVertices = new Vector3[((SectionHeight+1) * (SectionWidth+1))];
		newUVs = new Vector2[((SectionHeight+1) * (SectionWidth+1))];
		
		for(int i = 0; i <= SectionHeight; i++)
		{
			for(int j = 0; j <= SectionWidth; j++)
			{
				newVertices[(i * (SectionWidth+1) + j)] = new Vector3(((i * MeshHeight) - HalfMeshHeight),((j * MeshWidth) - HalfMeshWidth), ObjectCenter.z);
				newVertices[(i * (SectionWidth+1) + j)].x = ((i * MeshHeight) - HalfMeshHeight);
				newVertices[(i * (SectionWidth+1) + j)].y = ((j * MeshWidth) - HalfMeshWidth);
				newVertices[(i * (SectionWidth+1) + j)].z = ObjectCenter.z;
				
				newUVs[(i * (SectionWidth+1) + j)] = new Vector2(newVertices[(i * (SectionWidth+1) + j)].x,newVertices[(i * (SectionWidth+1) + j)].z);
		
				#region Region Debug
				//Debug.Log("Section Height: " + i + " Section Width: " + j);
				//Debug.Log("Number of Current Vertice: "+ (i * (SectionWidth+1) + j));
				//Debug.Log("Current Vertice: " + newVertices[(i * (SectionWidth+1) + j)]);
				//Debug.Log("Vertice[" + i + "][" + j + "]: " + newVertices[i][j]);
				#endregion
			}
		}
		
		CalculateTriangles();	//Funktion

				
		#region Region Debug
		//Debug.Log("Number of Vertices: "+ ((SectionHeight+1) * (SectionWidth+1)));
		#endregion
		
	}
	
	void CalculateTriangles()	//ToDo
	{

		int length = ((SectionHeight) * (SectionWidth)) * 6;
		newTriangles = new int[length];	
		
		#region Region Debug
		//Debug.Log("Length: " + length);
		#endregion
		
		int j=0;
		int line = 0;
		//for(int i = 0; i < length-1; i++)
		for(int i = 0; i < ((SectionHeight+1) * (SectionWidth+1))-2; i++)
		{
			if(i % (SectionWidth + 1) != 0 || i < SectionWidth + 1)
			{
				if(line > 0 )
				{
					if(line < SectionHeight )
					{
						newTriangles[j] = i;				
						newTriangles[j+1] = i+1;					
						newTriangles[j+2] = line * (SectionWidth + 1) + i;	
						j+=3;
					}

					
					newTriangles[j] = i;					
					newTriangles[j+1] = i+1;					
					newTriangles[j+2] = (line-1) * (SectionWidth + 1) + i;
					
					j+=3;				
					
				}
				else
				{
					newTriangles[j] = i;				
					newTriangles[j+1] = i+1;					
					newTriangles[j+2] = line * (SectionWidth + 1) + i;
					j+=3;
				}
				
				
			
			
			}
			else
			{
				line++;
			}
		
			#region Region Debug
			//Debug.Log("I: " + i + " J: " + j + " Line: " + line);
			#endregion
			
		}
		
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
		
		#region Region Debug
		//Debug.Log("Section Height: " + SectionHeight + " Section Width: " + SectionWidth);
		//Debug.Log("Mesh Height: " + MeshHeight + " Mesh Width: " + MeshWidth);
		#endregion
		
	}
	
	

	
	
	
	
	
	
	// Update is called once per frame
	void Update () 
	{
	
	}


}
