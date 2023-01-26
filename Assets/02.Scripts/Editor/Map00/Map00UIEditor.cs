using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(Map00UI))]
public class Map00UIEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var mapUI = (Map00UI)target;

		if (GUILayout.Button("PlayWelcomeAnim"))
		{
			mapUI.PlayWelcomeAnim();
		}
		if (GUILayout.Button("Reset"))
		{
			mapUI.Reset();
		}
	}
}
