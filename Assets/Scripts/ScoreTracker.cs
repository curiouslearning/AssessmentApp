using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//data class used to store all data on the user's performance from a given question
//gets information from GameManagerScript, Receptacle, and TouchProcessor
public class Score {
	int questionNumber;
	bool answeredCorrectly;
	TouchSummary touches;
	float timeTaken;
	//question Type implementation
	public Score (int qNum)
	{
		questionNumber = qNum;
	}
//****************
//SetterFunctions*
//****************
	public void addScore (bool answer)
	{
		if(answer)
			answeredCorrectly = true;
		else 
			answeredCorrectly = false;	
	}
	public void addTime (float time)
	{
		timeTaken = time;
	}
	public void addTouch (TouchSummary t)
	{
		touches = t;
		t = null;
	}
//****************
//GetterFunctions*
//****************
	public int getNum ()
	{
		return questionNumber;
	}
	public float getTime ()
	{
		return timeTaken;
	}
	public void printTouches () //debugger
	{
		Debug.Log("\tTouches: " + "\n\t\ttouch count: " + touches.touchCount + "\n\t\tdrag count: " + touches.dragCount + "\n\t\tselection count: " +touches.selectionCount);
		touches.printList();
	}
	public bool isCorrect()
	{
		return answeredCorrectly;
	}
	
}


//Class that organizes and collects all information regarding performance on questions. Individual question data is stored in instances of
//the score class, all of which are stored in the List<Score> contained within the class.
//Directly coupled with TouchProcessor
//Observes:			(Update this list with all new additions
//			GameManagerScript
//			Receptacle
public class ScoreTracker : Observer {
	public GameManagerScript reference;
	public CollisionNotification receptacle;
	public List<Score> scoreList;
	int totalScore;
	Score s;	

	void Start () {
		s = new Score(reference.questionNumber);	
		scoreList = new List<Score>();
		reference.GetComponent<Subject>().addObserver(new Subject.gameManagerNotify(this.onNotify));
		receptacle.GetComponent<Subject>().addObserver(new Subject.GameObjectNotify(this.onNotify));
	}

	public override void onNotify (EventInstance<GameObject> e)
	{
		GameObject answer = e.signaler;
		s.addScore(e.signaler.GetComponent<StimulusScript>().returnIsCorrect());
		s.addTime(reference.questionTime);				
	}
	public override void onNotify (EventInstance<GameManagerScript> e)
	{
		//if(e.type ==eType.EndGame)
			//printList (); //debugger
	}

	public void addTouch (TouchSummary t)
	{
		s.addTouch(t);
		t = null;
		changeQuestion();
	}
	void printList () //debugger
	{
		Debug.Log("total score: " + totalScore);
		for(int i = 0; i < scoreList.Count; i++)
		{
			Debug.Log( "Question: " + scoreList[i].getNum());
			Debug.Log( "\tisCorrect is: " + scoreList[i].isCorrect());
			Debug.Log("\ttime taken: " + scoreList[i].getTime());
			scoreList[i].printTouches();
		}	
	}
	
	// Update is called once per frame
	void changeQuestion () {
		if (s.isCorrect())
			totalScore++;
		else
			totalScore--;
		scoreList.Add(s);
		s = new Score(reference.questionNumber);		
	}
}
