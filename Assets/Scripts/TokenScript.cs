using UnityEngine;
using System.Collections;

public class TokenScript : MonoBehaviour {

	bool isTarget;
	Selectable touchInput;
	Vector3 startPos;
	
	void Start() {
		isTarget = GetComponentInParent<StimulusScript>().returnIsTarget();
		touchInput = this.GetComponent<Selectable>();
		if(touchInput == null)
		{
			Debug.LogError("could not get selectable");
		}
		StimulusScript s = GetComponentInParent<StimulusScript>();
		touchInput.initP(onSelect, s.onSelect);
		touchInput.initO(offSelect);
		GameObject.Find("Main Camera").GetComponent<ScreenCleaner>().registerObject (this.gameObject, OnscreenObjectList.MyObjectTypes.Token);
	}

	void OnDestroy ()
	{
		if(GameObject.Find("Main Camera") == null)
			return;
		GameObject.Find("Main Camera").GetComponent<ScreenCleaner>().deRegisterObject (this.gameObject, OnscreenObjectList.MyObjectTypes.Token); //deregister with screen cleaner
	}	
	
	
	public bool returnIsTarget ()
	{
		return isTarget;
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
