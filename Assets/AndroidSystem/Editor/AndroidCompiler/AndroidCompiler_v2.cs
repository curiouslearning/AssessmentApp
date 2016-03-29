using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AndroidCompiler_v2 : EditorWindow
{
    // Created window
    private static AndroidCompiler_v2 _window;

    private AndroidJavaCompiler javaCompiler;
    private AndroidNativeCompiler nativeCompiler;

    private string[] platformList;
    private int platformIndex;

    private static AndroidCompilerConfig config;

    private Texture2D banner;

    [MenuItem("Window/Android Compiler v.2")]
    static void Init()
    {
        if (_window == null)
        {
            _window = EditorWindow.GetWindow<AndroidCompiler_v2>(true, "Android Compiler Utility");
            _window.Show();
        }
    }

    public AndroidCompiler_v2()
    {
        minSize = new Vector2(400, 50);

        config = AndroidCompilerConfig.LoadOrCreate();
        platformList = config.platformList.ToArray();
        platformIndex = config.platformList.FindIndex(delegate (string p)
        {
            return p == config.Platform;
        });

        javaCompiler = new AndroidJavaCompiler(config);
        nativeCompiler = new AndroidNativeCompiler(config);

        banner = new Texture2D(1, 1);
        byte[] imageBytes = File.ReadAllBytes(AndroidCompilerConstants.BANNER_PATH);
        banner.LoadImage(imageBytes);
        banner.Apply();
    }

    void OnDestroy()
    {
        _window = null;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            BannerOnGUI();
            PlatformsOnGUI();
            ButtonsOnGUI();
            JavaLibrariesOnGUI();
            NativeLibrariesOnGUI();
        }
        EditorGUILayout.EndVertical();
    }

    private void BannerOnGUI()
    {
        Rect bounds = this.position;
        float bannerWidth = bounds.width;
        float bannerHeight = bannerWidth * 0.125f;
        EditorGUI.DrawPreviewTexture(new Rect(0, 0, bannerWidth, bannerHeight), banner);
        GUILayout.Space(bannerHeight + 5);
    }

    private void PlatformsOnGUI()
    {
        int newIndex = 0;
        newIndex = EditorGUILayout.Popup("Android Platforms", platformIndex, platformList);
        if (newIndex != platformIndex)
        {
            config.Platform = config.platformList[newIndex];
            platformIndex = newIndex;
        }

        GUILayout.Space(5);
    }

    private void JavaLibrariesOnGUI()
    {
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Java Android Libraries", "");
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Add Classpath Entry", GUILayout.Width(170.0f), GUILayout.Height(25.0f)))
            {
                string path = AndroidJavaCompiler.AndroidSDKPath + AndroidCompilerConstants.PLUGINS_PLATFORM_PATH + config.Platform;
                string jarPath = EditorUtility.OpenFilePanel("Select JAR file", path, "jar");
                if (!String.IsNullOrEmpty(jarPath))
                {
                    config.AddClasspathEntry(jarPath);
                }
            }

            if (GUILayout.Button("Inspect Project Libraries", GUILayout.Width(170.0f), GUILayout.Height(25.0f)))
            {
                List<string> libraryFiles = SOHelper.SearchFiles(Application.dataPath, "*.jar");
                foreach (string jar in libraryFiles)
                {
                    // GUARD Don't add compiled library to classpath
                    if (!jar.Contains(AndroidCompilerConstants.UNITY_ANDROID_SYSTEM_JAR_NAME))
                    {
                        config.AddClasspathEntry(Application.dataPath + jar);
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();
		
		GUILayout.Space(5);
		
        int i = 0;
		string temp = "Android library ";

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField(temp + (i + 1), config.classpath[i++], GUILayout.ExpandWidth(true));
        EditorGUILayout.TextField(temp + (i + 1), config.classpath[i++], GUILayout.ExpandWidth(true));
        EditorGUI.EndDisabledGroup();

        for (; i < config.classpath.Count; i++)
        {
			EditorGUILayout.BeginHorizontal();
			{
                GUILayout.Space(5);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(temp + (i + 1), config.classpath[i], GUILayout.ExpandWidth(true));
                EditorGUI.EndDisabledGroup();

				if (GUILayout.Button("Remove", GUILayout.Width(60.0f), GUILayout.Height(15.0f)))
				{
					config.RemoveClasspathEntry(i);
				}

                GUILayout.Space(5);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void NativeLibrariesOnGUI()
    {
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Native Android Libraries", "");
        GUILayout.Space(5);

        Rect bounds = this.position;
        string text = "This plugin still don't expose configuration for native compiling.\n" +
                      "If you help to develop or integrate a new native library, please consult\n" + 
                      "Android NDK documentation in http://developer.android.com/ndk/index.html\n" +
                      "or send an email to fourthskyinteractive@gmail.com.";
        EditorGUILayout.LabelField(text, GUILayout.Width(bounds.width), GUILayout.Height(100));
    }

    private void ButtonsOnGUI()
    {
        // TODO check width to draw buttons in horizontal or vertical

        EditorGUILayout.BeginHorizontal(GUILayout.Height(60.0f));
        {
            OpenSDKManagerOnGUI();
            CompileOnGUI();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OpenSDKManagerOnGUI()
    {
        if (GUILayout.Button("Open SDK Manager", GUILayout.Height(60.0f)))
        {
            SOHelper.ExecuteProcessNoWait("\"" + AndroidJavaCompiler.AndroidSDKPath + AndroidCompilerConstants.SDK_MANAGER_EXE_PATH, "", "");
        }
    }

    private void CompileOnGUI()
    {
        if (GUILayout.Button("Compile Java Classes", GUILayout.Height(60.0f)))
        {
            javaCompiler.GeneratePackage(true,
                delegate (bool success) {
                    if (success)
                    {
                        UnityEngine.Debug.Log("Android Java classes compiled successfully");
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Problem compiling Android Java classes, see log for details");
                    }
                }
            );
        }

        if (GUILayout.Button("Compile Native Code", GUILayout.Height(60.0f)))
        {
            nativeCompiler.GeneratePackage(true,
                delegate (bool success) {
                    if (success)
                    {
                        UnityEngine.Debug.Log("Android native code compiled successfully");
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Problem compiling Android native code, see log for details");
                    }
                }
            );
        }
    }

    private static void OpenPreferencesWindow()
    {
        var asm = Assembly.GetAssembly(typeof(EditorWindow));
        var T = asm.GetType("UnityEditor.PreferencesWindow");
        var M = T.GetMethod("ShowPreferencesWindow", BindingFlags.NonPublic | BindingFlags.Static);
        M.Invoke(null, null);
    }
}
