using UnityEngine;
using UnityEngine.UI;

using static Enums;

public class CharacterSelectBtn : MonoBehaviour
{
	[SerializeField] private Image _characterProfileImage;

	private NetObjectType _charType;
	private LobbyUI _ui;

	public void Init(NetObjectType type)
	{
		_characterProfileImage.sprite = Data.GetCharacterProfile(type);
		_ui = GameObject.Find("Lobby_UI").GetComponent<LobbyUI>();
		_charType = type;
	}

	public void OnSelected()
	{
		User.CharType = _charType;
		_ui.MoveToLobbyUI();
	}
}
