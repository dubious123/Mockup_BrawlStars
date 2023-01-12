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
	private GameRule00 _gameRule;
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
		_gameRule = new GameRule00()
		{
			OnRoundStart = OnRoundStart,
			OnRoundEnd = OnRoundEnd,
			OnGameEnd = OnGameEnd,
			OnPlayerDead = OnPlayerDead
		};

		_cPlayers = new CPlayer[6];
		_frameInfoQueue = new ConcurrentQueue<GameFrameInfo>();
		_spawnPoints = data.SpawnPoints;
		World = new(data, _gameRule);
		//Enter(User.TeamId, User.CharType);
		IsReady = true;
		Network.RegisterSend(new C_GameReady(User.UserId));
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
				Debug.Log("empty");
				yield return 0f;
			}

			World.InputInfo = info;
			World.Update();
			_currentTick++;
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
		Debug.Log("StartGame");
		World.OnWorldStart();
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

	private void OnRoundStart()
	{
		Debug.Log("OnRoundStart");
	}

	private void OnRoundEnd(GameRule00.RoundResult result)
	{
		Debug.Log("OnRoundEnd");
	}

	private void OnGameEnd(GameRule00.GameResult result)
	{
		Debug.Log("OnGameEnd");
	}

	private void OnPlayerDead(NetCharacter character)
	{
		Debug.Log($"OnPlayerDead{character.NetObjId.InstanceId}");
		_mapUI.OnPlayerDead((uint)character.NetObjId.InstanceId);
		CPlayers[character.NetObjId.InstanceId].gameObject.SetActive(false);
	}
}
