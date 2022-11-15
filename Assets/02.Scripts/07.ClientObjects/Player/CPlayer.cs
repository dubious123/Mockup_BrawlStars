using Server.Game;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CPlayer : MonoBehaviour
{
	public NetCharacter NPlayer { get; set; }
	public short TeamId { get; set; }
	[field: SerializeField] public Animator Animator { get; set; }
	[field: SerializeField] public Image StunIndicator { get; set; }
	public int MaxHp => NPlayer.MaxHp;
	public int Hp => NPlayer.Hp;
	public sVector3 Lookdir => NPlayer.TargetLookDir;
	public sVector3 MoveDir => NPlayer.TargetMoveDir;

	public virtual void Init(NetCharacter character, short teamId)
	{
		NPlayer = character;
		TeamId = teamId;
	}

	public virtual void HandleOneFrame()
	{
		if (NPlayer.IsDead())
		{
			Animator.SetBool(AnimatorMeta.IsDead_Bool, true);
			return;
		}

		if (NPlayer.CCFlag.HasFlag(Enums.CCFlags.Stun))
		{
			Animator.SetBool(AnimatorMeta.IsStun_Bool, true);
		}

		transform.SetPositionAndRotation((Vector3)NPlayer.Position, (Quaternion)NPlayer.Rotation);
	}
}