using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
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
	public override void Init(object param)
	{
		var req = param as S_EnterGame;
		Scenetype = SceneType.Game;
		_characters = new BaseCharacter[6];
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
	public void UpdatePlayer(short teamId, Vector2 movePos, Vector2 lookDir)
	{
		var character = _characters[teamId];
		if (character is null) return;
		var pos = new Vector3(movePos.x, character.transform.position.y, movePos.y);
		Debug.Log((character.transform.position - pos).magnitude);
		if ((character.transform.position - pos).magnitude > 1.5)
		{
			character.transform.position = pos;
		}
		character.Look(new Vector3(lookDir.x, 0, lookDir.y));
	}
	public void Exit()
	{

	}
}
