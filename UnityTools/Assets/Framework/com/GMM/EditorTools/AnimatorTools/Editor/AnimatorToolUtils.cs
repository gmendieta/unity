using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

namespace com.GMM.AnimatorTool
{
	public static class AnimatorToolUtils
	{
		/// <summary>
		/// Gets an array with all the AnimationClip names inside an AnimatorController
		/// </summary>
		public static string[] GetAnimationClipStrArray(AnimatorController ac)
		{
			List<string> animList = new List<string>();
			for (int i=0; i<ac.layerCount; ++i)
			{
				StateMachine sm = ac.GetLayer(i).stateMachine;
				for (int j=0; j<sm.stateCount; ++j)
				{
					State state = sm.GetState(j);
					AnimationClip clip = state.GetMotion() as AnimationClip;
					if (clip != null)
					{
						animList.Add(clip.name);
					}
				}
			}
			return animList.ToArray();
		}
		
		/// <summary>
		/// Gets an array with all the AnimationClips inside an AnimatorController
		/// </summary>
		public static AnimationClip[] GetAnimationClipArray(AnimatorController ac)
		{
			List<AnimationClip> animList = new List<AnimationClip>();
			for (int i=0; i<ac.layerCount; ++i)
			{
				StateMachine sm = ac.GetLayer(i).stateMachine;
				for (int j=0; j<sm.stateCount; ++j)
				{
					State state = sm.GetState(j);
					AnimationClip clip = state.GetMotion() as AnimationClip;
					if (clip != null)
					{
						animList.Add(clip);
					}
				}
			}
			return animList.ToArray();
		}
		
		/// <summary>
		/// Build AnimClipInfo from AnimationClip
		/// </summary>
		public static AnimClipInfo AnimationClip2AnimClipInfo(AnimationClip animationClip)
		{
			//		Stopwatch stopwatch = new Stopwatch();
			//		stopwatch.Start();
			AnimClipInfo animClipInfo = new AnimClipInfo(animationClip);
			AnimationClipCurveData[] animationClipCurveDataArray = AnimationUtility.GetAllCurves(animationClip, true);
			for (int i=0; i<animationClipCurveDataArray.Length; ++i)
			{
				AnimationClipCurveData curveData = animationClipCurveDataArray[i];
				AnimClipPathInfo animClipPathInfo = animClipInfo.GetAnimClipPathInfo(curveData.path);
				// GMM Create AnimClipPathInfo if does not exist
				if (animClipPathInfo == null)
				{
					animClipPathInfo = new AnimClipPathInfo(animationClip, curveData.path);
					animClipInfo.animClipPathInfoList.Add(animClipPathInfo);
				}

				AnimClipComponentInfo animClipTypeInfo = animClipPathInfo.GetAnimClipComponentInfo(curveData.type);
				if (animClipTypeInfo == null)
				{
					animClipTypeInfo = new AnimClipComponentInfo(animationClip, curveData.type);
					animClipPathInfo.animClipComponentInfoList.Add(animClipTypeInfo);
				}

				AnimClipCurveInfo animClipCurveInfo = animClipTypeInfo.GetAnimClipCurveInfo(curveData.propertyName);
				if (animClipCurveInfo == null)
				{
					animClipCurveInfo = new AnimClipCurveInfo(animationClip, curveData);
					animClipTypeInfo.animClipCurveInfoList.Add(animClipCurveInfo);
				}
			}			
			//		stopwatch.Stop();
			//		UnityEngine.Debug.Log (string.Format("[UpdateAnimClipInfo] TimeElapsed: {0}", stopwatch.Elapsed));
			return animClipInfo;
		}
	}
}