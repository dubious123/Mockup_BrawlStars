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

using UnityEngine;
using UnityEngine.InputSystem;

using static Enums;
using static GameFrameInfo;

public class Scene_Map1 : BaseScene
{
	public long CurrentTick => _currentTick;
	public bool GameStarted => _gameStarted;
	#region SerializeField
	//[SerializeField] AssetReference _dog;
	[SerializeField] private GameObject _dog;
	[SerializeField] private GameObject _gameMessage_waiting;
	[SerializeField] private InputActionAsset _inputAsset;
	[SerializeField] private Transform[] _spawnPoint;
	[SerializeField] private CPlayer[] _playerRenderers;
	#endregion
	private ConcurrentQueue<GameFrameInfo> _frameInfoQueue;
	private IEnumerator<float> _coHandler;
	private object _lock = new();
	private long _currentTick = 0;
	private bool _gameStarted = false;

	public NetWorld World;

	public override void Init(object param)
	{
		var req = param as S_EnterGame;
		Scenetype = SceneType.Game;
		_playerRenderers = new CPlayer[6];
		_frameInfoQueue = new ConcurrentQueue<GameFrameInfo>();
		World = new NetWorld(GetWorldData);
		Enter(req.TeamId, (CharacterType)req.PlayerInfo.CharacterType);
		IsReady = true;
		Network.RegisterSend(new C_GameReady(User.UserId));
	}

	public WorldData GetWorldData()
	{
		var go = GameObject.Find("@NetObjects");
		var renderers = go.GetComponentsInChildren<CBoxCollider2DGizmoRenderer>();
		var worldData = new WorldData()
		{
			NetObjectDatas = new NetObjectData[renderers.Length]
		};

		for (uint i = 0; i < renderers.Length; i++)
		{
			worldData.NetObjectDatas[i] = new NetObjectData()
			{
				NetObjectId = i << 1,
				Position = (sVector3)renderers[i].transform.position,
				Rotation = sQuaternion.identity,
				Collider = new NetBoxCollider2DData()
				{
					Size = (sVector2)renderers[i].Size,
					Offset = (sVector2)renderers[i].Offset,
				}
			};
		}
		return worldData;
	}

	private void FixedUpdate()
	{
		_coHandler?.MoveNext();
	}

	private IEnumerator<float> Co_FixedUpdate()
	{
		GameFrameInfo info;
		while (true)
		{
			while (_frameInfoQueue.TryDequeue(out info) == false)
			{
				//todo
				yield return 0f;
			}

			World.InputInfo = info;
			World.Update();

			for (int i = 0; i < 6; i++)
			{
				_playerRenderers[i]?.HandleOneFrame();
			}

			_currentTick++;
			yield return 0f;
		}
	}

	public void Enter(short teamId, CharacterType type)
	{
		Debug.Assert(_playerRenderers[teamId] is null);
		var player = Instantiate(_dog, _spawnPoint[teamId].position, Quaternion.identity).GetComponent<CPlayer>();
		player.Init(new NetCharacterDog((sVector3)player.transform.position, sQuaternion.identity, NetObjectTag.Character, World));
		World.AddNewNetObject((uint)teamId, player.NPlayer);
		_playerRenderers[teamId] = player;
		if (User.TeamId == teamId)
		{
			var playerInput = player.gameObject.AddComponent<PlayerInput>();
			{
				playerInput.actions = _inputAsset;
				playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
				playerInput.actions.Enable();
			}

			player.gameObject.AddComponent<DogController>().Init(player.NPlayer);
			Camera.main.GetComponent<GameCameraController>().FollowTarget = player.transform;
		}
	}

	public void StartGame(float waitTime)
	{
		Timing.CallDelayed(waitTime, Internal_StartGame);
	}

	private void Internal_StartGame()
	{
		_gameMessage_waiting.SetActive(false);
		_gameStarted = true;
		LogMgr.Log(LogSourceType.Game, "---------------StartGame----------------");
		_coHandler = Co_FixedUpdate();
	}

	public void Exit()
	{

	}

	public void HandleGameState(S_BroadcastGameState req)
	{
		//Debug.Assert(_characters[teamId] is not null);
		var info = new GameFrameInfo(req, actions =>
			actions.Select(action => new GameActionContext()
			{
				ActionCode = action.ActionCode,
				//Subject = _characters[action.Subject],
				//Objects = action.Objects.Select(id => _characters[id]).ToArray(),
			}));

		_frameInfoQueue.Enqueue(info);
	}
}
