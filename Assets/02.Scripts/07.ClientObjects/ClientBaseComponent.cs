using Server.Game;

using UnityEngine;

public abstract class ClientBaseComponent<T> : MonoBehaviour where T : NetBaseComponent
{
	public bool Active { get; set; }

	public abstract void Init(T netComponent);

	public abstract void OnNetFrameUpdate();

	public abstract void Interpretate(float ratio);

	protected abstract class SnapShot
	{
		public abstract void TakePicture(T netComponent);
	}
}
