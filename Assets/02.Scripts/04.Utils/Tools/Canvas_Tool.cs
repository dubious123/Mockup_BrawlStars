using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Canvas_Tool
{
	public static Vector2 WorldtoCanvasRectPos(Camera cam, Vector2 canvasSizeDelta, Vector3 worldPos)
	{
		Vector2 viewPos = cam.WorldToViewportPoint(worldPos);
		return new Vector2(
			((viewPos.x * canvasSizeDelta.x) - (canvasSizeDelta.x * 0.5f)),
			((viewPos.y * canvasSizeDelta.y) - (canvasSizeDelta.y * 0.5f)));
	}
}
