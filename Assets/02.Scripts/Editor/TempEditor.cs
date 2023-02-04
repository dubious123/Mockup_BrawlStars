using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(Temp))]
public class TempEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var effect = (Temp)target;

		if (GUILayout.Button("PlayEffect"))
		{
			effect.Run();
		}


		if (GUILayout.Button("PlayEffect2"))
		{
			effect.Run2();
		}
	}
}