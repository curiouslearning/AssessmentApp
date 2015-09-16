using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Serializable class to store stimulus data for question data persistance
/// </summary>
[Serializable]
public class serStim{
	public bool isCorrect;
	public bool hasBeenTarget;
	public string audio;
	public string sprite;
	public string stimType;
	public string hostStim;
	public string hostStimType;
	public bool isDraggable;
	public Difficulty difficulty;
	public Category category; 
}
/// <summary>
/// Serializable class to store customization event data for question data persistance
/// </summary>
public class serCustomizationOption{
	public Sprite texture;
	public bool isDraggable;
	public int bodyPart;
}	


/// <summary>
/// A class to hold information about each question in the assessment.
/// Extends Scriptable Object.
/// </summary>

/*
 * Contains:
 * 		serStim array of stimuli
 * 		prompt for host
 *		question number
 * initialized by fileWrapper, passed to Spawner by GameManagerScript for question initialization
 * Stored in a GameManagerScript Queue
 */
public class Question : ScriptableObject {
	
	int questionNumber;
	List<serStim> stimuli;
	List<serCustomizationOption> options;
	serCustomizationOption tempOption;
	serStim tempStim;
	serStim prompt;
	bool customizationEvent;
	Category cat;

	void Awake ()
	{
		customizationEvent = false;
		stimuli = new List<serStim>();
		options = new List<serCustomizationOption>();
	}

	/// <summary>
	/// returns the question's category.
	/// </summary>
	/// <returns>Category cat.</returns>
	public Category getCat ()
	{
		return cat;
	}
	
	/// <summary>
	/// Init the question for a standard question event.
	/// </summary>
	/// <param name="num">Question Number</param>
	/// <param name="sprites">Visual stimuli</param>
	/// <param name="sounds">Auditory stimuli</param>
	/// <param name="correct">Target stimulus</param>
	/// <param name="cat">Question category</param>
	public void init (int qNum, List<serStim> stimList, Category c)
	{
		questionNumber = qNum;
		customizationEvent = false;
		cat = c;
		
		stimuli = stimList;
		for (int i = 0; i<stimuli.Count; i++)
		{
			if(stimuli[i] != null)
				stimuli[i].isDraggable = true;
			else {
				Debug.LogError("null stimuli exception");
			}
		}
		
	}
	/// <summary>
	/// Init the question for a customization event
	/// </summary>
	/// <param name="num"> Question Number.</param>
	/// <param name="textures">Option Textures.</param>
	/// <param name="bodyPart">Index of body part being customized.</param>
	public void init (int qNum, List<Sprite> textures, int bodyPart)
	{
		questionNumber = qNum;
		customizationEvent = true;
		for (int i = 0; i<textures.Count; i++)
		{
			tempOption = new serCustomizationOption();
			tempOption.texture = textures[i];
			tempOption.isDraggable = true;
			tempOption.bodyPart = bodyPart;
			options.Add(tempOption);
		}
		if (options.Count < 4)
		{
			addBlanks((4-options.Count));
		}
		
	}

	/// <summary>
	/// Adds blank options to fully populate SOO.
	/// </summary>
	/// <param name="count">4 - the options list count.</param>
	void addBlanks(int count)
	{
		tempOption = new serCustomizationOption();
		for (int i = 0; i < count; i++)
		{
			tempOption.isDraggable = false;
			options.Add(tempOption);
		}
	}
	/// <summary>
	/// Gets the indicated stimulus
	/// </summary>
	/// <returns>indicated stimulus</returns>
	/// <param name="index">Index of desired stimulus</param>
	public serStim getStim (int index) {
		serStim butts = new serStim();
		if(stimuli[index] != null)
			return stimuli[index];
		else {
			Debug.LogError("returning blank stim");
		}
		return butts;
	}
	/// <summary>
	/// Gets the indicated option.
	/// </summary>
	/// <returns>Indicated option.</returns>
	/// <param name="index">Index of desired option.</param>
	public serCustomizationOption getOption (int index) {return options[index];}
	
	/// <summary>
	/// Returns the option list count.
	/// </summary>
	/// <returns>option list count.</returns>
	public int countOptions ()
	{
		return options.Count;
	}
	/// <summary>
	/// Gets the question number.
	/// </summary>
	/// <returns>Question number.</returns>
	public int getNumber () {return questionNumber;}
	/// <summary>
	/// Gets the question category.
	/// </summary>
	/// <returns>Category enum.</returns>
	public bool isCustomizationEvent () {return customizationEvent;}
}
