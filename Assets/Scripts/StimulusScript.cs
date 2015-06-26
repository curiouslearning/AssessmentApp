using UnityEngine;
using System.Collections;

public class StimulusScript : MonoBehaviour {

	private bool isCorrect;
	private bool isBeingDragged; 
	private bool isDraggable;
	private Vector3 curPos;
	private Vector3 homePos;
	private Vector3 startScale;

	void Start ()
	{
		startScale = transform.localScale*0.6f;
		isBeingDragged = false;
		curPos = transform.position;
	}

	// Methods for accessing variables

	public bool returnIsCorrect() {
		return isCorrect;
	}

	public bool returnIsBeingDragged() {
		return isBeingDragged;
	}

	public bool returnIsDraggable() {
		return isDraggable;
	}

	public Vector3 returnHomePos() {
		return homePos;  
	}


	// Methods for setting variables

	public void setIsCorrect(bool input) {
		isCorrect = input;
	}

	public void setIsBeingDragged(bool input) {
		isBeingDragged = input;
	}

	public void setIsDraggable(bool input) {
		isDraggable = input;
	}

	public void setHomePos() {
		homePos = transform.position;
	}
	public void scaleToTarget (float mod)
	{
		transform.localScale = startScale*mod;
	}
	public void resetScale ()
	{
		transform.localScale = startScale;
	}

	// private Stimulus resize() {
	// size = size * distanceToHost/someConstant
	// }

	public	void setStim (serStim input) {
		this.isCorrect = input.isCorrect;
		this.isDraggable = input.isDraggable;
		this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/" + input.sprite);
		this.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/" + input.audio);
	}
	

	void Update () {

	}
}
