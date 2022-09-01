using ServerCore.Packets;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static Enums;

public class Scene_Lobby : BaseScene
{
	[SerializeField] TMP_Dropdown _dropDwon;
	public override async Task A_Init(object param)
	{
		await Task.Run(() =>
		{
			DontDestroyOnLoad(new GameObject("@User", typeof(User)));
			User.CharType = (CharacterType)param;
		});
	}
	public void SelectCharacterType(int value)
	{
		User.CharType = (CharacterType)value;
	}
	public void EnterGame()
	{
		Network.RegisterSend(new C_EnterGame { CharacterType = (ushort)User.CharType });
	}

}
