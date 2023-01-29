using MEC;

using UnityEngine;
using UnityEngine.SceneManagement;

using static Enums;

public class Scene_Map1 : BaseScene
{
	public static GameStartInfo StartInfo { get; set; }
	public int CurrentFrameCount => _netGameLoop.FrameNum;
	public NetGameLoopHandler NetGameLoop => _netGameLoop;
	public ClientGameLoopHandler ClientGameLoop => _clientGameLoop;
	[field: SerializeField] public Map00UI UI { get; private set; }

	[SerializeField] private WorldDataHelper _dataHelper;
	[SerializeField] private AudioClip _ingameBgm;
	[SerializeField] private ClientGameLoopHandler _clientGameLoop;

	private readonly NetGameLoopHandler _netGameLoop = new();

	public override void Init(object param)
	{
		Scene.PlaySceneChangeAnim();
		Scenetype = SceneType.Game;
		_netGameLoop.Init(_dataHelper.GetWorldData(), StartInfo);
		_clientGameLoop.Init(_netGameLoop);
		IsReady = true;
		UI.PlayWelcomeAnim(StartGame);
	}

	public void StartGame()
	{
		Audio.PlayAudio(_ingameBgm, _ingameBgm.name, true);
		_netGameLoop.StartGame();
	}

	public void EndGame()
	{
		JobMgr.PushUnityJob(() =>
		{
			Timing.CallDelayed(2, () =>
			{
				Scene.MoveTo(SceneType.Lobby, User.CharType, LoadSceneMode.Single);
			});
		});
	}
}
