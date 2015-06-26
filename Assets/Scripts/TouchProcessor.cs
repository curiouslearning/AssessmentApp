using UnityEngine;
using System.Collections;

public class TouchProcessor : MonoBehaviour {
	float screenPoint;
	Vector3 startPos;
	Vector3 curScreenPoint;
	Vector3 curPos;
	Vector3 offset;
	GameObject selection;
	Transform parentBuffer;
	public GameObject target;
	public float distanceMod;

	// Use this for initialization
	void Start () {
		selection = null;
		screenPoint = 0;
	
	}
	
	void Update () {
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began)
			{
				RaycastHit2D touchHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), -Vector2.up);
				if(touchHit.collider != null)
				{
					startPos= Camera.main.ScreenToWorldPoint(touch.position);
					startPos.z = 0;
					selection = touchHit.transform.gameObject;
					parentBuffer = selection.transform.parent;
					selection.transform.parent = null;
					Debug.Log(selection.gameObject.name); //debugger
					offset= selection.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, screenPoint));
				}
			
			}
			else if (touch.phase == TouchPhase.Moved||touch.phase == TouchPhase.Stationary)
			{
				if(selection != null)
				{
					curScreenPoint = new Vector3(touch.position.x, touch.position.y, screenPoint);
					curPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
					selection.transform.position = curPos;
					//selection.GetComponent<StimulusScript>().scaleToTarget(Vector3.Distance(selection.transform.position, target.transform.position)*distanceMod);
				}
					
			}

			if (touch.phase == TouchPhase.Ended)
			{	
				if(selection != null)
				{
					selection.transform.position = selection.GetComponent<StimulusScript>().returnHomePos();
					//selection.GetComponent<StimulusScript>().resetScale();
					selection.transform.parent = parentBuffer;
					selection = null;
				}
				
			}
		}
	
	}
}
