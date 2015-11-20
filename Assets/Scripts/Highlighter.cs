﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to control the highlighting game object prefab. Always call Highlighter::reset when finished with animation
/// </summary>
public class Highlighter : Observer {

/* TODO:
 *	Put in reference to controller
 *	logic for receiving highlight requests
 * 	scale star-burst to parent
 */
	Animator controller;
	//triggering variables
	public List<Subject> subjects;
	public eType[] start;
	public eType[] stop;
	void Awake () {
		controller = this.GetComponent<Animator>();
		//this.gameObject.SetActive(false);
	}

	void Start ()
	{
		for (int i = 0; i < subjects.Count; i++)
		{
			subjects[i].addObserver(new Subject.GameObjectNotify (this.onNotify));
		}
	}
	bool isPlaying ()
	{
		return (controller.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("Highlight")|| controller.GetAnimatorTransitionInfo(0).anyState);
	}
	/// <summary>
	/// Begin cycling the highlight animation.
	/// </summary>
	public void highlight ()
	{
		this.transform.localPosition = new Vector3 (0f,0.1f,0f); //make sure highlight is centered over parent
		//this.gameObject.SetActive(true);
		if(isPlaying ()) //prevent multiple calls from interrupting animation
		{
			return;
		}
		else
		{
			controller.SetBool("Highlight", true);
		}
		
	}
	/// <summary>
	/// Reset this instance.
	/// </summary>
	public void reset () 
	{
		controller.SetBool("Highlight", false);
		//this.gameObject.SetActive(false);
	}
	bool listContains (eType e, eType[] list)
	{
		for (int i = 0; i < list.Length; i++)
		{
			if (e == list[i])
				return true;
		}
		return false; 
	}	
	public override void onNotify (EventInstance<GameObject> e)
	{
		if (listContains (e.type, start))
		{
			highlight();
		}
		else if (listContains(e.type, stop))
		{
			reset();
		}
	}
}
