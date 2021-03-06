//Created by Max Merchel

//not used

using UnityEngine;
using System.Collections;

public class CubeMeshConstruction : MonoBehaviour 
{

	private GameObject NewObject;			//GameObject for the New Object
	public string ObjectName = "New Cube";	//Name of the New GameObject
	
	public Vector3 ObjectCenter;		//Center of the new Object
	public Vector3 ObjectRotation;		//Rotation of the new Object
	private Quaternion RotationQuat;	//Rotation Quaternion of the new Object
	
	public float ObjectHeight = 10.0f;		//Height of the new Objec
	public float ObjectWidth = 10.0f;		//Width of the new Object
	public float ObjectDepth = 10.0f;		//Depth of the new Object
	
	public float MaxObjectMeshHeight = 1.0f;	//Max Mech section Height
	private float MeshHeight;
	public float MaxObjectMeshWidth  = 1.0f;	//Max Mesh section Width
	private float MeshWidth;
	public float MaxObjectMeshDepth  = 1.0f;	//Max Mesh section Width
	private float MeshDepth;
		
	private int SectionHeight;
	private int SectionWidth;
	private int SectionDepth;
	
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
	
	public enum Parts{one, three, six}; //1 = all sides the same Texture / 3 = opposite sides are the same Texture / 6 = 1 Texture for every side 
	public Parts TextureParts;
	private int TexturePartNumber;
	
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
				NewObject.AddComponent("MeshDeformation");
			}
			
			#region Region Debug
			//for( int i = 0; i < newVertices.Length; i++)
			//{
			//	Debug.DrawRay(newVertices[i],newVertices[i].normalized, Color.red, 20.5f);
			//}
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
		float HalfMeshDepth = ObjectDepth / 2;
		
		int length = ((SectionHeight * SectionWidth + SectionWidth * SectionDepth + SectionDepth * SectionHeight) * 2 + 2);

		newVertices = new Vector3[length];
		newUVs = new Vector2[length];
		
		int height = 0;
		int width = 0;
		int depth;
		bool odd;
		
		if((SectionDepth / 2) % 1 == 0)
		{
			depth = (SectionDepth / 2);
			odd = false;
		}
		else
		{			
			depth = ((SectionDepth + 1) / 2);
			odd = true;
		}
		
		int face = 0;
		
		for(int i = 0; i < length; i++)
		{
			#region Region switch(face)
			switch(face)
			{
				case 0:
					newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), HalfMeshDepth);
					
					//newUVs[i] = new Vector2(newVertices[i].x,newVertices[i].y);
					newUVs[i] = new Vector2((MeshHeight * height)/(SectionHeight * TexturePartNumber),(MeshWidth * width)/(SectionWidth));	
									
					if(width == SectionWidth)
					{		
						height++;
						
						if(height > SectionHeight)
						{
							face++;
							height = SectionHeight;
							depth--;
						}
					
					}
					width++;
					
					break;
				case 1:
					
					
					if(odd)
					{
						if(depth < 0)
						{
							newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth+0.5)/SectionDepth)*ObjectDepth) );	
						}
						else
						{
							newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth-0.5)/SectionDepth)*ObjectDepth) );	
						}
						

						if(width == SectionWidth)
						{
							depth--;
							
							if(depth == 0)
							{
								depth--;
							}
							
							if(depth < -((SectionDepth + 1) / 2))
							{
								face++;
								depth = -((SectionDepth + 1) / 2);
								
								height--;
							}
						}
					}
					else
					{
						
						newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth)/SectionDepth)*ObjectDepth) );	
						
						if(width == SectionWidth)
						{
							depth--;
							
							if(depth < -((SectionDepth) / 2))
							{
								face++;
								depth = -((SectionDepth) / 2);
								
								height--;
							}
						}
					}
					
					
					newUVs[i] = new Vector2(newVertices[i].x,newVertices[i].y);
					//newUVs[i] = new Vector2(1-(MeshDepth * depth)/SectionDepth, (MeshWidth * width)/SectionWidth);	

					
					width++;
					
					
					break;
				case 2:
					newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), -HalfMeshDepth);
					
					//newUVs[i] = new Vector2(newVertices[i].x,newVertices[i].y);
					newUVs[i] = new Vector2((MeshHeight * height)/(SectionHeight * TexturePartNumber),(MeshWidth * width)/(SectionWidth));	
					
					
					if(width == SectionWidth)
					{
						height--;
						
						if(height < 0)
						{
							face++;
							height = 0;
							depth++;
						}
						
					}
					
					width++;
					
					break;
				case 3:
					
					if(odd)
					{
						if(depth < 0)
						{
							newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth+0.5)/SectionDepth)*ObjectDepth) );	
						}
						else
						{
							newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth-0.5)/SectionDepth)*ObjectDepth) );	
						}
						

						if(width == SectionWidth)
						{
							depth++;
							
							if(depth == 0)
							{
								depth++;
							}
							
							if(depth >= ((SectionDepth + 1) / 2))
							{
								face++;
								depth = -((SectionDepth + 1) / 2)+ 1;
								height++;
							}
						}
					}
					else
					{
						
						newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth)/SectionDepth)*ObjectDepth) );	
						
						if(width == SectionWidth)
						{
							depth++;
							
							
							if(depth >= ((SectionDepth) / 2))
							{
								face++;
								depth = -((SectionDepth) / 2)+1;
								height++;
								width = -1;
							}
						}
					}
					
					newUVs[i] = new Vector2(newVertices[i].x,newVertices[i].y);
					//newUVs[i] = new Vector2(1-(MeshDepth * depth)/SectionDepth, (MeshWidth * width)/SectionWidth);	

					
					width++;
					
					break;
				case 4:
					#region Region Debug
					//Debug.Log ("i: " + i + " depth: " + depth + " height: " + height);
					#endregion Debug
					
					if(odd)
					{
						if(depth < 0)
						{
							newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth+0.5)/SectionDepth)*ObjectDepth) );	
						}
						else
						{
							newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth-0.5)/SectionDepth)*ObjectDepth) );	
						}
						
						depth++;
						
						if(depth == 0)
						{
							depth++;
						}
						
						
						if(depth >= ((SectionDepth + 1) / 2) )//+ 1)
						{
								depth = -((SectionDepth + 1) / 2)+ 1;
								height++;
						
							if(height == SectionHeight)
							{
								face++;
								height = 1;
								width = SectionWidth;
							}
						
						}						
					}
					else
					{
						
						newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth)/SectionDepth)*ObjectDepth) );	
						
						depth++;
						
						if(depth >= (SectionDepth / 2) )// + 1)
						{
							depth = -(SectionDepth / 2) + 1;
							height++;
							
							if(height == SectionHeight)
							{
								face++;
								height = 1;
								width = SectionWidth;
							}
						}

					}
					
					
					newUVs[i] = new Vector2(newVertices[i].x,newVertices[i].z);		
					
					#region Region Debug
					//Debug.Log ("i: " + i + " depth: " + depth + " height: " + height);
					#endregion Debug
					
					break;
				case 5:
					
					if(odd)
					{
						if(depth < 0)
						{
							newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth+0.5)/SectionDepth)*ObjectDepth) );	
						}
						else
						{
							newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth-0.5)/SectionDepth)*ObjectDepth) );	
						}
						
						depth--;
						
						if(depth == 0)
						{
							depth--;
						}
						
						
						if(depth <= -((SectionDepth + 1) / 2)  + 1)
						{
								depth = ((SectionDepth + 1) / 2)- 1;
								height++;
						
							if(height == SectionHeight)
							{
								face++;
								height = 1;
								width = SectionWidth;
							}
						
						}
					}
					else
					{
						
						newVertices[i] = new Vector3(((height * MeshHeight) - HalfMeshHeight),((width * MeshWidth) - HalfMeshWidth), (((float)(depth)/SectionDepth)*ObjectDepth) );	
						
						depth--;
						
						if(depth <= -(SectionDepth / 2) + 1)
						{
							depth = (SectionDepth / 2)- 1;
							height++;
							
							if(height == SectionHeight)
							{
								face++;
								height = 1;
								width = SectionWidth;
							}
						}

					}
					
					
					newUVs[i] = new Vector2(newVertices[i].x,newVertices[i].z);

					
					break;
			}
			#endregion switch(face)
			
			
			
			if(width > SectionWidth)
			{
				width = 0;
			}
			
				#region Region Debug
				//Debug.Log("Section Height: " + i + " Section Width: " + j);
				//Debug.Log("Number of Current Vertice: "+ (i * (SectionWidth+1) + j));
				//Debug.Log("Current Vertice: " + newVertices[(i * (SectionWidth+1) + j)]);
				//Debug.Log("Vertice[" + i + "][" + j + "]: " + newVertices[i][j]);
				#endregion
		}
		
		
		CalculateTriangles();	//Funktion
		
	}
	
	void CalculateTriangles()
	{
		
		int length = ((SectionHeight * SectionWidth) + (SectionWidth * SectionDepth) + (SectionDepth * SectionHeight)) * 12;
		
		newTriangles = new int[length];	
		
		int subtract = ((SectionDepth - 1) * (SectionHeight-   1)) * 2 + 1;
		
		#region Region Debug
		//Debug.Log("Triangle length: " + length);
		#endregion
		
		int j=0;
		int line = 0;

		#region for
		for(int i = 0; i < (((SectionHeight * SectionWidth + SectionWidth * SectionDepth + SectionDepth * SectionHeight + 2) * 2) - subtract)-1; i++)
		{
			#region Region Debug
			//Debug.Log("I: " + i);
			//Debug.Log("Mod: " +((i+1) % (SectionWidth+1)));
			#endregion
			
			if(((i+1) % (SectionWidth+1)) != 0)
			{
				if(line > 0 )
				{
					if(line < (2 * (SectionHeight + SectionDepth)-1) )
					{
						newTriangles[j] = i;
						newTriangles[j+1] = (SectionWidth + 1) + i;
						newTriangles[j+2] = i + 1;
						
						#region Region Debug
						//Debug.Log("Triangle j: " + newTriangles[j] + " ; " + newTriangles[(j + 1)] + " ; " + newTriangles[(j + 2)] );
						//Debug.Log("J: " + j + " Triangle j: " + newTriangles[j]);
						//Debug.Log("J: " + (j + 1) + " Triangle j: " + newTriangles[(j + 1)]);
						//Debug.Log("J: " + (j + 2) + " Triangle j: " + newTriangles[(j + 2)]);
						#endregion
						
						j+=3;
					}
					else
					{
						newTriangles[j] = i;
						newTriangles[j+1] = i - (line * (SectionWidth + 1));
						newTriangles[j+2] = i + 1;
						
						#region Region Debug
						//Debug.Log("Triangle j: " + newTriangles[j] + " ; " + newTriangles[(j + 1)] + " ; " + newTriangles[(j + 2)] );
						//Debug.Log("J: " + j + " Triangle j: " + newTriangles[j]);
						//Debug.Log("J: " + (j + 1) + " Triangle j: " + newTriangles[(j + 1)]);
						//Debug.Log("J: " + (j + 2) + " Triangle j: " + newTriangles[(j + 2)]);
						#endregion
						
						j+=3;
					}

					
					newTriangles[j] = i;
					newTriangles[j+1] = i + 1;
					newTriangles[j+2] = i + 1 - (SectionWidth + 1);		
					
					#region Region Debug
					//Debug.Log("Triangle j: " + newTriangles[j] + " ; " + newTriangles[(j + 1)] + " ; " + newTriangles[(j + 2)] );
					//Debug.Log("J: " + j + " Triangle j: " + newTriangles[j]);
					//Debug.Log("J: " + (j + 1) + " Triangle j: " + newTriangles[(j + 1)]);
					//Debug.Log("J: " + (j + 2) + " Triangle j: " + newTriangles[(j + 2)]);
					#endregion
					
					j+=3;				
					
				}
				else
				{
					
					newTriangles[j] = (SectionWidth + 1) + i;
					newTriangles[j+1] = i+1;	
					newTriangles[j+2] = i;								
					
					#region Region Debug
					//Debug.Log("Triangle j: " + newTriangles[j] + " ; " + newTriangles[(j + 1)] + " ; " + newTriangles[(j + 2)] );
					//Debug.Log("J: " + j + " Triangle j: " + newTriangles[j]);
					//Debug.Log("J: " + (j + 1) + " Triangle j: " + newTriangles[(j + 1)]);
					//Debug.Log("J: " + (j + 2) + " Triangle j: " + newTriangles[(j + 2)]);
					#endregion
					
					j+=3;
					
					newTriangles[j] = i;
					newTriangles[j+1] = i + 1;
					newTriangles[j+2] = i + 1 + ((2 * (SectionHeight + SectionDepth) - 1) * (SectionWidth + 1));		
										
					#region Region Debug
					//Debug.Log("Triangle j: " + newTriangles[j] + " ; " + newTriangles[(j + 1)] + " ; " + newTriangles[(j + 2)] );
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
		#endregion for
		
		
		#region Region wings
		bool left = true, uptriangle = false;
		int k = 0, deep = 0, k2=0;
		line = 0;
		int SectionsAround = (SectionHeight + SectionDepth) * 2;
		
		
		for(int i = 0; i < (4 * (SectionHeight * SectionDepth) - 1) - 1; i++)
		{
			uptriangle = false;
			
			if(((i+1) % (SectionDepth + 1)) != 0)
			{
				#region Region left
				if(left)
				{	
					
					if(line > 0)
					{
						
						
						if(line < SectionHeight)
						{
							uptriangle = true;
							
							if(deep == 0)
							{
								k = (SectionsAround - SectionDepth - deep) * (SectionWidth + 1) - (line * (SectionWidth + 1));								
								newTriangles[j+2] = k - (SectionWidth + 1);	
								
								if(deep + 1 < SectionDepth)
								{
									k2 = SectionsAround  * (SectionWidth + 1) + deep + ((line - 1) * (SectionWidth + 1));
									newTriangles[j] = k2;
								}
								else
							 	{
									k2 = ((line) * (SectionWidth + 1));
									newTriangles[j] = k2;
								}
								
							}
							else
							{
								k = SectionsAround  * (SectionWidth + 1) + deep -1 + ((line - 1) * (SectionWidth + 1));
								
								if(line + 1 == SectionHeight) 
								{
									newTriangles[j+2] = (SectionsAround - SectionDepth - deep - (line +1)) * (SectionWidth + 1);
								}
								else
								{
									newTriangles[j+2] = (SectionsAround * (SectionWidth+1)) + (deep - 1) + (line * (SectionWidth + 1)) ;
								}
								
									if(deep + 1 < SectionDepth)
									{
										k2 = k + 1;
										newTriangles[j] = k2;
									}
									else
								 	{
										k2 = ((line) * (SectionWidth + 1));
										newTriangles[j] = k2;
									}
								
							}
							
							newTriangles[j+1] = k;
						
							
	
							#region Region Debug
							
							//Debug.Log("K: " + k + " Deep: " + deep + " Line: " + line);
							//Debug.Log("Triangle: " + newTriangles[j] + " ; " + newTriangles[(j + 1)] + " ; " + newTriangles[(j + 2)]);
							//Debug.Log("J: " + j + " Triangle j: " + newTriangles[j]);
							//Debug.Log("J: " + (j + 1) + " Triangle j: " + newTriangles[(j + 1)]);
							//Debug.Log("J: " + (j + 2) + " Triangle j: " + newTriangles[(j + 2)]);
							#endregion
							
							j+=3;
							
						}
						
						if(deep == 0)
						{
							if(uptriangle)
							{
								newTriangles[j+1] = k2;
							}
							else
							{
								k = (SectionsAround - SectionDepth - deep) * (SectionWidth + 1) - (line * (SectionWidth + 1));	
								
								if(line == SectionHeight)
								{
									newTriangles[j+1] = k - (SectionWidth + 1);
								}
								else
								{								
									if(deep + 1 < SectionDepth)
									{
										newTriangles[j+1] = SectionsAround  * (SectionWidth + 1) + deep + ((line - 1) * (SectionWidth + 1));
															
										
									}
									else 
								 	{
										newTriangles[j+1] = ((line) * (SectionWidth + 1));
									}
								}
							}
	
							
							if(line - 1 == 0)
							{
								newTriangles[j+2] = k + 2 * (SectionWidth + 1);
							}
							else 
							{
								newTriangles[j+2] = (SectionsAround * (SectionWidth+1)) + (deep) + ((line - 2) * (SectionWidth + 1));
							}
							
							
							
						}
						else
						{
							if(uptriangle)
							{
								newTriangles[j+1] = k2;
							}
							else
							{								
								if(line == SectionHeight)
								{
									k = (SectionsAround - SectionDepth - deep - line) * (SectionWidth + 1);
								}
								else
								{							
									k = SectionsAround  * (SectionWidth + 1) + deep - 1 + ((line - 1) * (SectionWidth + 1));
								}
							
								if(deep + 1 < SectionDepth)
								{			
									if(line == SectionHeight)
									{
										newTriangles[j+1] = k - (SectionWidth + 1);
									}
									else
									{
										newTriangles[j+1] = k + 1;
									}
								}
								else
							 	{
									newTriangles[j+1] = ((line) * (SectionWidth + 1));
								}
							
							}
							
							if(line - 1 == 0) 
							{
								if(deep + 1 < SectionDepth)
								{
									newTriangles[j+2] = ((SectionsAround - SectionDepth) + (deep + 1)) * (SectionWidth + 1);
								}
								else
							 	{
									newTriangles[j+2] = 0;
								}	
							}
							else if(deep + 1 < SectionDepth)
							{								
								newTriangles[j+2] = (SectionsAround * (SectionWidth+1)) + (deep) + ((line - 2) * (SectionWidth + 1));
							}
							else
						 	{
								newTriangles[j+2] = ((line - 1) * (SectionWidth + 1));
							}
	
						}
						
						newTriangles[j] = k;	

				
						#region Region Debug
						//Debug.Log("K: " + k + " Deep: " + deep + " Line: " + line);
						//Debug.Log("Triangle: " + newTriangles[j] + " ; " + newTriangles[(j + 1)] + " ; " + newTriangles[(j + 2)]);
						//Debug.Log("J: " + j + " Triangle j: " + newTriangles[j]);
						//Debug.Log("J: " + (j + 1) + " Triangle j: " + newTriangles[(j + 1)]);
						//Debug.Log("J: " + (j + 2) + " Triangle j: " + newTriangles[(j + 2)]);
						#endregion
						
						j+=3;				
	
							
						deep++;

						
					}
					else 	// line == 0					
					{
						k = (SectionsAround - SectionDepth + deep) * (SectionWidth + 1);

					
						if(deep + 1 < SectionDepth)
						{
							newTriangles[j] = k + (SectionWidth + 1);
						}
						else
					 	{
							newTriangles[j] = 0;
						}
						
						newTriangles[j+1] = k;
						
						if(deep == 0)
						{							
							newTriangles[j+2] = k - (SectionWidth + 1);								
						}
						else
						{
							//newTriangles[j+2] = (SectionsAround * SectionWidth) + (deep - 1); //Fehler
							newTriangles[j+2] = SectionsAround  * (SectionWidth + 1) + deep - 1;
						}
						

						#region Region Debug
												
						//Debug.Log("K: " + k + " Deep: " + deep + " Line: " + line);
						//Debug.Log("Triangle: " + newTriangles[j] + " ; " + newTriangles[(j + 1)] + " ; " + newTriangles[(j + 2)]);
						//Debug.Log("J: " + j + " Triangle j: " + newTriangles[j]);
						//Debug.Log("J: " + (j + 1) + " Triangle j: " + newTriangles[(j + 1)]);
						//Debug.Log("J: " + (j + 2) + " Triangle j: " + newTriangles[(j + 2)]);
						#endregion
						
						j+=3;
						deep++;
					}

					
				}	
				#endregion Region left
				
				#region Region right
				else //right 
				{
					
				}
				#endregion Region right
			}
			else 	//modulo == 0
			{
				line++;
				deep = 0;
				
				if(line > SectionHeight)
				{
					line = 0;
					left = false;
				}
			}

			#region Region Debug
			//Debug.Log("I: " + i + " J: " + j + " Line: " + line + " Deep: " + deep);			
			#endregion
		}
		
		#endregion Region wings
		
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
		
		if((ObjectDepth / MaxObjectMeshDepth) % 1 == 0.0f)
		{
			SectionDepth = (int)(ObjectDepth / MaxObjectMeshDepth);
		}
		else
		{
			SectionDepth = (int)(ObjectDepth / MaxObjectMeshDepth) + 1;
		}
		MeshDepth = ObjectDepth / SectionDepth;	
	}
}
