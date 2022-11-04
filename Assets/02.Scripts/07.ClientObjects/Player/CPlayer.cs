using Server.Game;

using UnityEngine;

public class CPlayer : MonoBehaviour
{
	public NetCharacter NPlayer { get; set; }
	[field: SerializeField] public Animator Animator { get; set; }
	public int MaxHp => NPlayer.MaxHp;
	public int Hp => NPlayer.Hp;

	public virtual void Init(NetCharacter character)
	{
		NPlayer = character;
	}

	public virtual void HandleOneFrame()
	{
		if (NPlayer.IsDead())
		{
			Animator.SetBool(AnimatorMeta.IsDead_Bool, true);
			return;
		}

		transform.SetPositionAndRotation((Vector3)NPlayer.Position, (Quaternion)NPlayer.Rotation);
	}
}