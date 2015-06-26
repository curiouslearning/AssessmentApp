using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObsNode {
	ObsNode next;
	ObsNode prev;
	Observer data;
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
	
	public void removeSelf ()
	{
		next.setPrev(prev);
		prev.setNext(next);
		next = null;
		prev = null;
	}
}
public class Subject : MonoBehaviour {
	private int numObservers;
	private ObsNode head;
	private ObsNode tail;
	void Start ()
	{
		numObservers = 0;
	}
	public void notify (EventInstance<GameObject> e)
	{
		Debug.Log("in notify");
		ObsNode temp;
		temp = head;
		while (temp != null){
			Debug.Log ("notifying: " + temp.getData().gameObject.name);
			temp.getData ().onNotify(e);
			temp = temp.forward();
		}		
	}
	public void addObserver (Observer observer)
	{
		ObsNode node = new ObsNode ();
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
			Debug.Log("there are " + numObservers + " observers in the queue"); //debugger
	}
	public void removeObserver (ObsNode node)
	{
		//prevent head or tail from calling observer self
		if(node == head)
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
		}                                                             
	}

		
		 
}


