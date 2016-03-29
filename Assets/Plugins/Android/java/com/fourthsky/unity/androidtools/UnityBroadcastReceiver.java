package com.fourthsky.unity.androidtools;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

/**
 * 
 * @author evandro.paulino
 *
 */
public class UnityBroadcastReceiver extends BroadcastReceiver {

	static {
		System.loadLibrary("unityandroidsystem");
	}
	
	private final int mCallbackPtr;
	
	public UnityBroadcastReceiver(int callbackPtr) {
		mCallbackPtr = callbackPtr;
	}
	
	@Override
	public void onReceive(Context context, Intent intent) {
		// Call native callback
		if (mCallbackPtr != 0) {
			this.nativeOnReceive(mCallbackPtr, context, intent);
		}
	}
	
	/**
	 * 
	 * @param callback
	 * @param context
	 * @param intent
	 */
	private native void nativeOnReceive(int callback, Context context, Intent intent);

}
