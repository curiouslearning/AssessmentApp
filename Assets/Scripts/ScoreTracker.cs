using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ScoreTracker : Observer {

	//timekeeping variables
	public int questionNumber;
	public float questionTime;
	public float firstLimit;
	public float secondLimit;
	public float pointTime;
	float timeLimit;
	float startTime;
	private bool firstTouchHappened;
	private bool pauseTimer;
	private bool broadcastSent;
	
	//component variables
	public GameObject spawner;
	SpawnerScript spawnHolder;
	public GameObject receptacle; 
	public GameObject gCollector;
	GameObject stimOrgOb;
	SOOScript sooHolder;
	Animator animator;

	//Event variables
	public Subject eventHandler;

	// List of questions scores
	public List<Score> scoreList;
	
	// scorekeeping variables
	bool gameOver;
	int totalScore;
	int numCorrect;
	int numWrong;
	int numAnswered;
	Category currentCategory;
	Category lastCategory;
	Score s;
	
	Subject.GameObjectNotify gOObserver;

	// ***********************************************
	// Initialization - Awake and Start
	// ***********************************************

	void Awake ()
	{
        s = new Score(questionNumber);	
		s.setTimedOut (false);
		s.setDifficulty (Difficulty.Easy);
		s.setCategory (Category.Customization);
		currentCategory = s.returnCategory ();
		lastCategory = currentCategory;
	}	
	void Start () {	
		animator = GameObject.Find ("MainCharacter").GetComponent<Animator>();
		gameOver = false;
		scoreList = new List<Score>();
		addSubjects();
		questionNumber = 0;
		questionTime = 0f;
		startTime = Time.time;
		timeLimit = firstLimit;
		firstTouchHappened = false;
		broadcastSent = false;
		pauseTimer = false;
		startQuestion ();
	}

	void addSubjects ()
	{
		gOObserver = new Subject.GameObjectNotify (this.onNotify);
		receptacle.GetComponent<Subject> ().addObserver (gOObserver); 
		CollisionNotification trashHolder;
		spawnHolder = spawner.GetComponent<SpawnerScript>();
		trashHolder = receptacle.GetComponent<CollisionNotification>();
		trashHolder.sub.addObserver(new Subject.GameObjectNotify(this.onNotify));
		trashHolder = gCollector.GetComponent<CollisionNotification>();	
		trashHolder.sub.addObserver(new Subject.GameObjectNotify(this.onNotify));
		this.GetComponent<TouchProcessor>().eventWrapper.addObserver(new Subject.GameObjectNotify(this.onNotify));
	}

	// ********************************************************
	// onNotify and endGame
	// ********************************************************
	
	public override void onNotify (EventInstance<GameObject> e)
	{
		//s.addTime(questionTime);	
		if(e.type == eType.FingerDown|| e.type == eType.Drag)
		{
			if(!firstTouchHappened)
				firstTouchHappened = true;
		}
		if(e.type == eType.Grab)
		{
			pauseTimer = true;
		}
		if(e.type == eType.FingerUp)
		{
			pauseTimer = false;
		}
		else if (e.type == eType.Trashed)
		{
			s.addTime(questionTime);

			Destroy(e.signaler);
			//don't end the world
			
			//figure out how to make this happen after score tracker updates category
			changeQuestion();
			return; //prevent repeated action on same event
		}
		else if (e.type == eType.Selected)
		{
			if(e.signaler.GetComponent<StimulusScript>().returnIsTarget())
			{
				Debug.Log("Caught a correct stimulus");
				s.addScore(true);
			}
			else{s.addScore(false);}

			e.signaler.gameObject.SetActive(false);
			sooHolder.move(1);
			return;
		}
	}

	void endGame ()
	{
		eventHandler.sendEvent (eType.EndGame);

		//Printing it to the debug log allows us to see the entire
		//string that will be sent in the broadcast, which contains
		//all the data collected from the latest game.

	}
	
	public void addTouch (TouchSummary t) // called by TouchProcessor
	{
		s.addTouch(t);
		t = null;
		//changeQuestion();
	}
	
		
	void checkAnswer()
	{
		string response;
		string value;
		if (s.isCorrect()) {
			totalScore++;
			numCorrect++;
			numWrong = 0; 
			response = "correct";
			Debug.Log("correct! NumWrong: " + numWrong);
		} else {
			totalScore--;
			numWrong++; 
			numCorrect = 0;
			response = "incorrect";
			Debug.Log("incorrect! NumWrong: " + numWrong);
		}
		//data recording
		value = ("Question: " + s.getNum().ToString() + ", Result: " + response + ", time: " + s.getTime().ToString() + ", total score: " + totalScore);
		AndroidBroadcastIntentHandler.BroadcastJSONData("Question Answer", value);
	}
	void updateDifficulty()
	{
		if (s.returnDifficulty().Equals(Difficulty.Easy) || s.returnCategory().Equals (Difficulty.Medium)) {
			Difficulty diff = s.returnDifficulty();
			s.setDifficulty (getNextDifficulty(diff));
			AndroidBroadcastIntentHandler.BroadcastJSONData("Difficulty Change", diff.ToString());  //data recording
		} else
			s.setDifficulty (Difficulty.Hard);
			AndroidBroadcastIntentHandler.BroadcastJSONData("Difficulty Change", "Hard");  //data recording
	}
	

	void setCategory()
	{
		if (s.returnCategory().Equals(Category.Customization)) {//only ever spend one question in customization 	
			numCorrect = 0;
			numWrong = 0;
			numAnswered = 0;
			currentCategory = getNextCategory(lastCategory);
			s.setDifficulty(Difficulty.Easy);
			AndroidBroadcastIntentHandler.BroadcastJSONData("Difficulty Change", "Easy");  //data recording
			AndroidBroadcastIntentHandler.BroadcastJSONData("Category Change", lastCategory.ToString()); //data recording
		} else if (numWrong >= 4 || numAnswered  >= 20) { //change category and drop difficulty level after 4 wrong answers	
			lastCategory = currentCategory;
			s.setCategory (Category.Customization);
			currentCategory = Category.Customization;
			AndroidBroadcastIntentHandler.BroadcastJSONData("Category Change", "Customization"); //data recording
		}

	}

	Category getNextCategory (Category last)
	{
		switch (last)
		{
			case Category.Customization:
				return Category.ReceptiveVocabulary;

			case Category.ReceptiveVocabulary:
				return Category.LetterNameRecognition;	

			case Category.LetterNameRecognition:
				return Category.LetterSoundMatching;
			
			case Category.LetterSoundMatching:
				return Category.CVCWordIdentification;
	
			case Category.CVCWordIdentification:
				return Category.SightWordIdentification;

			case Category.SightWordIdentification:
				return Category.BlendingWordIdentification; //HACK UNTIL RHYMING CASE IS IMPLEMENTED

			case Category.RhymingWordMatching:
				return Category.BlendingWordIdentification;

			case Category.BlendingWordIdentification:
				return Category.PseudowordMatching;
	
			case Category.PseudowordMatching:
				gameOver = true;
				endGame();
				return Category.PseudowordMatching;
		}
		return Category.Customization;	
	}

	Difficulty getNextDifficulty(Difficulty curDiff)
	{
		switch(curDiff)
		{
		case Difficulty.Easy:
			return Difficulty.Medium;
		case Difficulty.Medium:
			return Difficulty.Hard;
		case Difficulty.Hard:
			return Difficulty.Hard;
		}
		return curDiff;
	}

	public Difficulty resetDifficulty (Difficulty curDiff)
	{	
		switch(curDiff)
		{
		case Difficulty.Easy:
			return Difficulty.Easy;
		case Difficulty.Medium:
			return Difficulty.Easy;
		case Difficulty.Hard:
			return Difficulty.Medium;
		}
		return curDiff;
	}
	
	// *******************************************************
	// startQuestion, changeQuestion, Update
	// *******************************************************

	// for use at the beginning of the game
	void startQuestion() {
		//sendEvent (eType.NewQuestion);
		stimOrgOb = spawnHolder.spawnNext(currentCategory,s.returnDifficulty(),questionNumber);
		sooHolder = stimOrgOb.GetComponent<SOOScript>();
		sooHolder.move(0);
	} 
	
	void changeQuestion () {
		timeLimit = firstLimit;
		questionTime = 0;
		startTime = Time.time;
		questionNumber++;
		numAnswered++;

		checkAnswer();	

		
		if (numCorrect >= 3) {
			// If this case is true, the player has exhausted all available categories and difficulties
			if (s.returnCategory() == Category.PseudowordMatching && s.returnDifficulty() == Difficulty.Hard) {
				Debug.Log("Game over!");
				gameOver = true;
				endGame();
				return;
			} else {
				// if the player answers three consecutive questions correctly, numCorrect is
				// reset and an event notification of type ChangeDifficulty is sent out, which
				// will be picked up by GameManager.
				numCorrect = 0;
				Difficulty temp = s.returnDifficulty();
				scoreList.Add(s);
				s = new Score(questionNumber);
				s.setCategory(currentCategory);
				s.setDifficulty(temp);
				// the difficulty and category variables in the current score variable
				// must also be adjusted appropriately.
				//updateDifficulty();
			}	
		} 
		else {
			Difficulty temp = s.returnDifficulty();
			scoreList.Add(s);
			s = new Score(questionNumber);
			s.setCategory(currentCategory);
			s.setDifficulty(temp);
			setCategory();
			if(gameOver)
				return;
		}

		stimOrgOb = spawnHolder.spawnNext(currentCategory,s.returnDifficulty(),questionNumber);
		firstTouchHappened = false;
		broadcastSent = false;
		sooHolder = stimOrgOb.GetComponent<SOOScript>();
		sooHolder.move(0);

		//data recording
		eventHandler.sendEvent (eType.NewQuestion);
		string value = "Question Number: " + questionNumber + ", Category: " + currentCategory + ", Difficulty: " + s.returnDifficulty();
		AndroidBroadcastIntentHandler.BroadcastJSONData("New Question", value);
	}
	

	void Update() 
	{
		if (gameOver) {
			string junk = "meaningless";
			return;
		}
		// questionTime keeps track of the elapsed time since the
		// start of the current question.  It must be updated
		// frequently, which is why it is placed in Update().
		while(pauseTimer)
		{
			timeLimit += Time.deltaTime;
			return;
		}
		questionTime = Time.time - startTime;
		// if questionTime goes over 15 seconds it sends a
		// TimeOut (which will be picked up in ScoreTracker)
		// event and moves on to the next question, calling
		// move(1) on sooHolder
		if(questionTime >= pointTime)
		{
			animator.SetTrigger("Point");
			pointTime += Time.time;
		}
		if (questionTime >= timeLimit){
			startTime = Time.time;

			s.setTimedOut(true);
			s.addTime (timeLimit);
			s.addScore (false);
		
			AndroidBroadcastIntentHandler.BroadcastJSONData ("TimeOut", ("Question Number: " + questionNumber.ToString() + ", Category: " + currentCategory.ToString() + ", Difficulty: " + s.returnDifficulty().ToString()));
			eventHandler.sendEvent (eType.TimedOut); // temporary fix here

			sooHolder.move (1);
		}
		else if (firstTouchHappened && !broadcastSent)
		{
			string value = ("First Touch for Question " + questionNumber.ToString() + " Occurred at " + Time.time.ToString());
			questionTime = 0;
			startTime = Time.time;
			timeLimit = secondLimit;
			AndroidBroadcastIntentHandler.BroadcastJSONData("First Touch", value);
			broadcastSent = true;
		}
		//if scene is changing do not process input
		//otherwise generate input commands and pass them to the proper objects
		// if 
		
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
	
	// ******************************************************************
	// Methods for organizing data collected by ScoreTracker and
	// placing it into strings
	// ******************************************************************
	
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
		
		DifficultyData[] ddarray = new DifficultyData[9];
		Category cat = Category.Customization;
		for (int i = 0; i < ddarray.Length; i++)
		{
			ddarray[i] = new DifficultyData(cat);
			cat = getNextCategory(cat);
		}
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
}

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

// Category is an enum that holds all possible kinds of
// questions that the app may ask a student.  Customization
// is included as a category so the character customization
// at the beginning of gameplay will be recognized as its
// own kind of activity;
/// <summary>
/// Enum containing all possible kinds of questions that the app may ask a student.
/// </summary>
public enum Category {Customization,
	ReceptiveVocabulary, 
	LetterNameRecognition, 
	LetterSoundMatching, 
	CVCWordIdentification, 
	SightWordIdentification, 
	RhymingWordMatching,
	BlendingWordIdentification, 
	PseudowordMatching};

/// <summary>
/// Inidicator for question difficulty.
/// </summary>
public enum Difficulty {Easy, Medium, Hard};


//Class that organizes and collects all information regarding performance on questions. Individual question data is stored in instances of
//the score class, all of which are stored in the List<Score> contained within the class.
//Directly coupled with TouchProcessor
//Observes:			(Update this list with all new additions
//			GameManagerScript
//			Receptacle


// Public class for concatenating various average scores and times to be used
// in broadcast intent.  ScoreTracker creates one instance of DifficultyData
// for each category and stores data on the student's performance for each
// category in those instances.

public class DifficultyData {

	// ints for holding combined scores for each level of difficulty 
	float easyScore;
	float mediumScore;
	float hardScore;

	// floats for holding combined times for each level of difficulty
	float easyTime;
	float mediumTime;
	float hardTime;

	// ints for holding the number of scores per level of difficulty
	int numEasy;
	int numMedium;
	int numHard;

	// ints and floats for holding calculated averages
	float easyScoreAverage;
	float mediumScoreAverage;
	float hardScoreAverage;
	
	float totalScoreAverage;
	
	float easyTimeAverage;
	float mediumTimeAverage;
	float hardTimeaverage; 
	
	float totalTimeAverage;
	
	Category cat; 

	// Constructor takes an instance of the Category enum so that instances
	// of DifficultyData are tagged with their corresponding Category
	public DifficultyData(Category category) {
		easyScore = 0f;
		mediumScore = 0f;
		hardScore = 0f;
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