using System.Collections.Generic;
using System.Linq;

using Server.Game;

using UnityEngine;

public class CEnvSystem : CBaseComponentSystem<NetEnv>
{
	[SerializeField] private GameObject _envGo;

	private NetEnvSystem _netEnvSystem;
	private Dictionary<NetEnv, CEnv> _envDict;

	public override void Init(NetBaseComponentSystem<NetEnv> netEnvSystem)
	{
		_netEnvSystem = netEnvSystem as NetEnvSystem;
		var cEnvDict = _envGo.GetComponentsInChildren<CEnv>(true).ToDictionary(env => env.transform.position);
#if UNITY_EDITOR
		if (cEnvDict.Count != _netEnvSystem.ComponentDict.Count)
		{
			throw new System.Exception();
		}
#endif
		_envDict = new(_netEnvSystem.ComponentDict.Count);
		foreach (var netEnv in _netEnvSystem.ComponentDict)
		{
			var cEnv = cEnvDict[(Vector3)netEnv.Position];
			_envDict.Add(netEnv, cEnv);
			cEnv.Init(netEnv);
		}

		_netEnvSystem.OnCharEnterTree = (netEnv, netChar) => JobMgr.PushUnityJob(() => (_envDict[netEnv] as CTree).OnCharacterEnter(netChar));
		_netEnvSystem.OnCharExitTree = (netEnv, netChar) => JobMgr.PushUnityJob(() => (_envDict[netEnv] as CTree).OnCharacterExit(netChar));
	}

	public override void OnNetFrameUpdate()
	{
	}

	public override void Interpretate(float _)
	{
	}

	public override void OnRoundStart()
	{
	}

	public override void OnRoundEnd()
	{
	}

	public override void Clear()
	{
	}

	public override void Reset()
	{
		foreach (var env in _envDict.Values)
		{
			env.Reset();
		}
	}
}
