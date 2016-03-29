using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FourthSky
{
    namespace Android
    {

        public static class AndroidVersions
        {
            public const int BASE = 1;
            public const int BASE_1_1 = 2;
            public const int CUPCAKE = 3;
            public const int DONUT = 4;
            public const int ECLAIR = 5;
            public const int ECLAIR_0_1 = 6;
            public const int ECLAIR_MR1 = 7;
            public const int FROYO = 8;
            public const int GINGERBREAD = 9;
            public const int GINGERBREAD_MR1 = 10;
            public const int HONEYCOMB = 11;
            public const int HONEYCOMB_MR1 = 12;
            public const int HONEYCOMB_MR2 = 13;
            public const int ICE_CREAM_SANDWICH = 14;
            public const int ICE_CREAM_SANDWICH_MR1 = 15;
            public const int JELLY_BEAN = 16;
            public const int JELLY_BEAN_MR1 = 17;
            public const int JELLY_BEAN_MR2 = 18;
            public const int KITKAT = 19;
            public const int KITKAT_WEAR = 20;
            public const int LOLLIPOP = 21;
            public const int LOLLIPOP_MR1 = 22;
            public const int MARSMALLOW = 23;
        }

        public static class AndroidSystem
        {

            // Delegate for activity result
            public delegate void OnActivityResultDelegate(int requestCode, int resultCode, AndroidJavaObject intent);
            public static event OnActivityResultDelegate OnActivityResult;

            // Delegate for on new intent
            public delegate void OnNewIntentDelegate(AndroidJavaObject intent);
            public static event OnNewIntentDelegate OnNewIntent;

            #region internal callback objects

            private delegate void OnActivityResultInternal(int requestCode, int resultCode, IntPtr intentPtr);
            private static OnActivityResultInternal onActivityResultInternal;
            private static GCHandle onActivityResultInternalHandle;
            private static IntPtr onActivityResultInternalPtr = IntPtr.Zero;

            private static void OnActivityResultInternalImpl(int requestCode, int resultCode, IntPtr intentPtr)
            {
#if UNITY_ANDROID
                if (OnActivityResult != null)
                {
                    AndroidJavaObject intent =
                            intentPtr.ToInt32() == 0 ? null
                                                     : AndroidSystem.ConstructJavaObjectFromPtr(intentPtr);

                    // Invoke callback
                    AndroidSystem.OnActivityResult(requestCode, resultCode, intent);

                    // Release callback objects, to prevent memory leak
                    onActivityResultInternalHandle.Free();
                    onActivityResultInternal = null;
                }
#endif
            }

            private delegate void OnNewIntentInternal(IntPtr intentPtr);
            private static OnNewIntentInternal onNewIntentInternal;
            private static GCHandle onNewIntentInternalHandle;
            private static IntPtr onNewIntentInternalPtr;

            private static void OnNewIntentInternalImpl(IntPtr intentPtr)
            {
#if UNITY_ANDROID
                if (OnNewIntent != null)
                {
                    AndroidJavaObject intent = AndroidSystem.ConstructJavaObjectFromPtr(intentPtr);

                    AndroidSystem.OnNewIntent(intent);

                    // Release callback objects, to prevent memory leak
                    onNewIntentInternalHandle.Free();
                    onNewIntentInternal = null;
                }
#endif
            }

            #endregion

            // Stores current Android version
            private static int SDK_VERSION = -1;

            // Name of activities used for Android System plugin
            internal static readonly string PLUGIN_PACKAGE = "com.fourthsky.unity.androidtools";
            internal static readonly string PLUGIN_ACTIVITY = PLUGIN_PACKAGE + ".UnityPlayerActivityEx";
            internal static readonly string PLUGIN_NATIVE_ACTIVITY = PLUGIN_PACKAGE + ".UnityPlayerNativeActivityEx";
            //private static readonly string ACTIVITY_CALLBACKS = PLUGIN_PACKAGE + ".UnityActivityCallbacks";

            // Android class names
            internal static readonly string UNITY_PLAYER = "com.unity3d.player.UnityPlayer";
            internal static readonly string INTENT = "android.content.Intent";
            internal static readonly string INTENT_FILTER = "android.content.IntentFilter";
            internal static readonly string PARCELABLE = "android.os.Parcelable";

            // Constants for return of StartActivityForResult
            public const int RESULT_OK = -1;
            public const int RESULT_CANCELED = 0x0;
            public const int RESULT_FIRST_USER = 0x1;

            /// <summary>
            /// Gets the Android version.
            /// </summary>
            /// <value>Android version in device.</value>
            public static int Version
            {
                get
                {
#if UNITY_ANDROID
                    if (SDK_VERSION == -1)
                    {
                        using (AndroidJavaClass Build_VERSION = new AndroidJavaClass("android.os.Build$VERSION"))
                        {
                            SDK_VERSION = Build_VERSION.GetStatic<int>("SDK_INT");
                        }
                    }
#endif
                    return SDK_VERSION;
                }
            }

            public static bool ActivityCallbacksSupported
            {
                get
                {
#if UNITY_ANDROID
                    string className = UnityActivity.Call<AndroidJavaObject>("getClass").Call<string>("getName");
                    return (className == PLUGIN_ACTIVITY || className == PLUGIN_NATIVE_ACTIVITY);
#else
					return false;
#endif
                }
            }

            public static AndroidJavaObject UnityActivity
            {
                get
                {
#if UNITY_ANDROID
                    using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass(UNITY_PLAYER))
                    {
                        return unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                    }
#else
					return null;				
#endif
                }
            }

            public static AndroidJavaObject UnityContext
            {
                get
                {
#if UNITY_ANDROID
                    using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass(UNITY_PLAYER))
                    {
                        using (AndroidJavaObject activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
                        {
                            return activityInstance.Call<AndroidJavaObject>("getApplicationContext");
                        }
                    }
#else
					return null;				
#endif
                }
            }

            public static string PackageName
            {
                get
                {
#if UNITY_ANDROID
                    return AndroidSystem.UnityActivity.Call<string>("getPackageName");
#else
					return "";
#endif
                }
            }

            public static int PackageVersion
            {
                get
                {
#if UNITY_ANDROID
                    using (AndroidJavaObject pkgManager = AndroidSystem.UnityContext
                                                                       .Call<AndroidJavaObject>("getPackageManager"))
                    {

                        AndroidJavaObject packageInfo = pkgManager.Call<AndroidJavaObject>("getPackageInfo",
                                                                                           AndroidSystem.PackageName,
                                                                                           0);
                        return packageInfo.Get<int>("versionCode");
                    }
#else
                    return -1;
#endif
                }
            }

            public static void PackIntentExtras(AndroidJavaObject intent, Hashtable extras)
            {
#if UNITY_ANDROID
                if (extras == null)
                {
                    return;
                }

                foreach (DictionaryEntry entry in extras)
                {
                    if (entry.Value is short)
                    {
                        intent.Call<AndroidJavaObject>("putExtra", (string)entry.Key, (short)entry.Value);
                    }
                    else if (entry.Value is int)
                    {
                        intent.Call<AndroidJavaObject>("putExtra", (string)entry.Key, (int)entry.Value);
                    }
                    else if (entry.Value is long)
                    {
                        intent.Call<AndroidJavaObject>("putExtra", (string)entry.Key, (long)entry.Value);
                    }
                    else if (entry.Value is float)
                    {
                        intent.Call<AndroidJavaObject>("putExtra", (string)entry.Key, (float)entry.Value);
                    }
                    else if (entry.Value is double)
                    {
                        intent.Call<AndroidJavaObject>("putExtra", (string)entry.Key, (double)entry.Value);
                    }
                    else if (entry.Value is string)
                    {
                        intent.Call<AndroidJavaObject>("putExtra", (string)entry.Key, (string)entry.Value);
                    }
                    else if (entry.Value is string[])
                    {
                        IntPtr putExtraPtr = AndroidJNIHelper.GetMethodID(intent.GetRawClass(), "putExtra", "(Ljava/lang/String;[Ljava/lang/String;)Landroid/content/Intent;");
                        IntPtr keyPtr = AndroidJNI.NewStringUTF((string)entry.Key);
                        IntPtr arrayPtr = AndroidJNIHelper.ConvertToJNIArray((string[])entry.Value);

                        jvalue[] args = new jvalue[2];
                        args[0] = new jvalue();
                        args[0].l = keyPtr;
                        args[1] = new jvalue();
                        args[1].l = arrayPtr;

                        AndroidJNI.CallObjectMethod(intent.GetRawObject(), putExtraPtr, args);

                        AndroidJNI.DeleteLocalRef(keyPtr);
                        AndroidJNI.DeleteLocalRef(arrayPtr);
                    }
                    else if (entry.Value is AndroidJavaObject)
                    {
                        AndroidJavaObject javaObj = entry.Value as AndroidJavaObject;

                        using (AndroidJavaClass _Parcelable = new AndroidJavaClass(AndroidSystem.PARCELABLE))
                        {
                            if (AndroidJNI.IsInstanceOf(javaObj.GetRawObject(), _Parcelable.GetRawClass()))
                            {
                                intent.Call<AndroidJavaObject>("putExtra", (string)entry.Key, javaObj);
                            }
                            else
                            {
                                throw new ArgumentException("Argument is not a Android Parcelable", "extra." + entry.Key);
                            }
                        }
                    }
                }
#endif
            }

            // TODO implement
            public static Hashtable ParseBundle(AndroidJavaObject bundleObject)
            {

                return null;
            }

            private static bool RegisterOnActivityResultCallback(OnActivityResultDelegate callback)
            {
#if UNITY_ANDROID

                // Store the callback
                OnActivityResult = callback;

                if (onActivityResultInternalPtr == IntPtr.Zero)
                {
                    // Get native pointer for delegate
                    onActivityResultInternal = new OnActivityResultInternal(OnActivityResultInternalImpl);
                    onActivityResultInternalHandle = GCHandle.Alloc(onActivityResultInternal);
                    onActivityResultInternalPtr = Marshal.GetFunctionPointerForDelegate(onActivityResultInternal);

                    if (onActivityResultInternalPtr == IntPtr.Zero)
                    {
                        Debug.LogError("Cannot get unmanaged pointer for OnActivityResult delegate, pointer value is " + onActivityResultInternalPtr.ToInt32());
                        return false;
                    }

                    return RegisterOnActivityResultCallback(onActivityResultInternalPtr);

                }
#endif

                return false;
            }

            public static bool RegisterOnNewIntentCallback(OnNewIntentDelegate callback)
            {
#if UNITY_ANDROID
                // Store the callback
                OnNewIntent = callback;

                if (onNewIntentInternalPtr == IntPtr.Zero)
                {
                    // Get native pointer for delegate
                    onNewIntentInternal = new OnNewIntentInternal(OnNewIntentInternalImpl);
                    onNewIntentInternalHandle = GCHandle.Alloc(onNewIntentInternal);
                    onNewIntentInternalPtr = Marshal.GetFunctionPointerForDelegate(onNewIntentInternal);

                    if (onNewIntentInternalPtr == IntPtr.Zero)
                    {
                        Debug.LogError("Cannot get unmanaged pointer for OnNewIntent delegate");
                        return false;
                    }

                    return RegisterOnNewIntentCallback(onNewIntentInternalPtr);
                }

                return true;
#else
                return false;
#endif
            }

#if UNITY_ANDROID
            [DllImport("unityandroidsystem")]
            public static extern bool RegisterOnActivityResultCallback(IntPtr callbackPtr);

            [DllImport("unityandroidsystem")]
            public static extern bool RegisterOnNewIntentCallback(IntPtr callbackPtr);
#endif

            public static AndroidJavaObject ConstructJavaObjectFromPtr(IntPtr javaPtr)
            {
#if UNITY_ANDROID
                // Pointer could be null, so return null object
                if (javaPtr.ToInt32() == 0)
                    return null;

#if UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1
				return new AndroidJavaObject(javaPtr);			
#else
                // Starting with Unity 4.2, AndroidJavaObject constructor with IntPtr arg is private,
                // so, construct using brute force :)
                Type t = typeof(AndroidJavaObject);
                Type[] types = new Type[1];
                types[0] = typeof(IntPtr);
                ConstructorInfo javaObjConstructor = t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, types, null);

                return javaObjConstructor.Invoke(new object[] { javaPtr }) as AndroidJavaObject;
#endif
#else
                return null;
#endif
            }

            public static void SendBroadcast(string action, Hashtable extras = null)
            {
#if UNITY_ANDROID
                AndroidJavaObject intent = new AndroidJavaObject(AndroidSystem.INTENT, action);
                // Add args to intent
                PackIntentExtras(intent, extras);

                SendBroadcast(intent);
#endif
            }

            public static void SendBroadcast(AndroidJavaObject intent)
            {
#if UNITY_ANDROID
                using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass(AndroidSystem.UNITY_PLAYER))
                {
                    using (AndroidJavaObject activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        activityInstance.Call("sendBroadcast", intent);
                    }
                }
#endif
            }

            public static void StartActivity(AndroidJavaObject intent)
            {
#if UNITY_ANDROID
                AndroidSystem.UnityActivity.Call("startActivity", intent);
#endif
            }

            public static bool StartActivityForResult(string action, int requestCode, OnActivityResultDelegate callback)
            {
#if UNITY_ANDROID
                return StartActivityForResult(action, null, requestCode, callback);
#else
                return false;
#endif
        }

            public static bool StartActivityForResult(string action, AndroidJavaObject uriData, int requestCode, OnActivityResultDelegate callback)
            {
#if UNITY_ANDROID
                if (callback == null)
                {
                    throw new System.ArgumentNullException("OnActivityResult callback cannot be null");
                }

                if (string.IsNullOrEmpty(action))
                {
                    throw new System.ArgumentNullException("");
                }

                // Create intent
                AndroidJavaObject intent = uriData != null ? new AndroidJavaObject(AndroidSystem.INTENT, action, uriData)
                                                           : new AndroidJavaObject(AndroidSystem.INTENT, action);

                // Start given action
                return StartActivityForResult(intent, requestCode, callback);
#else
				return false;
#endif
            }

            public static bool StartActivityForResult(AndroidJavaObject intent, int requestCode, OnActivityResultDelegate callback)
            {
                bool ret = false;

#if UNITY_ANDROID
                /*
				using (AndroidJavaObject pkgManager = AndroidSystem.UnityContext.Call<AndroidJavaObject>("getPackageManager")) {
					if (intent.Call<AndroidJavaObject>("resolveActivity", pkgManager) == null) {
						Debug.LogError("Requested action is not supported");
						return false;
					}
				}
				*/
                using (AndroidJavaObject activityInstance = AndroidSystem.UnityActivity)
                {
                    // Register given callback
                    ret = RegisterOnActivityResultCallback(callback);

                    activityInstance.Call("startActivityForResult", intent, requestCode);
                }
#endif
                return ret;
            }
        }
    }
}