using UnityEngine;
using System.Collections;

// Parent Event parsing Component for objects that can be selected by user.
// Objects will use customized, inherited members (i.e: SelectableStim) for functionality
public class Selectable : MonoBehaviour {
	public delegate void passInput (touchInstance t);
	passInput p;

	public void initP (passInput input)
	{
		p = input;
	}
	public void onSelect(touchInstance t)
	{
		p(t);
	}
}
