//Created by Max Merchel

using UnityEngine;
using System.Collections;

public class RasterCube : MonoBehaviour {

	private GameObject NewObject;			//GameObject for the New Object
	public string ObjectName = "New grid Cube";	//Name of the New GameObject
	
	public Vector3 ObjectCenter;		//Center of the new Object
	public Vector3 ObjectRotation;		//Rotation of the new Object
	private Quaternion RotationQuat;	//Rotation Quaternion of the new Object
	

	//Object size
	public float ObjectHeight = 10.0f;		//Height of the new Objec
	public float ObjectWidth = 10.0f;		//Width of the new Object
	public float ObjectDepth = 10.0f;		//Depth of the new Object
	
	//Half Object size
	private float HalfMeshHeight;	//half height
	private	float HalfMeshWidth;	//half width
	private	float HalfMeshDepth;	//half depth
	
	//Object sections
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
	
	public enum Parts{one, two, three, five, six}; //1 = all sides the same Texture / 3 = opposite sides are the same Texture / 6 = 1 Texture for every side 
	public Parts TextureParts;
	private int TexturePartNumber;
	
	private int[] faceAdd;
	
	// Use this for initialization
	void Start () 
	{	
		
		if(ObjectWidth > 0.0f && ObjectHeight > 0.0f && ObjectDepth > 0.0f)
		{
			
			HalfMeshHeight = ObjectHeight / 2;
			HalfMeshWidth = ObjectWidth / 2;
			HalfMeshDepth = ObjectDepth / 2;
			
			CreateObject();	//Funktion	
			CreateStartMeshSections();	//Funktion	
			
			if(SectionHeight == 1 && SectionWidth == 1 && SectionDepth == 1)
			{
				CreateStartMesh();//Funktion
			}
			else
			{
				CreateMesh();	//Funktion
			}
			
			
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
			case Parts.five:
				TexturePartNumber = 5;
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


	#region Region Start Mesh
	void CreateStartMesh()
	{
		//CreateStartMeshSections();	//Funktion		
		
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
		
	void CreateStartMeshSections()
	{
		//CalculateMeshSectionSize();
		
		int height = 0;
		int width = 0;
		int depth = 0;
		
		int face = -1;
		float facemulti = 0.0f;
		float sixOfOne = 1/6.0f;
		float fiveOfOne = 1/5.0f;
		float treeOfOne = 1/3.0f;
		float twoOfOne = 1/2.0f;
		
		startVertices = new Vector3[24];
		startUVs = new Vector2[24];
		
		int j = 0;
		
		
		for(int i = 0; i < 8; i++)
		{
			startVertices[j] = new Vector3(HalfMeshWidth - (width * ObjectWidth), HalfMeshHeight -(height * ObjectHeight), HalfMeshDepth - (depth * ObjectDepth));
			
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
					startVertices[4] = startVertices[j];
					startVertices[19] = startVertices[j];
					j++;
					break;					
				case 3:
					startVertices[5] = startVertices[i];
					startVertices[22] = startVertices[i];
					j+=3;
					break;
				case 4:
					startVertices[8] = startVertices[j];
					startVertices[18] = startVertices[j];
					j++;
					break;
				case 5:					
					startVertices[9] = startVertices[j];
					startVertices[23] = startVertices[j];
					j+=3;
					break;
				case 6:
					startVertices[12] = startVertices[j];
					startVertices[16] = startVertices[j];
					j++;
					break;
				case 7:
					startVertices[13] = startVertices[j];
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
			case 5:
				facemulti = fiveOfOne;
				break;
			case 3:
				facemulti = treeOfOne;
				break;
			case 2:
				facemulti = twoOfOne;
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
					switch(face)
					{
						case 0:							
						case 1:	
							startUVs[i  ] = new Vector2(face * facemulti, 0.0f);
							startUVs[i+1] = new Vector2(face * facemulti, 1.0f);
							startUVs[i+2] = new Vector2(facemulti * (face + 1), 0.0f);
							startUVs[i+3] = new Vector2(facemulti * (face + 1), 1.0f);
							break;
						case 2:							
						case 3:
							startUVs[i+2] = new Vector2(face * facemulti, 0.0f);
							startUVs[i+3] = new Vector2(face * facemulti, 1.0f);
							startUVs[i  ] = new Vector2(facemulti * (face + 1), 0.0f);
							startUVs[i+1] = new Vector2(facemulti * (face + 1), 1.0f);
							break;
						case 4:
							startUVs[i  ] = new Vector2(face * facemulti, 0.0f);
							startUVs[i+1] = new Vector2(face * facemulti, 1.0f);
							startUVs[i+2] = new Vector2(facemulti * (face + 1), 0.0f);
							startUVs[i+3] = new Vector2(facemulti * (face + 1), 1.0f);
							break;
						case 5:
							startUVs[i+1] = new Vector2(face * facemulti, 0.0f);
							startUVs[i  ] = new Vector2(face * facemulti, 1.0f);
							startUVs[i+3] = new Vector2(facemulti * (face + 1), 0.0f);
							startUVs[i+2] = new Vector2(facemulti * (face + 1), 1.0f);
							break;
					}
					break;
				case 5:						
					switch(face)
					{
						case 0:							
						case 1:	
							startUVs[i  ] = new Vector2(face * facemulti, 0.0f);
							startUVs[i+1] = new Vector2(face * facemulti, 1.0f);
							startUVs[i+2] = new Vector2(facemulti * (face + 1), 0.0f);
							startUVs[i+3] = new Vector2(facemulti * (face + 1), 1.0f);
							break;
						case 2:							
						case 3:
							startUVs[i+2] = new Vector2(face * facemulti, 0.0f);
							startUVs[i+3] = new Vector2(face * facemulti, 1.0f);
							startUVs[i  ] = new Vector2(facemulti * (face + 1), 0.0f);
							startUVs[i+1] = new Vector2(facemulti * (face + 1), 1.0f);
							break;
						case 4:
							startUVs[i  ] = new Vector2(face * facemulti, 0.0f);
							startUVs[i+1] = new Vector2(face * facemulti, 1.0f);
							startUVs[i+2] = new Vector2(facemulti * (face + 1), 0.0f);
							startUVs[i+3] = new Vector2(facemulti * (face + 1), 1.0f);
							break;
						case 5:
							startUVs[i+1] = new Vector2((face-1) * facemulti, 0.0f);
							startUVs[i  ] = new Vector2((face-1) * facemulti, 1.0f);
							startUVs[i+3] = new Vector2(facemulti * (face), 0.0f);
							startUVs[i+2] = new Vector2(facemulti * (face), 1.0f);
							break;
					}
					break;
				case 3:
					switch(face)
					{
						case 0:
							j = 0;
							startUVs[i  ] = new Vector2(j * facemulti, 0.0f);
							startUVs[i+1] = new Vector2(j * facemulti, 1.0f);
							startUVs[i+2] = new Vector2(facemulti * (j + 1), 0.0f);
							startUVs[i+3] = new Vector2(facemulti * (j + 1), 1.0f);
							break;
						case 1:							
							j = 1;
							startUVs[i  ] = new Vector2(j * facemulti, 0.0f);
							startUVs[i+1] = new Vector2(j * facemulti, 1.0f);
							startUVs[i+2] = new Vector2(facemulti * (j + 1), 0.0f);
							startUVs[i+3] = new Vector2(facemulti * (j + 1), 1.0f);
							break;
						case 2:	
							j = 0;
							startUVs[i+2] = new Vector2(j * facemulti, 0.0f);
							startUVs[i+3] = new Vector2(j * facemulti, 1.0f);
							startUVs[i  ] = new Vector2(facemulti * (j + 1), 0.0f);
							startUVs[i+1] = new Vector2(facemulti * (j + 1), 1.0f);
							break;
						case 3:
							j = 1;
							startUVs[i+2] = new Vector2(j * facemulti, 0.0f);
							startUVs[i+3] = new Vector2(j * facemulti, 1.0f);
							startUVs[i  ] = new Vector2(facemulti * (j + 1), 0.0f);
							startUVs[i+1] = new Vector2(facemulti * (j + 1), 1.0f);
							break;
						case 4:
							j = 2;
							startUVs[i  ] = new Vector2(j * facemulti, 0.0f);
							startUVs[i+1] = new Vector2(j * facemulti, 1.0f);
							startUVs[i+2] = new Vector2(facemulti * (j + 1), 0.0f);
							startUVs[i+3] = new Vector2(facemulti * (j + 1), 1.0f);
							break;
						case 5:
							j = 2;
							startUVs[i+1] = new Vector2(j * facemulti, 0.0f);
							startUVs[i  ] = new Vector2(j * facemulti, 1.0f);
							startUVs[i+3] = new Vector2(facemulti * (j + 1), 0.0f);
							startUVs[i+2] = new Vector2(facemulti * (j + 1), 1.0f);
							break;
					}
					break;
				case 2:									
					switch(face)
					{
						case 0:							
						case 1:							
							j = 0;
							startUVs[i  ] = new Vector2(j * facemulti, 0.0f);
							startUVs[i+1] = new Vector2(j * facemulti, 1.0f);
							startUVs[i+2] = new Vector2(facemulti * (j + 1), 0.0f);
							startUVs[i+3] = new Vector2(facemulti * (j + 1), 1.0f);
							break;
						case 2:							
						case 3:
							j = 0;
							startUVs[i+2] = new Vector2(j * facemulti, 0.0f);
							startUVs[i+3] = new Vector2(j * facemulti, 1.0f);
							startUVs[i  ] = new Vector2(facemulti * (j + 1), 0.0f);
							startUVs[i+1] = new Vector2(facemulti * (j + 1), 1.0f);
							break;
						case 4:
							j = 1;
							startUVs[i  ] = new Vector2(j * facemulti, 0.0f);
							startUVs[i+1] = new Vector2(j * facemulti, 1.0f);
							startUVs[i+2] = new Vector2(facemulti * (j + 1), 0.0f);
							startUVs[i+3] = new Vector2(facemulti * (j + 1), 1.0f);
							break;
						case 5:
							j = 1;
							startUVs[i+1] = new Vector2(j * facemulti, 0.0f);
							startUVs[i  ] = new Vector2(j * facemulti, 1.0f);
							startUVs[i+3] = new Vector2(facemulti * (j + 1), 0.0f);
							startUVs[i+2] = new Vector2(facemulti * (j + 1), 1.0f);
							break;
					}
					
					break;
				case 1:
					switch(face)
					{
						case 0:							
						case 1:	
							startUVs[i  ] = new Vector2(0.0f, 0.0f);
							startUVs[i+1] = new Vector2(0.0f, 1.0f);
							startUVs[i+2] = new Vector2(1.0f, 0.0f);
							startUVs[i+3] = new Vector2(1.0f, 1.0f);
							break;
						case 2:							
						case 3:
							startUVs[i+2] = new Vector2(0.0f, 0.0f);
							startUVs[i+3] = new Vector2(0.0f, 1.0f);
							startUVs[i  ] = new Vector2(1.0f, 0.0f);
							startUVs[i+1] = new Vector2(1.0f, 1.0f);
							break;
						case 4:
							startUVs[i  ] = new Vector2(0.0f, 0.0f);
							startUVs[i+1] = new Vector2(0.0f, 1.0f);
							startUVs[i+2] = new Vector2(1.0f, 0.0f);
							startUVs[i+3] = new Vector2(1.0f, 1.0f);
							break;
						case 5:
							startUVs[i+1] = new Vector2(0.0f, 0.0f);
							startUVs[i  ] = new Vector2(0.0f, 1.0f);
							startUVs[i+3] = new Vector2(1.0f, 0.0f);
							startUVs[i+2] = new Vector2(1.0f, 1.0f);
							break;
					}
					
					break;
			}
			

			//((face * 2)+ (i % 4))
			//startUVs[i] = new Vector2((height/TexturePartNumber) , width);

			#region Region Debug
			//Debug.Log ("i: " + i + " UV: " + startUVs[i] +  " UV2: " + startUVs[i+1] + " UV2: " + startUVs[i+2] +  " UV3: " + startUVs[i+3]);
			#endregion Region Debug
		}
		
		CalculateStartTriangles();	//Funktion
		
	}

	
	void CalculateStartTriangles()
	{
		
		startTriangles = new int[36];
		int j = 0,k = 0;
		
		for(int i = 0; i < 6; i++)
		{
			j = 4 * i;
			k = 6 * i;
			
			startTriangles[k] = j;
			startTriangles[k+1] = j+1;
			startTriangles[k+2] = j+2;
			
			startTriangles[k+3] = j+2;
			startTriangles[k+4] = j+1;
			startTriangles[k+5] = j+3;	
		}
		
	}
	
	#endregion Region Start Mesh
	
	
	#region Region Mesh	
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
	
		
	
	void CalculateMeshSectionSize()	
	{
		MeshHeight = ObjectHeight / SectionHeight;
		MeshWidth = ObjectWidth / SectionWidth;	
		MeshDepth = ObjectDepth / SectionDepth;	
	}
	
	void CreateMeshSections()
	{
		CalculateMeshSectionSize();
		
		int height = 0;
		int width = 0;
		int depth = 0;
		
		int face = 0;
		float facemulti = 0.0f;
		float sixOfOne = 1/6.0f;
		float fiveOfOne = 1/5.0f;
		float treeOfOne = 1/3.0f;
		float twoOfOne = 1/2.0f;
		
		faceAdd = new int[6];
		faceAdd[0] = (SectionWidth + 1) * (SectionHeight + 1);
		faceAdd[1] = faceAdd[0] + (SectionWidth + 1) * (SectionDepth + 1);
		faceAdd[2] = faceAdd[1] + faceAdd[0];
		faceAdd[3] = faceAdd[2] + (SectionWidth + 1) * (SectionDepth + 1);
		faceAdd[4] = faceAdd[3] + (SectionHeight + 1) * (SectionDepth + 1);
		faceAdd[5] = faceAdd[4] + (SectionHeight + 1) * (SectionDepth + 1);
		
		int length = ((SectionHeight + 1) * (SectionWidth + 1 + SectionDepth + 1) + (SectionWidth + 1) * (SectionDepth + 1)) *2;
		
		newVertices = new Vector3[length];
		newUVs = new Vector2[length];
		
		int j = 0;
		
		
		for(int i = 0; i < length; i++)
		{
			
			newVertices[i] = new Vector3(HalfMeshWidth -  (width * MeshWidth), HalfMeshHeight-(height * MeshHeight),HalfMeshDepth - (depth * MeshDepth));
						
			switch(face)
			{
				case 0:	
					if(width == SectionWidth)
					{		
						height++;
						width = -1;
						
						if(height > SectionHeight)
						{
							face++;
							height = SectionHeight;
						}
					
					}
					width++;
					
					break;
				case 1:	
					if(width == SectionWidth)
					{		
						depth++;
						width = -1;
						
						if(depth > SectionDepth)
						{
							face++;
							depth = SectionDepth;
						}
					
					}
					width++;
					
					break;					
				case 2:
					if(width == SectionWidth)
					{		
						height--;
						width = -1;
						
						if(height < 0)
						{
							face++;
							height = 0;
						}
					
					}
					width++;
					
					break;					
				case 3:
					if(width == SectionWidth)
					{		
						depth--;
						width = -1;
						
						if(depth < 0)
						{
							face++;
							depth = SectionDepth;
						}
					
					}
					width++;
					break;
				case 4:
					if(depth == 0)
					{
						depth = SectionDepth+1;
						height++;				
						
						if(height > SectionHeight)
						{
							height = 0;
							depth = 1;
							width = SectionWidth;
							face++;						
						}						
					}
					depth--;
					
					break;
				case 5:		
					
					#region Region Debug
					//Debug.Log ("i: " + i + " Vertice: " + newVertices[i]);
					//Debug.Log ("width: " + width + " height: " + height + " depth: " + depth);
					#endregion Region Debug
					
					if(depth == SectionDepth)
					{
						depth = -1;
						height++;				
						
						if(height > SectionHeight)
						{
							width = 0;
							depth = -1;
							height = SectionHeight;
							face++;					
						}						
					}
					depth++;
					break;
			}
			

			
			
			#region Region Debug
			//Debug.Log ("i: " + i + " Vertice: " + newVertices[i]);
			//Debug.Log ("width: " + width + " height: " + height + " depth: " + depth);
			#endregion Region Debug
		}
		
		switch(TexturePartNumber)
		{
			case 6:
				facemulti = sixOfOne;
				break;
			case 5:
				facemulti = fiveOfOne;
				break;
			case 3:
				facemulti = treeOfOne;
				break;
			case 2:
				facemulti = twoOfOne;
				break;
			case 1:
				facemulti = 0;
				break;
		}
		
		face = 0;
		
		width = 0;
		depth = 0;
		height = 0;
		
		#region Region Debug
		//Debug.Log ("length: " + length);
		//Debug.Log ("width: " + width + " height: " + height + " depth: " + depth);
		#endregion Region Debug
		
		for(int i = 0; i < length; i++)
		{
						
			switch(TexturePartNumber)
			{
				case 6:					
					switch(face)
					{
						case 0:	
						case 2:
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + face * facemulti, (float)height/SectionHeight);
							break;
						case 1:	
						case 3:
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + face * facemulti, (float)depth/SectionDepth);							
							break;
						case 4:
						case 5:
							newUVs[i] = new Vector2((depth * facemulti)/SectionDepth + face * facemulti, (float)height/SectionHeight);								
							break;
					}

					break;
				case 5:					
					switch(face)
					{
						case 0:	
						case 2:
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + face * facemulti, (float)height/SectionHeight);
							break;
						case 1:	
						case 3:
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + face * facemulti, (float)depth/SectionDepth);							
							break;
						case 4:
							newUVs[i] = new Vector2((depth * facemulti)/SectionDepth + face * facemulti, (float)height/SectionHeight);	
							break;
						case 5:
							newUVs[i] = new Vector2((depth * facemulti)/SectionDepth + (face-1) * facemulti, (float)height/SectionHeight);							
							break;
					}

					break;
				case 3:
					switch(face)
					{
						case 0:	
						case 2:
							j = 0;
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + j * facemulti, (float)height/SectionHeight);
							break;
						case 1:	
						case 3:
							j = 1;
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + j * facemulti, (float)depth/SectionDepth);
							break;
						case 4:
						case 5:
							j = 2;
							newUVs[i] = new Vector2((depth * facemulti)/SectionDepth + j * facemulti, (float)height/SectionHeight);
							break;
					}

					break;
				case 2:					
					switch(face)
					{
						case 0:	
						case 2:
							j = 0;
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + j * facemulti, (float)height/SectionHeight);
							break;
						case 1:	
						case 3:
							j = 0;
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + j * facemulti, (float)depth/SectionDepth);
							break;
						case 4:
						case 5:
							j = 1;
							newUVs[i] = new Vector2((depth * facemulti)/SectionDepth + j * facemulti, (float)height/SectionHeight);
							break;
					}

					break;
				case 1:
					switch(face)
					{
						case 0:	
						case 2:
							newUVs[i] = new Vector2((float)width/SectionWidth, (float)height/SectionHeight);
							break;
						case 1:	
						case 3:
							newUVs[i] = new Vector2((float)width/SectionWidth, (float)depth/SectionDepth);
							break;
						case 4:
						case 5:
							newUVs[i] = new Vector2((float)depth/SectionDepth, (float)height/SectionHeight);
							break;
					}					
					
					break;
			}
			
			switch(face)
			{
				case 0:	
					width++;
					if(width > SectionWidth)
					{
						width = 0;
						height++;
						
						if(height > SectionHeight)
						{
							face++;
							height = SectionHeight;
						}
						
					}
					break;
				case 1:	
					width++;
					if(width > SectionWidth)
					{
						width = 0;
						depth++;
						
						if(depth > SectionDepth)
						{
							face++;
							depth = SectionDepth;
						}
						
					}
					break;
				case 2:
					width++;
					if(width > SectionWidth)
					{
						width = 0;
						height--;
						
						if(height < 0)
						{
							face++;
							height = 0;
						}
						
					}
					break;
				case 3:
					width++;
					if(width > SectionWidth)
					{
						width = 0;
						depth--;
						
						if(depth < 0)
						{
							face++;
							depth = SectionDepth;
						}
						
					}
					break;
				case 4:	
					depth--;
					
					if(depth < 0)
					{
						depth = SectionDepth;						
						height++;
						
						if(height > SectionHeight)
						{
							face++;
							height = 0;
							depth = 0;
						}
					}
					break;
				case 5:
					depth++;
					
					if(depth > SectionDepth)
					{
						depth = 0;						
						height++;
						
						if(height > SectionHeight)
						{
							face++;
							height = 0;
						}
		
					}
					break;
			}
			

			#region Region Debug
			//Debug.Log ("i: " + i + " UV: " + newUVs[i].x + " , " + newUVs[i].y *1.0f);
			//Debug.Log ("i: " + i + " UV: " + newUVs[i]);
			#endregion Region Debug
		}
		
		CalculateTriangles();	//Funktion
		
	}

	
	void CalculateTriangles()
	{
		int length = ((SectionHeight * SectionWidth)+(SectionWidth * SectionDepth)+(SectionDepth*SectionHeight))*12;
		
		int vertexLength = ((SectionHeight+1)*(SectionWidth+1+SectionDepth+1)+(SectionWidth+1)*(SectionDepth+1))*2;
		
		newTriangles = new int[length];
		int j = 0;
		//int k = 0;
		int face = 0;

		#region Region Debug
		//Debug.Log ("length: " + length + " vertexLength: " + vertexLength);				
		//Debug.Log("Face0: " + faceAdd[0] + " Face1: " + faceAdd[1] + " Face2: " + faceAdd[2] + " Face2: " + faceAdd[3]);
		//Debug.Log("Face4: " + faceAdd[4] + " Face5: " + faceAdd[5]);
		#endregion
		
		
		for(int i = 0; i < vertexLength; i++)
		{
			switch(face)
			{
				case 0:		
					if(i <= SectionWidth)
					{
						break;
					}
					
					if(((i + 1) % (SectionWidth + 1)) != 0)
					{
						newTriangles[j] = i - SectionWidth;
						newTriangles[j+2] = i - (SectionWidth + 1);
						newTriangles[j+1] = i;
						
						newTriangles[j+4] = i;
						newTriangles[j+3] = i + 1;
						newTriangles[j+5] = newTriangles[j];
						
						
						
						#region Region Debug
						//Debug.Log ("J: " + j + " I: " + i + " Face: " + face);				
						//Debug.Log("Triangle: " + newTriangles[j] + " ; " + newTriangles[(j + 1)] + " ; " + newTriangles[(j + 2)]);
						//Debug.Log("Triangle: " + newTriangles[(j + 3)] + " ; " + newTriangles[(j + 4)] + " ; " + newTriangles[(j + 5)]);
						
						//k++;
						//Debug.Log ("K: " + k);
						#endregion
						
						j += 6;
					}
					else
					{
						if(i==(faceAdd[face]-1))
						{	
							#region Region Debug
							//Debug.Log ("J: " + j + " I: " + i + " Face: " + face);
							#endregion
							
							face++;			
						}
						 
					}
					
					break;
				case 1:
				case 2:
				case 3:				
					if(i < (faceAdd[face-1] + SectionWidth))
					{
						break;
					}
					
					if(((i + 1) % (SectionWidth + 1)) != 0)
					{
						newTriangles[j] = i - SectionWidth;
						newTriangles[j+2] = i - (SectionWidth + 1);
						newTriangles[j+1] = i;
						
						newTriangles[j+4] = i;
						newTriangles[j+3] = i + 1;
						newTriangles[j+5] = newTriangles[j];
						
		
						#region Region Debug
						//Debug.Log ("J: " + j + " I: " + i + " Face: " + face);						
						//Debug.Log("Triangle: " + newTriangles[j] + " ; " + newTriangles[(j + 1)] + " ; " + newTriangles[(j + 2)]);
						//Debug.Log("Triangle: " + newTriangles[(j + 3)] + " ; " + newTriangles[(j + 4)] + " ; " + newTriangles[(j + 5)]);
						
						//k++;
						//Debug.Log ("K: " + k);
						#endregion
						
						j += 6;
					}
					else
					{
						
						
						if(i==(faceAdd[face]-1))
						{
							#region Region Debug
							//Debug.Log ("J: " + j + " I: " + i + " Face: " + face);
							#endregion
							
							face++;															
						}
						 
					}
					
					break;
				case 4:
				case 5:
					if(i < faceAdd[face-1] + SectionDepth)
					{
						break;
					}
					
					if((((i - faceAdd[face-1]) + 1) % (SectionDepth + 1)) != 0)
					{
						newTriangles[j] = i - SectionDepth;
						newTriangles[j+2] = i - (SectionDepth + 1);
						newTriangles[j+1] = i;
						
						newTriangles[j+4] = i;
						newTriangles[j+3] = i + 1;
						newTriangles[j+5] = newTriangles[j];
						
						#region Region Debug
						//Debug.Log ("J: " + j + " I: " + i + " Face: " + face);						
						//Debug.Log("Triangle: " + newTriangles[j] + " ; " + newTriangles[(j + 1)] + " ; " + newTriangles[(j + 2)]);
						//Debug.Log("Triangle: " + newTriangles[(j + 3)] + " ; " + newTriangles[(j + 4)] + " ; " + newTriangles[(j + 5)]);
						
						//k++;
						//Debug.Log ("K: " + k);
						#endregion
						
						j += 6;
					}
					else
					{		
						#region Region Debug
						//Debug.Log ("I: " + i + " Face: " + face);
						#endregion
						
						if(i==(faceAdd[face]-1))
						{
							#region Region Debug
							//Debug.Log ("J: " + j + " I: " + i + " Face: " + face);
							#endregion
							face++;															
						}
						 
					}
					
					break;
			}
	
		}
		
	}
	
	#endregion Region Mesh
}
