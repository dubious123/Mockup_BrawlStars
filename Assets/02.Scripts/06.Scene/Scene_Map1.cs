using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using MEC;

using Newtonsoft.Json;

using Server.Game;
using Server.Game.Data;
using Server.Game.GameRule;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using static Enums;

public class Scene_Map1 : BaseScene
{
	public static GameStartInfo StartInfo { get; set; }
	public int CurrentFrameCount => _netGameLoop.CurrentFrameCount;
	public NetGameLoopHandler NetGameLoop => _netGameLoop;
	public ClientGameLoopHandler ClientGameLoop => _clientGameLoop;

	#region SerializeField
	[SerializeField] private WorldDataHelper _dataHelper;
	[SerializeField] private Map00UI _mapUI;
	[SerializeField] private AudioClip _ingameBgm;
	[SerializeField] private ClientGameLoopHandler _clientGameLoop;
	#endregion

	private readonly NetGameLoopHandler _netGameLoop = new();

	public override void Init(object param)
	{
		Scene.PlaySceneChangeAnim();
		Scenetype = SceneType.Game;
		_netGameLoop.Init(_dataHelper.GetWorldData(), StartInfo);
		_clientGameLoop.Init(_netGameLoop);
		IsReady = true;
		_mapUI.PlayWelcomeAnim(StartGame);
	}

	private void Update()
	{
		_clientGameLoop.MoveClientGameLoop();
	}

	private void StartGame()
	{
		Audio.PlayAudio(_ingameBgm, _ingameBgm.name, true);
		GameInput.SetGameInput();
		GameInput.SetActive(true);
		_netGameLoop.StartGame();
		_clientGameLoop.StartGame();
	}

	public void EndGame()
	{
		GameInput.SetActive(false);
		JobMgr.PushUnityJob(() =>
		{
			Timing.CallDelayed(0.5f, () =>
			{
				Scene.MoveTo(SceneType.Lobby, User.CharType, LoadSceneMode.Single);
			});
		});
	}

	private void HandleRoundStart(int waitMilliseconds)
	{
		Loggers.Debug.Information("Round Start Wait Time : {0}", waitMilliseconds);
		Task.Delay(waitMilliseconds).ContinueWith(_ =>
		{
			Loggers.Game.Information("Round Start");
		});

		//JobMgr.PushUnityJob(() =>
		//{
		//	foreach (var c in _cPlayers)
		//	{
		//		c?.OnRoundStart();
		//	}

		//	_mapUI.OnRoundStart();
		//});
	}

	private void OnRoundEnd(GameRule00.RoundResult result)
	{
		Loggers.Game.Information("Round End {0}", Enum.GetName(typeof(GameRule00.RoundResult), result));
		//JobMgr.PushUnityJob(() =>
		//{
		//	Timing.CallDelayed(Config.ROUND_END_WAIT_FRAMECOUNT / 60f, HandleRoundClear);
		//	_mapUI.OnRoundEnd(result);
		//});
	}

	//private void HandleRoundClear()
	//{
	//	_mapUI.OnRoundClear();
	//}

	//private void HandleRoundReset()
	//{
	//	_mapUI.OnRoundReset();
	//	foreach (var c in _cPlayers)
	//	{
	//		c?.OnRoundReset();
	//	}
	//}

	//private void OnMatchOver(GameRule00.MatchResult result)
	//{
	//	Loggers.Game.Information("Match Over {0}", Enum.GetName(typeof(GameRule00.MatchResult), result));
	//	EndGame();
	//}

	//private void OnPlayerDead(NetCharacter character)
	//{
	//	Loggers.Game.Information("Player dead {0}", character.NetObjId.InstanceId);
	//	JobMgr.PushUnityJob(() =>
	//	{
	//		_mapUI.OnPlayerDead(CPlayers[character.NetObjId.InstanceId]);
	//		CPlayers[character.NetObjId.InstanceId].OnDead();
	//	});
	//}
}
