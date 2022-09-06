using UnityEngine;
using static Enums;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
	public static void MoveTo(SceneType type, object sceneParam)
	{
		var handle = SceneManager.LoadSceneAsync((int)type);
		handle.completed += _ => GameObject.Find($"Scene{type}").GetComponent<BaseScene>().Init(sceneParam);
	}
}
