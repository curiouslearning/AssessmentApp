using UnityEngine;
using System.Collections;

public class Question : MonoBehaviour {

	private int questionNumber;
	private SOOScript stimuli;
	private StimulusScript correctAnswer;


	// Accessing variables

	public int returnQuestionNumber() {
		return questionNumber;
	}

	public SOOScript returnStimuli() {
		return stimuli;
	}

	public StimulusScript returnStimulusScript() {
		return correctAnswer;
	}

	// Setting variables

	public void setQuestionNumber(int input) {
		questionNumber = input;
	}

	public void setStimuli(SOOScript input) {
		stimuli = input;
	}

	public void setCorrectAnswer(StimulusScript input) {
		correctAnswer = input;
	}


	void Start () {
		this.questionNumber = questionNumber;
		this.stimuli = stimuli;
		this.correctAnswer = correctAnswer;
	}
	

	void Update () {
	
	}
}
