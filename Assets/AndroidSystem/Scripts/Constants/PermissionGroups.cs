using System;
using System.Collections.Generic;

namespace FourthSky
{
	namespace Android
	{
		public static class PermissionGroup
		{
			public const String ACCESSIBILITY_FEATURES = "android.permission-group.ACCESSIBILITY_FEATURES";
			public const String ACCOUNTS = "android.permission-group.ACCOUNTS";
			public const String AFFECTS_BATTERY = "android.permission-group.AFFECTS_BATTERY";
			public const String APP_INFO = "android.permission-group.APP_INFO";
			public const String AUDIO_SETTINGS = "android.permission-group.AUDIO_SETTINGS";
			public const String BLUETOOTH_NETWORK = "android.permission-group.BLUETOOTH_NETWORK";
			public const String BOOKMARKS = "android.permission-group.BOOKMARKS";
			public const String CALENDAR = "android.permission-group.CALENDAR";
			public const String CAMERA = "android.permission-group.CAMERA";
			public const String COST_MONEY = "android.permission-group.COST_MONEY";
			public const String DEVELOPMENT_TOOLS = "android.permission-group.DEVELOPMENT_TOOLS";
			public const String DEVICE_ALARMS = "android.permission-group.DEVICE_ALARMS";
			public const String DISPLAY = "android.permission-group.DISPLAY";
			public const String HARDWARE_CONTROLS = "android.permission-group.HARDWARE_CONTROLS";
			public const String LOCATION = "android.permission-group.LOCATION";
			public const String MESSAGES = "android.permission-group.MESSAGES";
			public const String MICROPHONE = "android.permission-group.MICROPHONE";
			public const String NETWORK = "android.permission-group.NETWORK";
			public const String PERSONAL_INFO = "android.permission-group.PERSONAL_INFO";
			public const String PHONE_CALLS = "android.permission-group.PHONE_CALLS";
			public const String SCREENLOCK = "android.permission-group.SCREENLOCK";
			public const String SOCIAL_INFO = "android.permission-group.SOCIAL_INFO";
			public const String STATUS_BAR = "android.permission-group.STATUS_BAR";
			public const String STORAGE = "android.permission-group.STORAGE";
			public const String SYNC_SETTINGS = "android.permission-group.SYNC_SETTINGS";
			public const String SYSTEM_CLOCK = "android.permission-group.SYSTEM_CLOCK";
			public const String SYSTEM_TOOLS = "android.permission-group.SYSTEM_TOOLS";
			public const String USER_DICTIONARY = "android.permission-group.USER_DICTIONARY";
			public const String VOICEMAIL = "android.permission-group.VOICEMAIL";
			public const String WALLPAPER = "android.permission-group.WALLPAPER";
			public const String WRITE_USER_DICTIONARY = "android.permission-group.WRITE_USER_DICTIONARY";
			
			public static readonly Dictionary<string, string> permissionGroups = new Dictionary<string, string>()
			{
				{ "ACCESSIBILITY_FEATURES", ACCESSIBILITY_FEATURES },
				{ "ACCOUNTS", ACCOUNTS },
				{ "AFFECTS_BATTERY", AFFECTS_BATTERY },
				{ "APP_INFO", APP_INFO },
				{ "AUDIO_SETTINGS", AUDIO_SETTINGS },
				{ "BLUETOOTH_NETWORK", BLUETOOTH_NETWORK },
				{ "BOOKMARKS", BOOKMARKS },
				{ "CALENDAR", CALENDAR },
				{ "CAMERA", CAMERA },
				{ "COST_MONEY", COST_MONEY },
				{ "DEVELOPMENT_TOOLS", DEVELOPMENT_TOOLS },
				{ "DEVICE_ALARMS", DEVICE_ALARMS },
				{ "DISPLAY", DISPLAY },
				{ "HARDWARE_CONTROLS", HARDWARE_CONTROLS },
				{ "LOCATION", LOCATION },
				{ "MESSAGES", MESSAGES },
				{ "MICROPHONE", MICROPHONE },
				{ "NETWORK", NETWORK },
				{ "PERSONAL_INFO", PERSONAL_INFO },
				{ "PHONE_CALLS", PHONE_CALLS },
				{ "SCREENLOCK", SCREENLOCK },
				{ "SOCIAL_INFO", SOCIAL_INFO },
				{ "STATUS_BAR", STATUS_BAR },
				{ "STORAGE", STORAGE },
				{ "SYNC_SETTINGS", SYNC_SETTINGS },
				{ "SYSTEM_CLOCK", SYSTEM_CLOCK },
				{ "SYSTEM_TOOLS", SYSTEM_TOOLS },
				{ "USER_DICTIONARY", USER_DICTIONARY },
				{ "VOICEMAIL", VOICEMAIL },
				{ "WALLPAPER", WALLPAPER },
				{ "WRITE_USER_DICTIONARY", WRITE_USER_DICTIONARY }
			};
		}
	}
}
