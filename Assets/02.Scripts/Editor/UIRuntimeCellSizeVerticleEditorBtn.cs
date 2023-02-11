using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(UIRuntimeCellSizeVerticle))]
public class UIRuntimeCellSizeVerticleEditorBtn : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var ui = (UIRuntimeCellSizeVerticle)target;
		if (GUILayout.Button("ArrangeCell"))
		{
			ui.Init();
		}
	}
}
