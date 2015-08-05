using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Obs node.
/// Node class for the observer list. Contains next and prev ObsNode pointers and a data pointer to an instance of an observer class.
/// </summary>
public class ObsNode {
	ObsNode next;
	ObsNode prev;
	Observer data;

//*******************
// Setter functions *
//*******************
	public void setData (Observer input)
	{
		data = input;

	}
	public void setNext (ObsNode iter)
	{
		next = iter;
	}
	public	void setPrev (ObsNode iter)
	{
		prev = iter;
	}

//*******************
// Getter functions *
//*******************
	public Observer getData ()
	{
		return data;
	}
	public ObsNode forward ()
	{
		return next;
	}

	public	ObsNode backward ()
	{
		return prev;
	}
/// <summary>
/// Removes this ObsNode from the Observer list.
/// </summary>
	public void removeSelf ()
	{
		next.setPrev(prev);
		prev.setNext(next);
		next = null;
		prev = null;
	}
}


/// <summary>
/// Subject
/// Notifier class for the Obeserver Design Pattern.
/// Add an instance of this class as a component to any class that requires Subject functionality.
/// Contains a doubly linked list of ObsNode pointers that Observers can add themselves to.
/// Notifies Observers with notify (eType, EventInstance<GameObject>).
/// </summary>
public class Subject : MonoBehaviour {
	
	private int numObservers;
	private ObsNode head;
	private ObsNode tail;

	//delegates for notifying using GameObjects
	public delegate void GameObjectNotify (EventInstance<GameObject> key);
	public event GameObjectNotify gameObjectEvent;
	//delegates for notifying using ints
	public delegate void gameManagerNotify (EventInstance<GameManagerScript> key);
	public event gameManagerNotify gMEvent;	
	// delegates for ScoreTracker notifications
	public delegate void scoreTrackerNotify (EventInstance<ScoreTracker> key);
	public event scoreTrackerNotify scoreEvent;
	//delegates for animator notifications
	public delegate void animatorNotify (EventInstance<AnimationManager> key);
	public event animatorNotify anEvent;
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
/*	public void oldNotify (EventInstance<GameObject> e)
	{
		ObsNode temp;
		temp = head;
		while (temp != null){
			Debug.Log ("notifying: " + temp.getData().gameObject.name);
			temp.getData ().onNotify(e);
			temp = temp.forward();
		}		
	}*/
/// <summary>
/// Adds the observer to the Event list.
/// Overriddent to accept multiple types of Delegates
/// </summary>
/// <param name="observer">The delegate method to be called by notify.</param>
	public void addObserver (GameObjectNotify observer)
	{
		gameObjectEvent += observer;
		numObservers++;
		/*ObsNode node = new ObsNode ();
		node.setData(observer);
		
		Debug.Log ("adding observer"); //debugger
		if (head == null)
		{
			Debug.Log("head is null"); //debugger
			head = node;
			tail = node;
			node.setNext(null);
			numObservers++;
		}
		else if (head == tail)
		{
			Debug.Log("count is one"); //debugger
			head.setNext(node);
			tail.setPrev(node);
			if(tail.forward() == null) { Debug.Log("tail.next is null"); } //debugger
		}
		else{
			Debug.Log("no edge case");
			tail.backward().setNext(node);
			tail.setPrev(node);
		}
		*/
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
		//prevent head or tail from calling observer self
		/*if(node == head)
		{
			head = node.forward();
			node.setData (null);
		}
		if(node == tail)
		{
			tail = node.backward();
			node.setData (null);
		}
		else {
			node.removeSelf();
		}*/                                                             
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


