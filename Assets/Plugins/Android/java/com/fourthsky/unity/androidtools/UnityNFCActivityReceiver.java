package com.fourthsky.unity.androidtools;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.IntentFilter.MalformedMimeTypeException;
import android.nfc.NfcAdapter;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.view.ViewGroup.MarginLayoutParams;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;

/**
 * Class responsible to handle Android Beam flow for Unity applications
 *
 */
public class UnityNFCActivityReceiver extends Activity {

	public static final String DATA_TYPE = "data_type";
	public static final String TECH_LIST = "tech_list";
	public static final String TAG_RECEIVING = "tag_receiving";
	
	
	private boolean isTagReceiving = false;
	private NfcAdapter mAdapter;
	private String dataType;
	private PendingIntent mPendingIntent;
	private IntentFilter[] mFilters;
	private String[][] mTechList;
	
	
	@Override
	protected void onCreate(Bundle bundle) {
		super.onCreate(bundle);

		mAdapter = NfcAdapter.getDefaultAdapter(getApplicationContext());
		
		// Configure
		configureNFC(getIntent());
		
	}

	protected void onNewIntent(Intent data) {
		String action = data.getAction();
		try {
			// Call onNewIntent flow
			if (NfcAdapter.ACTION_NDEF_DISCOVERED.equals(action) || 
				NfcAdapter.ACTION_TECH_DISCOVERED.equals(action) ||
				NfcAdapter.ACTION_TAG_DISCOVERED.equals(action) ||
				Intent.ACTION_VIEW.equals(action))
			{
				UnityActivityCallbacks.onNewIntent(data);
				isTagReceiving = false;
			}
		} catch (Exception ex) {
			Log.e("UnityAndroidSystem", "Error calling onNewIntent callback", ex);
		}
		
		// 
		finish();
	}

	@Override
	protected void onResume() {
		super.onResume();

		if (isTagReceiving) {
			mAdapter.enableForegroundDispatch(this, mPendingIntent, mFilters, mTechList);
		}
	}

	protected void onPause() {
		super.onPause();
		
		if (isTagReceiving) {
			mAdapter.disableForegroundDispatch(this);
		}
	}
	
	/**
	 * 
	 * 
	 */
	private void configureNFC(Intent callingIntent) {
		isTagReceiving = callingIntent.getBooleanExtra(TAG_RECEIVING, false); 
		if (isTagReceiving) {
			
			// Show view waiting for NFC tag
			setContentView(getResources().getIdentifier("activity_nfc", "layout", getPackageName()));
			
			// Configure tech list
			String[] techs = callingIntent.getStringArrayExtra(TECH_LIST);
			if (techs == null) {
				techs = new String[] { "NfcA", "NfcF", "Ndef" };
			}
			mTechList = new String[1][techs.length];
			for (int i = 0; i < techs.length; i++) {
				try {
					// Try finding the class
					Class<?> cls = Class.forName("android.nfc.tech." + techs[i]);
					
					// If class exists, add to array
					mTechList[0][i] = cls.getName();
					
				} catch (ClassNotFoundException e) {
					// If tag class doesn't exists, throw exception
					throw new RuntimeException("Tech " + techs[i] + " is invalid", e);
				}
			}
			
			// Configure pending intent
			Intent intent = new Intent(this, getClass()).addFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP); 
			mPendingIntent = PendingIntent.getActivity(this, 0, intent, 0);
			
			// Configure intent filter
			dataType = callingIntent.getStringExtra(DATA_TYPE);
			if (dataType == null || dataType.isEmpty()) {
				dataType = "*/*";
			}
			IntentFilter filter = new IntentFilter(NfcAdapter.ACTION_NDEF_DISCOVERED);
			try {
				filter.addDataType(dataType);
			} catch (MalformedMimeTypeException e) {
				throw new RuntimeException("fail", e);
			}
			
			mFilters = new IntentFilter[] { filter };
		}
	}
	
	private View createContentView() {
		LinearLayout layout = new LinearLayout(getApplicationContext());
		MarginLayoutParams mParams = new MarginLayoutParams(LayoutParams.MATCH_PARENT,LayoutParams.MATCH_PARENT);
		layout.setLayoutParams(mParams);
		layout.setOrientation(LinearLayout.VERTICAL);
		layout.setPadding(20, 0, 0, 0);
		
		TextView text = new TextView(getApplicationContext());
		mParams = new MarginLayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
		mParams.leftMargin = 16;
		mParams.rightMargin = 16;
		text.setLayoutParams(mParams);
		text.setText("Waiting for NFC Tag...");
		layout.addView(text);
		
		Button button =  new Button(getApplicationContext());
		mParams = new MarginLayoutParams(LayoutParams.FILL_PARENT, LayoutParams.WRAP_CONTENT);
		mParams.topMargin = 20;
		button.setLayoutParams(mParams);
		button.setText("Cancel");
		button.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View v) {
				finish();
			}
		});
		layout.addView(button);
		
		return layout;
	}
}
