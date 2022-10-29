using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MEC;

using UnityEngine;
using UnityEngine.InputSystem;

using static Enums;
using static GameFrameInfo;

public class Scene_Map1 : BaseScene
{
	public NetPhysics2D NPhysics2D { get; private set; }
	public long CurrentTick => _currentTick;
	public bool GameStarted => _gameStarted;
	#region SerializeField
	//[SerializeField] AssetReference _dog;
	[SerializeField] private GameObject _dog;
	[SerializeField] private GameObject _gameMessage_waiting;
	[SerializeField] private InputActionAsset _inputAsset;
	[SerializeField] private Transform[] _spawnPoint;
	[SerializeField] private BaseCharacter[] _characters;
	#endregion
	private ConcurrentQueue<GameFrameInfo> _frameInfoQueue;
	private IEnumerator<float> _coHandler;
	private object _lock = new();
	private long _currentTick = 0;
	private bool _gameStarted = false;
	public override void Init(object param)
	{
		var req = param as S_EnterGame;
		Scenetype = SceneType.Game;
		NPhysics2D = new NetPhysics2D();

		NPhysics2D.GetNewBoxCollider2D(new Wall() { Position = new sVector3(0.5f, 0, -12.5f) }, new sVector2(0f, 0f), new sVector2(18f, 1f));
		NPhysics2D.GetNewBoxCollider2D(new Wall() { Position = new sVector3(0.5f, 0, 12.5f) }, new sVector2(0f, 0f), new sVector2(18f, 1f));
		NPhysics2D.GetNewBoxCollider2D(new Wall() { Position = new sVector3(-9.5f, 0, 0) }, new sVector2(0f, 0f), new sVector2(1f, 25f));
		NPhysics2D.GetNewBoxCollider2D(new Wall() { Position = new sVector3(9.5f, 0, 0) }, new sVector2(0f, 0f), new sVector2(1f, 25f));
		NPhysics2D.GetNewBoxCollider2D(new Wall() { Position = new sVector3(0, 0, 0) }, new sVector2(0f, 0f), new sVector2(4f, 4f));

		_characters = new BaseCharacter[6];
		_frameInfoQueue = new ConcurrentQueue<GameFrameInfo>();
		Enter(req.TeamId, User.CharType);
		Camera.main.GetComponent<GameCameraController>().FollowTarget = _characters[User.TeamId].transform;
		IsReady = true;
		Network.RegisterSend(new C_GameReady(User.UserId));
	}

	private void FixedUpdate()
	{
		_coHandler?.MoveNext();
	}

	private IEnumerator<float> Co_FixedUpdate()
	{
		BaseCharacter character;
		GameFrameInfo info;
		StringBuilder sb = new();
		while (true)
		{
			sb.AppendLine($"[{DateTime.Now}.{DateTime.Now.Millisecond:000}]");
			sb.AppendLine($"-------------Start Frame [{_currentTick}]--------------");
			while (_frameInfoQueue.TryDequeue(out info) == false)
			{
				//todo
				yield return 0f;
			}

			info.ToString(sb);
			sb.AppendLine("Handling Frame Info");
			foreach (var ctx in info.ActionContexts)
			{
				foreach (var obj in ctx.Objects)
				{
					obj.OnGetHit(ctx.Subject.GetHitInfo(ctx.ActionCode));
				}
			}

			for (int i = 0; i < 6; i++)
			{
				character = _characters[i];
				if (character is null) continue;
				character.HandleInput(ref info.MoveInput[i], ref info.LookInput[i], info.ButtonPressed[i]);
				character.HandleOneFrame();
				sb.AppendLine($"Player[{i}] handle one frame result");
				sb.AppendLine($"({character.Position.x},{character.Position.x},{character.Position.z},)");
				sb.AppendLine($"({character.Rotation.x},{character.Rotation.y},{character.Rotation.z},)");
			}

			sb.AppendLine($"[{DateTime.Now}.{DateTime.Now.Millisecond:000}]");
			sb.AppendLine("--------Frame End--------");
			LogMgr.Log(LogSourceType.Game, sb.ToString());
			sb.Clear();
			_currentTick++;
			yield return 0f;
		}
	}

	public void Enter(short teamId, CharacterType type)
	{
		Debug.Assert(_characters[teamId] is null);
		var character = Instantiate(_dog, _spawnPoint[teamId].position, Quaternion.identity).GetComponent<BaseCharacter>();
		_characters[teamId] = character;
		character.TeamId = teamId;
		character.Init();
		if (User.TeamId == teamId)
		{
			var playerInput = character.gameObject.AddComponent<PlayerInput>();
			{
				playerInput.actions = _inputAsset;
				playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
				playerInput.actions.Enable();
			}
			character.gameObject.AddComponent<DogController>().Init(character);
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
				Subject = _characters[action.Subject],
				Objects = action.Objects.Select(id => _characters[id]).ToArray(),
			}));

		_frameInfoQueue.Enqueue(info);
	}
}
