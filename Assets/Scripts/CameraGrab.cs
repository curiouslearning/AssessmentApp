﻿using UnityEngine;
using System;
using System.Collections;
using System.IO;
using UnityEngine.UI;
public class CameraGrab : MonoBehaviour {
	WebCamDevice[] devices;
	WebCamTexture tex;
	int pictureCount;
	public RawImage rawImage;
	public AspectRatioFitter rawImageARF;
	RectTransform rawImageRT;
	bool tookPicture;
	bool deviceFound;
	public ScoreTracker gameInfo;
	private string id;

	// Use this for initialization
	void Start () {
		pictureCount = 0;
		tookPicture = false;
		tex = new WebCamTexture ();
		rawImageRT = rawImage.rectTransform;
		deviceFound = false;
		id = gameInfo.returnUser();
	}

	void takeAPicture()
	{
		string identifier = id + System.DateTime.Today.ToString ("mm_dd_yyy-hh:mm:ss");
		int counter = 0;
		//yield return new WaitForEndOfFrame ();
		while (!tex.isPlaying && counter < 500) {
			counter++;
		}
		Texture2D photo = new Texture2D (tex.width, tex.height);
		pictureCount++;
		if (pictureCount >= 4) { //hack to prevent black photos
			photo.SetPixels (tex.GetPixels());
			photo.Apply ();
			byte[] bytes = photo.EncodeToPNG();
			Debug.Log ("writing to: " + Application.persistentDataPath);
			File.WriteAllBytes (Application.persistentDataPath + identifier + ".png", bytes);
			tookPicture = true;
			tex.Stop ();
		}

	}
	// Update is called once per frame
	void Update()
	{
		if (!deviceFound) {
			devices = WebCamTexture.devices;
			for (int i = 0; i < devices.Length; i++) {
				if (devices [i].isFrontFacing) {
					tex = new WebCamTexture (devices [i].name, 512,512);
					rawImage.texture = tex;
					rawImage.material.mainTexture = tex;
					tex.Play ();	
					deviceFound = true;
				}
			}
		}
		if ( tex.width < 100 )
		{
			Debug.Log("Still waiting another frame for correct info...");
			return;
		}

		// change as user rotates iPhone or Android:
		if (!tookPicture) {
			int cwNeeded = tex.videoRotationAngle;
			// Unity helpfully returns the _clockwise_ twist needed
			// guess nobody at Unity noticed their product works in counterclockwise:
			int ccwNeeded = -cwNeeded;

			// IF the image needs to be mirrored, it seems that it
			// ALSO needs to be spun. Strange: but true.
			if ( tex.videoVerticallyMirrored ) ccwNeeded += 180;

			// you'll be using a UI RawImage, so simply spin the RectTransform
			rawImageRT.localEulerAngles = new Vector3(0f,0f,ccwNeeded);

			float videoRatio = (float)tex.width/(float)tex.height;

			// you'll be using an AspectRatioFitter on the Image, so simply set it
			rawImageARF.aspectRatio = videoRatio;

			// alert, the ONLY way to mirror a RAW image, is, the uvRect.
			// changing the scale is completely broken.
			if ( tex.videoVerticallyMirrored )
				rawImage.uvRect = new Rect(1,0,-1,1);  // means flip on vertical axis
			else{
				rawImage.uvRect = new Rect(0,0,1,1);  // means no flip
			}
			
			takeAPicture ();
		}
		// devText.text =
		//  videoRotationAngle+"/"+ratio+"/"+tex.videoVerticallyMirrored;
	}
}
