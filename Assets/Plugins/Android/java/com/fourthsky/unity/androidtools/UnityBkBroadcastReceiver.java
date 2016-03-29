/* References used
 * http://forum.unity3d.com/threads/what-is-default-shared-preference-name.66579/
 * http://n3vrax.wordpress.com/2011/07/15/listen-for-install-referrer-broadcast-message-get-referrer-on-install/
 * http://n3vrax.wordpress.com/2011/07/15/multiple-broadcast-receivers-in-the-same-app-for-the-same-action/
 * http://docs.unity3d.com/ScriptReference/PlayerPrefs.html
 */

package com.fourthsky.unity.androidtools;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.StreamCorruptedException;
import java.net.URISyntaxException;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

public class UnityBkBroadcastReceiver extends BroadcastReceiver {

	static {
		System.loadLibrary("unityandroidsystem");
	}
	
	private static final String TAG = UnityBkBroadcastReceiver.class.getName();	
	
	private static int mCallbackPtr;
	
	@Override
	public void onReceive(Context context, Intent intent) {
		
		synchronized (UnityBkBroadcastReceiver.class) {
			// Call native callback
			if (mCallbackPtr != 0) {
				this.nativeOnReceive(mCallbackPtr, context, intent);
			}
		}
		
		boolean error = false;
		String fileName = "broadcast_" + System.currentTimeMillis() + ".bgd";
		FileOutputStream fos = null;
		try {
			String intentUri = UnityAndroidSystemUtils.toUri(intent);
			
			fos = context.openFileOutput(fileName, Context.MODE_PRIVATE);
			fos.write(intentUri.getBytes());
			fos.close();
			
		} catch (FileNotFoundException e) {
			Log.e(TAG, "Cannot create file to serialize", e);
			error = true;
			
		} catch (IOException e) {
			Log.e(TAG, "Error serializing intent", e);
			error = true;
			
		} finally {
			// In case of error, delete file
			if (error) {
				context.deleteFile(fileName);				
			}
		}
		
	}
	
	public static Intent parseIntentFromFile(Context context, String fileName) {
		Intent intent = null;
		FileInputStream is = null;
		
		try {
			is = context.openFileInput(fileName);
		} catch (FileNotFoundException e) {
			Log.e(TAG, "Cannot read serialized intent", e);
			return null;
		}
		
		try {
			StringBuilder sb = new StringBuilder(1024);
			BufferedReader br = new BufferedReader(new InputStreamReader(is));
			String line;
			while ((line = br.readLine()) != null) {
			    sb.append(line);
			}
			
			intent = UnityAndroidSystemUtils.parseUri(sb.toString());
			
		} catch (StreamCorruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (URISyntaxException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		return intent;
	}	
	
	/**
	 * 
	 * @param callback
	 * @param context
	 * @param intent
	 */
	private native void nativeOnReceive(int callback, Context context, Intent intent);
	
}
