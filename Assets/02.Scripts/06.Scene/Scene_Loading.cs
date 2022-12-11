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
	[SerializeField] private AudioClip _loadingBgmClip;
	[SerializeField] private AudioClip _brawlIntroClip;
	private bool _loginSuccess;

	public override void Init(object param)
	{
		IsReady = false;
		Scene.PlaySceneChangeAnim();
		Scenetype = Enums.SceneType.Loading;
		switch ((Enums.SceneType)param)
		{
			case Enums.SceneType.Lobby:
				Timing.RunCoroutine(CoLoadLobbyScene().CancelWith(gameObject));
				break;
			case Enums.SceneType.Game:
				Timing.RunCoroutine(CoLoadGameScene().CancelWith(gameObject));
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

		Audio.PlayOnce(_loadingBgmClip);
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

		SceneManager.UnloadSceneAsync((int)Enums.SceneType.Loading);
	}

	private IEnumerator<float> CoLoadGameScene()
	{
		Audio.PlayOnce(_brawlIntroClip);
		var loadHandle = Scene.MoveTo(Enums.SceneType.Game, Enums.CharacterType.Knight, LoadSceneMode.Single);
		loadHandle.allowSceneActivation = false;
		while (loadHandle.progress < 0.9f)
		{
			_progressBar.UpdateProgress((int)(loadHandle.progress * 100));
			yield return 0f;
		}

		_progressBar.UpdateProgress(100);
		yield return Timing.WaitForSeconds(2);

		loadHandle.allowSceneActivation = true;
		yield break;
	}
}
