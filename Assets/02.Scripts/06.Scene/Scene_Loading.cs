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
		var startTime = Time.time;
		Audio.PlayOnce(_loadingBgmClip);
		_progressBar.UpdateProgress(10, "Connecting To Server");
		while (Network.Connected is false)
		{
			yield return Timing.WaitForSeconds(0.1f);
		}

		Network.RegisterSend(new C_Login
		{
#if UNITY_EDITOR
			loginId = Config.Id,
#elif UNITY_STANDALONE_WIN
			loginId = Config.Id + Application.dataPath.Split('/')[^2].Last(),
#endif
			loginPw = Config.Pw
		});

		while (_loginSuccess == false)
		{
			yield return 0;
		}

		var handle = Scene.MoveTo(Enums.SceneType.Lobby, Enums.NetObjectType.Character_Shelly, LoadSceneMode.Additive);
		handle.allowSceneActivation = false;

		while (handle.progress < 0.9f)
		{
			_progressBar.UpdateProgress((int)(handle.progress * 100));
			yield return 0;
		}

		_progressBar.UpdateProgress(100);
		for (; Time.time - startTime < _loadingBgmClip.length;)
		{
			yield return Timing.WaitForOneFrame;
		}

		handle.allowSceneActivation = true;
		SceneManager.UnloadSceneAsync((int)Enums.SceneType.Loading);
	}

	private IEnumerator<float> CoLoadGameScene()
	{
		var startTime = Time.time;
		Audio.PlayAudio(_brawlIntroClip, _brawlIntroClip.name, false);
		Timing.CallDelayed(_brawlIntroClip.length - 1, () => Audio.StopAudio(_brawlIntroClip.name, 1));
		var loadHandle = Scene.MoveTo(Enums.SceneType.Game, Enums.NetObjectType.Character_Shelly, LoadSceneMode.Single);
		loadHandle.allowSceneActivation = false;
		while (loadHandle.progress < 0.9f)
		{
			_progressBar.UpdateProgress((int)(loadHandle.progress * 100));
			yield return 0f;
		}

		_progressBar.UpdateProgress(100);
		for (; Time.time - startTime < _loadingBgmClip.length;)
		{
			yield return Timing.WaitForOneFrame;
		}

		loadHandle.allowSceneActivation = true;
		yield break;
	}
}
