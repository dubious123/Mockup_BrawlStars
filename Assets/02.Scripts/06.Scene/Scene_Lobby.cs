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
		DontDestroyOnLoad(new GameObject("@User", typeof(User)));
		User.CharType = (CharacterType)param;
	}
	public void SelectCharacterType(int value)
	{
		User.CharType = (CharacterType)value;
	}
	public void EnterGame()
	{
		Scene.MoveTo(SceneType.Game, User.CharType);
	}

}
