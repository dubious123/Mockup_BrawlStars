using UnityEngine;
using UnityEngine.SceneManagement;

using static Enums;

public class Scene : MonoBehaviour
{
	public static BaseScene CurrentScene => _instance._currentScene;

	private static Scene _instance;
	private BaseScene _currentScene;

	private void Awake()
	{
		_instance = this;
	}

	public static AsyncOperation MoveTo(SceneType type, object sceneParam, LoadSceneMode mode = LoadSceneMode.Single)
	{
		var handle = SceneManager.LoadSceneAsync((int)type, mode);
		handle.completed += _ =>
		{
			_instance._currentScene = GameObject.Find($"Scene{type}").GetComponent<BaseScene>();
			_instance._currentScene.Init(sceneParam);
		};

		return handle;
	}
}
