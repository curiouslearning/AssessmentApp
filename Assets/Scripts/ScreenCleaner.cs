using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnscreenObjectList {
	public enum MyObjectTypes { Stimulus=1, Token};
	public static Dictionary<MyObjectTypes, List<GameObject>> AllTypesOfObjects;
	public void registerObject (GameObject newObj, MyObjectTypes objType)
	{
		if(AllTypesOfObjects == null)
		{
			AllTypesOfObjects = new Dictionary<MyObjectTypes, List<GameObject>>();
		}
		if (!AllTypesOfObjects.ContainsKey(objType))
		{
			AllTypesOfObjects.Add(objType, new List<GameObject>());
		}
		AllTypesOfObjects[objType].Add(newObj);
	}

	
	public  void deRegisterObject (GameObject obj, MyObjectTypes objType)
	{
		if(AllTypesOfObjects == null || !AllTypesOfObjects.ContainsKey(objType))
		{
			Debug.Log("returning");
			return;
		}
		Debug.Log("removing " + obj.name);
		AllTypesOfObjects[objType].Remove(obj);
	}

	public void removeOrphans ()
	{
		if(AllTypesOfObjects == null)
			return;
		foreach(KeyValuePair<MyObjectTypes, List<GameObject>> s in AllTypesOfObjects)
		{
			if(!AllTypesOfObjects.ContainsKey(s.Key))
				continue;
			for (int i = 0; i < s.Value.Count; i++)
			{
				if (s.Value[i].transform.parent == null)
				{
					GameObject g = s.Value[i].gameObject;
					MonoBehaviour.Destroy(g);
				}
			}
		}
	}
	
}

public class ScreenCleaner : Observer {

	OnscreenObjectList objList;
	Subject signaler;
	// Use this for initialization
	void Start () {
		objList = new OnscreenObjectList();
		signaler = GetComponent<Subject>();
		signaler.addObserver(new Subject.GameObjectNotify(this.onNotify));
	}


	public void registerObject (GameObject g, OnscreenObjectList.MyObjectTypes t)
	{
		objList.registerObject(g, t);
	}	

	public void deRegisterObject (GameObject g, OnscreenObjectList.MyObjectTypes t)
	{
		objList.deRegisterObject (g, t);
	}
	public override void onNotify (EventInstance<GameObject> e)
	{
		if (e.type == eType.NewQuestion)
		{
			objList.removeOrphans();
		}
	}	
	
}
