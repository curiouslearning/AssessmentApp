using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FourthSky
{
    namespace Android
    {

        /// <summary>
        /// Broadcast receiver.
        /// </summary>
        public class BroadcastReceiver : AndroidWrapper
        {
            /// <summary>
            /// On receive delegate.
            /// </summary>
            public delegate void OnReceiveDelegate(AndroidJavaObject contextPtr, AndroidJavaObject intentPtr);

            public event OnReceiveDelegate OnReceive;

            private bool mRegistered;
            public bool Registered
            {
                get
                {
                    return mRegistered;
                }
            }

            private static List<string> mBroadcastFiles = new List<string>();

            #region internal delegate 

            private delegate void OnReceiveInternal(IntPtr contextPtr, IntPtr intentPtr);

            private OnReceiveInternal m_OnReceiveInternal;
            private GCHandle onReceiveInternalHandle;
            private IntPtr m_OnReceiveInternalPtr;

            void OnReceiveInternalImpl(IntPtr contextPtr, IntPtr intentPtr)
            {
                if (OnReceive != null)
                {
                    AndroidJavaObject context = AndroidSystem.ConstructJavaObjectFromPtr(contextPtr);
                    AndroidJavaObject intent = AndroidSystem.ConstructJavaObjectFromPtr(intentPtr);

                    OnReceive(context, intent);
                }
            }

            #endregion

            public BroadcastReceiver() : base()
            {

            }

            /// <summary>
            /// Gets the background broadcasts count.
            /// </summary>
            /// <returns>The background broadcasts count.</returns>
            public static int GetBackgroundBroadcastsCount()
            {
#if UNITY_ANDROID
                AndroidJavaObject context = AndroidSystem.UnityContext;
                AndroidJavaObject obj = context.Call<AndroidJavaObject>("fileList");
                if (obj.GetRawObject().ToInt32() != 0)
                {
                    int count = 0;
                    mBroadcastFiles = new List<string>();

                    string[] fileList = AndroidJNIHelper.ConvertFromJNIArray<string[]>(obj.GetRawObject());
                    foreach (string file in fileList)
                    {
                        if (file.StartsWith("broadcast_") && file.EndsWith(".bgd"))
                        {
                            mBroadcastFiles.Add(file);
                            count++;
                        }
                    }

                    return count;
                }
#endif
                return 0;
            }

            /// <summary>
            /// Gets the next background broadcast
            /// </summary>
            /// <returns>The next background broadcasts.</returns>
            public static AndroidJavaObject GetNextBackgroundBroadcasts()
            {
                AndroidJavaObject intent = null;
#if UNITY_ANDROID
                // If there's no pending broadcasts, return null
                if (mBroadcastFiles.Count <= 0)
                {
                    if (GetBackgroundBroadcastsCount() <= 0)
                    {
                        return null;
                    }
                }

                // Get first file
                string fileName = mBroadcastFiles[0];
                mBroadcastFiles.RemoveAt(0);

                // Parse Intent in Java
                IntPtr kls = AndroidJNI.FindClass("com/fourthsky/unity/androidtools/UnityBkBroadcastReceiver");
                IntPtr mthd = AndroidJNI.GetStaticMethodID(kls, "parseIntentFromFile", "(Landroid/content/Context;Ljava/lang/String;)Landroid/content/Intent;");

                IntPtr intentPtr = AndroidJNI.CallStaticObjectMethod(kls, mthd, AndroidJNIHelper.CreateJNIArgArray(new object[] { AndroidSystem.UnityContext, fileName }));
                if (intentPtr.ToInt32() != 0)
                {
                    intent = AndroidSystem.ConstructJavaObjectFromPtr(intentPtr);
                }


                /*
				// Parse file in C#
				AndroidJavaObject context = AndroidSystem.UnityContext;
				try {
					// Open file
					AndroidJavaObject fis = context.Call<AndroidJavaObject>("openFileInput", fileName);
					AndroidJavaObject ois = new AndroidJavaObject("java.io.ObjectInputStream", fis);

					// Read object
					AndroidJavaObject intentHelper = ois.Call<AndroidJavaObject>("readObject");
					intent = intentHelper.Get<AndroidJavaObject>("intent");

					// Close stream
					ois.Call("close");

				} catch (Exception ex) {
					Debug.Log("Error unmarshall serialized broadcast");
					Debug.LogException(ex);

				} finally {
					// Delete file (doesn't matter what...)
					context.Call<bool>("deleteFile", fileName);
				}
				*/
#endif
                return intent;
            }

            public void Register(params string[] actions)
            {
#if UNITY_ANDROID
                // Create java instance of broadcast receiver
                if (mJavaObject == null)
                {
                    // Prepare callback delegate
                    m_OnReceiveInternal = new OnReceiveInternal(OnReceiveInternalImpl);
                    onReceiveInternalHandle = GCHandle.Alloc(m_OnReceiveInternal);
                    m_OnReceiveInternalPtr = Marshal.GetFunctionPointerForDelegate(m_OnReceiveInternal);

                    IntPtr receiverPtr = CreateJavaBroadcastReceiver(m_OnReceiveInternalPtr);
                    if (IntPtr.Zero != receiverPtr)
                    {
                        mJavaObject = AndroidSystem.ConstructJavaObjectFromPtr(receiverPtr);

                    }
                    else
                    {
                        Debug.Log("Could not create broadcast receiver");
                    }
                }

                using (AndroidJavaObject activityInstance = AndroidSystem.UnityActivity)
                {
                    using (AndroidJavaObject intentFilter = new AndroidJavaObject(AndroidSystem.INTENT_FILTER))
                    {

                        // Configuring actions to receiver
                        foreach (string s in actions)
                        {
                            intentFilter.Call("addAction", s);
                        }

                        // Register broadcast receiver
                        try
                        {
                            activityInstance.Call<AndroidJavaObject>("registerReceiver", mJavaObject, intentFilter);

                        }
                        catch (Exception e)
                        {
                            string message = e.Message;

                            // I believe this exception occurs due to a bug in AndroidJavaObject class,
                            // because everything is OK after that, so I will ignore
                            if (message.Contains("Init'd AndroidJavaObject with null ptr"))
                            {
                                Debug.Log("Exception registering Broadcast Receiver, ignore: " + message);

                            }
                            else
                            {
                                throw e;
                            }
                        }
                    }
                }

                mRegistered = true;
#endif

            }

            public void Unregister()
            {
#if UNITY_ANDROID
                if (mRegistered && mJavaObject != null)
                {
                    using (AndroidJavaObject activityInstance = AndroidSystem.UnityActivity)
                    {

                        // Unregister broadcast receiver
                        activityInstance.Call("unregisterReceiver", mJavaObject);

                        // Release java broadcast receiver
                        mJavaObject.Dispose();
                        mJavaObject = null;

                        // Free internal delegate objects
                        onReceiveInternalHandle.Free();
                        m_OnReceiveInternal = null;

                    }

                    mRegistered = false;
                }
#endif
            }

            public int GetResultCode()
            {
#if UNITY_ANDROID
                if (mJavaObject != null)
                {
                    return mJavaObject.Call<int>("getResultCode");
                }
#endif

                return -1;
            }

            public void AbortBroadcast()
            {
#if UNITY_ANDROID
                if (mJavaObject != null)
                {
                    mJavaObject.Call("abortBroadcast");
                }
#endif
            }

            public void ClearAbortBroadcast()
            {
#if UNITY_ANDROID
                if (mJavaObject != null)
                {
                    mJavaObject.Call("clearAbortBroadcast");
                }
#endif
            }

            public bool GetAbortBroadcast()
            {
#if UNITY_ANDROID
                if (mJavaObject != null)
                {
                    return mJavaObject.Call<bool>("getAbortBroadcast");
                }
#endif
                return false;
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposed)
                {

                    if (disposing)
                    {
                        Unregister();
                    }
                }

                base.Dispose(disposing);
            }

#if UNITY_ANDROID
            [DllImport("unityandroidsystem")]
            private static extern IntPtr CreateJavaBroadcastReceiver(IntPtr callback);
#endif
        }

    }
}