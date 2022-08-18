using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
	public static void MoveTo(SceneType type, object sceneParam)
	{
		SceneManager.LoadScene((int)type);
		GameObject.Find("Scene").GetComponent<BaseScene>().Init(sceneParam);
	}
}
