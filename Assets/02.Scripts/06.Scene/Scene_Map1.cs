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
		if (User.TeamId == teamId)
		{
			character.gameObject.AddComponent<PlayerInput>().actions = _inputAsset;
			character.gameObject.AddComponent<DogController>();
		}

	}
	public void UpdatePlayer(short teamId, Vector2 movePos, Vector2 lookDir)
	{
		if (_characters[teamId] == null)
		{
			Enter(teamId, CharacterType.Dog);
		}
		if (User.TeamId == teamId) return;
		_characters[teamId].Move(new Vector3(movePos.x, 0, movePos.y));
		_characters[teamId].Look(new Vector3(lookDir.x, 0, lookDir.y));
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
