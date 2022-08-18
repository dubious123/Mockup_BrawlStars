using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Scene_Entry : BaseScene
{
	public override void Init(object param)
	{

	}
	private void Awake()
	{
		DontDestroyOnLoad(new GameObject("@Timing", typeof(Timing)));
		DontDestroyOnLoad(new GameObject("@Network", typeof(Network)));
		DontDestroyOnLoad(new GameObject("@Scene", typeof(Scene)));
		Scene.MoveTo(SceneType.Lobby, CharacterType.Dog);
	}
}
