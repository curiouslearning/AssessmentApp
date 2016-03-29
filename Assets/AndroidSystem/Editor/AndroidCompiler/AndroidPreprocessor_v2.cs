using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;

public class AndroidPreprocessor_v2 : AssetPostprocessor 
{
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
	{
        /*
		bool recompileAndroidJava = false;
		bool recompileAndroidNative = false;
        
		foreach (var str in importedAssets) {
			// Debug.Log("Reimported Asset: " + str);
			if (AndroidJavaCompiler.ShouldCompile(str)) {
				recompileAndroidJava = true;
				break;
			}
			if (AndroidNativeCompiler.ShouldCompile(str)) {
				recompileAndroidNative = true;
				break;
			}
		}
		foreach (var str in deletedAssets) {
			// Debug.Log("Deleted Asset: " + str);
			if (AndroidJavaCompiler.ShouldCompile(str)) {
				recompileAndroidJava = true;
				break;
			}
			if (AndroidNativeCompiler.ShouldCompile(str)) {
				recompileAndroidNative = true;
				break;
			}
		}
        
		foreach (var str in movedAssets) {
			// Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
			if (AndroidJavaCompiler.ShouldCompile(str)) {
				recompileAndroidJava = true;
				break;
			}
			if (AndroidNativeCompiler.ShouldCompile(str)) {
				recompileAndroidNative = true;
				break;
			}
		}

        AndroidCompilerConfig config = AndroidCompilerConfig.LoadOrCreate();

        if (recompileAndroidJava) {
        	UnityEngine.Debug.Log ("Compiling Android Java classes");
        	//Thread.Sleep(100);
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
        
        if (recompileAndroidNative) {
        	UnityEngine.Debug.Log ("Compiling Android native code");
        	//Thread.Sleep(100);
        	new AndroidNativeCompiler(config).GeneratePackage(false,
        		delegate(bool success) {
        			if (success) {
        				UnityEngine.Debug.Log ("Android native code compiled successfully");
        			} else {
        				UnityEngine.Debug.Log ("Problem compiling Android native code, see log for details");
        			}
        		}
        	);
        }

        AssetDatabase.Refresh();
        */
    }
}