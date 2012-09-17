//Created by Max Merchel

using UnityEngine;
using System.Collections;

public class NotQuadraticMeshConstruction : MonoBehaviour {
	
	private GameObject NewObject;				//GameObject for the New Object
	public string ObjectName = "New Object";	//Name of the New GameObject
	
	public Vector3 ObjectCenter;		//Center of the new Object
	public Vector3 ObjectRotation;		//Rotation of the new Object
	private Quaternion RotationQuat;	//Rotation Quaternion of the new Object
	
	public enum category {trapezium, pyramid}	//trapezium (U.K.) /  trapezoid (U.S.)
	public category ObjectType;
	
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
	
	public enum Parts{one, all}; //1 = all sides the same Texture  / all = 1 Texture for every side 
	public Parts TextureParts;
	private int TexturePartNumber;
	
	private int[] faceAdd;
	
	private int face;
	private int facemulti;
	
	public struct Triangle
	{
		public float alpha;
		public float beta;
		public float gamma;
		
	} 
	private Triangle frontTriangle, sideTriangle, groundTriangle;
	
	// Use this for initialization
	void Start () 
	{		
		if(ObjectWidth > 0.0f && ObjectHeight > 0.0f && ObjectDepth > 0.0f)
		{
			CalculateMeshSize();			
			
			CreateObject();
			
			CalculateTriangles();
			
			CreateMesh();
			
			
			
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
			Debug.LogError ("No GameObejct with name " + ObjectName + " created");
			#endregion
		}
	}
	
	void CalculateMeshSize()
	{
		HalfMeshHeight = ObjectHeight / 2;
		HalfMeshWidth = ObjectWidth / 2;
		HalfMeshDepth = ObjectDepth / 2;
		
		MeshHeight = ObjectHeight / SectionHeight;
		MeshWidth = ObjectWidth / SectionWidth;	
		MeshDepth = ObjectDepth / SectionDepth;	
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
			case Parts.all:
				switch(ObjectType)
				{
					case category.trapezium:
						TexturePartNumber = 4;
						break;
					case category.pyramid:
						TexturePartNumber = 5;
						break;
				}
				break;			
			case Parts.one:
			default:
				TexturePartNumber = 1;
				break;
		}
	
	}
	
	void CalculateTriangles()
	{
		float a = ObjectHeight;
		float b = HalfMeshWidth;
		float c = Mathf.Sqrt((a*a)+(b*b));
				
		frontTriangle.alpha = Mathf.Asin((a/c));
		frontTriangle.beta = Mathf.Asin((b/c));
		frontTriangle.gamma = 90/(180*Mathf.PI);
		
		
		a = ObjectDepth;
		b = HalfMeshWidth;
		c = Mathf.Sqrt((a*a)+(b*b));
				
		groundTriangle.alpha = Mathf.Asin((a/c));
		groundTriangle.beta = Mathf.Asin((b/c));	
		groundTriangle.gamma = 90/(180*Mathf.PI);
		

		a = ObjectHeight;
		b = HalfMeshDepth;
		c = Mathf.Sqrt((a*a)+(b*b));
		
		sideTriangle.alpha = Mathf.Asin((a/c));
		sideTriangle.beta = Mathf.Asin((b/c));	
		sideTriangle.gamma = 90/(180*Mathf.PI);
		
	}
	
	
	void CreateMesh()
	{
		CreateMeshSections();
		CreateUV();
		
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
		
		face = 0;
		
		int height = 0;
		int width = 0;
		int depth = 0;
		
		int length = 400;	// !!no length calculation!!		
		
		newVertices = new Vector3[length];
		
		float vertexHeight, vertexWidth, vertexDepth;
		float a, b, c;
		bool left = false, right = false;
		bool rightSide = false;
		
		for(int i = 0; i < length; i++)
		{
			switch(ObjectType)
			{
				case category.trapezium:
					
					break;
				case category.pyramid:
					#region Region pyramid
					switch(face)
					{
						//Ground
						case 0:
							newVertices[i] = new Vector3(-HalfMeshWidth +  (width * MeshWidth), -HalfMeshHeight ,-HalfMeshDepth + (depth * MeshDepth));
							
							if(width == SectionWidth)
							{		
								depth++;
								width = -1;
								
								if(depth > SectionDepth)
								{
									face++;
									depth = 0;
								}
							
							}
							width++;							
							break;
							
						//Front
						case 1:
							
							if (height != 0)
							{
								if(height != SectionHeight)
								{
									vertexWidth = width * MeshWidth;
									
									if (vertexWidth < HalfMeshWidth)
									{
										vertexWidth = ObjectWidth - vertexWidth;
										rightSide = true;
									}
									
									vertexHeight = height * MeshHeight;
									//vertexDepth = 
										
																		
									a =	vertexWidth / Mathf.Tan(frontTriangle.beta);
									
									
																			
									if(left == false)
									{
										if(vertexHeight <= a)
										{
										
											if(vertexHeight == a)
											{
												newVertices[i] = new Vector3((width * MeshWidth) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)) - HalfMeshDepth);
												left = true;
											}
											else if( width > 0)
											{
												b = vertexHeight / Mathf.Tan (frontTriangle.alpha);	
												
												if(vertexWidth - MeshWidth < b)
												{
													newVertices[i] = new Vector3(b - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)) - HalfMeshDepth);
													left = true;
												}
												
											}
										}
										else //vertexHeight > a									
										{
											b = vertexHeight / Mathf.Tan (frontTriangle.alpha);
											
											if((b/MeshWidth) < (SectionWidth / 2))
											{
												width = (int)(b / MeshWidth);
						
												newVertices[i] = new Vector3(b - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)) - HalfMeshDepth);
												left = true;
												
											}
											else
											{
												#region Region Debug Error
												Debug.LogError ("Wrong Triangle Calculation");
												Debug.LogError ("Face: " + face + " I: " + i);
												#endregion Region Debug Error
											}
										}
									} 
									else if(right == false)
									{
										if(vertexHeight == a)
										{
											newVertices[i] = new Vector3((width * MeshWidth) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)) - HalfMeshDepth);
											right = true;
										}
										else if (rightSide == false)
										{
											newVertices[i] = new Vector3((width * MeshWidth) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)) - HalfMeshDepth);	
										}
										else if (rightSide == true)
										{
											b = vertexHeight / Mathf.Tan (frontTriangle.alpha);	
											
											if(vertexWidth - MeshWidth < b)
											{
												newVertices[i] = new Vector3(ObjectWidth - b, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)) - HalfMeshDepth);
												right = true;
												
											}
											else
											{
												newVertices[i] = new Vector3((width * MeshWidth) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)) - HalfMeshDepth);	
											}
											
										}
									}
	
																	
								}
								else //heigt == SectionHeight
								{
									newVertices[i] = new Vector3(0.0f, HalfMeshHeight, 0.0f);	
									right = true;
								}															
								
							}
							else //heigt == 0
							{
								newVertices[i] = new Vector3(-HalfMeshWidth +  (width * MeshWidth), -HalfMeshHeight ,-HalfMeshDepth);
							}
							
							if(right == true)
							{
								width = SectionWidth;
							}
							
							if(width == SectionWidth)
							{		
								height++;
								width = -1;
								left = false;
								right = false;
								
								rightSide = false;
								
								if(height > SectionHeight)
								{
									face++;
									height = 0;
								}
							
							}
							width++;
							
							break;
						//Left
						case 2:
							break;
							
						//Back
						case 3:
							
							if (height != 0)
							{
								if(height != SectionHeight)
								{
									vertexWidth = width * MeshWidth;
									
									if (vertexWidth < HalfMeshWidth)
									{
										vertexWidth = ObjectWidth - vertexWidth;
										rightSide = true;
									}
									
									vertexHeight = height * MeshHeight;
									//vertexDepth = 
										
																		
									a =	vertexWidth / Mathf.Tan(frontTriangle.beta);
									
									
																			
									if(left == false)
									{
										if(vertexHeight <= a)
										{
										
											if(vertexHeight == a)
											{
												newVertices[i] = new Vector3((width * MeshWidth) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)));
												left = true;
											}
											else if( width > 0)
											{
												b = vertexHeight / Mathf.Tan (frontTriangle.alpha);	
												
												if(vertexWidth - MeshWidth < b)
												{
													newVertices[i] = new Vector3(b - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)));
													left = true;
												}
												
											}
										}
										else //vertexHeight > a									
										{
											b = vertexHeight / Mathf.Tan (frontTriangle.alpha);
											
											if((b/MeshWidth) < (SectionWidth / 2))
											{
												width = (int)(b / MeshWidth);
						
												newVertices[i] = new Vector3(b - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)));
												left = true;
												
											}
											else
											{
												#region Region Debug Error
												Debug.LogError ("Wrong Triangle Calculation");
												Debug.LogError ("Face: " + face + " I: " + i);
												#endregion Region Debug Error
											}
										}
									} 
									else if(right == false)
									{
										if(vertexHeight == a)
										{
											newVertices[i] = new Vector3((width * MeshWidth) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)));
											right = true;
										}
										else if (rightSide == false)
										{
											newVertices[i] = new Vector3((width * MeshWidth) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)));	
										}
										else if (rightSide == true)
										{
											b = vertexHeight / Mathf.Tan (frontTriangle.alpha);	
											
											if(vertexWidth - MeshWidth < b)
											{
												newVertices[i] = new Vector3(ObjectWidth - b, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)));
												right = true;
												
											}
											else
											{
												newVertices[i] = new Vector3((width * MeshWidth) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(vertexHeight / Mathf.Tan(sideTriangle.alpha)));	
											}
											
										}
									}
	
																	
								}
								else //heigt == SectionHeight
								{
									newVertices[i] = new Vector3(0.0f, HalfMeshHeight, 0.0f);	
									right = true;
								}															
								
							}
							else //heigt == 0
							{
								newVertices[i] = new Vector3(-HalfMeshWidth +  (width * MeshWidth), -HalfMeshHeight ,HalfMeshDepth);
							}
							
							if(right == true)
							{
								width = 0;
							}
							
							if(width == 0)
							{		
								height++;
								width = SectionWidth + 1;
								left = false;
								right = false;
								
								rightSide = false;
								
								if(height > SectionHeight)
								{
									face++;
									height = 0;
								}
							
							}
							width--;
							
							break;
						
						//Right
						case 4:
							break;
						case 5:	//no pyramid face
							break;
					}
					#endregion pyramid
					break;
			}

		}

	}
	
	
	void CreateUV()
	{
		
		face = 0;
		
		if(TexturePartNumber == 1)
		{
			facemulti = 0;
		}
		else
		{
			facemulti = 1 / TexturePartNumber;
		}
		
		
	}
	
	
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
