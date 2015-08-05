using UnityEngine;
using System.Collections;



/// <summary>
/// Collision notification.
/// Attached to the Host's Receptacle and garbage collector. 
/// Destroys selected stimulus and sends an EventInstance with the stimulus and respective eType ("Selected" or "Trashed") to the observer list.
/// </summary>
public class CollisionNotification : MonoBehaviour {
	public Subject sub;
	public eType type;
	GameObject selected;
	public string dragTag;
	void Awake ()
	{
		sub = GetComponent<Subject>();
	}
	void OnTriggerEnter2D (Collider2D col){ 
		Debug.Log("caught: " + col.gameObject.name); //debugger
		if(col.gameObject.tag != dragTag){ //prevent unwanted collisions from affecting gameplay
			Debug.Log("oops!");
			return;
		}
		selected = col.gameObject;
		EventInstance<GameObject> e = new EventInstance<GameObject>();
		e.setEvent(type, selected);
		col.transform.parent = null;
		sub.notify(e);
		Debug.Log("notified!");
		selected = null;
	}
}
