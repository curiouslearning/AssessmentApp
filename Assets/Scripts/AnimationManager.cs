using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Animation manager Extends Observer.
/// A component for GameObjects that contain animations. Monitors the event
/// system and organizes the firing of animations and changing of customizable options.
/// </summary>
public class AnimationManager : Observer {
	Animator animator;
	public Animator squareCard;
	public Animator rectangleCard;
	Material square;
	Material rectangle;
	public GameObject[] subjects;
	public TextAsset optionsList;
	const int NUMBODYPARTS = 7;
	const int NUMOPTIONS = 10;
	public GameObject[] bodyParts;
	public Texture2D[][] optionTextures;
	Dictionary <string, Texture2D> optionDict; //for fast lookup of a selected texture
	public int defaultPos; //standard index for the default texture for each body part
	string[] sourceLines;
	bool[] bodyPartCustomized; //tracks which options have been customized
	

	// Use this for initialization
	void Awake () {
		optionDict = new Dictionary<string, Texture2D>();
		bodyPartCustomized = new bool[NUMBODYPARTS];
		optionTextures = new Texture2D[NUMBODYPARTS][];
		for(int i = 0; i < NUMBODYPARTS; i++)
		{
			bodyPartCustomized[i] = false;
			optionTextures[i] = new Texture2D[NUMOPTIONS];
		}		
		animator = GetComponent<Animator>();
		square = squareCard.GetComponent<MeshRenderer>().material;
		rectangle = rectangleCard.GetComponent<MeshRenderer>().material;
		initTextures();
	
	}
	void Start()
	{		
		addSelfToSubjects();	
	}

	/// <summary>
	/// Function for initializing the Observer design pattern.
	//  NOTE: ANY NEW SUBJECT MUST BE MANUALLY INSERTED INTO THE ARRAY EITHER IN THE EDITOR OR IN START()
	/// </summary>
	void addSelfToSubjects()
	{
		GameObject temp;
		for (int i = 0; i < subjects.Length; i++)
		{
			temp = subjects[i];
			temp.GetComponent<Subject>().addObserver(new Subject.GameObjectNotify(this.onNotify));
			
		}
	}	
	public override void registerGameObjectWithSoo(GameObject SOO)
	{
		base.registerGameObjectWithSoo(SOO);
	}

	/// <summary>
	/// Parses the optionsList data into the textures array.
	/// </summary>
	void parseData()
	{
		sourceLines = optionsList.text.Split('\n');
		for (int i= 1; i < sourceLines.Length; i++)
		{
			//index each texture by its body part and its position in the options hierarchy (0 is default texture)
			string[] values = sourceLines[i].Split(',');
			int bodyPart = int.Parse(values[1]);			
			int texturePosition = int.Parse(values[2]);
			Texture2D t =  Resources.Load<Texture2D>("Textures/" + values[0]);
			optionTextures[bodyPart][texturePosition] = t;
			optionDict.Add(values[0], t);

		}
	}

	public void setHostMedia (serStim m)
	{
		
		if (m.hostStimType== "audio")
		{
			AudioClip audio = Resources.Load<AudioClip>("Audio/" + m.hostStim);	
			if(audio == null)
			{
				Debug.LogError("Audio did not load properly");
			}
			setHostMediaInternal (audio);
		}
		else{
			//Debug.Log("vis stim with name: " + m.hostStim);
			Sprite sprite = Resources.Load<Sprite>("Art/" + m.hostStim);
			setHostMediaInternal (sprite);
		}
	}

	void setHostMediaInternal (AudioClip a)
	{
		AudioSource host = GetComponent<AudioSource>();
		host.clip = a;
		square.mainTexture = null;
		rectangle.mainTexture = null;
	}
	void setHostMediaInternal (Sprite s)
	{
		if ((s.rect.xMax-s.rect.xMin) >= (s.rect.yMax-s.rect.yMin +50)) //if the texture is a rectangle
		{
			rectangle.mainTexture = s.texture;
			square.mainTexture = null;
		}
		else {
			square.mainTexture = s.texture;
			rectangle.mainTexture = null;
		}
	}
	/// <summary>
	/// Creates atlases for each body part, sets the UVs to default values
	/// </summary>
	void initTextures ()
	{
		parseData();
		for (int i =0; i< bodyParts.Length; i++)
		{
			//pack textures for body part into an atlas, store dimensions in a Rect
			SkinnedMeshRenderer bodyPart = bodyParts[i].GetComponent<SkinnedMeshRenderer>();
			bodyPart.material.mainTexture = optionTextures[i][defaultPos];
			/*atlas = new Texture2D(atlasDimensions, atlasDimensions);
			texturePositions[i] = atlas.PackTextures(optionTextures[i], padding, atlasSize);
			atlases[i] = atlas;
			//set bodyPart texture to the default texture in the atlas
			bodyPart.material.mainTexture=atlas;
			bodyPart.material.mainTextureScale = new Vector2 (texturePositions[i][defaultPos].x, texturePositions[i][defaultPos].y);*/
			
		}		
	}


	/// <summary>
	/// Method for handling events this class is listening for
	/// </summary>
	/// <param name="e">Event Instance.</param>
	public override void onNotify (EventInstance<GameObject> e)
	{	
		if (e.type == eType.Selected || e.type == eType.TimedOut)
		{
			animator.ResetTrigger("Landed");
			animator.SetTrigger("Success");
			GetComponent<AudioSource>().clip = null;
			if(e.type == eType.Selected)
			{
				if(e.signaler.GetComponent<StimulusScript>() != null && e.signaler.GetComponent<StimulusScript>().isOption()) //if selected object is a body part
				{
					StimulusScript s = e.signaler.GetComponent<StimulusScript>();
					changeBodyPart(s.getBodyPart(), s.getTextureName());
				}
			}
			return;
		}
		if (e.type == eType.Grab)
		{
			animator.SetTrigger("Point");
		}
		if (e.type == eType.Ready)
		{
			animator.SetTrigger("Landed");
			animator.SetTrigger("Highlight");
			if(GetComponent<AudioSource>().clip != null)
			{
				GetComponent<AudioSource>().Play();
			}
			if(square.mainTexture != null)
			{
				animator.SetTrigger("ShowCard");
				squareCard.SetTrigger("ShowCard");
			}
			else if (rectangle.mainTexture != null)  //Commented out while rectangle card is nonexistent
			{
				animator.SetTrigger("ShowCard");
				rectangleCard.SetTrigger("ShowCard");
				
			}
		}
	}

	
	/// <summary>
	/// Changes the body part.
	/// </summary>
	/// <param name="part">Index of the bone to be changed in bodyParts.</param>
	/// <param name="newTexture"> Replacement Texture.</param>
	void changeBodyPart (int part, string newTexture)
	{
		GameObject temp = bodyParts[part];	
		temp.GetComponent<SkinnedMeshRenderer>().material.mainTexture = optionDict[newTexture];
	}

	/// <summary>
	/// Gets the options for the first not-customized body part.
	/// </summary>
	/// <returns>texture options converted into sprites.</returns>
	public List<Sprite> getOptions ()
	{
		List<Sprite> options;
		options = new List<Sprite>();
		int curBodyPart = getBodyPartInternal();
		Texture2D[] textures = optionTextures[curBodyPart];
		for(int j = 0; j < textures.Length; j++)  //convert and package options
		{
			if(textures[j] != null)
			{
				Sprite s = Sprite.Create(textures[j], new Rect(0,0, textures[j].width, textures[j].height), new Vector2 (0.5f, 0.5f));
				s.name = textures[j].name;
				options.Add(s); 
			}
		}	
		return options;
	}

	
	/// <summary>
	/// Returns the index of the first not-yet-customized body part.
	/// </summary>
	/// <returns>Body part index.</returns>
	public int getBodyPart ()
	{
		int i =0;
		while (i < bodyPartCustomized.Length && bodyPartCustomized[i] == true ) {
			i++;
		}	
		
		return i-1;
	}
	int getBodyPartInternal()
	{
		int i =0;
		while (i < bodyPartCustomized.Length && bodyPartCustomized[i] == true ) {i++;}
		if (i >= bodyPartCustomized.Length) //reset body part list to maintain customizability
		{
			for (int j = 1; j < bodyPartCustomized.Length; j++)
			{
				bodyPartCustomized[j] = false;
			}
			i = 0;
		}
		bodyPartCustomized[i] = true;
		return i;
	}
	// Update is called once per frame
	void Update () {

	}
}
