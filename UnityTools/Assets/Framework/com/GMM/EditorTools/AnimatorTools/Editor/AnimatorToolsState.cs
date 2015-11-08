using UnityEngine;
using System.Collections;

namespace com.GMM.AnimatorTool
{
	public abstract class AnimatorToolsState
	{
		protected GameObject gameObject = null;
		protected AnimClipInfo animClipInfo = null;
		// GMM Scroll Position
		protected Vector2 scrollPosition = Vector2.zero;
		// GMM Lock Selection
		protected bool selectionLocked = false;
		// GMM Selection of AnimationClip Popup
		protected int selectedAnimClipIndex = 0;
		protected int lastSelectedAnimClipIndex = int.MinValue;

		public abstract void SetAnimationClip(AnimationClip animationClip);		
		/// <summary>
		/// Display Info and modify AnimClipInfo
		/// </summary>
		public abstract void DisplayInfo();
		/// <summary>
		/// Display the scene info.
		/// </summary>
		public abstract void DisplaySceneInfo();
	}
}