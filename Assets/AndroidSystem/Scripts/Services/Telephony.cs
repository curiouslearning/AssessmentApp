using System;
using UnityEngine;

namespace FourthSky
{
	namespace Android
	{
		namespace Services
		{
			public class SmsMessage 
			{
				public string OriginAddress {
					get {
						return originAddress;
					}
				}

				public string MessageBody {
					get {
						return messageBody;
					}
				}

				public long Timestamp {
					get {
						return timestamp;
					}
				}

				private readonly string originAddress;
				private readonly string messageBody;
				private readonly long timestamp;

				public SmsMessage(string addr, string body, long stamp) {
					originAddress = addr;
					messageBody = body;
					timestamp = stamp;
				}
			}
			
			public static class Telephony
			{
				#region public methods

				public static bool SendSMS(string phoneNumber, string message, Action<bool> sentCallback, Action<bool> deliveredCallback)
				{
#if UNITY_ANDROID
					AndroidJavaObject sentPendingIntent = null;
					AndroidJavaObject deliveredPendingIntent = null;
					
					// Only register broadcast receiver if callbacks if given
					if (sentCallback != null || deliveredCallback != null)
					{
						// Store callbacks
						smsSentCallback = sentCallback;
						smsDeliveredCallback = deliveredCallback;
						
						// Prepare broadcast receiver
						if (smsReceiver == null)
						{
							smsReceiver = new BroadcastReceiver();
							smsReceiver.OnReceive += OnReceive;
							
							// Register to receive events
							smsReceiver.Register(BroadcastActions.Telephony.ACTION_SMS_SENT,
                                                 BroadcastActions.Telephony.ACTION_SMS_DELIVER,
                                                 BroadcastActions.Telephony.ACTION_SMS_RECEIVED);
						}
						
						// Create pending intent to trigger broadcast
						using (AndroidJavaClass PendingIntent = new AndroidJavaClass("android.app.PendingIntent"))
						{
							if (smsSentCallback != null)
							{
								sentPendingIntent = PendingIntent.CallStatic<AndroidJavaObject>("getBroadcast", 
								                                                                AndroidSystem.UnityContext,
								                                                                0,
								                                                                new AndroidJavaObject(AndroidSystem.INTENT, BroadcastActions.Telephony.ACTION_SMS_SENT),
								                                                                0);
							}
							
							if (smsDeliveredCallback != null)
							{
								deliveredPendingIntent = PendingIntent.CallStatic<AndroidJavaObject>("getBroadcast", 
								                                                                     AndroidSystem.UnityContext,
								                                                                     0,
								                                                                     new AndroidJavaObject(AndroidSystem.INTENT, BroadcastActions.Telephony.ACTION_SMS_DELIVER),
								                                                                     0);
							}
						}
					}
					
					// Send the message
					if (message.Length <= 160)
					{
						SmsManager.Call("sendTextMessage",	
						                phoneNumber, 
						                null, 
						                message, 
						                sentPendingIntent,
						                deliveredPendingIntent);
						
						// Dispose the C# peers, it's already referenced by Android
						sentPendingIntent.Dispose();
						deliveredPendingIntent.Dispose();
					}
					else
					{
						// If message is greater than 160, divide the message in multiple parts
						Debug.Log("Message has more than 160 characters, don't implemented yet");
						return false;
					}
					
					return true;
#else
					return false;
#endif
					
				}

				public static bool ListenForSms(Action<SmsMessage[]> callback) 
				{
#if UNITY_ANDROID
					if (callback != null)
					{
						// Prepare broadcast receiver
						if (smsReceiver == null)
						{
							smsReceiver = new BroadcastReceiver();
							smsReceiver.OnReceive += OnReceive;
							
							// Register to receive events
							smsReceiver.Register(BroadcastActions.Telephony.ACTION_SMS_SENT,
                                                 BroadcastActions.Telephony.ACTION_SMS_DELIVER,
                                                 BroadcastActions.Telephony.ACTION_SMS_RECEIVED);
						}

						smsReceivedCallback = callback;
					}

					return true;
#else
					return false;
#endif
				}

				public static void StopListenForSms() {
					smsReceivedCallback = null;
				}
				
				/// <summary>
				/// Makes the a phone call.
				/// </summary>
				/// <returns><c>true</c>, if call was made, <c>false</c> otherwise.</returns>
				/// <param name="phoneNumber">Phone number.</param>
				/// <param name="callImmediately">If set to <c>true</c> call immediately.</param>
				public static bool MakeCall(string phoneNumber, bool callImmediately = true)
				{
#if UNITY_ANDROID
					// Fill intent
					AndroidJavaObject intent = new AndroidJavaObject (AndroidSystem.INTENT, 
					                                                  callImmediately ? ActivityActions.ACTION_CALL
					                                                  				  : ActivityActions.ACTION_DIAL);
					using (AndroidJavaClass Uri = new AndroidJavaClass("android.net.Uri"))
					{
						intent.Call<AndroidJavaObject>("setData", Uri.CallStatic<AndroidJavaObject>("parse", "tel:" + phoneNumber));
					}
					
					// Call dialer
					AndroidSystem.UnityActivity.Call ("startActivity", intent);
					
					return true;
#else
					return false;
#endif
				}
				
				#endregion
				
				#region private variables
				private const int MAKE_CALL_ID = 999813;
				
				public static readonly string SMS_EXTRA_NAME = "pdus";
				
				// Broadcast receiver to handle sms events
				private static BroadcastReceiver smsReceiver;

				private static Action<bool> smsSentCallback = null;
				private static Action<bool> smsDeliveredCallback = null;
				private static Action<SmsMessage[]> smsReceivedCallback = null;

#if UNITY_ANDROID
				// android.telephony.SmsManager object
				private static AndroidJavaObject mSmsManager;
#endif
				
				#endregion
				
				
				#region private methods
	
#if UNITY_ANDROID
				private static AndroidJavaObject SmsManager {
					get {
						if (mSmsManager == null)
						{
							using(AndroidJavaClass SmsManager = new AndroidJavaClass("android.telephony.SmsManager"))
							{
								mSmsManager = SmsManager.CallStatic<AndroidJavaObject>("getDefault");
								if (mSmsManager == null || mSmsManager.GetRawObject().ToInt32() == 0) {
									return null;
								}
							}
						}
						
						return mSmsManager;
					}
				}

				private static SmsMessage[] ExtractSmsMessages(AndroidJavaObject intent) {
					SmsMessage[] messages = null;

					AndroidJavaObject extras = intent.Call<AndroidJavaObject>("getExtras");
					if (extras != null && extras.GetRawObject().ToInt32() != 0) 
					{
						AndroidJavaObject pdus = extras.Call<AndroidJavaObject>("get", SMS_EXTRA_NAME);
						if (pdus != null &&  pdus.GetRawObject().ToInt32() != 0)
						{
							AndroidJavaObject[] smss = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(pdus.GetRawObject());
							if (smss != null && smss.Length > 0) 
							{
								using (AndroidJavaClass SmsMessage = new AndroidJavaClass("android.telephony.SmsMessage")) {
									messages = new SmsMessage[smss.Length];
									for (int i = 0; i < smss.Length; i++) 
									{
										AndroidJavaObject smsMsg = SmsMessage.CallStatic<AndroidJavaObject>("createFromPdu", smss[i]);
											
										string addr = smsMsg.Call<string>("getOriginatingAddress");
										string body = smsMsg.Call<string>("getMessageBody");
										long timestamp = smsMsg.Call<long>("getTimestampMillis");
										messages[i] = new SmsMessage(addr, body, timestamp);
									}
								}
							}
						}
					}

					return messages;
				}
				
				/// <summary>
				/// Handles SMS sent and delivered broadcasts
				/// </summary>
				/// <param name="context">activity where broadcast receiver was registered</param>
				/// <param name="intent">Intent containing info about broadcast</param>
				private static void OnReceive(AndroidJavaObject context, AndroidJavaObject intent)
				{
					string action = intent.Call<string> ("getAction");
					
					// Get result code 
					int resultCode = smsReceiver.GetResultCode ();
					
					if (BroadcastActions.Telephony.ACTION_SMS_SENT == action )
					{
						if (smsSentCallback != null)
							smsSentCallback.Invoke((resultCode == AndroidSystem.RESULT_OK));
					}
					else if (BroadcastActions.Telephony.ACTION_SMS_DELIVER == action )
					{
						if (smsDeliveredCallback != null)
							smsDeliveredCallback.Invoke((resultCode == AndroidSystem.RESULT_OK));
					}
					else if (BroadcastActions.Telephony.ACTION_SMS_RECEIVED == action ) 
					{
						if (smsReceivedCallback != null)  
						{
							SmsMessage[] messages = ExtractSmsMessages(intent);
							smsReceivedCallback.Invoke(messages);
						}
					}
				}

#endif
				
				#endregion
			}
		}
	}
}	