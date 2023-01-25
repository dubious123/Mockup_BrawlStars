using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using UnityEngine.UIElements;

using static Enums;

public class Scene_Map1 : BaseScene
{
	public static GameStartInfo StartInfo { private get; set; }
	public int CurrentTick => _world.GameRule.CurrentRoundFrameCount;
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
	private ConcurrentQueue<GameFrameInfo> _frameInfoQueue = new();
	private CharacterController _input;
	private System.Timers.Timer _gameLoopTimer;
	private bool _gameStarted = false;

	public NetWorld _world;
	public GameState State { get; private set; } = GameState.Waiting;

	public override void Init(object param)
	{
		Debug.Log("Init");
		Scenetype = SceneType.Game;
		Scene.PlaySceneChangeAnim();
		var data = _dataHelper.GetWorldData();
		_world = new(data, new GameRule00()
		{
			OnRoundEnd = OnRoundEnd,
			OnMatchOver = OnMatchOver,
			OnPlayerDead = OnPlayerDead,
		});

		_gameLoopTimer = new System.Timers.Timer(1000 / 60d);
		_gameLoopTimer.Elapsed += MoveGameLoop;
		_gameLoopTimer.Start();
		GetComponentInChildren<CEnvSystem>(true).Init(_world.EnvSystem);
		_spawnPoints = data.SpawnPoints;
		_cPlayers = new CPlayer[Config.MAX_PLAYER_COUNT];
		for (int i = 0; i < StartInfo.CharacterType.Length; i++)
		{
			Enter((short)i, StartInfo.CharacterType[i]);
		}

		IsReady = true;
		StartGame();
	}

	private int _lock = 0;
	private void MoveGameLoop(object abj, ElapsedEventArgs args)
	{
		if (Interlocked.CompareExchange(ref _lock, 1, 0) == 1)
		{
			return;
		}

		Loggers.Debug.Information("entering, {0}", Enum.GetName(typeof(GameState), State));
		switch (State)
		{
			case GameState.Waiting:
				break;
			case GameState.Started:
				HandleOneFrame();
				break;
			case GameState.Ended:
				Loggers.Debug.Information("Start Fake Update");
				_world.UpdateInputs(GameFrameInfo.GetDefault(Config.MAX_PLAYER_COUNT));
				_world.Update();
				break;
		}

		Loggers.Debug.Information("exiting");
		Interlocked.Exchange(ref _lock, 0);
	}

	private void HandleOneFrame()
	{
		Loggers.Game.Information("---------------Frame [{0}]----------------", CurrentTick);
		Loggers.Game.Information("Reserve Count : {0}", _frameInfoQueue.Count);
		if (_frameInfoQueue.TryDequeue(out var info) is false || CurrentTick != info.FrameNum)
		{
			Loggers.Error.Information("frame queue is empty or invalid frameCount {0}", CurrentTick);
			return;
		}

		_world.UpdateInputs(info);
		_world.Update();
		_input.SendInput(CurrentTick + Config.FRAME_BUFFER_COUNT);
		JobMgr.PushUnityJob(_mapUI.HandleOneFrame);
		foreach (var player in _world.CharacterSystem.ComponentDict)
		{
			Loggers.Game.Information("Player [{0}]", player.NetObj.ObjectId.InstanceId);
			Loggers.Game.Information("Position [{0:x},{1:x},{2:x}]] : ", player.Position.x.RawValue, player.Position.y.RawValue, player.Position.z.RawValue);
		}

		Loggers.Game.Information("------------------------------------------");
	}

	public void Enter(short teamId, NetObjectType type)
	{
		Debug.Assert(_cPlayers[teamId] is null);
		var netCharacter = _world.ObjectBuilder.GetNewObject(type).GetComponent<NetCharacter>();
		var parent = netCharacter.Team == TeamType.Blue ? _playerParentBlue : _playerParentRed;
		var cPlayer = Instantiate(_shelly, (Vector3)_spawnPoints[teamId], Quaternion.identity, parent).GetComponent<CPlayer>();
		netCharacter.OnFrameStart = () => JobMgr.PushUnityJob(cPlayer.HandleOneFrame);
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
			_input = cPlayer.gameObject.AddComponent<CharacterController>();
			_input.Init();
			_cam.Init(cPlayer.transform);
		}
	}

	public void StartGame()
	{
		Loggers.Game.Information("---------------StartGame----------------");
		foreach (var nPlayer in _world.CharacterSystem.ComponentDict)
		{
			CPlayers[nPlayer.NetObjId.InstanceId].Init(nPlayer, (short)nPlayer.NetObjId.InstanceId);
		}

		_mapUI.OnGameStart(HandleMatchStart);
		Network.StartSyncTime();
	}

	public void HandleGameInput(S_GameFrameInfo req)
	{
		var info = new GameFrameInfo(req);
		_frameInfoQueue.Enqueue(info);
		if (info.FrameNum > 0)
		{
			return;
		}
		else if (info.FrameNum < 0)
		{
			_input.SendInput(info.FrameNum + 1);
		}
		else
		{
			HandleRoundStart();
		}
	}

	public void EndGame()
	{
		Network.StopSyncTime();
		JobMgr.PushUnityJob(() =>
		{
			Timing.CallDelayed(0.5f, () =>
			{
				Scene.MoveTo(SceneType.Lobby, User.CharType, LoadSceneMode.Single);
			});
		});
	}

	private void HandleMatchStart()
	{
		Loggers.Game.Information("Match Start");
		_world.Reset();
		_gameStarted = true;
		Audio.PlayAudio(_ingameBgm, _ingameBgm.name, true);
		foreach (var c in _cPlayers)
		{
			c?.OnMatchStart();
		}

		StartReserveInputBuffer();
	}

	private void StartReserveInputBuffer()
	{
		_frameInfoQueue.Clear();
		_input.SendInput(-Config.FRAME_BUFFER_COUNT);
	}

	private void HandleRoundStart()
	{
		Loggers.Game.Information("Round Start");
		State = GameState.Started;

		JobMgr.PushUnityJob(() =>
		{
			foreach (var c in _cPlayers)
			{
				c?.OnRoundStart();
			}

			_mapUI.OnRoundStart();
		});
	}

	private void OnRoundEnd(GameRule00.RoundResult result)
	{
		State = GameState.Ended;
		Loggers.Game.Information("Round End {0}", Enum.GetName(typeof(GameRule00.RoundResult), result));
		JobMgr.PushUnityJob(() =>
		{
			Timing.CallDelayed(Config.ROUND_END_WAIT_FRAMECOUNT / 60f, HandleRoundClear);
			_mapUI.OnRoundEnd(result);
		});
	}

	private void HandleRoundClear()
	{
		Loggers.Game.Information("Round Clear");
		JobMgr.PushUnityJob(() =>
		{
			_mapUI.OnRoundClear();
			foreach (var c in _cPlayers)
			{
				c?.OnRoundClear();
			}

			_world.Clear();
			State = GameState.Waiting;
			Timing.CallDelayed(Config.ROUND_CLEAR_WAIT_FRAMECOUNT / 60f, HandleRoundReset);
		});

	}

	private void HandleRoundReset()
	{
		Loggers.Game.Information("Round Reset");
		_world.Reset();
		JobMgr.PushUnityJob(() =>
		{
			Timing.CallDelayed(Config.ROUND_RESET_WAIT_FRAMECOUNT / 60f, () =>
			{
				_mapUI.OnRoundReset();
				foreach (var c in _cPlayers)
				{
					c?.OnRoundReset();
				}

				StartReserveInputBuffer();
			});
		});
	}

	private void OnMatchOver(GameRule00.MatchResult result)
	{
		Loggers.Game.Information("Match Over {0}", Enum.GetName(typeof(GameRule00.MatchResult), result));
		State = GameState.Ended;

		JobMgr.PushUnityJob(() =>
		{
			_mapUI.OnMatchOver();
			Timing.CallDelayed(3f, () =>
			{
				_gameLoopTimer.Stop();
				EndGame();
			});
		});
	}

	private void OnPlayerDead(NetCharacter character)
	{
		Loggers.Game.Information("Player dead {0}", character.NetObjId.InstanceId);
		JobMgr.PushUnityJob(() =>
		{
			_mapUI.OnPlayerDead(CPlayers[character.NetObjId.InstanceId]);
			CPlayers[character.NetObjId.InstanceId].OnDead();
		});
	}
}
