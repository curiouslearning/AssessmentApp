using UnityEngine;
using System;
using System.IO;
using System.Collections;
using FourthSky.Android;
using System.Collections.Generic;

public class AndroidBroadcastIntentHandler : MonoBehaviour {

	private static string packageName;
	private static readonly string testBroadcastAction = "org.curiouslearning.AssessmentApp";
	private static readonly string incomingAction = "edu.mit.media.prg.ros_android_intents.ros_to_intent";
	private BroadcastReceiver testBroadcastReceiver;
	private static string broadcastMessage = "no message recieved yet";
	public static List<GameObject> listeners = new List<GameObject>();
	
	public static int tablet_id = 1;


	
	// Use this for initialization
	void Awake () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_ANDROID
		if (!Application.isEditor) {
			//packageName = AndroidSystem.UnityActivity.Call<string>("getPackageName");
		
		
			// Create broadcast receiver
			testBroadcastReceiver = new BroadcastReceiver();
			testBroadcastReceiver.OnReceive += OnReceive;
			testBroadcastReceiver.Register(incomingAction);
		}

#endif

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public static void BroadcastHashtable(Hashtable hashtable) {
		foreach (DictionaryEntry entry in hashtable)
		{
	    Console.WriteLine("{0}, {1}", entry.Key, entry.Value);
		}
	}
	public static void BroadcastData(string key, string value) {
	#if UNITY_ANDROID
		Hashtable extras = new Hashtable();
		extras.Add(key, value);
		AndroidSystem.SendBroadcast(AndroidBroadcastIntentHandler.testBroadcastAction, extras);
		AndroidBroadcastIntentHandler.sendLog("org.curiouslearning.AssessmentApp", value);
	#endif	
	}
	public static void BroadcastJSONData(string key, string value) {
		string msg = "{\"key\":\""+ key + "\"";
		msg = msg + ", \"value\":\"" + value +"\"";
		msg = msg + "}";
		AndroidBroadcastIntentHandler.BroadcastData("data", msg);
		Debug.Log ("Broadcasting: " + msg); //debugger
	}
	

	private static void OnReceive(AndroidJavaObject contextPtr, AndroidJavaObject intentPtr) {
		AndroidJavaObject intent = intentPtr;
		string action = intent.Call<string>("getAction");
		broadcastMessage = action;
		if (action == incomingAction) {
			string message = intent.Call<string>("getStringExtra", "data");
			Debug.Log(broadcastMessage);
			
			foreach (GameObject go in AndroidBroadcastIntentHandler.listeners) {
				go.BroadcastMessage("OnReceive", message);
				Debug.Log(go.name);
			}
		} 
		
		
		

	}
	
	private static void  sendLog(string _name, string _value) {
		var unixTime = System.DateTime.Now.ToUniversalTime() - new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		
		Hashtable extras = new Hashtable();
		extras.Add("DATABASE_NAME", "mainPipeline");
		extras.Add("TIMESTAMP", (long)unixTime.TotalMilliseconds);
		extras.Add("NAME", _name);
		extras.Add("VALUE", _value);
			
		AndroidSystem.SendBroadcast("edu.mit.media.funf.RECORD", extras);
	}
}
