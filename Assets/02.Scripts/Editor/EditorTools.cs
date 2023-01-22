#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class EditorTools
{
	#region Always Start From Scene 0
	private const string playFromFirstMenuStr = "Edit/Always Start From Scene 0 &p";

	private static bool playFromFirstScene
	{
		get { return EditorPrefs.HasKey(playFromFirstMenuStr) && EditorPrefs.GetBool(playFromFirstMenuStr); }
		set { EditorPrefs.SetBool(playFromFirstMenuStr, value); }
	}

	[MenuItem(playFromFirstMenuStr, false, 150)]
	private static void PlayFromFirstSceneCheckMenu()
	{
		playFromFirstScene = !playFromFirstScene;
		Menu.SetChecked(playFromFirstMenuStr, playFromFirstScene);

		ShowNotifyOrLog(playFromFirstScene ? "Play from scene 0" : "Play from current scene");
	}

	// The menu won't be gray out, we use this validate method for update check state
	[MenuItem(playFromFirstMenuStr, true)]
	private static bool PlayFromFirstSceneCheckMenuValidate()
	{
		Menu.SetChecked(playFromFirstMenuStr, playFromFirstScene);
		return true;
	}

	// This method is called before any Awake. It's the perfect callback for this feature
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void LoadFirstSceneAtGameBegins()
	{
		if (!playFromFirstScene)
			return;

		if (EditorBuildSettings.scenes.Length == 0)
		{
			Debug.LogWarning("The scene build list is empty. Can't play from first scene.");
			return;
		}

		foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
			go.SetActive(false);

		SceneManager.LoadScene((int)Enums.SceneType.Entry);
	}

	private static void ShowNotifyOrLog(string msg)
	{
		if (Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
			EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent(msg));
		else
			Debug.Log(msg); // When there's no scene view opened, we just print a log
	}
	#endregion

	#region Build and Run Multiplayer
	[MenuItem("Tools/Run Multiplayer/1 Players")]
	private static void PerformWin62Build1()
	{
		PerformWin64Build(1);
	}
	[MenuItem("Tools/Run Multiplayer/2 Players")]
	private static void PerformWin62Build2()
	{
		PerformWin64Build(2);
	}

	[MenuItem("Tools/Run Multiplayer/3 Players")]
	private static void PerformWin62Build3()
	{
		PerformWin64Build(3);
	}

	[MenuItem("Tools/Run Multiplayer/4 Players")]
	private static void PerformWin62Build4()
	{
		PerformWin64Build(4);
	}

	[MenuItem("Tools/Run Multiplayer/5 Players")]
	private static void PerformWin62Build5()
	{
		PerformWin64Build(5);
	}

	[MenuItem("Tools/Run Multiplayer/6 Players")]
	private static void PerformWin62Build6()
	{
		PerformWin64Build(6);
	}

	[MenuItem("Tools/Build Multiplayer/3 Players")]
	private static void BuildWin62Build3()
	{
		for (int i = 2; i < 6; i++)
		{
			BuildWin62Build(i);
		}
	}


	private static void PerformWin64Build(int playerCount)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
		for (int i = 0; i < playerCount; i++)
		{
			BuildPipeline.BuildPlayer(GetScenePaths(),
				"Builds/Win64" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
				BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
		}
	}

	private static void BuildWin62Build(int index)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
		BuildPipeline.BuildPlayer(GetScenePaths(),
			"Builds/Win64" + GetProjectName() + index.ToString() + "/" + GetProjectName() + index.ToString() + ".exe",
			BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
	}

	private static string GetProjectName()
	{
		string[] s = Application.dataPath.Split('/');
		return s[s.Length - 2];
	}

	private static string[] GetScenePaths()
	{
		var scenes = new string[EditorBuildSettings.scenes.Length];
		for (int i = 0; i < scenes.Length; i++)
		{
			scenes[i] = EditorBuildSettings.scenes[i].path;
		}
		return scenes;
	}

	#endregion
}
#endif

