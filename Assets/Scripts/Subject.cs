using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Subject : MonoBehaviour {
	private int numObservers;
	private Observer head;
	private Observer tail;
	void Start ()
	{
	}
	public void notify (EventInstance<GameObject> e)
	{
		Observer temp;
		temp = head.forward();
		while (temp != tail){
			temp.onNotify(e);
			temp.forward();
		}		
	}
	public void addObserver (Observer observer)
	{
		if (head == null)
		{
			head = observer;
			tail = observer;
		}
		else if (head == tail)
		{
			head.setNext(observer);
			tail.setPrev(observer);
		}
		else{
			tail.backward().setNext(observer);
			tail.setPrev(observer);
		}
	}
	void removeObserver (Observer observer)
	{
		//prevent head or tail from calling observer self
		if(observer == head)
			head = observer.forward();
		if(observer == tail)
			tail = observer.backward();
		else {
			observer.removeSelf();
		}                                                             
	}

		
		 
}


