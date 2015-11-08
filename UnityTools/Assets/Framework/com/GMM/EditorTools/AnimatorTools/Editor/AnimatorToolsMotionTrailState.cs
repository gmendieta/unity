using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

namespace com.GMM.AnimatorTool
{
	public class AnimatorToolsMotionTrailState : AnimatorToolsState 
	{
		private bool _displayViewportConfig = false;
		private float dotSize = 1f;
		private float bezierWidth = 1f;
		private Color displayColor = Color.red;

		public Vector3 p1 = Vector3.zero;
		public Vector3 p2 = Vector3.zero;

		private Texture _texGlasses = null;
		private Texture _texGlassesNo = null;

		private List<AnimClipComponentInfo> _showingComponents = null;

		private void OnDestroy()
		{
			_texGlasses = null;
		}

		/// <summary>
		/// Sets the animation clip
		/// </summary>
		public override void SetAnimationClip (AnimationClip animationClip)
		{
			this.animClipInfo = AnimatorToolUtils.AnimationClip2AnimClipInfo(animationClip);
			if (_texGlasses == null) _texGlasses = Resources.Load("glasses") as Texture;
			if (_texGlassesNo == null) _texGlassesNo = Resources.Load("glasses_no") as Texture;
			if (_showingComponents == null) _showingComponents = new List<AnimClipComponentInfo>();
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

					// GMM Display Viewport configuration
					DisplayVieportConfig();

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

							pathInfo.isExpanded = EditorGUILayout.Foldout(pathInfo.isExpanded, new GUIContent(string.Format("Path: {0}", pathInfo.animClipPath)));

							if (pathInfo.isExpanded == true)
							{
								++EditorGUI.indentLevel;
								for (int j=0; j<pathInfo.animClipComponentInfoList.Count; ++j)
								{
									AnimClipComponentInfo componentInfo = pathInfo.animClipComponentInfoList[j];
									if (componentInfo.type.Equals(typeof(UnityEngine.Transform)))
									{									
										EditorGUILayout.BeginHorizontal();

										EditorGUILayout.LabelField(new GUIContent(componentInfo.type.ToString()));

										// GMM One Button or another if we are already showing the element or not
										if (_showingComponents.Contains(componentInfo) == true)
										{
											if (GUILayout.Button(_texGlassesNo, GUILayout.Width(30), GUILayout.Height(20)))
											{
												_showingComponents.Remove(componentInfo);
											}
										}
										else
										{
											if (GUILayout.Button(_texGlasses, GUILayout.Width(30), GUILayout.Height(20)))
											{
												_showingComponents.Add(componentInfo);
											}
										}

										EditorGUILayout.EndHorizontal();
									}
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

		/// <summary>
		/// Display Scene Spline points
		/// </summary>
		public override void DisplaySceneInfo ()
		{
			if (_showingComponents != null && _showingComponents.Count > 0)
			{
				for (int i=0; i<_showingComponents.Count; ++i)
				{
					AnimClipComponentInfo componentInfo = _showingComponents[i];
					AnimClipCurveInfo xCurve = componentInfo.GetAnimClipCurveInfo("m_LocalPosition.x");
					AnimClipCurveInfo yCurve = componentInfo.GetAnimClipCurveInfo("m_LocalPosition.y");
					AnimClipCurveInfo zCurve = componentInfo.GetAnimClipCurveInfo("m_LocalPosition.z");
					Keyframe[] xKeys = xCurve.animationClipCurveData.curve.keys; 
					Keyframe[] yKeys = yCurve.animationClipCurveData.curve.keys;
					Keyframe[] zKeys = zCurve.animationClipCurveData.curve.keys;

					Vector3 lastPoint = Vector3.zero;
					Vector3 lastTangent = Vector3.zero;
					Vector3 currentPoint = Vector3.zero;
					Vector3 currentTangent = Vector3.zero;

					for (int j=0; j<xKeys.Length; ++j)
					{
						currentPoint = new Vector3(xKeys[j].value, yKeys[j].value, zKeys[j].value);
						//currentTangent = new Vector3(xKeys[j].inTangent, yKeys[j].inTangent, zKeys[j].inTangent);

						Handles.FreeMoveHandle(currentPoint, Quaternion.identity, dotSize, Vector3.zero, Handles.CircleCap);
						if (j > 0)
						{
							//Handles.DrawBezier(lastPoint, currentPoint, lastTangent, currentTangent, displayColor, null, bezierWidth);
							Handles.DrawBezier(lastPoint, currentPoint, lastPoint, currentPoint, displayColor, null, bezierWidth);
						}

						//lastTangent = new Vector3(xKeys[j].outTangent, yKeys[j].outTangent, zKeys[j].outTangent);
						lastPoint = currentPoint;
					}
				}
			}
		}

		/// <summary>
		/// Display Editor visual configuration foldout for Scene Spline points
		/// </summary>
		private void DisplayVieportConfig()
		{
			_displayViewportConfig = EditorGUILayout.Foldout(_displayViewportConfig, new GUIContent("Visual Config"));
			if (_displayViewportConfig == true)
			{
				++EditorGUI.indentLevel;

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent("Dot Size: "));
				dotSize = EditorGUILayout.Slider(dotSize, 0.1f, 5.0f);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent("Bezier Width: "));
				bezierWidth = EditorGUILayout.Slider(bezierWidth, 0.1f, 5.0f);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent("Color: "));
				displayColor = EditorGUILayout.ColorField(displayColor);
				EditorGUILayout.EndHorizontal();

				--EditorGUI.indentLevel;
			}
		}
	}
}