using UnityEngine;
using System.Collections;

//a base Observer class other classes can inherit from for event observation functionality
public class Observer : MonoBehaviour {
	Observer next;
	Observer prev;

	void Start ()
	{
		prev = null;
		next = null;
	}


	public void setNext (Observer iter)
	{
		next = iter;
	}
	 public	void setPrev (Observer iter)
	{
		prev = iter;
	}

	public virtual void onNotify(EventInstance<GameObject> e){}
	public Observer forward ()
	{
		return next;
	}

	public	Observer backward ()
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
