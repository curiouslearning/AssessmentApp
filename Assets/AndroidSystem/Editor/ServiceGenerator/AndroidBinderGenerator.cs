using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using FourthSky.Android;
using Object = UnityEngine.Object;

public class AndroidBinderGenerator : EditorWindow {

	// char array can't be const for compiler reasons but this should still be treated as such.
	private readonly char[] kInvalidPathChars = new char[] {'<', '>', ':', '"', '|', '?', '*', (char)0};
	private readonly char[] kPathSepChars = new char[] {'/', '\\'};
	
	private const int kButtonWidth = 120;
	private const int kLabelWidth = 85;
	
	private AIDLInterface iface = null;
	
	private string m_Directory = string.Empty;
	private string m_AidlFilePath = string.Empty;
	private string m_MonoAidlCode = string.Empty;
	
	private Vector2 m_PreviewScroll;
	
	class Styles
	{
		public GUIContent m_WarningContent = new GUIContent (string.Empty/*, EditorGUIUtility.LoadRequired("Builtin Skins/Icons/console.warnicon.sml.png") as Texture2D*/);
		public GUIStyle m_PreviewBox = new GUIStyle ("OL Box");
		public GUIStyle m_PreviewTitle = new GUIStyle ("OL Title");
		public GUIStyle m_LoweredBox = new GUIStyle ("TextField");
		public GUIStyle m_HelpBox = new GUIStyle ("helpbox");
		
		public Styles ()
		{
			m_LoweredBox.padding = new RectOffset (1, 1, 1, 1);
		}
	}
	private static Styles m_Styles;
	
	// Add menu item to the Window menu
	[MenuItem ("Window/Android Binder Generator")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		EditorWindow.GetWindow<AndroidBinderGenerator>(true, "Android Binder Generator");
	}
	
	public AndroidBinderGenerator() {
		// But allow to scale down to smaller size
		minSize = new Vector2 (850, 400);	
	}
	
	// Implement your own editor GUI here.
	void OnGUI () {
		if (m_Styles == null)
			m_Styles = new Styles ();
		
		EditorGUIUtility.LookLikeControls (85);
		
		EditorGUILayout.BeginHorizontal ();
		{
			GUILayout.Space (10);
			
			PreviewGUI ();
			GUILayout.Space (10);
			
			EditorGUILayout.BeginVertical ();
			{
				m_AidlFilePath = EditorGUILayout.TextField("AIDL File Path", m_AidlFilePath, GUILayout.ExpandWidth (true));
				
				m_Directory = EditorGUILayout.TextField ("Save in", m_Directory, GUILayout.ExpandWidth (true));
				HelpField ("Click a folder in the Project view to select.");
		
				GUILayout.Space (10);
				
				CreateButtonsGUI();		
			} EditorGUILayout.EndVertical ();		
		
			GUILayout.Space (10);
		
		} EditorGUILayout.EndHorizontal ();
	}
	
	private void PreviewGUI () {
		float previewWidth = position.width - 380f;
		EditorGUILayout.BeginVertical (GUILayout.Width (Mathf.Max (position.width * 0.4f, previewWidth)));
		{
			// Reserve room for preview title
			Rect previewHeaderRect = GUILayoutUtility.GetRect (new GUIContent ("Preview"), m_Styles.m_PreviewTitle);
			
			// Secret! Toggle curly braces on new line when double clicking the script preview title
			Event evt = Event.current;
			if (evt.type == EventType.MouseDown && evt.clickCount == 2 && previewHeaderRect.Contains (evt.mousePosition))
			{
				EditorPrefs.SetBool ("CurlyBracesOnNewLine", !EditorPrefs.GetBool ("CurlyBracesOnNewLine"));
				Repaint ();
			}

			// Preview scroll view
			m_PreviewScroll = EditorGUILayout.BeginScrollView (m_PreviewScroll, m_Styles.m_PreviewBox, GUILayout.Width(previewWidth));
			{
				EditorGUILayout.BeginHorizontal ();
				{
					// Tiny space since style has no padding in right side
					GUILayout.Space (5);
		
					// Preview text itself
					Rect r = GUILayoutUtility.GetRect (
						new GUIContent (m_MonoAidlCode),
						//EditorStyles.miniLabel,
						EditorStyles.textArea,
						GUILayout.ExpandWidth (true),
						GUILayout.ExpandHeight (true));
					EditorGUI.SelectableLabel (r, m_MonoAidlCode, EditorStyles.miniLabel);
				
				} EditorGUILayout.EndHorizontal ();
			
			} EditorGUILayout.EndScrollView ();

			// Draw preview title after box itself because otherwise the top row
			// of pixels of the slider will overlap with the title
			GUI.Label (previewHeaderRect, new GUIContent ("Preview"), m_Styles.m_PreviewTitle);
			
			GUILayout.Space (4);
		} EditorGUILayout.EndVertical ();
	}	
	
	private void Create() {
		if (!Directory.Exists (TargetDir ()))
			Directory.CreateDirectory (TargetDir ());
		
		var writer = new StreamWriter (TargetPath());
		writer.Write (m_MonoAidlCode);
		writer.Close ();
		writer.Dispose ();
		AssetDatabase.Refresh ();
		
		Close ();
		GUIUtility.ExitGUI ();
	}
	
	// Called 100 times per second on all visible
	// windows.
	void Update () {
		
	}
	
	// This function is called when the object
	// is loaded.
	void OnEnable () {
		OnSelectionChange();
	}
	
	// Handles selection change on editor
	private void OnSelectionChange ()
	{
		if (Selection.activeObject == null)
			return;
		
		if (IsFolder (Selection.activeObject)) {
			m_Directory = AssetPathWithoutAssetPrefix (Selection.activeObject);
		}
		
		Repaint();
	}
	
	private void LoadAndProcessAIDLFile() {
		m_AidlFilePath = EditorUtility.OpenFilePanel("AIDL File", "", "aidl");
		
		// Only open file is path is not empty
		if (!string.IsNullOrEmpty(m_AidlFilePath)) {
			string text = File.ReadAllText(m_AidlFilePath);
			
			iface = new AIDLInterface();
			iface.ParseInterface(text);
			
			m_MonoAidlCode = AIDLGenerator.GenerateBinder(iface);		
		}
	}
	
	private void HelpField (string helpText) {
		GUILayout.BeginHorizontal ();
		GUILayout.Label (string.Empty, GUILayout.Width (kLabelWidth - 4));
		GUILayout.Label (helpText, m_Styles.m_HelpBox);
		GUILayout.EndHorizontal ();
	}
	
	private string TargetDir () {
		return Path.Combine ("Assets", m_Directory.Trim (kPathSepChars));
	}
	
	private string TargetPath () {
		return Path.Combine (TargetDir (), iface.name + ".cs");
	}
	
	private bool IsFolder (Object obj) {
		return Directory.Exists(AssetDatabase.GetAssetPath(obj));
	}
	
	private string AssetPathWithoutAssetPrefix (Object obj) {
		return AssetDatabase.GetAssetPath(obj).Substring(7);
	}
	
	private bool ClassExists (string className) {
		return AppDomain.CurrentDomain.GetAssemblies ().Any (a => a.GetType (className, false) != null);
	}
	
	private bool ClassNameIsInvalid () {
		return !System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(iface.name);
	}
	
	private bool InvalidTargetPath ()
	{
		if (m_Directory.IndexOfAny (kInvalidPathChars) >= 0)
			return true;
		if (TargetDir().Split (kPathSepChars, StringSplitOptions.None).Contains (string.Empty))
			return true;
		return false;
	}
	
	private bool CanCreate() {
		return 	iface != null 				&&
			   	//!ClassExists(iface.name) 	&&
				!ClassNameIsInvalid()		&&
				m_Directory != null			&&
				m_Directory != "";
				
	}
	
	private void CreateButtonsGUI() {
		bool canCreate = CanCreate();
		
		// Create string to tell the user what the problem is
		string blockReason = string.Empty;
		if (!canCreate) {
			if (iface == null) {
				blockReason = "Choose an AIDL file to be parsed.";
				
			} else {
				if (File.Exists (TargetPath()))
					blockReason = "A script called \"" + iface.name + "\" already exists at that path.";
				//else if (ClassExists(iface.name))
				//	blockReason = "A class called \"" + iface.name + "\" already exists.";
				else if (ClassNameIsInvalid ())
					blockReason = "The script name may only consist of a-z, A-Z, 0-9, _.";
				else if (InvalidTargetPath ())
					blockReason = "The folder path contains invalid characters.";
					
			}
		}
		
		// Warning about why the script can't be created
		if (blockReason != string.Empty)
		{
			m_Styles.m_WarningContent.text = blockReason;
			GUILayout.BeginHorizontal (m_Styles.m_HelpBox);
			{
				GUILayout.Label (m_Styles.m_WarningContent, EditorStyles.wordWrappedMiniLabel);
			} GUILayout.EndHorizontal ();
		}
		
		// "Open AIDL" and "Generate" buttons
		GUILayout.BeginHorizontal ();
		{
			GUILayout.FlexibleSpace ();

			//if (GUI.Button(new Rect(5, 150, 130, 30), "Open AIDL File")) {
			if (GUILayout.Button ("Open AIDL File", GUILayout.Width (kButtonWidth))) {
				LoadAndProcessAIDLFile();
			}
			
			bool guiEnabledTemp = GUI.enabled;
			GUI.enabled = canCreate;
			if (GUILayout.Button ("Generate", GUILayout.Width (kButtonWidth))) {
				Create();
			}
			GUI.enabled = guiEnabledTemp;
			
		} GUILayout.EndHorizontal ();
		
	}
}