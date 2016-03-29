package com.fourthsky.unity.androidtools;

import java.net.URISyntaxException;
import java.util.Set;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.ComponentName;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.IntentFilter.MalformedMimeTypeException;
import android.graphics.Rect;
import android.net.Uri;
import android.nfc.NdefMessage;
import android.nfc.NfcAdapter;
import android.os.Bundle;
import android.os.Parcelable;
import android.telephony.SmsMessage;
import android.util.Base64;
import android.util.Log;

public class UnityAndroidSystemUtils {
	
	private static final String TAG = UnityAndroidSystemUtils.class.getName();
	
	/**
	 * Initialize NFC with default parameters
	 * @param activity activity to listen
	 * @return true if OK
	 */
	public static boolean enableSimpleNFCDispatch(Activity activity) {
		return enableNFCDispatch(activity, "*/*", new String[] { "NfcF", "NfcA", "NfcB", "NfcV", "Ndef"});
	}
	
	
	/**
	 * Initialize NFC to listen for tags
	 * @param activity activity to listen
	 * @param dataType data type to read from tag
	 * @param techs techs to receive from tags
	 * @return
	 */
	public static boolean enableNFCDispatch(Activity activity, String dataType, String[] techs) {
		NfcAdapter adapter = NfcAdapter.getDefaultAdapter(activity.getApplicationContext());
		
		Intent intent = new Intent(activity, activity.getClass()).addFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP); 
		PendingIntent pendingIntent = PendingIntent.getActivity(activity, 0, intent, 0);
		
		IntentFilter filter = new IntentFilter(NfcAdapter.ACTION_NDEF_DISCOVERED);
		try {
			filter.addDataType(dataType);
		} catch (MalformedMimeTypeException e) {
			// TODO Auto-generated catch block
			throw new RuntimeException("Data type invalid", e);
		}
		
		IntentFilter[] filters = new IntentFilter[] { filter };
		
		String[][] techLists = new String[1][techs.length];
		for (int i = 0; i < techs.length; i++) {
			try {
				// Try finding the class
				Class<?> cls = Class.forName("android.nfc.tech." + techs[i]);
				
				// If class exists, add to array
				techLists[0][i] = cls.getName();
				
			} catch (ClassNotFoundException e) {
				// If tag class doesn't exists, throw exception
				throw new RuntimeException("Tech " + techs[i] + " is invalid", e);
			}
			
		}
		
		// Finally, enable
		adapter.enableForegroundDispatch(activity, pendingIntent, filters, techLists);
		
		return true;
	}
	
	public static void disableNFCDispatch(Activity activity) {
		NfcAdapter adapter = NfcAdapter.getDefaultAdapter(activity.getApplicationContext());
		if (adapter != null) 
			adapter.disableForegroundDispatch(activity);
	}
	
	public static String toUri(Intent intent) {
		StringBuilder uri = new StringBuilder(1024);
		uri.append("#Intent;");
		
		if (intent.getScheme() != null) {
            uri.append("scheme=").append(intent.getScheme()).append(';');
        }
        if (intent.getAction() != null) {
            uri.append("action=").append(Uri.encode(intent.getAction())).append(';');
        }
        if (intent.getCategories() != null) {
        	Set<String> categories = intent.getCategories();
        	for (String category : categories) {
        		uri.append("category=").append(Uri.encode(category)).append(';');
        	}
        }
        if (intent.getType() != null) {
            uri.append("type=").append(Uri.encode(intent.getType(), "/")).append(';');
        }
        if (intent.getFlags() != 0) {
            uri.append("launchFlags=0x").append(Integer.toHexString(intent.getFlags())).append(';');
        }
        if (intent.getPackage() != null) {
            uri.append("package=").append(Uri.encode(intent.getPackage())).append(';');
        }
        if (intent.getComponent() != null) {
            uri.append("component=").append(Uri.encode(
            		intent.getComponent().flattenToShortString(), "/")).append(';');
        }
        if (intent.getSourceBounds() != null) {
            uri.append("sourceBounds=")
                    .append(Uri.encode(intent.getSourceBounds().flattenToString()))
                    .append(';');
        }
        if (intent.getExtras() != null) {
            for (String key : intent.getExtras().keySet()) {
                final Object value = intent.getExtras().get(key);
                char entryType =
                        value instanceof String    ? 'S' :
                        value instanceof Boolean   ? 'B' :
                        value instanceof Byte      ? 'b' :
                        value instanceof Character ? 'c' :
                        value instanceof Double    ? 'd' :
                        value instanceof Float     ? 'f' :
                        value instanceof Integer   ? 'i' :
                        value instanceof Long      ? 'l' :
                        value instanceof Short     ? 's' :
                        '\0';

                if (entryType != '\0') {
                    uri.append(entryType);
                    uri.append('.');
                    uri.append(Uri.encode(key));
                    uri.append('=');
                    uri.append(Uri.encode(value.toString()));
                    uri.append(';');
                
                } else {
                	if (value instanceof byte[]) {
                		uri.append("[b.");
                		uri.append(Uri.encode(key));
                        uri.append('=');
                        uri.append(Uri.encode(Base64.encodeToString((byte[])value, Base64.DEFAULT)));
                        uri.append(';');
                	}
                	
                	if (value instanceof NdefMessage) {
                		uri.append("L." + NdefMessage.class.getName());
                        uri.append('.');
                        uri.append(Uri.encode(key));
                        uri.append('=');
                        uri.append(Uri.encode(Base64.encodeToString(((NdefMessage) value).toByteArray(), Base64.DEFAULT)));
                        uri.append(';');
                	}
                }
            }
        }
        
        uri.append("end");
		
		return uri.toString();
	}
	
	public static Intent parseUri(String uri) throws URISyntaxException {
		Intent intent = new Intent(Intent.ACTION_VIEW);
		int i = 0;
		
		// simple case
        i = uri.lastIndexOf("#");
        if (i == -1) return new Intent(Intent.ACTION_VIEW, Uri.parse(uri));

        // old format Intent URI
        //if (!uri.startsWith("#Intent;", i)) return getIntentOld(uri);

        // new format
        //Intent intent = new Intent(ACTION_VIEW);
        //Intent baseIntent = intent;

        // fetch data part, if present
        String data = i >= 0 ? uri.substring(0, i) : null;
        String scheme = null;
        i += "#Intent;".length();

        // loop over contents of Intent, all name=value;
        while (!uri.startsWith("end", i)) {
            int eq = uri.indexOf('=', i);
            if (eq < 0) eq = i-1;
            int semi = uri.indexOf(';', i);
            String value = eq < semi ? Uri.decode(uri.substring(eq + 1, semi)) : "";

            // action
            if (uri.startsWith("action=", i)) {
                intent.setAction(value);
            }

            // categories
            else if (uri.startsWith("category=", i)) {
                intent.addCategory(value);
            }

            // type
            else if (uri.startsWith("type=", i)) {
                intent.setType(value);
            }

            // launch flags
            else if (uri.startsWith("launchFlags=", i)) {
                intent.setFlags(Integer.decode(value).intValue());
            }

            // package
            else if (uri.startsWith("package=", i)) {
                intent.setPackage(value);
            }

            // component
            else if (uri.startsWith("component=", i)) {
                intent.setComponent(ComponentName.unflattenFromString(value));
            }

            // scheme
            else if (uri.startsWith("scheme=", i)) {
                scheme = value;
            }

            // source bounds
            else if (uri.startsWith("sourceBounds=", i)) {
                intent.setSourceBounds(Rect.unflattenFromString(value));
            }

            // selector
            else if (semi == (i+3) && uri.startsWith("SEL", i)) {
                intent = new Intent();
            }

            // extra
            else {
                String key = Uri.decode(uri.substring(i + 2, eq));
                // add EXTRA
                if      (uri.startsWith("S.", i)) intent.putExtra(key, value);
                else if (uri.startsWith("B.", i)) intent.putExtra(key, Boolean.parseBoolean(value));
                else if (uri.startsWith("b.", i)) intent.putExtra(key, Byte.parseByte(value));
                else if (uri.startsWith("c.", i)) intent.putExtra(key, value.charAt(0));
                else if (uri.startsWith("d.", i)) intent.putExtra(key, Double.parseDouble(value));
                else if (uri.startsWith("f.", i)) intent.putExtra(key, Float.parseFloat(value));
                else if (uri.startsWith("i.", i)) intent.putExtra(key, Integer.parseInt(value));
                else if (uri.startsWith("l.", i)) intent.putExtra(key, Long.parseLong(value));
                else if (uri.startsWith("s.", i)) intent.putExtra(key, Short.parseShort(value));
                else if (uri.startsWith("[b.", i)) {
                	int keyStart = key.lastIndexOf('.');
                	String type = key.substring(0, keyStart);
                }
                else if (uri.startsWith("L.", i)) {
                	// Extract type
                	int keyStart = key.lastIndexOf('.');
                	String type = key.substring(0, keyStart);
                	String parcelableKey = key.substring(keyStart + 1);
                	try {
						intent.putExtra(parcelableKey, parseObject(type, value));
					} catch (Exception e) {
						Log.e(TAG, "Error parsing object, continue", e);
					}
                }
                else throw new URISyntaxException(uri, "unknown EXTRA type", i);
            }

            // move to the next item
            i = semi + 1;
        }

        /*
        if (intent != baseIntent) {
            // The Intent had a selector; fix it up.
            baseIntent.setSelector(intent);
            intent = baseIntent;
        }
        */

        if (data != null) {
            if (data.startsWith("intent:")) {
                data = data.substring(7);
                if (scheme != null) {
                    data = scheme + ':' + data;
                }
            }

            if (data.length() > 0) {
                try {
                    intent.setData(Uri.parse(data));
                } catch (IllegalArgumentException e) {
                    throw new URISyntaxException(uri, e.getMessage());
                }
            }
        }

        return intent;
	}
	
	private static Parcelable parseObject(String type, String base64Data) throws Exception {
		if (NdefMessage.class.getName().equals(type)) {
			byte[] data = Base64.decode(base64Data, 0);
			NdefMessage msg = new NdefMessage(data);
			return msg;
		}
		
		return null;
	}
}
