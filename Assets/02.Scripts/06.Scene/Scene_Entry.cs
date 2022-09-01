using MEC;
using System.Threading.Tasks;
using UnityEngine;

public class Scene_Entry : BaseScene
{
	public override Task A_Init(object param)
	{
		return null;
	}
	private void Awake()
	{
		DontDestroyOnLoad(new GameObject("@Timing", typeof(Timing)));
		DontDestroyOnLoad(new GameObject("@Network", typeof(Network)));
		DontDestroyOnLoad(new GameObject("@PacketQueue", typeof(PacketQueue)));
		DontDestroyOnLoad(new GameObject("@Scene", typeof(Scene)));
	}
}
