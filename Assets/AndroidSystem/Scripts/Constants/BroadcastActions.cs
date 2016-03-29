using System;
namespace FourthSky
{
	namespace Android
	{
		public static class BroadcastActions
		{
			public const string ACTION_POWER_CONNECTED = "android.intent.action.ACTION_POWER_CONNECTED";
			public const string ACTION_POWER_DISCONNECTED = "android.intent.action.ACTION_POWER_DISCONNECTED";
			public const string ACTION_SHUTDOWN = "android.intent.action.ACTION_SHUTDOWN";
			public const string ACTION_AIRPLANE_MODE = "android.intent.action.AIRPLANE_MODE";
			public const string ACTION_BATTERY_CHANGED = "android.intent.action.BATTERY_CHANGED";
			public const string ACTION_BATTERY_LOW = "android.intent.action.BATTERY_LOW";
			public const string ACTION_BATTERY_OKAY = "android.intent.action.BATTERY_OKAY";
			public const string ACTION_BOOT_COMPLETED = "android.intent.action.BOOT_COMPLETED";
			public const string ACTION_CAMERA_BUTTON = "android.intent.action.CAMERA_BUTTON";
			public const string ACTION_CONFIGURATION_CHANGED = "android.intent.action.CONFIGURATION_CHANGED";
			public const string ACTION_CONTENT_CHANGED = "android.intent.action.CONTENT_CHANGED";
			public const string ACTION_DATA_SMS_RECEIVED = "android.intent.action.DATA_SMS_RECEIVED";
			public const string ACTION_DATE_CHANGED = "android.intent.action.DATE_CHANGED";
			public const string ACTION_DEVICE_STORAGE_LOW = "android.intent.action.DEVICE_STORAGE_LOW";
			public const string ACTION_DEVICE_STORAGE_OK = "android.intent.action.DEVICE_STORAGE_OK";
			public const string ACTION_DOCK_EVENT = "android.intent.action.DOCK_EVENT";
			public const string ACTION_DOWNLOAD_COMPLETE = "android.intent.action.DOWNLOAD_COMPLETE";
			public const string ACTION_DOWNLOAD_NOTIFICATION_CLICKED = "android.intent.action.DOWNLOAD_NOTIFICATION_CLICKED";
			public const string ACTION_DREAMING_STARTED = "android.intent.action.DREAMING_STARTED";
			public const string ACTION_DREAMING_STOPPED = "android.intent.action.DREAMING_STOPPED";
			public const string ACTION_EXTERNAL_APPLICATIONS_AVAILABLE = "android.intent.action.EXTERNAL_APPLICATIONS_AVAILABLE";
			public const string ACTION_EXTERNAL_APPLICATIONS_UNAVAILABLE = "android.intent.action.EXTERNAL_APPLICATIONS_UNAVAILABLE";
			public const string ACTION_FETCH_VOICEMAIL = "android.intent.action.FETCH_VOICEMAIL";
			public const string ACTION_GTALK_CONNECTED = "android.intent.action.GTALK_CONNECTED";
			public const string ACTION_GTALK_DISCONNECTED = "android.intent.action.GTALK_DISCONNECTED";
			public const string ACTION_HEADSET_PLUG = "android.intent.action.HEADSET_PLUG";
			public const string ACTION_INPUT_METHOD_CHANGED = "android.intent.action.INPUT_METHOD_CHANGED";
			public const string ACTION_LOCALE_CHANGED = "android.intent.action.LOCALE_CHANGED";
			public const string ACTION_MANAGE_PACKAGE_STORAGE = "android.intent.action.MANAGE_PACKAGE_STORAGE";
			public const string ACTION_MEDIA_BAD_REMOVAL = "android.intent.action.MEDIA_BAD_REMOVAL";
			public const string ACTION_MEDIA_BUTTON = "android.intent.action.MEDIA_BUTTON";
			public const string ACTION_MEDIA_CHECKING = "android.intent.action.MEDIA_CHECKING";
			public const string ACTION_MEDIA_EJECT = "android.intent.action.MEDIA_EJECT";
			public const string ACTION_MEDIA_MOUNTED = "android.intent.action.MEDIA_MOUNTED";
			public const string ACTION_MEDIA_NOFS = "android.intent.action.MEDIA_NOFS";
			public const string ACTION_MEDIA_REMOVED = "android.intent.action.MEDIA_REMOVED";
			public const string ACTION_MEDIA_SCANNER_FINISHED = "android.intent.action.MEDIA_SCANNER_FINISHED";
			public const string ACTION_MEDIA_SCANNER_SCAN_FILE = "android.intent.action.MEDIA_SCANNER_SCAN_FILE";
			public const string ACTION_MEDIA_SCANNER_STARTED = "android.intent.action.MEDIA_SCANNER_STARTED";
			public const string ACTION_MEDIA_SHARED = "android.intent.action.MEDIA_SHARED";
			public const string ACTION_MEDIA_UNMOUNTABLE = "android.intent.action.MEDIA_UNMOUNTABLE";
			public const string ACTION_MEDIA_UNMOUNTED = "android.intent.action.MEDIA_UNMOUNTED";
			public const string ACTION_MY_PACKAGE_REPLACED = "android.intent.action.MY_PACKAGE_REPLACED";
			public const string ACTION_NEW_OUTGOING_CALL = "android.intent.action.NEW_OUTGOING_CALL";
			public const string ACTION_NEW_VOICEMAIL = "android.intent.action.NEW_VOICEMAIL";
			public const string ACTION_PACKAGE_ADDED = "android.intent.action.PACKAGE_ADDED";
			public const string ACTION_PACKAGE_CHANGED = "android.intent.action.PACKAGE_CHANGED";
			public const string ACTION_PACKAGE_DATA_CLEARED = "android.intent.action.PACKAGE_DATA_CLEARED";
			public const string ACTION_PACKAGE_FIRST_LAUNCH = "android.intent.action.PACKAGE_FIRST_LAUNCH";
			public const string ACTION_PACKAGE_FULLY_REMOVED = "android.intent.action.PACKAGE_FULLY_REMOVED";
			public const string ACTION_PACKAGE_INSTALL = "android.intent.action.PACKAGE_INSTALL";
			public const string ACTION_PACKAGE_NEEDS_VERIFICATION = "android.intent.action.PACKAGE_NEEDS_VERIFICATION";
			public const string ACTION_PACKAGE_REMOVED = "android.intent.action.PACKAGE_REMOVED";
			public const string ACTION_PACKAGE_REPLACED = "android.intent.action.PACKAGE_REPLACED";
			public const string ACTION_PACKAGE_RESTARTED = "android.intent.action.PACKAGE_RESTARTED";
			public const string ACTION_PACKAGE_VERIFIED = "android.intent.action.PACKAGE_VERIFIED";
			public const string ACTION_PHONE_STATE = "android.intent.action.PHONE_STATE";
			public const string ACTION_PROVIDER_CHANGED = "android.intent.action.PROVIDER_CHANGED";
			public const string ACTION_PROXY_CHANGE = "android.intent.action.PROXY_CHANGE";
			public const string ACTION_REBOOT = "android.intent.action.REBOOT";
			public const string ACTION_SCREEN_OFF = "android.intent.action.SCREEN_OFF";
			public const string ACTION_SCREEN_ON = "android.intent.action.SCREEN_ON";
			public const string ACTION_TIMEZONE_CHANGED = "android.intent.action.TIMEZONE_CHANGED";
			public const string ACTION_TIME_SET = "android.intent.action.TIME_SET";
			public const string ACTION_TIME_TICK = "android.intent.action.TIME_TICK";
			public const string ACTION_UID_REMOVED = "android.intent.action.UID_REMOVED";
			public const string ACTION_USER_PRESENT = "android.intent.action.USER_PRESENT";
			public const string ACTION_WALLPAPER_CHANGED = "android.intent.action.WALLPAPER_CHANGED";
				
			public static class Bluetooth
			{
				public const string ACTION_PLAYING_STATE_CHANGED = "android.bluetooth.adapter.action.PLAYING_STATE_CHANGED";
				public const string ACTION_DISCOVERY_FINISHED = "android.bluetooth.adapter.action.DISCOVERY_FINISHED";
				public const string ACTION_DISCOVERY_STARTED = "android.bluetooth.adapter.action.DISCOVERY_STARTED";
				public const string ACTION_LOCAL_NAME_CHANGED = "android.bluetooth.adapter.action.LOCAL_NAME_CHANGED";
				public const string ACTION_SCAN_MODE_CHANGED = "android.bluetooth.adapter.action.SCAN_MODE_CHANGED";
				public const string ACTION_STATE_CHANGED = "android.bluetooth.adapter.action.STATE_CHANGED";
				public const string ACTION_ACL_CONNECTED = "android.bluetooth.device.action.ACL_CONNECTED";
				public const string ACTION_ACL_DISCONNECTED = "android.bluetooth.device.action.ACL_DISCONNECTED";
				public const string ACTION_ACL_DISCONNECT_REQUESTED = "android.bluetooth.device.action.ACL_DISCONNECT_REQUESTED";
				public const string ACTION_BOND_STATE_CHANGED = "android.bluetooth.device.action.BOND_STATE_CHANGED";
				public const string ACTION_CLASS_CHANGED = "android.bluetooth.device.action.CLASS_CHANGED";
				public const string ACTION_FOUND = "android.bluetooth.device.action.FOUND";
				public const string ACTION_NAME_CHANGED = "android.bluetooth.device.action.NAME_CHANGED";
				public const string ACTION_PAIRING_REQUEST = "android.bluetooth.device.action.PAIRING_REQUEST";
				public const string ACTION_UUID = "android.bluetooth.device.action.UUID";
				public const string ACTION_DEVICE_SELECTED = "android.bluetooth.devicepicker.action.DEVICE_SELECTED";
				public const string ACTION_LAUNCH = "android.bluetooth.devicepicker.action.LAUNCH";
				//public const string ACTION_CONNECTION_STATE_CHANGED = "android.bluetooth.input.profile.action.CONNECTION_STATE_CHANGED";
				//public const string ACTION_CONNECTION_STATE_CHANGED = "android.bluetooth.pan.profile.action.CONNECTION_STATE_CHANGED"

				public static class LE 
				{
					public const string ACTION_GATT_CONNECTED            = "android.bluetooth.le.ACTION_GATT_CONNECTED";
					public const string ACTION_GATT_DISCONNECTED         = "android.bluetooth.le.ACTION_GATT_DISCONNECTED";
					public const string ACTION_GATT_DEVICES_DISCOVERED   = "android.bluetooth.le.ACTION_GATT_DEVICES_DISCOVERED";
					public const string ACTION_GATT_SERVICES_DISCOVERED  = "android.bluetooth.le.ACTION_GATT_SERVICES_DISCOVERED";
					public const string ACTION_GATT_RSSI_READ            = "android.bluetooth.le.ACTION_GATT_RSSI_READ";
					public const string ACTION_GATT_DATA_AVAILABLE       = "android.bluetooth.le.ACTION_GATT_DATA_AVAILABLE";
				}

				public static class Headset
				{
					public const string ACTION_VENDOR_SPECIFIC_HEADSET_EVENT = "android.bluetooth.headset.action.VENDOR_SPECIFIC_HEADSET_EVENT";
					public const string ACTION_AUDIO_STATE_CHANGED = "android.bluetooth.headset.profile.action.AUDIO_STATE_CHANGED";
					public const string ACTION_CONNECTION_STATE_CHANGED_HEADSET = "android.bluetooth.headset.profile.action.CONNECTION_STATE_CHANGED";
				}
				
				public static class A2dp
				{
					public const string ACTION_CONNECTION_STATE_CHANGED = "android.bluetooth.a2dp.profile.action.CONNECTION_STATE_CHANGED";
					public const string ACTION_PLAYING_STATE_CHANGED = "android.bluetooth.a2dp.profile.action.PLAYING_STATE_CHANGED";
				}
			}
			
			public static class DeviceAdmin
			{
				public const string ACTION_PASSWORD_CHANGED = "android.app.action.ACTION_PASSWORD_CHANGED";
				public const string ACTION_PASSWORD_EXPIRING = "android.app.action.ACTION_PASSWORD_EXPIRING";				public const string ACTION_PASSWORD_FAILED = "android.app.action.ACTION_PASSWORD_FAILED";
				public const string ACTION_PASSWORD_SUCCEEDED = "android.app.action.ACTION_PASSWORD_SUCCEEDED";
				public const string ACTION_DEVICE_ADMIN_DISABLED = "android.app.action.DEVICE_ADMIN_DISABLED";
				public const string ACTION_DEVICE_ADMIN_DISABLE_REQUESTED = "android.app.action.DEVICE_ADMIN_DISABLE_REQUESTED";
				public const string ACTION_DEVICE_ADMIN_ENABLED = "android.app.action.DEVICE_ADMIN_ENABLED";
			}
			
			public static class Hardware
			{
				public const string ACTION_NEW_PICTURE = "android.hardware.action.NEW_PICTURE";
				public const string ACTION_NEW_VIDEO = "android.hardware.action.NEW_VIDEO";
				public const string ACTION_QUERY_KEYBOARD_LAYOUTS = "android.hardware.input.action.QUERY_KEYBOARD_LAYOUTS";
			}
			
			public static class Media
			{
				public const string ACTION_SCO_AUDIO_STATE_UPDATED = "android.media.ACTION_SCO_AUDIO_STATE_UPDATED";
				public const string ACTION_AUDIO_BECOMING_NOISY = "android.media.AUDIO_BECOMING_NOISY";
				public const string ACTION_RINGER_MODE_CHANGED = "android.media.RINGER_MODE_CHANGED";
				public const string ACTION_SCO_AUDIO_STATE_CHANGED = "android.media.SCO_AUDIO_STATE_CHANGED";
				public const string ACTION_VIBRATE_SETTING_CHANGED = "android.media.VIBRATE_SETTING_CHANGED";
				public const string ACTION_CLOSE_AUDIO_EFFECT_CONTROL_SESSION = "android.media.action.CLOSE_AUDIO_EFFECT_CONTROL_SESSION";
				public const string ACTION_OPEN_AUDIO_EFFECT_CONTROL_SESSION = "android.media.action.OPEN_AUDIO_EFFECT_CONTROL_SESSION";
			}

			public static class Telephony
			{
				public const string ACTION_SIM_FULL = "android.provider.Telephony.SIM_FULL";
				public const string ACTION_SMS_CB_RECEIVED = "android.provider.Telephony.SMS_CB_RECEIVED";
				public const string ACTION_SMS_EMERGENCY_CB_RECEIVED = "android.provider.Telephony.SMS_EMERGENCY_CB_RECEIVED";
                public const string ACTION_SMS_SENT = "android.provider.Telephony.SMS_SENT";
                public const string ACTION_SMS_RECEIVED = "android.provider.Telephony.SMS_RECEIVED";
                public const string ACTION_SMS_DELIVER = "android.provider.Telephony.SMS_DELIVER";
                public const string ACTION_SMS_REJECTED = "android.provider.Telephony.SMS_REJECTED";
				public const string ACTION_SMS_SERVICE_CATEGORY_PROGRAM_DATA_RECEIVED = "android.provider.Telephony.SMS_SERVICE_CATEGORY_PROGRAM_DATA_RECEIVED";
				public const string ACTION_WAP_PUSH_DELIVER = "android.provider.Telephony.WAP_PUSH_DELIVER";
				public const string ACTION_WAP_PUSH_RECEIVED = "android.provider.Telephony.WAP_PUSH_RECEIVED";
			}

			public static class Nfc
			{
				public const string ACTION_ADAPTER_STATE_CHANGED = "android.nfc.action.ADAPTER_STATE_CHANGED";
			}

			public static class Net
			{
				public const string ACTION_BACKGROUND_DATA_SETTING_CHANGED = "android.net.conn.BACKGROUND_DATA_SETTING_CHANGED";
				public const string ACTION_CONNECTIVITY_CHANGE = "android.net.conn.CONNECTIVITY_CHANGE";

				public static class Nsd
				{
					public const string ACTION_STATE_CHANGED = "android.net.nsd.STATE_CHANGED";
				}

				public static class Wifi
				{
					public const string ACTION_NETWORK_IDS_CHANGED = "android.net.wifi.NETWORK_IDS_CHANGED";
					public const string ACTION_RSSI_CHANGED = "android.net.wifi.RSSI_CHANGED";
					public const string ACTION_SCAN_RESULTS = "android.net.wifi.SCAN_RESULTS";
					public const string ACTION_STATE_CHANGE = "android.net.wifi.STATE_CHANGE";
					public const string ACTION_WIFI_STATE_CHANGED = "android.net.wifi.WIFI_STATE_CHANGED";

					public static class Supplicant
					{
						public const string ACTION_CONNECTION_CHANGE = "android.net.wifi.supplicant.CONNECTION_CHANGE";
						public const string ACTION_STATE_CHANGE = "android.net.wifi.supplicant.STATE_CHANGE";
					}

					public static class P2p
					{
						public const string ACTION_CONNECTION_STATE_CHANGE = "android.net.wifi.p2p.CONNECTION_STATE_CHANGE";
						public const string ACTION_DISCOVERY_STATE_CHANGE = "android.net.wifi.p2p.DISCOVERY_STATE_CHANGE";
						public const string ACTION_PEERS_CHANGED = "android.net.wifi.p2p.PEERS_CHANGED";
						public const string ACTION_STATE_CHANGED = "android.net.wifi.p2p.STATE_CHANGED";
						public const string ACTION_THIS_DEVICE_CHANGED = "android.net.wifi.p2p.THIS_DEVICE_CHANGED";
					}
				}

				public static class Speech
				{
					public static class Tts
					{
						public const string ACTION_TTS_QUEUE_PROCESSING_COMPLETED = "android.speech.tts.TTS_QUEUE_PROCESSING_COMPLETED";
						public const string ACTION_TTS_DATA_INSTALLED = "android.speech.tts.engine.TTS_DATA_INSTALLED";
					}
				}
			}
		}
	}
}
