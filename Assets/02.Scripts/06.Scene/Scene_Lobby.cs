using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using static Enums;

public class Scene_Lobby : BaseScene
{
	private bool _playBtnPressed = false;

	public override void Init(object param)
	{
		Scenetype = SceneType.Lobby;
		Scene.PlaySceneChangeAnim();
		GameInput.BasicAttackInputAction.started += PlayBtnPressedNormal;
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
		GameInput.BasicAttackInputAction.started -= PlayBtnPressedNormal;
		GetComponent<AudioSource>().Stop();
		SceneManager.UnloadSceneAsync((int)Scenetype);
		Scene.MoveTo(SceneType.SearchingPlayers, null, LoadSceneMode.Additive);
	}

	private void PlayBtnPressedNormal(InputAction.CallbackContext _)
	{
		Audio.PlayBtnPressedNormal();
	}
}
