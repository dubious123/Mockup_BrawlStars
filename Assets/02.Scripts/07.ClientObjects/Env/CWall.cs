using System.Collections;
using System.Collections.Generic;

using Server.Game;

using UnityEngine;

public class CWall : CEnv
{
	private NetWall _netWall;

	public override void Init(NetEnv netEnv)
	{
		_netWall = netEnv as NetWall;
		if (_netWall == null)
		{
			throw new System.Exception();
		}
	}
}
