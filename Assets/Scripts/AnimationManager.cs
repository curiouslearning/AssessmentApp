using UnityEngine;
using System.Collections;
using SmoothMoves;

/// <summary>
/// Animation manager
/// A component for GameObjects that contain animations. Monitors the event
// system and organizes the firing of animations and changing of customizable options.
// Extends Observer.
/// </summary>
public class AnimationManager : Observer {
	Animator animator;
	public GameObject[] subjects;
	public GameObject[] bodyParts;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		addSelfToSubjects();
	
	}
	/// <summary>
	/// Function for initializing the Observer design pattern.
	//  NOTE: ANY NEW SUBJECT MUST BE MANUALLY INSERTED INTO THE ARRAY EITHER IN THE EDITOR OR IN START()
	/// </summary>
	void addSelfToSubjects()
	{
		GameObject temp;
		for (int i = 0; i < subjects.Length; i++)
		{
			temp = subjects[i];
			temp.GetComponent<Subject>().addObserver(new Subject.GameObjectNotify(this.onNotify));
			
		}
	}
	/// <summary>
	/// Overridden method for handling events this class is listening for
	/// </summary>
	/// <param name="e">E.</param>
	public override void onNotify (EventInstance<GameObject> e)
	{
		if(e.type == eType.Selected && e.signaler.GetComponent<StimulusScript>().isOption())
		{
			int bodyPart = e.signaler.GetComponent<StimulusScript>().getBodyPart();
			Texture2D newTexture = Resources.Load<Texture2D>("Textures/" + e.signaler.GetComponent<StimulusScript>().getTextureName());
			changeBodyPart( bodyPart, newTexture);
			//grab texture info and send it to swapper	
		}
		else if (e.type == eType.Selected && !e.signaler.GetComponent<StimulusScript>().isOption())
		{
			animator.SetTrigger("Success");
		}
	}
	/// <summary>
	/// Changes the body part.
	/// </summary>
	/// <param name="part">Index of the bone to be changed in bodyParts.</param>
	/// <param name="newTexture"> Replacement Texture.</param>
	void changeBodyPart (int part, Texture2D newTexture)
	{
		GameObject temp = bodyParts[part];
		temp.GetComponent<SkinnedMeshRenderer>().material.mainTexture = newTexture;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
