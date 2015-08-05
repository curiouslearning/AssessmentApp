using UnityEngine;
using System.Collections;
using SmoothMoves;

public class AnimationManager : Observer {
	Animator animator;
	public GameObject[] subjects;
	public GameObject[] bodyParts;
	GameManagerScript gmHolder;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		addSelfToSubjects();
		gmHolder = GameObject.Find ("Main Camera").GetComponent<GameManagerScript>();
	
	}
	void addSelfToSubjects()
	{
		GameObject temp;
		for (int i = 0; i < subjects.Length; i++)
		{
			temp = subjects[i];
			temp.GetComponent<Subject>().addObserver(new Subject.GameObjectNotify(this.onNotify));
			
		}
	}

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
	void changeBodyPart (int part, Texture2D newTexture)
	{
		GameObject temp = bodyParts[part];
		temp.GetComponent<SkinnedMeshRenderer>().material.mainTexture = newTexture;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
