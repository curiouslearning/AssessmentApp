using UnityEngine;
using System.Collections;

public class StimulusScript : MonoBehaviour {

	private bool isCorrect;
	private bool isBeingDragged; 
	private bool isDraggable;
	private Transform curPos;
	private Vector3 size;

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

	public Transform returnCurPos() {
		return curPos;  
	}

	public Vector3 returnSize() {
		return curPos.lossyScale;
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

	public void setCurPos(Transform input) {
		curPos = input;
	}

	// private Stimulus resize() {
	// size = size * distanceToHost/someConstant
	// }

	void Start () {
		this.isCorrect = isCorrect;
		this.isBeingDragged = false;
		this.isDraggable = true;
		this.curPos = curPos;
		this.size = curPos.lossyScale;
	}
	

	void Update () {

	}
}
