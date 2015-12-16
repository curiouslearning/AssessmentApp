using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleBasket : Observer{

	public List<Subject> subjects;
	public Sprite basketSprite;
	public Sprite noSprite;
	Category currentCategory;
	ScoreTracker scoreTracker;
	GameObject highlight;
	//Tap support for MainCharacter
	public Selectable s;
	public AnimationManager m;
	// Use this for initialization
	void Start () {
		addSelfToSubjects();
		scoreTracker = GameObject.Find("Main Camera").GetComponent<ScoreTracker>();
		s.initP(m.onSelect);
		highlight = GetComponentInChildren<Highlighter>().gameObject;
		highlight.SetActive(false);
	}

	void addSelfToSubjects()
	{
		for (int i = 0; i < subjects.Count; i++)
		{
			subjects[i].addObserver(new Subject.GameObjectNotify(this.onNotify));
		}
	}

	public override void registerGameObjectWithSoo(GameObject SOO)
	{
		base.registerGameObjectWithSoo(SOO);
	}

	public override  void onNotify (EventInstance<GameObject> e)
	{
		if(e.type == eType.NewQuestion) 
		{
			currentCategory = scoreTracker.queryCategory();
			Debug.Log(currentCategory.ToString());
			if (currentCategory == Category.Customization)
			{
				highlight.SetActive(false);
			}	
		}
		if (e.type == eType.Selected || e.type == eType.TimedOut)
		{
			this.GetComponent<SpriteRenderer>().sprite = null;
		}
		if (e.type == eType.Ready && currentCategory != Category.Customization)
		{
			highlight.SetActive(true);
			this.GetComponent<SpriteRenderer>().sprite = basketSprite;
		}
	}
		
}
