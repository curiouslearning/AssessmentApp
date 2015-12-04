using UnityEngine;
using System.Collections;



/// <summary>
/// Collision notification.
/// Attached to the Host's Receptacle and garbage collector. 
/// Destroys selected stimulus and sends an EventInstance with the stimulus and respective eType ("Selected" or "Trashed") to the observer list.
/// </summary>
public class CollisionNotification : MonoBehaviour {
	public Subject sub;
	ScoreTracker track;
	public eType type;
	GameObject selected;
	public string dragTag;
	public string inactiveCategory; //the category that this particular receptacle will be inactive during. "None" maintains constant activity, "Test" is only on for Customization
	void Awake ()
	{
		sub = GetComponent<Subject>();
	}
	void Start()
	{
		track = GameObject.Find ("Main Camera").GetComponent<ScoreTracker>();
	}
	string updateCat()
	{
		Category cat = track.queryCategory();
		return cat.ToString();
	}
	void OnTriggerEnter2D (Collider2D col){
		if(updateCat() == inactiveCategory || (inactiveCategory == "Test" && updateCat() != "Customization"))
			return;
		 
		if(col.gameObject.tag != dragTag){ //prevent unwanted collisions from affecting gameplay
			return;
		}
		selected = col.gameObject;
		if (dragTag == "Stimulus" && selected.GetComponent<TokenScript>() != null)
		{
			sub.sendBoolEvent(eType.Selected, selected.GetComponent<TokenScript>().returnIsCorrect());
			Destroy(selected.gameObject);
		}
		else {
			sub.sendEvent(type, selected);			
		}
		selected = null;
	}
}
