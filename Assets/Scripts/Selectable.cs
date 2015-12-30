using UnityEngine;
using System.Collections;

// Parent Event parsing Component for objects that can be selected by user.
// Objects will use customized, inherited members (i.e: SelectableStim) for functionality
public class Selectable : MonoBehaviour {
	public delegate void passInput (touchInstance t);
	passInput p;
	passInput o;
	passInput p1;
	passInput o1;

	public void initP (passInput input)
	{
		p = input;
	}
	public void initP (passInput i1, passInput i2)
	{
		p = i1;
		p1 = i2;
	}

	public void initO (passInput i1, passInput i2)
	{
		o = i1;
		o1 = i2;
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
		if(p1 != null)
			p1(t);
	}
	public void offSelect(touchInstance t)
	{
		if (o != null)
			o(t);

		if(o1 != null)
			o1(t);
	}
}
