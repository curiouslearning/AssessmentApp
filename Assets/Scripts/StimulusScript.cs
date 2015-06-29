using UnityEngine;
using System.Collections;

/* StimulusScript
 * Class containg data and functionality for individual stimuli 
 * initialized by the SpawnerScript using Question instances
 * attached to the SOO instance as child objects
 */

public class StimulusScript : MonoBehaviour {

	private bool isCorrect; //bool for indicating the correct stimulus response in a question
	private bool isBeingDragged; 
	private bool isDraggable;
	private Vector3 homePos; //snapback functionality
	private Vector3 startScale; //scaling functionality

	void Start ()
	{
		startScale = transform.localScale*0.6f;
		isBeingDragged = false;
	}

//*******************
// Getter functions *
//*******************

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


//*******************
// Setter functions *
//*******************

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

//********************
// Scaling functions *
//********************

	public void scaleToTarget (float mod)
	{
		transform.localScale = startScale*mod;
	}

	public void resetScale ()
	{
		transform.localScale = startScale;
	}

//initialization function
	public	void setStim (serStim input) {
		this.isCorrect = input.isCorrect;
		this.isDraggable = input.isDraggable;
		this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/" + input.sprite);
		this.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/" + input.audio);
	}
	

	void Update () {

	}
}
