using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;

using static Enums;

public class Lobby : MonoBehaviour
{
	[SerializeField] private TMP_Dropdown _dropDwon;
	public void EnterGame()
	{
		SceneManager.LoadScene(sceneBuildIndex: (int)SceneType.Game);
	}

	public void SelectCharacterType(int value)
	{
		User.CharType = (NetObjectType)value;
	}
}
