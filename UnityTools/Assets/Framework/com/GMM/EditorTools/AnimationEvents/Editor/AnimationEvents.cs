using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace com.GMM.AnimationEvents
{
	public class AnimationEvents : EditorWindow 
	{

		private enum AnimationEventsSource
		{
			FROM_SCENE = 0,
			FROM_ASSETDATABASE = 1
		}

		[MenuItem ("GMM/Animation/Animation Events")]
		public static void ShowWindow ()
		{
			EditorWindow w = EditorWindow.GetWindow (typeof (AnimationEvents));
			w.title = "Animation Events";
		}

		private AnimationEventsSource animationEventSource;

		private string[] _assetGuids;
		private AnimationClip[] _animationClips;

		private bool[] _foldouts;
		private bool _updateFoldout = false;

		private Vector2 _scrollPosition = Vector2.zero;

		/// <summary>
		/// Destroy
		/// </summary>
		void OnDestroy()
		{

		}
		
		/// <summary>
		/// Window has been selected
		/// </summary>
		void OnFocus()
		{

		}

		
		/// <summary>
		/// Inspector Draw
		/// </summary>
		void OnGUI ()
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("From Scene"), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
			{
				animationEventSource = AnimationEventsSource.FROM_SCENE;
				_updateFoldout = true;
			}
			if (GUILayout.Button(new GUIContent("From AssetDatabase"), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
			{
				animationEventSource = AnimationEventsSource.FROM_ASSETDATABASE;
				_assetGuids = GetAssetGuidsFromAssetDatabase("AnimationClip");
				_updateFoldout = true;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator();

			if (_updateFoldout == true)
			{
				_foldouts = null;
				if (_assetGuids != null && _assetGuids.Length > 0)
				{
					_foldouts = new bool[_assetGuids.Length];
				}
				_updateFoldout = false;
			}

			if (_assetGuids != null)
			{
				_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);

				string assetPath = string.Empty;
				for(int i=0; i<_assetGuids.Length; ++i)
				{
					assetPath = AssetDatabase.GUIDToAssetPath(_assetGuids[i]);
					AnimationClip clip = AssetDatabase.LoadAssetAtPath(assetPath, typeof(AnimationClip)) as AnimationClip;
					AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);

					if (events == null || events.Length == 0)
					{
						_foldouts[i] = false;
					}

					EditorGUILayout.BeginHorizontal();
					_foldouts[i] = EditorGUILayout.Foldout(_foldouts[i], new GUIContent(string.Format("{0}({1})", assetPath, events.Length)));
					if (GUILayout.Button(new GUIContent("S"), GUILayout.Width(30)))
					{
						EditorGUIUtility.PingObject(clip);
					}
					EditorGUILayout.EndHorizontal();

					if (_foldouts[i] == true)
					{
						if (events != null && events.Length > 0)
						{
							++EditorGUI.indentLevel;
							for(int j=0; j<events.Length; ++j)
							{
								EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField(new GUIContent(string.Format("T: {0:F2} - F: {1}", events[j].time, events[j].functionName)));
								EditorGUILayout.EndHorizontal();
							}
							--EditorGUI.indentLevel;
						}
					}
					clip = null;
				}

				EditorGUILayout.EndScrollView();
			}
		}

		/// <summary>
		/// Gets the animation clip from asset database
		/// </summary>
		private string[] GetAssetGuidsFromAssetDatabase(string type)
		{
			return AssetDatabase.FindAssets(string.Format("t:{0}", type));
		}
	}
}