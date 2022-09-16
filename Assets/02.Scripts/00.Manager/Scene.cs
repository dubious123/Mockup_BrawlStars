using UnityEngine;
using static Enums;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
	public static BaseScene CurrentScene => _instance._currentScene;
	static Scene _instance;
	BaseScene _currentScene;
	void Awake()
	{
		_instance = this;
	}
	public static void MoveTo(SceneType type, object sceneParam)
	{
		var handle = SceneManager.LoadSceneAsync((int)type);
		handle.completed += _ =>
		{
			_instance._currentScene = GameObject.Find($"Scene{type}").GetComponent<BaseScene>();
			_instance._currentScene.Init(sceneParam);
		};
	}
}
