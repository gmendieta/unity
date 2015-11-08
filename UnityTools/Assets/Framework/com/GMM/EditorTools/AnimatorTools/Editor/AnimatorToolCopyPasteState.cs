//using UnityEngine;
//using UnityEditor;
//using UnityEditorInternal;
//using System.Collections;
//using System.Collections.Generic;
//
//namespace com.GMM.AnimatorTool
//{ 
//	public class AnimatorToolCopyPasteState : AnimatorToolsState
//	{
//		private float currentScrollViewHeight;
//		private bool resize = false;
//		private Rect cursorChangeRect;
//
//		private string clipboardPath = string.Empty;
//		private AnimationClipCurveData clipboardCurve = null;
//
//		public AnimatorToolCopyPasteState()
//		{
//			;
//		}
//		// GMM True if Clipboard has some data, False otherwise
//		private bool IsClipboardData { get { return string.IsNullOrEmpty(clipboardPath)==false; } }
//
//		/// <summary>
//		/// Sets the animation clip
//		/// </summary>
//		public override void SetAnimationClip (AnimationClip animationClip)
//		{
//			this.animClipInfo = AnimatorToolUtils.AnimationClip2AnimClipInfo(animationClip);
//		}
//
//		public override void DisplayInfo ()
//		{
//			// GMM LockSelection
//			if (selectionLocked == false)
//			{
//				gameObject = Selection.activeGameObject;
//			}
//			
//			if (gameObject == null)
//			{
//				EditorGUILayout.HelpBox("Select a GameObject", MessageType.Warning);
//			}
//			else
//			{
//				Animator animator = gameObject.GetComponent<Animator>();
//				if (animator == null || animator.runtimeAnimatorController == null)
//				{
//					EditorGUILayout.HelpBox("Select a GameObject with an Animator", MessageType.Warning);
//				}
//				else
//				{					
//					// GMM Selection GameObject and posibility of LockSelection
//					EditorGUILayout.BeginHorizontal();
//					EditorGUILayout.LabelField(new GUIContent(string.Format("Selected GameObject: {0}", gameObject.name)), EditorStyles.boldLabel);
//					Color _backup = GUI.backgroundColor; // GMM Backup
//					if (selectionLocked == false)
//					{
//						GUI.backgroundColor = Color.green;
//						if (GUILayout.Button(new GUIContent("Lock"), EditorStyles.miniButtonRight, GUILayout.Width(75)))
//						{
//							selectionLocked = true;
//						}
//					}
//					else 
//					{
//						GUI.backgroundColor = Color.red;
//						if (GUILayout.Button(new GUIContent("Unlock"), EditorStyles.miniButtonRight, GUILayout.Width(75)))
//						{
//							selectionLocked = false;
//						}
//					}
//					
//					GUI.backgroundColor = _backup;
//					EditorGUILayout.EndHorizontal();
//					
//					AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
//					
//					string[] animClipNames = AnimatorToolUtils.GetAnimationClipStrArray(ac);
//					if (selectedAnimClipIndex < 0 || selectedAnimClipIndex > animClipNames.Length)
//					{
//						selectedAnimClipIndex = 0;
//					}
//					
//					EditorGUILayout.BeginHorizontal();
//					
//					selectedAnimClipIndex = EditorGUILayout.Popup("Animation Clips", selectedAnimClipIndex, animClipNames, GUILayout.ExpandWidth(true));
//					if (GUILayout.Button(new GUIContent("Refresh"), EditorStyles.miniButtonRight, GUILayout.Width(75)))
//					{
//						AnimationClip[] animClips = AnimatorToolUtils.GetAnimationClipArray(ac);
//						
//						SetAnimationClip(animClips[selectedAnimClipIndex]);
//						lastSelectedAnimClipIndex = selectedAnimClipIndex; // Force next IF to be false
//					}
//					
//					EditorGUILayout.EndHorizontal();
//					EditorGUILayout.Separator();
//					
//					// GMM Update AnimClipInfo and lastSelectedAnimClipIndex if necessary
//					if (selectedAnimClipIndex != lastSelectedAnimClipIndex)
//					{
//						AnimationClip[] animClips = AnimatorToolUtils.GetAnimationClipArray(ac);
//						SetAnimationClip(animClips[selectedAnimClipIndex]);
//						lastSelectedAnimClipIndex = selectedAnimClipIndex;
//					}
//
//					// GMM Show Clipboard Path if exists
//					if (string.IsNullOrEmpty(clipboardPath) == false)
//					{
//						EditorGUILayout.BeginHorizontal();
//						
//						GUILayout.Box(new GUIContent(clipboardPath), EditorStyles.whiteLargeLabel, GUILayout.ExpandWidth(true));
//						if (GUILayout.Button(new GUIContent("Clear"), GUILayout.Width(50)))
//						{
//							clipboardPath = string.Empty;
//							clipboardCurve = null;
//						}
//						
//						EditorGUILayout.EndHorizontal();
//						
//						EditorGUILayout.Separator();
//					}
//					
//					// GMM Show Animation Info
//					if (this.animClipInfo != null)
//					{
//						scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);
//						
//						++EditorGUI.indentLevel;
//						
//						// GMM Traverse all Object that belongs to this AnimationClip
//						for (int i=0; i<this.animClipInfo.animClipPathInfoList.Count; ++i)
//						{
//							AnimClipPathInfo pathInfo = this.animClipInfo.animClipPathInfoList[i];
//							
//							// GMM Show Object Path inside Animation
//							pathInfo.isExpanded = EditorGUILayout.Foldout(pathInfo.isExpanded, new GUIContent(string.Format("Path: {0}", pathInfo.animClipPath)));
//							//EditorGUILayout.LabelField(new GUIContent(string.Format("Path: {0}", pathInfo.animClipPath)), EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
//
//							if (pathInfo.isExpanded == true)
//							{
//								++EditorGUI.indentLevel;
//								// GMM Traverse all Curves of an specific Object
//								for (int j=0; j<pathInfo.animClipCurveInfoList.Count; ++j)
//								{
//									EditorGUILayout.BeginHorizontal();
//									
//									AnimClipCurveInfo curveInfo = pathInfo.animClipCurveInfoList[j];
//									string curveHash = string.Format("{0}.{1}", curveInfo.animationClipCurveData.type, curveInfo.animationClipCurveData.propertyName);
//									EditorGUILayout.LabelField(new GUIContent(curveHash));
//									if (IsClipboardData == true)
//									{
//										if (GUILayout.Button(new GUIContent("Paste"), EditorStyles.miniButtonRight, GUILayout.Width(50)))
//										{
//											curveInfo.SetKeys(clipboardCurve);
//										}
//									}
//									if (GUILayout.Button(new GUIContent("Copy"), EditorStyles.miniButtonRight, GUILayout.Width(50)))
//									{
//										clipboardPath = string.Format("{0}./{1}./{2}", this.animClipInfo.animClip.name, pathInfo.animClipPath, curveHash);
//										clipboardCurve = curveInfo.animationClipCurveData;
//									}
//									
//									EditorGUILayout.EndHorizontal();
//								}
//								--EditorGUI.indentLevel;
//							}
//						}
//						--EditorGUI.indentLevel;
//						
//						EditorGUILayout.EndScrollView();
//					} // if (this.animClipInfo != null)
//
//
//				}
//			}
//		}
//	
//		public override void DisplaySceneInfo ()
//		{
//			;
//		}
//
//	}
//}