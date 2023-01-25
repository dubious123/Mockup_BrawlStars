using System.Collections.Generic;
using System.Linq;

using Server.Game;

using UnityEngine;

public class CEnvSystem : MonoBehaviour
{
	[SerializeField] private GameObject _envGo;

	private NetEnvSystem _netEnvSystem;
	private Dictionary<NetEnv, CEnv> _envDict;

	public void Init(NetEnvSystem netEnvSystem)
	{
		_netEnvSystem = netEnvSystem;
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

	public void OnRoundReset()
	{
		foreach (var env in _envDict.Values)
		{
			env.Reset();
		}
	}
}
