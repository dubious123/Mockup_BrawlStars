using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MEC;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Loading : BaseScene
{
	[SerializeField] private ProgressBar _progressBar;
	[SerializeField] private AudioSource _bgm;
	private bool _loginSuccess;

	public override void Init(object param)
	{
		IsReady = false;
		Scenetype = Enums.SceneType.Loading;
		switch ((Enums.SceneType)param)
		{
			case Enums.SceneType.Lobby:
				Timing.RunCoroutine(CoLoadLobbyScene());
				break;
			case Enums.SceneType.Game:
				Timing.RunCoroutine(CoLoadGameScene());
				break;
		}

		IsReady = true;
	}

	public void LoginSuccess()
	{
		_loginSuccess = true;
	}

	public void LoginFailed()
	{
#if UNITY_EDITOR
		Debug.Log("Login Failed");
#elif UNITY_STANDALONE_WIN
		Application.Quit();
#endif
	}

	private IEnumerator<float> CoLoadLobbyScene()
	{
		_progressBar.UpdateProgress(10, "Connecting To Server");
		Network.RegisterSend(new C_Login
		{
#if UNITY_EDITOR
			loginId = "jonghun",
#elif UNITY_STANDALONE_WIN
			loginId = "jonghun" + Application.dataPath.Split('/')[^2].Last(),
#endif
			loginPw = "0827"
		});

		_bgm.Play();
		while (_loginSuccess == false)
		{
			yield return 0;
		}

		var handle = Scene.MoveTo(Enums.SceneType.Lobby, Enums.CharacterType.Knight, LoadSceneMode.Additive);

		while (handle.isDone == false)
		{
			_progressBar.UpdateProgress((int)(handle.progress * 100));
			yield return 0;
		}
	}

	private IEnumerator<float> CoLoadGameScene()
	{
		yield break;
	}

	private IEnumerator CoUpdateProgress()
	{
		for (int i = 10; i < 101; i++)
		{
			_progressBar.UpdateProgress(i);
			yield return new WaitForSeconds(0.1f);
		}
	}
}
