using UnityEngine;
using System.Collections;


// Attached to the Host's Receptacle and garbage collector. Destroys selected stimulus 
// and sends a "Selected" EventInstance with the stimulus to the observer list
public class CollisionNotification : MonoBehaviour {
	public Subject sub;
	public eType type;
	GameObject selected;
	public string tag;
	void Awake ()
	{
		sub = GetComponent<Subject>();
	}
	void OnTriggerEnter2D (Collider2D col){ 
		Debug.Log("caught: " + col.gameObject.name); //debugger
		if(col.gameObject.tag != tag) //prevent unwanted collisions from affecting gameplay
			return;
		selected = col.gameObject;
		EventInstance<GameObject> e = new EventInstance<GameObject>();
		e.setEvent(type, selected);
		col.transform.parent = null;
		sub.notify(e);
		selected = null;
	}
}
