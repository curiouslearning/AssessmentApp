using UnityEngine;
using System.Collections;
using SmoothMoves;

/// <summary>
/// Animation manager Extends Observer.
/// A component for GameObjects that contain animations. Monitors the event
/// system and organizes the firing of animations and changing of customizable options.
/// </summary>
public class AnimationManager : Observer {
	Animator animator;
	public GameObject[] subjects;
	public TextAsset optionsList;
	public GameObject[] bodyParts;
	public Texture2D[][] textures;
	Texture2D[] atlases;
	Texture2D atlas;
	Rect[][] texturePositions;
	public int defaultPos; //standard index for the default texture for each body part
	public int atlasDimensions;
	public int padding;
	public int atlasSize;
	

	// Use this for initialization
	void Awake () {
		animator = GetComponent<Animator>();
		initTextures();
		//init atlases here
	
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
	/// Overloaded method for handling events this class is listening for
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

	void initTextures ()
	{
		for (int i =0; i< bodyParts.Length; i++)
		{
			//pack textures for body part into an atlas, store dimensions in a Rect
			SkinnedMeshRenderer bodyPart = bodyParts[i].GetComponent<SkinnedMeshRenderer>();
			atlas = new Texture2D(atlasDimensions, atlasDimensions);
			texturePositions[i] = atlas.PackTextures(textures[i], padding, atlasSize);
			atlases[i] = atlas;
			//set bodyPart texture to the default texture in the atlas
			bodyPart.material.mainTexture=atlas;
			bodyPart.material.mainTextureScale = new Vector2 (texturePositions[i][defaultPos].x, texturePositions[i][defaultPos].y);
			
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
	
	// Update is called once per frame
	void Update () {

	}
}
