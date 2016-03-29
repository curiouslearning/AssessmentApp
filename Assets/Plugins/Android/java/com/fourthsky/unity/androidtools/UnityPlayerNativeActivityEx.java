package com.fourthsky.unity.androidtools;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.unity3d.player.UnityPlayerNativeActivity;

/**
 * 
 * @author Evandro
 *
 */
public class UnityPlayerNativeActivityEx extends UnityPlayerNativeActivity {

	@Override
	protected void onCreate(Bundle bundle) {
		super.onCreate(bundle);

	}
	
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		UnityActivityCallbacks.onActivityResult(requestCode, resultCode, data);
	}

	@Override
	protected void onDestroy () {
		super.onDestroy();
	}
}
