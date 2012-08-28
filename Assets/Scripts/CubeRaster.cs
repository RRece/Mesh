using UnityEngine;
using System.Collections;

public class CubeRaster : MonoBehaviour 
{
	private GameObject NewObject;			//GameObject for the New Object
	public string ObjectName = "New grid Cube";	//Name of the New GameObject
	
	public Vector3 ObjectCenter;		//Center of the new Object
	public Vector3 ObjectRotation;		//Rotation of the new Object
	private Quaternion RotationQuat;	//Rotation Quaternion of the new Object
	
	public float Height = 10.0f;	//Height of the new Objec
	public float Width = 10.0f;		//Width of the new Object
	public float Depth = 10.0f;		//Depth of the new Object	
	
	private float ObjectHeight;		//Height of the new Objec
	private float ObjectWidth;		//Width of the new Object
	private float ObjectDepth;		//Depth of the new Object
	
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
	
	public enum Parts{one, two, three, six}; //1 = all sides the same Texture / 3 = opposite sides are the same Texture / 6 = 1 Texture for every side 
	public Parts TextureParts;
	private int TexturePartNumber;
	
	// Use this for initialization
	void Start () 
	{
		ObjectHeight = Width;		//Height and Widht have to change positions
		ObjectWidth = Height;		//because, i do not know
		ObjectDepth = Depth;		
		
		if(ObjectWidth > 0.0f && ObjectHeight > 0.0f && ObjectDepth > 0.0f)
		{
			ObjectHeight = Width;		//Height and Widht have to change positions
			ObjectWidth = Height;		//because, i do not know
			ObjectDepth = Depth;	
			
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
			case Parts.two:
				TexturePartNumber = 2;
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
		
		int j = 0;
		
		
		for(int i = 0; i < 8; i++)
		{
			startVertices[j] = new Vector3(HalfMeshHeight -(height * ObjectHeight), (HalfMeshWidth - (width * ObjectWidth)), HalfMeshDepth - (depth * ObjectDepth));
			
			switch(i)
			{
				case 0:					
					startVertices[14] = startVertices[j];
					startVertices[17] = startVertices[j];
					j++;
					break;
				case 1:					
					startVertices[15] = startVertices[j];
					startVertices[20] = startVertices[j];
					j++;
					break;					
				case 2:
					startVertices[j+2] = startVertices[j];
					startVertices[19] = startVertices[j];
					j++;
					break;					
				case 3:
					startVertices[j+2] = startVertices[i];
					startVertices[22] = startVertices[i];
					j+=3;
					break;
				case 4:
					startVertices[j+2] = startVertices[j];
					startVertices[18] = startVertices[j];
					j++;
					break;
				case 5:					
					startVertices[j+2] = startVertices[j];
					startVertices[23] = startVertices[j];
					j+=3;
					break;
				case 6:
					startVertices[j+2] = startVertices[j];
					startVertices[16] = startVertices[j];
					j++;
					break;
				case 7:
					startVertices[j+2] = startVertices[j];
					startVertices[21] = startVertices[j];
					break;
			}
			

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
			
			
			#region Region Debug
			//Debug.Log ("i: " + i + " Vertice: " + startVertices[i]);
			//Debug.Log ("width: " + width + " height: " + height + " depth: " + depth);
			#endregion Region Debug
		}
		
		switch(TexturePartNumber)
		{
			case 6:
					facemulti = sixOfOne;
				break;
			case 3:
					facemulti = treeOfOne;
				break;
			case 1:
				facemulti = 0;
				break;
		}
		
		face=-1;
		
		
		
		for(int i = 0; i < 24; i+=4)		
		{
			face++;
			
			switch(TexturePartNumber)
			{
				case 6:					
					startUVs[i  ] = new Vector2(face * facemulti, 0.0f);
					startUVs[i+1] = new Vector2(face * facemulti, 1.0f);
					startUVs[i+2] = new Vector2(facemulti * (face + 1), 0.0f);
					startUVs[i+3] = new Vector2(facemulti * (face + 1), 1.0f);
					break;
				case 3:
					
					switch(face)
					{
						case 0:							
						case 2:
							j = 0;
							break;							
						case 1:
						case 3:
							j = 1;
							break;
						case 4:							
						case 5:
							j = 2;
							break;
					}
										
					startUVs[i  ] = new Vector2(j * facemulti, 0.0f);
					startUVs[i+1] = new Vector2(j * facemulti, 1.0f);
					startUVs[i+2] = new Vector2(facemulti * (j + 1), 0.0f);
					startUVs[i+3] = new Vector2(facemulti * (j + 1), 1.0f);
					break;
				case 2:					
					switch(face)
					{
						case 0:							
						case 1:							
						case 2:
						case 3:
							j = 0;
							break;
						case 4:							
						case 5:
							j = 1;
							break;
					}
										
					startUVs[i  ] = new Vector2(j * facemulti, 0.0f);
					startUVs[i+1] = new Vector2(j * facemulti, 1.0f);
					startUVs[i+2] = new Vector2(facemulti * (j + 1), 0.0f);
					startUVs[i+3] = new Vector2(facemulti * (j + 1), 1.0f);
					break;
				case 1:
					startUVs[i  ] = new Vector2(0.0f, 0.0f);
					startUVs[i+1] = new Vector2(0.0f, 1.0f);
					startUVs[i+2] = new Vector2(1.0f, 0.0f);
					startUVs[i+3] = new Vector2(1.0f, 1.0f);
					break;
			}
			
			
			
			
			
			
			
			//((face * 2)+ (i % 4))
			//startUVs[i] = new Vector2((height/TexturePartNumber) , width);
			
			
			
			
			#region Region Debug
			//Debug.Log ("i: " + i + " UV: " + startUVs[i] +  " UV2: " + startUVs[i+1] + " UV2: " + startUVs[i+2] +  " UV3: " + startUVs[i+3]);
			#endregion Region Debug
		}
		
		CalculateTriangles();	//Funktion
		
	}

	
	void CalculateTriangles()
	{
		
		//startTriangles = new int[36]{1,0,2,1,2,3,3,2,4,3,4,5,5,4,6,5,6,7,7,6,0,7,0,1,0,6,2,6,4,2,7,1,3,7,3,5};
		startTriangles = new int[36];
		int j = 0,k = 0;
		
		for(int i = 0; i < 6; i++)
		{
			j = 4 * i;
			k = 6 * i;
			
			startTriangles[k] = j;
			startTriangles[k+1] = j+2;
			startTriangles[k+2] = j+1;
			
			startTriangles[k+3] = j+1;
			startTriangles[k+4] = j+2;
			startTriangles[k+5] = j+3;
			
		}
		
	}
}
