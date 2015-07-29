using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//data class used to store all data on the user's performance from a given question
//gets information from GameManagerScript, Receptacle, and TouchProcessor
public class Score {
	int questionNumber;
	bool answeredCorrectly;
	bool timedOut;
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
	public void setTimedOut(bool b) 
	{
		timedOut = b;
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
	public bool returnTimedOut()
	{
		return timedOut;
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
	int numCorrect;
	int numWrong;
	int numAnswered;
	Subject.gameManagerNotify gMObserver;
	Subject.GameObjectNotify gOObserver;
	Score s;	
	public Subject eventHandler;

	void Start () {
		s = new Score(reference.questionNumber);	
		s.setTimedOut (true);
		scoreList = new List<Score>();
		gMObserver = new Subject.gameManagerNotify (this.onNotify);
		gOObserver = new Subject.GameObjectNotify (this.onNotify);
		reference.GetComponent<Subject>().addObserver(gMObserver); 
		receptacle.GetComponent<Subject> ().addObserver (gOObserver); 
	}

	public override void onNotify (EventInstance<GameObject> e)
	{
		GameObject answer = e.signaler;
		s.addScore(e.signaler.GetComponent<StimulusScript>().returnIsCorrect());
		s.addTime(reference.questionTime);				
	}
	public override void onNotify (EventInstance<GameManagerScript> e) //synchronize this or atomize variables
	{
		Debug.Log ("eType: " + e.type);
		if (e.type == eType.Timeout) {
			reference.GetComponent<Subject>().removeObserver(gOObserver);
			s.setTimedOut(true);
			//s.addScore(false);
			//s.addTime(reference.questionTime);
			Debug.Log ("recieved a Timeout notification from Game Manager");
			Debug.Log ("timedOut: " + s.returnTimedOut());
			return;
		}

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
			Debug.Log ("\ttimed out: " + scoreList[i].returnTimedOut());
			scoreList[i].printTouches();
		}	
	}
	
	// Update is called once per frame
	void changeQuestion () {
		numAnswered++;
		if (s.isCorrect ()) {
			totalScore++;
			numCorrect++;
		} else {
			totalScore--;
			numWrong++; 
		}
		Debug.Log ("numRight " + numCorrect);
		Debug.Log ("numWrong " + numWrong);
		Debug.Log ("totalScore " + totalScore);
		Debug.Log ("numAnswered " + numAnswered); 
		if (numCorrect >= 3) {
			numCorrect = 0;
			EventInstance<ScoreTracker> e;
			e = new EventInstance<ScoreTracker> ();
			e.setEvent (eType.ChangeDifficulty, this);
			eventHandler.notify (e);
			Debug.Log ("sent ChangeDifficulty notification");
		} else if (numWrong >= 4) {
			numWrong = 0;
			EventInstance<ScoreTracker> e;
			e = new EventInstance<ScoreTracker> ();
			e.setEvent (eType.ChangeCategory, this);
			eventHandler.notify (e);
			Debug.Log ("sent ChangeCategory notification");
		} else if (numAnswered >= 20) {
			numAnswered = 0;
			EventInstance<ScoreTracker> e;
			e = new EventInstance<ScoreTracker> ();
			e.setEvent (eType.ChangeCategory, this);
			eventHandler.notify (e);
			Debug.Log ("sent ChangeCategory notification");
		}
		scoreList.Add(s);
		s = new Score(reference.questionNumber);		
	}





}
