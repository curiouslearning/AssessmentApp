using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckAssets : MonoBehaviour {
	public TextAsset stimList;
	string sourceLines;
	List <serStim> stimPool;
	Dictionary <string, Difficulty> diffParser;
	Dictionary <string, Category> catParser;

	void Awake()
	{
		stimPool = new List<serStim>();
		diffParser = new Dictionary<string, Difficulty>();
		diffParser.Add ("Easy", Difficulty.Easy);
		diffParser.Add ("Medium", Difficulty.Medium);
		diffParser.Add ("Hard", Difficulty.Hard);
		initCatParser();
		parseData();
		checkAssets();
	}
	void parseData()
	{ 
		string[] sourceLines = stimList.text.Split('\n');
		for(int i = 1; i < sourceLines.Length; i++)
		{
			serStim data = new serStim();
			string [] vals = sourceLines[i].Split(',');
			data.hasBeenTarget = false;
			if(vals[1] == "audio")
			{
				data.audio = vals[0];
			}
			else
			{
				data.sprite = vals[0];
			}
			data.stimType = vals[1];
			data.difficulty = diffParser[vals[2].TrimEnd('\r')];	
			data.hostStimType = vals[3];
			data.hostStim = vals[4];  //what the host shows/says when this stimulus is the target
			data.category = catParser[vals[5].TrimEnd('\r')];
			stimPool.Add(data);
		}
	}
	void initCatParser()	
	{
		catParser = new Dictionary<string, Category>();
		catParser.Add("ReceptiveVocabulary",Category.ReceptiveVocabulary);
		catParser.Add("BlendingWordIdentification",Category.BlendingWordIdentification);
		catParser.Add("Customization",Category.Customization);
		catParser.Add("CVCWordIdentification",Category.CVCWordIdentification);
		catParser.Add("LetterNameID",Category.LetterNameRecognition);
		catParser.Add("LetterSoundMatching",Category.LetterSoundMatching);
		catParser.Add("PseudoWordMatching",Category.PseudowordMatching);
		catParser.Add("RhymingWordMatching",Category.RhymingWordMatching);
		catParser.Add("SightWordIdentification",Category.SightWordIdentification);
	}

	void checkAssets()
	{
		Sprite s = new Sprite();
		for (int i = 0; i< stimPool.Count; i++)
		{
			if(stimPool[i].stimType.Equals("visual"))
			{
				s = Resources.Load<Sprite>("Art/" + stimPool[i].sprite);

			}
			else if (stimPool[i].category.Equals(Category.LetterNameRecognition)){
				s = Resources.Load<Sprite>("Art/" + stimPool[i].hostStim);
			}
			if(s.Equals(null))
					continue;
			Debug.Log(s.ToString() + ": ");
			float x = s.rect.xMax - s.rect.xMin;
			float y = s.rect.yMax - s.rect.yMin;
			Debug.Log("\t" + y + " , " + x);
		}
	}
}
