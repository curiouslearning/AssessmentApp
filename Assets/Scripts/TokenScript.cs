using UnityEngine;
using System.Collections;

public class TokenScript : MonoBehaviour {

	bool isCorrect;
	Selectable touchInput;
	
	void start() {
		isCorrect = false;
		touchInput = this.GetComponent<Selectable>();
		if(touchInput == null)
		{
			Debug.LogError("could not get selectable");
		}
		touchInput.initP(onSelect);
		touchInput.initO(offSelect);
	}
	
	public void setIsCorrect (bool val)
	{
		isCorrect = val;
	}
	
	public bool returnIsCorrect ()
	{
		return isCorrect;
	}
	
	public void onSelect(touchInstance t)
	{
		GetComponent<Animator>().SetBool("Selected", true);
	}
	public void offSelect(touchInstance t)
	{
		GetComponent<Animator>().SetBool("Selected", false);
	}
}
