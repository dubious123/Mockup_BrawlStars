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
	PriorityQueue<InputInfo, long>[] _inputQueues;
	public long CurrentTick => _currentTick;
	long _currentTick = 0;
	private void FixedUpdate()
	{
		Co_FixedUpdate().MoveNext();
	}
	private IEnumerator<float> Co_FixedUpdate()
	{
		BaseCharacter character;
		PriorityQueue<InputInfo, long> inputQueue;
		while (true)
		{
			_currentTick++;
			for (int i = 0; i < 6; i++)
			{
				character = _characters[i];
				inputQueue = _inputQueues[i];
				if (character is null) continue;
				while (inputQueue.Count <= 0)
				{
					yield return 0f;
				}
				while (inputQueue.Peek().TargetTick > _currentTick)
				{
					character.StopAll();
					yield return 0f;
				}
				character.AwakeAll();
				character.HandleInput(inputQueue.Dequeue());
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
	public override void Init(object param)
	{
		var req = param as S_EnterGame;
		Scenetype = SceneType.Game;
		_characters = new BaseCharacter[6];
		_inputQueues = new PriorityQueue<InputInfo, long>[6];
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
	}
	public void Enter(short teamId, CharacterType type)
	{
		Debug.Assert(_characters[teamId] is null);
		var character = Instantiate(_dog, _spawnPoint[teamId].position, Quaternion.identity).GetComponent<BaseCharacter>();
		_characters[teamId] = character;
		_inputQueues[teamId] = new();
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
		var queue = _inputQueues[teamId];
		if (queue is null) return;
		_inputQueues[teamId].Enqueue(info, info.TargetTick);
	}

}
