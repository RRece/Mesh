using UnityEngine;
using System.Collections;

public class CubeRaster : MonoBehaviour 
{
	private GameObject NewObject;			//GameObject for the New Object
	public string ObjectName = "New grid Cube";	//Name of the New GameObject
	
	public Vector3 ObjectCenter;		//Center of the new Object
	public Vector3 ObjectRotation;		//Rotation of the new Object
	private Quaternion RotationQuat;	//Rotation Quaternion of the new Object
	
	public float ObjectHeight = 10.0f;		//Height of the new Objec
	public float ObjectWidth = 10.0f;		//Width of the new Object
	public float ObjectDepth = 10.0f;		//Depth of the new Object
	
	public int SectionHeight = 2;	//min 1
	public int SectionWidth = 2;	//min 1
	public int SectionDepth = 2;	//min 1
	
	private float MeshHeight;	//Mesh section Height	
	private float MeshWidth;	//Mesh section Width
	private float MeshDepth;	//Mesh section Width
		
	private Vector3[] startVertices;	//Vertices for Mesh corner
	private Vector2[] startUVs;			//UVs for Mesh corner
	private int[] startTriangles;		//Triangles for Mesh corner
	
	private Vector3[] newVertices;		//Vertices for Mesh raster
	private Vector2[] newUVs;			//UVs for Mesh raster
	private int[] newTriangles;			//Triangles for Mesh raster
		
	private UnityEngine.Mesh newMesh;		//Mesh from GameObject
	private MeshCollider newMeshCollider;	//Mesh Collider	
	private Transform newTransform;			//Transform of the new Object
	private Renderer newRenderer;			//renderer of the new Object
	
	public bool DestroyAfterConstruction = false; 
	public bool Deformation = false;
	
	public Material ObjectMaterial;	//Material of the Object
	
	public enum Parts{one, three, six}; //1 = all sides the same Texture / 3 = opposite sides are the same Texture / 6 = 1 Texture for every side 
	public Parts TextureParts;
	private int TexturePartNumber;
	
	// Use this for initialization
	void Start () 
	{
		if(ObjectWidth > 0.0f && ObjectHeight > 0.0f && ObjectDepth > 0.0f)
		{
			CreateObject();	//Funktion			
			CreateStartMesh();//Funktion
			
			//CreateMesh();	//Funktion
			
			if(Deformation)
			{
				NewObject.AddComponent("MeshDeformation");
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
		
		if(SectionHeight < 1)
		{
			SectionHeight = 1;
		}
		if(SectionWidth < 1)
		{
			SectionWidth = 1;
		}
		if(SectionDepth < 1)
		{
			SectionDepth = 1;
		}
		
		switch(TextureParts)
		{
			case Parts.six:
				TexturePartNumber = 6;
				break;
			case Parts.three:
				TexturePartNumber = 3;
				break;
			case Parts.one:
			default:
				TexturePartNumber = 1;
				break;
		}
	
	}
	
	void CreateMesh()
	{
		//CreateMeshSections();	//Funktion		
		
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
	
	void CreateStartMesh()
	{
		CreateMeshSections();	//Funktion		
		
		newMesh.vertices = startVertices;
		newMesh.uv = startUVs;
		newMesh.triangles = startTriangles;
		
		newMesh.RecalculateBounds();
		newMesh.RecalculateNormals();
		
		newMeshCollider.sharedMesh = newMesh;	
		
		//Not good, but the mesh typ must change to work correct 
		newMeshCollider.smoothSphereCollisions = true;
		newMeshCollider.smoothSphereCollisions = false;
	}
	
	void CalculateMeshSectionSize()	
	{
		MeshHeight = ObjectHeight / SectionHeight;
		MeshWidth = ObjectWidth / SectionWidth;	
		MeshDepth = ObjectDepth / SectionDepth;	
	}
	
	void CreateMeshSections()
	{
		CalculateMeshSectionSize();
		
		float HalfMeshHeight = ObjectHeight / 2;
		float HalfMeshWidth = ObjectWidth / 2;
		float HalfMeshDepth = ObjectDepth / 2;
		
		int height = 0;
		int width = 0;
		int depth = 0;
		
		int face = -1;
		float facemulti = 0.0f;
		float sixOfOne = 1/6.0f;
		float treeOfOne = 1/3.0f;
		
		startVertices = new Vector3[24];
		startUVs = new Vector2[24];
		
		for(int i = 0; i < 8; i++)
		{
			if(i%4==0)
			{
				face++;
				switch(TexturePartNumber)
				{
					case 6:
						facemulti = face;
						break;
					case 3:
						if(face < 3)
						{
							facemulti = face;
						}
						else
						{
							facemulti = (face - 3);
						}
						break;
					case 1:
						facemulti = 0;
						break;
				}
			}
			
			startVertices[i] = new Vector3((HalfMeshHeight -(height * ObjectHeight) ),(HalfMeshWidth - (width * ObjectWidth)), HalfMeshDepth - (depth * ObjectDepth));
			startUVs[i] = new Vector2((height/TexturePartNumber) , width);
			
			
			//startVertices[i] = new Vector3((HalfMeshHeight -(height * MeshHeight) ),(HalfMeshWidth - (width * MeshWidth)), HalfMeshDepth - (depth * MeshDepth));
			//startUVs[i] = new Vector2((MeshHeight * height)/(SectionHeight * TexturePartNumber),(MeshWidth * width)/(SectionWidth));
			

			width++;
			if(width > 1)
			{
				width = 0;
				height++;
				
				if(height > 1)
				{
					height = 1;
					depth++;
					
					if(depth > 1)
					{
						depth = 1;
						height = 0;
					}
				}
				
			}
			
			
			
			Debug.Log ("i: " + i + " Vertice: " + startVertices[i] + " UV: " + startUVs[i]);
			Debug.Log ("width: " + width + " height: " + height + " depth: " + depth);
		}
		
		CalculateTriangles();	//Funktion
		
	}

	
	void CalculateTriangles()
	{
		
		startTriangles = new int[36]{1,0,2,1,2,3,3,2,4,3,4,5,5,4,6,5,6,7,7,6,0,7,0,1,0,6,2,6,4,2,7,1,3,7,3,5};

		
	}
}
