//Created by Max Merchel

using UnityEngine;
using System.Collections;

public class NotQuadraticMeshConstruction : MonoBehaviour {
	
	private GameObject NewObject;			//GameObject for the New Object
	public string ObjectName = "New Object";	//Name of the New GameObject
	
	public Vector3 ObjectCenter;		//Center of the new Object
	public Vector3 ObjectRotation;		//Rotation of the new Object
	private Quaternion RotationQuat;	//Rotation Quaternion of the new Object
	
	public enum category {tetrahedron, pyramid}
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
	
	public enum Parts{one, all}; //1 = all sides the same Texture / 3 = opposite sides are the same Texture / 6 = 1 Texture for every side 
	public Parts TextureParts;
	private int TexturePartNumber;
	
	private int[] faceAdd;
	
	private int face;
	private int facemulti;
	
	// Use this for initialization
	void Start () 
	{
		if(ObjectWidth > 0.0f && ObjectHeight > 0.0f && ObjectDepth > 0.0f)
		{
			CalculateMeshSize();			
			
			CreateObject();
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
			Debug.LogError ("No GameObejct created");
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
					case category.tetrahedron:
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
	
	
	void CreateMesh()
	{
		CreateMeshSections();
		
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
