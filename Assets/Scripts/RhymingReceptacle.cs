using UnityEngine;
using System.Collections;

/// <summary>
/// Class to handle two receptacle rhyming category responses.
/// </summary>

public class RhymingReceptacle : MonoBehaviour{
	public RhymingReceptacle r2;
	public Subject sub;
	public eType type;
	public GameObject selected;
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
		if (dragTag == "Stimulus" && selected.GetComponent<TokenScript>() != null)
		{
			if(r2.selected != null)
			{
				bool result = (selected.GetComponent<TokenScript>().returnIsTarget() && r2.selected.GetComponent<TokenScript>().returnIsTarget());
				sub.sendBoolEvent(eType.Selected, result);
				Destroy(selected.gameObject);
				Destroy(r2.selected.gameObject);
			}
		}	
		selected = null;
	}
}
