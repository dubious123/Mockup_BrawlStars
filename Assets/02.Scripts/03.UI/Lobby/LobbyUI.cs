using System;

using UnityEngine;

using static Enums;

public class LobbyUI : MonoBehaviour
{
	[SerializeField] private GameObject _lobbyUI;
	[SerializeField] private GameObject _characterSelectUI;
	[SerializeField] private Transform[] _characters;
	[SerializeField] private Transform _characterCamTrans;
	[SerializeField] private Transform _characterSelectContentTrans;
	[SerializeField] private GameObject _characterSelectProfilePrefab;

	private void Start()
	{
		for (var i = NetObjectType.Character_Shelly; i <= NetObjectType.Character_Spike; ++i)
		{
			var profile = Instantiate(_characterSelectProfilePrefab, _characterSelectContentTrans).GetComponent<CharacterSelectBtn>();
			profile.Init(i);
		}

		_characterSelectContentTrans.GetComponent<UIRuntimeCellSizeVerticle>().Init();
		MoveToLobbyUI();
	}

	public void MoveToCharacterSelectUI()
	{
		_lobbyUI.SetActive(false);
		_characterSelectUI.SetActive(true);
	}

	public void MoveToLobbyUI()
	{
		var index = Math.Clamp((int)User.CharType, (int)NetObjectType.Character_Shelly, (int)NetObjectType.Character_Spike) - (int)NetObjectType.Character_Shelly;
		_characterCamTrans.position = new Vector3(_characters[index].position.x, _characterCamTrans.position.y, _characterCamTrans.position.z);
		_characterSelectUI.SetActive(false);
		_lobbyUI.SetActive(true);
	}
}
