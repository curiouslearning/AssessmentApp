using UnityEngine;
using System.Collections;

//class for Object Deleter functionality
public class CatchAndDelete : MonoBehaviour {
	void OnTriggerEnter2D (Collider2D collisionObject){
		Destroy(collisionObject.gameObject);
	}
}
