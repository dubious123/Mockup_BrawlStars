using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;

using OpenCover.Framework.Model;

using Palmmedia.ReportGenerator.Core;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(ProfileHolder))]
public class UIProfileHolderEditorBtn : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var holder = (ProfileHolder)target;
		if (GUILayout.Button("PlayOnDead"))
		{
			holder.OnDead();
		}
		if (GUILayout.Button("Reset"))
		{
			holder.Reset();
		}
	}
}
