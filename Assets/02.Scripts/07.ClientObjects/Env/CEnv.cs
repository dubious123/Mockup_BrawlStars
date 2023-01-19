using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server.Game;

using UnityEngine;

public abstract class CEnv : MonoBehaviour
{
	public abstract void Init(NetEnv netEnv);
}
