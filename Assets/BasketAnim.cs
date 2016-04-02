using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasketAnim : AnimationManager{
	Highlighter h;

	// Use this for initialization
	void Start () {
		base.addSelfToSubjects ();
		h = GetComponentInChildren<Highlighter> ();
		if (h == null) {
			Debug.Log ("null highlighter");
		}
	}

	public override void onNotify(EventInstance<GameObject> e)
	{
		if (e.type == eType.NewQuestion) {
			currentCategory = scoreTracker.queryCategory ();
			if (currentCategory == Category.Customization) {
				h.toggleActive (false);
			} else {
				h.toggleActive (true);
			}
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
