using MEC;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Entry : BaseScene
{
	[SerializeField] private GameObject _eventSystemPrefab;
	[SerializeField] private GameObject _sceneChangeAnimPrefab;
	[SerializeField] private GameObject _audioPrefab;
	[SerializeField] private GameObject _inputPrefab;
	[SerializeField] private GameObject _dataPrefab;
	[SerializeField] private LogoAnim _logoAnim;

	public override void Init(object param)
	{
		Scenetype = Enums.SceneType.Entry;
		IsReady = true;
	}

	private void Awake()
	{
		//Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
		Screen.SetResolution(3840 / 2, 2160 / 2, false);
		Application.targetFrameRate = 180;
		Time.fixedDeltaTime = (float)((sfloat)1 / (sfloat)60f);
		DontDestroyOnLoad(new GameObject("@Loggers", typeof(Loggers)));
		DontDestroyOnLoad(new GameObject("@JobMgr", typeof(JobMgr)));
		DontDestroyOnLoad(new GameObject("@Timing", typeof(Timing)));
		DontDestroyOnLoad(new GameObject("@Network", typeof(Network)));
		DontDestroyOnLoad(new GameObject("@User", typeof(User)));
		DontDestroyOnLoad(new GameObject("@Scene", typeof(Scene)));
		Instantiate(_sceneChangeAnimPrefab, GameObject.Find("@Scene").transform);
		InstantiateFromPrefab(_eventSystemPrefab, "@EventSystem");
		InstantiateFromPrefab(_audioPrefab, "@Audio");
		InstantiateFromPrefab(_inputPrefab, "@Input");
		InstantiateFromPrefab(_dataPrefab, "@Data");

		Config.Init();
		GameInput.Init();
		Loggers.Init();
		JobMgr.Init();
		Network.Init();
		Audio.Init();
		Data.Init();
		Scene.Init();

		_logoAnim.PlayAnim(() => Scene.MoveTo(Enums.SceneType.Loading, Enums.SceneType.Lobby, LoadSceneMode.Additive));
	}

	private GameObject InstantiateFromPrefab(GameObject prefab, string name)
	{
		var instance = Instantiate(prefab);
		instance.name = name;
		DontDestroyOnLoad(instance);
		return instance;
	}
}
