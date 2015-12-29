using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TouchProcessor
/// InputWrapper for touch based input
/// Contains functionality for dragging, tapping, stimulus snap-back
/// contains a Subject component for touch event functionality
/// Directly coupled with ScoreTracker
/// </summary>

/* Observes: 		(Update this list with each new addition to an Observer list
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
	public Subject eventWrapper;
	TouchSummary touchSum;
	touchInstance t;
	public ScoreTracker scoring;

	// Use this for initialization
	void Start () {
		selection = null;
		screenPoint = 0;
		touchSum = new TouchSummary();	
		parentBuffer = null;
		Camera.main.GetComponent<ScoreTracker>().GetComponent<Subject>().addObserver(new Subject.scoreTrackerNotify(this.onNotify));
	}

	public override void onNotify (EventInstance<ScoreTracker> e)
	{
		if (e.type == eType.EndGame) {
			Debug.Log("Caught Endgame");
			return;
		} else if (e.type == eType.NewQuestion) {
			scoring.addTouch (touchSum);
			touchSum = new TouchSummary ();
			//send stuff to score tracker
		}
	}
	
	void sendTouch(string key, Vector2 pos)
	{
		string value = "posX: " + pos.x.ToString() + ", posY: " + pos.y.ToString();
		AndroidBroadcastIntentHandler.BroadcastJSONData(key, value);
	}
	void sendEvent(eType type)
	{
		EventInstance<GameObject> e;
		e = new EventInstance<GameObject>();
		e.setEvent(type, this.gameObject);
		eventWrapper.notify(e);
	}
	
	void Update () {
		for (int i = 0; i < Input.touchCount; i++)
		{
			if(t == null)
				t = new touchInstance(); //begin storing info about the touch instance
			Touch touch = Input.GetTouch(i);  
			if (touch.phase == TouchPhase.Began)
			{
				sendTouch("touchBegin", touch.position);
				sendEvent(eType.FingerDown); 
				//search for an object directly under the finger
				RaycastHit2D touchHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector3.back);
				if(touchHit.collider != null && touchHit.collider.gameObject.tag != "Suspended") //we got a hit
				{	
					selection = touchHit.transform.gameObject; 
					AndroidBroadcastIntentHandler.BroadcastJSONData("PlayerSelection", selection.gameObject.name);
					if(selection.gameObject.tag == "Stimulus")
					{
						parentBuffer = selection.transform.parent;  //store and remove the parent to prevent weird parent-child behavior during dragging
						selection.transform.parent = null;
						offset= selection.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, screenPoint));
					}
					
					if(selection.GetComponent<Selectable>() != null) {
						selection.GetComponent<Selectable>().onSelect(t); //notify the selection it has been touched
					}
				}
			
			}
			else if (touch.phase == TouchPhase.Moved||touch.phase == TouchPhase.Stationary)
			{
				t.addTime(touch.deltaTime);
				if(selection != null && selection.gameObject.tag == "Stimulus") 
				{
					sendEvent(eType.Grab);
					if(touch.phase == TouchPhase.Moved && t.getTime() > 0.5f){
						t.setTouch(eType.Drag);
					}
					//translate current finger position to new object position  
					curScreenPoint = new Vector3(touch.position.x, touch.position.y, screenPoint);
					curPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
					selection.transform.position = curPos;
					//scale size of object with respect to the Distance between its current position and the host's receptacle
					if (selection.GetComponent<StimulusScript>() != null)
					{
						selection.GetComponent<StimulusScript>().scaleToTarget(Vector3.Distance(curPos, target.transform.position)*distanceMod);
					}
						
				}
					
			}

			if (touch.phase == TouchPhase.Ended)
			{	
				sendTouch("End Touch", touch.position);
				if(t.isInit()){ //if t.type was not modified in a Drag phase
					t.setTouch(eType.Tap);
				}
				t.addTime(touch.deltaTime);
				if(selection != null) //if object was not placed in receptacle
				{
					t.storeSelection(selection.name);
					if(selection.gameObject.tag == "Stimulus")
					{
						if(selection.GetComponent<StimulusScript>() != null){ 
							//return and rescale object, add it back to SOO as a child
							selection.transform.position = selection.GetComponent<StimulusScript>().returnHomePos();
							selection.GetComponent<StimulusScript>().resetScale();
						}
						else
						{
							selection.transform.position = selection.GetComponent<TokenScript>().returnStartPos();
						}
						if(selection.GetComponent<Selectable>() != null) {
							selection.GetComponent<Selectable>().offSelect(t);
						}
						selection.transform.parent = parentBuffer;			
					}
					selection = null;
				}
				sendEvent(eType.FingerUp);
				touchSum.addTouch(t); //store touch instance data
				t = null;
			}
		}
	
	}
}

/// <summary>
/// Information wrapper containing all information for a given question. Used for packaging touch data into an Event.
/// </summary>
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
	/// <summary>
	/// Adds a TouchInstance to the question's list of discrete touches
	/// </summary>
	/// <param name="t"></param>
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
	public string printListString() {
		string str = "";
		str = (str + "\n\t\t\ttouchList: ");
		for (int i = 0; i < touchList.Count; i++)
		{
			str = (str + "\n\t\t\ttype: " + touchList[i].getType() + "\n\t\t\ttime: " + touchList[i].getTime());
			str = (str + "\n\t\t\tselection: " + touchList[i].getName());
		}
		return str;
	}
}


/// <summary>
/// Class to store information about a given touch instance.
/// </summary>
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
	
	/// <returns> duration of the touch instance.</returns>
	public float getTime () {return duration;}
	/// <returns> the name of the selected object (empty string if no selection)</returns>
	public string getName () {return selectionName;}
	/// <returns>The touch type (enum eType).</returns>
	public eType getType() {return type;}
	/// <summary>
	/// checks to see if touch type has been modified since initialization
	/// </summary>
	/// <returns><c>true</c>, if type was not modified, <c>false</c> otherwise.</returns>
	public bool isInit()
	{
		return (type == eType.Init);
	}
	//******************
	// Setter Functions*
	//******************
	/// <summary>
	/// Increments duration by the given amount 
	/// </summary>
	/// <param name="time">elapsed time</param>
	public void addTime(float time )
	{
		duration += time;
	}
	/// <summary>
	/// Stores the selected object name.
	/// </summary>
	/// <param name="selected">Selected object name.</param>
	public void storeSelection (string selected)
	{
		selectionName = selected;
	}
	/// <summary>
	/// sets the instance to the type of touch event.
	/// </summary>
	/// <param name="t"> touch type</param>
	public void setTouch (eType t)
	{
		type = t;
	}
}
