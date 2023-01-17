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

using static Enums;

public class Scene_Map1 : BaseScene
{
	public long CurrentTick => _currentTick;
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
	private object _lock = new();
	private long _currentTick = 0;
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

		_cPlayers = new CPlayer[6];
		_frameInfoQueue = new ConcurrentQueue<GameFrameInfo>();
		_spawnPoints = data.SpawnPoints;
		IsReady = true;
		Network.RegisterSend(new C_GameReady(User.UserId));
		Network.StartSyncTime();
	}

	private void FixedUpdate()
	{
		_coHandler?.MoveNext();
	}

	private IEnumerator<float> Co_FixedUpdate()
	{
		GameFrameInfo info;
		Loggers.Game.Information("---------------StartGame----------------");
		while (true)
		{
			Loggers.Game.Information("---------------Frame [{0}]----------------", _currentTick);
			while (_frameInfoQueue.TryDequeue(out info) == false)
			{
				//todo
				//Debug.Log("empty");
				yield return 0f;
			}

			World.UpdateInputs(info);
			World.Update();
			_currentTick++;
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

		cPlayer.Init(netCharacter, teamId);
	}

	public void StartGame(float waitTime)
	{
		World.Reset();
		_mapUI.OnGameStart(Internal_StartGame);
	}

	private void Internal_StartGame()
	{
		_gameStarted = true;
		foreach (var c in _cPlayers)
		{
			c?.StartGame();
		}

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
	}

	private void OnMatchStart()
	{
		Loggers.Game.Information("Match Start");
	}

	private void OnRoundStart()
	{
		Loggers.Game.Information("Round Start");
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
			c?.OnClear();
		}
	}

	private void OnRoundReset()
	{
		Loggers.Game.Information("Round Reset");
		foreach (var c in _cPlayers)
		{
			c?.Reset();
		}
	}

	private void OnMatchOver(GameRule00.MatchResult result)
	{
		Loggers.Game.Information("Match End {0}", Enum.GetName(typeof(GameRule00.MatchResult), result));
	}

	private void OnPlayerDead(NetCharacter character)
	{
		Loggers.Game.Information("Player dead {0}", character.NetObjId.InstanceId);
		_mapUI.OnPlayerDead((uint)character.NetObjId.InstanceId);
		CPlayers[character.NetObjId.InstanceId].OnDead();
	}
}
