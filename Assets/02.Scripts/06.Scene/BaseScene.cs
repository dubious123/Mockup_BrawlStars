using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

using static Enums;
public abstract class BaseScene : MonoBehaviour
{
	public SceneType Scenetype { get; set; }
	public bool IsReady { get; protected set; } = false;
	public abstract void Init(object param);
}
