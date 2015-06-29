using UnityEngine;
using System.Collections;

/* TouchProcessor
 * InputWrapper for touch based input
 * Contains functionality for dragging, tapping, stimlus snap-back
 * contains a Subject component for touch event functionality
 */
public class TouchProcessor : MonoBehaviour {
	float screenPoint;
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
				RaycastHit2D touchHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), -Vector2.up); //search for Stimuli under tap
				if(touchHit.collider != null)
				{
					selection = touchHit.transform.gameObject; 
					parentBuffer = selection.transform.parent;  //store and remove the parent to prevent weird parent-child behavior during dragging
					selection.transform.parent = null;
					Debug.Log(selection.gameObject.name); //debugger
					offset= selection.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, screenPoint));
				}
			
			}
			else if (touch.phase == TouchPhase.Moved||touch.phase == TouchPhase.Stationary)
			{
				if(selection != null)
				{ //translate current finger position to new object position  
					curScreenPoint = new Vector3(touch.position.x, touch.position.y, screenPoint);
					curPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
					selection.transform.position = curPos;
					//scale size of object with respect to the size of the Distance between its current position and the host's receptacle
					selection.GetComponent<StimulusScript>().scaleToTarget(Vector3.Distance(selection.transform.position, target.transform.position)*distanceMod);
				}
					
			}

			if (touch.phase == TouchPhase.Ended)
			{	
				if(selection != null) //if object was not deleted
				{
					//return and rescale object, add it back to SOO as a child
					selection.transform.position = selection.GetComponent<StimulusScript>().returnHomePos();
					selection.GetComponent<StimulusScript>().resetScale();
					selection.transform.parent = parentBuffer;
					selection = null;
				}
				
			}
		}
	
	}
}
