using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;

public class AndroidPostprocessor : AssetPostprocessor 
{
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
	{
		bool recompileAndroidJava = false;
        
		foreach (var str in importedAssets) {
			// Debug.Log("Reimported Asset: " + str);
			if (AndroidJavaCompiler.ShouldCompile(str)) {
				recompileAndroidJava = true;
				break;
			}
		}
		foreach (var str in deletedAssets) {
			// Debug.Log("Deleted Asset: " + str);
			if (AndroidJavaCompiler.ShouldCompile(str)) {
				recompileAndroidJava = true;
				break;
			}
		}
        
		foreach (var str in movedAssets) {
			// Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
			if (AndroidJavaCompiler.ShouldCompile(str)) {
				recompileAndroidJava = true;
				break;
			}
		}

        AndroidCompilerConfig config = AndroidCompilerConfig.LoadOrCreate();

        if (recompileAndroidJava) {
        	UnityEngine.Debug.Log ("Compiling Android Java classes");
        	new AndroidJavaCompiler(config).GeneratePackage(false,
        		delegate(bool success) {
        			if (success) {
        				UnityEngine.Debug.Log ("Android Java classes compiled successfully");
        			} else {
        				UnityEngine.Debug.Log ("Problem compiling Android Java classes, see log for details");
        			}
        		}
        	);
        }
        
        AssetDatabase.Refresh();

    }
}