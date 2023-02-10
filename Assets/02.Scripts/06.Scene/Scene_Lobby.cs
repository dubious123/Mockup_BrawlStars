using System;
using System.Collections.Generic;

using MEC;

using UnityEngine;
using UnityEngine.SceneManagement;

using static Enums;

public class Scene_Lobby : BaseScene
{
	[SerializeField] private GameObject[] _characters;
	[SerializeField] private Transform _characterCamTrans;

	private bool _playBtnPressed = false;
	private int _camIndex;
	private CoroutineHandle _coHandle;
	private readonly Vector3 _camOffset = new(0, 460, -975);

	public override void Init(object param)
	{
		Scenetype = SceneType.Lobby;
		Scene.PlaySceneChangeAnim();
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
		Timing.KillCoroutines(_coHandle);
		SceneManager.UnloadSceneAsync((int)Scenetype);
		Scene.MoveTo(SceneType.SearchingPlayers, null, LoadSceneMode.Additive);
	}

	public void SelectCharacterL(bool isLeft)
	{
		var temp = isLeft ? --User.CharType : ++User.CharType;
		User.CharType = (NetObjectType)Math.Clamp((int)temp, (int)NetObjectType.Character_Shelly, (int)NetObjectType.Character_Spike);
		var index = User.CharType - NetObjectType.Character_Shelly;
		if (_camIndex != index)
		{
			Timing.KillCoroutines(_coHandle);
			_coHandle = Timing.RunCoroutine(Co_MoveCam(index));
			_camIndex = index;
		}
	}

	private IEnumerator<float> Co_MoveCam(int targetIndex)
	{
		var currentPos = _characterCamTrans.position;
		var targetPos = _characters[targetIndex].transform.position + _camOffset;
		for (float i = 0; i < 1f; i += Time.deltaTime)
		{
			_characterCamTrans.position = Vector3.Lerp(currentPos, targetPos, i);
			yield return 0f;
		}

		_characterCamTrans.position = targetPos;
		yield break;
	}
}
