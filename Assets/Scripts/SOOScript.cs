using UnityEngine;
using System.Collections;

public class SOOScript : MonoBehaviour {

	private StimulusScript[] stimArray = new StimulusScript[4];
	private Transform[] posArray = new Transform[4];
	private Transform[] destArray = new Transform[4];
	private int lengthStimArray;


	// Methods for accessing variables
	

	public StimulusScript[] returnStimArray() {
		return stimArray;
	}

	public Transform[] returnPosArray() {
		return posArray;
	}

	public Transform[] returnDestArray() {
		return destArray;
	}

	// Methods for setting variables

	public void setStimArray(StimulusScript[] input) {
		stimArray = input;
	}

	public void setPosArray(Transform[] input) {
		posArray = input;
	}

	public void setDestArray(Transform[] input) {
		destArray = input;
	}


	public StimulusScript updateStimPos(Transform newPos, StimulusScript stim) {
		stim.setCurPos (newPos);
		return stim; 
	}

	 

	public StimulusScript[] releaseStim(StimulusScript stim) {
		StimulusScript[] temp = new StimulusScript[lengthStimArray];
		for (int i = 0; i < lengthStimArray; i++) {
			if (stimArray[i].Equals (stim)) {
				i++;
			} else temp[i] = stimArray[i];
		} 
		stimArray = temp;
		return stimArray;
	}


	public StimulusScript[] move() {
		StimulusScript[] temp = new StimulusScript[lengthStimArray];
		for (int i = 0; i < lengthStimArray; i++) {
			 temp[i] = updateStimPos (destArray[i], stimArray[i]);
		} 
		stimArray = temp;
		return stimArray;
	}


	void Start () {
		this.stimArray = stimArray;
		this.posArray = posArray;
		this.destArray = destArray;
		lengthStimArray = stimArray.Length;
	}

	void Update () {
		lengthStimArray = stimArray.Length;
	
	}
}
