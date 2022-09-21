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
		Scenetype = SceneType.Game;
		_characters = new BaseCharacter[6];
		//var handle = _dog.LoadAssetAsync<GameObject>();
		//handle.Completed += _ =>
		//{
		//	Enter(User.TeamId, User.CharType);
		//	Camera.main.GetComponent<GameCameraController>().FollowTarget = _characters[User.TeamId].transform;
		//	IsReady = true;
		//};
		Enter(User.TeamId, User.CharType);
		Camera.main.GetComponent<GameCameraController>().FollowTarget = _characters[User.TeamId].transform;
		IsReady = true;
	}
	public void Enter(short teamId, CharacterType type)
	{
		//var character = Instantiate(_dog.Asset as GameObject, _spawnPoint[teamId].position, Quaternion.identity).GetComponent<BaseCharacter>();
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
		if (_characters[teamId] == null)
		{
			Enter(teamId, CharacterType.Dog);
		}
		if (User.TeamId == teamId) return;
		//if (movePos == Vector2.zero) return;
		//_characters[teamId].Move(new Vector3(movePos.x, 0, movePos.y));
		//if (lookDir == Vector2.zero)
		//{
		//	lookDir = Vector2.right;
		//}
		//_characters[teamId].Look(new Vector3(lookDir.x, 0, lookDir.y));
	}
	public void Move(short teamId, Vector2 movePos)
	{
		if (User.TeamId == teamId)
		{
			return;
		}
		_characters[teamId].Move(new Vector3(movePos.x, 0, movePos.y));
	}
	public void Look(short teamId, Vector2 lookDir)
	{
		if (User.TeamId == teamId)
		{
			return;
		}
		_characters[teamId].Look(new Vector3(lookDir.x, 0, lookDir.y));
	}
	public void Exit()
	{

	}
}
