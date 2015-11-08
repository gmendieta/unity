using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;


public class AnimationCopier: EditorWindow
{
	//[MenuItem ("GMM/Animation Copier")]
	public static void ShowWindow ()
	{
		EditorWindow.GetWindow(typeof(AnimationCopier));
	}
	private AnimationClip selectedAnimationClip;
	private CurveInformation curveInformation;
	private Vector2 scrollViewVector;
	private static List<AnimationClipCurveData> animationCurveClipboard = new List<AnimationClipCurveData>();
	public void OnGUI()
	{
		EditorGUILayout.LabelField("");
		var animationClips = Resources.FindObjectsOfTypeAll<AnimationClip>().ToList();
		EditorGUILayout.BeginHorizontal();
		int selectedAnimationClipIndex = EditorGUILayout.Popup("Animation Clips",animationClips.IndexOf(selectedAnimationClip), animationClips.Select(x => x.name).ToArray() );
		if (selectedAnimationClipIndex < 0)
		{
			selectedAnimationClipIndex = 0;
		}
		if(selectedAnimationClip != animationClips[selectedAnimationClipIndex] || curveInformation == null)
		{
			curveInformation = new CurveInformation(animationClips[selectedAnimationClipIndex].name);
		}
		selectedAnimationClip = animationClips[selectedAnimationClipIndex];
		
		if (GUILayout.Button("Copy", EditorStyles.miniButton))
		{
			animationCurveClipboard = curveInformation.GetSelectedAnimationCurves();
		}
		if (GUILayout.Button("Copy All", EditorStyles.miniButton))
		{
			animationCurveClipboard = AnimationUtility.GetAllCurves(selectedAnimationClip, true).ToList();
		}
		if (GUILayout.Button("Paste", EditorStyles.miniButton))
		{
			Paste();
		}
		if (GUILayout.Button("Remove", EditorStyles.miniButton))
		{
			var curvesToDelete =  curveInformation.GetSelectedAnimationCurves();
			var allCurves = curveInformation.GetSelectedAnimationCurves(new List<AnimationClipCurveData>(), true);
			selectedAnimationClip.ClearCurves();
			foreach (var curveInfo in allCurves)
			{
				if (curveInfo == null)
				{
					continue;
				}
				if (!curvesToDelete.Contains(curveInfo))
				{
					InsertCurve(curveInfo);
				}
			}
			Refresh();
			
		}
		if (GUILayout.Button("Refresh", EditorStyles.miniButton))
		{
			Refresh();
		}
		EditorGUILayout.EndHorizontal();
		foreach (AnimationClipCurveData curveData in AnimationUtility.GetAllCurves(selectedAnimationClip, true))
		{
			UpdateCurveInformation(selectedAnimationClip.name , curveInformation, curveData);
			
		}
		
		scrollViewVector = EditorGUILayout.BeginScrollView(scrollViewVector);
		curveInformation.DisplayCurveInformation();
		EditorGUILayout.EndScrollView();
	}
	
	private void Refresh()
	{
		
		curveInformation = new CurveInformation(selectedAnimationClip.name);
	}
	
	
	private void Paste()
	{    
		foreach (AnimationClipCurveData animationClipCurveData in animationCurveClipboard)
		{
			if (animationClipCurveData == null)
			{
				continue;
			}
			InsertCurve(animationClipCurveData);
		}
	}
	
	private void InsertCurve(AnimationClipCurveData animationClipCurveData)
	{
		EditorCurveBinding editorCurveBinding = new EditorCurveBinding();
		editorCurveBinding.path = animationClipCurveData.path;
		editorCurveBinding.propertyName = animationClipCurveData.propertyName;
		editorCurveBinding.type = animationClipCurveData.type;
		
		AnimationUtility.SetEditorCurve(selectedAnimationClip, editorCurveBinding, animationClipCurveData.curve);
	}
	
	private void UpdateCurveInformation(string nameOfClip, CurveInformation curveInformationToUpdate, AnimationClipCurveData animationCruveData)
	{
		
		List<string> curveInformationNames = animationCruveData.path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
		
		curveInformationNames.Insert(0, nameOfClip);
		curveInformationNames.Add(animationCruveData.type.ToString());
		curveInformationNames.Add(animationCruveData.propertyName);
		
		curveInformationToUpdate.AddIfNonExistant(curveInformationNames, animationCruveData);
	}
	private class CurveInformation
	{
		public bool IsChecked { get; set; }
		public AnimationClipCurveData AnimationClipCurveData { get; set; }
		
		public string Name{get; private set;}
		
		
		public List<CurveInformation> Children { get; private set; }
		public CurveInformation(string name)
		{
			Name = name;
			Children = new List<CurveInformation>();
		}
		
		public void DisplayCurveInformation()
		{
			IsChecked = EditorGUILayout.ToggleLeft(Name, IsChecked);
			
			EditorGUI.indentLevel++;
			foreach (var child in Children)
			{
				child.DisplayCurveInformation();
			}
			EditorGUI.indentLevel--;
		}
		public List<AnimationClipCurveData> GetSelectedAnimationCurves(List<AnimationClipCurveData> animationCurves = null, bool overwriteChecked = false)
		{
			if (animationCurves == null)
			{
				animationCurves = new List<AnimationClipCurveData>();
			}
			if (this.IsChecked || overwriteChecked)
			{
				animationCurves.Add(this.AnimationClipCurveData);
				foreach (var child in Children)
				{
					animationCurves =child.GetSelectedAnimationCurves(animationCurves, true);
				}
			}
			else
			{
				foreach (var child in Children)
				{
					animationCurves = child.GetSelectedAnimationCurves(animationCurves, false);
				}
			}
			return animationCurves;
		}
		
		public CurveInformation AddIfNonExistant(List<string> path, AnimationClipCurveData animationCLipCurveData)
		{
			
			if(Name.Equals(path[0]))
			{
				if (path.Count == 1)
				{
					AnimationClipCurveData = animationCLipCurveData;
					return this;
				}
				var pathReduced = path;
				pathReduced.RemoveAt(0);
				foreach (CurveInformation curveInformation in Children)
				{
					if (curveInformation.Name.Equals(pathReduced[0]))
					{
						CurveInformation childResult = curveInformation.AddIfNonExistant(pathReduced, animationCLipCurveData);
						if (childResult != null)
						{
							return childResult;
						}
					}
				}
			}
			CurveInformation newChild = new CurveInformation(path[0]);
			Children.Add(newChild);
			if (path.Count == 1)
			{
				newChild.AnimationClipCurveData = animationCLipCurveData;
				return newChild;
			}
			else
			{
				var pathReduced = path;
				pathReduced.RemoveAt(0);
				return newChild.AddIfNonExistant(pathReduced, animationCLipCurveData);
			}
		}
	}
}