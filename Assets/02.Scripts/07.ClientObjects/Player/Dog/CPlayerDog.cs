
using Server.Game;

using UnityEngine;

public class CPlayerDog : CPlayer
{
	[field: SerializeField] public CDogBasicAttack BasicAttack { get; set; }
	[field: SerializeField] public CDogBash Bash { get; set; }

	public override void Init(NetCharacter character)
	{
		base.Init(character);
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
