using MEC;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Entry : BaseScene
{
	[SerializeField] private GameObject _eventSystemPrefab;
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
		DontDestroyOnLoad(new GameObject("@Audio", typeof(Audio)));
		DontDestroyOnLoad(Instantiate(_eventSystemPrefab));
		Loggers.Init();
		JobMgr.Init();
		Network.Init();
		Audio.Init();
		Scene.MoveTo(Enums.SceneType.Loading, Enums.SceneType.Lobby);
	}
}
