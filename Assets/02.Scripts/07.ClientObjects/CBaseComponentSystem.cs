using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server.Game;

using UnityEngine;

public abstract class CBaseComponentSystem<T> : MonoBehaviour where T : NetBaseComponent
{
	public abstract void Init(NetBaseComponentSystem<T> netSystem);
	public abstract void MoveClientLoop();
	public abstract void Reset();
}
