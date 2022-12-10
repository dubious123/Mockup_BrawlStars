using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class Audio : MonoBehaviour
{
	private static Audio _instance;

	public static void Init()
	{
		_instance = GameObject.Find("@Audio").GetComponent<Audio>();
		_instance.AddComponent<AudioListener>();
	}
}
