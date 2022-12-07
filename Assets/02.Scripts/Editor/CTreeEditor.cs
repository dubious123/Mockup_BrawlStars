using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(CTree))]
public class CTreeEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var fade = (CTree)target;

		if (GUILayout.Button("FadeIn"))
		{
			fade.FadeIn();
		}

		else if (GUILayout.Button("FadeOut"))
		{
			fade.FadeOut();
		}

		else if (GUILayout.Button("Shake"))
		{
			fade.Shake();
		}
	}
}
