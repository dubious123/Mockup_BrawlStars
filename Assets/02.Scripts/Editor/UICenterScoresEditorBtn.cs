using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(CenterScores))]
public class UICenterScoresEditorBtn : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var scores = (CenterScores)target;
		if (GUILayout.Button("PlayerWin"))
		{
			scores.OnPlayerWin();
		}

		if (GUILayout.Button("PlayerLose"))
		{
			scores.OnPlayerLose();

		}

		if (GUILayout.Button("Reset"))
		{
			scores.Reset();
		}

		if (GUILayout.Button("OnNextRound"))
		{
			scores.OnRoundStart();
		}
	}
}
