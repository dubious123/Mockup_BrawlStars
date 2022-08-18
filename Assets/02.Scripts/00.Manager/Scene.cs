using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using UnityEngine.SceneManagement;
using MEC;

public class Scene : MonoBehaviour
{
	public static void MoveTo(SceneType type, object sceneParam)
	{
		var handle = SceneManager.LoadSceneAsync((int)type);
		handle.completed += _ => GameObject.Find($"Scene{type}").GetComponent<BaseScene>().A_Init(sceneParam);
	}
}
