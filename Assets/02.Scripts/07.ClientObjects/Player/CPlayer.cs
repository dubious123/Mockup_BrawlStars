using Server.Game;

using UnityEngine;

public class CPlayer : MonoBehaviour
{
	public NetCharacter NPlayer { get; set; }
	[field: SerializeField] public Animator Animator { get; set; }
	public int Hp { get; set; }

	public virtual void Init(NetCharacter character)
	{
		Hp = 100;
		NPlayer = character;
	}

	public virtual void HandleOneFrame()
	{
		transform.SetPositionAndRotation((Vector3)NPlayer.Position, (Quaternion)NPlayer.Rotation);
	}
}