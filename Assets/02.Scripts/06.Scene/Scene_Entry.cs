using Logging;
using MEC;
using System.Threading.Tasks;
using UnityEngine;
using static S_EnterGame;

public class Scene_Entry : BaseScene
{
	public override void Init(object param)
	{
		Scenetype = Enums.SceneType.Entry;
		IsReady = true;
	}
	private void Awake()
	{
		Screen.SetResolution(1920, 1080, false);
		Application.targetFrameRate = 300;
		//Debug.Log(1 / 60);
		Time.fixedDeltaTime = 1 / 60f;
		DontDestroyOnLoad(new GameObject("@JobMgr", typeof(JobMgr)));
		DontDestroyOnLoad(new GameObject("@Log", typeof(LogMgr)));
		DontDestroyOnLoad(new GameObject("@Timing", typeof(Timing)));
		DontDestroyOnLoad(new GameObject("@Network", typeof(Network)));
		DontDestroyOnLoad(new GameObject("@Scene", typeof(Scene)));
	}
}
