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
	public string printTouchesString()
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
		Debug.Log ("eType: " + e.type);
		if (e.type == eType.Timeout) {
			reference.GetComponent<Subject>().removeObserver(gOObserver);
			s.setTimedOut(true);
			Debug.Log ("recieved a Timeout notification from Game Manager");
			Debug.Log ("timedOut: " + s.returnTimedOut());
			return;
		}
		
		if (e.type == eType.EndGame) {
			
			Debug.Log(printListString()); //debugger
			
			// *************************************************************************
			// Code for sending broadcasts containing the data collected by ScoreTracker
			// *************************************************************************
			
			AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent");  
			AndroidJavaObject intentObject = new AndroidJavaObject ("android.content.Intent");
			
			intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string> ("ACTION_SEND")); 
			intentObject.Call<AndroidJavaObject>("setType", "text/plain");
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), printListString());
			AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");
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
	
	DifficultyData retrieveStruct(DifficultyData[] ddarray, Category cat) {
		int length = ddarray.Length;
		for (int i = 0; i < length; i++) {
			if (ddarray[i].categoryMatch(cat)) { 
				return ddarray[i];
			}
		}
		throw new System.ArgumentException ("Variable of type Category not found in ddarray");
	}
	
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
			retrieveStruct(ddarray,currentScore.returnCategory ()).addScore (currentScore);
		}
		
		for (int a = 0; a < ddarray.Length; a++) {
			answer = (answer + ddarray[a].toString ());
		}
		return answer;
	}
	
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
		st = (st + averagesBreakdown ());
		st = (st + averageTime());
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
			if (s.returnDifficulty().Equals(Difficulty.Easy) || s.returnCategory().Equals (Difficulty.Medium)) {
				Difficulty diff = s.returnDifficulty();
				s.setDifficulty (diff++); 
			} else
				s.setDifficulty (Difficulty.Hard);
		} else if (numWrong >= 4) {
			numWrong = 0;
			EventInstance<ScoreTracker> e;
			e = new EventInstance<ScoreTracker> ();
			e.setEvent (eType.ChangeCategory, this);
			eventHandler.notify (e);
			Debug.Log ("sent ChangeCategory notification");
			Category cat = s.returnCategory();
			s.setCategory (cat++);
		} else if (numAnswered >= 20) {
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
// in broadcast intent
public class DifficultyData {
	
	int easyScore;
	int mediumScore;
	int hardScore;
	
	float easyTime;
	float mediumTime;
	float hardTime;
	
	int numEasy;
	int numMedium;
	int numHard;
	
	int easyScoreAverage;
	int mediumScoreAverage;
	int hardScoreAverage;
	
	int totalScoreAverage;
	
	float easyTimeAverage;
	float mediumTimeAverage;
	float hardTimeaverage; 
	
	float totalTimeAverage;
	
	Category cat; 
	
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
	
	public bool categoryMatch(Category category) {
		return (cat.Equals (category));
	}
	
	public string toString() {
		easyScoreAverage = easyScore / numEasy;
		mediumScoreAverage = mediumScore / numMedium;
		hardScoreAverage = hardScore / numHard;
		
		totalScoreAverage = (easyScore + mediumScore + hardScore) / (numEasy + numMedium + numHard);
		
		easyTimeAverage = easyTime / numEasy;
		mediumTimeAverage = mediumTime / numMedium;
		hardTimeaverage = hardTime / numHard; 
		
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
		return str;
	}
	
}