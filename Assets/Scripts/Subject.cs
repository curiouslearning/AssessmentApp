using UnityEngine;
using System.Collections;
using System.Collections.Generic;




/// <summary>
/// Subject
/// Notifier class for the Obeserver Design Pattern.
/// Add an instance of this class as a component to any class that requires Subject functionality.
/// Contains a doubly linked list of ObsNode pointers that Observers can add themselves to.
/// Notifies Observers with notify (eType, EventInstance<GameObject>).
/// </summary>
public class Subject : MonoBehaviour {
	
	private int numObservers;

	//delegates for notifying using GameObjects
	public delegate void GameObjectNotify (EventInstance<GameObject> key);
	public event GameObjectNotify gameObjectEvent;
	//delegates for notifying using ints
	public delegate void gameManagerNotify (EventInstance<GameManagerScript> key);
	public event gameManagerNotify gMEvent;	
	// delegates for ScoreTracker notifications
	public delegate void scoreTrackerNotify (EventInstance<ScoreTracker> key);
	public event scoreTrackerNotify scoreEvent;
	/*delegates for animator notifications     COMMENTED OUT DUE TO UNUSE
	public delegate void animatorNotify (EventInstance<AnimationManager> key);
	public event animatorNotify anEvent;*/
	void Start ()
	{
		numObservers = 0;
	}


	/// <summary>
	/// calls the onNotify() methods of all Observers in the Event list.
	/// Overriddent to accept multiple types of Delegates.
	/// </summary>
	/// <param name="e">An EventInstance containing the Subject and the event type enum.</param>
	public void notify(EventInstance<GameObject> e)
	{
		Debug.Log("in go notify");
		if (gameObjectEvent != null){
			Debug.Log("notifying");
			gameObjectEvent(e);
		}
	}
	public void notify(EventInstance<GameManagerScript> e)
	{
		Debug.Log("in gm notify");
		if (gMEvent != null){
			Debug.Log("notifying");
			gMEvent(e);
		}
	}
	public void notify(EventInstance<ScoreTracker> e)
	{
		Debug.Log ("in go notify");
		if (scoreEvent != null) {
			Debug.Log("notifying");
			scoreEvent(e);
		}
	}

/// <summary>
/// Adds the observer to the Event list.
/// Overriddent to accept multiple types of Delegates
/// </summary>
/// <param name="observer">The delegate method to be called by notify.</param>
	public void addObserver (GameObjectNotify observer)
	{
		gameObjectEvent += observer;
		numObservers++;
		Debug.Log("there are " + numObservers + " observers in the queue"); //debugger
	}
	public void addObserver (gameManagerNotify ob)
	{
		gMEvent += ob;
		numObservers++;		
		Debug.Log("there are " + numObservers + " observers in the queue"); //debugger
	}
	public void addObserver (scoreTrackerNotify ob)
	{
		scoreEvent += ob;
		numObservers++;
		Debug.Log ("there are " + numObservers + " observers in the queue"); //debugger
	}
/// <summary>
/// Removes the observer from the Event List.
/// Overridden to accept multiple Delegate types.
/// </summary>
/// <param name="ob">The observer to be removed.</param>
	public void removeObserver (GameObjectNotify ob)
	{
		gameObjectEvent -= ob; 
		numObservers--;                                                             
	}
	public void removeObserver (gameManagerNotify ob)
	{
		gMEvent -= ob;
		numObservers--;
	}
	public void removeObserver (scoreTrackerNotify ob)
	{
		scoreEvent -= ob;
		numObservers--;
	} 		 
}