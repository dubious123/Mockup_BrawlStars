using System;
using System.Linq;
using System.Threading.Tasks;

using Logging;

using MEC;

using UnityEngine;

using static GameFrameInfo;
using static S_EnterGame;

public class Scene_Entry : BaseScene
{

	public override void Init(object param)
	{
		Scenetype = Enums.SceneType.Entry;
		IsReady = true;
	}

	[Serializable]
	public struct test1
	{
		public test2 t;
		public test1(uint d)
		{
			t = test2.fromRaw(d);
		}
	}

	[Serializable]
	public struct test2
	{
		public readonly uint a;
		private test2(uint d)
		{
			a = d;
		}

		public static test2 fromRaw(uint d)
		{
			return new test2(d);
		}
	}

	private void Awake()
	{
		Screen.SetResolution(1920, 1080, false);
		Application.targetFrameRate = 300;
		Time.fixedDeltaTime = (float)((sfloat)1 / (sfloat)60f);
		DontDestroyOnLoad(new GameObject("@JobMgr", typeof(JobMgr)));
		DontDestroyOnLoad(new GameObject("@Log", typeof(LogMgr)));
		DontDestroyOnLoad(new GameObject("@Timing", typeof(Timing)));
		DontDestroyOnLoad(new GameObject("@Network", typeof(Network)));
		DontDestroyOnLoad(new GameObject("@Scene", typeof(Scene)));
	}
}
