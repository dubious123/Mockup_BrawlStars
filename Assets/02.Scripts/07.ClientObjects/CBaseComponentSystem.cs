using Server.Game;

using UnityEngine;

public abstract class CBaseComponentSystem<T> : MonoBehaviour, IClientComponentSystem where T : NetBaseComponent
{
	public abstract void Init(NetBaseComponentSystem<T> netSystem);
	public abstract void OnNetFrameUpdate();
	public abstract void Interpretate(float t);
	public abstract void Reset();
	public abstract void Clear();
	public abstract void OnRoundStart();
	public abstract void OnRoundEnd();
}
