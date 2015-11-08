using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

namespace com.GMM.AnimatorTool
{
	public class AnimatorToolFixState : AnimatorToolsState 
	{
		/// <summary>
		/// Sets the animation clip
		/// </summary>
		public override void SetAnimationClip (AnimationClip animationClip)
		{
			this.animClipInfo = AnimatorToolUtils.AnimationClip2AnimClipInfo(animationClip);
		}

		/// <summary>
		/// Display Info
		/// </summary>
		public override void DisplayInfo ()
		{
			// GMM LockSelection
			if (selectionLocked == false)
			{
				gameObject = Selection.activeGameObject;
			}
			
			if (gameObject == null)
			{
				EditorGUILayout.HelpBox("Select a GameObject", MessageType.Warning);
			}
			else
			{
				Animator animator = gameObject.GetComponent<Animator>();
				if (animator == null || animator.runtimeAnimatorController == null)
				{
					EditorGUILayout.HelpBox("Select a GameObject with an Animator", MessageType.Warning);
				}
				else
				{
										
					// GMM Selection GameObject and posibility of LockSelection
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(string.Format("Selected GameObject: {0}", gameObject.name)), EditorStyles.boldLabel);
					Color _backup = GUI.backgroundColor; // GMM Backup
					if (selectionLocked == false)
					{
						GUI.backgroundColor = Color.green;
						if (GUILayout.Button(new GUIContent("Lock"), EditorStyles.miniButtonRight, GUILayout.Width(75)))
						{
							selectionLocked = true;
						}
					}
					else 
					{
						GUI.backgroundColor = Color.red;
						if (GUILayout.Button(new GUIContent("Unlock"), EditorStyles.miniButtonRight, GUILayout.Width(75)))
						{
							selectionLocked = false;
						}
					}

					GUI.backgroundColor = _backup;
					EditorGUILayout.EndHorizontal();
					
					AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
										
					string[] animClipNames = AnimatorToolUtils.GetAnimationClipStrArray(ac);
					if (selectedAnimClipIndex < 0 || selectedAnimClipIndex > animClipNames.Length)
					{
						selectedAnimClipIndex = 0;
					}
					
					EditorGUILayout.BeginHorizontal();

					selectedAnimClipIndex = EditorGUILayout.Popup("Animation Clips", selectedAnimClipIndex, animClipNames, GUILayout.ExpandWidth(true));
					if (GUILayout.Button(new GUIContent("Refresh"), EditorStyles.miniButtonRight, GUILayout.Width(75)))
					{
						AnimationClip[] animClips = AnimatorToolUtils.GetAnimationClipArray(ac);
						
						SetAnimationClip(animClips[selectedAnimClipIndex]);
						lastSelectedAnimClipIndex = selectedAnimClipIndex; // Force next IF to be false
					}
					
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Separator();

					// GMM Update AnimClipInfo and lastSelectedAnimClipIndex if necessary
					if (selectedAnimClipIndex != lastSelectedAnimClipIndex)
					{
						AnimationClip[] animClips = AnimatorToolUtils.GetAnimationClipArray(ac);
						SetAnimationClip(animClips[selectedAnimClipIndex]);
						lastSelectedAnimClipIndex = selectedAnimClipIndex;
					}

					if (this.animClipInfo != null)
					{
						scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);
						
						++EditorGUI.indentLevel;
						for (int i=0; i<this.animClipInfo.animClipPathInfoList.Count; ++i)
						{
							AnimClipPathInfo pathInfo = this.animClipInfo.animClipPathInfoList[i];
							
							EditorGUILayout.BeginHorizontal();
							
							if (pathInfo.isEditing == false)
							{
								pathInfo.isExpanded = EditorGUILayout.Foldout(pathInfo.isExpanded, new GUIContent(string.Format("Path: {0}", pathInfo.animClipPath)));
								//EditorGUILayout.LabelField(new GUIContent(string.Format("Path: {0}", this.animClipPath)), EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
								if (GUILayout.Button(new GUIContent("Edit"), EditorStyles.miniButtonRight, GUILayout.Width(60)))
								{
									pathInfo.newAnimClipPath = pathInfo.animClipPath;
									pathInfo.isEditing = true;
								}
							}
							else // Editing == true
							{
								_backup = GUI.backgroundColor;
								GUI.backgroundColor = Color.yellow;
								
								pathInfo.newAnimClipPath = EditorGUILayout.TextField(pathInfo.newAnimClipPath, GUILayout.ExpandWidth(true));
								if (GUILayout.Button(new GUIContent("Save"), EditorStyles.miniButtonRight, GUILayout.Width(60)))
								{
									pathInfo.ReplacePath(pathInfo.animClipPath, pathInfo.newAnimClipPath);
									pathInfo.isEditing = false;
								}
								if (GUILayout.Button(new GUIContent("Cancel"), EditorStyles.miniButtonRight, GUILayout.Width(60)))
								{
									pathInfo.isEditing = false;
								}
								
								GUI.backgroundColor = _backup;
							}
							
							EditorGUILayout.EndHorizontal();
							
							if (pathInfo.isExpanded == true)
							{
								++EditorGUI.indentLevel;
								for (int j=0; j<pathInfo.animClipComponentInfoList.Count; ++j)
								{
									AnimClipComponentInfo componentInfo = pathInfo.animClipComponentInfoList[j];
									EditorGUILayout.LabelField(new GUIContent(componentInfo.type.ToString()));
									++EditorGUI.indentLevel;
									for (int k=0; k<componentInfo.animClipCurveInfoList.Count; ++k)
									{
										EditorGUILayout.LabelField(new GUIContent(componentInfo.animClipCurveInfoList[k].animationClipCurveData.propertyName));
									}
									--EditorGUI.indentLevel;
								}
								--EditorGUI.indentLevel;
							}
						}
						--EditorGUI.indentLevel;
						
						EditorGUILayout.EndScrollView();
					} // if (this.animClipInfo != null)
				}
			}
		}

		public override void DisplaySceneInfo ()
		{
			;
		}
	}
}