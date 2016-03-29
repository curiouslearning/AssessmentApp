using UnityEngine;
using System;
using System.Collections;

namespace FourthSky
{
	namespace Android
	{
		namespace Services
		{

			public class AndroidCursor : AndroidWrapper {

				private string[] columnNames;

				private AndroidCursor(AndroidJavaObject cursor)
				{
					JavaObject = cursor;
				}

				protected override void Dispose(bool disposing) {
					
					if (!this.disposed) {
						if (disposing) {
							
							if (JavaObject != null) {
								// First, close the cursor
								JavaObject.Call ("close");

								// Now, release Java object
								JavaObject.Dispose();
								JavaObject = null;
							}
							
						}
					}
					this.disposed = true;
				}

				public int RowCount {
					get {
						return JavaObject.Call<int>("getCount");
					}
				}

				public string[] ColumnNames {
					get {
						if (columnNames == null)
						{
							columnNames = JavaObject.Call<string[]>("getColumnNames");
						}

						return columnNames;
					}
				}

				public int ColumnCount {
					get {
						return JavaObject.Call<int>("getColumnCount");
					}
				}

				public bool Closed {
					get {
						return JavaObject.Call<bool>("isClosed");
					}
				}

				public bool MoveToFirst ()
				{
					return JavaObject.Call<bool> ("moveToFirst");
				}

				public bool MoveToNext ()
				{
					return JavaObject.Call<bool> ("moveToNext");
				}

				public int GetColumnIndex (string columnName)
				{
					return JavaObject.Call<int> ("getColumnIndex", columnName);
				}

				public ReturnType Get<ReturnType>(int columnIndex)
				{
					ReturnType result = default(ReturnType);
					if (typeof(ReturnType).IsPrimitive)
					{
						if (typeof(ReturnType) == typeof(int))
						{
							result = JavaObject.Call<ReturnType>("getInt", columnIndex);
						}
						else
						{
							if (typeof(ReturnType) == typeof(short))
							{
								result = JavaObject.Call<ReturnType>("getShort", columnIndex);
							}
							else
							{
								if (typeof(ReturnType) == typeof(long))
								{
									result = JavaObject.Call<ReturnType>("getLong", columnIndex);
								}
								else
								{
									if (typeof(ReturnType) == typeof(float))
									{
										result = JavaObject.Call<ReturnType>("getFloat", columnIndex);
									}
									else
									{
										if (typeof(ReturnType) == typeof(double))
										{
											result = JavaObject.Call<ReturnType>("getDouble", columnIndex);
										}
										else
										{
											result = default(ReturnType);
										}
									}
								}
							}
						}
					}
					else
					{
						if (typeof(ReturnType) == typeof(string))
						{
							result = JavaObject.Call<ReturnType>("getString", columnIndex);
						}
						else
						{
							if (typeof(ReturnType) == typeof(byte[]))
							{
								result = JavaObject.Call<ReturnType>("getBlob", columnIndex);
							}
						}
					}

					return result;
				}

				/// <summary>
				/// Get the specified columnName.
				/// </summary>
				/// <param name="columnName">Column name.</param>
				/// <typeparam name="ReturnType">The 1st type parameter.</typeparam>
				public ReturnType Get<ReturnType>(string columnName) {
					int idx = GetColumnIndex (columnName);
					return Get<ReturnType> (idx);
				}

				/// <summary>
				/// Opens the cursor.
				/// </summary>
				/// <returns>The cursor.</returns>
				/// <param name="contentUri">Content URI.</param>
				/// <param name="columns">List of columns for the result</param>
				/// <param name="selection">Selection clause (WHERE clause)</param>
				/// <param name="selectionArgs">Arguments for selection</param>
				/// <param name="sortOrder">Sort order, by column and ASC or DESC</param>
				public static AndroidCursor Open(string contentUri, string[] columns, string selection = "", string[] selectionArgs = null, string sortOrder = "")
				{
					// TODO if Uri is null or empty, throw error
					if (string.IsNullOrEmpty(contentUri))
					{
						throw new System.ArgumentException("contentUri cannot be null");
					}

					// Uri of the file for the gallery 
					AndroidJavaObject uri = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse", contentUri);
					
					return Open (uri, columns, selection, selectionArgs, sortOrder);
				}

				public static AndroidCursor Open(AndroidJavaObject contentUri, string[] columns, string selection = "", string[] selectionArgs = null, string sortOrder = "")
				{
					// TODO if Uri is null or empty, throw error
					if (contentUri == null || contentUri.GetRawObject() == IntPtr.Zero)
					{
						throw new System.ArgumentException("contentUri cannot be null");
					}

					// Array of column names to get from database (array of java strings)
					AndroidJavaObject proj = null;
					if (columns != null && columns.Length > 0)
					{
						proj = AndroidSystem.ConstructJavaObjectFromPtr(AndroidJNI.NewObjectArray(columns.Length, 
						                                                                          AndroidJNI.FindClass("java/lang/String"), 
						                                                                          AndroidJNI.NewStringUTF("")));
						
						for (int i = 0; i < columns.Length; i++)
							AndroidJNI.SetObjectArrayElement(proj.GetRawObject(), i, AndroidJNI.NewStringUTF(columns[i]));
					}
					
					// Arguments for selection (array of java strings)
					AndroidJavaObject args = null;
					if (selectionArgs != null && selectionArgs.Length > 0)
					{
						args = AndroidSystem.ConstructJavaObjectFromPtr(AndroidJNIHelper.ConvertToJNIArray(selectionArgs));
					}
					
					AndroidJavaObject cursor = null;
					
					if (AndroidSystem.Version > AndroidVersions.GINGERBREAD_MR1)
					{
						// This code is for Android 3.0 (SDK 11) and up
						using (AndroidJavaObject loader = new AndroidJavaObject("android.content.CursorLoader", 
						                                                        AndroidSystem.UnityActivity))
						{
							loader.Call ("setUri", contentUri);
							loader.Call ("setProjection", proj);
							loader.Call ("setSelection", selection);
							loader.Call ("setSelectionArgs", args);
							loader.Call ("setSortOrder", sortOrder);
							
							cursor = loader.Call<AndroidJavaObject>("loadInBackground");
						}
					}
					else
					{
						// These two lines is for any Android version, but is deprecated for Android 3.0 and up
						// Need to get method through AndroidJNI
						IntPtr managedQueryMethod = AndroidJNIHelper.GetMethodID(AndroidSystem.UnityActivity.GetRawClass(), "query");
						IntPtr cursorPtr = AndroidJNI.CallObjectMethod(AndroidSystem.UnityActivity.GetRawObject(), 
						                                               managedQueryMethod,
						                                               AndroidJNIHelper.CreateJNIArgArray(new object[] {contentUri, 
																														proj, 
																														selection, 
																														args, 
																														sortOrder}));
						cursor = AndroidSystem.ConstructJavaObjectFromPtr(cursorPtr);
						
					}
					
					return new AndroidCursor(cursor);
				}

			}
		}
	}
}
