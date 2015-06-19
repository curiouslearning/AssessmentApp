using UnityEngine;
using System.Collections;

public class Draggable : MonoBehaviour {
	float screenPoint;
	Vector3 startPos;
	Vector3 curScreenPoint;
	Vector3 curPos;
	Vector3 offset;
	float touchTime;
	float touchStart;
	Subject eManager;
	EventInstance<Draggable> e;
	// Use this for initialization
	void Start () {
		screenPoint = 0;
		touchTime = 0;
		e = new EventInstance <Draggable>();
	}

	public void onSelect (Vector3 position) {
		startPos= Camera.main.ScreenToWorldPoint(position);
		offset= transform.position - Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, screenPoint));
		
	}	
	public void onDrag (Vector3 position) {
		curScreenPoint = new Vector3(position.x, position.y, screenPoint);
		curPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPos;
	}
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			touchStart = Time.time;
			if (touch.phase == TouchPhase.Began)
			{
				Debug.Log("began");
				startPos= Camera.main.ScreenToWorldPoint(touch.position);
				startPos.z = 0;
				Debug.Log(gameObject.name); //debugger
				offset= transform.position - Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, screenPoint));	
				
			}
			else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
			{
				//e.setEvent(eType.Dragged, this);
				//eManager.notify(e);
				Debug.Log("moved");
				curScreenPoint = new Vector3(touch.position.x, touch.position.y, screenPoint);
				curPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
				transform.position = curPos;	
			}
		
			else if (touch.phase == TouchPhase.Ended)
			{	
				Debug.Log("ended");	
				if (touchTime < 0.5)
				{
					//send a tap message
					Debug.Log("quicktap");
					//e.setEvent(eType.Tapped, this);
				}
			}
		}
		touchTime = Time.time - touchStart;		
	}
}
