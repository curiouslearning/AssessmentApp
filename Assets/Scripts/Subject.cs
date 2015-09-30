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
	// delegates for ScoreTracker notifications
	public delegate void scoreTrackerNotify (EventInstance<ScoreTracker> key);
	public event scoreTrackerNotify scoreEvent;
	// delegates for bool notifications
	public delegate void boolNotify (EventInstance<bool> key);
	public event boolNotify boolEvent;
	/*delegates for animator notifications     COMMENTED OUT DUE TO UNUSE
	public delegate void animatorNotify (EventInstance<AnimationManager> key);
	public event animatorNotify anEvent;*/
	void Start ()
	{
		numObservers = 0;
	}

	/// <summary>
	/// Packages the event and sends it to the observers.
	/// </summary>
	/// <param name="type">Event Type.</param>
	public void sendEvent (eType type)
	{
		EventInstance<GameObject> e;
		e = new EventInstance<GameObject> ();
		e.setEvent (type, this.gameObject);
		notify (e);	
	}

	public void sendEvent (eType type, GameObject g)
	{
		EventInstance<GameObject> e;
		e = new EventInstance<GameObject> ();
		e.setEvent (type, g);
		notify (e);	
	}	
	
	public void sendBoolEvent (eType type, bool val)
	{
		EventInstance<bool> e;
		e = new EventInstance<bool>();
		e.signaler = val;
		e.type = type;
		notify (e);
	}
	/// <summary>
	/// calls the onNotify() methods of all Observers in the Event list.
	/// Overriddent to accept multiple types of Delegates.
	/// </summary>
	/// <param name="e">An EventInstance containing the Subject and the event type enum.</param>
	public void notify(EventInstance<GameObject> e)
	{
		if (gameObjectEvent != null){
			gameObjectEvent(e);
		}
	}

	public void notify(EventInstance<ScoreTracker> e)
	{
		if (scoreEvent != null) {
			scoreEvent(e);
		}
	}
	
	public void notify(EventInstance<bool> e)
	{
		if(boolEvent != null){
			boolEvent(e);
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
	}

	public void addObserver (scoreTrackerNotify ob)
	{
		scoreEvent += ob;
		numObservers++;
	}

	public void addObserver (boolNotify ob)
	{
		boolEvent += ob;
		numObservers++;
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
	public void removeObserver (scoreTrackerNotify ob)
	{
		scoreEvent -= ob;
		numObservers--;
	} 		
	public void removeObserver (boolNotify ob)
	{
		boolEvent -= ob;
		numObservers--;
	} 
}