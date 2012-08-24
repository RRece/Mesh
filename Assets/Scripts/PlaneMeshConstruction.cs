using UnityEngine;
using System.Collections;

public class PlaneMeshConstruction : MonoBehaviour 
{
	
	private GameObject NewObject;					//GameObject for the New Object
	public string ObjectName = "New Plane";	//Name of the New GameObject
	
	public Vector3 ObjectCenter;		//Center of the new Object
	public Vector3 ObjectRotation;		//Rotation of the new Object
	private Quaternion RotationQuat;	//Rotation Quaternion of the new Object
	
	public float ObjectHeight = 10.0f;		//Height of the new Objec
	public float ObjectWidth = 10.0f;		//Width of the new Object
	
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
	private Renderer newRenderer;			//renderer of the new Object
	
	public bool DestroyAfterConstruction = false; 
	public bool Deformation = false;
	
	public Material ObjectMaterial;	//Material of the Object
	
	
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
				
			//Debug.Log("Number of Vertices: "+ ((SectionHeight+1) * (SectionWidth+1)));
				
			//Debug.Log("Section Height: " + SectionHeight + " Section Width: " + SectionWidth);
			//Debug.Log("Mesh Height: " + MeshHeight + " Mesh Width: " + MeshWidth);
			#endregion
			
			if(Deformation)
			{
				
				NewObject.AddComponent<MeshDeformation>();

			}
			
			
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
		

		NewObject.AddComponent(typeof(MeshCollider));
		NewObject.AddComponent("MeshRenderer");
		

		newMesh = NewObject.AddComponent<MeshFilter>().mesh;
		newMeshCollider = NewObject.GetComponent(typeof(MeshCollider)) as MeshCollider; 
		newRenderer = NewObject.GetComponent<MeshRenderer>().renderer;
		
		newTransform = NewObject.GetComponent<Transform>();
		newTransform.position = ObjectCenter;
		
		RotationQuat = Quaternion.Euler(ObjectRotation);
		newTransform.rotation = RotationQuat;
		
		newRenderer.material = ObjectMaterial;
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
		
		
		#region Region Debug
		//Debug.DrawRay(newTransform.position, newMesh.normals[3],Color.blue,15.5f);
		#endregion
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
				
				newUVs[(i * (SectionWidth+1) + j)] = new Vector2(newVertices[(i * (SectionWidth+1) + j)].x,newVertices[(i * (SectionWidth+1) + j)].y);
		
				#region Region Debug
				//Debug.Log("Section Height: " + i + " Section Width: " + j);
				//Debug.Log("Number of Current Vertice: "+ (i * (SectionWidth+1) + j));
				//Debug.Log("Current Vertice: " + newVertices[(i * (SectionWidth+1) + j)]);
				//Debug.Log("Vertice[" + i + "][" + j + "]: " + newVertices[i][j]);
				#endregion
			}
		}
		
		CalculateTriangles();	//Funktion
		
	}
	
	void CalculateTriangles()
	{

		int length = ((SectionHeight) * (SectionWidth)) * 6;
		newTriangles = new int[length];	
		
		#region Region Debug
		//Debug.Log("Triangle length: " + length);
		//Debug.Log("Section Height: " + SectionHeight + " Section Width: " + SectionWidth);
		#endregion
		
		int j=0;
		int line = 0;
		//for(int i = 0; i < length-1; i++)
		for(int i = 0; i < ((SectionHeight+1) * (SectionWidth+1))-1; i++)
		{
			#region Region Debug
			//Debug.Log("I: " + i);
			//Debug.Log("Mod: " +((i+1) % (SectionWidth+1)));
			#endregion
			if(((i+1) % (SectionWidth+1)) != 0)
			{
				if(line > 0 )
				{
					if(line < SectionHeight )
					{
						newTriangles[j] = i;
						newTriangles[j+1] = i+1;
						newTriangles[j+2] = (SectionWidth + 1) + i;
						#region Region Debug						
						//Debug.Log("J: " + j + " Triangle j: " + newTriangles[j]);
						//Debug.Log("J: " + (j + 1) + " Triangle j: " + newTriangles[(j + 1)]);
						//Debug.Log("J: " + (j + 2) + " Triangle j: " + newTriangles[(j + 2)]);
						#endregion
						
						j+=3;
					}

					
					newTriangles[j] = i;								
					newTriangles[j+1] = i+1 - (SectionWidth + 1);		//hier 
					newTriangles[j+2] = i+1;
					
					#region Region Debug
					//Debug.Log("J: " + j + " Triangle j: " + newTriangles[j]);
					//Debug.Log("J: " + (j + 1) + " Triangle j: " + newTriangles[(j + 1)]);
					//Debug.Log("J: " + (j + 2) + " Triangle j: " + newTriangles[(j + 2)]);
					#endregion
					
					j+=3;				
					
				}
				else
				{
					
					newTriangles[j] = (SectionWidth + 1) + i;
					newTriangles[j+1] = i;
					newTriangles[j+2] = i+1;					
					
					#region Region Debug
					//Debug.Log("J: " + j + " Triangle j: " + newTriangles[j]);
					//Debug.Log("J: " + (j + 1) + " Triangle j: " + newTriangles[(j + 1)]);
					//Debug.Log("J: " + (j + 2) + " Triangle j: " + newTriangles[(j + 2)]);
					#endregion
					
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
	}
	

}
