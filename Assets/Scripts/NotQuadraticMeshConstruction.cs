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
	private float facemulti;
	
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
		newUVs = new Vector2[length];
		
		float vertexHeight, vertexWidth, vertexDepth;
		float a, b, c;
		bool left = false, right = false;
		bool rightSide = false;
		bool depthLine = false;
		
		#region UV
		bool UVfrontHeight;
		if( ObjectHeight >= ObjectWidth)
		{
			UVfrontHeight = true;
		}
		else
		{
			UVfrontHeight = false;
		}
		
		bool UVsideHeight;
		if( ObjectHeight >= ObjectDepth)
		{
			UVsideHeight = true;
		}
		else
		{
			UVsideHeight = false;
		}
		
		facemulti = 1 / TexturePartNumber;
		
		#endregion
		
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
							
							#region Region UV
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + face * facemulti, (float)depth/SectionDepth);							
							#endregion UV
							
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
							
						#region Region Front
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
									
									vertexDepth = (depth + 1) * MeshDepth;
									c = vertexHeight / Mathf.Tan(sideTriangle.alpha);
									
									if(c <= vertexDepth)
									{
										if(c == vertexDepth)
										{
											//Depth == Height
											depth++;
										}
										else
										{
											depthLine = true;
											vertexHeight = vertexDepth / Mathf.Tan(sideTriangle.beta);	//Calculate new vertexHeight
											
										}
									}
																		
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
							
							#region Region UV
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + face * facemulti, (float)height/SectionHeight);
							#endregion UV
							
							if(right == true)
							{
								width = SectionWidth;
							}
							
							if(width == SectionWidth)
							{		
								if(depthLine == true)
								{
									depthLine = false;
								}
								else
								{
									height++;
								}	
								
								width = -1;
								left = false;
								right = false;
								
								rightSide = false;
								
								if(height > SectionHeight)
								{
									face++;
									height = 0;
									depth = 0;
								}
							
							}
							width++;
							
							break;
						#endregion Front
							
						#region Region Left
						case 2:
							
							if (height != 0)
							{
								if(height != SectionHeight)
								{
									vertexDepth = depth * MeshDepth;
									
									if (vertexDepth < HalfMeshDepth)
									{
										vertexDepth = ObjectDepth - vertexDepth;
										rightSide = true;
									}
									
									vertexHeight = height * MeshHeight;
									
									vertexWidth = (width + 1) * MeshWidth;	
									
									b = vertexHeight / Mathf.Tan(frontTriangle.alpha);
									
									if(b <= vertexWidth)
									{
										if(b == vertexWidth)
										{
											//Width == Height
											width++;
										}
										else
										{
											depthLine = true;
											vertexHeight = vertexWidth / Mathf.Tan(frontTriangle.beta);	//Calculate new vertexHeight
											
										}
									}
																		
									a =	vertexWidth / Mathf.Tan(sideTriangle.beta);
									
									
																			
									if(left == false)
									{
										if(vertexHeight <= a)
										{
										
											if(vertexHeight == a)
											{
												newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(depth * MeshDepth) - HalfMeshDepth);
												left = true;
											}
											else if( width > 0)
											{
												c = vertexHeight / Mathf.Tan (sideTriangle.alpha);	
												
												if(vertexWidth - MeshWidth < c)
												{
													newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,c - HalfMeshDepth);
													left = true;
												}
												
											}
										}
										else //vertexHeight > a									
										{
											c = vertexHeight / Mathf.Tan (sideTriangle.alpha);
											
											if((c/MeshDepth) < (SectionDepth / 2))
											{
												depth = (int)(c / MeshDepth);
						
												newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,c - HalfMeshDepth);
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
											newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(depth * MeshDepth) - HalfMeshDepth);
											right = true;
										}
										else if (rightSide == false)
										{
											newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(depth * MeshDepth) - HalfMeshDepth);
										}
										else if (rightSide == true)
										{
											c = vertexHeight / Mathf.Tan (sideTriangle.alpha);	
											
											if(vertexWidth - MeshWidth < b)
											{
												newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,ObjectDepth - c);
												right = true;
												
											}
											else
											{
												newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(depth * MeshDepth) - HalfMeshDepth);
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
								newVertices[i] = new Vector3(-HalfMeshWidth, -HalfMeshHeight ,(depth * MeshDepth) - HalfMeshDepth);
							}
							
							#region Region UV
							newUVs[i] = new Vector2((depth * facemulti)/SectionDepth + face * facemulti, (float)height/SectionHeight);	
							#endregion UV
							
							if(right == true)
							{
								depth = SectionDepth;
							}
							
							if(depth == SectionWidth)
							{		
								if(depthLine == true)
								{
									depthLine = false;
								}
								else
								{
									height++;
								}								
								depth = -1;
								left = false;
								right = false;
								
								rightSide = false;
								
								if(height > SectionHeight)
								{
									face++;
									height = 0;
									depth = 0;
									width = SectionWidth;
								}
							
							}
							depth++;
							
							
							break;
						#endregion Left
							
						#region Region Back
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
									
									vertexDepth = (depth + 1) * MeshDepth;
									c = vertexHeight / Mathf.Tan(sideTriangle.alpha);
									
									if(c <= vertexDepth)
									{
										if(c == vertexDepth)
										{
											//Depth == Height
											depth++;
										}
										else
										{
											depthLine = true;
											vertexHeight = vertexDepth / Mathf.Tan(sideTriangle.beta);	//Calculate new vertexHeight
											
										}
									}
										
																		
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
							
							#region Region UV
							newUVs[i] = new Vector2((width * facemulti)/SectionWidth + face * facemulti, (float)height/SectionHeight);
							#endregion UV
							
							if(right == true)
							{
								width = 0;
							}
							
							if(width == 0)
							{		
								if(depthLine == true)
								{
									depthLine = false;
								}
								else
								{
									height++;
								}				
								
								width = SectionWidth + 1;
								left = false;
								right = false;
								
								rightSide = false;
								
								if(height > SectionHeight)
								{
									face++;
									height = 0;
									depth = SectionDepth;
									width = 1;
								}
							
							}
							width--;
							
							break;
						#endregion Back
						
						#region Region Right
						case 4:
							
							if (height != 0)
							{
								if(height != SectionHeight)
								{
									vertexDepth = depth * MeshDepth;
									
									if (vertexDepth < HalfMeshDepth)
									{
										vertexDepth = ObjectDepth - vertexDepth;
										rightSide = true;
									}
									
									vertexHeight = height * MeshHeight;
									
									vertexWidth = (width + 1) * MeshWidth;	
									
									b = vertexHeight / Mathf.Tan(frontTriangle.alpha);
									
									if(b <= vertexWidth)
									{
										if(b == vertexWidth)
										{
											//Width == Height
											width++;
										}
										else
										{
											depthLine = true;
											vertexHeight = vertexWidth / Mathf.Tan(frontTriangle.beta);	//Calculate new vertexHeight
											
										}
									}
																		
									a =	vertexWidth / Mathf.Tan(sideTriangle.beta);
									
									
																			
									if(left == false)
									{
										if(vertexHeight <= a)
										{
										
											if(vertexHeight == a)
											{
												newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(depth * MeshDepth) - HalfMeshDepth);
												left = true;
											}
											else if( width > 0)
											{
												c = vertexHeight / Mathf.Tan (sideTriangle.alpha);	
												
												if(vertexWidth - MeshWidth < c)
												{
													newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,c - HalfMeshDepth);
													left = true;
												}
												
											}
										}
										else //vertexHeight > a									
										{
											c = vertexHeight / Mathf.Tan (sideTriangle.alpha);
											
											if((c/MeshDepth) < (SectionDepth / 2))
											{
												depth = (int)(c / MeshDepth);
						
												newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,c - HalfMeshDepth);
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
											newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(depth * MeshDepth) - HalfMeshDepth);
											right = true;
										}
										else if (rightSide == false)
										{
											newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(depth * MeshDepth) - HalfMeshDepth);
										}
										else if (rightSide == true)
										{
											c = vertexHeight / Mathf.Tan (sideTriangle.alpha);	
											
											if(vertexWidth - MeshWidth < b)
											{
												newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,ObjectDepth - c);
												right = true;												
											}
											else
											{
												newVertices[i] = new Vector3((vertexHeight / Mathf.Tan(frontTriangle.alpha)) - HalfMeshWidth, vertexHeight - HalfMeshHeight ,(depth * MeshDepth) - HalfMeshDepth);
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
								newVertices[i] = new Vector3(-HalfMeshWidth, -HalfMeshHeight ,(depth * MeshDepth) - HalfMeshDepth);
							}
							
							#region Region UV
							newUVs[i] = new Vector2((depth * facemulti)/SectionDepth + face * facemulti, (float)height/SectionHeight);	
							#endregion UV
							
							if(right == true)
							{
								depth = 0;
							}
							
							if(depth == 0)
							{		
								if(depthLine == true)
								{
									depthLine = false;
								}
								else
								{
									height++;
								}								
								depth = SectionWidth + 1;
								left = false;
								right = false;
								
								rightSide = false;
								
								if(height > SectionHeight)
								{
									face++;
									height = 0;
									depth = 0;
									width = 0;
								}
							
							}
							depth--;
							
							break;
							#endregion Right
							
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
