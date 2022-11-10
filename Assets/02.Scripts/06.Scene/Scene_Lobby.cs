

using UnityEngine;

using static Enums;

public class Scene_Lobby : BaseScene
{
	public override void Init(object param)
	{
		Scenetype = SceneType.Lobby;
		DontDestroyOnLoad(new GameObject("@User", typeof(User)));
		User.CharType = (CharacterType)param;
		User.CharType = CharacterType.Knight;
		IsReady = true;
	}

	public void EnterGame()
	{
		Network.RegisterSend(new C_EnterGame { CharacterType = (ushort)User.CharType, UserId = User.UserId });
	}

}
