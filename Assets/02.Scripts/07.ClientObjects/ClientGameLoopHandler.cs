using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server.Game;

using UnityEngine;

public class ClientGameLoopHandler : MonoBehaviour
{
	[field: SerializeField] public CPlayerSystem PlayerSystem { get; private set; }
	[field: SerializeField] public CEnvSystem EnvSystem { get; private set; }

	private NetGameLoopHandler _netGameLoop;
	private NetWorld _netWorld => _netGameLoop.World;

	public void Init(NetGameLoopHandler netGameLoop)
	{
		_netGameLoop = netGameLoop;
		PlayerSystem.Init(_netWorld.CharacterSystem);
		EnvSystem.Init(_netWorld.EnvSystem);
	}

	public void StartGame()
	{
		enabled = true;
	}

	public void MoveClientGameLoop()
	{
		PlayerSystem.MoveClientLoop();
		EnvSystem.MoveClientLoop();
	}
}
