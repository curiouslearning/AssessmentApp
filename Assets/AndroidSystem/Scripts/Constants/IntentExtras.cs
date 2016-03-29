using System;

namespace FourthSky
{
	namespace Android
	{
		public static class IntentExtras
		{
			
			public static class Bluetooth
			{
				public const string EXTRA_DEVICE = "android.bluetooth.device.extra.DEVICE";
				public const string EXTRA_UUID = "android.bluetooth.device.extra.UUID";
				public const string EXTRA_BOND_STATE = "android.bluetooth.device.extra.BOND_STATE";

				public static class LE 
				{
					public const string EXTRA_STATUS                     = "android.bluetooth.le.EXTRA_STATUS";
					public const string EXTRA_ADDRESS                    = "android.bluetooth.le.EXTRA_ADDRESS";
					public const string EXTRA_DATA                       = "android.bluetooth.le.EXTRA_DATA";
					public const string EXTRA_RSSI                       = "android.bluetooth.le.EXTRA_RSSI";
				}
			}

			public static class NFC
			{
				public const string EXTRA_NDEF_MESSAGES = "android.nfc.extra.NDEF_MESSAGES";
			}

			public static class Media
			{
				public const string EXTRA_OUTPUT = "output";
			}

		}
	}
}