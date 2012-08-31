using UnityEngine;
using System.Collections;

public class CubeRaster : MonoBehaviour 
{
	private GameObject NewObject;			//GameObject for the New Object
	public string ObjectName = "New grid Cube";	//Name of the New GameObject
	
	public Vector3 ObjectCenter;		//Center of the new Object
	public Vector3 ObjectRotation;		//Rotation of the new Object
	private Quaternion RotationQuat;	//Rotation Quaternion of the new Object
	
	//Object size
	public float Height = 10.0f;	//Height of the new Objec
	public float Width = 10.0f;		//Width of the new Object
	public float Depth = 10.0f;		//Depth of the new Object	
	
	private float ObjectHeight;		//Height of the new Objec
	private float ObjectWidth;		//Width of the new Object
	private float ObjectDepth;		//Depth of the new Object
	
	//Half Object size
	private float HalfMeshHeight;	//half height
	private	float HalfMeshWidth;	//half width
	private	float HalfMeshDepth;	//half depth
	
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
		float treeOfOne = 1/3.0f;
		float twoOfOne = 1/2.0f;
		
		startVertices = new Vector3[24];
		startUVs = new Vector2[24];
		
		int j = 0;
		
		
		for(int i = 0; i < 8; i++)
		{
			startVertices[j] = new Vector3(HalfMeshHeight -(height * ObjectHeight), HalfMeshWidth - (width * ObjectWidth), HalfMeshDepth - (depth * ObjectDepth));
			
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
			startTriangles[k+1] = j+2;
			startTriangles[k+2] = j+1;
			
			startTriangles[k+3] = j+1;
			startTriangles[k+4] = j+2;
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
		float treeOfOne = 1/3.0f;
		float twoOfOne = 1/2.0f;
		
		int face0 = (SectionHeight + 1) * (SectionWidth + 1);
		int face1 = face0 + (SectionHeight + 1) * (SectionDepth + 1);
		int face2 = face1 + face0;
		int face3 = face2 + (SectionHeight + 1) * (SectionDepth + 1);
		int face4 = face3 + (SectionWidth + 1) * (SectionDepth + 1);
		int face5 = face4 + (SectionWidth + 1) * (SectionDepth + 1);
		
		
		int length = ((SectionHeight + 1) * (SectionWidth + 1 + SectionDepth + 1) + (SectionWidth + 1) * (SectionDepth + 1)) *2;
		
		newVertices = new Vector3[length];
		newUVs = new Vector2[length];
		
		int j = 0;
		
		
		for(int i = 0; i < length; i++)
		{
			
			switch(face)
			{
				case 0:		
//					if(i == 0)
//					{					
//						newVertices[i] = startVertices[0];
//					}
//					else if(i == SectionHeight)
//					{
//						newVertices[i] = startVertices[1];
//					}
//					else if(i == (face0 - SectionHeight))
//					{
//						newVertices[i] = startVertices[2];
//					}
//					else if(i == (face0 - 1))
//					{
//						newVertices[i] = startVertices[3];
//					}
//					else
//					{						
//						newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
//					}
					newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
					
					
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
//					if(i < (face0 + SectionHeight))
//					{					
//						if(i == face0)
//						{
//							newVertices[i] = startVertices[4];
//						} 
//						else if(i == (face0 + SectionHeight))
//						{
//							newVertices[i] = startVertices[5];
//						}
//						else 
//						{
//							newVertices[i] = newVertices[i-(SectionHeight + 1)];
//						}
//					}					
//					else if(i == (face1 - (SectionHeight + 1)))
//					{
//						newVertices[i] = startVertices[6];
//					}
//					else if(i == (face1 - 1))
//					{
//						newVertices[i] = startVertices[7];
//					}
//					else
//					{
//						newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
//					}
										newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
					
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
					
//					if(i < (face1 + SectionHeight))
//					{					
//						if(i == face1)
//						{
//							newVertices[i] = startVertices[8];
//						} 
//						else if(i == (face1 + SectionHeight))
//						{
//							newVertices[i] = startVertices[9];
//						}
//						else
//						{
//							newVertices[i] = newVertices[i-(SectionHeight + 1)];
//						}
//					}
//					else if(i == (face2 - (SectionHeight + 1)))
//					{
//						newVertices[i] = startVertices[10];
//					}
//					else if(i == (face2 - 1))
//					{
//						newVertices[i] = startVertices[11];
//					}
//					else
//					{
//						newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
//					}
					newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
					
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
//					if(i < (face2 + SectionHeight))
//					{					
//						if(i == face2)
//						{
//							newVertices[i] = startVertices[12];
//						} 
//						else if(i == (face2 + SectionHeight))
//						{
//							newVertices[i] = startVertices[13];
//						}
//						else 
//						{
//							newVertices[i] = newVertices[i-(SectionHeight + 1)];
//						}
//					}					
//					else if(i >= ((face3) - (SectionHeight + 1)))
//					{
//						if(i == ((face3) - (SectionHeight + 1)))
//						{
//						newVertices[i] = startVertices[14];
//						}
//						else if(i == ((face3) - 1))
//						{
//							newVertices[i] = startVertices[15];
//						}
//						else
//						{
//							newVertices[i] = newVertices[i - ((face3) - (SectionHeight+1))];
//						}
//					}
//					else
//					{
//						newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
//					}
					
					newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
					
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
					
//					if(i <= (face3 + SectionDepth))
//					{					
//						if(i == face3)
//						{
//							newVertices[i] = startVertices[16];
//						} 
//						else if(i == (face3 + SectionDepth))
//						{
//							newVertices[i] = startVertices[17];
//						}
//						else 
//						{
//							newVertices[i] = newVertices[(face2 + depth * (SectionWidth +1))];
//						}
//					}
//					else if(i <= ((face4) - (SectionDepth + 1)))
//					{
//						if(i == ((face4) - (SectionDepth + 1)))
//						{
//						newVertices[i] = startVertices[19];
//						}
//						else if(i == ((face4) - 1))
//						{
//							newVertices[i] = startVertices[19];
//						}
//						else
//						{
//							newVertices[i] = newVertices[(face0 + depth * (SectionWidth +1))];
//						}
//					}
//					else if(depth == SectionDepth)
//					{
//						newVertices[i] = newVertices[(face1 + width * (SectionWidth +1))];
//					}
//					else if(depth == 0)
//					{
//						newVertices[i] = newVertices[(width * (SectionWidth +1))];
//					}
//					else
//					{
//						newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
//					}
					newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
					
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
//					if(i <= (face4 + SectionDepth))
//					{					
//						if(i == face4)
//						{
//							newVertices[i] = startVertices[20];
//						} 
//						else if(i == (face4 + SectionDepth))
//						{
//							newVertices[i] = startVertices[21];
//						}
//						else 
//						{
//							newVertices[i] = newVertices[(face2 + SectionWidth + depth * (SectionWidth +1))];
//						}
//					}
//					else if(i <= ((face5) - (SectionDepth + 1)))
//					{
//						if(i == ((face5) - (SectionDepth + 1)))
//						{
//						newVertices[i] = startVertices[22];
//						}
//						else if(i == ((face5) - 1))
//						{
//							newVertices[i] = startVertices[23];
//						}
//						else
//						{
//							newVertices[i] = newVertices[(face0 + SectionWidth + depth * (SectionWidth +1))];
//						}
//					}
//					else if(depth == SectionDepth)
//					{
//						newVertices[i] = newVertices[(face1 + SectionWidth + width * (SectionWidth +1))];
//					}
//					else if(depth == 0)
//					{
//						newVertices[i] = newVertices[SectionWidth + (width * (SectionWidth +1))];
//					}
//					else
//					{
//						newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
//					}
					
					newVertices[i] = new Vector3(HalfMeshHeight-(height * MeshHeight),HalfMeshWidth -  (width * MeshWidth), HalfMeshDepth - (depth * MeshDepth));
					
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
			Debug.Log ("i: " + i + " Vertice: " + newVertices[i]);
			Debug.Log ("width: " + width + " height: " + height + " depth: " + depth);
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
		
		for(int i = 0; i < length; i++)		
		{
						
			switch(TexturePartNumber)
			{
				case 6:					
					switch(face)
					{
						case 0:	
						case 2:
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + face * facemulti, height/SectionHeight);
							break;
						case 1:	
						case 3:
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + face * facemulti, depth/SectionDepth);
							break;
						case 4:
						case 5:
							newUVs[i] = new Vector2((depth * facemulti)/SectionDepth + face * facemulti, width/SectionWidth);
							break;
					}

					break;
				case 3:
					switch(face)
					{
						case 0:	
						case 2:
							j = 0;
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + j * facemulti, height/SectionHeight);
							break;
						case 1:	
						case 3:
							j = 1;
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + j * facemulti, depth/SectionDepth);
							break;
						case 4:
						case 5:
							j = 2;
							newUVs[i] = new Vector2((depth * facemulti)/SectionDepth + j * facemulti, width/SectionWidth);
							break;
					}

					break;
				case 2:					
					switch(face)
					{
						case 0:	
						case 2:
							j = 0;
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + j * facemulti, height/SectionHeight);
							break;
						case 1:	
						case 3:
							j = 0;
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + j * facemulti, depth/SectionDepth);
							break;
						case 4:
						case 5:
							j = 1;
							newUVs[i] = new Vector2((depth * facemulti)/SectionDepth + j * facemulti, width/SectionWidth);
							break;
					}

					break;
				case 1:
					switch(face)
					{
						case 0:	
						case 2:
							newUVs[i] = new Vector2(width/SectionWidth, height/SectionHeight);
							break;
						case 1:	
						case 3:
							newUVs[i] = new Vector2(width/SectionWidth, depth/SectionDepth);
							break;
						case 4:
						case 5:
							newUVs[i] = new Vector2(depth/SectionDepth, width/SectionWidth);
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
						width = SectionWidth;
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
						width++;
						
						if(width > SectionWidth)
						{
							face++;
							width = 0;
							depth = 0;
						}
						
					}
					break;
				case 5:
					depth++;
					
					if(depth > SectionDepth)
					{
						depth = 0;
						width++;
						
						if(width > SectionWidth)
						{
							face++;
							width = 0;
						}
						
					}
					break;
			}
			
			
			
			
			#region Region Debug
			//Debug.Log ("i: " + i + " UV: " + startUVs[i] +  " UV2: " + startUVs[i+1] + " UV2: " + startUVs[i+2] +  " UV3: " + startUVs[i+3]);
			#endregion Region Debug
		}
		
		CalculateTriangles();	//Funktion
		
	}

	
	void CalculateTriangles()
	{
		int length=((SectionHeight * SectionWidth)+(SectionWidth * SectionDepth)+(SectionDepth*SectionHeight))*12;
		int vertexLength = ((SectionHeight+1)*(SectionWidth+1+SectionDepth+1)+(SectionWidth+1)*(SectionDepth+1))*2;
		newTriangles = new int[length];
		int j = 0;
		//int k = 0;
		int face = 0;
		
		int[] faceAdd;
		faceAdd = new int[6];
		faceAdd[0] = (SectionHeight + 1) * (SectionWidth + 1);
		faceAdd[1] = faceAdd[0] + (SectionWidth + 1) * (SectionDepth + 1);
		faceAdd[2] = faceAdd[1] + faceAdd[0];
		faceAdd[3] = faceAdd[2] + (SectionWidth + 1) * (SectionDepth + 1);
		faceAdd[4] = faceAdd[3] + (SectionHeight + 1) * (SectionDepth + 1);
		faceAdd[5] = faceAdd[4] + (SectionHeight + 1) * (SectionDepth + 1);
		
		
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
						newTriangles[j  ] = i - (SectionWidth + 1);
						newTriangles[j+1] = i;
						newTriangles[j+2] = i - SectionWidth;
						
						newTriangles[j+3] = newTriangles[j+2];
						newTriangles[j+4] = i;
						newTriangles[j+5] = i + 1;
						
						
						
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
						newTriangles[j  ] = i - (SectionWidth + 1);
						newTriangles[j+1] = i;
						newTriangles[j+2] = i - SectionWidth;
						
						newTriangles[j+3] = newTriangles[j+2];
						newTriangles[j+4] = i;
						newTriangles[j+5] = i + 1;
						
						
						
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
						//Debug.Log ("Else i: " + i + " FaceAdd-1: " + (faceAdd[face]-1) + " FaceAdd: " + faceAdd[face]);
						#endregion
						
						if(i==(faceAdd[face]-1))
						{
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
					
					if(((i + 1) % (SectionDepth + 1)) != 0)
					{
						newTriangles[j  ] = i - (SectionDepth + 1);
						newTriangles[j+1] = i;
						newTriangles[j+2] = i - SectionDepth;
						
						newTriangles[j+3] = newTriangles[j+2];
						newTriangles[j+4] = i;
						newTriangles[j+5] = i + 1;
						
						
						
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
						//Debug.Log ("Else i: " + i + " FaceAdd-1: " + (faceAdd[face]-1) + " FaceAdd: " + faceAdd[face]);
						#endregion
			
						if(i==(faceAdd[face]-1))
						{
							face++;															
						}
						 
					}
					
					break;
			}
	
		}
		
	}
	
	#endregion Region Mesh
	
	
	
	
	
}



/*
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

*/