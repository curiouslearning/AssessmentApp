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
	// Like GameManager, Score has variables that hold the difficulty and
	// category for the current question.  
	Difficulty difficulty;
	Category category;
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
	public void setDifficulty(Difficulty d)
	{
		difficulty = d;
	}
	public void setCategory(Category c)
	{
		category = c;
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
	public string printTouchesString() // used in broadcast intent
	{
		string str = "";
		str = (str + ("\n\tTouches: " + "\n\t\ttouch count: " + touches.touchCount + "\n\t\tdrag count: " + touches.dragCount + "\n\t\tselection count: " +touches.selectionCount));
		str = (str + touches.printListString ());
		return str;
	}
	public bool isCorrect()
	{
		return answeredCorrectly;
	}
	public bool returnTimedOut()
	{
		return timedOut;
	}
	public Difficulty returnDifficulty() 
	{
		return difficulty;
	}
	public Category returnCategory()
	{
		return category;
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
		s.setTimedOut (false);
		s.setDifficulty (Difficulty.Easy);
		s.setCategory (Category.Customization);
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
		// ScoreTracker observes notifications sent from GameManager.  If
		// the event is of type Timeout, the bool timedOut of the current
		// Score variable s is set to true.
		Debug.Log ("eType: " + e.type);
		if (e.type == eType.Timeout) {
			reference.GetComponent<Subject>().removeObserver(gOObserver);
			s.setTimedOut(true);
			Debug.Log ("recieved a Timeout notification from Game Manager");
			Debug.Log ("timedOut: " + s.returnTimedOut());
			return;
		}

		// if the GameManager notification is of type EndGame, the app sends
		// out a broadcast containing all the data collected by ScoreTracker.
		
		if (e.type == eType.EndGame) {
			
			Debug.Log(printListString()); //debugger - printListString() is used in the broadcast.
			                              //Printing it to the debug log allows us to see the entire
			                              //string that will be sent in the broadcast, which contains
			                              //all the data collected from the latest game.
			
			// *************************************************************************
			// Code for sending broadcasts containing the data collected by ScoreTracker
			// *************************************************************************

			// Instantiate the class Intent
			AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent");  
			// Instantiate the object Intent
			AndroidJavaObject intentObject = new AndroidJavaObject ("android.content.Intent");
			// Call setAction on the Intent object with "ACTION_SEND" as a parameter
			intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string> ("ACTION_SEND")); 
			// Set the type of the Intent to plain text by calling setType
			intentObject.Call<AndroidJavaObject>("setType", "text/plain");
			// call putExtra on intentObject and set printListString() as a parameter in order
			// to broadcast the data collected by ScoreTracker over the course of the game
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), printListString());
			// Instantiate the class UnityPlayer
			AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			// Instantiate the object currentActivity
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");
			// Call the activity with our intent
			currentActivity.Call("sendBroadcast", intentObject);    
		}
		
		
	}
	
	public void addTouch (TouchSummary t)
	{
		s.addTouch(t);
		t = null;
		changeQuestion();
	}
	void printList () //debugger
	{
		Debug.Log("TOTAL SCORE: " + totalScore);
		for(int i = 0; i < scoreList.Count; i++)
		{
			Debug.Log( "Question: " + scoreList[i].getNum());
			Debug.Log( "\tisCorrect is: " + scoreList[i].isCorrect());
			Debug.Log("\ttime taken: " + scoreList[i].getTime());
			Debug.Log ("\ttimed out: " + scoreList[i].returnTimedOut());
			scoreList[i].printTouches();
		}	
	}

	// retrieveStruct is a method needed for averagesBreakdown() to work.  It
	// retrieves a given DifficultyData instance from an array based on the
	// input Category. *Note: retrieveStruct assumes that each DifficultyData 
	// instance in the array is tagged with a unique Category
	DifficultyData retrieveStruct(DifficultyData[] ddarray, Category cat) {
		int length = ddarray.Length;
		for (int i = 0; i < length; i++) {
			if (ddarray[i].categoryMatch(cat)) { 
				return ddarray[i];
			}
		}
		throw new System.ArgumentException ("Variable of type Category not found in ddarray");
	}

	// averagesBreakdown creates an array of DifficultyData variables, one for each
	// category in the Category enum.  It then adds each score in scoreList to the 
	// apropriate DifficultyData variable, using retrieveStruct to select the correct
	// variable from the DifficultyData array.  It then prints out a string containing
	// all the data that has been added to each DifficultyData variable.
	string averagesBreakdown() {
		
		DifficultyData Customization = new DifficultyData (Category.Customization);
		DifficultyData ReceptiveVocabulary = new DifficultyData (Category.ReceptiveVocabulary);
		DifficultyData LetterNameRecognition = new DifficultyData (Category.LetterNameRecognition);
		DifficultyData LetterSoundMatching = new DifficultyData (Category.LetterSoundMatching);
		DifficultyData CVCWordIdentification = new DifficultyData (Category.CVCWordIdentification);
		DifficultyData SightWordIdentification = new DifficultyData (Category.SightWordIdentification);
		DifficultyData RhymingWordMatching = new DifficultyData (Category.RhymingWordMatching);
		DifficultyData BlendingWordIdentification = new DifficultyData (Category.BlendingWordIdentification);
		DifficultyData PseudoWordMatching = new DifficultyData (Category.PseudowordMatching);
		
		DifficultyData[] ddarray = {Customization, ReceptiveVocabulary, LetterNameRecognition, LetterSoundMatching,
			CVCWordIdentification, SightWordIdentification, RhymingWordMatching, BlendingWordIdentification,
			PseudoWordMatching};
		string answer = "";
		
		for (int i = 0; i < scoreList.Count; i++) {
			Score currentScore = scoreList[i];
			retrieveStruct(ddarray,currentScore.returnCategory()).addScore(currentScore);
		}
		
		for (int a = 0; a < ddarray.Length; a++) {
			answer = (answer + ddarray[a].toString ());
		}
		return answer;
	}

	// averageTime prints out the average time taken per question across all
	// categories and difficulties.
	string averageTime() {
		float timeSum = 0f;
		int numQuestions = 0;
		for (int i = 0; i < scoreList.Count; i++) {
			timeSum = timeSum + scoreList[i].getTime();
			numQuestions++;
		}
		return ("average question time across all categories and difficulties: " + timeSum / numQuestions);
	}
	
	string printListString () //for broadcasts
	{
		string st = "";
		st = (st + "TOTAL SCORE: " + totalScore + "\n\n");
		st = (st + averageTime());
		st = (st + averagesBreakdown ());
		st = (st + "\n\n");
		for(int i = 0; i < scoreList.Count; i++)
		{
			st = (st + "\nQuestion: " + scoreList[i].getNum());
			st = (st + "\nCorrect?: " + scoreList[i].isCorrect());
			st = (st + "\ntime taken: " + scoreList[i].getTime());
			st = (st + "\ntimed out: " + scoreList[i].returnTimedOut());
			st = (st + "\nCategory: " + scoreList[i].returnCategory());
			st = (st + "\nDifficulty: " + scoreList[i].returnDifficulty() + "\n");
			st = (st + scoreList[i].printTouchesString() + "\n\n");
		}	
		return st;
	}
	
	// Update is called once per frame
	void changeQuestion () {
		numAnswered++;
		if (s.isCorrect ()) {
			totalScore++;
			numCorrect++;
			// numWrong must be reset to 0 when the player is correct, otherwise
			// scoreTracker will send notifications when the player gets
			// three nonconsecutive wrong answers.
			numWrong = 0; 
		} else {
			totalScore--;
			numWrong++; 
			// numRight must be reset to 0 when the player is correct, otherwise
			// scoreTracker will send notifications when the player gets
			// four nonconsecutive right answers.
			numCorrect = 0;
		}
		Debug.Log ("numRight " + numCorrect);
		Debug.Log ("numWrong " + numWrong);
		
		Debug.Log ("totalScore " + totalScore);
		Debug.Log ("numAnswered " + numAnswered); 
		if (numCorrect >= 3) {
			// if the player answers three consecutive questions correctly, numCorrect is
			// reset and an event notification of type ChangeDifficulty is sent out, which
			// will be picked up by GameManager.
			numCorrect = 0;
			EventInstance<ScoreTracker> e;
			e = new EventInstance<ScoreTracker> ();
			e.setEvent (eType.ChangeDifficulty, this);
			eventHandler.notify (e);
			Debug.Log ("sent ChangeDifficulty notification");
			// the difficulty and category variables in the current score variable
			// must also be adjusted appropriately.
			if (s.returnDifficulty().Equals(Difficulty.Easy) || s.returnCategory().Equals (Difficulty.Medium)) {
				Difficulty diff = s.returnDifficulty();
				s.setDifficulty (diff++); 
			} else
				s.setDifficulty (Difficulty.Hard);
		} else if (numWrong >= 4) {
			// if the player answers four consecutive questions wrong, a ChangeCategory
			// notification is sent out, category and difficulty variables in the current
			// score variable are adjusted.
			numWrong = 0;
			EventInstance<ScoreTracker> e;
			e = new EventInstance<ScoreTracker> ();
			e.setEvent (eType.ChangeCategory, this);
			eventHandler.notify (e);
			Debug.Log ("sent ChangeCategory notification");
			Category cat = s.returnCategory();
			s.setCategory (cat++);
		} else if (numAnswered >= 20) {
			// if twenty questions in one category are answered, a ChangeCategory
			// notification is sent out, category and difficulty variables in the current
			// score variable are adjusted.
			numAnswered = 0;
			EventInstance<ScoreTracker> e;
			e = new EventInstance<ScoreTracker> ();
			e.setEvent (eType.ChangeCategory, this);
			eventHandler.notify (e);
			Debug.Log ("sent ChangeCategory notification");
			Category cat = s.returnCategory();
			s.setCategory(cat++);
		}
		scoreList.Add(s);
		s = new Score(reference.questionNumber);		
	}
	
}

// Public class for concatenating various average scores and times to be used
// in broadcast intent.  ScoreTracker creates one instance of DifficultyData
// for each category and stores data on the student's performance for each
// category in those instances.

public class DifficultyData {

	// ints for holding combined scores for each level of difficulty 
	int easyScore;
	int mediumScore;
	int hardScore;

	// floats for holding combined times for each level of difficulty
	float easyTime;
	float mediumTime;
	float hardTime;

	// ints for holding the number of scores per level of difficulty
	int numEasy;
	int numMedium;
	int numHard;

	// ints and floats for holding calculated averages
	int easyScoreAverage;
	int mediumScoreAverage;
	int hardScoreAverage;
	
	int totalScoreAverage;
	
	float easyTimeAverage;
	float mediumTimeAverage;
	float hardTimeaverage; 
	
	float totalTimeAverage;
	
	Category cat; 

	// Constructor takes an instance of the Category enum so that instances
	// of DifficultyData are tagged with their corresponding Category
	public DifficultyData(Category category) {
		easyScore = 0;
		mediumScore = 0;
		hardScore = 0;
		easyTime = 0f;
		mediumTime = 0f;
		hardTime = 0f;
		numEasy = 0;
		numMedium = 0;
		numHard = 0;
		this.cat = category;
	}

	// addScore increments variables based on what difficulty
	// the given Score variable is tagged with
	public void addScore (Score s) {
		Difficulty diff = s.returnDifficulty();
		float time = s.getTime();
		int score;
		if (s.isCorrect ())
			score = 1;
		else
			score = -1;
		if (diff.Equals (Difficulty.Easy)) {
			easyScore += score; 
			easyTime += time;
			numEasy++;
		} else if (diff.Equals (Difficulty.Medium)) {
			mediumScore += score;
			mediumTime += time;
			numMedium++;
		} else {
			hardScore += score;
			hardTime += time;
			numHard++;
		}
	}

	// categoryMatch returns a bool indicating whether the given category
	// matches the category of the current DifficultyData instance
	public bool categoryMatch(Category category) {
		return (cat.Equals (category));
	}

	// toString assigns the average-holding variables their correct values
	// with a series of if statements (to prevent divide-by-zero errors) and then
	// uses the average-holding variables to create a long string containing all the
	// data collected by the DifficultyData instance.

	public string toString() {

		if (numEasy > 0) 
			easyScoreAverage = easyScore / numEasy;
		else
			easyScoreAverage = 0;
		if (numMedium > 0)
			mediumScoreAverage = mediumScore / numMedium;
		else
			mediumScoreAverage = 0;
		if (numHard > 0)
			hardScoreAverage = hardScore / numHard;
		else
			hardScoreAverage = 0;

		if (numEasy == 0 && numMedium == 0 && numHard == 0)
			totalScoreAverage = 0;
		else	
			totalScoreAverage = (easyScore + mediumScore + hardScore) / (numEasy + numMedium + numHard);

		if (numEasy > 0)
			easyTimeAverage = easyTime / numEasy;
		else 
			easyTimeAverage = 0f;
		if (numMedium > 0)
			mediumTimeAverage = mediumTime / numMedium;
		else
			mediumTimeAverage = 0f;
		if (numHard > 0)
			hardTimeaverage = hardTime / numHard;
		else
			hardTimeaverage = 0f;
		if (numEasy == 0 && numMedium == 0 && numHard == 0)
			totalTimeAverage = 0f;
		else
			totalTimeAverage = (easyTime + mediumTime + hardTime) / (numEasy + numMedium + numHard);
		
		string str = "";
		str = (str + "\nAverage score for Category " + cat + ": " + totalScoreAverage);
		str = (str + "\nAverage time for Category " + cat + ": " + totalTimeAverage);
		str = (str + "\n\nAverage score for easy questions in " + cat + ": " + easyScoreAverage);
		str = (str + "\nAverage score for medium questions in " + cat + ": " + mediumScoreAverage);
		str = (str + "\nAverage score for hard questions in " + cat + ": " + hardScoreAverage);
		str = (str + "\n\nAverage time for easy questions in" + cat + ": " + easyTimeAverage);
		str = (str + "\nAverage time for medium questions in" + cat + ": " + mediumTimeAverage);
		str = (str + "\nAverage time for hard questions in" + cat + ": " + hardTimeaverage);
		str = (str + "\n\n\n");
		return str;
	}
	
}