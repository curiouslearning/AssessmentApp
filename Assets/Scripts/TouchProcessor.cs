using UnityEngine;
using System.Collections;

public class TouchProcessor : MonoBehaviour {
	float screenPoint;
	Vector3 startPos;
	Vector3 curScreenPoint;
	Vector3 curPos;
	Vector3 offset;
	GameObject selection;

	// Use this for initialization
	void Start () {
		selection = null;
		screenPoint = 0;
	
	}
	
	void Update () {
		Debug.Log("fixed");
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began)
			{
				Debug.Log("began");
				startPos= Camera.main.ScreenToWorldPoint(touch.position);
				startPos.z = 0;
				RaycastHit2D touchHit = Physics2D.Raycast(transform.position, -Vector2.up);
				if(touchHit.collider != null)
				{
					selection = touchHit.transform.gameObject;
					Debug.Log(selection.gameObject.name); //debugger
					offset= selection.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, screenPoint));
				}
			
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				Debug.Log("moved");
				if(selection != null)
				{
					curScreenPoint = new Vector3(touch.position.x, touch.position.y, screenPoint);
					curPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
					selection.transform.position = curPos;
				}
					
			}
		}
		if (Input.touchCount == 0)
		{	
				Debug.Log("ended");
				selection = null;
				
		}
		Debug.Log("out of for loop");
	
	}
}
