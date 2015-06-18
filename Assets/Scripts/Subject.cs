using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Subject : MonoBehaviour {
	private int numObservers;
	private Observer head;
	private Observer tail;
	void Start ()
	{
		head = new Observer();
		tail = new Observer();
	}
	/*public void notify (EventInstance<T> e)
	{
		Observer temp;
		temp = head.forward();
		while (temp != tail){
			temp.onNotify(e);
			temp.forward();
		}		
	}*/
	public void addObserver (Observer observer)
	{
		if (head == null)
		{
			head.setNext(observer);
			tail.setPrev(observer);
		}
		else{
			tail.backward().setNext(observer);
			tail.setPrev(observer);
		}
	}
	private void removeObserver (Observer observer)
	{
		//prevent head or tail from calling observer self
		if(observer == head || observer == tail)
			return;
		observer.removeSelf();                                                                  
	}

		
		 
}


