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

[CustomEditor(typeof(CProjectileSpikeStickAround_Aoe))]
public class TempEditor2 : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var aoe = (CProjectileSpikeStickAround_Aoe)target;

		if (GUILayout.Button("GenerateResetData"))
		{
			aoe.GenerateResetData();
		}
	}
}
