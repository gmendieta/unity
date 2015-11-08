
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using com.GMM.AnimatorTool;

public class AnimationTools : EditorWindow
{
	private GameObject gameObject = null;
	private Animator animator = null;

	private string[] tabsStrArray = new string[] {"Fix", "Motion Trail"};
	private int selectedTabIndex = 0;
	
	private AnimatorToolsState fixState = null;
	//private AnimatorToolsState copyPasteState = null;
	private AnimatorToolsState motionTrailState = null;

	// GMM Copy / Paste
	private AnimationClipCurveData animationCurveClipboard;
	

	[MenuItem ("GMM/Animation/Animation Tools")]
	public static void ShowWindow ()
	{
		EditorWindow w = EditorWindow.GetWindow (typeof (AnimationTools));
		w.title = "Animation Tools";
	}

	/// <summary>
	/// Destroy
	/// </summary>
	void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		fixState = null;
		//copyPasteState = null;
		motionTrailState = null;
	}

	/// <summary>
	/// Window has been selected
	/// </summary>
	void OnFocus()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	/// <summary>
	/// Inspector Draw
	/// </summary>
	void OnGUI ()
	{
		selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, tabsStrArray);
		switch (selectedTabIndex)
		{
		case 0: // Fix
			if (fixState == null) fixState = new AnimatorToolFixState();
			fixState.DisplayInfo();
			break;
//		case 1: // Copy/Paste
//			if (animatorToolCopyPasteState == null) animatorToolCopyPasteState = new AnimatorToolCopyPasteState();
//			animatorToolCopyPasteState.DisplayInfo();
//			break;
		case 1:
			if (motionTrailState == null) motionTrailState = new AnimatorToolsMotionTrailState();
			motionTrailState.DisplayInfo();
			break;
		}
	}

	/// <summary>
	/// Scene Draw
	/// </summary>
	void OnSceneGUI(SceneView sceneView)
	{
		switch (selectedTabIndex)
		{
		case 0: // Fix
			if (fixState == null) return;
			fixState.DisplaySceneInfo();
			break;
//		case 1: // Copy/Paste
//			if (animatorToolCopyPasteState == null) animatorToolCopyPasteState = new AnimatorToolCopyPasteState();
//			animatorToolCopyPasteState.DisplayInfo();
//			break;
		case 1:
			if (motionTrailState == null) return;
			motionTrailState.DisplaySceneInfo();
			break;
		}
	}
}
