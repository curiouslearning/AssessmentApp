using UnityEngine;
using System.Collections;

public class Draggable : MonoBehaviour {
	float screenPoint;
	Vector3 startPos;
	Vector3 curScreenPoint;
	Vector3 curPos;
	Vector3 offset;
	// Use this for initialization
	void Start () {
		screenPoint = 0;
	
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
				Debug.Log("moved");
				curScreenPoint = new Vector3(touch.position.x, touch.position.y, screenPoint);
				curPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
				transform.position = curPos;		
			}
		
			else if (touch.phase == TouchPhase.Ended)
			{	
				Debug.Log("ended");	
			}
		}		
	}
}
