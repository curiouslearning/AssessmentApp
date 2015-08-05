using UnityEngine;
using System.Collections;
/// <summary>
/// A base Observer class other classes can inherit from for event observation functionality
/// </summary>

public class Observer : MonoBehaviour {
	/// <summary>
	/// base method that can be overridden for specific event handling functionality.
	/// </summary>
	/// <param name="e">Event Instance containing the subject and they type of event thrown.</param>
	public virtual void onNotify(EventInstance<GameObject> e){}	
	public virtual void onNotify (EventInstance<int> e){}
	public virtual void onNotify (EventInstance<GameManagerScript> e){}
	public virtual void onNotify (EventInstance<ScoreTracker> e) {}
}
