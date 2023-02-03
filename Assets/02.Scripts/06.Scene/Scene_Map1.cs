using System.Collections.Generic;

using MEC;

using UnityEngine;
using UnityEngine.SceneManagement;

using static Enums;

public class Scene_Map1 : BaseScene
{
	public static GameStartInfo StartInfo { get; set; }
	public int CurrentFrameCount => _netGameLoop.FrameNum;
	public NetGameLoopHandler NetGameLoop => _netGameLoop;
	public ClientGameLoopHandler ClientGameLoop => _clientGameLoop;
	[field: SerializeField] public Map00UI UI { get; private set; }

	[SerializeField] private WorldDataHelper _dataHelper;
	[SerializeField] private AudioClip _ingameBgm;
	[SerializeField] private ClientGameLoopHandler _clientGameLoop;
	[SerializeField] private Material _envColorChange1;
	[SerializeField] private Material _envColorChange2;
	[SerializeField] private Transform _envBase;

	private CoroutineHandle _envCoHandle;

	private readonly NetGameLoopHandler _netGameLoop = new();

	public override void Init(object param)
	{
		Scene.PlaySceneChangeAnim();
		Scenetype = SceneType.Game;
		_netGameLoop.Init(_dataHelper.GetWorldData(), StartInfo);
		_clientGameLoop.Init(_netGameLoop);
		IsReady = true;
		UI.gameObject.SetActive(true);
		UI.PlayWelcomeAnim(StartGame);
		_envCoHandle = Timing.RunCoroutine(Co_EnvChangeColor());
		if (User.Team == TeamType.Red)
		{
			_envBase.Rotate(new(0, 1, 0), 180);
		}
	}

	public void StartGame()
	{
		Audio.PlayAudio(_ingameBgm, _ingameBgm.name, true);
		_netGameLoop.StartGame();
	}

	public void EndGame()
	{
		JobMgr.PushUnityJob(() =>
		{
			Timing.CallDelayed(2, () =>
			{
				Timing.KillCoroutines(_envCoHandle);
				Scene.MoveTo(SceneType.Lobby, User.CharType, LoadSceneMode.Single);
			});
		});
	}

	private IEnumerator<float> Co_EnvChangeColor()
	{
		for (var delta = 0f; ; delta += Time.deltaTime)
		{
			var sine = Mathf.Sin(delta) + 0.5f;
			_envColorChange1.SetFloat("_Ratio", sine);
			_envColorChange2.SetFloat("_Ratio", sine);
			yield return 0f;
		}
	}
}
