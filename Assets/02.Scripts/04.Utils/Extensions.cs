using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class Extensions
{
	public static AnimationClip GetAnimationClipOrNull(this RuntimeAnimatorController anim, string name)
	{
		return Animator_Tool.GetAnimationClipOrNull(anim, name);
	}

	public static Vector2 WorldtoCanvasRectPos(this Camera cam, Vector2 canvasSizeDelta, Vector3 worldPos)
	{
		return Canvas_Tool.WorldtoCanvasRectPos(cam, canvasSizeDelta, worldPos);
	}

	/// <summary>
	/// set child and return child
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="child"></param>
	/// <returns></returns>
	public static GameObject SetChild(this GameObject parent, GameObject child)
	{
		child.transform.SetParent(parent.transform);
		return child;
	}
}
