using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_SearchingPlayers : BaseScene
{
	[SerializeField] private SearchingPlayersUI _ui;

	public override void Init(object param)
	{
		Scenetype = Enums.SceneType.SearchingPlayers;
		var targetPlayerCount = (int)param;
		_ui.Init(targetPlayerCount);
		GetComponent<AudioSource>().Play();
		Network.RegisterSend(new C_EnterGame() { CharacterType = (int)Enums.NetObjectType.Character_Shelly, UserId = User.UserId });
		IsReady = true;
	}

	public void UpdatePlayerFound(ushort foundPlayersCount) => _ui.UpdateText(foundPlayersCount);

	public void OnGameReady()
	{
		Scene.MoveTo(Enums.SceneType.Loading, Enums.SceneType.Game, LoadSceneMode.Additive);
	}
}
