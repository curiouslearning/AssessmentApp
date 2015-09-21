using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleBasket : Observer{

	public List<Subject> subjects;
	public Sprite basketSprite;
	public Sprite noSprite;
	// Use this for initialization
	void Start () {
		addSelfToSubjects();
	
	}

	void addSelfToSubjects()
	{
		for (int i = 0; i < subjects.Count; i++)
		{
			subjects[i].addObserver(new Subject.GameObjectNotify(this.onNotify));
		}
	}

	public void registerWithSoo(GameObject SOO)
	{
		SOO.GetComponent<Subject>().addObserver(new Subject.GameObjectNotify(this.onNotify));
	}

	void onNotify (EventInstance<GameObject> e)
	{
		if (e.type == eType.Selected || e.type == eType.TimedOut)
		{
			this.GetComponent<SpriteRenderer>().sprite = null;
		}
		if (e.type == eType.Ready)
		{
			this.GetComponent<SpriteRenderer>().sprite = basketSprite;
		}
	}	
}
