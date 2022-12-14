using MEC;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.SceneManagement;

using static Enums;

public class Scene : MonoBehaviour
{
	public static BaseScene CurrentScene => _instance._currentScene;

	private static Scene _instance;
	private BaseScene _currentScene;
	private UIChangeSceneAnim _changeSceneAnim;

	private void Awake()
	{
		_instance = this;
	}

	public static void Init()
	{
		_instance._changeSceneAnim = _instance.GetComponentInChildren<UIChangeSceneAnim>();
	}

	public static AsyncOperation MoveTo(SceneType type, object sceneParam, LoadSceneMode mode = LoadSceneMode.Single)
	{
		//var scene = SceneManager.GetSceneByBuildIndex((int)type);
		//if (scene.isLoaded)
		//{
		//	var asyncHandle = SceneManager.UnloadSceneAsync(scene);
		//}
		var handle = SceneManager.LoadSceneAsync((int)type, mode);
		handle.completed += _ =>
		{
			_instance._currentScene = GameObject.Find($"Scene{type}").GetComponent<BaseScene>();
			_instance._currentScene.Init(sceneParam);
		};

		return handle;
	}

	public static void PlaySceneChangeAnim()
	{
		_instance._changeSceneAnim.FadeOut();
	}
}
