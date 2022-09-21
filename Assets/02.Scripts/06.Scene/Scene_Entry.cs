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
		//S_EnterGame packet = new S_EnterGame();
		//packet.PlayerInfoArr = new PlayerInfoDto[6];
		//var str = JsonUtility.ToJson(packet);
		DontDestroyOnLoad(new GameObject("@Log", typeof(LogMgr)));
		DontDestroyOnLoad(new GameObject("@Timing", typeof(Timing)));
		DontDestroyOnLoad(new GameObject("@Network", typeof(Network)));
		DontDestroyOnLoad(new GameObject("@PacketQueue", typeof(PacketQueue)));
		DontDestroyOnLoad(new GameObject("@Scene", typeof(Scene)));
	}
}
