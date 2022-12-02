using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(HudHealthTextEffect))]
public class HudHealthTextEffectEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var effect = (HudHealthTextEffect)target;

		if (GUILayout.Button("PlayEffect"))
		{
			effect.PlayEffect();
		}
	}
}
