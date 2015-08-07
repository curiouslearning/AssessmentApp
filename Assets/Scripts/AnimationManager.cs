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
	public GameObject[] subjects;
	public TextAsset optionsList;
	const int NUMBODYPARTS = 7;
	const int NUMOPTIONS = 10;
	public GameObject[] bodyParts;
	public Texture2D[][] optionTextures;
	Texture2D[] atlases;
	Texture2D atlas;
	Rect[][] texturePositions;
	public int defaultPos; //standard index for the default texture for each body part
	public int atlasDimensions;
	public int padding;
	int atlasSize;
	string[] sourceLines;
	bool[] bodyPartCustomized; //tracks which options have been customized
	

	// Use this for initialization
	void Awake () {
		atlasSize = (atlasDimensions+padding)^2;
		bodyPartCustomized = new bool[NUMBODYPARTS];
		optionTextures = new Texture2D[NUMBODYPARTS][];
		atlases = new Texture2D[NUMBODYPARTS];
		texturePositions = new Rect[NUMBODYPARTS][];
		for(int i = 0; i < NUMBODYPARTS; i++)
		{
			bodyPartCustomized[i] = false;
			optionTextures[i] = new Texture2D[NUMOPTIONS];	
		}		
		animator = GetComponent<Animator>();
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
			optionTextures[bodyPart][texturePosition] = Resources.Load<Texture2D>("Textures/" + values[0]);

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
			atlas = new Texture2D(atlasDimensions, atlasDimensions);
			texturePositions[i] = atlas.PackTextures(optionTextures[i], padding, atlasSize);
			atlases[i] = atlas;
			//set bodyPart texture to the default texture in the atlas
//			bodyPart.material.mainTexture=atlas;
//			bodyPart.material.mainTextureScale = new Vector2 (texturePositions[i][defaultPos].x, texturePositions[i][defaultPos].y);
			
		}
		bodyParts[2].GetComponent<SkinnedMeshRenderer>().material.mainTexture = atlases[2];
		bodyParts[2].GetComponent<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2 (texturePositions[2][defaultPos].width, texturePositions[2][defaultPos].height);
		bodyParts[2].GetComponent<SkinnedMeshRenderer>().material.mainTextureOffset = new Vector2 (texturePositions[2][defaultPos].x, texturePositions[2][defaultPos].y);

		
	}


	/// <summary>
	/// Method for handling events this class is listening for
	/// </summary>
	/// <param name="e">Event Instance.</param>
	public override void onNotify (EventInstance<GameObject> e)
	{
		if(e.type == eType.Selected && e.signaler.GetComponent<StimulusScript>().isOption())
		{
			int bodyPart = e.signaler.GetComponent<StimulusScript>().getBodyPart();
			Texture2D newTexture = Resources.Load<Texture2D>("Textures/" + e.signaler.GetComponent<StimulusScript>().getTextureName());
			changeBodyPart( bodyPart, newTexture);
			//grab texture info and send it to swapper	
		}
		else if (e.type == eType.Selected && !e.signaler.GetComponent<StimulusScript>().isOption())
		{
			animator.SetTrigger("Success");
		}
	}

	
	/// <summary>
	/// Changes the body part.
	/// </summary>
	/// <param name="part">Index of the bone to be changed in bodyParts.</param>
	/// <param name="newTexture"> Replacement Texture.</param>
	void changeBodyPart (int part, Texture2D newTexture)
	{
		GameObject temp = bodyParts[part];
		temp.GetComponent<SkinnedMeshRenderer>().material.mainTexture = newTexture;
	}

	/// <summary>
	/// Gets the options for the first not-customized body part.
	/// </summary>
	/// <returns>texture options converted into sprites.</returns>
	public List<Sprite> getOptions ()
	{
		List<Sprite> options;
		options = new List<Sprite>();
		int i = 0;
		while(bodyPartCustomized[i] != true){i++;} //find the first non-customized body part
		Texture2D[] textures = optionTextures[i];
		for(int j = 0; j < textures.Length; j++)  //convert and package options
		{
			Sprite s = Sprite.Create(textures[i], new Rect(0,0, textures[i].width, textures[i].height), new Vector2 (0.5f, 0.5f));
			options.Add(s); 
		}
		bodyPartCustomized[i] = true;
		return options;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
