using UnityEngine;
using System.Collections;

public class AnimationManager : Observer {
	Animator animator;
	public GameObject[] subjects;

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

	void onNotify (EventInstance<GameObject> e)
	{
		animator.SetTrigger("Success");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
