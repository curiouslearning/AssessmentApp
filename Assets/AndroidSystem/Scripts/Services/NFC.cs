using System;
using UnityEngine;
using FourthSky.Android;

namespace FourthSky
{
    namespace Android
    {
        namespace Services
        {
            public enum TypeNameField : short
            {
                Empty = 0,
                WellKnown = 1,
                MimeMedia = 2,
                AbsoluteUri = 3,
                External = 4,
                Unknown = 5,
                Unchanged = 6
            }

            public static class RTD
            {
                public static readonly byte[] RTD_TEXT = { 0x54 };  // "T"
                public static readonly byte[] RTD_URI = { 0x55 };   // "U"
                public static readonly byte[] RTD_SMART_POSTER = { 0x53, 0x70 };  // "Sp"
                public static readonly byte[] RTD_ALTERNATIVE_CARRIER = { 0x61, 0x63 };  // "ac"
                public static readonly byte[] RTD_HANDOVER_CARRIER = { 0x48, 0x63 };  // "Hc"
                public static readonly byte[] RTD_HANDOVER_REQUEST = { 0x48, 0x72 };  // "Hr"
                public static readonly byte[] RTD_HANDOVER_SELECT = { 0x48, 0x73 }; // "Hs"
                public static readonly byte[] RTD_ANDROID_APP = System.Text.Encoding.UTF8.GetBytes("android.com:pkg");
            }

            public struct Record
            {

                public static readonly string[] URI_PREFIXES = new string[] {
                    "",
                    "http://www.",
                    "https://www.",
                    "http://",
                    "https://",
                    "tel:",
                    "mailto:",
                    "ftp://anonymous:anonymous@",
                    "ftp://ftp.",
                    "ftps://",
                    "sftp://",
                    "smb://",
                    "nfs://",
                    "ftp://",
                    "dav://",
                    "news:",
                    "telnet://",
                    "imap:",
                    "rtsp://",
                    "urn:",
                    "pop:",
                    "sip:",
                    "sips:",
                    "tftp:",
                    "btspp://",
                    "btl2cap://",
                    "btgoep://",
                    "tcpobex://",
                    "irdaobex://",
                    "file://",
                    "urn:epc:id:",
                    "urn:epc:tag:",
                    "urn:epc:pat:",
                    "urn:epc:raw:",
                    "urn:epc:",
                    "urn:nfc:",
                };

                public byte[] id;
                public byte[] type;
                public TypeNameField tnf;
                public byte[] payload;

                public bool IsText
                {
                    get
                    {
                        return (TypeNameField.WellKnown == tnf) && (NFC.ByteArraysEqual(RTD.RTD_TEXT, type));
                    }
                }

                public bool IsUri
                {
                    get
                    {
                        return (TypeNameField.WellKnown == tnf && NFC.ByteArraysEqual(RTD.RTD_URI, type))
                            || (TypeNameField.AbsoluteUri == tnf)
                            || (TypeNameField.External == tnf);
                    }
                }

                public string TypeNameFieldASCII
                {
                    get
                    {
                        switch (tnf)
                        {
                            case TypeNameField.WellKnown: return "well known";
                            case TypeNameField.MimeMedia: return "media";
                            case TypeNameField.AbsoluteUri: return "absolute uri";
                            case TypeNameField.External: return "external";
                            case TypeNameField.Unknown: return "unknown";
                            case TypeNameField.Unchanged: return "unchanged";
                            case TypeNameField.Empty:
                            default:
                                return "empty";
                        }
                    }
                }

#pragma warning disable 168
                public string PayloadASCII
                {
                    get
                    {
                        try
                        {
                            return System.Text.Encoding.ASCII.GetString(payload);
                        }
                        catch (Exception ex)
                        {
                            return "";
                        }
                    }
                }

                public string TypeASCII
                {
                    get
                    {
                        if (TypeNameField.MimeMedia == tnf)
                        {
                            try
                            {
                                return System.Text.Encoding.ASCII.GetString(type);
                            }
                            catch (Exception ex)
                            {
                                return "";
                            }
                        }

                        return "";
                    }
                }

#pragma warning restore 168

                public string StringValue
                {
                    get
                    {
                        if (TypeNameField.WellKnown == tnf)
                        {
                            if (NFC.ByteArraysEqual(RTD.RTD_TEXT, type))
                            {
                                int langLength = (int)payload[0];
                                string langCode = System.Text.Encoding.UTF8.GetString(payload, 1, langLength);
                                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(langCode);
                                if (encoding != null)
                                {
                                    return encoding.GetString(payload, 1 + langLength, payload.Length);
                                }
                            }
                        }

                        return "";
                    }
                }

                public string UriValue
                {
                    get
                    {
                        switch (tnf)
                        {
                            case TypeNameField.WellKnown:
                                {

                                    if (NFC.ByteArraysEqual(RTD.RTD_URI, type))
                                    {
                                        if (payload.Length < 2)
                                        {
                                            break;
                                        }

                                        // payload[0] contains the URI Identifier Code, as per
                                        // NFC Forum "URI Record Type Definition" section 3.2.2.
                                        int prefixIndex = (payload[0] & (byte)0xFF);
                                        if (prefixIndex < 0 || prefixIndex >= URI_PREFIXES.Length)
                                        {
                                            break;
                                        }
                                        string prefix = URI_PREFIXES[prefixIndex];
                                        string suffix = System.Text.Encoding.UTF8.GetString(payload, 1, payload.Length);
                                        return prefix + suffix;
                                    }
                                    else if (NFC.ByteArraysEqual(RTD.RTD_SMART_POSTER, type))
                                    {
                                        // TODO implement
                                    }
                                }
                                break;
                            case TypeNameField.AbsoluteUri:
                                {
                                    string uri = System.Text.Encoding.UTF8.GetString(type);
                                    return uri.ToLower();
                                }
                            case TypeNameField.External:
                                {
                                    return "vnd.android.nfc://ext/" + System.Text.Encoding.ASCII.GetString(type);
                                }
                            default:
                                return "";
                        }

                        return "";
                    }
                }

                public static Record CreateText(string text, string langCode = "en")
                {
                    Record r = new Record();

                    byte[] langBytes = System.Text.Encoding.UTF8.GetBytes(langCode);
                    byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
                    r.payload = new byte[textBytes.Length + 1 + langBytes.Length];
                    r.payload[0] = (byte)(0x3F & langBytes.Length);
                    Array.Copy(langBytes, 0, r.payload, 1, langBytes.Length);
                    Array.Copy(textBytes, 0, r.payload, 1 + langBytes.Length, textBytes.Length);

                    r.tnf = TypeNameField.WellKnown;
                    r.type = RTD.RTD_TEXT;
                    r.id = new byte[0];

                    return r;
                }

                public static Record CreateUri(string uri)
                {
                    Record r = new Record();

                    int prefix = 0;
                    for (int i = 1; i < URI_PREFIXES.Length; i++)
                    {
                        if (uri.StartsWith(URI_PREFIXES[i]))
                        {
                            prefix = i;
                            break;
                        }
                    }
                    if (prefix > 0) uri = uri.Substring(URI_PREFIXES[prefix].Length);
                    byte[] uriBytes = System.Text.Encoding.UTF8.GetBytes(uri);
                    r.payload = new byte[uriBytes.Length + 1];
                    r.payload[0] = (byte)prefix;
                    Array.Copy(uriBytes, 0, r.payload, 1, r.payload.Length);

                    r.tnf = TypeNameField.WellKnown;
                    r.type = RTD.RTD_URI;
                    r.id = new byte[0];

                    return r;
                }

                public static Record CreateMime(String mimeType, byte[] data)
                {
                    Record r = new Record();

                    r.tnf = TypeNameField.MimeMedia;
                    r.type = System.Text.Encoding.ASCII.GetBytes(mimeType);
                    r.id = new byte[0];
                    r.payload = data;

                    return r;
                }

                public static Record CreateBinary(byte[] data)
                {
                    Record r = new Record();

                    r.tnf = TypeNameField.Unknown;
                    r.type = null;
                    r.id = null;
                    r.payload = data;

                    return r;
                }

                public static Record CreateExternal(string domain, string type, byte[] data)
                {
                    Record r = new Record();

                    byte[] domainBytes = System.Text.Encoding.UTF8.GetBytes(domain.ToLower());
                    byte[] typeBytes = System.Text.Encoding.UTF8.GetBytes(type.ToLower());
                    byte[] b = new byte[domainBytes.Length + 1 + typeBytes.Length];

                    Array.Copy(domainBytes, 0, b, 1, domainBytes.Length);
                    b[domainBytes.Length] = (byte)':';
                    Array.Copy(typeBytes, 0, b, 1, typeBytes.Length);

                    r.tnf = TypeNameField.External;
                    r.type = b;
                    r.id = new byte[0];
                    r.payload = data;

                    return r;
                }

                public static Record CreateAAR()
                {
                    Record r = new Record();

                    r.tnf = TypeNameField.External;
                    r.type = RTD.RTD_ANDROID_APP;
                    r.id = new byte[0];
                    r.payload = System.Text.Encoding.ASCII.GetBytes(AndroidSystem.PackageName);

                    return r;
                }
            }

            public class Tag
            {

                private byte[] mId;
                private string[] mTechList;

#if UNITY_ANDROID
                internal readonly AndroidJavaObject mTagObj;

                public Tag(AndroidJavaObject obj)
                {
                    mTagObj = obj;
                }
#endif

                public Tag()
                {
                    
                }

                public byte[] id
                {
                    get
                    {
#if UNITY_ANDROID
                        if (mId == null)
                        {
                            using (AndroidJavaObject obj = mTagObj.Call<AndroidJavaObject>("getId"))
                            {
                                if (obj.GetRawObject().ToInt32() != 0)
                                    mId = AndroidJNIHelper.ConvertFromJNIArray<byte[]>(obj.GetRawObject());
                            }
                        }
#endif
                        return mId;
                    }
                }

                public string[] techList
                {
                    get
                    {
#if UNITY_ANDROID
                        if (mTechList == null)
                        {
                            using (AndroidJavaObject obj = mTagObj.Call<AndroidJavaObject>("getTechList"))
                            {
                                if (obj.GetRawObject().ToInt32() != 0)
                                    mTechList = AndroidJNIHelper.ConvertFromJNIArray<string[]>(obj.GetRawObject());
                            }
                        }
#endif
                        return mTechList;
                    }
                }
            }

            public abstract class TagTechnology
            {
#if UNITY_ANDROID
                protected readonly AndroidJavaObject mTechObj;
#endif
                public bool Connected
                {
                    get
                    {
#if UNITY_ANDROID
                        return mTechObj.Call<bool>("isConnected");
#else
						return false;
#endif
                    }
                }

#if UNITY_ANDROID
                public TagTechnology(AndroidJavaObject tech)
                {
                    mTechObj = tech;
                }
#else
				public TagTechnology () {
					
				}
#endif

                ~TagTechnology()
                {
                    Close();
                }

                public void Connect()
                {
#if UNITY_ANDROID
                    if (mTechObj != null)
                        mTechObj.Call("connect");
#endif
                }

                public void Close()
                {
#if UNITY_ANDROID
                    if (mTechObj != null)
                        mTechObj.Call("close");
#endif
                }

            }

            public class Ndef : TagTechnology
            {

                public const string MIFARE_CLASSIC = "com.nxp.ndef.mifareclassic";
                public const string NFC_FORUM_TYPE_1 = "org.nfcforum.ndef.type1";
                public const string NFC_FORUM_TYPE_2 = "org.nfcforum.ndef.type2";
                public const string NFC_FORUM_TYPE_3 = "org.nfcforum.ndef.type3";
                public const string NFC_FORUM_TYPE_4 = "org.nfcforum.ndef.type4";

                public static Ndef Get(Tag tag)
                {
#if UNITY_ANDROID
                    using (AndroidJavaClass NdefClass = new AndroidJavaClass("android.nfc.tech.Ndef"))
                    {
                        AndroidJavaObject ndef = NdefClass.CallStatic<AndroidJavaObject>("get", tag.mTagObj);
                        return new Ndef(ndef);
                    }
#else
					return null;
#endif
                }

#if UNITY_ANDROID
                private Ndef(AndroidJavaObject tech) : base(tech)
                {

                }
#else
				private Ndef () : base() {
					
				}
#endif

                public string Type
                {
                    get
                    {
#if UNITY_ANDROID
                        if (mTechObj != null)
                            return mTechObj.Call<string>("getType");
#endif
                        return "";
                    }
                }

                public bool CanMakeReadOnly
                {
                    get
                    {
#if UNITY_ANDROID
                        if (mTechObj != null)
                            return mTechObj.Call<bool>("canMakeReadOnly");
#endif
                        return false;
                    }
                }

                public int MaxSize
                {
                    get
                    {
#if UNITY_ANDROID
                        if (mTechObj != null)
                            return mTechObj.Call<int>("getMaxSize");
#endif
                        return 0;
                    }
                }

#if UNITY_ANDROID
                public AndroidJavaObject CachedNdefMessage
                {
                    get
                    {
                        if (mTechObj != null)
                            return mTechObj.Call<AndroidJavaObject>("getCachedNdefMessage");

                        return null;
                    }
                }

                public AndroidJavaObject NdefMessage
                {
                    get
                    {
                        if (mTechObj != null)
                            return mTechObj.Call<AndroidJavaObject>("getNdefMessage");

                        return null;
                    }
                }
#endif
                public bool Writable
                {
                    get
                    {
#if UNITY_ANDROID
                        if (mTechObj != null)
                            return mTechObj.Call<bool>("isWritable");
#endif
                        return false;
                    }
                }

                public bool MakeReadOnly()
                {
#if UNITY_ANDROID
                    if (mTechObj != null)
                        return mTechObj.Call<bool>("makeReadOnly");
#endif
                    return false;
                }

#if UNITY_ANDROID
                public void WriteNdefMessage(AndroidJavaObject msg)
                {
                    if (mTechObj != null)
                        mTechObj.Call("writeNdefMessage", msg);
                }
#endif
            }


            public static class NFC
            {
                #region public methods

                public static bool Supported
                {
                    get
                    {
#if UNITY_ANDROID
                        if (NfcAdapter != null && NfcAdapter.GetRawObject() != IntPtr.Zero)
                        {
                            return true;
                        }
#endif
                        Debug.LogError("NFC adapter not present in this device");
                        return false;
                    }
                }

                public static bool Enabled
                {
                    get
                    {
#if UNITY_ANDROID
                        if (!NFC.Supported)
                        {
                            return false;
                        }

                        bool enabled = NfcAdapter.Call<bool>("isEnabled");
                        if (!enabled)
                        {
                            Debug.LogError("NFC Adapter is disabled");
                            return false;
                        }

                        if (AndroidSystem.Version >= AndroidVersions.JELLY_BEAN)
                        {
                            enabled = NfcAdapter.Call<bool>("isNdefPushEnabled");
                            if (!enabled)
                            {
                                Debug.LogError("Android Bean is disabled");
                                return false;
                            }
                        }

                        return true;
#else
						return false;
#endif
                    }
                }

                public static bool Publish(string message, Action sentCallback = null)
                {
                    // Register callback to listen when messages was sent
                    //RegisterOnPushCompletedCallback(sentCallback);

                    Record[] records = { Record.CreateText(message) };
                    return InternalPublish(records);
                }

                public static bool Publish(byte[] data, Action sentCallback = null)
                {
                    // Register callback to listen when messages was sent
                    //RegisterOnPushCompletedCallback(sentCallback);

                    Record[] records = { Record.CreateBinary(data) };
                    return InternalPublish(records);
                }

                public static bool Publish(Record[] records, Action sendCallback = null)
                {
                    // Register callback to listen when messages was sent
                    //RegisterOnPushCompletedCallback(sentCallback);

                    return InternalPublish(records);
                }

                public static void CancelPublish()
                {
#if UNITY_ANDROID
                    IntPtr method = AndroidJNIHelper.GetMethodID(NfcAdapter.GetRawClass(), "setNdefPushMessage");
                    jvalue[] args = new jvalue[2];
                    args[0].l = IntPtr.Zero;
                    args[1].l = IntPtr.Zero;
                    AndroidJNI.CallVoidMethod(NfcAdapter.GetRawObject(), method, args);
#endif
                }

                public static bool Subscribe(Action<Record[]> callback)
                {
                    // Store callback
                    NFC.recordCallback = callback;
#if UNITY_ANDROID
                    // Register for NFC event
                    AndroidSystem.RegisterOnNewIntentCallback(OnNewIntent);
                    return true;
#else
					return false;
#endif
                }

                public static bool ListenNFCTag(Action<Record[]> callback)
                {
                    return ListenNFCTag("*/*", callback);
                }

                public static bool ListenNFCTag(String dataType, Action<Record[]> callback)
                {
                    // Store callback
                    NFC.recordCallback = callback;
#if UNITY_ANDROID
                    if (!NFC.Supported)
                        return false;

                    // Register for NFC event
                    AndroidSystem.RegisterOnNewIntentCallback(OnNewIntent);

                    // Start activity to receive NFC events (reduce dependencies)
                    AndroidJavaClass receiverClass = new AndroidJavaClass(AndroidSystem.PLUGIN_PACKAGE + NFC_RECEIVER);
                    AndroidJavaObject intent = new AndroidJavaObject(AndroidSystem.INTENT, AndroidSystem.UnityActivity, receiverClass);

                    intent.Call<AndroidJavaObject>("putExtra", "data_type", dataType);
                    intent.Call<AndroidJavaObject>("putExtra", "tag_receiving", true);
                    intent.Call<AndroidJavaObject>("putExtra", "techList", new string[] { "Ndef", "NfcA", "NfcF" });

                    AndroidSystem.StartActivity(intent);

                    return true;
#else
					return false;
#endif
                }

                #endregion

                #region private variables and constants
                private const string NFC_PKG = "android.nfc";
                private const string TECH_PKG = "android.nfc.tech";
                private const string NFC_ADAPTER = NFC_PKG + ".NfcAdapter";
                private const string NDEF_RECORD = NFC_PKG + ".NdefRecord";
                private const string NDEF_MESSAGE = NFC_PKG + ".NdefMessage";
                private const string NDEF = TECH_PKG + ".Ndef";

                private const string NFC_RECEIVER = ".UnityNFCActivityReceiver";

//#pragma warning disable 169
                private static Action messageSentCallback;
                private static Action<string> stringCallback;
                private static Action<byte[]> binaryCallback;
                private static Action<Record[]> recordCallback;
                private static Action<Tag> tagCallback;
//#pragma warning restore 169

#if UNITY_ANDROID
                private static AndroidJavaObject mNfcAdapter;

//#pragma warning disable 169
                private static IntPtr pushCallbackProxy;
//#pragma warning restore 169
                                
                //private static NdefPushCompleteCallback pushCallback;

                /*
				private class NdefPushCompleteCallback : AndroidJavaProxy {
					
					public NdefPushCompleteCallback() 
					: base("android.nfc.NfcAdapter$OnNdefPushCompleteCallback") {
						
					}
					
					void onNdefPushComplete(AndroidJavaObject evt) {
						if (NFC.messageSentCallback != null) {
							NFC.messageSentCallback.Invoke();
						}
						
						NFC.messageSentCallback = null;
					}				
				}
				
				*/
#endif

                #endregion

                #region private methods

#if UNITY_ANDROID
                private static AndroidJavaObject NfcAdapter
                {
                    get
                    {
                        if (mNfcAdapter == null)
                        {
                            using (AndroidJavaClass NfcAdapter = new AndroidJavaClass(NFC_ADAPTER))
                            {
                                mNfcAdapter = NfcAdapter.CallStatic<AndroidJavaObject>("getDefaultAdapter", AndroidSystem.UnityContext);
                            }
                        }

                        return mNfcAdapter;
                    }
                }
#endif

                public static bool ByteArraysEqual(byte[] b1, byte[] b2)
                {
                    if (b1 == b2) return true;
                    if (b1 == null || b2 == null) return false;
                    if (b1.Length != b2.Length) return false;
                    for (int i = 0; i < b1.Length; i++)
                    {
                        if (b1[i] != b2[i]) return false;
                    }
                    return true;
                }

                private static bool InternalPublish(Record[] records)
                {
#if UNITY_ANDROID
                    try
                    {
                        AndroidJavaObject[] javaRecords = new AndroidJavaObject[records.Length];
                        for (int i = 0; i < records.Length; i++)
                        {
                            Record rec = records[i];
                            javaRecords[i] = new AndroidJavaObject(NDEF_RECORD, rec.tnf, rec.type, rec.id, rec.payload);
                        }

                        // Create message with records
                        AndroidJavaObject ndefMessage = new AndroidJavaObject(NDEF_MESSAGE, new object[] { records });

                        jvalue[] args = { new jvalue(), new jvalue(), new jvalue() };
                        args[0].l = ndefMessage.GetRawObject();
                        args[1].l = AndroidSystem.UnityActivity.GetRawObject();
                        args[2].l = AndroidJNI.NewObjectArray(1,
                                                              AndroidJNI.FindClass("android/app/Activity"),
                                                              AndroidSystem.UnityActivity.GetRawObject());
                        IntPtr method = AndroidJNIHelper.GetMethodID(NfcAdapter.GetRawClass(),
                                                                     "setNdefPushMessage",
                                                                     "(Landroid/nfc/NdefMessage;Landroid/app/Activity;[Landroid/app/Activity;)V");
                        AndroidJNI.CallVoidMethod(NfcAdapter.GetRawObject(),
                                                   method,
                                                   args);

                        return true;

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Error publishing NFC message: " + ex.Message);

                        return false;
                    }
#else
					return false;
#endif
                }

                /*
				private static bool InternalPublish (AndroidJavaObject message) {
					try {
						jvalue [] args = { new jvalue(), new jvalue(), new jvalue() };
						args[0].l = message.GetRawObject();
						args[1].l = AndroidSystem.UnityActivity.GetRawObject();
						args[2].l = AndroidJNI.NewObjectArray(1,
						                                      AndroidJNI.FindClass("android/app/Activity"),
						                                      AndroidSystem.UnityActivity.GetRawObject());
						IntPtr method = AndroidJNIHelper.GetMethodID(NfcAdapter.GetRawClass(), 
						                                             "setNdefPushMessage",
						                                             "(Landroid/nfc/NdefMessage;Landroid/app/Activity;[Landroid/app/Activity;)V");
						AndroidJNI.CallVoidMethod (NfcAdapter.GetRawObject(), 
						                           method, 
						                           args);
						
						return true;
						
					} catch (Exception ex) {
						Debug.LogError("Error publishing NFC message: " + ex.Message);
						
						return false;
					}
				}
				*/

                /*
				private static void RegisterOnPushCompletedCallback (Action sentCallback = null) {
					
					messageSentCallback = sentCallback;
					
					try {
						// Create push completed callback
						if (pushCallback == null || pushCallbackProxy == IntPtr.Zero) {
							pushCallback = new NdefPushCompleteCallback();
							pushCallbackProxy = AndroidJNIHelper.CreateJavaProxy(pushCallback);
							
							// Prepare arguments
							jvalue[] args = { new jvalue (), new jvalue (), new jvalue () };
							args[0].l = pushCallbackProxy;
							args[1].l = AndroidSystem.UnityActivity.GetRawObject();
							args[2].l = AndroidJNI.NewObjectArray(1,
							                                      AndroidJNI.FindClass("android/app/Activity"),
							                                      AndroidSystem.UnityActivity.GetRawObject());
							// Register push callback
							IntPtr method = AndroidJNIHelper.GetMethodID(NfcAdapter.GetRawClass(), 
							                                             "setOnNdefPushCompleteCallback", 
							                                             "(Landroid/nfc/NfcAdapter$OnNdefPushCompleteCallback;Landroid/app/Activity;[Landroid/app/Activity;)V");
							AndroidJNI.CallVoidMethod (NfcAdapter.GetRawObject(), method, args);
						}
						
					} catch (Exception ex) {
						Debug.LogError("Error registering OnPushCompleted callback: " + ex.Message);
					}
				}
				*/

#if UNITY_ANDROID
                private static void OnNewIntent(AndroidJavaObject intent)
                {
                    try
                    {
                        // Check intent action
                        string action = intent.Call<string>("getAction");
                        if (ActivityActions.NFC.ACTION_NDEF_DISCOVERED == action)
                        {
                            HandleNDEF(intent);
                        }
                        else if (ActivityActions.NFC.ACTION_TECH_DISCOVERED == action)
                        {
                            HandleTech(intent);
                        }
                        else if (ActivityActions.NFC.ACTION_TAG_DISCOVERED == action)
                        {
                            HandleTag(intent);
                        }
                        else if (ActivityActions.ACTION_VIEW == action)
                        {
                            HandleActionView(intent);
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }

                private static void HandleNDEF(AndroidJavaObject intent)
                {
                    // Get NdefMessages from received intent
                    AndroidJavaObject[] rawMsgs = intent.Call<AndroidJavaObject[]>("getParcelableArrayExtra", IntentExtras.NFC.EXTRA_NDEF_MESSAGES);
                    if (rawMsgs != null)
                    {

                        // Get first (and maybe only) NdefMessage
                        AndroidJavaObject msg = rawMsgs[0];
                        if (msg != null)
                        {

                            // Get records for message
                            AndroidJavaObject[] records = msg.Call<AndroidJavaObject[]>("getRecords");
                            if (records != null)
                            {

                                Record[] recordsArray = new Record[records.Length];
                                for (int i = 0; i < records.Length; i++)
                                {
                                    AndroidJavaObject record = records[i];
                                    Record rec = new Record();
                                    rec.id = record.Call<byte[]>("getId");
                                    rec.type = record.Call<byte[]>("getType");
                                    rec.tnf = (TypeNameField)record.Call<short>("getTnf");
                                    rec.payload = record.Call<byte[]>("getPayload");

                                    recordsArray[i] = rec;
                                }

                                recordCallback.Invoke(recordsArray);
                            }
                        }
                    }
                }

                private static void HandleTech(AndroidJavaObject intent)
                {

                }

                private static void HandleTag(AndroidJavaObject intent)
                {

                }

                private static void HandleActionView(AndroidJavaObject intent)
                {

                }
#endif

                #endregion
            }
        }
    }
}