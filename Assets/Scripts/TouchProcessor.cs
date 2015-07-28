using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//class to store information about a given touch instance
public class touchInstance{
	eType type;
	float duration;
	string selectionName;
	public touchInstance(){
		type = eType.Init;
		duration =0;
		selectionName = "";
	}
//******************
// Getter Functions*
//******************
	public float getTime () {return duration;}
	public string getName () {return selectionName;}
	public eType getType() {return type;}
	public bool isInit()
	{
		return (type == eType.Init);
	}
//******************
// Setter Functions*
//******************
	public void addTime(float time )
	{
		duration += time;
	}
	public void storeSelection (string selected)
	{
		selectionName = selected;
	}
	public void setTouch (eType t)
	{
		type = t;
	}
}

//Information wrapper containing all information for a given question 
//wrapped up in an event and sent to the Game Manager/score handler
public class TouchSummary {
	public List<touchInstance> touchList;
	public int touchCount;
	public int dragCount;
	public int selectionCount;
	public TouchSummary () {
		touchList = new List<touchInstance>();
		touchCount = 0;
		dragCount = 0;
		selectionCount = 0;
	}
	public void addTouch (touchInstance t)
	{
		if(t.getType() == eType.Drag)
			dragCount++;
		else if (t.isInit())
			Debug.LogError("t.type was never updated!"); //error debugger
		else 
			touchCount++;
		if (t.getName() != "")
			selectionCount++;
		touchList.Add(t);
	}
	public void printList() //debugger
	{
		
		Debug.Log("\t\ttouchList: ");
		for (int i = 0; i < touchList.Count; i++)
		{
			Debug.Log("\t\t\ttype: " + touchList[i].getType() + "\n\t\t\ttime: " + touchList[i].getTime());
			Debug.Log("\t\t\tselection: " + touchList[i].getName());
		}
	}
}
/* TouchProcessor
 * InputWrapper for touch based input
 * Contains functionality for dragging, tapping, stimulus snap-back
 * contains a Subject component for touch event functionality
 * Directly coupled with ScoreTracker
 * Observes: 		(Update this list with each new addition to an Observer list
 * 			GameManagerScript
 */
public class TouchProcessor : Observer {
	float screenPoint;
	Vector3 curScreenPoint;
	Vector3 curPos;
	Vector3 offset;
	GameObject selection;
	Transform parentBuffer;
	public GameObject target; //the host's receptacle
	public float distanceMod; //sizing modifier
//Event Materials
	Subject eventWrapper;
	TouchSummary touchSum;
	touchInstance t;
	public ScoreTracker scoring;

	// Use this for initialization
	void Start () {
		selection = null;
		screenPoint = 0;
		touchSum = new TouchSummary();	
		parentBuffer = null;
		Camera.main.GetComponent<GameManagerScript>().GetComponent<Subject>().addObserver(new Subject.gameManagerNotify(this.onNotify));
	}

	public override void onNotify (EventInstance<GameManagerScript> e)
	{
		if (e.type == eType.EndGame) {
			return;
			Debug.Log ("adding a question"); //debugger
		} else if (e.type == eType.NewQuestion) {
			scoring.addTouch (touchSum);
			touchSum = new TouchSummary ();
			//send stuff to score tracker
		}
	}
	
	void Update () {
		for (int i = 0; i < Input.touchCount; i++)
		{
			if(t == null)
				t = new touchInstance(); //begin storing info about the touch instance
			Touch touch = Input.GetTouch(i);  
			if (touch.phase == TouchPhase.Began)
			{ 
				//search for an object directly under the finger
				RaycastHit2D touchHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), -Vector2.up);
				if(touchHit.collider != null) //we got a hit
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
				t.addTime(touch.deltaTime);
				if(selection != null)
				{
					if(touch.phase == TouchPhase.Moved && t.getTime() > 0.5f)
						t.setTouch(eType.Drag);
					//translate current finger position to new object position  
					curScreenPoint = new Vector3(touch.position.x, touch.position.y, screenPoint);
					curPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
					selection.transform.position = curPos;
					//scale size of object with respect to the Distance between its current position and the host's receptacle
					selection.GetComponent<StimulusScript>().scaleToTarget(Vector3.Distance(curPos, target.transform.position)*distanceMod);
				}
					
			}

			if (touch.phase == TouchPhase.Ended)
			{	
				if(t.isInit()){ //if t.type was not modified in a Drag phase
					t.setTouch(eType.Tap);
				}
				t.addTime(touch.deltaTime);
				if(selection != null) //if object was not placed in receptacle
				{
					t.storeSelection(selection.name);
					selection.GetComponent<Selectable>().onSelect(t); //notify the selection it has been touched
					//return and rescale object, add it back to SOO as a child
					selection.transform.position = selection.GetComponent<StimulusScript>().returnHomePos();
					selection.GetComponent<StimulusScript>().resetScale();
					selection.transform.parent = parentBuffer;
					selection = null;
				}
				touchSum.addTouch(t); //store touch instance data
				t = null;
			}
		}
	
	}
}
