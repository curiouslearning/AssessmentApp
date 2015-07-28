using UnityEngine;
using System.Collections;

public class AnimationManager : Observer {
	Animator animator;
	public GameObject[] subjects;
	public GameObject[] bodyParts;
	public Material newTexture;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		addSelfToSubjects();
	
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
		animator.SetTrigger("Success");
	}
	void changeBodyPart (int part, Material newMat)
	{
		GameObject temp = bodyParts[part];
		temp.GetComponent<SkinnedMeshRenderer>().material = newMat;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
