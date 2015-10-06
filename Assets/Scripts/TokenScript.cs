using UnityEngine;
using System.Collections;

public class TokenScript : MonoBehaviour {

	bool isCorrect;
	Selectable touchInput;
	Vector3 startPos;
	
	void Start() {
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
		GetComponent<Animator>().enabled =false;
		GetComponent<Animator>().SetBool("Selected", true);
	}
	public void offSelect(touchInstance t)
	{
		GetComponent<Animator>().enabled = true;
		GetComponent<Animator>().SetBool("Selected", false);
	}
	public void setPos ()
	{
		startPos = transform.position;
	}
	public Vector3 returnStartPos ()
	{
		return startPos;
	}	
}
