using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to control the highlighting game object prefab. Always call Highlighter::reset when finished with animation
/// </summary>
public class Highlighter : Observer {

	Animator controller;
	//triggering variables
	public List<Subject> subjects;
	public eType[] start;
	public eType[] stop;
	public bool active;
	bool running; //coroutine failsafe;
	public Renderer r;
	Subject.GameObjectNotify eventHandler;

	void Awake () {
		controller = this.GetComponent<Animator>();
		if (r == null) {
			r = GetComponent<Renderer> (); //default to renderer on this gameobject
		}
		active = false;
		running = false; 
		eventHandler = this.onNotify;
	}

	void Start ()
	{
		if (subjects != null) {
			registerSubjects ();
		}
	}

	/// <summary>
	/// Initialize the subject list of a highlighter instance
	/// </summary>
	/// <param name="list">List.</param>
	public void registerSubjects (Subject[] list)
	{
		if (list == null) {
			throw new System.ArgumentNullException ("your subject list is null!");
		}
		subjects.AddRange (list);
		registerSubjects ();
	}

	//internal member, assumes subjects is not null
	void registerSubjects()
	{
		for (int i = 0; i < subjects.Count; i++)
		{
			try{
				subjects[i].addObserver(eventHandler);
			}
			catch(System.Exception e) {
				Debug.Log (e + " on: " + this.gameObject.name + " at subjects index: " + i);
			}
		}
	}
	/// <summary>
	/// Toggles whether or not this highlighter is active.
	/// </summary>
	/// <param name="b">If set to <c>true</c> b.</param>
	public void toggleActive(bool b)
	{
		active = b;
	}
	/// <summary>
	/// returns the activity state of the instance
	/// </summary>
	/// <returns><c>true</c>, if active was checked, <c>false</c> otherwise.</returns>
	public bool checkActive ()
	{
		return active;
	}
	/// <summary>
	/// Begin cycling the highlight animation.
	/// </summary>
	public void highlight ()
	{
		if (active) {
			running = true;
			StartCoroutine (Flash ());
		}
	}
	IEnumerator Flash (){
		while (running) {
			r.enabled = false;
			yield return new WaitForSeconds (.25f);
			r.enabled = true;
			yield return new WaitForSeconds (.25f);
		}
	}


	/// <summary>
	/// Reset this instance.
	/// </summary>
	public void reset () 
	{
		StopCoroutine (Flash());
		running = false;
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
			try{
			reset();
			}
			catch(System.Exception l)
			{
				Debug.Log ("Missing Reference: " + l.Source);
			}
		}
	}
	void OnDisable ()
	{
		for (int i = 0; i < subjects.Count; i++)
		{
			subjects [i].removeObserver (eventHandler);
		}
	}
}
