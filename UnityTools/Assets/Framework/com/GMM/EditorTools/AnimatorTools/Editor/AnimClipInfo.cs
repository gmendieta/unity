
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace com.GMM.AnimatorTool
{
	/// <summary>
	/// Anim Info
	/// Store the reference of an Scene object and a list of AnimClipInfo
	/// </summary>
	public class AnimInfo
	{
		public GameObject obReference = null;
		public List<AnimClipInfo> animClipInfoList = null;

		public AnimInfo(GameObject obReference)
		{
			this.obReference = obReference;
		}
	}

	public class AnimClipInfo
	{
		public AnimationClip animClip;
		public List<AnimClipPathInfo> animClipPathInfoList = null;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public AnimClipInfo(AnimationClip clip)
		{
			this.animClip = clip;
			animClipPathInfoList = new List<AnimClipPathInfo>();
		}
		
		/// <summary>
		/// Gets AnimClipPathInfo with path if exists, NULL otherwise
		/// </summary>
		public AnimClipPathInfo GetAnimClipPathInfo(string path)
		{
			for (int i=0; i<animClipPathInfoList.Count; ++i)
			{
				if (animClipPathInfoList[i].animClipPath.Equals(path))
				{
					return animClipPathInfoList[i];
				}
			}
			return null;
		}
	}

	public class AnimClipPathInfo
	{
		private AnimationClip animationClip = null;
		public string animClipPath = string.Empty;
		
		public string newAnimClipPath = string.Empty;
		
		public bool isExpanded = false;
		public bool isEditing = false;
		
		public List<AnimClipComponentInfo> animClipComponentInfoList = null;
		
		public AnimClipPathInfo(AnimationClip clip, string obPath)
		{
			this.animationClip = clip;
			this.animClipPath = obPath;
			animClipComponentInfoList = new List<AnimClipComponentInfo>();
		}

		/// <summary>
		/// Gets AnimClipTypeInfo if exits, Null otherwise
		/// </summary>
		public AnimClipComponentInfo GetAnimClipComponentInfo(System.Type type)
		{
			for (int i=0; i<animClipComponentInfoList.Count; ++i)
			{
				if (animClipComponentInfoList[i].type.Equals(type) == true)
				{
					return animClipComponentInfoList[i];
				}
			}
			return null;
		}

		/// <summary>
		/// Gets AnimClipCurveInfo with Type and Property
		/// </summary>
		public AnimClipCurveInfo GetAnimClipCurveInfo(System.Type type, string property)
		{
			for (int i=0; i<animClipComponentInfoList.Count; ++i)
			{
				if (animClipComponentInfoList[i].type.Equals(type) == true)				    
				{
					return animClipComponentInfoList[i].GetAnimClipCurveInfo(property);
				}
			}
			return null;
		}
		
		/// <summary>
		/// Replaces the Path of all AnimationClipCurveData
		/// </summary>
		public void ReplacePath(string path, string newPath)
		{
			this.animClipPath = newPath;
			for (int i=0; i<animClipComponentInfoList.Count; ++i)
			{
				for (int j=0; j<animClipComponentInfoList[i].animClipCurveInfoList.Count; ++j)
				{
					AnimationClipCurveData curveData = animClipComponentInfoList[i].animClipCurveInfoList[j].animationClipCurveData;
					if (curveData.path.Equals(path))
					{
						curveData.path = newPath;
						
						// Save Internal Info
						EditorCurveBinding curveBinding = new EditorCurveBinding();
						curveBinding.path = newPath;
						curveBinding.propertyName = curveData.propertyName;
						curveBinding.type = curveData.type;
						
						AnimationUtility.SetEditorCurve(this.animationClip, curveBinding, curveData.curve);
					}
				}
			}
		}
	}

	public class AnimClipComponentInfo
	{
		private AnimationClip animationClip = null;
		public System.Type type = null;

		public List<AnimClipCurveInfo> animClipCurveInfoList = null;

		public AnimClipComponentInfo(AnimationClip clip, System.Type type)
		{
			this.animationClip = clip;
			this.type = type;
			animClipCurveInfoList = new List<AnimClipCurveInfo>();
		}

		public AnimClipCurveInfo GetAnimClipCurveInfo(string property)
		{
			for (int i=0; i<animClipCurveInfoList.Count; ++i)
			{
				if (animClipCurveInfoList[i].animationClipCurveData.propertyName.Equals(property) == true)
				{
					return animClipCurveInfoList[i];
				}
			}
			return null;
		}
	}

	public class AnimClipCurveInfo
	{
		private AnimationClip animationClip = null;
		public AnimationClipCurveData animationClipCurveData = null;

		public AnimClipCurveInfo(AnimationClip clip, AnimationClipCurveData curveData)
		{
			this.animationClip = clip;
			this.animationClipCurveData = curveData;
		}

		/// <summary>
		/// Set the data from parameter curve to animationClipCurveData
		/// </summary>
		public void SetKeys(AnimationClipCurveData curveData)
		{
			EditorCurveBinding curveBinding = new EditorCurveBinding();

			curveBinding.path = animationClipCurveData.path;
			curveBinding.propertyName = animationClipCurveData.propertyName;
			curveBinding.type = animationClipCurveData.type;

			AnimationUtility.SetEditorCurve(this.animationClip, curveBinding, curveData.curve);
		}
	}
}