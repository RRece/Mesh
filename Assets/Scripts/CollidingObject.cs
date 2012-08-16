using UnityEngine;
using System.Collections;

public class CollidingObject : MonoBehaviour {
	
	
	public Vector3 Direction = new Vector3(0.0f,500.0f,2000.0f);
	
	
	// Use this for initialization
	void Start () {
		
	 	rigidbody.AddForce(Direction);
	}
	
}
