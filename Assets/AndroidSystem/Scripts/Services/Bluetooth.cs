using System;
using System.Collections.Generic;
using UnityEngine;
using FourthSky.Android;

namespace FourthSky
{
	namespace Android
	{
		namespace Services
		{
			public class BluetoothDevice
			{
				public readonly AndroidJavaObject JavaObject;
				
				private string mAddress;
				private string mName;
				
				// List of supported bluetooth services
				private List<string> mServices;
				
				public BluetoothDevice(AndroidJavaObject jObj) {
					if (jObj == null || jObj.GetRawObject() == IntPtr.Zero) {
						throw new UnityException("AndroidJavaObject could not be null");
					}
					
					this.JavaObject = jObj;
				}
				
				public string Address {
					get {
						if (mAddress == null)
							mAddress = JavaObject.Call<string>("getAddress");
							
						return mAddress;
					}
				}
				
				public string Name {
					get {
						if (mName == null)
							mName = JavaObject.Call<string>("getName");
							
						return mName;
					}
				}
				
				public List<string> Services {
					get {
						return mServices;
					}
				}
			}

			public static class Bluetooth
			{
				
				public static bool Supported {
					get {
#if UNITY_ANDROID
						if (BluetoothAdapter != null && BluetoothAdapter.GetRawObject() != IntPtr.Zero) {
							return true;
						}
#endif		
						return false;
					}
				}
				
				public static bool LowEnergySupported {
					get {
#if UNITY_ANDROID
						if (Supported) {
							using (AndroidJavaObject pkgManager = AndroidSystem
																	.UnityContext
																	.Call<AndroidJavaObject>("getPackageManager")) {
								
								return pkgManager.Call<bool>("hasSystemFeature", 
															 "android.hardware.bluetooth_le");
							}
						}
#endif
						
						return false;
					}
				}

				public static bool Enabled {
					get {
						if (Supported) {
							return BluetoothAdapter.Call<bool>("isEnabled");
						}

						return false;
					}
				}
				
				public static void RequestEnable(Action<bool> callback) {
					if (Bluetooth.Enabled)
						callback.Invoke(true);

					AndroidSystem.StartActivityForResult(ActivityActions.Bluetooth.ACTION_REQUEST_ENABLE, BLUETOOTH_ENABLE, 
															(int requestCode, int resultCode, AndroidJavaObject intent) => {
																if (requestCode == BLUETOOTH_ENABLE) {
																	switch(resultCode) {
																	case AndroidSystem.RESULT_OK:
																		if (callback != null)
																			callback.Invoke(true);
																		break;
																	case AndroidSystem.RESULT_CANCELED:
																		if (callback != null)
																			callback.Invoke(false);
																		break;
																	default:
																		if (callback != null)
																			callback.Invoke(false);
																		break;
																	}
																}
															});
				}
				
				// This also enables bluetooth
				public static void RequestDiscoverable(Action<bool> callback) {
					
					AndroidSystem.StartActivityForResult(ActivityActions.Bluetooth.ACTION_REQUEST_DISCOVERABLE, BLUETOOTH_DISCOVERABILITY, 
															(int requestCode, int resultCode, AndroidJavaObject intent) => {
																if (requestCode == BLUETOOTH_DISCOVERABILITY) {
																	switch(resultCode) {
																	case AndroidSystem.RESULT_OK:
																		if (callback != null)
																			callback.Invoke(true);
																		break;
																	case AndroidSystem.RESULT_CANCELED:
																		if (callback != null)
																			callback.Invoke(false);
																		break;
																	default:
																		if (callback != null)
																			callback.Invoke(false);
																		break;
																	}
																}
															});
				}

				public static void ScanClassicDevices (Action<BluetoothDevice> callback) {
					// Prepare broadcast receiver
					RegisterReceiver();
					
					// Start discovery
					if (BluetoothAdapter.Call<bool>("startDiscovery")) {
						mScanningClassic = true;
					}

					mScanningClassic = false;
				}

				public static void ScanLowEnergyDevices (Action<BluetoothDevice> callback) {
					// Prepare broadcast receiver
					RegisterReceiver();
					
					// Start discovery
					if (BluetoothAdapter.Call<bool> ("startLeScan", mLowEnergyListener)) {
						mScanningLE = true;
					}
					
					mScanningLE = false;
				}
				
				public static void StopScan () {
					if (mScanningClassic) {
						BluetoothAdapter.Call<bool> ("stopDiscovery");
						mScanningClassic = false;
						
					} else if (mScanningLE) {
						BluetoothAdapter.Call("stopLeScan", mLowEnergyListener);						
						mScanningLE = false;
					
					}
				}
				
				public static void ForceEnable () {
					if (!BluetoothAdapter.Call<bool>("isEnabled")) {
						BluetoothAdapter.Call<bool>("enable");
					}
				}
				
				public static void ForceDisable () {
					if (BluetoothAdapter.Call<bool>("isEnabled")) {
						BluetoothAdapter.Call<bool>("disable");
					}
				}
				
				
				#region private methods
				
				// Constants (this could be any numbers)
				private const int BLUETOOTH_ENABLE = 912396;
				private const int BLUETOOTH_DISCOVERABILITY = 912397;
				
				// Broadcast actions for Bluetooth Low Energy 
				private static string[] BROADCAST_ACTIONS = {
					BroadcastActions.Bluetooth.ACTION_DISCOVERY_STARTED,
					BroadcastActions.Bluetooth.ACTION_DISCOVERY_FINISHED,
					BroadcastActions.Bluetooth.ACTION_FOUND,
					BroadcastActions.Bluetooth.ACTION_UUID,
					BroadcastActions.Bluetooth.ACTION_ACL_DISCONNECTED,
					BroadcastActions.Bluetooth.ACTION_ACL_DISCONNECT_REQUESTED,
					BroadcastActions.Bluetooth.LE.ACTION_GATT_CONNECTED,
					BroadcastActions.Bluetooth.LE.ACTION_GATT_DISCONNECTED,
					BroadcastActions.Bluetooth.LE.ACTION_GATT_DEVICES_DISCOVERED,
					BroadcastActions.Bluetooth.LE.ACTION_GATT_SERVICES_DISCOVERED,
					BroadcastActions.Bluetooth.LE.ACTION_GATT_RSSI_READ,
					BroadcastActions.Bluetooth.LE.ACTION_GATT_DATA_AVAILABLE
				};
				
				private static AndroidJavaObject mAdapter;
				private static AndroidJavaObject mLowEnergyListener;
				private static BroadcastReceiver mReceiver;
				
				private static bool mScanningClassic = false;
				private static bool mScanningLE = false;
				private static Action<BluetoothDevice> deviceCallback;
				
				private static AndroidJavaObject BluetoothAdapter {
					get {
						if (mAdapter == null) {
							using (AndroidJavaClass BluetoothManager = new AndroidJavaClass("android.bluetooth.BluetoothAdapter")) {
								mAdapter = BluetoothManager.CallStatic<AndroidJavaObject>("getDefaultAdapter");
							}
						}
						
						return mAdapter;
					}
				}
				
				private static void RegisterReceiver() {
					if (mReceiver == null) {
						mReceiver = new BroadcastReceiver();
						mReceiver.OnReceive += (context, intent) => {
							string action = intent.Call<string>("getAction");
							
							if (BroadcastActions.Bluetooth.ACTION_DISCOVERY_STARTED == action) {
								
							}
							else if (BroadcastActions.Bluetooth.ACTION_DISCOVERY_FINISHED == action) {
								
							}
							else if (BroadcastActions.Bluetooth.ACTION_FOUND == action) {
								
							}
							else if (BroadcastActions.Bluetooth.ACTION_UUID == action) {
								
							}
							else if (BroadcastActions.Bluetooth.ACTION_ACL_DISCONNECTED == action) {
								
							}
							else if (BroadcastActions.Bluetooth.ACTION_ACL_DISCONNECT_REQUESTED == action) {
								
							}
							else if (BroadcastActions.Bluetooth.LE.ACTION_GATT_CONNECTED == action) {
								
							}
							else if (BroadcastActions.Bluetooth.LE.ACTION_GATT_DISCONNECTED == action) {
								
							}
							else if (BroadcastActions.Bluetooth.LE.ACTION_GATT_DEVICES_DISCOVERED == action) {
								
							}
							else if (BroadcastActions.Bluetooth.LE.ACTION_GATT_SERVICES_DISCOVERED == action) {
								
							}
							else if (BroadcastActions.Bluetooth.LE.ACTION_GATT_RSSI_READ == action) {
								
							}
							else if (BroadcastActions.Bluetooth.LE.ACTION_GATT_DATA_AVAILABLE == action) {
								
							}
						};
						
						mReceiver.Register (BROADCAST_ACTIONS);
					}
				}
				
				#endregion
				
			}
		}
	}
}