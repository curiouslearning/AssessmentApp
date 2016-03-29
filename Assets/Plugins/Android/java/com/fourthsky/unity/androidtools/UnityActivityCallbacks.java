package com.fourthsky.unity.androidtools;

import android.app.Activity;
import android.content.Intent;
import android.nfc.NdefMessage;
import android.nfc.NfcAdapter;
import android.util.Log;

public class UnityActivityCallbacks {
	
	static {
		System.loadLibrary("unityandroidsystem");
	}

	private static int mActivityResultCallbackPtr;
	private static int mNewIntentCallbackPtr;
	
	public static void setActivityResultCallback(int ptr) {
		mActivityResultCallbackPtr = ptr;
	}
	
	public static void setNewIntentCallback(int ptr) {
		mNewIntentCallbackPtr = ptr;
	}
	
	public static void onNewIntent(Intent data) {
		if (UnityActivityCallbacks.mNewIntentCallbackPtr != 0) {
			UnityActivityCallbacks.nativeOnNewIntent(UnityActivityCallbacks.mNewIntentCallbackPtr, data);
		}
	}
	
	/**
	 * Native method that invokes C# callback
	 * @param callback
	 * @param data
	 */
	private static native void nativeOnNewIntent(int callback, Intent data);

	public static void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (UnityActivityCallbacks.mActivityResultCallbackPtr != 0) {
			UnityActivityCallbacks.nativeOnActivityResult(UnityActivityCallbacks.mActivityResultCallbackPtr, requestCode, resultCode, data);
		}
	}

	/**
	 * Native method that invokes C# callback
	 * @param callback
	 * @param requestCode
	 * @param resultCode
	 * @param data
	 */
	private static native void nativeOnActivityResult(int callback, int requestCode, int resultCode, Intent data);
	
}
