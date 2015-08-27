using UnityEngine;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityTest;

[TestFixture]
public class ScoreTrackerTestSuite{
	int numWrong;
	int numCorrect;
	int numAnswered;
	int totalScore;
	Category currentCategory;
	Category lastCategory;
	Score s;
	float questionTime;
	int questionNumber;
	float startTime;
	List<Score> scoreList;
	bool gameOver;
	
	
	public ScoreTrackerTestSuite()
	{
		lastCategory = Category.BlendingWordIdentification;
		resetVars();
		scoreList = new List<Score>();
		s = new Score(4);
	}

	void resetVars ()
	{
		gameOver = false;
		numWrong = 0;
		numCorrect = 0;
		numAnswered = 0;
		totalScore = 0;
		questionNumber = 0;
	}


	void checkAnswer()
	{
		if (s.isCorrect ()) {
			totalScore++;
			numCorrect++;
			numWrong = 0; 
		} else {
			totalScore--;
			numWrong++; 
			numCorrect = 0;
		}
	}
		
	Category getNextCategory ()
	{
		switch (lastCategory)
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
			return Category.RhymingWordMatching;
			
		case Category.RhymingWordMatching:
			return Category.BlendingWordIdentification;
			
		case Category.BlendingWordIdentification:
			return Category.PseudowordMatching;
			
		case Category.PseudowordMatching:
			//endGame();
			return Category.PseudowordMatching;
		}
		return Category.Customization;	
	}

	void setCategory()
	{
		if (s.returnCategory().Equals(Category.Customization)) {//only ever spend one question in customization 	
			numCorrect = 0;
			numWrong = 0;
			numAnswered = 0;
			currentCategory = getNextCategory();
			s.setDifficulty(Difficulty.Easy);
		} else if (numWrong >= 4 || numAnswered  >= 20) { //change category and drop difficulty level after 4 wrong answers	
			lastCategory = currentCategory;
			s.setCategory (Category.Customization);
			currentCategory = Category.Customization;
		}
		
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

	void updateDifficulty()
	{
		if (s.returnDifficulty().Equals(Difficulty.Easy) || s.returnCategory().Equals (Difficulty.Medium)) {
			Difficulty diff = s.returnDifficulty();
			s.setDifficulty (getNextDifficulty(diff)); 
		} else
			s.setDifficulty (Difficulty.Hard);
	}


	void changeQuestion () {
		questionTime = 0;
		startTime = Time.time;
		questionNumber++;
		numAnswered++;
		
		checkAnswer();	
			
		
		if (numCorrect >= 3) {
			// If this case is true, the player has exhausted all available categories and difficulties
			if (s.returnCategory() == Category.PseudowordMatching && s.returnDifficulty() == Difficulty.Hard) {
				gameOver = true;
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
				updateDifficulty();
			}	
		} 
		else {
			Difficulty temp = s.returnDifficulty();
			scoreList.Add(s);
			s = new Score(questionNumber);
			s.setCategory(currentCategory);
			s.setDifficulty(temp);
			setCategory();
		}
		
		/*Debug.Log ("questionNUmber: " + questionNumber);
		stimOrgOb = spawnHolder.spawnNext(currentCategory,s.returnDifficulty(),questionNumber);
		Debug.Log("got a new SOO");
		sooHolder = stimOrgOb.GetComponent<SOOScript>();
		sooHolder.move(0);
		
		sendEvent (eType.NewQuestion);*/
	}
	



	[Test]
	public void testChangeQuestion()
	{
		s.addScore(true);
		s.setTimedOut(false);
		s.setDifficulty(Difficulty.Hard);
		s.setCategory(Category.PseudowordMatching);
		resetVars();
		totalScore = 3;
		numCorrect = 3;
		changeQuestion();
		Assert.That(totalScore == 4);
		Assert.That(gameOver == true);

		resetVars();
		totalScore = 3;
		numCorrect = 4;
		s.addScore(true);
		s.setDifficulty(Difficulty.Easy);
		changeQuestion();
		Assert.False(gameOver);
		Assert.That(numCorrect == 0);
		Assert.That (s.returnDifficulty() ==Difficulty.Medium);

		resetVars();
		totalScore = 3;
		numCorrect = 2;
		numWrong = 3;
		s.addScore(false);
		s.setTimedOut(true);
		s.setDifficulty(Difficulty.Easy);
		s.setCategory(Category.ReceptiveVocabulary);
		currentCategory = Category.ReceptiveVocabulary;
		changeQuestion();
		Assert.False(gameOver);
		Assert.That (currentCategory == Category.Customization);

		resetVars();
		numCorrect = 2;
		s.setTimedOut(true);
		lastCategory = Category.PseudowordMatching;
		s.setCategory(Category.Customization);	
		changeQuestion();
		Assert.That(currentCategory == Category.PseudowordMatching);	

		resetVars();
		numCorrect = 2;
		totalScore = 2;
		s.addScore(false);
		s.setTimedOut(false);
		lastCategory = Category.PseudowordMatching;
		s.setCategory(Category.Customization);	
		changeQuestion();
		Assert.That(currentCategory == Category.PseudowordMatching);
	
		resetVars();
		numCorrect = 2;
		totalScore = 2;
		s.addScore(true);
		s.setTimedOut(false);
		lastCategory = Category.PseudowordMatching;
		s.setCategory(Category.Customization);	
		changeQuestion();
		Assert.That(currentCategory == Category.PseudowordMatching);	
	}


	[Test]
	public void testCheckAnswer()
	{
		s.addScore(true);
		totalScore = 0;
		numCorrect = 0;
		numWrong = 0;
		checkAnswer();
		Assert.That(totalScore == 1);
		Assert.That(numCorrect == 1);
		Assert.That(numWrong == 0);
		s.addScore(false);
		checkAnswer();
		Assert.That(totalScore == 0);
		Assert.That(numCorrect == 0);
		Assert.That(numWrong == 1);
	}


	[Test]
	public void testGetNextDifficulty()
	{
		Assert.That(getNextDifficulty(Difficulty.Easy) == Difficulty.Medium);
		Assert.That(getNextDifficulty(Difficulty.Medium) == Difficulty.Hard);
		Assert.That(getNextDifficulty(Difficulty.Hard) == Difficulty.Hard);
	}


	[Test]
	public void testUpdateDifficulty()
	{
		s.setDifficulty(Difficulty.Easy);
		updateDifficulty();
		Assert.That(s.returnDifficulty() == Difficulty.Medium);
		updateDifficulty();		
		Assert.That(s.returnDifficulty() == Difficulty.Hard);
		updateDifficulty();
		Assert.That(s.returnDifficulty() == Difficulty.Hard);
	}


	[Test]
	public void testGetNextCategory()
	{
		lastCategory = Category.Customization;
		Assert.That(getNextCategory() == Category.ReceptiveVocabulary);
		lastCategory = Category.ReceptiveVocabulary;
		Assert.That(getNextCategory() == Category.LetterNameRecognition);
		lastCategory = Category.LetterNameRecognition;
		Assert.That(getNextCategory() == Category.LetterSoundMatching);
		lastCategory = Category.LetterSoundMatching;
		Assert.That(getNextCategory() == Category.CVCWordIdentification);
		lastCategory = Category.CVCWordIdentification;
		Assert.That(getNextCategory() == Category.SightWordIdentification);
		lastCategory = Category.SightWordIdentification;
		Assert.That(getNextCategory() == Category.RhymingWordMatching);
		lastCategory = Category.RhymingWordMatching;
		Assert.That(getNextCategory() == Category.BlendingWordIdentification);
		lastCategory = Category.BlendingWordIdentification;
		Assert.That(getNextCategory() == Category.PseudowordMatching);
		lastCategory = Category.PseudowordMatching;
		Assert.That(getNextCategory() == Category.PseudowordMatching);
	}


	[Test]
	public void testSetCategory ()
	{
		bool catIsRV = (currentCategory == Category.ReceptiveVocabulary);
		bool catIsCust = (currentCategory == Category.Customization);

		//Test first if Statement
		currentCategory = Category.Customization;
		s.setCategory(Category.Customization);
		numWrong = 20;
		numCorrect = 10;
		numAnswered = 80;
		setCategory();
		Assert.That(numCorrect== 0);
		Assert.That(numWrong == 0);
		Assert.That(numAnswered == 0);
		Assert.That(currentCategory == Category.PseudowordMatching);
		Assert.That (s.returnDifficulty() == Difficulty.Easy);

		//Test 2nd if statement: numWrong & !numAnswered
		currentCategory = Category.CVCWordIdentification;
		s.setCategory(Category.CVCWordIdentification);
		numWrong = 4;
		numCorrect = 5;
		numAnswered = 9;
		setCategory();
		Assert.That(catIsCust);
		Debug.Log(s.returnCategory());
		Assert.That (s.returnCategory() == Category.Customization);
		Assert.That (lastCategory == Category.CVCWordIdentification);

		//Test 2nd if statement: !numWrong & numAnswered
		currentCategory = Category.CVCWordIdentification;
		s.setCategory(Category.CVCWordIdentification);
		numWrong = 2;
		numCorrect = 18;
		numAnswered = 20;
		setCategory();
		Assert.That(catIsCust);
		Assert.That (lastCategory == Category.CVCWordIdentification);
		Assert.That (s.returnCategory() == Category.Customization);
		
		//Test 2nd if statement: numWrong & numAnswered
		currentCategory = Category.CVCWordIdentification;
		s.setCategory(Category.CVCWordIdentification);
		numWrong = 4;
		numCorrect = 16;
		numAnswered = 20;
		setCategory();
		Assert.That(catIsCust);
		Assert.That (lastCategory == Category.CVCWordIdentification);
		Assert.That (s.returnCategory() == Category.Customization);					
	}

	
}
