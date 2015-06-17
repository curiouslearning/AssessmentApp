using UnityEngine;
using System.Collections;

//communicator class between the Trail prefab and InputProcessor.cs.
//Notifies TouchProcessing of collisions
//Destroys itself when DestroySelf() is called by TouchProcessing

public class CollisionNotification : MonoBehaviour {
	public static CollisionNotification instance;
	void Awake(){
		instance = this;
	}
	void OnTriggerEnter2D (Collider2D collisionObject){
		GameObject sliced = collisionObject.transform.root.gameObject;
	}
	void DestroySelf (){
		Destroy (this.gameObject);
	}
}
