package com.fourthsky.unity.androidtools;

import android.content.ComponentName;
import android.content.ServiceConnection;
import android.os.IBinder;

public final class UnityServiceConnection implements ServiceConnection {

	static {
		System.loadLibrary("unityandroidsystem");
	}
	
	private final int mConnectedCallbackPtr;
	private final int mDisconnectedCallbackPtr;
	
	public UnityServiceConnection(int connectedCallbackPtr, int disconnectCallbackPtr) {
		this.mConnectedCallbackPtr = connectedCallbackPtr;
		this.mDisconnectedCallbackPtr = disconnectCallbackPtr;
	}
	
	@Override
	public void onServiceConnected(ComponentName name, IBinder binder) {
		// Call native callback
		if (mConnectedCallbackPtr != 0) {
			nativeOnServiceConnected(mConnectedCallbackPtr, name, binder);
		}
	}

	@Override
	public void onServiceDisconnected(ComponentName name) {
		// Call native callback
		if (mDisconnectedCallbackPtr != 0) {
			nativeOnServiceDisconnected(mDisconnectedCallbackPtr, name);
		}
	}
	
	/**
	 * 
	 * @param callback
	 * @param name
	 * @param binder
	 */
	private native void nativeOnServiceConnected(int callback, ComponentName name, IBinder binder);
	
	/**
	 * 
	 * @param callback
	 * @param name
	 */
	private native void nativeOnServiceDisconnected(int callback, ComponentName name);
}