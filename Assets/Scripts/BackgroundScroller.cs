using UnityEngine;
using System.Collections;

public class BackgroundScroller : Observer {

	public float scrollSpeed;
	public float tileSizeZ;
	public bool scrolling;
	
	private Vector3 startPostition;
	// Use this for initialization
	void Start () {
		startPostition = transform.position;
		GameObject.Find ("Main Camera").GetComponent<ScoreTracker>().eventHandler.addObserver(new Subject.GameObjectNotify(this.onNotify));
		GameObject.Find ("Receptacle").GetComponent<CollisionNotification>().sub.addObserver(new Subject.GameObjectNotify(this.onNotify));
		GameObject.Find ("Receptacle").GetComponent<CollisionNotification>().sub.addObserver(new Subject.boolNotify(this.onNotify));
	}
	
	public bool isScrolling()
	{
		return scrolling;
	}
	
	void toggleScroll (bool b)
	{
			scrolling = b;
	}

	public override void onNotify(EventInstance<GameObject> e)
	{
		if (e.type == eType.Ready)
		{ 
			toggleScroll(false);
		}
		else if (e.type == eType.Transition|| e.type == eType.TimedOut)
		{
			toggleScroll(true);
		}
	} 
	public override void onNotify (EventInstance<bool> e)
	{
		if(e.type == eType.Transition)
			toggleScroll(true);
	}

	public override void registerGameObjectWithSoo(GameObject SOO)
	{
		base.registerGameObjectWithSoo(SOO);
	}
	
	// Update is called once per frame
	void Update () {
		if (scrolling){
			float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
			transform.position = startPostition + Vector3.left * newPosition;	
		}
	}
}
