using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Enums;

public class Scene_Entry : BaseScene
{
	public override Task A_Init(object param)
	{
		return null;
	}
	private void Awake()
	{
		DontDestroyOnLoad(new GameObject("@Timing", typeof(Timing)));
		DontDestroyOnLoad(new GameObject("@Network", typeof(Network)));
		DontDestroyOnLoad(new GameObject("@Scene", typeof(Scene)));
	}
}
