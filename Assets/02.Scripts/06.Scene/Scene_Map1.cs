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
	[SerializeField] private AudioClip _gameWinBgm;
	[SerializeField] private AudioClip _gameLoseBgm;
	[SerializeField] private AudioClip _gameDrawBgm;
	[SerializeField] private ClientGameLoopHandler _clientGameLoop;
	[SerializeField] private Material _envColorChange1;
	[SerializeField] private Material _envColorChange2;
	[SerializeField] private Color _envColor1;
	[SerializeField] private Color _envColor2;

	[SerializeField] private Transform _envBase;

	private CoroutineHandle _envCoHandle;

	private readonly NetGameLoopHandler _netGameLoop = new();

	public override void Init(object param)
	{
		_envColorChange1.SetColor("_BaseColor", _envColor1);
		_envColorChange2.SetColor("_BaseColor", _envColor2);
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
		Audio.StopAudio(_ingameBgm.name);
		if (_netGameLoop.MatchRes == Server.Game.GameRule.GameRule00.MatchResult.None)
		{
			Audio.PlayOnce(_gameDrawBgm);
		}
		else if (_netGameLoop.MatchRes == Server.Game.GameRule.GameRule00.MatchResult.Blue)
		{
			Audio.PlayOnce(User.Team == TeamType.Blue ? _gameWinBgm : _gameLoseBgm);
		}
		else
		{
			Audio.PlayOnce(User.Team == TeamType.Red ? _gameWinBgm : _gameLoseBgm);
		}

		Camera.main.GetComponent<GameCameraController>().OnGameEnd();
		Timing.CallDelayed(3.5f, () =>
		{
			JobMgr.PushUnityJob(() =>
			{
				Timing.KillCoroutines();
				Scene.MoveTo(SceneType.Lobby, User.CharType, LoadSceneMode.Single);
			});
		});
	}

	private IEnumerator<float> Co_EnvChangeColor()
	{
		for (var delta = 0f; ; delta += Time.deltaTime)
		{
			var ratio = Mathf.Clamp(Mathf.Sin(delta) + 0.5f, 0, 1);
			_envColorChange1.SetColor("_BaseColor", Color.Lerp(_envColor1, _envColor2, ratio));
			_envColorChange2.SetColor("_BaseColor", Color.Lerp(_envColor2, _envColor1, ratio));
			yield return 0f;
		}
	}

	private void OnApplicationQuit()
	{
		_envColorChange1.SetColor("_BaseColor", _envColor1);
		_envColorChange2.SetColor("_BaseColor", _envColor2);
	}
}
