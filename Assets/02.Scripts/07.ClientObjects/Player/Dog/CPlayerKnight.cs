
using Server.Game;

using UnityEngine;

public class CPlayerKnight : CPlayer
{
	[field: SerializeField] public CKnightBasicAttack BasicAttack { get; set; }
	[field: SerializeField] public CKnightBash Bash { get; set; }

	public override void Init(NetCharacter character, short teamId)
	{
		base.Init(character, teamId);
		BasicAttack.Init(this);
		Bash.Init(this);
	}

	public override void HandleOneFrame()
	{
		base.HandleOneFrame();
		Animator.SetFloat(AnimatorMeta.Speed_Float, (float)NPlayer.MoveSpeed * (float)NPlayer.TargetMoveDir.magnitude);
		BasicAttack.HandleOneFrame();
		Bash.HandleOneFrame();
	}
}
