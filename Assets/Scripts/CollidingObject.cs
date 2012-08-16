using UnityEngine;
using System.Collections;

public class CollidingObject : MonoBehaviour {
	
	
	public Vector3 Direction;
	
	
	// Use this for initialization
	void Start () {
		
	 	rigidbody.AddForce(Direction);
	}
	
}
