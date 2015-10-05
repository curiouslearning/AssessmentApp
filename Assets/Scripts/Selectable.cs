using UnityEngine;
using System.Collections;

// Parent Event parsing Component for objects that can be selected by user.
// Objects will use customized, inherited members (i.e: SelectableStim) for functionality
public class Selectable : MonoBehaviour {
	public delegate void passInput (touchInstance t);
	passInput p;
	passInput o;

	public void initP (passInput input)
	{
		p = input;
	}
	public void initO (passInput input)
	{
		o = input;
	}
	public void onSelect(touchInstance t)
	{
		if(p != null)
			p(t);
		else {
			Debug.LogError("null p");
		}
	}
	public void offSelect(touchInstance t)
	{
		if (o != null)
			o(t);
	}
}
