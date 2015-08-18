using UnityEngine;
using System.Collections;

/* EventInstance.cs
 * Contains event signaling data types (enum eType, EventInstance <T>)
 */
public enum eType {Tap, Trashed, Selected, Drag, Init, NewQuestion, EndGame, TimedOut}; //basic enum that contains all signalable events


/// <summary>
/// EventInstance <T>
/// Generic class that wraps an eType signal and the signaler object for
/// notify to pass to an observer list.
/// The Subject passing the EventInstance and the Observer receiving it must
/// have the same type of EventInstance<T>  (i.e: EventInstance<GameObject>)
/// </summary>
public class EventInstance <T> 
{
	public eType type;
	public T signaler;
	/// <summary>
	/// Initializes the instance with the subject and the event type 
	/// </summary>
	/// <param name="input">event type</param>
	/// <param name="param"> the subject</param>
	/// <typeparam name="T">type parameter restricting use to classes.</typeparam>
	public void setEvent <T> (eType input, T param) where T : class
	{
		type = input;
		signaler = param;  
	}
	
}
