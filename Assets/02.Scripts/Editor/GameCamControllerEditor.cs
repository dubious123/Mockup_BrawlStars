using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(GameCameraController))]
public class GameCamControllerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var cam = (GameCameraController)target;
		if (GUILayout.Button("Shakes"))
		{
			cam.Shake();
		}
	}
}
