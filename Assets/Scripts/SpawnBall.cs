using UnityEngine;
using System.Collections;

public class SpawnBall : MonoBehaviour {
	
	
	public GameObject Ball;
	
	public float MinStartImpulse = 20.0f;
	public float MaxStartImpulse = 250.0f;
	private float StartImpulse;
	public float ImpulseLoadStrange = 1.0f;

	private Vector3 ObjectPosition;	//Position of the object	
	private bool Load = false;
	
	
	// Use this for initialization
	void Start () 
	{
		ObjectPosition = transform.position;
		StartImpulse = MinStartImpulse;
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		
		if (Input.GetButtonDown("Fire1"))
		{
			Load = true;
		}
		if(Load)
		{
			if(StartImpulse < MaxStartImpulse)
			StartImpulse += ImpulseLoadStrange;
		}
		
	    if (Input.GetButtonUp("Fire1"))
		{
			Load = false;	
			
			GameObject BallClone;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			#region Region Debug
			
			//Debug.DrawRay(ObjectPosition,ray.direction, Color.green, 4.0f);
			//Debug.Log(ray.direction +", "+ Quaternion.Euler(ray.direction));
			//Debug.Log(ray.direction +", "+ RayQuaternion);
			Debug.Log(StartImpulse);
			#endregion
			
            if (Physics.Raycast(ray))
			{				
                BallClone = Instantiate(Ball, ObjectPosition, Quaternion.LookRotation(ray.direction)) as GameObject;		
				BallClone.rigidbody.AddRelativeForce(0.0f,0.0f,StartImpulse,ForceMode.Impulse);			
				Destroy(BallClone, 10.0f);
				
				StartImpulse = MinStartImpulse;				
			}
			
		}
	}
}
