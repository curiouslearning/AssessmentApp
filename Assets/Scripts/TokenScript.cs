using UnityEngine;
using System.Collections;

public class TokenScript : MonoBehaviour {

	bool isTarget;
	Selectable touchInput;
	Vector3 startPos;
	Animator animator;
	

	void Start() {
		animator = this.GetComponent<Animator> ();
		if (animator == null) {
			Debug.Log ("empty animator!");
		}
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

	/// <summary>
	/// Parses touch events for the Secondary Character's Token
	/// </summary>
	/// <param name="t">T.</param>
	public void onSelect(touchInstance t)
	{
		Debug.Log (t.getType ());
		if (t.getType () == eType.Tap) {
			animator.SetTrigger ("Speak");
		} else {
			//GetComponent<Animator> ().SetBool ("Selected", true);
		}
	}
	public void offSelect(touchInstance t)
	{
		GetComponent<Animator> ().SetBool ("Selected", false);
	}
	public void talk()
	{
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
