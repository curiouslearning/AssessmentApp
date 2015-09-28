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
		if(col.gameObject.tag != dragTag){ //prevent unwanted collisions from affecting gameplay
			return;
		}
		selected = col.gameObject;
		if (dragTag == "Stimulus" && selected.GetComponent<StimulusScript>().Equals(null))
		{
			sub.sendBoolEvent(eType.Selected, selected.GetComponent<TokenScript>().returnIsCorrect());
			Destroy(selected.gameObject);
		}
		else {
			sub.sendEvent(type);			
		}
		selected = null;
	}
}
