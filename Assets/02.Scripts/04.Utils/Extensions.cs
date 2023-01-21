using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public static partial class Extensions
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

	public static Transform FirstChildOrDefault(this Transform parent, Func<Transform, bool> query)
	{
		if (parent.childCount == 0)
		{
			return null;
		}

		Transform result = null;
		for (int i = 0; i < parent.childCount; i++)
		{
			var child = parent.GetChild(i);
			if (query(child))
			{
				return child;
			}
			result = FirstChildOrDefault(child, query);
		}

		return result;
	}

	public static T ChangeAlpha<T>(this T g, float newAlpha)
		where T : Graphic
	{
		var color = g.color;
		color.a = newAlpha;
		g.color = color;
		return g;
	}

	public static void SetMinX(this RectTransform rect, float x)
	{
		rect.anchorMin = new Vector2(x, rect.anchorMin.y);
	}

	public static void SetMaxX(this RectTransform rect, float x)
	{
		rect.anchorMax = new Vector2(x, rect.anchorMax.y);
	}

	public static void SetMinY(this RectTransform rect, float y)
	{
		rect.anchorMin = new Vector2(rect.anchorMin.x, y);
	}

	public static void SetMaxY(this RectTransform rect, float y)
	{
		rect.anchorMax = new Vector2(rect.anchorMax.x, y);
	}

	public static void SetAnchorX(this RectTransform rect, float x)
	{
		rect.anchorMin = new Vector2(x, rect.anchorMin.y);
		rect.anchorMax = new Vector2(x, rect.anchorMax.y);
	}

	public static void SetAnchorY(this RectTransform rect, float y)
	{
		rect.anchorMin = new Vector2(rect.anchorMin.x, y);
		rect.anchorMax = new Vector2(rect.anchorMax.x, y);
	}

	public static void SetAnchorX(this RectTransform rect, Vector2 anchorX)
	{
		rect.anchorMin = new Vector2(anchorX.x, rect.anchorMin.y);
		rect.anchorMax = new Vector2(anchorX.y, rect.anchorMax.y);
	}

	public static void SetAnchorY(this RectTransform rect, Vector2 anchorY)
	{
		rect.anchorMin = new Vector2(rect.anchorMin.x, anchorY.x);
		rect.anchorMax = new Vector2(rect.anchorMax.x, anchorY.y);
	}

	public static void SetAnchorDeltaX(this RectTransform rect, Vector2 deltaX)
	{
		rect.anchorMin = new Vector2(rect.anchorMin.x + deltaX.x, rect.anchorMin.y);
		rect.anchorMax = new Vector2(rect.anchorMax.x + deltaX.y, rect.anchorMax.y);
	}

	public static void SetAnchorDeltaY(this RectTransform rect, Vector2 deltaY)
	{
		rect.anchorMin = new Vector2(rect.anchorMin.x, rect.anchorMin.y + deltaY.x);
		rect.anchorMax = new Vector2(rect.anchorMax.x, rect.anchorMax.y + deltaY.y);
	}

	public static Dictionary<K, V> ResetValues<K, V>(this Dictionary<K, V> dic, V value = default)
	{
		dic.Keys.ToList().ForEach(x => dic[x] = value);
		return dic;
	}
}
