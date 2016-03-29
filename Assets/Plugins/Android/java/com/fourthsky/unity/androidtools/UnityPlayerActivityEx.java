package com.fourthsky.unity.androidtools;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.unity3d.player.UnityPlayerActivity;

/**
 * 
 * @author Evandro
 *
 */
public class UnityPlayerActivityEx extends UnityPlayerActivity {
	
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
