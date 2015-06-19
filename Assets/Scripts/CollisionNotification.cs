using UnityEngine;
using System.Collections;


// Attached to the Host's Receptacle and garbage collector. Destroys selected stimulus 
// and sends a "Selected" EventInstance with the stimulus to the observer list
public class CollisionNotification : MonoBehaviour {
	public Subject sub;
	public eType type;
	GameObject selected;
	void Awake ()
	{
		sub = GetComponent<Subject>();
	}
	void OnTriggerEnter2D (Collider2D col){ 
		selected = col.gameObject;
		EventInstance<GameObject> e = new EventInstance<GameObject>();
		e.setEvent(type, col);
		col.transform.parent.GetComponent<SOOScript>().releaseStim(col.gameObject);
		Destroy (col.gameObject);
		sub.notify(e);
	}
}
