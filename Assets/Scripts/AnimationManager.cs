using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Animation manager Extends Observer.
/// A component for GameObjects that contain animations. Monitors the event
/// system and organizes the firing of animations and changing of customizable options.
/// </summary>
public class AnimationManager : Observer {
	public  Animator animator;
	public Subject eventHandler;
	public  ScoreTracker scoreTracker;
	public GameObject[] subjects;
	public Category currentCategory;
	public Selectable touchDetect;
//	public SpriteRenderer talkBubble;
//	public Sprite talkBubbleSprite;

    /// <summary>
    /// create dictionaries, initialize main character
    /// </summary>

    void Awake () {
		Application.targetFrameRate = 15;
		animator = GetComponent<Animator>();
		touchDetect.initP (this.onSelect);
	}

    /// <summary>
    /// call addSelfToSubjects, ensure scoreTracker finds "Main Camera"
    /// </summary>

	void Start()
	{	
		eventHandler = GetComponent<Subject> ();	
		addSelfToSubjects();
		scoreTracker = GameObject.Find("Main Camera").GetComponent<ScoreTracker>();
	}

	/// <summary>
	/// Function for initializing the Observer design pattern.
	/// NOTE: ANY NEW SUBJECT MUST BE MANUALLY INSERTED INTO THE ARRAY EITHER IN THE EDITOR OR IN START()
	/// </summary>
    
	public virtual void addSelfToSubjects()
	{
		GameObject temp;
		for (int i = 0; i < subjects.Length; i++)
		{
			temp = subjects[i];
			temp.GetComponent<Subject>().addObserver(new Subject.GameObjectNotify(this.onNotify));
			temp.GetComponent<Subject>().addObserver(new Subject.boolNotify(this.onNotify));
			
		}
	}	
		


	/// <summary>
	/// Chooses a random animation from an array of strings
	/// </summary>
	/// <param name="s">S.</param>
	protected virtual void randomAnimation (string[] s)
	{
		int val = Random.Range(0, s.Length);
		Debug.Log ("using animation: " + s [val]);
		startAnimation (s [val]);
		return;
	}

	/// <summary>
	/// Starts the given animation.
	/// </summary>
	/// <param name="s">S.</param>
	//TODO: refactor for polymorphism
	protected virtual void startAnimation(string s)
	{
		Debug.Log ("calling startAnimation with string: " + "\"" + s + "\"");
		switch (s) {
		default:
			animator.SetTrigger (s);
			break;	
		}
	}
	public virtual void onSelect (touchInstance t)
	{
		Debug.LogError ("base case!");
	}
}
