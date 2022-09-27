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
	ConcurrentQueue<GameFrameInfo> _frameInfoQueue;
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
		_frameInfoQueue = new ConcurrentQueue<GameFrameInfo>();
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
		Co_FixedUpdate().MoveNext();

	}
	private IEnumerator<float> Co_FixedUpdate()
	{
		BaseCharacter character;
		GameFrameInfo info;
		while (true)
		{
			while (_frameInfoQueue.TryDequeue(out info) == false)
			{
				//todo
				yield return 0f;
			}
			LogMgr.Log(LogSourceType.Debug, $"----------------move one frame, current tick : [{_currentTick}]-------------------");
			for (int i = 0; i < 6; i++)
			{
				character = _characters[i];
				if (character is null) continue;
				character.HandleInput(info.MoveInput[i], info.LookInput[i]);
				character.HandleOneFrame();
			}
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
	public void EnqueueFrameInfo(in GameFrameInfo info)
	{
		//Debug.Assert(_characters[teamId] is not null);
		LogMgr.Log(LogSourceType.Debug, $"[Tick : {_currentTick}]\nEnqueueing {JsonUtility.ToJson(info)}");
		_frameInfoQueue.Enqueue(info);
	}

}
