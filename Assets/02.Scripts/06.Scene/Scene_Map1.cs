using Logging;
using MEC;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using Utils;
using static Enums;

public class Scene_Map1 : BaseScene
{
	#region SerializeField
	//[SerializeField] AssetReference _dog;
	[SerializeField] GameObject _dog;
	[SerializeField] InputActionAsset _inputAsset;
	[SerializeField] Transform[] _spawnPoint;
	[SerializeField] BaseCharacter[] _characters;
	#endregion
	ConcurrentQueue<InputInfo>[] _inputBuffers;
	//PriorityQueue<InputInfo, long>[] _inputQueues;
	public long CurrentTick => _currentTick;
	public bool GameStarted => _gameStarted;
	private object _lock = new();
	long _currentTick = 0;
	bool _gameStarted = false;
	public override void Init(object param)
	{
		var req = param as S_EnterGame;
		Scenetype = SceneType.Game;
		_characters = new BaseCharacter[6];
		_inputBuffers = new ConcurrentQueue<InputInfo>[6];
		//var handle = _dog.LoadAssetAsync<GameObject>();
		//handle.Completed += _ =>
		//{
		//	Enter(User.TeamId, User.CharType);
		//	Camera.main.GetComponent<GameCameraController>().FollowTarget = _characters[User.TeamId].transform;
		//	IsReady = true;
		//};
		for (int i = 0; i < req.PlayerInfoArr.Length; i++)
		{
			var playerInfo = req.PlayerInfoArr[i];
			if (playerInfo.CharacterType == 0) continue;
			Enter((short)i, (CharacterType)playerInfo.CharacterType);
		}
		Camera.main.GetComponent<GameCameraController>().FollowTarget = _characters[User.TeamId].transform;
		IsReady = true;
		Network.RegisterSend(new C_GameReady(User.UserId));
	}
	private void FixedUpdate()
	{
		if (_gameStarted == false) return;
		LogMgr.Log(LogSourceType.Debug, "Update Start");
		Co_FixedUpdate().MoveNext();
		LogMgr.Log(LogSourceType.Debug, "Update End");

	}
	private IEnumerator<float> Co_FixedUpdate()
	{
		BaseCharacter character;
		//PriorityQueue<InputInfo, long> inputQueue;
		ConcurrentQueue<InputInfo> inputBuffer;
		while (true)
		{
			_currentTick++;
			for (int i = 0; i < 6; i++)
			{
				character = _characters[i];
				inputBuffer = _inputBuffers[i];
				if (character is null) continue;

				if (inputBuffer.TryPeek(out var input) == false || input.TargetTick > _currentTick)
				{
					character.HandleInput(default(InputInfo));
					continue;
				}
				inputBuffer.TryDequeue(out input);
				LogMgr.Log(LogSourceType.Debug, $"[Tick : {_currentTick}]\nDequeueing {JsonUtility.ToJson(input)}");
				character.HandleInput(input);
				character.AwakeAll();
			}
			for (int i = 0; i < 6; i++)
			{
				character = _characters[i];
				if (character is null) continue;
				character.HandleOneFrame();
			}
			yield return 0f;
		}
	}

	public void Enter(short teamId, CharacterType type)
	{
		Debug.Assert(_characters[teamId] is null);
		var character = Instantiate(_dog, _spawnPoint[teamId].position, Quaternion.identity).GetComponent<BaseCharacter>();
		_characters[teamId] = character;
		_inputBuffers[teamId] = new();
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
	void Internal_StartGame()
	{
		LogMgr.Log(LogSourceType.Debug, "---------------StartGame----------------");
		_gameStarted = true;
	}
	public void UpdatePlayer(short teamId, Vector2 moveDir, Vector2 lookDir)
	{
		var character = _characters[teamId];
		if (character is null || teamId == User.TeamId) return;
		//character.Move(new Vector3(moveDir.x, 0, moveDir.y));
		//character.Look(new Vector3(lookDir.x, 0, lookDir.y));
	}
	public void Exit()
	{

	}
	public void EnqueueInputInfo(short teamId, in InputInfo info)
	{
		//Debug.Assert(_characters[teamId] is not null);
		var buffer = _inputBuffers[teamId];
		if (buffer is null) return;
		LogMgr.Log(LogSourceType.Debug, $"[Tick : {_currentTick}]\nEnqueueing {JsonUtility.ToJson(info)}");
		buffer.Enqueue(info);
	}

}
