using Logging;
using MEC;
using System.Threading.Tasks;
using UnityEngine;

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
		DontDestroyOnLoad(new GameObject("@Log", typeof(LogMgr)));
		DontDestroyOnLoad(new GameObject("@Timing", typeof(Timing)));
		DontDestroyOnLoad(new GameObject("@Network", typeof(Network)));
		DontDestroyOnLoad(new GameObject("@PacketQueue", typeof(PacketQueue)));
		DontDestroyOnLoad(new GameObject("@Scene", typeof(Scene)));
	}
}
