//Created by Max Merchel

using UnityEngine;
using System.Collections;

public class SpawnRay : MonoBehaviour {
	
	public GameObject PaticleRay;
	
	private GameObject PaticleClone;
	private Ray ray;
	
	private bool Load = false;
	
	
	private Vector3 ObjectPosition;	//Position of the object	
	
	// Use this for initialization
	void Start () 
	{
		ObjectPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		        
		
		if (Input.GetButtonDown("Fire2"))
		{
			
			//if (Physics.Raycast(ray))
			//{				
                PaticleClone = Instantiate(PaticleRay, ObjectPosition, Quaternion.LookRotation(ray.direction)) as GameObject;		
				Load = true;
					
			//}
		}
		
		if(Load)
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			PaticleClone.transform.LookAt(ray.GetPoint(20.0f));
			
			#region Region Debug			
			//Debug.DrawRay(ObjectPosition,ray.direction, Color.green, 4.0f);
			//Debug.Log(ray.direction +", "+ Quaternion.Euler(ray.direction));
			//Debug.Log(ray.direction +", "+ RayQuaternion);
			#endregion
		}
		
		
	    if (Input.GetButtonUp("Fire2"))
		{
			Load = false;	
			Destroy(PaticleClone);	
		}
	
	}
}
