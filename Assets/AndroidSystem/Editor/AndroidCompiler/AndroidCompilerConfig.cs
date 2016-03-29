using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

public class AndroidCompilerConfig : ScriptableObject
{
    [SerializeField]
    private string platform;

    [SerializeField]
    public List<string> classpath;

    [SerializeField]
    public List<string> platformList;

    public string Platform
    {
        get
        {
            return platform;
        }
        set
        {
            // Set platform adjusts first classpath entry, that is the "android.jar" library
            platform = value;
            string path = AndroidJavaCompiler.AndroidSDKPath +
                          AndroidCompilerConstants.PLUGINS_PLATFORM_PATH +
                          platform +
                          "/" +
                          AndroidCompilerConstants.ANDROID_JAR_NAME;

            if (classpath.Count <= 0)
            {
                classpath.Add(path);
            }
            else
            {
                classpath[0] = path;
            }

            Save();
        }
    }

    public static AndroidCompilerConfig LoadOrCreate()
    {
        AndroidCompilerConfig configAsset = AssetDatabase.LoadAssetAtPath<AndroidCompilerConfig>(AndroidCompilerConstants.CONFIG_LOCATION_PATH);
        if (configAsset == null)
        {
            configAsset = AndroidCompilerConfig.Create();

            // Save and load from resources
            AssetDatabase.CreateAsset(configAsset, AndroidCompilerConstants.CONFIG_LOCATION_PATH);
            AssetDatabase.SaveAssets();

            configAsset = AssetDatabase.LoadAssetAtPath<AndroidCompilerConfig>(AndroidCompilerConstants.CONFIG_LOCATION_PATH);
        }

        return configAsset;
    }

    public static AndroidCompilerConfig Create()
    {
        // Create configuration
        AndroidCompilerConfig config = ScriptableObject.CreateInstance<AndroidCompilerConfig>();
        config.Init();

        return config;
    }

    public void AddClasspathEntry(string entry)
    {
        classpath.Add(entry);
        Save();
    }

    public void RemoveClasspathEntry(int index)
    {
        classpath.RemoveAt(index);
        Save();
    }

    public void Save()
    {
        EditorUtility.SetDirty((UnityEngine.Object)this);
    }

    private void Init()
    {
		{
			platformList = new List<string>();

			string sdkPath = AndroidJavaCompiler.AndroidSDKPath;
			if (!String.IsNullOrEmpty(sdkPath))
			{
				// Load platform list
				DirectoryInfo[] dirs = new DirectoryInfo(sdkPath + AndroidCompilerConstants.PLUGINS_PLATFORM_PATH).GetDirectories();
				for (int i = 0; i < dirs.Length; i++)
				{
					platformList.Add(dirs[i].Name);
				}

				// Reverse list, the newer platforms are the first
				platformList.Reverse();
			}
		}
		
        // Set platform (do not choose test platforms, the ones finished with letters)
		foreach (string plat in platformList)
		{
			int ret = 0;
			if (int.TryParse(plat.Substring(plat.Length - 1, 1), out ret))
            {
				this.Platform = plat;
				break;
			}
		}

        // Now, second classpath entry is Unity Android library, classes.jar
        // This library is dependent of Unity version
        string unityClasspath = EditorApplication.applicationContentsPath + AndroidCompilerConstants.CLASSES_JAR;
        this.classpath.Add(unityClasspath);
    }
}
