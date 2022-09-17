using ServerCore.Packets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enums;

public class Scene_Lobby : BaseScene
{
	[SerializeField] TMP_Dropdown _dropDwon;
	public override void Init(object param)
	{
		Scenetype = SceneType.Lobby;
		DontDestroyOnLoad(new GameObject("@User", typeof(User)));
		User.CharType = (CharacterType)param;
		IsReady = true;
	}
	public void SelectCharacterType(int value)
	{
		User.CharType = (CharacterType)value;
	}
	public void EnterGame()
	{
		Network.RegisterSend(new C_EnterGame { CharacterType = (ushort)User.CharType, UserId = User.UserId });
	}

}
