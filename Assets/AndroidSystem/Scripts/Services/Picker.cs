using System;
using System.IO;
using UnityEngine;


namespace FourthSky
{
    namespace Android
    {
        namespace Services
        {
            public struct PhoneContact
            {
                public string Name;
                public string PhoneNumber;
                public string Email;
            }

            public static class Picker
            {
                // Delegate that will receive the picked image
                public delegate void OnImagePickedDelegate(Texture2D tex);

                static Picker()
                {
                    helperGO = new GameObject("Helper Picker");
                    helperGO.hideFlags = HideFlags.HideInHierarchy;
                    GameObject.DontDestroyOnLoad(helperGO);
                    pickerHelper = helperGO.AddComponent<PickerBehaviorHelper>();
                }

                public static void PickImageFromGallery(OnImagePickedDelegate callback)
                {
                    // Store callback
                    pickerHelper.RegisterDelegate(callback);

#if UNITY_ANDROID
                    // Alert: "$" character denotes inner classes
                    // From MediaStore.Images class (there are also MediaStore.Video, MediaStore.Audio and MediaStore.Files) ...
                    using (AndroidJavaClass mediaClass = new AndroidJavaClass("android.provider.MediaStore$Images$Media"))
                    {

                        // ... and start gallery app to pick an image.
                        AndroidJavaObject pickImageURI = mediaClass.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI");
                        AndroidSystem.StartActivityForResult(ActivityActions.ACTION_PICK, pickImageURI, PICK_GALLERY, Picker.OnActivityResult);
                    }
#endif
                }

                public static void PickImageFromCamera(OnImagePickedDelegate callback)
                {
                    // Store callback
                    pickerHelper.RegisterDelegate(callback);

#if UNITY_ANDROID
                    // Create temporary file
                    AndroidJavaObject imageFile = CreateImageFile();
                    if (imageFile != null && imageFile.GetRawObject().ToInt32() != 0)
                    {
                        AndroidJavaObject fileUri = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("fromFile", imageFile);

                        // Start camera app
                        AndroidJavaObject intent = new AndroidJavaObject(AndroidSystem.INTENT, ActivityActions.Media.ACTION_IMAGE_CAPTURE);
                        intent.Call<AndroidJavaObject>("putExtra", IntentExtras.Media.EXTRA_OUTPUT, fileUri);

                        AndroidSystem.StartActivityForResult(intent, PICK_CAMERA, Picker.OnActivityResult);
                    }
#endif
                }

                public static bool PickContact(Action<PhoneContact> callback)
                {
#if UNITY_ANDROID
                    if (!AndroidSystem.ActivityCallbacksSupported)
                    {
                        Debug.Log("Activity callbacks not supported, cannot pick contact from device list");
                        return false;
                    }

                    // Store callback
                    pickContactCallback = callback;

                    // Get Uri of contacts action
                    AndroidJavaObject uri = new AndroidJavaClass("android.provider.ContactsContract$Contacts").GetStatic<AndroidJavaObject>("CONTENT_URI");

                    // Open contacts activity
                    return AndroidSystem.StartActivityForResult(ActivityActions.ACTION_PICK, uri, PICK_CONTACT, Picker.OnActivityResult);
#else
					return false;
#endif
                }


                // OnActivityResult constants variables
                private static readonly int PICK_GALLERY = 13463;
                private static readonly int PICK_CONTACT = 13464;
                private static readonly int PICK_CAMERA = 13465;
                private static string imagePickPath = "No image choosen";

                private static GameObject helperGO = null;
                private static PickerBehaviorHelper pickerHelper;

                private static Action<PhoneContact> pickContactCallback = null;

#if UNITY_ANDROID
                // OnActivityResult callback
                private static void OnActivityResult(int requestCode, int resultCode, AndroidJavaObject intent)
                {
                    try
                    {
                        if (requestCode == PICK_CONTACT)
                        {
                            if (resultCode == AndroidSystem.RESULT_OK)
                            {
                                // Get URI path 
                                AndroidJavaObject uri = intent.Call<AndroidJavaObject>("getData");

                                PhoneContact contact = new PhoneContact();

                                // First, query for contact name and phone
                                string[] columns = { "_id", "display_name", "data1" };
                                using (AndroidCursor cursor = AndroidCursor.Open(uri, columns))
                                {
                                    if (cursor.MoveToNext())
                                    {
                                        // Fill contact
                                        contact.Name = cursor.Get<string>(columns[1]);
                                        contact.PhoneNumber = cursor.Get<string>(columns[2]);

                                        string id = cursor.Get<string>(columns[0]);

                                        // Now, query for e-mail in another database
                                        string[] columnsEmail = { "data1" };
                                        using (AndroidCursor emailCursor = AndroidCursor.Open("", columnsEmail, "_id = ?", new string[] { id }))
                                        {
                                            if (emailCursor.MoveToNext())
                                            {
                                                contact.Email = emailCursor.Get<string>(columnsEmail[0]);
                                            }
                                        }
                                    }
                                }

                                // Finally, notify callback with chosen contact
                                pickContactCallback.Invoke(contact);

                                // In the end, clear callback reference
                                pickContactCallback = null;
                            }

                        }
                        else if (requestCode == PICK_GALLERY)
                        {
                            switch (resultCode)
                            {
                                case AndroidSystem.RESULT_OK:

                                    // Get URI path (for example: content://media/external/images/media/712)
                                    string contentUri = intent.Call<AndroidJavaObject>("getData").Call<string>("toString");
                                    //Debug.Log("URI from picked image: " + contentUri);

                                    // Get real path of file (for example: mnt/images/IMG357.jpg )
                                    string[] columns = { "_data" };
                                    using (AndroidCursor cursor = AndroidCursor.Open(contentUri, columns))
                                    {
                                        if (cursor.MoveToNext())
                                        {
                                            // Finally, get image file path
                                            imagePickPath = cursor.Get<string>("_data");
                                        }
                                    }

                                    //Debug.Log("File path of picked image: " + imagePickPath);

                                    // Defer image loading to picker helper, 
                                    // that will load the image in the main thread
                                    pickerHelper.NotifyImageLoaded(imagePickPath);

                                    break;

                                case AndroidSystem.RESULT_CANCELED:
                                    Debug.Log("Pick image operation cancelled");
                                    break;

                                default:
                                    Debug.LogError("Error occurred picking image from gallery");
                                    break;

                            }

                        }
                        else if (requestCode == PICK_CAMERA)
                        {
                            // Add to gallery (just to track existency)
                            AddToGallery(imagePickPath);

                            // Just pass the already created path to callback
                            if (imagePickPath != null)
                            {
                                //pickerHelper.NotifyImageLoaded(imagePickPath);
                                pickerHelper.imgPath = imagePickPath;
                                pickerHelper.foundTexture = true;
                            }

                        }



                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }

                private static AndroidJavaObject CreateImageFile()
                {
                    // Create an image file name
                    //string timeStamp = Time.time.ToString();
                    string timeStamp = DateTime.Now.TimeOfDay.TotalMilliseconds.ToString();
                    string imageFileName = "JPEG_" + timeStamp + "_";

                    AndroidJavaClass Environment = new AndroidJavaClass("android.os.Environment");
                    AndroidJavaClass File = new AndroidJavaClass("java.io.File");

                    string picturesPath = Environment.GetStatic<string>("DIRECTORY_PICTURES");
                    AndroidJavaObject storageDir = Environment.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", picturesPath);
                    AndroidJavaObject image = File.CallStatic<AndroidJavaObject>("createTempFile", imageFileName, ".jpg", storageDir);

                    imagePickPath = /*"file:" + */image.Call<string>("getAbsolutePath");

                    return image;

                }

                private static void AddToGallery(string imagePath)
                {
                    AndroidJavaObject mediaScanIntent = new AndroidJavaObject(AndroidSystem.INTENT, ActivityActions.ACTION_MEDIA_SCANNER_SCAN_FILE);
                    AndroidJavaObject f = new AndroidJavaObject("java.io.File", imagePath);
                    AndroidJavaObject contentUri = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("fromFile", f);
                    mediaScanIntent.Call<AndroidJavaObject>("setData", contentUri);
                    AndroidSystem.SendBroadcast(mediaScanIntent);
                }
#endif

            }

        }
    }
}

