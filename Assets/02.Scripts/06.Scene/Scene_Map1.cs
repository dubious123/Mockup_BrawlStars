using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MEC;

using Newtonsoft.Json;

using Server.Game;
using Server.Game.Data;
using Server.Game.GameRule;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using static Enums;

public class Scene_Map1 : BaseScene
{
	public int CurrentTick => World.GameRule.CurrentRoundFrameCount;
	public int MaxTick => World.GameRule.MaxFrameCount;
	public bool GameStarted => _gameStarted;
	public CPlayer[] CPlayers => _cPlayers;

	#region SerializeField
	//[SerializeField] AssetReference _dog;
	[SerializeField] private GameObject _knight;
	[SerializeField] private GameObject _shelly;
	[SerializeField] private InputActionAsset _inputAsset;
	[SerializeField] private WorldDataHelper _dataHelper;
	[SerializeField] private Map00UI _mapUI;
	[SerializeField] private GameCameraController _cam;
	[SerializeField] private Transform _playerParentBlue, _playerParentRed;
	[SerializeField] private AudioClip _ingameBgm;
	#endregion

	private sVector3[] _spawnPoints;
	private CPlayer[] _cPlayers;
	private ConcurrentQueue<GameFrameInfo> _frameInfoQueue;
	private IEnumerator<float> _coHandler;
	private bool _gameStarted = false;

	public NetWorld World;

	public override void Init(object param)
	{
		Debug.Log("Init");
		Scenetype = SceneType.Game;
		Scene.PlaySceneChangeAnim();
		var data = _dataHelper.GetWorldData();
		World = new(data, new GameRule00()
		{
			OnMatchStart = OnMatchStart,
			OnRoundStart = OnRoundStart,
			OnRoundEnd = OnRoundEnd,
			OnRoundClear = OnRoundClear,
			OnRoundReset = OnRoundReset,
			OnMatchOver = OnMatchOver,
			OnPlayerDead = OnPlayerDead,
		});

		GetComponentInChildren<CEnvSystem>(true).Init(World.EnvSystem);
		_cPlayers = new CPlayer[6];
		_frameInfoQueue = new ConcurrentQueue<GameFrameInfo>();
		_spawnPoints = data.SpawnPoints;
		IsReady = true;
		Network.RegisterSend(new C_GameReady(User.UserId));
		Network.StartSyncTime();
	}

	//private void FixedUpdate()
	//{
	//	_coHandler?.MoveNext();
	//}

	private IEnumerator<float> Co_FixedUpdate()
	{
		GameFrameInfo info;
		Loggers.Game.Information("---------------StartGame----------------");
		while (true)
		{
			Loggers.Game.Information("---------------Frame [{0}]----------------", CurrentTick);
			_frameInfoQueue.TryDequeue(out info);
			World.UpdateInputs(info);
			World.Update();
			_mapUI.HandleOneFrame();
			Loggers.Game.Information("------------------------------------------");
			yield return 0f;
		}
	}

	public void Enter(short teamId, NetObjectType type)
	{
		Debug.Assert(_cPlayers[teamId] is null);
		var netCharacter = World.ObjectBuilder.GetNewObject(type).GetComponent<NetCharacter>();
		var parent = netCharacter.Team == TeamType.Blue ? _playerParentBlue : _playerParentRed;
		var cPlayer = Instantiate(_shelly, (Vector3)_spawnPoints[teamId], Quaternion.identity, parent).GetComponent<CPlayer>();
		netCharacter.OnFrameStart = cPlayer.HandleOneFrame;
		_cPlayers[teamId] = cPlayer;
		if (User.TeamId == teamId)
		{
			var playerInput = cPlayer.gameObject.AddComponent<PlayerInput>();
			{
				playerInput.actions = _inputAsset;
				playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
				playerInput.actions.Enable();
			}

			User.Team = netCharacter.Team;
			cPlayer.gameObject.AddComponent<CharacterController>().Init();
			_cam.Init(cPlayer.transform);
		}
	}

	public void StartGame(float waitTime)
	{
		World.Reset();
		foreach (var nPlayer in World.CharacterSystem.ComponentDict)
		{
			CPlayers[nPlayer.NetObjId.InstanceId].Init(nPlayer, (short)nPlayer.NetObjId.InstanceId);
		}

		_mapUI.OnGameStart(Internal_StartGame);
	}

	private void Internal_StartGame()
	{
		_gameStarted = true;
		Audio.PlayAudio(_ingameBgm, _ingameBgm.name, true);
		_coHandler = Co_FixedUpdate();
	}

	public void Exit()
	{

	}

	public void HandleGameState(S_GameFrameInfo req)
	{
		//Debug.Assert(_characters[teamId] is not null);
		var info = new GameFrameInfo(req);
		_frameInfoQueue.Enqueue(info);
		JobMgr.PushUnityJob(() => _coHandler?.MoveNext());
	}

	public void EndGame()
	{
		Network.StopSyncTime();
		Timing.CallDelayed(3f, () =>
		{
			Scene.MoveTo(SceneType.Lobby, User.CharType, LoadSceneMode.Single);
			_coHandler = null;
		});
	}

	private void OnMatchStart()
	{
		Loggers.Game.Information("Match Start");
		foreach (var c in _cPlayers)
		{
			c?.OnMatchStart();
		}
	}

	private void OnRoundStart()
	{
		Loggers.Game.Information("Round Start");
		foreach (var c in _cPlayers)
		{
			c?.OnRoundStart();
		}

		_mapUI.OnRoundStart();
	}

	private void OnRoundEnd(GameRule00.RoundResult result)
	{
		Loggers.Game.Information("Round End {0}", Enum.GetName(typeof(GameRule00.RoundResult), result));
		_mapUI.OnRoundEnd(result);
	}

	private void OnRoundClear()
	{
		Loggers.Game.Information("Round Clear");
		foreach (var c in _cPlayers)
		{
			c?.OnRoundClear();
		}

		_mapUI.OnRoundClear();
	}

	private void OnRoundReset()
	{
		Loggers.Game.Information("Round Reset");
		foreach (var c in _cPlayers)
		{
			c?.OnRoundReset();
		}

		_mapUI.OnRoundReset();
	}

	private void OnMatchOver(GameRule00.MatchResult result)
	{
		Loggers.Game.Information("Match Over {0}", Enum.GetName(typeof(GameRule00.MatchResult), result));
		_mapUI.OnMatchOver();
	}

	private void OnPlayerDead(NetCharacter character)
	{
		Loggers.Game.Information("Player dead {0}", character.NetObjId.InstanceId);
		_mapUI.OnPlayerDead(CPlayers[character.NetObjId.InstanceId]);
		CPlayers[character.NetObjId.InstanceId].OnDead();
	}
}
