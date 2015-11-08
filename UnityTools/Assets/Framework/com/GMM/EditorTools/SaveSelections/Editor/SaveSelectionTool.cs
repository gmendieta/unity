using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SaveSelection
{
	public string id = string.Empty;
	public string sceneId = string.Empty;
	public string activeOb = string.Empty;
	public string[] obList;
}

/// <summary>
/// Tool to save Hierarchy selections
/// </summary>
public class SaveSelectionTool : EditorWindow
{	
	private List<SaveSelection> selectionList = new List<SaveSelection>();

	private string selectionName = string.Empty;
	private Vector2 scrollPosition = Vector2.zero;


	[MenuItem ("GMM/Save Selection Tool")]
	public static void ShowWindow ()
	{
		EditorWindow w = EditorWindow.GetWindow (typeof (SaveSelectionTool));
		w.title = "Selection Tool";
	}	

	void OnGUI ()
	{
		GUILayout.Label ("Save Selection Tool: ", EditorStyles.boldLabel);        
		EditorGUILayout.Space ();

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField(new GUIContent("Name"), GUILayout.Width(50));

		Color _color = GUI.color;
		if (string.IsNullOrEmpty(selectionName)) GUI.color = Color.red;
		selectionName = EditorGUILayout.TextField(selectionName, GUILayout.ExpandWidth(true));
		GUI.color = _color;

		EditorGUILayout.EndHorizontal();
		
		if (GUILayout.Button (new GUIContent("Save Selection")))
		{
			SaveSelection();
		}
		if (GUILayout.Button (new GUIContent("Clear Selections")))
		{
			selectionList.Clear();
		}

		if (selectionList.Count == 0)
		{
			EditorGUILayout.HelpBox("No Selections saved!!", MessageType.Info);
		}
		else
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);

			for (int i=0; i<selectionList.Count; ++i)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(selectionList[i].id, GUILayout.ExpandWidth(true));
				// GMM Select a Selection
				if (GUILayout.Button(new GUIContent("S"), GUILayout.Width (30)))
			    {
					Select(selectionList[i]);
				}
				// GMM Remove Selection
				if (GUILayout.Button(new GUIContent("-"), GUILayout.Width (30)))
				{
					selectionList.Remove(selectionList[i]);
				}
				EditorGUILayout.EndHorizontal();
			}

			// GMM Two spaces in order to let some space before scroll end
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.EndScrollView();
		}
		
	}

	/// <summary>
	/// Creates and Save a selection
	/// </summary>
	protected void SaveSelection()
	{
		GameObject[] selection = Selection.gameObjects;
		if (selection.Length > 0)
		{
			SaveSelection s = new SaveSelection();
			s.id = selectionName;
			s.sceneId = Application.loadedLevelName;
			s.obList = new string[selection.Length];
			s.activeOb = GetGameObjectHierarchyPath(Selection.activeGameObject);
			for (int i=0; i<selection.Length; ++i)
			{
				s.obList[i] = GetGameObjectHierarchyPath(selection[i]);
			}
			selectionList.Add(s);
		}
	}

	/// <summary>
	/// Select the specified GameObject
	/// </summary>
	protected void Select(SaveSelection s)
	{
		Debug.Log ("[SaveSelectionTool] Select " + s.id);
		GameObject[] obs = new GameObject[s.obList.Length];
		for (int i=0; i<s.obList.Length; ++i)
		{
			Debug.Log ("Looking " + s.obList[i]);
			obs[i] = GameObject.Find(s.obList[i]);
		}
		Selection.objects = obs;
		Selection.activeGameObject = GameObject.Find(s.activeOb);
	}

	#region UTILS

	private SaveSelection GetSaveSelection(string id)
	{
		for (int i=0; i<selectionList.Count; ++i)
		{
			if (selectionList[i].id == id)
			{
				return selectionList[i];
			}
		}
		return null;
	}

	/// <summary>
	/// Gets a GameObject Hierarchy path, in the way /GameObject/GameObject/GameObject...
	/// It seems the only way to know where an object is in the hierarchy. Maybe some Hash function
	/// </summary>
	private string GetGameObjectHierarchyPath(GameObject ob)
	{
		string path = string.Format("/{0}", ob.name);
		Transform t = ob.transform;
		Transform p = ob.transform.parent;
		while (p != null)
		{
			path = string.Format("/{0}{1}", p.gameObject.name, path);
			p = p.parent;
		}
		return path;
	}
	#endregion
			
}
