

using MEC;

using UnityEngine;
using UnityEngine.SceneManagement;

using static Enums;

public class Scene_Lobby : BaseScene
{
	private bool _playBtnPressed = false;

	public override void Init(object param)
	{
		Scenetype = SceneType.Lobby;
		Scene.PlaySceneChangeAnim();
		DontDestroyOnLoad(new GameObject("@User", typeof(User)));
		User.CharType = (NetObjectType)param;
		User.CharType = NetObjectType.Character_Shelly;
		IsReady = true;
		GetComponent<AudioSource>().PlayDelayed(1f);
	}

	public void EnterGame()
	{
		if (_playBtnPressed)
		{
			Debug.Log("Btn already pressed");
			return;
		}

		_playBtnPressed = true;
		Network.RegisterSend(new C_EnterGame() { CharacterType = (int)Enums.NetObjectType.Character_Shelly, UserId = User.UserId });
		SceneManager.UnloadSceneAsync((int)Scenetype);
		Scene.MoveTo(SceneType.SearchingPlayers, 1, LoadSceneMode.Additive);
	}
}
