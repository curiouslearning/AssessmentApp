using UnityEngine;
using System.Collections;

//a base Observer class other classes can inherit from for event observation functionality
public class Observer : MonoBehaviour {
	public virtual void onNotify(EventInstance<GameObject> e){}	
	public virtual void onNotify (EventInstance<int> e){}
	public virtual void onNotify (EventInstance<GameManagerScript> e){}
}
