using UnityEngine;
using System.Collections;

public enum eType {Tapped, Trashed, Selected, Dragged};
public class EventInstance <T> 
{
	public eType type;
	public T signaler;
	public void setEvent <T> (eType input, T param) where T : class
	{
		type = input;
		signaler = param;
	}

}
