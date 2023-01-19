using System;

using MEC;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Entry : BaseScene
{
	[SerializeField] private GameObject _eventSystemPrefab;
	[SerializeField] private GameObject _sceneChangeAnimPrefab;
	[SerializeField] private GameObject _audioPrefab;
	[SerializeField] private LogoAnim _logoAnim;

	public override void Init(object param)
	{
		Scenetype = Enums.SceneType.Entry;
		IsReady = true;
	}

	private void Awake()
	{
		Screen.SetResolution(3840 / 2, 2160 / 2, false);
		Application.targetFrameRate = 300;
		Time.fixedDeltaTime = (float)((sfloat)1 / (sfloat)60f);
		DontDestroyOnLoad(new GameObject("@Loggers", typeof(Loggers)));
		DontDestroyOnLoad(new GameObject("@JobMgr", typeof(JobMgr)));
		DontDestroyOnLoad(new GameObject("@Timing", typeof(Timing)));
		DontDestroyOnLoad(new GameObject("@Network", typeof(Network)));
		DontDestroyOnLoad(new GameObject("@Scene", typeof(Scene)));
		Instantiate(_sceneChangeAnimPrefab, GameObject.Find("@Scene").transform);
		DontDestroyOnLoad(Instantiate(_eventSystemPrefab));
		var go = Instantiate(_audioPrefab);
		go.name = "@Audio";
		DontDestroyOnLoad(go);
		Loggers.Init();
		JobMgr.Init();
		Network.Init();
		Audio.Init();
		Scene.Init();
		_logoAnim.PlayAnim(() => Scene.MoveTo(Enums.SceneType.Loading, Enums.SceneType.Lobby, LoadSceneMode.Additive));
	}
}
