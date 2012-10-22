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
	
	private int length;		//max Vertice length
	
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
	
	public struct TriangleVertecis
	{
		public int[] left;
		public int[] right;
		public int last;
		public int lines;		
		public int first;	
	} 
	
	private TriangleVertecis VertecisFront, VerticesBack, VerticesLeft, VerticesRight, VerticesGround;
	
	
	
	// Use this for initialization
	void Start () 
	{		
		if(ObjectWidth > 0.0f && ObjectHeight > 0.0f && ObjectDepth > 0.0f)
		{
			CalculateMeshSize();			
			
			CreateObject();
			
			CalculateTriangles();
			TriangleVertecisInit();
			
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
	
	void TriangleVertecisInit()
	{
		VertecisFront.left = new int[SectionHeight + SectionDepth];
		VertecisFront.right = new int[SectionHeight + SectionDepth];
		
		VerticesBack.left = new int[SectionHeight + SectionDepth];
		VerticesBack.right = new int[SectionHeight + SectionDepth];
		
		VerticesLeft.left = new int[SectionHeight + SectionWidth];
		VerticesLeft.right = new int[SectionHeight + SectionWidth];
		
		VerticesRight.left = new int[SectionHeight + SectionWidth];
		VerticesRight.right = new int[SectionHeight + SectionWidth];
		
		VerticesGround.left = new int[SectionDepth + SectionWidth];
		VerticesGround.right = new int[SectionDepth + SectionWidth];
	}
	
	void CreateUV()
	{
		
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

		
		if(TexturePartNumber == 1)
		{
			facemulti = 0;
		}
		else
		{
			facemulti = 1 / TexturePartNumber;
		}
		
		
	}
	
	void CreateMesh()
	{
		CreateUV();
		CreateMeshSections();
		createTriangles();
		
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
		
		length = 400;	// !!no length calculation!!	
		
		int length2;
		
		if(SectionWidth >= SectionDepth)
		{
			length = (((SectionWidth + 1) * (SectionDepth + 1)) + 4 * ((SectionWidth + 1) * ((SectionHeight + 1) + (SectionDepth + 1))));
		}
		else //SectionWidth < SectionDepth
		{
			length = (((SectionWidth + 1) * (SectionDepth + 1)) + 4 * ((SectionDepth + 1) * ((SectionHeight + 1) + (SectionWidth + 1))));
			
		}
		#region Region Debug
		//Debug.Log("Length: " + length);
		#endregion debug
		
		newVertices = new Vector3[length];
		newUVs = new Vector2[length];
		
		float vertexHeight = 0.0f, vertexWidth = 0.0f, vertexDepth = 0.0f;
		float a = 0.0f, b = 0.0f, c = 0.0f;
		bool left = false, right = false;
		bool rightSide = false;
		bool depthLine = false;
		
		int j = 0;
			
		
		for(int i = 0; i < length; i++)
		{
			switch(ObjectType)
			{
				case category.trapezium:
					#region Region trapezium
					switch(face)
					{
						case 0:
							#region Region Ground
													
							#endregion Ground
							break;

						case 1:
							#region Region Front
							
							#endregion Front
							break;

						case 2:				
							#region Region Left
							
							#endregion Left
							break;

						case 3:
							#region Region Right
							
							#endregion Right
							break;
							
						case 4:	//no trapezium face
							break;
					}
					#endregion trapezium
					break;
				case category.pyramid:
					#region Region pyramid
					switch(face)
					{
						#region Region Ground
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
									
									VertecisFront.first = i + 1;
								}
							
							}
							width++;
							
							#region Region Debug
							//Debug.Log("i: " + i + " Vertex: " + newVertices[i]);
							#endregion  Debug
							
							break;
						#endregion Ground
							
						#region Region Front
						case 1:
							#region Region Debug
							//Debug.Log("i: " + i + " j: " + j + " Height: " + height + " Width: " + width + " Depth:" + depth);
							#endregion  Debug
							
							if (height != 0)
							{
								if(height != SectionHeight)
								{
	
									vertexWidth = width * MeshWidth;
									
									if (vertexWidth > HalfMeshWidth)
									{
										vertexWidth = -1 * (vertexWidth - ObjectWidth);
										
										rightSide = true;
									}
									
									if(depthLine == false)
									{
										vertexHeight = height * MeshHeight;	
									
										vertexDepth = (depth + 1) * MeshDepth;
										
										c = (Mathf.Sin(sideTriangle.beta) * vertexHeight) / Mathf.Sin(sideTriangle.alpha);
										
										#region Region Debug
										//Debug.Log("VertexHeight: " + vertexHeight +" VertexDepth: " + vertexDepth + " C: " + c);
										#endregion  Debug
										
										if(c >= vertexDepth)
										{
											if(c == vertexDepth)
											{
												//Depth == Height
												depth++;
											}
											else //c > vertexDepth
											{
												depthLine = true;
												vertexHeight = (Mathf.Sin(sideTriangle.alpha) * vertexDepth) / Mathf.Sin(sideTriangle.beta);
												
											}
										}
										else //c < vertexDepth
										{
											vertexDepth = c;
										}
									}
									
									a = (Mathf.Sin(frontTriangle.alpha)* vertexWidth) / Mathf.Sin(frontTriangle.beta);
								
									#region Region Debug
									//Debug.Log("a:" + a);
									#endregion  Debug
									
									if(left == false)
									{
										if(a == vertexHeight)
										{
											left = true;
										}
										else //if(a < vertexHeight)
										{
											b = (Mathf.Sin(frontTriangle.beta) * vertexHeight) / Mathf.Sin(frontTriangle.alpha);
											vertexWidth = b;
											
											if(b / MeshWidth > width)
											{
												#region Region Debug
												//Debug.Log("i: " + i + " Width: " + width + " B:" + b + " MeshWidth:" + MeshWidth);
												#endregion  Debug
												
												width = (int)(b / MeshWidth);

											}
											
											left = true;
										}
										
										VertecisFront.left[j] = i;
										
										#region Region Debug
										//Debug.Log("Front.Left " + j + ": " + VertecisFront.left[j]);
										#endregion  Debug
									}
									else  //left == true
									{
										#region Region Debug
										//Debug.Log("a:" + a + " vertexHeight: " + vertexHeight);
										#endregion  Debug
										
										if(a == vertexHeight)
										{
											right = true;
										}
										else if(a < vertexHeight)
										{
											b = (Mathf.Sin(frontTriangle.beta) * vertexHeight) / Mathf.Sin(frontTriangle.alpha);
											
											vertexWidth = b;	
											right = true;
										}
										
										if(rightSide == true)
										{
											vertexWidth = -1 * (vertexWidth - ObjectWidth);													
										}
										
									}
									
									newVertices[i] = new Vector3(vertexWidth -HalfMeshWidth, vertexHeight -HalfMeshHeight , vertexDepth - HalfMeshDepth);
									
								} 
								else //height == SectionHeight
								{
									newVertices[i] = new Vector3(0.0f, HalfMeshHeight, 0.0f);	
									width = SectionWidth;
									VertecisFront.last = i;
									
									#region Region Debug
									//Debug.Log("Front.Last: " + VertecisFront.last);
									#endregion  Debug
								}
								
							}
							else //height == 0
							{
								if(left == false)
								{
									VertecisFront.left[j] = i;
									left = true;
									
									#region Region Debug
									//Debug.Log("Front.Left " + j + ": " + VertecisFront.left[j]);
									#endregion  Debug
								}
								
								if(width == SectionWidth)
								{
									right = true;
								}
								
								newVertices[i] = new Vector3((width * MeshWidth) -HalfMeshWidth, -HalfMeshHeight ,- HalfMeshDepth);
							}
									
							if(right == true)
							{
								if(newVertices[i] == newVertices[i-1])
								{
									i--;
								}
								VertecisFront.right[j] = i;
								j++;
								width = SectionWidth;
								
								#region Region Debug
								//Debug.Log("Front.Right " + (j-1) + ": " + VertecisFront.right[j-1]);
								#endregion  Debug
								
							}
							
							if(width == SectionWidth)
							{								
								if(depthLine == true)
								{
									depthLine = false;
									depth++;
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
									VertecisFront.lines = j;									
									
									face++;
									height = 0;
									depth = 0;
									j = 0;
									
									VerticesLeft.first = i + 1;
								}
							
							}
							
							width++;
						
							#region Region Debug
							//Debug.Log("i: " + i + " Vertex: " + newVertices[i]);
							#endregion  Debug
							
							break;
						#endregion Front
							
						#region Region Left
						case 2:
							#region Region Debug
							//Debug.Log("i: " + i + " j: " + j + " Height: " + height + " Width: " + width + " Depth:" + depth);
							#endregion  Debug
							
							if (height != 0)
							{
								if(height != SectionHeight)
								{
									vertexDepth = depth * MeshDepth;									
									
									if (vertexDepth > HalfMeshDepth)
									{
										vertexDepth = -1 * (vertexDepth - ObjectWidth);
										
										rightSide = true;
									}
									
									if(depthLine == false)
									{
										vertexHeight = height * MeshHeight;										
										
										vertexWidth = (width + 1) * MeshWidth;
										
										b = (Mathf.Sin(frontTriangle.beta) * vertexHeight) / Mathf.Sin(frontTriangle.alpha);
										
										#region Region Debug
										//Debug.Log("VertexHeight: " + vertexHeight +" VertexDepth: " + vertexDepth + " C: " + c);
										#endregion  Debug
										
										if(b >= vertexWidth)
										{
											if(b == vertexWidth)
											{
												//Depth == Height
												width++;
											}
											else //b > vertexDepth
											{
												depthLine = true;
												vertexHeight = (Mathf.Sin(frontTriangle.alpha) * vertexWidth) / Mathf.Sin(frontTriangle.beta);
												
											}
										}
										else //b < vertexDepth
										{
											vertexWidth = b;
										}
									}
																		
									a = (Mathf.Sin(sideTriangle.alpha)* vertexDepth) / Mathf.Sin(sideTriangle.beta);
									
									if(left == false)
									{
										if(a == vertexHeight)
										{
											left = true;
										}
										else //if(a < vertexHeight)
										{
											c = (Mathf.Sin(sideTriangle.beta) * vertexHeight) / Mathf.Sin(sideTriangle.alpha);
											vertexDepth = c;
											
											if(c / MeshDepth > depth)
											{
												#region Region Debug
												//Debug.Log("i: " + i + " Depth: " + depth + " C:" + c + " MeshDepth:" + MeshWidth);
												#endregion  Debug
												depth = (int)(c / MeshDepth);

											}
											
											left = true;
										}
																				
										VerticesLeft.left[j] = i;
										
										#region Region Debug
										//Debug.Log("Left.Left " + j + ": " + VerticesLeft.left[j]);
										#endregion  Debug
									}
									else //left == true
									{
										if(a == vertexHeight)
										{
											right = true;
										}
										else if(a < vertexHeight)
										{
											c = (Mathf.Sin(sideTriangle.beta) * vertexHeight) / Mathf.Sin(sideTriangle.alpha);
											
											vertexDepth = c;
											right = true;
										}
										
										if(rightSide == true)
										{
											vertexDepth = -1 * (vertexDepth - ObjectDepth);	
										}
									}
									
									newVertices[i] = new Vector3(vertexWidth -HalfMeshWidth, vertexHeight -HalfMeshHeight , vertexDepth - HalfMeshDepth);	
									
								} 
								else //height == SectionHeight
								{
									newVertices[i] = new Vector3(0.0f, HalfMeshHeight, 0.0f);	
									depth = SectionDepth;
									VerticesLeft.last = i;
									
									#region Region Debug
									//Debug.Log("Left.Last: " + VerticesLeft.last);
									#endregion  Debug
								}
								
							}
							else //height == 0
							{
								if(left == false)
								{
									VerticesLeft.left[j] = i;
									left = true;
									
									#region Region Debug
									//Debug.Log("Left.Left " + j + ": " + VerticesLeft.left[j]);
									#endregion  Debug
								}
								
								if(depth == SectionDepth)
								{
									right = true;
								}
								
								newVertices[i] = new Vector3(-HalfMeshWidth, -HalfMeshHeight ,(depth * MeshDepth) - HalfMeshDepth);
							}
									
							if(right == true)
							{
								if(newVertices[i] == newVertices[i-1])
								{
									i--;
								}
								VerticesLeft.right[j] = i;
								j++;
								depth = SectionDepth;
								
								#region Region Debug
								//Debug.Log("Left.Right " + (j-1) + ": " + VerticesLeft.right[j-1]);
								#endregion  Debug
								
							}
							
							if(depth == SectionDepth)
							{								
								if(depthLine == true)
								{
									depthLine = false;
									width++;
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
									VerticesLeft.lines = j;								
									
									face++;
									height = 0;
									width = 0;
									depth = SectionDepth - 1;
									j = 0;
									
									VerticesBack.first = i + 1;
								}
							}
							depth++;
							
							#region Region Debug
							//Debug.Log("i: " + i + " Vertex: " + newVertices[i]);
							#endregion  Debug

							break;
						#endregion Left
							
						#region Region Back
						case 3:
							if (height != 0)
							{
								if(height != SectionHeight)
								{
	
									vertexWidth = width * MeshWidth;
									
									if (vertexWidth > HalfMeshWidth)
									{
										vertexWidth = -1 * (vertexWidth - ObjectWidth);
										rightSide = true;
									}
									#region Region Debug
									//Debug.Log("VertexDepth: " + vertexDepth + " i: " + (i - 1));
									#endregion  Debug
									
									if(depthLine == false)
									{
										vertexHeight = height * MeshHeight;	
									
										vertexDepth = (depth - 1) * MeshDepth;
										
										c = (Mathf.Sin(sideTriangle.beta) * vertexHeight) / Mathf.Sin(sideTriangle.alpha);
										
										#region Region Debug
										//Debug.Log("VertexHeight: " + vertexHeight +" VertexDepth: " + vertexDepth + " C: " + c);
										#endregion  Debug
										
										if(c >= (ObjectDepth - vertexDepth))
										{
											if(c == (ObjectDepth - vertexDepth))
											{
												//Depth == Height
												depth--;
											}
											else //c > vertexDepth
											{
												depthLine = true;
												vertexHeight = (Mathf.Sin(sideTriangle.alpha) * (ObjectDepth - vertexDepth)) / Mathf.Sin(sideTriangle.beta);
												
												#region Region Debug
												//Debug.Log("VertexDepth: " + vertexDepth + " Depth:" + (ObjectDepth - vertexDepth) + " C: " + c);
												#endregion  Debug
											}
										}
										else //c < vertexDepth
										{
											vertexDepth = c;
										}
									}
																		
									a = (Mathf.Sin(frontTriangle.alpha)* vertexWidth) / Mathf.Sin(frontTriangle.beta);
									
									if(left == false)
									{
										if(a == vertexHeight)
										{
											left = true;
										}
										else //a < vertexHeight)
										{
											b = (Mathf.Sin(frontTriangle.beta) * vertexHeight) / Mathf.Sin(frontTriangle.alpha);
											vertexWidth = b;
											
											if(b / MeshWidth > width)
											{
												#region Region Debug
												//Debug.Log("i: " + i + " Width: " + width + " B:" + b + " MeshWidth:" + MeshWidth);
												#endregion  Debug
												width = (int)(b / MeshWidth);

											}
											
											left = true;
										}
										
										VerticesBack.left[j] = i;
										
										#region Region Debug
										//Debug.Log("Back.Left " + j + ": " + VerticesBack.left[j]);
										#endregion  Debug
									}
									else //left == true
									{
										if(a == vertexHeight)
										{											
											right = true;
										}
										else if(a < vertexHeight)
										{
											b = (Mathf.Sin(frontTriangle.beta) * vertexHeight) / Mathf.Sin(frontTriangle.alpha);
											
											vertexWidth = b;
											right = true;
										}
										
										if(rightSide == true)
										{
											vertexWidth = -1 * (vertexWidth - ObjectWidth);
										}

									}
									
									if(depthLine == false)
									{
										newVertices[i] = new Vector3(vertexWidth -HalfMeshWidth, vertexHeight -HalfMeshHeight , -1 * (vertexDepth - HalfMeshDepth));
									}
									else
									{
										newVertices[i] = new Vector3(vertexWidth -HalfMeshWidth, vertexHeight -HalfMeshHeight , (vertexDepth - HalfMeshDepth));
									}

									
								} 
								else //height == SectionHeight
								{
									newVertices[i] = new Vector3(0.0f, HalfMeshHeight, 0.0f);	
									width = SectionWidth;
									VerticesBack.last = i;
									
									#region Region Debug
									//Debug.Log("Back.Last: " + VerticesBack.last);
									#endregion  Debug
								}
								
							}
							else //height == 0
							{
								if(left == false)
								{
									VerticesBack.left[j] = i;
									left = true;
									
									#region Region Debug
									//Debug.Log("Back.Left " + j + ": " + VerticesBack.left[j]);
									#endregion  Debug
								}
								
								if(width == SectionWidth)
								{
									right = true;
								}
								
								newVertices[i] = new Vector3((width * MeshWidth) -HalfMeshWidth, -HalfMeshHeight , HalfMeshDepth);
							}
									
							if(right == true)
							{
								if(newVertices[i] == newVertices[i-1])
								{
									i--;
								}
								
								VerticesBack.right[j] = i;
								j++;
								width = SectionWidth;								
								
								#region Region Debug
								//Debug.Log("Back.Right " + (j-1) + ": " + VerticesBack.right[j-1]);
								#endregion  Debug
								
								
								
							}
							
							if(width == SectionWidth)
							{								
								if(depthLine == true)
								{
									depthLine = false;
									depth--;
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
									VerticesBack.lines = j;									
									
									face++;
									height = 0;
									depth = 0;
									j = 0;
									
									width = SectionWidth - 1;
									VerticesRight.first = i + 1;
								}
							
							}
							
							width++;
							
							#region Region Debug
							//Debug.Log("i: " + i + " Vertex: " + newVertices[i]);
							#endregion  Debug
							
							break;
						#endregion Back
						
						#region Region Right
						case 4:
							#region Region Debug
							Debug.Log("i: " + i + " j: " + j + " Height: " + height + " Width: " + width + " Depth:" + depth);
							#endregion  Debug
							
							if (height != 0)
							{
								if(height != SectionHeight)
								{
									vertexDepth = depth * MeshDepth;									
									
									if (vertexDepth > HalfMeshDepth)
									{
										vertexDepth = -1 * (vertexDepth - ObjectWidth);
										
										rightSide = true;
									}
									
									if(depthLine == false)
									{
										vertexHeight = height * MeshHeight;										
										
										vertexWidth = (width - 1) * MeshWidth;
										
										b = (Mathf.Sin(frontTriangle.beta) * vertexHeight) / Mathf.Sin(frontTriangle.alpha);
										
										#region Region Debug
										//Debug.Log("VertexHeight: " + vertexHeight +" VertexDepth: " + vertexDepth + " C: " + c);
										#endregion  Debug
										
										if(b >= (ObjectWidth - vertexWidth))
										{
											if(b == (ObjectWidth - vertexWidth))
											{
												//Width == Height
												width--;
											}
											else //b > vertexDepth
											{
												depthLine = true;
												vertexHeight = (Mathf.Sin(frontTriangle.alpha) * (ObjectWidth - vertexWidth)) / Mathf.Sin(frontTriangle.beta);
												
											}
										}
										else //b < vertexWidth
										{
											vertexWidth = b;
										}
									}
																		
									a = (Mathf.Sin(sideTriangle.alpha)* vertexDepth) / Mathf.Sin(sideTriangle.beta);
									
									if(left == false)
									{
										if(a == vertexHeight)
										{											
											left = true;
										}
										else if(a < vertexHeight)
										{
											c = (Mathf.Sin(sideTriangle.beta) * vertexHeight) / Mathf.Sin(sideTriangle.alpha);
											vertexDepth = c;
											
											if(c / MeshDepth > depth)	//Wo ist der Fehler???
											{
												#region Region Debug
												Debug.Log("i: " + i + " Depth: " + depth + " C:" + c + " MeshDepth:" + MeshWidth);
												#endregion  Debug
												
												depth = (int)(c / MeshDepth);

											}
											
											left = true;
										}
										
										VerticesRight.left[j] = i;
										
										#region Region Debug
										Debug.Log("Right.Left " + j + ": " + VerticesRight.left[j]);
										#endregion  Debug
									}
									else //left == true
									{
	
										if(a == vertexHeight)
										{
											right = true;
										}
										else if(a < vertexHeight)
										{
											c = (Mathf.Sin(sideTriangle.beta) * vertexHeight) / Mathf.Sin(sideTriangle.alpha);
											
											vertexDepth = c;
											right = true;
										}										
										
										if(rightSide == true)
										{
											vertexDepth = -1 * (vertexDepth - ObjectDepth);	
										}
										
									}
									
									if(depthLine == false)
									{
										newVertices[i] = new Vector3(-1 * (vertexWidth - HalfMeshWidth), vertexHeight -HalfMeshHeight , vertexDepth - HalfMeshDepth);
									}
									else
									{
										newVertices[i] = new Vector3((vertexWidth - HalfMeshWidth), vertexHeight -HalfMeshHeight , vertexDepth - HalfMeshDepth);
									}

									
								} 
								else //height == SectionHeight
								{
									newVertices[i] = new Vector3(0.0f, HalfMeshHeight, 0.0f);	
									depth = SectionDepth;
									VerticesRight.last = i;
									
									#region Region Debug
									//Debug.Log("Right.Last: " + VerticesRight.last);
									#endregion  Debug
								}
								
							}
							else //height == 0
							{
								if(left == false)
								{
									VerticesRight.left[j] = i;
									left = true;
									
									#region Region Debug
									//Debug.Log("Left.Left " + j + ": " + VerticesLeft.left[j]);
									#endregion  Debug
								}
								
								if(depth == SectionDepth)
								{
									right = true;
								}
								
								newVertices[i] = new Vector3(HalfMeshWidth, -HalfMeshHeight ,(depth * MeshDepth)- HalfMeshDepth);
							}
									
							if(right == true)
							{
								if(newVertices[i] == newVertices[i-1])
								{
									i--;
								}
								VerticesRight.right[j] = i;
								j++;
								depth = SectionDepth;
								
								#region Region Debug
								//Debug.Log("Right.Right " + (j-1) + ": " + VerticesRight.right[j-1]);
								#endregion  Debug
								
							}
							
							if(depth == SectionDepth)
							{								
								if(depthLine == true)
								{
									depthLine = false;
									width--;
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
									VerticesRight.lines = j;								
									
									face++;
									height = 0;
									width = 0;
									depth = SectionDepth - 1;
									j = 0;
									
								}
							}
							depth++;
							
							#region Region Debug
							Debug.Log("i: " + i + " Vertex: " + newVertices[i]);
							#endregion  Debug
							
							break;
							#endregion Right
							
						case 5:	//no pyramid face
							i = length;
							break;
					}
					#endregion pyramid
					break;
			}

		}

	}
	
	
	void createTriangles()
	{
		int j = 0;		
		int line = 0;
		
		face = 0;
		int TrianglesLength = length * 3;
		newTriangles = new int[TrianglesLength];
		
		switch(ObjectType)
			{
				case category.trapezium:
					#region Region trapezium
					switch(face)
					{
						case 0:
							#region Region Ground
													
							#endregion Ground
							break;

						case 1:
							#region Region Front
							
							#endregion Front
							break;

						case 2:				
							#region Region Left
							
							#endregion Left
							break;

						case 3:
							#region Region Right
							
							#endregion Right
							break;
							
						case 4:	//no trapezium face
							break;
					}
					#endregion trapezium
					break;
				case category.pyramid:
					#region Region pyramid
					
				#region Region Debug
				//Debug.Log("Ground: fist: " + (0) + " last: " + (VertecisFront.first - 1));
				//Debug.Log("Front: fist: " + VertecisFront.first + " last: " + VertecisFront.last);
				//Debug.Log("Left: fist: " + VerticesLeft.first + " last: " + VerticesLeft.last);
				//Debug.Log("Back: fist: " + VerticesBack.first + " last: " + VerticesBack.last);
				//Debug.Log("Right: fist: " + VerticesRight.first + " last: " + VerticesRight.last);
				#endregion Debug
				
				
				for(int k = 0; k < 5; k++)
				{
					#region Region Debug
					//Debug.Log("k: " + k + " Face: " + face);
					#endregion Debug
					
					switch(face)
					{
					case 0:
						#region Region Ground
						int groundVertices = (SectionHeight+1) * (SectionWidth+1);
						
						for(int i = 0; i < groundVertices; i++)
						{
							if(i >= SectionWidth)
							{
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
									#endregion
									
									j += 6;							
								}	
							}
						}
						
						face++;
						
						#endregion Ground
						break;

					case 1:
						#region Region Front
							
						for(int i = VertecisFront.first; i < VertecisFront.last; i++)
						{
							#region Region Debug
							//Debug.Log("i: " + i + " Line: " + line);
							#endregion Debug
							
							int A = (VertecisFront.right[line] + 1)- VertecisFront.left[line];
							int B = (VertecisFront.right[line + 1] + 1) - VertecisFront.left[line + 1];
							int VerticesDifference = (A - B) /2	+ B;
								
							#region Region Debug
							//Debug.Log("A: " + A + " B: " + B + " VerticesDifference:" + VerticesDifference);
							#endregion Debug
							
							if(i + 1 == VertecisFront.right[line] && i + 2 == VertecisFront.last)
							{
								newTriangles[j] = i + 1;
								newTriangles[j+2] = i + 2;
								newTriangles[j+1] = i;
									
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
								
								j += 3;
								
								
								face++;								
								line = 0;
								break;
							}
							else if((i > VertecisFront.left[line] && i < VertecisFront.right[line] - 1)
								|| ((i == VertecisFront.left[line] || i == VertecisFront.right[line] - 1) && A == B ))
							{
								newTriangles[j] = i + VerticesDifference;
								newTriangles[j+1] = i + VerticesDifference + 1;
								newTriangles[j+2] = i;
								
								newTriangles[j+4] = i;
								newTriangles[j+3] = i + 1;
								newTriangles[j+5] = i + VerticesDifference + 1;
							
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								//Debug.Log("newTriangles[j+3]: " + newTriangles[j+3] + " newTriangles[j+4]: " + newTriangles[j+4] + " newTriangles[j+5]: " + newTriangles[j+5]);	
								#endregion Debug
								
								j += 6;
							}
							else if(i == VertecisFront.left[line])
							{
								newTriangles[j] = i + 1;
								if(line + 1 < VertecisFront.lines)
								{
									newTriangles[j+2] = VertecisFront.left[line + 1];
								}
								else
								{
									newTriangles[j+2] = VertecisFront.last;
								}	
								newTriangles[j+1] = i;
									
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
									
								j += 3;
							}
							else if(i == VertecisFront.right[line] - 1)
							{
								newTriangles[j] = i + 1;
								newTriangles[j+2] = VertecisFront.right[line + 1];
								newTriangles[j+1] = i;
									
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
									
								j += 3;
							}
							
							if(i == VertecisFront.right[line])
							{
								line++;
							}
								
							
						}
							
						#endregion Front
						break;

					case 2:				
						#region Region Left
							
						for(int i = VerticesLeft.first; i < VerticesLeft.last; i++)
						{
								
							#region Region Debug
							//Debug.Log ("i: " + i + " j: " + j + " Line: " + line);
							#endregion Debug
								
							int A = (VerticesLeft.right[line] + 1)- VerticesLeft.left[line];
							int B = (VerticesLeft.right[line + 1] + 1) - VerticesLeft.left[line + 1];
							int VerticesDifference = (A - B) /2	+ B;
								
							#region Region Debug
							//Debug.Log("Left.Right: " + VerticesLeft.right[line] + " Left.Left: " + VerticesLeft.left[line]);
							//Debug.Log("A: " + A + " B: " + B + " VerticesDifference:" + VerticesDifference);
							#endregion Debug
							
							if(i + 1 == VerticesLeft.right[line] && i + 2 == VerticesLeft.last)
							{
								newTriangles[j] = i + 1;
								newTriangles[j+1] = i + 2;
								newTriangles[j+2] = i;
								
								j += 3;
									
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
								
								face++;								
								line = 0;
								break;
							}
							else if((i > VerticesLeft.left[line] && i < VerticesLeft.right[line] - 1) 
								|| ((i == VerticesLeft.left[line] || i == VerticesLeft.right[line] - 1) && A == B ))
							{
								newTriangles[j] = i + VerticesDifference;
								newTriangles[j+2] = i + VerticesDifference + 1;
								newTriangles[j+1] = i;
								
								newTriangles[j+5] = i;
								newTriangles[j+3] = i + 1;
								newTriangles[j+4] = i + VerticesDifference + 1;
									
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								//Debug.Log("newTriangles[j+3]: " + newTriangles[j+3] + " newTriangles[j+4]: " + newTriangles[j+4] + " newTriangles[j+5]: " + newTriangles[j+5]);	
								#endregion Debug
								
								j += 6;
							}
							else if(i == VerticesLeft.left[line])
							{
								newTriangles[j] = i + 1;
								
								if(line + 1 < VerticesLeft.lines)
								{
									newTriangles[j+1] = VerticesLeft.left[line + 1];
								}
								else
								{
									newTriangles[j+1] = VerticesLeft.last;
								}	
									
								newTriangles[j+2] = i;
								
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
									
								j += 3;
							}
							else if(i == VerticesLeft.right[line] - 1)
							{
								newTriangles[j] = i + 1;
								newTriangles[j+1] = VerticesLeft.right[line + 1];
								newTriangles[j+2] = i;
								
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
									
								j += 3;
							}
							else if(i == VerticesLeft.right[line])
							{
								line++;
							}
						}
						
						#endregion Left
						break;

					case 3:
						#region Region Back
							
						for(int i = VerticesBack.first; i < VerticesBack.last; i++)
						{
							#region Region Debug
							//Debug.Log ("i: " + i + " j: " + j + " Line: " + line);
							#endregion Debug
							
							int A = (VerticesBack.right[line] + 1)- VerticesBack.left[line];
							int B = (VerticesBack.right[line + 1] + 1) - VerticesBack.left[line + 1];
							int VerticesDifference = (A - B) /2	+ B;
							
							if(i + 1 == VerticesBack.right[line] && i + 2 == VerticesBack.last)
							{
								newTriangles[j] = i + 1;
								newTriangles[j+1] = i + 2;
								newTriangles[j+2] = i;
								
								j += 3;
								
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
								
								face++;								
								line = 0;
								break;
							}
							else if((i > VerticesBack.left[line] && i < VerticesBack.right[line] - 1) 
								|| ((i == VerticesBack.left[line] || i == VerticesBack.right[line] - 1) && A == B ))
							{
								newTriangles[j] = i + VerticesDifference;
								newTriangles[j+2] = i + VerticesDifference + 1;
								newTriangles[j+1] = i;
								
								newTriangles[j+5] = i;
								newTriangles[j+3] = i + 1;
								newTriangles[j+4] = i + VerticesDifference + 1;
								
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								//Debug.Log("newTriangles[j+3]: " + newTriangles[j+3] + " newTriangles[j+4]: " + newTriangles[j+4] + " newTriangles[j+5]: " + newTriangles[j+5]);	
								#endregion Debug	
									
								j += 6;
							}
							else if(i == VerticesBack.left[line])
							{
								newTriangles[j] = i + 1;									
								
								if(line + 1 < VerticesBack.lines)
								{
									newTriangles[j+1] = VerticesBack.left[line + 1];
								}
								else
								{
									newTriangles[j+1] = VerticesBack.last;
								}	
									
								newTriangles[j+2] = i;
									
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
								
								j += 3;
							}
							else if(i == VerticesBack.right[line] - 1)
							{
								newTriangles[j] = i + 1;
								newTriangles[j+1] = VerticesBack.right[line + 1];
								newTriangles[j+2] = i;
								
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
									
								j += 3;
							}
							else if(i == VerticesBack.right[line])
							{
								line++;
							}
						}					
						
						#endregion Back							
						break;

					case 4:
						#region Region Right
							
						for(int i = VerticesRight.first; i < VerticesRight.last; i++)
						{
							#region Region Debug
							//Debug.Log ("i: " + i + " j: " + j + " Line: " + line);
							#endregion Debug
								
							int A = (VerticesRight.right[line] + 1)- VerticesRight.left[line];
							int B = (VerticesRight.right[line + 1] + 1) - VerticesRight.left[line + 1];
							int VerticesDifference = (A - B) /2	+ B;
							
							if(i + 1 == VerticesRight.right[line] && i + 2 == VerticesRight.last)
							{
								newTriangles[j] = i + 1;
								newTriangles[j+2] = i + 2;
								newTriangles[j+1] = i;
								
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
									
								j += 3;								
								
								face++;								
								line = 0;
								break;
							}
							else if((i > VerticesRight.left[line] && i < VerticesRight.right[line] - 1) 
								|| ((i == VerticesRight.left[line] || i == VerticesRight.right[line] - 1) && A == B ))
							{
								newTriangles[j] = i + VerticesDifference;
								newTriangles[j+1] = i + VerticesDifference + 1;
								newTriangles[j+2] = i;
								
								newTriangles[j+4] = i;
								newTriangles[j+3] = i + 1;
								newTriangles[j+5] = i + VerticesDifference + 1;
									
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								//Debug.Log("newTriangles[j+3]: " + newTriangles[j+3] + " newTriangles[j+4]: " + newTriangles[j+4] + " newTriangles[j+5]: " + newTriangles[j+5]);	
								#endregion Debug
								
								j += 6;
							}
							else if(i == VerticesRight.left[line])
							{
								newTriangles[j] = i + 1;
									
								if(line + 1 < VerticesRight.lines)
								{
									newTriangles[j+2] = VerticesRight.left[line + 1];
								}
								else
								{
									newTriangles[j+2] = VerticesRight.last;
								}	
									
								newTriangles[j+1] = i;
								
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
									
								j += 3;
							}
							else if(i == VerticesRight.right[line] - 1)
							{
								newTriangles[j] = i + 1;
								newTriangles[j+2] = VerticesRight.right[line + 1];
								newTriangles[j+1] = i;
								
								#region Region Debug
								//Debug.Log("j: " + j + " newTriangles[j]: " + newTriangles[j] + " newTriangles[j+1]: " + newTriangles[j+1] + " newTriangles[j+2]: " + newTriangles[j+2]);
								#endregion Debug
									
								j += 3;
							}
							else if(i == VerticesRight.right[line])
							{
								line++;
							}
						}
						
						#endregion Right
						break;
						
					case 5:	//no pyramid face
						
						break;
					}
				}
				
				#endregion pyramid
				break;
			}

	}
	
	
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
